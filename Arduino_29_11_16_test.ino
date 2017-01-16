// Arduino Programm zum Stellen der Ventile

#include </usr/share/arduino/hardware/tools/avr/lib/avr/include/stdlib.h>    // Auf Pi richtigen Pfad hinzufügen

#define BitTime (1/100000)
#define SENTENCE_MAX 14
#define WAITING 0
#define COLLECTING 1
#define DONE 2
#define WAIT 10
#define STEPS 100     // Schritte vom Nullpunkt zum Maximum oder Minimum
#define VALVE_STEPS 31600/STEPS
#define VALVE_MAX 4

int state = WAITING;
int sentence_length;
int valve_num;
int check;
int i;                // Hilfsvariable
int x;                // Hilfsvariable
char sentence[SENTENCE_MAX];
char valve1[3];
char valve2[3];
char valve3[3]; 
char valve4[3];

// Initialisiert I/O-Ports
void setup()
{
  state = WAITING;
  sentence_length = 0;

  pinMode(1,OUTPUT);  // UART
  pinMode(2,OUTPUT);
  pinMode(3,OUTPUT);
  pinMode(4,OUTPUT);
  pinMode(5,OUTPUT);
  pinMode(6,OUTPUT);
  pinMode(7,OUTPUT);
  pinMode(8,OUTPUT);
  pinMode(9,OUTPUT);
  
  pinMode(13,OUTPUT);

  Serial.begin(9600);
}

void blink(int x) {
  for(int i=0; i<x; i++) {
    digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
    delay(500);               // wait for a second
    digitalWrite(13, LOW);    // turn the LED off by making the voltage LOW
    delay(500);               // wait for a second
  }
}

// Führt Schrittbefehl aus
void doSteps(char command[], int valve_num)
{
  //long order = atol(&command[2]);
  int order = command[2];
  // Letzte Zahl aus int filtern
  //if (order < 110) {
  //  order = order - 100;
  //}
  //else {
  //  order = order - 110;
  //}
  
  if (command[0] == 1) {
    if (command[1] == 1) {
      digitalWrite(valve_num*2+1, HIGH);
      for (i=0; i<order; i++) {
        for (x=0; x<VALVE_STEPS; x++) {
          digitalWrite(valve_num*2, HIGH);
          delayMicroseconds(WAIT);
          digitalWrite(valve_num*2, LOW);
          delayMicroseconds(WAIT);          
        }       
      }
    }
    else {
      digitalWrite(valve_num*2+1, LOW);
      for (x=0; x<VALVE_STEPS; x++) {
          digitalWrite(valve_num*2, HIGH);
          delayMicroseconds(WAIT);
          digitalWrite(valve_num*2, LOW);
          delayMicroseconds(WAIT);          
      }  
    }
  }
}

// Wertet das Eingehende Signal vom Pi aus und führt die notwendigen Schritte durch
void read_signal(char command[])
{
  
  for (i=0; i<3; i++){
    valve1[i] = command[i];
    valve2[i] = command[i+3];
    valve3[i] = command[i+6];
    valve4[i] = command[i+9];
  }

  doSteps(valve1,1);
  doSteps(valve2,2);
  doSteps(valve3,3);
  doSteps(valve4,4);
  
  Serial.write(3);    //Signal dass Schritt durchgeführt und nächstes Datenpaket geschickt werden kann
}

void setZero(int valve_num)
{
  // Ventile ganz hoch fahren
  digitalWrite(valve_num*2+1, HIGH);
  for (i=0; i<(2*STEPS); i++) {    
    for (x=0; x<VALVE_STEPS; x++) {
          digitalWrite(valve_num*2, HIGH);
          delayMicroseconds(WAIT);
          digitalWrite(valve_num*2, LOW);
          delayMicroseconds(WAIT);          
    }  
  }

  // Ventile in Mittelposition stellen
  digitalWrite(valve_num*2+1, LOW);
  for (i=0; i<STEPS; i++) {
    for (x=0; x<VALVE_STEPS; x++) {
          digitalWrite(valve_num*2, HIGH);
          delayMicroseconds(WAIT);
          digitalWrite(valve_num*2, LOW);
          delayMicroseconds(WAIT);        
    }  
  }
}

// Überprüft, ob das Signal zur Startposition gegeben wurde
int check_zero(char command[])
{  
  for(i=0; i<SENTENCE_MAX-2; i++) {    
    if(command[i] != 1 ) { 
      return 1;
    }
  }
  for(i=1; i<VALVE_MAX+1; i++) {
    setZero(i);
  }  
  Serial.write(3);    //Signal dass Schritt durchgeführt und nächstes Datenpaket geschickt werden kann
  return 0;
}

void loop()
{
  char in =0; 
  
  if (Serial.available()!=0)
  {
    in = Serial.read();
    
    if(in=='$') {
      state = COLLECTING;
      sentence_length = 0; 
    }
  
    if (in=='@') {
      state = DONE;   
    }
    
  if (state == COLLECTING && sentence_length<SENTENCE_MAX && (in!='$')) {
    sentence[sentence_length]=(in-48);
    Serial.write(in-48);
    sentence_length++;
  }

  if (state==DONE) {

    check = check_zero(sentence); 
    
    if (check  == 1) {
      
      read_signal(sentence);
      
    }

    state=WAITING;
  }
}
}


// Schauen ob pyserial überhaupt möglich wenn mehrere USB Teilnehmer
  

  
    
 
  
  
