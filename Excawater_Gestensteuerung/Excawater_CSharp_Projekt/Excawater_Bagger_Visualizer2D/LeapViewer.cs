using System;
using System.Drawing;
using System.Windows.Forms;
using Leap;
using BaggerLibrary;

namespace LeapMotion
{
    public partial class LeapViewer : Form
    {

        private static double DegreeToRad = (float)(Math.PI / 180);

        Bagger bagger = new Bagger()
        {
            Länge_Ausleger = 300,
            Länge_Löffelstiel = 200,
            FaktorRotation = 0.5f,
            TimeToReact = 200,
            FaktorBewegung = 2
        };

        LeapHand leftHandControl = new LeapHand(), rightHandControl = new LeapHand();

        Hand rightHand, leftHand;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LeapViewer()
        {
            InitializeComponent();
            leftHandControl.Name = "hand1";
            rightHandControl.Name = "hand2";
            leftHandControl.Location = new Point(10, 10);
            rightHandControl.Location = new Point(150, 10);
            this.Controls.Add(leftHandControl);
            this.Controls.Add(rightHandControl);

            bagger.Reset();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeapViewer_Load(object sender, EventArgs e)
        {
            bagger.Start();
            bagger.BaggerData += RefreshValue;
        }

        private void LeapViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            bagger.BaggerData -= RefreshValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bagger.Pull();
        }

        private void RefreshValue()
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke((Action)(() => { RefreshValue(); }));
                }
                catch { }
            }
            else
            {
                rightHand = bagger.LeapData.RightHand;
                leftHand = bagger.LeapData.LeftHand;

                leftHandControl.loadHand(leftHand);
                rightHandControl.loadHand(rightHand);

                winkelText.Text = string.Format("#Ausleger:{0:N2}\n#Stiel:{1:N2}\n#Löffel:{2:N2}\n#Rotation:{3:N2}\nFPS:{4:N2}\nBaggerPos(X:{5:N0}, Y:{6:N0})\nRPinch:{7}",
                    bagger.Winkel_Ausleger,
                    bagger.Winkel_Löffelstiel,
                    bagger.Winkel_Löffel,
                    bagger.Winkel_Rotation,
                    bagger.LeapData.FPS,
                    bagger.Position.z,
                    bagger.Position.y,
                    bagger.LeapData.RightPinchTime);

                try
                {
                    this.Refresh();
                }
                catch { }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 4);
            //zeichen
            try
            {
                //Startpunkt zu Ausleger
                double AX = bagger.Länge_Ausleger *
                    Math.Cos(bagger.Winkel_Ausleger * DegreeToRad);
                double AY = -bagger.Länge_Ausleger *
                    Math.Sin(bagger.Winkel_Ausleger * DegreeToRad);

                //Ausleger zu Stiel
                double BX = bagger.Länge_Löffelstiel *
                    Math.Sin((bagger.Winkel_Löffelstiel + bagger.Winkel_Ausleger) * DegreeToRad - Math.PI * 0.5d);
                double BY = bagger.Länge_Löffelstiel *
                    Math.Cos((bagger.Winkel_Löffelstiel + bagger.Winkel_Ausleger) * DegreeToRad - Math.PI * 0.5d);

                //Stiel zu Löffel
                double CX = 30 *
                    Math.Sin((bagger.Winkel_Ausleger + bagger.Winkel_Löffelstiel - bagger.Winkel_Löffel) * DegreeToRad - Math.PI * 0.5d);
                double CY = 30 *
                    Math.Cos((bagger.Winkel_Ausleger + bagger.Winkel_Löffelstiel - bagger.Winkel_Löffel) * DegreeToRad - Math.PI * 0.5d);

                //Rotation
                double RX = 30 *
                    Math.Sin(bagger.Winkel_Rotation * DegreeToRad);
                double RY = 30 *
                    Math.Cos(bagger.Winkel_Rotation * DegreeToRad);

                Bitmap image = new Bitmap(600, 400);
                using (Graphics g = Graphics.FromImage(image))
                {
                    Point Offset, P1, P2, P3;
                    Offset = P1 = new Point(50, 220);
                    P1.Offset((int)AX, (int)AY);//bewege Punkt 1 zur Spitze
                    P2 = P1;
                    P2.Offset((int)BX, (int)BY);//bewege Punkt 2 zum Löffel
                    P3 = P2;
                    P3.Offset((int)CX, (int)CY);//bewege Punkt 3 zu Löffel Ende

                    //zeige die Rotation
                    g.DrawEllipse(Pens.Black, 0, 0, 60, 60);
                    g.DrawLine(Pens.Red, 30, 30, 30 + (float)RX, 30 - (float)RY);

                    g.DrawLine(pen, Offset, P1);
                    g.DrawLine(pen, P1, P2);
                    g.DrawLine(pen, P2, P3);

                    //provisorisch
                    g.DrawEllipse(Pens.Red, Offset.X + 290, Offset.Y - 70, 140, 140);
                    //g.FillRectangle(hand1.PinchStrength > 0.5 ? Brushes.Red : Brushes.Black, P2.X, P2.Y, 3, 3);
                }
                BaggerArmVisual.Image = image;
                BaggerArmVisual.Refresh();

                image.Dispose();
            }
            catch { }
            //base.OnPaint(e);
        }
    }
}
