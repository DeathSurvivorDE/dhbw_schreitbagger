import serial
import time
from RPi import GPIO
import pygame
from threading import *
import logging
import os
import datetime
from Tkinter import *




def horn():
    print("horn")
    '''
    Funktion zum schalten des Relais für die Hupe des Baggers
    '''
    GPIO.output (11, False)
    time.sleep(1)
    GPIO.output (11, True)

def light():
    print("light")
    """
    Funktion zum schalten des Relais zur Bestromung des Scheinwerfersystems am Bagger
    """
    GPIO.output(13, not GPIO.input(13))
    

def taillight():
    print("taillight")
    """
    Funktion zum schalten des Relais zur Bestromung des Rücklichtsystems am Bagger
    """
    GPIO.output(19, not(GPIO.input(19)))
    
def FU():
    print("FU")
    """
    Funktion zum Freischalten der Leistungsschaltung des Frequenzumrichters
    """
    GPIO.output(15, not(GPIO.input(15)))

def FU_reset():
    print("FU_reset")
    """
    Funktion zum restten des Frequenzumrichters
    """
    GPIO.output(15, False)
    time.sleep(2)
    GPIO.output(15, True)

def pump():
    print("pump")
    """
    Funktion zum Starten der Entwässerungspumpe
    """
    GPIO.output(23, not(GPIO.input(23)))
    
def change_controler():
        print("change_controler")
        '''
        Funktion zum wechseln des Eingabegeräts/Controlers  #Messagebox noch einbauen
        '''
        global controler_state
        controler_state = not controler_state
        showinfo('Controler Change', ' Sie haben %s  als Controler ausgewählt' % (controler_list[controler_state]))
        logging.info('Controler wurde zu %s geändert' % (controler_list[controler_state]))



def find_controler():   #Zuweisung der Controler
    print("find_controler")
    for i in range(pygame.joystick.get_count()):
            if 'Gamepad' in pygame.joystick.Joystick(i).get_name():
                    gamepad = pygame.joystick.Joystick(i)
                    gamepad.init()
            elif 'Thrustmaster' in pygame.joystick.Joystick(i).get_name():
                    JS_1 = pygame.joystick.Joystick(i)
                    JS_1.init()
            else:
                    JS_2 = pygame.joystick.Joystick(i)
                    JS_2.init()


    logging.info('Controler Initialized')



def controler_read():   #es werden die Achspositionen der Joysticks und des Gamepads ausgelesen

        print("controler_read")
        '''
        Funktion zum Lesen der Axpositionen der Controler 
        
        '''       
        while 1:
                
                global controler_state
                global controler_pos
                global controler_pos_old
                global controler_pos_new
                global speed
                global Travel_max
                global DeadBand
                pygame.event.pump()
                lock.acquire()  
                controler_state=False
                controler_pos_old_high=[0,0,0,0]
                controler_pos_old_low=[0,0,0,0]
                
                if controler_state:
                	for i in range (4):                             
                		 controler_pos[i] = gamepad.get_axis(i)
                		 logging.info("Pad %f" %gamepad.get_axis(i))
                		               		 
                else:
                        for i in range (2):                           
                                controler_pos_new[i] = (JS_1.get_axis(i)*step_width)
                                
                                #controler_pos[i+2] = JS_2.get_axis(i*step_width)                                                                   
                                                                                                                                                                                                                            
                for k in range(valve_amount):
                    controler_pos_old_high[i]=((controler_pos_old[i])+tolerance)
                    controler_pos_old_low[i]=((controler_pos_old[i])-tolerance)
                
                for i in range(valve_amount):
                    
                    if ((controler_pos_new[i] < controler_pos_old_low[i])or(controler_pos_new[i] > controler_pos_old_high[i])):                       
                        controler_pos_old[i] = controler_pos_new[i] 
                        controler_pos[i] = controler_pos_new[i]
                        
                lock.release()
                time.sleep(BitTime)








