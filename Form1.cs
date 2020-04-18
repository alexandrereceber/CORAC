using RegistroWindows;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ServerClienteOnline.Utilidades;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ServerClienteOnline.Conectividade;
using System.Drawing;
using System.Threading.Tasks;

namespace CORAC
{

    public partial class CORAC_TPrincipal : Form
    {
        RegistroWin32 ChavesCORAC;
        List<KeyValuePair<string, string>> KeysValues;

        private bool ArmazenarAlteracoesCampos(string Chave, string Valor)
        {
            int count = 0;
            try
            {
                if (KeysValues == null) KeysValues = new List<KeyValuePair<string, string>>();
                if(KeysValues.Count > 0)
                {
                    foreach(KeyValuePair<string, string> ParChvVlr in KeysValues)
                    {
                        if(ParChvVlr.Key == Chave)
                        {
                            if (ParChvVlr.Value == Valor) return true;
                            KeysValues.RemoveAt(count);
                            KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                            return true;
                        }
                        count++;
                    }
                    KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                }
                else
                {
                    KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                }
            }catch(Exception E)
            {
                return false;
            }


            return true;

        }

        private void ObterConfiguracoes()
        {
            ChavesCORAC = new RegistroWin32();
            ChavesCORAC.SetTratador_Erros(TipoSaidaErros.ComponenteAndFile);
            ChavesCORAC.Componente_Log = webBrowser_Log;

            if (!ChavesCORAC.Existe_Chave_CORAC())
            {
                if (!ChavesCORAC.Criar_Chaves_Campos_CORAC())
                {
                    MessageBox.Show("Não foi possível obter acesso ao registro. A aplicações está terminando!");
                    Application.Exit();
                }
                else
                {
                    ChavesCORAC.LocalMachine_CamposChave("software\\CORAC");
                }

            }
            else
            {
                ChavesCORAC.LocalMachine_CamposChave("software\\CORAC");
            }
        }

        private void CheckComponentes()
        {
            try
            {

            }catch(Exception e)
            {
                webBrowser_Log.DocumentText += e.Message;

            }
        }

        /**
         * <summary>
             Alterar a cor de uma imagem para outra.
             <list type="number">
                 <item><paramref name="Caixa_Imagem_Status"/>: Picture onde está localizada a imagem;</item>
                 <item><paramref name="Cor"/>: Cor que deseja localizar para substituição;</item>
                 <item><paramref name="Substituir_Cor"/>: Cor para substituição;</item>
             </list>
          </summary>
         */
        private Bitmap Change_Color(Image Imagems, Color Cor, Color Substituir_Cor)
        {
            try
            {
                Bitmap Obj = (Bitmap)Imagems;
                for (var ii = 0; ii < Obj.Height; ii++)
                {
                    for (var i = 0; i < Obj.Width; i++)
                    {
                        Color g = Obj.GetPixel(i, ii);
                        if (g.A == Cor.A && g.R == Cor.R && g.G == Cor.G && g.B == Cor.B)
                        {
                            Obj.SetPixel(i, ii, Substituir_Cor);
                        }
                    }
                }
                
                return Obj;
            }
            catch (Exception e)
            {
                webBrowser_Log.DocumentText += e.Message;
                return null;
            }


        }

        /**
         * <summary>
            Verifica se existe saída para internet.
         * </summary>
         */
        private async Task<bool> Verirficar_Conectividade()
        {

            Image CopiaImagem = picture_Internet_Status.Image;
            picture_Internet_Status.SizeMode = PictureBoxSizeMode.CenterImage;
            picture_Internet_Status.Image = Properties.Resources.Wait;

            if (await Conexoes.VerificarConectividade())
            {
                Color Vermelho = Color.FromArgb(255, 255, 0, 0);
                Color Azul = Color.FromArgb(255, 0, 1, 255);
                Bitmap Internet_ON = Change_Color(CopiaImagem, Vermelho, Azul);
                picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Internet_Status.Image = Internet_ON;
                return true;
            }
            else
            {
                Color Vermelho = Color.FromArgb(255, 255, 0, 0);
                Color Azul = Color.FromArgb(255, 0, 1, 255);
                Bitmap Internet_ON = Change_Color(CopiaImagem, Azul, Vermelho);
                picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Internet_Status.Image = Internet_ON;
                return false;
            }


        }

