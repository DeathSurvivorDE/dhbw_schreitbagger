using System;
using Leap;

namespace BaggerLibrary
{
    public delegate void NewBaggerData();
    public delegate void Logger(object sender);

    public class Bagger
    {
        /// <summary>Ein Event das eintritt wenn neue Werte von der Leap API gepulled wurden</summary>
        public event NewBaggerData BaggerData;
        /// <summary>Ein Event das eintritt wenn in das Log geschrieben wurde</summary>
        public event Logger OnLog;
        
        ///<summary>Bestimmt die maximale Bewegung des Baggers pro schritt</summary>
        public float MaxMovement { get; set; }
        /// <summary>Maximaler Winkel des Auslegers (default:180)</summary>
        public float TimeToReact { get; set; }

        /// <summary>Minimaler Winkel des Auslegers (default: 0 => gerade Vorraus)</summary>
        public float MinWinkelAusleger { get; set; }
        /// <summary>Maximaler Winkel des Auslegers (default:80)</summary>
        public float MaxWinkelAusleger { get; set; }
        /// <summary>Minimaler Winkel des Auslegers (default:0)</summary>
        public float MinWinkelStiel { get; set; }
        /// <summary>Maximaler Winkel des Auslegers (default:160)</summary>
        public float MaxWinkelStiel { get; set; }
        /// <summary>Minimaler Winkel des Auslegers (default:0)</summary>
        public float MinWinkelLöffel { get; set; }
        /// <summary>Maximaler Winkel des Auslegers (default:180)</summary>
        public float MaxWinkelLöffel { get; set; }

        /// <summary>Default: 0</summary>
        public float FaktorBewegung { get; set; }
        /// <summary>Default: 0</summary>
        public float FaktorLöffel { get; set; }
        /// <summary>Faktor für die gesamte Armgeschwindigkeit (default: 0)</summary>
        public float FaktorRotation { get; set; }

        /// <summary>Abstand des Löffels im Normalzustand</summary>
        public float Länge_NormLöffel
        {
            get
            {
                return (float)Math.Sqrt(Länge_Löffelstiel * Länge_Löffelstiel + Länge_Ausleger * Länge_Ausleger);
            }
        }

        Vector position;

        /// <summary>
        /// Position des Baggerlöffels
        /// </summary>
        public Vector Position
        {
            get
            {
                return position;
            }
            internal set
            {
                Vector old = position;
                position = value;
                try
                {
                    WinkelAktualisieren();
                }
                catch (ArgumentOutOfRangeException rangeEx)
                {
                    position = old;
                    //Trigger OnLoog Event
                    Say(rangeEx.Message);
                    //throw rangeEx;
                }
            }
        }

        float winkel_Ausleger;
        /// <summary>Winkel des Auslegers in Grad
        /// (Wirft eine OutOfRangeException)</summary>
        public float Winkel_Ausleger
        {
            get { return winkel_Ausleger; }
            internal set
            {
                if (value < MinWinkelAusleger || value > MaxWinkelAusleger)
                    throw new ArgumentOutOfRangeException(
                        string.Format("Winkel_Löffelstiel wurde ein Wert außerhalb des angegebenen Bereiches zugewiesen. ({0}:{1})",
                        MinWinkelAusleger, MaxWinkelAusleger));
                winkel_Ausleger = value;
            }
        }

        float winkel_Löffelstiel;
        /// <summary>Winkel des Löffelstiels in Grad
        /// (Wirft eine OutOfRangeException)</summary>
        public float Winkel_Löffelstiel
        {
            get { return winkel_Löffelstiel; }
            internal set
            {
                if (value < MinWinkelStiel || value > MaxWinkelStiel)
                    throw new ArgumentOutOfRangeException(
                        string.Format("Winkel_Löffelstiel wurde ein Wert außerhalb des angegebenen Bereiches zugewiesen. ({0}:{1})",
                        MinWinkelStiel, MaxWinkelStiel));
                winkel_Löffelstiel = value;
            }
        }

        float winkel_Löffel;
        /// <summary>Winkel des Löffels in Grad
        /// (Wirft eine OutOfRangeException)</summary>
        public float Winkel_Löffel
        {
            get { return winkel_Löffel; }
            internal set
            {
                if (value < MinWinkelLöffel || value > MaxWinkelLöffel)
                    throw new ArgumentOutOfRangeException(
                        string.Format("Winkel_Löffelstiel wurde ein Wert außerhalb des angegebenen Bereiches zugewiesen. ({0}:{1})",
                        MinWinkelLöffel, MaxWinkelLöffel));
                winkel_Löffel = value;
            }
        }

        /// <summary>Winkel des Baggers in Rad</summary>
        public float Winkel_Rotation { get; internal set; }

        /// <summary>Länge des Auslegers in [cm]</summary>
        public float Länge_Ausleger { get; set; }

        /// <summary>Länge des Löffelstiels in [cm]</summary>
        public float Länge_Löffelstiel { get; set; }

