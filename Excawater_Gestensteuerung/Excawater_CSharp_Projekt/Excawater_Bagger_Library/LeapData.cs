using System;
using System.ComponentModel;
using Leap;

namespace BaggerLibrary
{

    public delegate void NewValuesEventHandler();

    /// <summary>
    /// Gibt Standartwerte des Leapmotion Sensors zurück
    /// </summary>
    public class LeapData
    {
        /// <summary>Event tritt nach Ablauf der Zeitspanne ein</summary>
        public event NewValuesEventHandler NewValues;
        
        /// <summary>Leap Controller</summary>
        Controller leapController;

        /// <summary></summary>
        BackgroundWorker updater = new BackgroundWorker();

        /// <summary></summary>
        public int Interval { get; set; }

        /// <variables></variables>
        /// <summary>Array aller erkannten Hände</summary>
        System.Collections.Generic.List<Hand> hands;

        public DateTime LastUpdate { get; private set; }

        DateTime
            rightPinchTime,
            rightGrabTime,
            leftPinchTime,
            leftGrabTime;

        public float RightPinchTime
        {
            get
            {
                return (float)(LastUpdate - rightPinchTime).TotalMilliseconds;
            }
        }
        public float RightGrabTime
        {
            get
            {
                return (float)(LastUpdate - rightGrabTime).TotalMilliseconds;
            }
        }
        public float LeftPinchTime
        {
            get
            {
                return (float)(LastUpdate - leftPinchTime).TotalMilliseconds;
            }
        }
        public float LeftGrabTime
        {
            get
            {
                return (float)(LastUpdate - leftGrabTime).TotalMilliseconds;
            }
        }

        public float PinchThreshold { get; private set; }
        public float GrabThreshold { get; private set; }

        Vector
            rightMovement,
            leftMovement;

        Hand
            leftHand,
            rightHand;


        public Hand LeftHand
        {
            get
            {
                return leftHand;
            }
            private set
            {
                if (value != null && leftHand != null)
                    LeftMovement = value.PalmPosition - leftHand.PalmPosition;
                else
                    LeftMovement = Vector.Zero;

                leftHand = value;
                CheckLeftHand();
            }
        }

        public Hand RightHand
        {
            get
            {
                return rightHand;
            }
            private set
            {
                if (value != null && rightHand != null)
                    RightMovement = value.PalmPosition - rightHand.PalmPosition;
                else
                    RightMovement = Vector.Zero;

                rightHand = value;
                CheckRightHand();
            }
        }

        /// <summary> Der Bewegungsvektor der linken Hand </summary>
        public Vector LeftMovement
        {
            get
            {
                return leftMovement;
            }
            private set
            {
                leftMovement = new Vector(value.x, value.y, -value.z);
            }
        }

        /// <summary> Der Bewegungsvektor der rechten Hand </summary>
        public Vector RightMovement
        {
            get
            {
                return rightMovement;
            }
            private set
            {
                rightMovement = new Vector(value.x, value.y, -value.z);
            }
        }

        float fps;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Zeitspanne zum nächsten Event in [ms]</param>
        public LeapData(int interval, float pinchThreshold, float grabThreshold)
        {
            // Setup Variables
            LastUpdate = DateTime.Now;
            rightPinchTime = DateTime.Now;
            rightGrabTime = DateTime.Now;
            leftPinchTime = DateTime.Now;
            leftGrabTime = DateTime.Now;

            PinchThreshold = pinchThreshold;
            GrabThreshold = grabThreshold;

            leapController = new Controller();
            //erlaubt das aktualisieren im Hintergrund
            leapController.SetPolicy(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);

            //leapController.FrameReady += LeapController_FrameReady;
            updater.DoWork += Updater_DoWork;
            updater.RunWorkerCompleted += Updater_RunWorkerCompleted;
        }

        /// <summary>
        /// Wartet das Angegebene Interval ab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Updater_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(Interval);
        }

        private void Updater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PullData();
            updater.RunWorkerAsync();
        }

        private void LeapController_FrameReady(object sender, FrameEventArgs e)
        {
            PullData();
        }
        
        /// <summary>
        /// Liest den nächsten Frame vom Leap Motion Controller und sammelt Daten
        /// </summary>
        public void PullData()
        {
            if (leapController.IsConnected)
            {
                //Frame f = leapController.Frame();
                hands = leapController.Frame().Hands;
                fps = leapController.Frame().CurrentFramesPerSecond;

                LastUpdate = DateTime.Now;
                GetHands();

                //Trigger Event
                if (NewValues != null)
                    NewValues();
            }
        }

        /// <summary>
        /// Startet auslesen
        /// </summary>
        //public void Start(int interval) { this.updateInterval = interval; this.Start(); }
        public void Start()
        {
            leapController.StartConnection();
            updater.RunWorkerAsync();
        }

        /// <summary>
        /// Stoppt auslesen und beendet die LeapVerbindung
        /// <!--Der GC würde sie zwar beenden aber in Mono wird dieser nicht aufgerufen-->
        /// </summary>
        public void Stop()
        {
            leapController.StopConnection();
        }
        
        /// <summary>
        /// Versucht die linke und rechte Hand zu finden
        /// (erlaubt nur 1 linke und 1 rechte Hand)
        /// </summary>
        private void GetHands()
        {
            switch (hands.Count)
            {
                case 1:
                    if (hands[0].IsLeft)
                    {
                        LeftHand = hands[0];
                        RightHand = null;
                    }
                    else
                    {
                        RightHand = hands[0];
                        LeftHand = null;
                    }
                    break;
                case 2:
                    if (hands[0].IsLeft & hands[1].IsRight)
                    {
                        LeftHand = hands[0];
                        RightHand = hands[1];
                    }
                    else if (hands[0].IsRight & hands[1].IsLeft)
                    {
                        RightHand = hands[0];
                        LeftHand = hands[1];
                    }
                    else
                    {
                        RightHand = null;
                        LeftHand = null;
                    }
                    break;
                default:
                    RightHand = null;
                    LeftHand = null;
                    break;
            }
        }

        /// <summary>
        /// Überprüft die Stati der linken Hand
        /// </summary>
        private void CheckLeftHand()
        {
            if (LeftHand == null)
            {
                leftPinchTime = LastUpdate;
                leftGrabTime = LastUpdate;
            }
            else
            {
                if (LeftHand.PinchStrength < PinchThreshold)
                    leftPinchTime = LastUpdate;
                if (LeftHand.GrabStrength < GrabThreshold)
                    leftGrabTime = LastUpdate;
            }
        }

        /// <summary>
        /// Überprüft die Stati der rechten Hand
        /// </summary>
        private void CheckRightHand()
        {
            if (RightHand == null)
            {
                rightPinchTime = LastUpdate;
                rightGrabTime = LastUpdate;
            }
            else
            {
                if (RightHand.PinchStrength < PinchThreshold)
                    rightPinchTime = LastUpdate;
                if (RightHand.GrabStrength < GrabThreshold)
                    rightGrabTime = LastUpdate;
            }
        }
        
        /////////////////////////GETTER/////////////////////////

        public int HandCount
        {
            get { return hands.Count; }
        }

        public float FPS
        {
            get { return this.fps; }
        }
    }
}
