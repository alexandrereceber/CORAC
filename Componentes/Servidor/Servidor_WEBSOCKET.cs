using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace ServerClienteOnline.Server
{
    class AcessoRemoto
    {

        public async void Iniciar_AcessoRemoto(object ObjAC)
        {
            var gg = 0;
        }
    }

    class Servidor_WEBSOCKET : Tratador_Erros, IDisposable, IServidor
    {
        HttpListener Servidor;
        private bool Active = false;

        public __Autenticacao TEndPoint { get; set; }
        List<string> Prefixos = new List<string>();

        private IAuthHTML _Auth;
        private IGClienteHTML _GerenciadorCliente;

        AcessoRemoto ControleAC = new AcessoRemoto();

        Pacote_Base Base;
        public Servidor_WEBSOCKET()
        {
            try
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }
        }

        /**
            <summary>
                Verifica se os serviços auxiliares estão configurados.
            </summary>
         */
        private void Checked_Servicos_Auxiliares()
        {
            try
            {
                //if (_CMDs == null) throw new Exception("Nenhum tratador de comados foi identificado.");
                // if (_Auth == null) throw new Exception("Nenhum processador de autenticação foi identificado.");
                //if (_GerenciadorCliente == null) throw new Exception("Nenhum gerenciado de processo foi identificado.");
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }

        }


        /**
          * Data: 20/04/2019
          * Propriedade que atribui uma classe que executará comandos e retornará uma reposta.
          * Return: IGCliente
          */
        public IGClienteHTML Gerenciador_Cliente
        {
            set { _GerenciadorCliente = value; }
        }

        /**
         * <summary>
         * 
         * </summary>
         */
        public void AddPrefixos(string Protocolo = "http", string Dominio = "*", string Pasta = null, int PORT = 80)
        {
            Protocolo = Protocolo == null ? "http" : Protocolo;
            Dominio = Dominio == null ? "*" : Dominio;
            Pasta = Pasta == null ? "" : Pasta;

            Prefixos.Add(Protocolo + "://" + Dominio + ":" + PORT + "/" + Pasta);
        }

        private void PrefixoServidor()
        {
            if (Prefixos.Count == 0) throw new Exception("Não foram definidos nenhum prefixo.");

            foreach (var i in Prefixos)
            {
                Servidor.Prefixes.Add(i);
            }
        }

        /*Dá início ao servidor*/
        public bool StartServidor()
        {
            try
            {
                Checked_Servicos_Auxiliares();

                Servidor = new HttpListener();
                PrefixoServidor();
                Servidor.Start();

                IAsyncResult AceitarCliente = Servidor.BeginGetContext(IniciarConversa, Servidor);

                return true; ;
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return false;
            }

        }

        /**
          * Data: 27/03/2019
          * Dá início à comunicação, criando uma thread para cada cliente
          * Return: string
          */
        private void IniciarConversa(IAsyncResult s)
        {
            Pacote_Error P_Error;

            Thread AcessoRemoto, Chat;
            try
            {

                HttpListener Server = (HttpListener)s.AsyncState;
                HttpListenerContext Aceitar = Server.EndGetContext(s);

                HttpListenerRequest ObterConteudo = Aceitar.Request;
                StreamReader _Conteudo = new StreamReader(ObterConteudo.InputStream);

                HttpListenerResponse ObterResposta = Aceitar.Response;

                /**
                 * Coloca o servidor para receber outras conexões.
                 */
                Server.BeginGetContext(IniciarConversa, Server);

                //try
                //{


                //    string __Conteudo = _Conteudo.ReadLine();
                //    /*-----------------------Processamento----------------------------*/

                //    dynamic QTP = DeserializarPacote(__Conteudo);
                //    switch (Base.Pacote)
                //    {
                //        case TipoPacote.AcessoRemoto:
                //            Pacote_AcessoRemoto CMM = (Pacote_AcessoRemoto)QTP; //Transforma string em um objeto da classe Pacote_Auth

                //            //_GerenciadorCliente?._OAuth(CMM.Chave);

                //            bool _Autenticado = (bool)_Auth.HTML_Autenticado(CMM.Chave);

                //            if (!_Autenticado) throw new Exception("Usuário não autenticado ou bloqueado.");

                //            /**
                //             * Lembrar de realizar uma alteração no pacote de autenticação que indique que método está requisitando-a
                //             */
                //            _GerenciadorCliente?.ConectarCliente(Aceitar.Request.RemoteEndPoint, _Auth.GetAutenticacao);

                //            /*
                //             * Requisita liberação de acesso à visualização da tela.
                //             */

                //            //Pacote_AcessoRemoto PCT = new Pacote_AcessoRemoto();
                //            //PCT.Resposta = "";
                //            //string DadosPacote = SerializarPacote(PCT);
                //            //Obtém o Barramento de escrita com a cliente
                //            //ObterResposta.ContentLength64 = DadosPacote.Length;
                //            //ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                //            //ObterResposta.Close();

                //            break;

                //        case TipoPacote.Echo:
                //            Pacote_PingReplay RPY = new Pacote_PingReplay();
                //            DadosPacote = SerializarPacote(RPY);
                //            //Obtém o Barramento de escrita com a cliente
                //            ObterResposta.ContentLength64 = DadosPacote.Length;
                //            ObterResposta.OutputStream.Write(ASCIIEncoding.Unicode.GetBytes(DadosPacote), 0, DadosPacote.Length);
                //            ObterResposta.Close();
                //            break;


                //        default:
                //            P_Error = new Pacote_Error();
                //            P_Error.Error = true;
                //            P_Error.Mensagem = "Esse tipo de pacote não é permitido.";
                //            P_Error.Numero = DadosExcecao.HResult;

                //            DadosPacote = SerializarPacote(P_Error);
                //            ObterResposta.ContentLength64 = DadosPacote.Length;
                //            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                //            ObterResposta.Close();
                //            break;
                //    }

                //}
                //catch (Exception e)
                //{
                //    TratadorErros(e, this.GetType().Name);

                //    P_Error = new Pacote_Error();
                //    P_Error.Error = Excecao;
                //    P_Error.Mensagem = DadosExcecao.Message;
                //    P_Error.Numero = DadosExcecao.HResult;

                //    string Erros = SerializarPacote(P_Error);
                //    ObterResposta.ContentLength64 = Erros.Length;
                //    ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(Erros), 0, Erros.Length);
                //    ObterResposta.Close();

                //    Excecao = false;
                //    DadosExcecao = null;
                //}

                //Uri URi = Aceitar.Request.Url;
                //switch (URi.LocalPath)
                //{
                //    case "/AcessoRemoto/":
                //        //Thread AC = new Thread(ControleAC.Iniciar_AcessoRemoto);
                //        //AC.Name = "AcessoRemoto";
                //        //AC.Start(Aceitar);
                //        break;

                //    default:

                //        break;
                //}

                /**
                 * Coloca o servidor em listen novamente;
                 */

                //HttpListenerWebSocketContext WebSocket_CORAC =  await aceita.AcceptWebSocketAsync(null);


                //WebSocket Obter_Contexto_WEBSOCKET = WebSocket_CORAC.WebSocket;


                //ArraySegment<byte> DadosRecebendo = new ArraySegment<byte>(new byte[5]);
                // WebSocketReceiveResult Resultado_WS = await Obter_Contexto_WEBSOCKET.ReceiveAsync(DadosRecebendo, new CancellationToken());

                //while (Obter_Contexto_WEBSOCKET.State == WebSocketState.Open)
                //{
                //    var sf = Screen.AllScreens;
                //    Bitmap printscreen = new Bitmap(sf[0].Bounds.Width, sf[0].Bounds.Height);

                //    Size ss = new Size(sf[0].Bounds.Width, sf[1].Bounds.Height);
                //    Graphics tela = Graphics.FromImage(printscreen);
                //    tela.CopyFromScreen(0, 0, 0, 0, ss);
                //    //pictureBox1.Image = printscreen;

                //    MemoryStream Img = new MemoryStream();

                //    printscreen.Save(Img, ImageFormat.Png);
                //    byte[] trans = Img.ToArray();
                //    string kk = Convert.ToBase64String(trans);
                //    ArraySegment<byte> DadosEnviando = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(kk));

                //    await Obter_Contexto_WEBSOCKET.SendAsync(DadosEnviando, WebSocketMessageType.Text, true, CancellationToken.None);

                //}
                /*Inicia novamente o estado de escuta com o fim de ouvir outros clientes*/

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }


        }

        private void Tratar_AcessoRemoto()
        {

        }

        /**
          * Data: 02/04/2019
          * Propriedade que atribui uma classe para autenticação de máquinas.
          * Return: IAuth
          */
        public IAuthHTML Autenticador
        {
            set { _Auth = value; }
        }

        /**
       * Data: 22/03/2019
       * Transforma o pacote em string.
       * Return: string
       */
        public string SerializarPacote(ITipoPacote Conteudo)
        {
            try
            {
                string SubPct = JsonConvert.SerializeObject(Conteudo);

                Pacote_Base PctBase = new Pacote_Base();
                PctBase.Pacote = Conteudo.GetTipoPacote();
                PctBase.Conteudo = SubPct;

                string SerializarPacote = JsonConvert.SerializeObject(PctBase);
                return SerializarPacote + "     ";

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name); ;
                return null;
            }

        }

        /**
           * Data: 02/04/2019
           * Transforma uma string em pacote para acesso.
           * Return: string
           */
        private dynamic DeserializarPacote(string Pacote)
        {
            try
            {

                Base = JsonConvert.DeserializeObject<Pacote_Base>(Pacote);
                switch (Base.Pacote)
                {
                    case TipoPacote.Auth:
                        Pacote_Auth Auth = JsonConvert.DeserializeObject<Pacote_Auth>(Base.Conteudo);
                        return Auth;

                    case TipoPacote.Comando:
                        Pacote_Comando Exec = JsonConvert.DeserializeObject<Pacote_Comando>(Base.Conteudo);
                        return Exec;

                    case TipoPacote.File:
                        Pacote_File File = JsonConvert.DeserializeObject<Pacote_File>(Base.Conteudo);
                        return File;

                    case TipoPacote.FileSystem:
                        Pacote_SystemFile FileSystem = JsonConvert.DeserializeObject<Pacote_SystemFile>(Base.Conteudo);
                        return FileSystem;

                    case TipoPacote.Echo:
                        Pacote_PingEcho Ping = JsonConvert.DeserializeObject<Pacote_PingEcho>(Base.Conteudo);
                        return Ping;

                    case TipoPacote.Replay:
                        Pacote_PingReplay Replay = JsonConvert.DeserializeObject<Pacote_PingReplay>(Base.Conteudo);
                        return Replay;

                    case TipoPacote.Inicializacao:
                        Pacote_Inicializacao Inicializacao = JsonConvert.DeserializeObject<Pacote_Inicializacao>(Base.Conteudo);
                        return Inicializacao;


                    default:
                        throw new Exception("Tentativa de envio de pacote não reconhecida pelo sistema.");

                }
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name); ;
                return false;
            }


        }


        public bool StopServidor()
        {
            try { Servidor.Stop(); Active = false; return true; } catch (Exception e) { TratadorErros(e, this.GetType().Name); Active = false; return false; }

        }

        public bool StatusServidor()
        {
            return Active;
        }
    }
}