        /**
         * <summary>
            Verifica se existe atualizações do sistema CORAC, esta verificação é realizada através da internet em um site próprio.
         * </summary>
         */
        private async Task<bool> Verificar_Atualizacoes()
        {
            Image CopiaImagem = picture_Atualizacoes_CORAC.Image;

            Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            Color Azul = Color.FromArgb(255, 0, 1, 255);

            try
            {
                picture_Atualizacoes_CORAC.Image = Properties.Resources.Wait;
                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_Update_CORAC"));
                WebRequest HTTP_CORAC = WebRequest.CreateHttp(EndURI);
                HTTP_CORAC.Method = "POST";
                HTTP_CORAC.ContentType = "application/x-www-form-urlencoded";
                HTTP_CORAC.Timeout = 3;
                Stream dataStream = await HTTP_CORAC.GetRequestStreamAsync();

                /**
                 * Falta implementar mais  
                 */


                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;


                Bitmap Internet_ON = Change_Color(CopiaImagem, Vermelho, Azul);
                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Atualizacoes_CORAC.Image = Internet_ON;

                return true;
            }
            catch (Exception E)
            {
                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                Bitmap Internet_ON = Change_Color(CopiaImagem, Azul, Vermelho);

                picture_Atualizacoes_CORAC.Image = Internet_ON;
                
                webBrowser_Log.DocumentText += E.Message;
                
                return false;
            }
        }

        /**
         * <summary>
            Verifica se o software CORAC está licenciado. Esta verificação ocorre através da internet em umm site próprio do CORAC.
         * </summary>
         */
        private async Task<bool> Verificar_Registro()
        {
            Image CopiaImagem = pictureBox_Registro_CORAC.Image;

            Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            Color Azul = Color.FromArgb(255, 0, 1, 255);

            try
            {
                pictureBox_Registro_CORAC.Image = Properties.Resources.Wait;
                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

                Uri EndURI = new Uri("http://192.168.15.4/CORAC/REGISTRO");
                WebRequest HTTP_CORAC = WebRequest.CreateHttp(EndURI);
                HTTP_CORAC.Method = "POST";
                HTTP_CORAC.ContentType = "application/x-www-form-urlencoded";
                HTTP_CORAC.Timeout = 3;
                Stream dataStream = await HTTP_CORAC.GetRequestStreamAsync();

                /**
                 * Falta implementar mais complexa envolvendo o Banco de dados 
                 */


                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;


                Bitmap Internet_ON = Change_Color(CopiaImagem, Vermelho, Azul);
                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox_Registro_CORAC.Image = Internet_ON;

                return true;
            }
            catch (Exception E)
            {
                Bitmap Internet_ON = Change_Color(CopiaImagem, Azul , Vermelho);
                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox_Registro_CORAC.Image = Internet_ON;
                
                webBrowser_Log.DocumentText += E.Message;

                return false;
            }
        }