        /// <summary>
        /// Gibt einen StandartWert zurück der
        /// welcher dem Mittelpunkt des LeapMotion Sensors entspricht.
        /// (X, Y, Z) -> (0, 0, sqrt(Y^2+Z^2))
        /// </summary>
        public Vector StandardRuheOrt
        {
            get
            {
                return new Vector(0, 0, Länge_NormLöffel);
            }
        }
        
        /// <summary>Datenpool mit Triggerevent</summary>
        LeapData leapData = new LeapData(25, 0.5f, 0.5f);

        public LeapData LeapData
        {
            get
            {
                return leapData;
            }
        }

        /// <summary>
        /// Erstellt eine BaggerInstanz mit Default Werten (Diese sollten angepasst werden)
        /// </summary>
        public Bagger()
        {
            VariablenInitialisieren();

            leapData.NewValues += ValuesTriggered;
            Winkel_Ausleger = 45f;
        }

        /// <summary>
        /// Belegt alle Variablen mit "MagicNumbers". Diese sollten nach der Instanzierung angepasst werden.
        /// </summary>
        private void VariablenInitialisieren()
        {
            FaktorLöffel = 1;
            FaktorRotation = 1;
            TimeToReact = 200;
            FaktorBewegung = 1;
            Länge_Ausleger = 1;
            Länge_Löffelstiel = 1;
            MaxMovement = 10;
            MaxWinkelAusleger = 80;
            MaxWinkelLöffel = 180;
            MaxWinkelStiel = 160;
            MinWinkelAusleger = 0;
            MinWinkelLöffel = 0;
            MinWinkelStiel = 0;
            Position = StandardRuheOrt;
        }

        public void Pull()
        {
            leapData.PullData();
        }

        public void Start()
        {
            Say("Start");
            leapData.Start();
        }

        public void Stop()
        {
            Say("Stop");
            leapData.Stop();
            if (BaggerData != null)
                BaggerData();
        }

        /// <summary>
        /// Bewegt die Baggerarm in die gegebene Richtung (X wird ignoriert)
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector direction, float pinchTime, float grabTime)
        {
            Say("Moving");
            if (pinchTime < TimeToReact)
                return;

            //Validierung(Keine zu großen Bewegungen)
            if (direction.Magnitude > MaxMovement)
                direction = direction.Normalized * MaxMovement;

            //Umwandlung der Vertikalen und Tiefenbewegung in die Armposition
            Position += direction * FaktorBewegung;
        }

        public void Rotate(Vector direction, float pinchTime, float grabTime)
        {
            Say("Rotate");
            if (pinchTime < TimeToReact)
                return;
            //Umwandlung der Horizontalen Handbewegung in die Ratation
            Winkel_Rotation += direction.x * FaktorRotation;
        }

        public void Grab(float grabStrength, float rightGrabTime)
        {
            Say("Grab");
            //grabTime ist vorerst irrelevant
            Winkel_Löffel = grabStrength * (MaxWinkelLöffel - MinWinkelLöffel) * FaktorLöffel;
        }

        public void Reset()
        {
            Position = StandardRuheOrt;
        }

        /// <summary>
        /// Berechnet die neuen Winkel des Baggerarms und überprüft den Wertebereich
        /// </summary>
        /// <returns></returns>
        private void WinkelAktualisieren()
        {
            Say("Winkel");
            // Winkel aktualisieren
            float X, Y, L1, L2, L;
            X = Position.z;
            Y = Position.y;
            L1 = Länge_Ausleger;
            L2 = Länge_Löffelstiel;
            L = Position.DistanceTo(Vector.Zero);
            // Wertebereich überprüfen (nicht kleiner 0, nicht größer als der gesamte Arm)
            L = L < 0 ? 0 : L;
            L = L > L1 + L2 ? L1 + L2 : L;
            
            // relativer Winkel zur Waagerechten
            Winkel_Ausleger = (float)((Math.Acos((L2 * L2 - L1 * L1 - L * L) / (-2 * L1 * L)) + Math.Atan(Y / X)) * 180 / Math.PI);
            // relativer Winkel zur Senkrechten
            Winkel_Löffelstiel = (float)((Math.Acos((L * L - L1 * L1 - L2 * L2) / (-2 * L1 * L2))) * 180 / Math.PI);
            
        }

        /// <summary>
        /// Triggert das Event
        /// </summary>
        bool arbeitend = false;
        private void ValuesTriggered()
        {
            Say("Trigger");
            if (arbeitend)
            {
                //MessageBox.Show("arbeitend==true;\n(Hat den letzten Trigger nicht vollständig bearbeiten können.)", "Haha!!");
                return;
            }
            arbeitend = true;
            
            Move(leapData.RightMovement, leapData.RightPinchTime, leapData.RightGrabTime);
            Rotate(leapData.RightMovement, leapData.RightPinchTime, leapData.RightGrabTime);
            Grab(leapData.RightHand != null ? leapData.RightHand.GrabStrength : 0f, leapData.RightGrabTime);

            if (BaggerData != null)
                BaggerData();

            arbeitend = false;
        }

        private void Say(string v)
        {
            if (OnLog != null)
                OnLog("Library: " + v);
        }

    }
}
