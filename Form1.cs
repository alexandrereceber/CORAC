using RegistroWindows;
using System;
using System.Windows.Forms;

namespace CORAC
{
    public partial class CORAC_TPrincipal : Form
    {
        public CORAC_TPrincipal()
        {
            RegistroWin32 ChavesCORAC = new RegistroWin32();
            if(!ChavesCORAC.Existe_Chave_CORAC())
            {
                if (!ChavesCORAC.Criar_Chaves_Campos_CORAC())
                {
                    MessageBox.Show("Não foi possível obter acesso ao registro. A aplicações está terminando!");
                    Application.Exit();
                }

            }
            else
            {
                //ChavesCORAC.LocalMachine_CamposChave();
            }


            InitializeComponent();

        }


        private void picture_Status_CORAC_MouseClick(object sender, MouseEventArgs e)
        {
            //int p = e.X;
            //int c = e.Y;
        }

        private void textBox_Atualizacao_Caminho_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Caminho do servidor para verificar se existe nova versão do sistema.";
        }

        private void textBox_LDAP_Caminho_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Caminho do servidor LDAP ou Active Directory";

        }

        private void textBox_Credencial_Usuario_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Nome de usuário que será utilizado para autenticação nos diversos servidores.";

        }

        private void textBox_Credencial_Senha_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Senha para autenticação do usuário.";

        }

        private void textBox5_BD_CORAC_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Endereço WEB do servidor de banco de dados do CORAC";

        }


        private void toolStripMenuItem1_MAN_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void CORAC_TPrincipal_Load(object sender, EventArgs e)
        {
            Data_Sistema_TLPrincipal.Text = DateTime.Now.Date.ToString();
            this.MaximizeBox = false;
            this.MinimizeBox = false;

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void Comp_Notication_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void CORAC_TPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else Notificacao.Visible = false;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

    //Bitmap Obj = (Bitmap)picture_Internet_CORAC.Image;
    //        for (var i = 0; i<Obj.Height; i++) 
    //        {
    //            for (var ii = 0; ii<Obj.Width; ii++)
    //            {
    //               Color g = Obj.GetPixel(i, ii);
    //                if(g.A == 255 && g.R==33 && g.G==117 && g.B == 170)
    //                {
    //                    Obj.SetPixel(i, ii, Color.Red);
    //                }
    //            }
    //        }
    //        picture_Internet_CORAC.Image = Obj;
}
