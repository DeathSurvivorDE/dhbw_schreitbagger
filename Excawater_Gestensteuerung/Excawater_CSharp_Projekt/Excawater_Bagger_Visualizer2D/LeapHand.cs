//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
using System;
using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeapMotion
{
    public partial class LeapHand : Control
    {
        private System.Windows.Forms.Label HandPositionX;
        private System.Windows.Forms.Label HandPositionY;
        private System.Windows.Forms.Label HandPositionZ;
        private System.Windows.Forms.Label HandStatus;
        private System.Windows.Forms.Label HandGehste;

        public LeapHand()
        {
            this.HandPositionX = new System.Windows.Forms.Label();
            this.HandPositionY = new System.Windows.Forms.Label();
            this.HandPositionZ = new System.Windows.Forms.Label();
            this.HandStatus = new System.Windows.Forms.Label();
            this.HandGehste = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // HandPositionX
            // 
            this.HandPositionX.AutoSize = true;
            this.HandPositionX.Location = new System.Drawing.Point(3, 0);
            this.HandPositionX.Name = "HandPositionX";
            this.HandPositionX.Size = new System.Drawing.Size(51, 20);
            this.HandPositionX.TabIndex = 0;
            this.HandPositionX.Text = "label1";
            // 
            // HandPositionY
            // 
            this.HandPositionY.AutoSize = true;
            this.HandPositionY.Location = new System.Drawing.Point(3, 20);
            this.HandPositionY.Name = "HandPositionY";
            this.HandPositionY.Size = new System.Drawing.Size(51, 20);
            this.HandPositionY.TabIndex = 1;
            this.HandPositionY.Text = "label1";
            // 
            // HandPositionZ
            // 
            this.HandPositionZ.AutoSize = true;
            this.HandPositionZ.Location = new System.Drawing.Point(3, 40);
            this.HandPositionZ.Name = "HandPositionZ";
            this.HandPositionZ.Size = new System.Drawing.Size(51, 20);
            this.HandPositionZ.TabIndex = 2;
            this.HandPositionZ.Text = "label1";
            // 
            // HandStatus
            // 
            this.HandStatus.AutoSize = true;
            this.HandStatus.Location = new System.Drawing.Point(6, 60);
            this.HandStatus.Name = "HandStatus";
            this.HandStatus.Size = new System.Drawing.Size(51, 20);
            this.HandStatus.TabIndex = 3;
            this.HandStatus.Text = "label1";
            // 
            // HandStatus
            // 
            this.HandGehste.AutoSize = true;
            this.HandGehste.Location = new System.Drawing.Point(6, 80);
            this.HandGehste.Name = "HandGehste";
            this.HandGehste.Size = new System.Drawing.Size(51, 40);
            this.HandGehste.TabIndex = 3;
            this.HandGehste.Text = "label1";
            // 
            // LeapHand
            // 
            this.Controls.Add(this.HandPositionX);
            this.Controls.Add(this.HandPositionY);
            this.Controls.Add(this.HandPositionZ);
            this.Controls.Add(this.HandStatus);
            this.Controls.Add(this.HandGehste);
            this.Name = "LeapHand";
            this.Size = new System.Drawing.Size(102, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void loadHand(Leap.Hand hand)
        {
            Leap.Vector v = hand != null ? hand.PalmPosition : Leap.Vector.Zero;
            this.HandPositionX.Text = string.Format("X: {0:N1}", v.x);
            this.HandPositionY.Text = string.Format("Y: {0:N1}", v.y);
            this.HandPositionZ.Text = string.Format("Z: {0:N1}", v.z);
            this.HandStatus.BackColor = hand != null ? Color.Green : Color.Red;
            this.HandStatus.Text = hand != null ? hand.IsRight ? "Rechts" : "Links" : "Invalid";
            this.HandGehste.Text = string.Format("Pinch:{0:P}\nGrab:{1:P}",
                hand != null ? hand.PinchStrength : 0,
                hand != null ? hand.GrabStrength : 0);
            this.HandGehste.BackColor = hand != null && Math.Max(hand.PinchStrength, hand.GrabStrength) > 0.5 ? (hand.PinchStrength > hand.GrabStrength ? Color.GreenYellow : Color.DeepSkyBlue) : this.BackColor;
            //this.Refresh();
        }
    }
}
