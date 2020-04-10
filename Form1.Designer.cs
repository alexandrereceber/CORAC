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
            this.Layout_Organizacao_CORAC = new System.Windows.Forms.TableLayoutPanel();
            this.Internet_Status_GroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Atualizacao_Status_GoupBox = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.Banco_Dados_Status_GroupBox = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.Registro_Status_GroupBox = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.Config = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_Credencial_Senha = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Credencial_Usuario = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_Atualizacao_Caminho = new System.Windows.Forms.TextBox();
            this.Caminho_Serv_Atual_Corac = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_Autenticacao_WEB = new System.Windows.Forms.RadioButton();
            this.radioButton_Autenticacao_LDAP = new System.Windows.Forms.RadioButton();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox5_BD_CORAC = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.picture_Internet_CORAC = new System.Windows.Forms.PictureBox();
            this.picture_Atualizacoes_CORAC = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox6_HTTP = new System.Windows.Forms.PictureBox();
            this.pictureBox5_LDAP = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tabCORAC.SuspendLayout();
            this.Tab_Status.SuspendLayout();
            this.Layout_Organizacao_CORAC.SuspendLayout();
            this.Internet_Status_GroupBox.SuspendLayout();
            this.Atualizacao_Status_GoupBox.SuspendLayout();
            this.Banco_Dados_Status_GroupBox.SuspendLayout();
            this.Registro_Status_GroupBox.SuspendLayout();
            this.Config.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Internet_CORAC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Atualizacoes_CORAC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6_HTTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5_LDAP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCORAC
            // 
            this.tabCORAC.Controls.Add(this.Tab_Status);
            this.tabCORAC.Controls.Add(this.Config);
            this.tabCORAC.Location = new System.Drawing.Point(8, 8);
            this.tabCORAC.Name = "tabCORAC";
            this.tabCORAC.SelectedIndex = 0;
            this.tabCORAC.Size = new System.Drawing.Size(888, 533);
            this.tabCORAC.TabIndex = 0;
            // 
            // Tab_Status
            // 
            this.Tab_Status.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Tab_Status.Controls.Add(this.Layout_Organizacao_CORAC);
            this.Tab_Status.Location = new System.Drawing.Point(4, 22);
            this.Tab_Status.Name = "Tab_Status";
            this.Tab_Status.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Status.Size = new System.Drawing.Size(880, 507);
            this.Tab_Status.TabIndex = 0;
            this.Tab_Status.Text = "Status";
            // 
            // Layout_Organizacao_CORAC
            // 
            this.Layout_Organizacao_CORAC.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.Layout_Organizacao_CORAC.ColumnCount = 3;
            this.Layout_Organizacao_CORAC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout_Organizacao_CORAC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout_Organizacao_CORAC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.Layout_Organizacao_CORAC.Controls.Add(this.Internet_Status_GroupBox, 0, 0);
            this.Layout_Organizacao_CORAC.Controls.Add(this.Atualizacao_Status_GoupBox, 1, 0);
            this.Layout_Organizacao_CORAC.Controls.Add(this.Banco_Dados_Status_GroupBox, 0, 1);
            this.Layout_Organizacao_CORAC.Controls.Add(this.Registro_Status_GroupBox, 2, 0);
            this.Layout_Organizacao_CORAC.Location = new System.Drawing.Point(6, 6);
            this.Layout_Organizacao_CORAC.Name = "Layout_Organizacao_CORAC";
            this.Layout_Organizacao_CORAC.RowCount = 2;
            this.Layout_Organizacao_CORAC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout_Organizacao_CORAC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout_Organizacao_CORAC.Size = new System.Drawing.Size(868, 501);
            this.Layout_Organizacao_CORAC.TabIndex = 0;
            // 
            // Internet_Status_GroupBox
            // 
            this.Internet_Status_GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Internet_Status_GroupBox.Controls.Add(this.button1);
            this.Internet_Status_GroupBox.Controls.Add(this.picture_Internet_CORAC);
            this.Internet_Status_GroupBox.Location = new System.Drawing.Point(5, 5);
            this.Internet_Status_GroupBox.Name = "Internet_Status_GroupBox";
            this.Internet_Status_GroupBox.Size = new System.Drawing.Size(268, 241);
            this.Internet_Status_GroupBox.TabIndex = 7;
            this.Internet_Status_GroupBox.TabStop = false;
            this.Internet_Status_GroupBox.Text = "Internet";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(84, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Verificar";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Atualizacao_Status_GoupBox
            // 
            this.Atualizacao_Status_GoupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Atualizacao_Status_GoupBox.Controls.Add(this.picture_Atualizacoes_CORAC);
            this.Atualizacao_Status_GoupBox.Controls.Add(this.button2);
            this.Atualizacao_Status_GoupBox.Location = new System.Drawing.Point(281, 5);
            this.Atualizacao_Status_GoupBox.Name = "Atualizacao_Status_GoupBox";
            this.Atualizacao_Status_GoupBox.Size = new System.Drawing.Size(268, 241);
            this.Atualizacao_Status_GoupBox.TabIndex = 8;
            this.Atualizacao_Status_GoupBox.TabStop = false;
            this.Atualizacao_Status_GoupBox.Text = "Atualizações";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(98, 176);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Verificar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Banco_Dados_Status_GroupBox
            // 
            this.Banco_Dados_Status_GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Banco_Dados_Status_GroupBox.Controls.Add(this.pictureBox2);
            this.Banco_Dados_Status_GroupBox.Controls.Add(this.button4);
            this.Banco_Dados_Status_GroupBox.Location = new System.Drawing.Point(5, 254);
            this.Banco_Dados_Status_GroupBox.Name = "Banco_Dados_Status_GroupBox";
            this.Banco_Dados_Status_GroupBox.Size = new System.Drawing.Size(268, 242);
            this.Banco_Dados_Status_GroupBox.TabIndex = 10;
            this.Banco_Dados_Status_GroupBox.TabStop = false;
            this.Banco_Dados_Status_GroupBox.Text = "Banco de Dados";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(86, 187);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Verificar";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // Registro_Status_GroupBox
            // 
            this.Registro_Status_GroupBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Registro_Status_GroupBox.Controls.Add(this.pictureBox1);
            this.Registro_Status_GroupBox.Controls.Add(this.button3);
            this.Registro_Status_GroupBox.Location = new System.Drawing.Point(560, 8);
            this.Registro_Status_GroupBox.Name = "Registro_Status_GroupBox";
            this.Registro_Status_GroupBox.Size = new System.Drawing.Size(300, 234);
            this.Registro_Status_GroupBox.TabIndex = 9;
            this.Registro_Status_GroupBox.TabStop = false;
            this.Registro_Status_GroupBox.Text = "Registro";
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button3.Location = new System.Drawing.Point(124, 176);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(81, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Verificar";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Config
            // 
            this.Config.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Config.Controls.Add(this.tableLayoutPanel1);
            this.Config.Location = new System.Drawing.Point(4, 22);
            this.Config.Name = "Config";
            this.Config.Padding = new System.Windows.Forms.Padding(3);
            this.Config.Size = new System.Drawing.Size(880, 507);
            this.Config.TabIndex = 1;
            this.Config.Text = "Configurações";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.CausesValidation = false;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox4, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(868, 495);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.Controls.Add(this.textBox_Credencial_Senha);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox_Credencial_Usuario);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(98, 256);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(762, 103);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Credenciais";
            // 
            // textBox_Credencial_Senha
            // 
            this.textBox_Credencial_Senha.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Credencial_Senha.Location = new System.Drawing.Point(404, 49);
            this.textBox_Credencial_Senha.Name = "textBox_Credencial_Senha";
            this.textBox_Credencial_Senha.Size = new System.Drawing.Size(195, 22);
            this.textBox_Credencial_Senha.TabIndex = 5;
            this.textBox_Credencial_Senha.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label2.Location = new System.Drawing.Point(348, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Senha:";
            // 
            // textBox_Credencial_Usuario
            // 
            this.textBox_Credencial_Usuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Credencial_Usuario.Location = new System.Drawing.Point(94, 49);
            this.textBox_Credencial_Usuario.Name = "textBox_Credencial_Usuario";
            this.textBox_Credencial_Usuario.Size = new System.Drawing.Size(191, 22);
            this.textBox_Credencial_Usuario.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(30, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Usuário:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_Atualizacao_Caminho);
            this.groupBox1.Controls.Add(this.Caminho_Serv_Atual_Corac);
            this.groupBox1.Location = new System.Drawing.Point(98, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(762, 104);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Atualização CORAC";
            // 
            // textBox_Atualizacao_Caminho
            // 
            this.textBox_Atualizacao_Caminho.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Atualizacao_Caminho.Location = new System.Drawing.Point(100, 46);
            this.textBox_Atualizacao_Caminho.Name = "textBox_Atualizacao_Caminho";
            this.textBox_Atualizacao_Caminho.Size = new System.Drawing.Size(357, 22);
            this.textBox_Atualizacao_Caminho.TabIndex = 1;
            // 
            // Caminho_Serv_Atual_Corac
            // 
            this.Caminho_Serv_Atual_Corac.AutoSize = true;
            this.Caminho_Serv_Atual_Corac.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Caminho_Serv_Atual_Corac.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Caminho_Serv_Atual_Corac.Location = new System.Drawing.Point(26, 28);
            this.Caminho_Serv_Atual_Corac.Name = "Caminho_Serv_Atual_Corac";
            this.Caminho_Serv_Atual_Corac.Size = new System.Drawing.Size(0, 13);
            this.Caminho_Serv_Atual_Corac.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox2.Controls.Add(this.radioButton_Autenticacao_WEB);
            this.groupBox2.Controls.Add(this.radioButton_Autenticacao_LDAP);
            this.groupBox2.Controls.Add(this.pictureBox6_HTTP);
            this.groupBox2.Controls.Add(this.pictureBox5_LDAP);
            this.groupBox2.Location = new System.Drawing.Point(98, 131);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(762, 107);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tipo Autenticação";
            // 
            // radioButton_Autenticacao_WEB
            // 
            this.radioButton_Autenticacao_WEB.AutoSize = true;
            this.radioButton_Autenticacao_WEB.Location = new System.Drawing.Point(589, 48);
            this.radioButton_Autenticacao_WEB.Name = "radioButton_Autenticacao_WEB";
            this.radioButton_Autenticacao_WEB.Size = new System.Drawing.Size(50, 17);
            this.radioButton_Autenticacao_WEB.TabIndex = 3;
            this.radioButton_Autenticacao_WEB.TabStop = true;
            this.radioButton_Autenticacao_WEB.Text = "WEB";
            this.radioButton_Autenticacao_WEB.UseVisualStyleBackColor = true;
            // 
            // radioButton_Autenticacao_LDAP
            // 
            this.radioButton_Autenticacao_LDAP.AutoSize = true;
            this.radioButton_Autenticacao_LDAP.Location = new System.Drawing.Point(193, 43);
            this.radioButton_Autenticacao_LDAP.Name = "radioButton_Autenticacao_LDAP";
            this.radioButton_Autenticacao_LDAP.Size = new System.Drawing.Size(73, 17);
            this.radioButton_Autenticacao_LDAP.TabIndex = 2;
            this.radioButton_Autenticacao_LDAP.TabStop = true;
            this.radioButton_Autenticacao_LDAP.Text = "LDAP/AD";
            this.radioButton_Autenticacao_LDAP.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox4.Controls.Add(this.button6);
            this.groupBox4.Controls.Add(this.textBox5_BD_CORAC);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(98, 380);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(762, 103);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Banco de Dados CORAC";
            // 
            // textBox5_BD_CORAC
            // 
            this.textBox5_BD_CORAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5_BD_CORAC.Location = new System.Drawing.Point(94, 49);
            this.textBox5_BD_CORAC.Name = "textBox5_BD_CORAC";
            this.textBox5_BD_CORAC.Size = new System.Drawing.Size(292, 22);
            this.textBox5_BD_CORAC.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label4.Location = new System.Drawing.Point(24, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Caminho:";
            // 
            // picture_Internet_CORAC
            // 
            this.picture_Internet_CORAC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.picture_Internet_CORAC.Cursor = System.Windows.Forms.Cursors.Default;
            this.picture_Internet_CORAC.ErrorImage = null;
            this.picture_Internet_CORAC.Image = global::CORAC.Properties.Resources.Internet;
            this.picture_Internet_CORAC.InitialImage = null;
            this.picture_Internet_CORAC.Location = new System.Drawing.Point(55, 40);
            this.picture_Internet_CORAC.Name = "picture_Internet_CORAC";
            this.picture_Internet_CORAC.Size = new System.Drawing.Size(134, 131);
            this.picture_Internet_CORAC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture_Internet_CORAC.TabIndex = 1;
            this.picture_Internet_CORAC.TabStop = false;
            // 
            // picture_Atualizacoes_CORAC
            // 
            this.picture_Atualizacoes_CORAC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picture_Atualizacoes_CORAC.Cursor = System.Windows.Forms.Cursors.Default;
            this.picture_Atualizacoes_CORAC.ErrorImage = null;
            this.picture_Atualizacoes_CORAC.Image = global::CORAC.Properties.Resources.Update_System_126px;
            this.picture_Atualizacoes_CORAC.InitialImage = null;
            this.picture_Atualizacoes_CORAC.Location = new System.Drawing.Point(67, 39);
            this.picture_Atualizacoes_CORAC.Name = "picture_Atualizacoes_CORAC";
            this.picture_Atualizacoes_CORAC.Size = new System.Drawing.Size(134, 131);
            this.picture_Atualizacoes_CORAC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture_Atualizacoes_CORAC.TabIndex = 3;
            this.picture_Atualizacoes_CORAC.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = global::CORAC.Properties.Resources.Banco_Dados_256px;
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(75, 50);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(101, 131);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::CORAC.Properties.Resources.Registro_256px;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(94, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(134, 131);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox6.Image = global::CORAC.Properties.Resources.Config_Servico_BD_CORAC_256px;
            this.pictureBox6.Location = new System.Drawing.Point(10, 396);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(70, 71);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 6;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox5.Image = global::CORAC.Properties.Resources.Tipo_Autenticacao;
            this.pictureBox5.Location = new System.Drawing.Point(3, 153);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(85, 63);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 5;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox3.Image = global::CORAC.Properties.Resources.Config_Update_CORAC_128px_fw;
            this.pictureBox3.Location = new System.Drawing.Point(3, 20);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(85, 83);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox6_HTTP
            // 
            this.pictureBox6_HTTP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox6_HTTP.Image = global::CORAC.Properties.Resources.Conf_Autent_HTTP;
            this.pictureBox6_HTTP.Location = new System.Drawing.Point(490, 15);
            this.pictureBox6_HTTP.Name = "pictureBox6_HTTP";
            this.pictureBox6_HTTP.Size = new System.Drawing.Size(93, 84);
            this.pictureBox6_HTTP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6_HTTP.TabIndex = 4;
            this.pictureBox6_HTTP.TabStop = false;
            // 
            // pictureBox5_LDAP
            // 
            this.pictureBox5_LDAP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox5_LDAP.Image = global::CORAC.Properties.Resources.LDAP_CORAC;
            this.pictureBox5_LDAP.Location = new System.Drawing.Point(114, 21);
            this.pictureBox5_LDAP.Name = "pictureBox5_LDAP";
            this.pictureBox5_LDAP.Size = new System.Drawing.Size(73, 65);
            this.pictureBox5_LDAP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5_LDAP.TabIndex = 3;
            this.pictureBox5_LDAP.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox4.Image = global::CORAC.Properties.Resources.Config_Userassword_256px;
            this.pictureBox4.Location = new System.Drawing.Point(10, 272);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(70, 71);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label3.Location = new System.Drawing.Point(30, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Caminho:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(463, 37);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(97, 40);
            this.button5.TabIndex = 3;
            this.button5.Text = "Verificar";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(392, 40);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(97, 40);
            this.button6.TabIndex = 5;
            this.button6.Text = "Verificar";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(634, 40);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(97, 40);
            this.button7.TabIndex = 6;
            this.button7.Text = "Autenticar";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // CORAC_TPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 549);
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
            this.Layout_Organizacao_CORAC.ResumeLayout(false);
            this.Internet_Status_GroupBox.ResumeLayout(false);
            this.Atualizacao_Status_GoupBox.ResumeLayout(false);
            this.Banco_Dados_Status_GroupBox.ResumeLayout(false);
            this.Registro_Status_GroupBox.ResumeLayout(false);
            this.Config.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Internet_CORAC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture_Atualizacoes_CORAC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6_HTTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5_LDAP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCORAC;
        private System.Windows.Forms.TabPage Tab_Status;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TabPage Config;
        private System.Windows.Forms.TableLayoutPanel Layout_Organizacao_CORAC;
        private System.Windows.Forms.GroupBox Internet_Status_GroupBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox picture_Internet_CORAC;
        private System.Windows.Forms.GroupBox Atualizacao_Status_GoupBox;
        private System.Windows.Forms.PictureBox picture_Atualizacoes_CORAC;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox Registro_Status_GroupBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox Banco_Dados_Status_GroupBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_Atualizacao_Caminho;
        private System.Windows.Forms.Label Caminho_Serv_Atual_Corac;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox6_HTTP;
        private System.Windows.Forms.PictureBox pictureBox5_LDAP;
        private System.Windows.Forms.RadioButton radioButton_Autenticacao_WEB;
        private System.Windows.Forms.RadioButton radioButton_Autenticacao_LDAP;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_Credencial_Senha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Credencial_Usuario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox5_BD_CORAC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button5;
    }
}

