namespace LeapMotion
{
    partial class LeapViewer
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.winkelText = new System.Windows.Forms.Label();
            this.BaggerArmVisual = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BaggerArmVisual)).BeginInit();
            this.SuspendLayout();
            // 
            // winkelText
            // 
            this.winkelText.AutoSize = true;
            this.winkelText.Location = new System.Drawing.Point(12, 185);
            this.winkelText.Name = "winkelText";
            this.winkelText.Size = new System.Drawing.Size(51, 20);
            this.winkelText.TabIndex = 0;
            this.winkelText.Text = "label1";
            // 
            // BaggerArmVisual
            // 
            this.BaggerArmVisual.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BaggerArmVisual.Location = new System.Drawing.Point(0, 406);
            this.BaggerArmVisual.Name = "BaggerArmVisual";
            this.BaggerArmVisual.Size = new System.Drawing.Size(1198, 552);
            this.BaggerArmVisual.TabIndex = 1;
            this.BaggerArmVisual.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1111, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LeapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 958);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BaggerArmVisual);
            this.Controls.Add(this.winkelText);
            this.Name = "LeapViewer";
            this.Text = "LeapViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LeapViewer_FormClosing);
            this.Load += new System.EventHandler(this.LeapViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BaggerArmVisual)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label winkelText;
        private System.Windows.Forms.PictureBox BaggerArmVisual;
        private System.Windows.Forms.Button button1;
    }
}

