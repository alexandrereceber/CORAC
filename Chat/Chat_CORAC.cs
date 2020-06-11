using ServerClienteOnline.Interfaces;
using ServerClienteOnline.Server;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.WMIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC.Chat
{
    
    public partial class Chat_CORAC : Form
    {
        WebSocket Obter_Contexto_WEBSOCKET;
        HttpListenerContext IAC;
        public bool Semafaro = false;
        bool Cls_User = false;

        string UsuarioLogado = null;
        public HtmlDocument CHAT;

        delegate void Mensagem_Suporte(ITipoPacote Suporte);
        delegate void FecharCaixa();

        bool userClose = false;
        bool Enviar_PacoteDigitando = false;
        public bool Close_User()
        {
            return Cls_User;
        }
        public void Send_SCK_Listener(ref object WEB_SCK)
        {
            try
            {
                Dictionary<int, object> Objts = (Dictionary<int, object>)WEB_SCK;
                Objts.TryGetValue(0, out object AcessoRemoto_WBSCKT);
                Obter_Contexto_WEBSOCKET = (WebSocket)AcessoRemoto_WBSCKT;

                Objts.TryGetValue(1, out object AcessoRemoto_IAC);
                IAC = (HttpListenerContext)AcessoRemoto_IAC;
            }
            catch(Exception e)
            {
                Tratador_Erros TratarErr = new Tratador_Erros();
                TratarErr.SetTratador_Erros(TipoSaidaErros.Arquivo);
                TratarErr.TratadorErros(e, this.GetType().Name);
            }


        }
        public Chat_CORAC()
        {
            InitializeComponent();
            UsuarioLogado = ((string)Get_WMI.Obter_Atributo("Win32_ComputerSystem", "Username")).Replace("\\", "-");
        }

        private void Chat_CORAC_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            
        }

        private void Chat_CORAC_Leave(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Chat_CORAC_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private async void Chat_CORAC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(userClose == false)
            {
                DialogResult Resposta = MessageBox.Show("Tem certeza que deseja encerra o atendimento?", "Encerrar atendimento!.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                if (Resposta != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (Obter_Contexto_WEBSOCKET.State != WebSocketState.Aborted)
                    {
                        Cls_User = true;
                        Pacote_UserCloseDialog Fechar = new Pacote_UserCloseDialog();
                        Fechar.Close = Obter_Contexto_WEBSOCKET.State;
                        Fechar.Mensagem = "Close_User_Dialog";

                        ArraySegment<byte> DadosEnviando = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Fechar)));
                        await Obter_Contexto_WEBSOCKET.SendAsync(DadosEnviando, WebSocketMessageType.Text, true, CancellationToken.None);

                        Pacote_CloseConection Close = new Pacote_CloseConection();
                        Close.Close = Obter_Contexto_WEBSOCKET.State;
                        string StgFechamento = Converter_JSON_String.SerializarPacote(Close);

                        CancellationToken Token = new CancellationToken();
                        await Obter_Contexto_WEBSOCKET.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, StgFechamento, Token);
                    }

                }
            }
            

        }
        private async void enviarMensagem(ITipoPacote Pacote)
        {
            if(Pacote.GetTipoPacote() == TipoPacote.Chat_User)
            {
                Pacote_ChatUser Pacote_MSG = (Pacote_ChatUser)Pacote;

                if (Obter_Contexto_WEBSOCKET.State == WebSocketState.Open)
                {
                    while (Semafaro) { }
                    Semafaro = true;
                    ArraySegment<byte> EnviandoDados = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote_MSG)));
                    await Obter_Contexto_WEBSOCKET.SendAsync(EnviandoDados, WebSocketMessageType.Text, true, CancellationToken.None);
                    Semafaro = false;

                    HtmlElement ChatList = CHAT.GetElementById("Chat-List");
                    object[] Mensagem = new object[3];
                    Mensagem[0] = Pacote_MSG.Nome;
                    Mensagem[1] = Pacote_MSG.Mensagem;
                    Mensagem[2] = Pacote_MSG.Hora;
                    CHAT.InvokeScript("CriarMensagemUser", Mensagem);
                    MensagemEnviar.Clear();
                    CHAT.Body.ScrollIntoView(false);

                }
            }else if(Pacote.GetTipoPacote() == TipoPacote.Chat_Digitando)
            {
                Pacote_ChatDigitando Pacote_MSG = (Pacote_ChatDigitando)Pacote;
                while (Semafaro) { }
                Semafaro = true;
                ArraySegment<byte> EnviandoDados = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote_MSG)));
                await Obter_Contexto_WEBSOCKET.SendAsync(EnviandoDados, WebSocketMessageType.Text, true, CancellationToken.None);
                Semafaro = false;
            }

        }
        public void EscreverMensagem(ITipoPacote PCT)
        {
            object[] Mensagem;
            if (PCT.GetTipoPacote() == TipoPacote.Chat_Suporte)
            {

                Pacote_ChatSuporte Msm_Suporte = (Pacote_ChatSuporte)PCT;
                HtmlElement ChatList = CHAT.GetElementById("Chat-List");
                Mensagem = new object[3];
                Mensagem[0] = Msm_Suporte.Nome;
                Mensagem[1] = Msm_Suporte.Mensagem;
                Mensagem[2] = Msm_Suporte.Hora;

                CHAT.InvokeScript("CriarMensagemSuporte", Mensagem);
                CHAT.Body.ScrollIntoView(false);

            }
            else if (PCT.GetTipoPacote() == TipoPacote.Chat_Digitando)
            {
                Pacote_ChatDigitando VH = (Pacote_ChatDigitando)PCT;
                Mensagem = new object[1];
                Mensagem[0] = VH.Digitando;
                CHAT.InvokeScript("Chat_UserDigitando", Mensagem);
            }


        }
        public void ReceberMensagem(ITipoPacote PCT)
        {
            if (this.InvokeRequired)
            {
                Mensagem_Suporte Suporte = EscreverMensagem;
                this.Invoke(Suporte, PCT);
            }

        } 
        private void FecharCaixaDialog()
        {
            userClose = true;
            Close();
        }
        public void CloseCaixa()
        {
            FecharCaixa Fx = FecharCaixaDialog;
            Invoke(Fx);
        }
        
        private void Botao_Enviar_Mensagem_Click(object sender, EventArgs e)
        {
            if(MensagemEnviar.Text != "")
            {
                Pacote_ChatUser Pacote_MSG = new Pacote_ChatUser();
                Pacote_MSG.Nome = UsuarioLogado;
                Pacote_MSG.Mensagem = MensagemEnviar.Text;
                enviarMensagem(Pacote_MSG);
            }


        }

        private void MensagemEnviar_TextChanged(object sender, EventArgs e)
        {
            TextBox CxM = (TextBox)sender;

            if (CxM.Modified)
            {
                if(CxM.Text.Length == 0)
                {
                    if (Enviar_PacoteDigitando)
                    {
                        CxM.Modified = false;
                    }
                }
            }
        }

        private void MensagemEnviar_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (MensagemEnviar.Text != "")
                {
                    Pacote_ChatUser Pacote_MSG = new Pacote_ChatUser();
                    Pacote_MSG.Nome = UsuarioLogado;
                    Pacote_MSG.Mensagem = MensagemEnviar.Text;
                    enviarMensagem(Pacote_MSG);
                }
            }
        }

        private void webchat_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            CHAT = webchat.Document;
        }

        private void MensagemEnviar_ModifiedChanged(object sender, EventArgs e)
        {
            if (!Enviar_PacoteDigitando)
            {
                Enviar_PacoteDigitando = true;
                Pacote_ChatDigitando Digi = new Pacote_ChatDigitando();
                Digi.Digitando = true;

                enviarMensagem(Digi);
            }
            else
            {
                Enviar_PacoteDigitando = false;
                Pacote_ChatDigitando Digi = new Pacote_ChatDigitando();
                Digi.Digitando = false;
                
                enviarMensagem(Digi);

            }
        }
    }
}
