namespace CORAC
{
    partial class CORAC_TPrincipal
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CORAC_TPrincipal));
            this.tabCORAC = new System.Windows.Forms.TabControl();
            this.Tab_Status = new System.Windows.Forms.TabPage();
            this.Registro_Status_GroupBox = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.Atualizacao_Status_GoupBox = new System.Windows.Forms.GroupBox();
            this.picture_Atualizacoes_CORAC = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.Internet_Status_GroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.picture_Internet_CORAC = new System.Windows.Forms.PictureBox();
            this.Config = new System.Windows.Forms.TabPage();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabCORAC.SuspendLayout();
            this.Tab_Status.SuspendLayout();
            this.Registro_Status_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.Atualizacao_Status_GoupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Atualizacoes_CORAC)).BeginInit();
            this.Internet_Status_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Internet_CORAC)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCORAC
            // 
            this.tabCORAC.Controls.Add(this.Tab_Status);
            this.tabCORAC.Controls.Add(this.Config);
            this.tabCORAC.Location = new System.Drawing.Point(8, 8);
            this.tabCORAC.Name = "tabCORAC";
            this.tabCORAC.SelectedIndex = 0;
            this.tabCORAC.Size = new System.Drawing.Size(868, 434);
            this.tabCORAC.TabIndex = 0;
            // 
            // Tab_Status
            // 
            this.Tab_Status.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Tab_Status.Controls.Add(this.Registro_Status_GroupBox);
            this.Tab_Status.Controls.Add(this.Atualizacao_Status_GoupBox);
            this.Tab_Status.Controls.Add(this.Internet_Status_GroupBox);
            this.Tab_Status.Location = new System.Drawing.Point(4, 22);
            this.Tab_Status.Name = "Tab_Status";
            this.Tab_Status.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Status.Size = new System.Drawing.Size(860, 408);
            this.Tab_Status.TabIndex = 0;
            this.Tab_Status.Text = "Status";
            // 
            // Registro_Status_GroupBox
            // 
            this.Registro_Status_GroupBox.Controls.Add(this.pictureBox1);
            this.Registro_Status_GroupBox.Controls.Add(this.button3);
            this.Registro_Status_GroupBox.Location = new System.Drawing.Point(589, 18);
            this.Registro_Status_GroupBox.Name = "Registro_Status_GroupBox";
            this.Registro_Status_GroupBox.Size = new System.Drawing.Size(254, 217);
            this.Registro_Status_GroupBox.TabIndex = 5;
            this.Registro_Status_GroupBox.TabStop = false;
            this.Registro_Status_GroupBox.Text = "Registro";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::CORAC.Properties.Resources.Registro_256px;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(64, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(134, 131);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(90, 176);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Verificar";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Atualizacao_Status_GoupBox
            // 
            this.Atualizacao_Status_GoupBox.Controls.Add(this.picture_Atualizacoes_CORAC);
            this.Atualizacao_Status_GoupBox.Controls.Add(this.button2);
            this.Atualizacao_Status_GoupBox.Location = new System.Drawing.Point(311, 18);
            this.Atualizacao_Status_GoupBox.Name = "Atualizacao_Status_GoupBox";
            this.Atualizacao_Status_GoupBox.Size = new System.Drawing.Size(254, 217);
            this.Atualizacao_Status_GoupBox.TabIndex = 4;
            this.Atualizacao_Status_GoupBox.TabStop = false;
            this.Atualizacao_Status_GoupBox.Text = "Atualizações";
            // 
            // picture_Atualizacoes_CORAC
            // 
            this.picture_Atualizacoes_CORAC.Cursor = System.Windows.Forms.Cursors.Default;
            this.picture_Atualizacoes_CORAC.ErrorImage = null;
            this.picture_Atualizacoes_CORAC.Image = global::CORAC.Properties.Resources.Update_System_126px;
            this.picture_Atualizacoes_CORAC.InitialImage = null;
            this.picture_Atualizacoes_CORAC.Location = new System.Drawing.Point(64, 39);
            this.picture_Atualizacoes_CORAC.Name = "picture_Atualizacoes_CORAC";
            this.picture_Atualizacoes_CORAC.Size = new System.Drawing.Size(134, 131);
            this.picture_Atualizacoes_CORAC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture_Atualizacoes_CORAC.TabIndex = 3;
            this.picture_Atualizacoes_CORAC.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(90, 176);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Verificar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Internet_Status_GroupBox
            // 
            this.Internet_Status_GroupBox.Controls.Add(this.button1);
            this.Internet_Status_GroupBox.Controls.Add(this.picture_Internet_CORAC);
            this.Internet_Status_GroupBox.Location = new System.Drawing.Point(22, 18);
            this.Internet_Status_GroupBox.Name = "Internet_Status_GroupBox";
            this.Internet_Status_GroupBox.Size = new System.Drawing.Size(266, 217);
            this.Internet_Status_GroupBox.TabIndex = 3;
            this.Internet_Status_GroupBox.TabStop = false;
            this.Internet_Status_GroupBox.Text = "Internet";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Verificar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // picture_Internet_CORAC
            // 
            this.picture_Internet_CORAC.Cursor = System.Windows.Forms.Cursors.Default;
            this.picture_Internet_CORAC.ErrorImage = null;
            this.picture_Internet_CORAC.Image = ((System.Drawing.Image)(resources.GetObject("picture_Internet_CORAC.Image")));
            this.picture_Internet_CORAC.InitialImage = null;
            this.picture_Internet_CORAC.Location = new System.Drawing.Point(61, 39);
            this.picture_Internet_CORAC.Name = "picture_Internet_CORAC";
            this.picture_Internet_CORAC.Size = new System.Drawing.Size(134, 131);
            this.picture_Internet_CORAC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture_Internet_CORAC.TabIndex = 1;
            this.picture_Internet_CORAC.TabStop = false;
            this.picture_Internet_CORAC.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picture_Status_CORAC_MouseClick);
            // 
            // Config
            // 
            this.Config.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Config.Location = new System.Drawing.Point(4, 22);
            this.Config.Name = "Config";
            this.Config.Padding = new System.Windows.Forms.Padding(3);
            this.Config.Size = new System.Drawing.Size(860, 408);
            this.Config.TabIndex = 1;
            this.Config.Text = "Configurações";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // CORAC_TPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 450);
            this.Controls.Add(this.tabCORAC);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.Name = "CORAC_TPrincipal";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CORAC";
            this.TopMost = true;
            this.tabCORAC.ResumeLayout(false);
            this.Tab_Status.ResumeLayout(false);
            this.Registro_Status_GroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.Atualizacao_Status_GoupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture_Atualizacoes_CORAC)).EndInit();
            this.Internet_Status_GroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture_Internet_CORAC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCORAC;
        private System.Windows.Forms.TabPage Tab_Status;
        private System.Windows.Forms.TabPage Config;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox Internet_Status_GroupBox;
        private System.Windows.Forms.GroupBox Atualizacao_Status_GoupBox;
        private System.Windows.Forms.PictureBox picture_Internet_CORAC;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox picture_Atualizacoes_CORAC;
        private System.Windows.Forms.GroupBox Registro_Status_GroupBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button3;
    }
}

