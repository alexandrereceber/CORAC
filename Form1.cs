using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC
{
    public partial class CORAC_TPrincipal : Form
    {
        public CORAC_TPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap Obj = (Bitmap)picture_Internet_CORAC.Image;
            for (var i = 0; i < Obj.Height; i++) 
            {
                for (var ii = 0; ii < Obj.Width; ii++)
                {
                   Color g = Obj.GetPixel(i, ii);
                    if(g.A == 255 && g.R==33 && g.G==117 && g.B == 170)
                    {
                        Obj.SetPixel(i, ii,Color.Red);
                    }
                }
            }
            picture_Internet_CORAC.Image = Obj;

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

        private void button8_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            

            Comp_Notication.ShowBalloonTip(1000,"dd","dd",ToolTipIcon.Error);
                        }
    }
}