        /**
         * <summary>
            Verifica se o software CORAC está licenciado. Esta verificação ocorre através da internet em umm site próprio do CORAC.
         * </summary>
         */
        private async Task<bool> Verificar_Servidor_CORAC()
        {
            Image CopiaImagem = pictureBox_Servidor_CORAC.Image;
            pictureBox_Servidor_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

            Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            Color Azul = Color.FromArgb(255, 0, 1, 255);
            try
            {
                pictureBox_Servidor_CORAC.Image = Properties.Resources.Wait;
                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
                WebRequest HTTP_CORAC = WebRequest.CreateHttp(EndURI);
                HTTP_CORAC.Method = "POST";
                HTTP_CORAC.ContentType = "application/x-www-form-urlencoded";
                HTTP_CORAC.Timeout = 3;
                WebResponse DadosStream = await HTTP_CORAC.GetResponseAsync();
                Stream dataStream = DadosStream.GetResponseStream();

                char[] Dados_CORAC_Signature = new char[67];

                StreamReader LerDados_CORAC = new StreamReader(dataStream);
                await LerDados_CORAC.ReadAsync(Dados_CORAC_Signature, 0, 67);
                byte[] D = ASCIIEncoding.UTF8.GetBytes(Dados_CORAC_Signature);
                string Dados = ASCIIEncoding.UTF8.GetString(D);
                Assinatura Sign = JsonConvert.DeserializeObject<Assinatura>(Dados);

                pictureBox_Servidor_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;


                if (Sign.Sistema == "CORAC" && Sign.Signacture == "a4b315c63dca8337dc70ef6a336310f4")
                {
                    Bitmap Internet_ON = Change_Color(CopiaImagem, Vermelho, Azul);
                    pictureBox_Servidor_CORAC.Image = Internet_ON;

                    return true;
                }
                else
                {
                    Bitmap Internet_ON = Change_Color(CopiaImagem, Azul, Vermelho);
                    pictureBox_Servidor_CORAC.Image = Internet_ON;
                    return false;
                }


            }
            catch (Exception E)
            {
                pictureBox_Servidor_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                Bitmap Internet_ON = Change_Color(CopiaImagem, Azul, Vermelho);
                pictureBox_Servidor_CORAC.Image = Internet_ON;
                
                webBrowser_Log.DocumentText += E.Message;

                return true;

            }
        }

        private async Task<bool> Loaders()
        {
            if (await Verirficar_Conectividade())
            {
                Task Atualiza = Task.Run(Verificar_Atualizacoes);
                Task Registro = Task.Run(Verificar_Registro);
                return true;
            }
            else
            {
                return false;
            }
        }

        public CORAC_TPrincipal()
        {
            ObterConfiguracoes();

            InitializeComponent();

            Loaders();

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
            Status_Informacao.Text = "Endereço WEB do servidor de banco de dados do CORAC.";

        }


        private void toolStripMenuItem1_MAN_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            this.Show();
        }

        private void CORAC_TPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                Data_Sistema_TLPrincipal.Text = DateTime.Now.Date.ToString();
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Text_Box_Path_Update_CORAC.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_Update_CORAC");
                
                bool AutenticLDAP = Convert.ToBoolean(ChavesCORAC.Obter_ConteudoCampo("LDAP_Type_Autentication"));
                if (AutenticLDAP)
                {
                    textBox_Path_Type_AutenticationLDAP.Enabled = true;
                    radioButton_LDAP_Type_Autentication.Checked = true;
                }
                else
                {
                    textBox_Path_Type_AutenticationLDAP.Enabled = false;
                }
                bool AutenticWEB = Convert.ToBoolean(ChavesCORAC.Obter_ConteudoCampo("WEB_Type_Autentication"));
                if (AutenticWEB)
                {
                    textBox_Path_Type_AutenticationLDAP.Enabled = false;
                    radioButton_WEB_Type_Autentication.Checked = true;
                }
                else
                {

                }

