namespace CORAC.Logo
{
    partial class LogoAcessoRemoto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LOGO_AR = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LOGO_AR)).BeginInit();
            this.SuspendLayout();
            // 
            // LOGO_AR
            // 
            this.LOGO_AR.Image = global::CORAC.Properties.Resources.AR_LOGO;
            this.LOGO_AR.Location = new System.Drawing.Point(12, 12);
            this.LOGO_AR.Name = "LOGO_AR";
            this.LOGO_AR.Size = new System.Drawing.Size(141, 114);
            this.LOGO_AR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LOGO_AR.TabIndex = 0;
            this.LOGO_AR.TabStop = false;
            // 
            // LogoAcessoRemoto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(162, 135);
            this.Controls.Add(this.LOGO_AR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogoAcessoRemoto";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LogoAcessoRemoto";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.LogoAcessoRemoto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LOGO_AR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox LOGO_AR;
    }
}