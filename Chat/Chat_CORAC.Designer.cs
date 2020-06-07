﻿namespace CORAC.Chat
{
    partial class Chat_CORAC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat_CORAC));
            this.tableLayoutPanel_Chat = new System.Windows.Forms.TableLayoutPanel();
            this.CabecalhoChat = new System.Windows.Forms.PictureBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MensagemEnviar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.AvisoTelaShare = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Chat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CabecalhoChat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Chat
            // 
            this.tableLayoutPanel_Chat.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Chat.ColumnCount = 1;
            this.tableLayoutPanel_Chat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Chat.Controls.Add(this.CabecalhoChat, 0, 0);
            this.tableLayoutPanel_Chat.Controls.Add(this.webBrowser1, 0, 2);
            this.tableLayoutPanel_Chat.Controls.Add(this.splitContainer1, 0, 3);
            this.tableLayoutPanel_Chat.Controls.Add(this.splitContainer2, 0, 1);
            this.tableLayoutPanel_Chat.Location = new System.Drawing.Point(-1, -2);
            this.tableLayoutPanel_Chat.Name = "tableLayoutPanel_Chat";
            this.tableLayoutPanel_Chat.RowCount = 4;
            this.tableLayoutPanel_Chat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64.13793F));
            this.tableLayoutPanel_Chat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.86207F));
            this.tableLayoutPanel_Chat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 336F));
            this.tableLayoutPanel_Chat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel_Chat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Chat.Size = new System.Drawing.Size(807, 537);
            this.tableLayoutPanel_Chat.TabIndex = 0;
            // 
            // CabecalhoChat
            // 
            this.CabecalhoChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.CabecalhoChat.Image = global::CORAC.Properties.Resources.Cabecalho_fw;
            this.CabecalhoChat.InitialImage = null;
            this.CabecalhoChat.Location = new System.Drawing.Point(4, 4);
            this.CabecalhoChat.Name = "CabecalhoChat";
            this.CabecalhoChat.Size = new System.Drawing.Size(799, 84);
            this.CabecalhoChat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CabecalhoChat.TabIndex = 0;
            this.CabecalhoChat.TabStop = false;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            this.webBrowser1.Location = new System.Drawing.Point(4, 146);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(799, 330);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.Url = new System.Uri("http://192.168.15.10/CORAC/ChatOnline/", System.UriKind.Absolute);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(4, 483);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MensagemEnviar);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(799, 50);
            this.splitContainer1.SplitterDistance = 711;
            this.splitContainer1.TabIndex = 4;
            // 
            // MensagemEnviar
            // 
            this.MensagemEnviar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MensagemEnviar.Location = new System.Drawing.Point(97, 15);
            this.MensagemEnviar.Name = "MensagemEnviar";
            this.MensagemEnviar.Size = new System.Drawing.Size(587, 27);
            this.MensagemEnviar.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mensagem:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::CORAC.Properties.Resources.enviarmensagem;
            this.pictureBox1.Location = new System.Drawing.Point(16, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Location = new System.Drawing.Point(4, 95);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.AvisoTelaShare);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(799, 44);
            this.splitContainer2.SplitterDistance = 179;
            this.splitContainer2.TabIndex = 5;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(112, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(54, 43);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // AvisoTelaShare
            // 
            this.AvisoTelaShare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.AvisoTelaShare.AutoSize = true;
            this.AvisoTelaShare.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AvisoTelaShare.ForeColor = System.Drawing.SystemColors.Highlight;
            this.AvisoTelaShare.Location = new System.Drawing.Point(54, 8);
            this.AvisoTelaShare.Name = "AvisoTelaShare";
            this.AvisoTelaShare.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AvisoTelaShare.Size = new System.Drawing.Size(427, 29);
            this.AvisoTelaShare.TabIndex = 0;
            this.AvisoTelaShare.Text = "Tela compartilhado com Atendente.";
            this.AvisoTelaShare.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Chat_CORAC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 540);
            this.Controls.Add(this.tableLayoutPanel_Chat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            this.Name = "Chat_CORAC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SUPORTE REMOTO";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Chat_CORAC_Load);
            this.Leave += new System.EventHandler(this.Chat_CORAC_Leave);
            this.tableLayoutPanel_Chat.ResumeLayout(false);
            this.tableLayoutPanel_Chat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CabecalhoChat)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Chat;
        private System.Windows.Forms.PictureBox CabecalhoChat;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox MensagemEnviar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label AvisoTelaShare;
    }
}