def doSteps(TransferByte1):
        
        '''
        Funktion zur Befehlsgabe eines Schrittes der Ventile
        '''
        logging.info("doSteps started")
        global valve_pos
                
        #übergeben eines Datenbytes               
        logging.info(TransferByte1)
        ser.flush()
        ser.write(TransferByte1)
        ser.flush()
    
        #Abfrage ob Arduino prozedur fertig
        a=0
        
        merker = False
        while (merker == False):
                if(ser.inWaiting() >0):
                    ser.flush()
                    a = ser.read()
                    ser.flush()                  
                    logging.info("daten vom ardu %d" %ord(a))                    
                    print(ord(a))
                        

                    if (ord(a)==255):
                        ser.flush()
                        ser.write('$111111111111@')
                        ser.flush()
                        print("nomml null")
                                
                    if(ord(a)==3):
                        merker = True
                        print(merker)
                        
        ser.reset_input_buffer()       
        logging.info("doSteps Ende")






def setAllZero():   #am Anfang ausführen für alle

        print("setAllZero")
                                   
        '''                         
        Funktion zumsetzen der Ventile auf ihre Nullposition
        '''
        logging.info("ValveSetZero started")
        global toZeroString
        
        log.info("starting to set Valves to Zero-Position" )

        doSteps(toZeroString)       #Signal für Nullstelle anfahren '$111111111111@'
        

        for valve_num in range(valve_amount):
        
                valve_pos[valve_num]=0
       
        logging.info("Nullstellen fertig")




     

def valveSetPos():
    
        print("valveSetPos")
        global valve_pos
        global Controler_pos
        global valve_amount
        global TransferSignal
        global TransferListe
        TransferListe=[0,0,0,0,0,0,0,0,0,0,0,0]
        
        for k in range(500):
                
                for i in range(valve_amount):
                        pygame.event.pump()
                        lock.acquire()                #durch Zwischenspeicherung wird der Wert festgehalten
                        con_pos_temp = controler_pos[i]
                        lock.release()
                        diff =0

                        #Differenz zwischen Controler und Valve ermitteln
                        diff = valve_pos[i]-con_pos_temp
                        
                        #Wenn Differenz positiv-> valve_pos verringern
                        if (diff > 0):                                
                                diff = int(diff + 0.5)  #runden                              
                                if diff>9:
                                    diff =9
                                direction=0     
                                build_Transfer(i+1,direction,diff)
                                
                                valve_pos[i]=valve_pos[i]-diff  
                                
                        #Wenn Differenz negativ -> valve_pos inkrementieren                                
                        elif (diff < 0):                               
                                diff = int((diff - 0.5)*(-1))  #runden
                                if diff>9:
                                    diff =9
                                direction = 1
                                build_Transfer(i,direction,diff)
                                
                                valve_pos[i] = valve_pos[i]+diff                                
                                
                #TransferListe zu TransferSignal konvertieren nachdem Transferliste 4 mal aktualisiert wurde
                convertListToString()                
                #Transferbyte zum Arduino schicken
                doSteps(TransferSignal)                
                #Transferbyte rücksetzen für neuen Schreibvorgang
                TransferListe=[0,0,0,0,0,0,0,0,0,0,0,0]
                

                
                        


                                               
         


def build_Transfer(num,direction,diff):           
        global TransferListe
        global valve_amount
        Liste=[0,0,0,0,0,0,0,0,0,0,0,0]
        # [Ventilnr,Richtung,Schritte]
        
        #Ventil 1
        if (num==1):
                if (direction==1):
                        Liste[0]=1
                        Liste[1]=1
                        Liste[2]=diff
                if (direction==0):
                        Liste[0]=1
                        Liste[1]=0
                        Liste[2]=diff                        
        #Ventil 2
        if (num==2):
                if (direction==1):
                        Liste[3]=1
                        Liste[4]=1
                        Liste[5]=diff
                if (direction==0):
                        Liste[3]=1
                        Liste[4]=0
                        Liste[5]=diff                        
        #Ventil 3
        if (num==3):
                if (direction==1):
                        Liste[6]=1
                        Liste[7]=1
                        Liste[8]=diff
                if (direction==0):
                        Liste[6]=1
                        Liste[7]=0
                        Liste[8]=diff                        
        #Ventil 4
        if (num==4):
                if (direction==1):
                        Liste[9]=1
                        Liste[10]=1
                        Liste[11]=diff
                if (direction==0):
                        Liste[9]=1
                        Liste[10]=0
                        Liste[11]=diff

        for j in range(valve_amount*3):
                TransferListe[j]=TransferListe[j] | Liste[j]    #bitweise Verundung da vier Durchgänge
               ### VORSICHT FEHLER HÖHERE ZAHL GEWINNT IMMER

        
                
                
