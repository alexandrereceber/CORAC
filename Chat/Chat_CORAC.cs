using ServerClienteOnline.Server;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        ControleSend Semafaro;

        public void Send_SCK_Listener(ref object WEB_SCK)
        {
            try
            {
                Dictionary<int, object> Objts = (Dictionary<int, object>)WEB_SCK;
                Objts.TryGetValue(0, out object AcessoRemoto_WBSCKT);
                Obter_Contexto_WEBSOCKET = (WebSocket)AcessoRemoto_WBSCKT;

                Objts.TryGetValue(1, out object AcessoRemoto_IAC);
                IAC = (HttpListenerContext)AcessoRemoto_IAC;

                Objts.TryGetValue(2, out object AcessoRemoto_SMF);
                Semafaro = (ControleSend)AcessoRemoto_SMF;
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
        }

        private void Chat_CORAC_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
        }

        private void Chat_CORAC_Leave(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private async void pictureBox1_Click(object sender, EventArgs e)
        {
            Pacote_Comando Pacote = new Pacote_Comando();
            Pacote.Resposta = "ddd";

            if (Obter_Contexto_WEBSOCKET.State == WebSocketState.Open)
            {
                while (Semafaro.Semafaro) { }

                ArraySegment<byte> EnviandoDados = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote)));
                Semafaro.Semafaro = true;
                await Obter_Contexto_WEBSOCKET.SendAsync(EnviandoDados, WebSocketMessageType.Text, true, CancellationToken.None);
                Semafaro.Semafaro = false;

            }

        }
    }
}
