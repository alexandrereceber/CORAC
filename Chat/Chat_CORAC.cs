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

        Pacote_ChatUser Pacote_MSG = new Pacote_ChatUser();
        public HtmlDocument CHAT;

        delegate void Mensagem_Suporte(Pacote_ChatSuporte Suporte);
        delegate void FecharCaixa();
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
            Pacote_MSG.Nome = ((string)Get_WMI.Obter_Atributo("Win32_ComputerSystem", "Username")).Replace("\\", "-");
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
            DialogResult Resposta = MessageBox.Show("Tem certeza que deseja encerra o atendimento?", "Encerrar atendimento!.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            if (Resposta != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                if(Obter_Contexto_WEBSOCKET.State != WebSocketState.Aborted)
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
        private async void enviarMensagem()
        {
            if (MensagemEnviar.Text != "")
            {
                Pacote_MSG.Mensagem = MensagemEnviar.Text;

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
            }
        }
        public void EscreverMensagem(Pacote_ChatSuporte PCT)
        {
            HtmlElement ChatList = CHAT.GetElementById("Chat-List");
            object[] Mensagem = new object[3];
            Mensagem[0] = PCT.Nome;
            Mensagem[1] = PCT.Mensagem;
            Mensagem[2] = PCT.Hora;
            CHAT.InvokeScript("CriarMensagemSuporte", Mensagem);
            CHAT.Body.ScrollIntoView(false);

        }
        public void ReceberMensagem(Pacote_ChatSuporte PCT)
        {
            if (this.InvokeRequired)
            {
                Mensagem_Suporte Suporte = this.EscreverMensagem;
                this.Invoke(Suporte, PCT);
            }

        } 
        private void FecharCaixaDialog()
        {
            Close();
        }
        public void CloseCaixa()
        {
            FecharCaixa Fx = FecharCaixaDialog;
            Invoke(Fx);
        }
        
        private void Botao_Enviar_Mensagem_Click(object sender, EventArgs e)
        {
            enviarMensagem();

            //ChatList.InnerHtml += "<li class=\"chat - item\"><div class=\"chat-img\"><img src=\"http://192.168.15.10/CORAC/Imagens/Chat/chat_user.png\" alt=\"Usuário\"></div><div class=\"chat-content\"><h6 class=\"font-medium\">alex</h6><div class=\"box bg-light-info\">oi</div></div><div class=\"chat-time\">10</div></li>";
        }

        private void MensagemEnviar_TextChanged(object sender, EventArgs e)
        {

        }

        private void MensagemEnviar_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                enviarMensagem();
            }
        }

        private void webchat_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            

        }

        private void webchat_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            CHAT = webchat.Document;
        }

    }
}