def convertListToString():
    
    
        #aus der Transferliste wird ein String gebildet
        global TransferSignal
        global TransferListe
        
        TransferSignal='$'
        for k in range(valve_amount*3):
                TransferSignal=TransferSignal+str(TransferListe[k])

        TransferSignal=TransferSignal+'@'
        
        
                
                
        
        
                                
        
                                               
                

sdir = "/home/pi/Bagger_Software"

if not os.path.exists(str(sdir + "/log")) :
    os.makedirs(str(sdir + "/log"))   #anlegen Loggverzeichnis
    

dt = datetime.datetime.now()          #Auslesen der Aktuelllen Systemzeit

d = str(dt.strftime('%d-%m-%Y_%H-%M'))

loggfile = str(sdir + "/log/bagger_GUI-log_" + d)                    #Erstellung Logg-file-Adresse
logging.basicConfig(filename = loggfile, format = '%(asctime)s : %(levelname)s : %(message)s', level = logging.INFO)   #initialiesierung der Log funktion
log = logging.getLogger()


logging.info('Start des Moduls Bagger_Controll_Modul')





lock = Lock()
controler_list= {True: "Gamepad", False: "Joystik"}
controler_state = True

valve_amount = 4
ser=serial.Serial('/dev/ttyACM0',9600)
GPIO_List =[11,13,15,19,21,23]

controler_pos = [0,0,0,0]
controler_pos_old = [0,0,0,0]
controler_pos_new = [0,0,0,0]
valve_pos = [0,0,0,0]
valve_n = 0
step_width = 100
tolerance = 5
toZeroString='$111111111111@'
signalLength=12

Travel_max = (31600/316)                                            ##############
speed = (31600/316)                                                 ##############
acceleration = 100000
BitTime = 1/acceleration
logging.info("bittime: %f" %BitTime)
DeadBand = 0.05
portal_open=True
TransferSignal='$000000000000@'
TransferListe=[0,0,0,0,0,0,0,0,0,0,0,0]

GPIO.setwarnings(False)
GPIO.setmode(GPIO.BOARD)                           #Initialisierung und Configuration der GPIO Einheit

#Initialisierung der einzelnen genutzten GPIO ports
GPIO.setup(GPIO_List, GPIO.OUT)   

GPIO.output(GPIO_List, True)
logging.info('RaspberryPi GPIO-Ports Initialized')

pygame.init()					#initialize pygame
pygame.joystick.init()				#initialize joysticks


for i in range(pygame.joystick.get_count()):
        if 'Gamepad' in pygame.joystick.Joystick(i).get_name():
                gamepad = pygame.joystick.Joystick(i)
                gamepad.init()
        elif 'Thrustmaster' in pygame.joystick.Joystick(i).get_name():
                JS_1 = pygame.joystick.Joystick(i)
                JS_1.init()
        else:
                JS_2 = pygame.joystick.Joystick(i)
                JS_2.init()


logging.info('Controler Initialized')


setAllZero()

# Thread in dem die Controller kontinuierlich ausgelesen werden
t1=Thread(target = controler_read)
t1.start()

logging.info("started controler_read - thread")



#--------------------------Mainloop-------------------
print("reached mainloop")
FU()
valveSetPos()


        

        



                
        