                textBox_Path_Type_AutenticationLDAP.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_Type_AutenticationLDAP");
                textBox_Username.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Username");
                textBox_Password.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Password");
                textBox_Path_ServerWEB_CORAC.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                textBox_Path_ServerIP_CORAC.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerIP_CORAC");
                textBox_Path_ServerPorta_CORAC.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerPorta_CORAC");
                textBox_Path_ServerIP_AR.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerIP_AR");
                textBox_Path_ServerPorta_AR.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerPorta_AR");
            }
            catch(Exception E)
            {
                webBrowser_Log.DocumentText += E.Message;
            }


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

        private void SalvaConfiguracoes_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult Resposta = MessageBox.Show("Tem certeza que deseja realizar essa operação?", "Salvar configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Resposta == DialogResult.Yes)
                {
                    if (KeysValues == null) MessageBox.Show("Nenhuma alteração foi identificada.", "Alterações");
                    if (KeysValues.Count > 0)
                    {
                        if(ChavesCORAC.Gravar_ConteudoCampo(TipoChave.LocalMachine, "software\\CORAC", ref KeysValues))
                        {
                            MessageBox.Show("O dados foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("O dados não foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //KeysValues.Clear();
                    }
                }
            }
            catch (Exception E)
            {
                webBrowser_Log.DocumentText = E.Message;
            }
            
        }

        private void Text_Box_Path_Update_CORAC_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerPorta_CORAC_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_Path_Type_AutenticationLDAP_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Username_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Password_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerWEB_CORAC_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerIP_CORAC_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerPorta_CORAC_Leave(object sender, EventArgs e)
        {

            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerIP_AR_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void textBox_Path_ServerPorta_AR_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }

        private void radioButton_LDAP_Type_Autentication_Click(object sender, EventArgs e)
        {
            RadioButton T = sender as RadioButton;

            if (T.Checked)
            {
                textBox_Path_Type_AutenticationLDAP.Enabled = false;
                textBox_Path_Type_AutenticationLDAP.Clear();
                ArmazenarAlteracoesCampos((string)T.Tag, Convert.ToString(T.Checked));
                ArmazenarAlteracoesCampos("LDAP_Type_Autentication", "False");
            }
        }

        private void radioButton_WEB_Type_Autentication_Click(object sender, EventArgs e)
        {
            textBox_Path_Type_AutenticationLDAP.Clear();
            textBox_Path_Type_AutenticationLDAP.Enabled = false;
            Status_Informacao.Text = "Autenticação WEB. Utiliza o endereço do servidor CORAC WEB.";

        }

        private void textBox_Path_ServerIP_CORAC_Enter(object sender, EventArgs e)
        {
            textBox_Path_Type_AutenticationLDAP.Enabled = true;
            Status_Informacao.Text = "Autenticação LDAP.";

        }

        private void textBox_Path_ServerPorta_CORAC_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Porta do servidor local CORAC.";

        }

        private void textBox_Path_ServerIP_AR_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Endereço do servidor local de acesso remoto.";

        }

        private void textBox_Path_ServerPorta_AR_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Porta do servidor local de acesso remoto.";

        }

        private void radioButton_LDAP_Type_Autentication_Leave(object sender, EventArgs e)
        {

        }

        private void textBox_Path_ServerPorta_CORAC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox T = sender as TextBox;

            decimal Porta;
            if (decimal.TryParse(T.Text, out Porta))
            {
                if (!(Porta > 1000 && Porta < 65535))
                {
                    T.Clear();
                    MessageBox.Show("O valor da porta deve estar entre 1000 e 65535", "Porta inválida!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            else
            {
                T.Clear();
                MessageBox.Show("O valor digitado não representa um número de porta válida!", "Porta inválida!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void textBox_Path_ServerPorta_CORAC_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox_Path_ServerPorta_AR_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox T = sender as TextBox;

            decimal Porta;
            if (decimal.TryParse(T.Text, out Porta))
            {
                if (!(Porta > 1000 && Porta < 65535))
                {
                    T.Clear();
                    MessageBox.Show("O valor da porta deve estar entre 1000 e 65535.","Porta inválida!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            else
            {
                T.Clear();
                MessageBox.Show("O valor digitado não representa um número de porta válida!", "Porta inválida!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void radioButton_LDAP_Type_Autentication_Click_1(object sender, EventArgs e)
        {
            RadioButton T = sender as RadioButton;

            if (T.Checked)
            {
                textBox_Path_Type_AutenticationLDAP.Enabled = true;
                ArmazenarAlteracoesCampos((string)T.Tag, Convert.ToString(T.Checked));
                ArmazenarAlteracoesCampos("LDAP_Type_Autentication", "True");
            }
        }

        private void Text_Box_Path_Update_CORAC_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void Text_Box_Path_Update_CORAC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox T = (TextBox)sender;
            if (T.Modified)
            {
                Regex CampoCORACAtualiza = new Regex("^http[s]?://");
                if (!CampoCORACAtualiza.IsMatch(T.Text))
                {
                    e.Cancel = true;
                    MessageBox.Show("O campo precisa iniciar com http(s)://", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
        }

        private async void button_Verificar_Atualizacao_CORAC_Click(object sender, EventArgs e)
        {
            if (Text_Box_Path_Update_CORAC.Text.Length > 0)
            {
                try
                {
                    pictureBox_Atualizacao_CORAC.Image = Properties.Resources.Wait;
                    Uri EndURI = new Uri(Text_Box_Path_Update_CORAC.Text);
                    WebRequest HTTP_CORAC = WebRequest.CreateHttp(EndURI);
                    HTTP_CORAC.Method = "POST";
                    HTTP_CORAC.ContentType = "application/x-www-form-urlencoded";
                    HTTP_CORAC.Timeout = 3;
                    Stream dataStream = await HTTP_CORAC.GetRequestStreamAsync();
                    
                    pictureBox_Atualizacao_CORAC.Image = Properties.Resources.Acepty;

                }
                catch (Exception E)
                {
                    pictureBox_Atualizacao_CORAC.Image = Properties.Resources.No_Acepty;

                }
            }
            else
            {
                MessageBox.Show("Endereço inválido!", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private async void button_Servidor_WEB_Click(object sender, EventArgs e)
        {
            if (textBox_Path_ServerWEB_CORAC.Text.Length > 0)
            {
                try
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.Wait;
                    Uri EndURI = new Uri(textBox_Path_ServerWEB_CORAC.Text);
                    WebRequest HTTP_CORAC = WebRequest.CreateHttp(EndURI);
                    HTTP_CORAC.Method = "POST";
                    HTTP_CORAC.ContentType = "application/x-www-form-urlencoded";
                    HTTP_CORAC.Timeout = 3;
                    WebResponse DadosStream = await HTTP_CORAC.GetResponseAsync();
                    Stream dataStream = DadosStream.GetResponseStream();

                    char[] Dados_CORAC_Signature = new char[67];

                    StreamReader LerDados_CORAC = new StreamReader(dataStream);
                    await LerDados_CORAC.ReadAsync(Dados_CORAC_Signature, 0, 67);
                    byte[] D = ASCIIEncoding.UTF8.GetBytes(Dados_CORAC_Signature);
                    string Dados = ASCIIEncoding.UTF8.GetString(D);
                    Assinatura Sign = JsonConvert.DeserializeObject<Assinatura>(Dados);
                    if(Sign.Sistema =="CORAC" && Sign.Signacture == "a4b315c63dca8337dc70ef6a336310f4")
                    {
                        pictureBox_Servidor_WEB.Image = Properties.Resources.Acepty;
                    }
                    else
                    {
                        pictureBox_Servidor_WEB.Image = Properties.Resources.No_Acepty;
                    }
                    
                    
                }
                catch (Exception E)
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.No_Acepty;

                }
            }
            else
            {
                MessageBox.Show("Endereço inválido!", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void textBox_Path_ServerWEB_CORAC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox T = (TextBox)sender;
            if (T.Modified)
            {
                Regex CampoCORACAtualiza = new Regex("^http[s]?://");
                if (!CampoCORACAtualiza.IsMatch(T.Text))
                {
                    e.Cancel = true;
                    MessageBox.Show("O campo precisa iniciar com http(s)://", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
        }

        private void picture_Internet_Status_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void picture_Internet_Status_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private async void button_VerificarInternet_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verirficar_Conectividade();
            }finally
            {
                B.Enabled = true;
            }
        }

        private async void button_AtualizacoesCORAC_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Atualizacoes();
            }finally
            {
                B.Enabled = true;
            }
        }

        private async void button_RegistroMaquina_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Registro();
            }
            finally
            {
                B.Enabled = true;
            }
        }

        private async void button_Server_WEB_CORAC_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Servidor_CORAC();
            }
            finally
            {
                B.Enabled = true;
            }
        }
    }


    class Assinatura
    {
        [JsonProperty("Sistema")]
        public string Sistema { get; set; }

        [JsonProperty("Signacture")]
        public string Signacture { get; set; }
    }


}
