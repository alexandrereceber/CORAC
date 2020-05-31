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
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace ServerClienteOnline.Server
{
    class AcessoRemoto_WEBSOCKET: Tratador_Erros
    {
        private IGClienteHTML _GerenciadorCliente;
        private IAuthHTML _Auth;
        private int Buffer_Server { get; set; }

        List<int> Lista_Conexoes_WEBSOCKET = new List<int>();

        private bool Semafaro = false;
        private bool Gerente_WEBSOCKET(int IP)
        {
            
            if(Lista_Conexoes_WEBSOCKET.Count > 0)
            {
                if (Lista_Conexoes_WEBSOCKET.IndexOf(IP) >= 0) return true; //Continuar conexão.

                return false; //Já existe uma conexão com esse ip, favor desconectar e tentat novamente
            }
            else
            {
                Lista_Conexoes_WEBSOCKET.Add(IP);

                return false; //Continuar conexão.
            }
        }

        private bool Desconectar_SOCKET(int IP)
        {
            bool S = Lista_Conexoes_WEBSOCKET.Remove(IP);
            return S;
        }
        public async Task<bool> Iniciar_Begin_AcessoRemoto(object ObjAC)
        {
            WebSocket Obter_Contexto_WEBSOCKET;
            HttpListenerContext IAC;

            try
            {


                /**
                 * Lembrar de criar um gerente de conecções websocket para evitar conflitos.
                 */
                IAC = (HttpListenerContext)ObjAC;

                int IP = IAC.Request.RemoteEndPoint.Address.GetHashCode();
                if (Gerente_WEBSOCKET(IP))
                {
                    Desconectar_SOCKET(IP);
                    throw new Exception("Já existem outras conexões vindas desse dispositivo, favor desconectá-los e tentar novamente.");
                }

                HttpListenerWebSocketContext WebSocket_CORAC = await IAC.AcceptWebSocketAsync(null);
                Obter_Contexto_WEBSOCKET = WebSocket_CORAC.WebSocket;
                
                Pacote_AcessoRemoto_Config_INIT Pacote_Inicial = new Pacote_AcessoRemoto_Config_INIT();
                //ArraySegment<byte> DadosEnviando = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote_Inicial)));
                /**
                 * aguarda resposta do navegador antes de continuar.
                 */
                await enviarPacotes(Obter_Contexto_WEBSOCKET, WebSocketMessageType.Text, Pacote_Inicial);
                
                try
                {
                    ArraySegment<byte> DadosRecebendo = new ArraySegment<byte>(new byte[Buffer_Server]);
                    WebSocketReceiveResult Resultado_WS = await Obter_Contexto_WEBSOCKET.ReceiveAsync(DadosRecebendo, new CancellationToken());

                    WebSocketCloseStatus? p = Resultado_WS.CloseStatus;
                    if (p == WebSocketCloseStatus.EndpointUnavailable)
                    {
                        throw new Exception("Ocorreu um erro não avaliado!.");
                    }

                    string Pacote_String = ASCIIEncoding.UTF8.GetString(DadosRecebendo.Array);
                    
                    Converter_JSON_String.DeserializarPacote(Pacote_String, out Pacote_Base Base, out object Saida);
                    Pacote_AcessoRemoto_Config_INIT PIC = (Pacote_AcessoRemoto_Config_INIT)Saida;

                    
                    //if (true)
                    if (_GerenciadorCliente.Validar_Chave_AR(PIC.Chave_AR))
                    {
                        Task<bool> RCB =  ReceberPacotes(IAC, Obter_Contexto_WEBSOCKET);
                        //await RCB.ConfigureAwait(true);
                        Task<bool> EFM = enviarFrame(IAC, Obter_Contexto_WEBSOCKET);
                        Task[] FluxoDados = new Task[2];
                        FluxoDados[0] = RCB;
                        FluxoDados[1] = EFM;

                        Task.WaitAll(FluxoDados);
                        return true;
                    }
                    else
                    {
                        Pacote_Error PERR = new Pacote_Error();
                        PERR.Error = true;
                        PERR.Numero = 42001;
                        PERR.Mensagem = "Erro de autenticação!";
                        await closeConexao(IAC, Obter_Contexto_WEBSOCKET, PERR, WebSocketCloseStatus.NormalClosure);
                        return true;
                    }
                }
                catch(Exception e)
                {
                    Pacote_Error PERR = new Pacote_Error();
                    PERR.Error = true;
                    PERR.Mensagem = e.Message;
                    PERR.Numero = e.HResult;
                    await closeConexao(IAC, Obter_Contexto_WEBSOCKET, PERR, WebSocketCloseStatus.InternalServerError);
                    return true;
                }


                
            }
            catch (Exception e)
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                TratadorErros(e, this.GetType().Name);
                return true;
            }

        }
        private void encerramentoAtendimento()
        {
            MessageBox.Show("O atendimento foi encerrado. Obrigado! \nQualquer dúvida, entrar em contato com o departamento de tecnologia.","Atendimento finalizado!", MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
        }

        private void Encerramento_Falha()
        {
            MessageBox.Show("O atendimento foi encerrado devido a uma falha no acesso remoto!\nFavor, entrar em contato com o departamento de tecnologia.", "Atendimento finalizado!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

        }
        /**
         * <summary>
         * Recebe os pacotes da conexão SOCKEWEB.
         * </summary>
         */
        [STAThread]
        private async Task<bool> ReceberPacotes(HttpListenerContext Server, WebSocket Sck)
        {
            try
            {
                ArraySegment<byte> DadosRecebendo;
                Pacote_TecladoRemoto TecladoRmt;
                Pacote_MouseRemoto Mouse = new Pacote_MouseRemoto();
                Pacote_EventMouse EventosMouseRemotos;

                while (Sck.State == WebSocketState.Open)
                {
                    DadosRecebendo = new ArraySegment<byte>(new byte[Buffer_Server]);
                    CancellationToken Token = new CancellationToken();
                    WebSocketReceiveResult Resultado_WS = await Sck.ReceiveAsync(DadosRecebendo, Token);


                    WebSocketCloseStatus? p = Resultado_WS.CloseStatus;
                    if (p == WebSocketCloseStatus.EndpointUnavailable || p == WebSocketCloseStatus.Empty)
                    {
                        Pacote_CloseConection Close = new Pacote_CloseConection();
                        Close.Close = Sck.State;
                        await closeConexao(Server, Sck, Close, WebSocketCloseStatus.InternalServerError);
                        encerramentoAtendimento();
                        break;
                    }
                    string Pacote_String = ASCIIEncoding.UTF8.GetString(DadosRecebendo.Array);

                    int I = Pacote_String.IndexOf(Convert.ToChar(0));
                    bool isPacoteInicio = Pacote_String.Substring(0, 10) == "{\"Pacote\":";

                    bool isPacoteFim = Pacote_String.Substring(I - 1, 2) == "}\0";
                    if (!isPacoteInicio || !isPacoteFim) continue;

                    Console.WriteLine(Pacote_String);
                    Converter_JSON_String.DeserializarPacote(Pacote_String, out Pacote_Base Base, out object Saida);
                    
                    switch (Base.Pacote)
                    {
                        case TipoPacote.TecladoRemoto:
                            TecladoRmt = (Pacote_TecladoRemoto)Saida;
                            TecladoRmt.ChamarTeclas();


                            break;

                        case TipoPacote.EventMouse:
                            EventosMouseRemotos = (Pacote_EventMouse)Saida;
                            Mouse.Gerar_EventoMouse(EventosMouseRemotos);
                            //Pacote_Confirmacao Confirmado = new Pacote_Confirmacao();
                            //Confirmado.PacoteConfirmado = TipoPacote.EventMouse;

                            //await enviarPacotes(Sck, WebSocketMessageType.Text, Confirmado);
                            break;

                        default:
                            Pacote_Error PERR = new Pacote_Error();
                            PERR.Error = true;
                            PERR.Mensagem = "Pacote inexistente";
                            PERR.Numero = 42000;
                            await closeConexao(Server, Sck, PERR, WebSocketCloseStatus.InternalServerError);
                            //return true;
                            break;
                    }
                    

                }
                
                return true;
            }
            catch(Exception e)
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                TratadorErros(e, this.GetType().Name);

                Pacote_Error PERR = new Pacote_Error();
                PERR.Error = true;
                PERR.Mensagem = e.Message;
                PERR.Numero = 42003;
                Encerramento_Falha();
                await closeConexao(Server, Sck, PERR, WebSocketCloseStatus.InternalServerError);
                return true;
            }

        }

        private async Task<bool> closeConexao(HttpListenerContext Server, WebSocket Sck, ITipoPacote Pacote, WebSocketCloseStatus TipoFechamento)
        {

            try
            {
                
                ArraySegment<byte> DadosEnviando = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote)));
                await Sck.SendAsync(DadosEnviando, WebSocketMessageType.Text, true, CancellationToken.None);

                Pacote_CloseConection Close = new Pacote_CloseConection();
                Close.Close = Sck.State;
                string StgFechamento = Converter_JSON_String.SerializarPacote(Close);

                CancellationToken Token = new CancellationToken();
                await Sck.CloseOutputAsync(TipoFechamento, StgFechamento, Token);
                
                int IP = Server.Request.RemoteEndPoint.Address.GetHashCode();
                Desconectar_SOCKET(IP);
                return true;
            }
            catch (Exception e)
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                TratadorErros(e, this.GetType().Name);
                return false;
            }

        }
        /**
         * <summary>
         *  Envia os pacotes para o cliente da conexção WEBSOCKET.
         * </summary>
         */
        private async Task<bool> enviarPacotes(WebSocket Sck, WebSocketMessageType Tipo, ITipoPacote Pacote)
        {
            try
            {
                if (Sck.State != WebSocketState.Open) return false;
                while (Semafaro) { }
                ArraySegment<byte> EnviandoDados = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(Pacote)));
                Semafaro = true;
                await Sck.SendAsync(EnviandoDados, Tipo, true, CancellationToken.None);
                Semafaro = false;
                return true;
            }
            catch(Exception e)
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                TratadorErros(e, this.GetType().Name);
                return false;
            }

        }

        public int SetBuffer { set { Buffer_Server = value; } }
        /**
      * Data: 17/05/2020
      * Propriedade que atribuirá um componente para verificar credenciais.
      * Return: IGCliente
      */
        public IGClienteHTML Gerenciador_Cliente
        {
            set { _GerenciadorCliente = value; }
        }
        /**
          * Data: 17/05/2020
          * Propriedade que atribuirá uma classe para autenticação de máquinas.
          * Return: IAuth
          */
        public IAuthHTML Autenticador
        {
            set { _Auth = value; }
        }
        [STAThread]
        private async Task<bool> enviarFrame(HttpListenerContext Server, WebSocket Sck)
        {
            try
            {
                ArraySegment<byte> EnviandoDados;
                Pacote_FrameTelas CapturarTelas = new Pacote_FrameTelas();
                bool C = true;
                while (C)
                {
                    //if (Sck.State != WebSocketState.Open) break;
                    bool Yes = CapturarTelas.GerarTelas();
                    while (Semafaro & Yes) { }
                    EnviandoDados = new ArraySegment<byte>(ASCIIEncoding.UTF8.GetBytes(Converter_JSON_String.SerializarPacote(CapturarTelas)));
                    Semafaro = true;
                    await Sck.SendAsync(EnviandoDados, WebSocketMessageType.Text, true, CancellationToken.None);
                    Semafaro = false;

                    await Task.Delay(500, CancellationToken.None);
                }
                return true;
                //Pacote_CloseConection Close = new Pacote_CloseConection();
                //Close.Close = Sck.State;
                //await closeConexao(Server, Sck, Close, WebSocketCloseStatus.InternalServerError);

            }
            catch (Exception e)
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                TratadorErros(e, this.GetType().Name);
                return true;
            }

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

        AcessoRemoto_WEBSOCKET ControleAC = new AcessoRemoto_WEBSOCKET();

        //public AcessoRemoto_WEBSOCKET Contro { get { return ControleAC; } set { ControleAC = value; } }
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
                 if (_Auth == null) throw new Exception("Nenhum processador de autenticação foi identificado.");
                if (_GerenciadorCliente == null) throw new Exception("Nenhum gerenciado de processo foi identificado.");
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
        private  async void IniciarConversa(IAsyncResult s)
        {
            Pacote_Error P_Error;

            Thread AcessoRemoto, Chat;
            try
            {

                HttpListener Server = (HttpListener)s.AsyncState;

                HttpListenerContext Aceitar = Server.EndGetContext(s);

                Servidor.BeginGetContext(IniciarConversa, Server);

                HttpListenerRequest ObterConteudo = Aceitar.Request;
                StreamReader _Conteudo = new StreamReader(ObterConteudo.InputStream);

                HttpListenerResponse ObterResposta = Aceitar.Response;

                /**
                 * Coloca o servidor para receber outras conexões.
                 */

                Uri URi = Aceitar.Request.Url;
                switch (URi.LocalPath)
                {
                    case "/CORAC/AcessoRemoto/":

                        //AcessoRemoto = new Thread(ControleAC.Iniciar_Begin_AcessoRemoto);
                        //AcessoRemoto.Name = "AcessoRemoto";
                        //AcessoRemoto.Start(Aceitar);
                        
                        await ControleAC.Iniciar_Begin_AcessoRemoto(Aceitar);
                        ControleAC = new AcessoRemoto_WEBSOCKET();

                        break;

                    case "/AA_AcessoRemoto_SYN/":
                        try
                        {
                            string __Conteudo = _Conteudo.ReadLine();
                            /*-----------------------Processamento----------------------------*/

                            Converter_JSON_String.DeserializarPacote(__Conteudo, out Pacote_Base Base, out object QTP);
                            switch (Base.Pacote)
                            {
                                case TipoPacote.AcessoRemoto_SYN:
                                    Pacote_AcessoRemoto_SYN PAR = (Pacote_AcessoRemoto_SYN)QTP; //Transforma string em um objeto da classe Pacote_Auth

                                    //_GerenciadorCliente?._OAuth(CMM.Chave);

                                    bool _Autenticado = (bool)_Auth.HTML_Autenticado(PAR.Chave);

                                    if (!_Autenticado) throw new Exception("Usuário não autenticado ou bloqueado.");

                                    //Qualifica o tipo de serviço que requisitou a autenticação;
                                    _Auth.GetAutenticacao.Servico = TipoServico.AcessoRemoto;
                                    /**
                                     * Lembrar de realizar uma alteração no pacote de autenticação que indique que método está requisitando-a
                                     */
                                    _GerenciadorCliente?.ConectarCliente(Aceitar.Request.RemoteEndPoint, _Auth.GetAutenticacao);

                                    /*
                                     * Requisita liberação de acesso à visualização da tela.
                                     */
                                    DialogResult Resposta = MessageBox.Show("O atendente: " + _Auth.GetAutenticacao.Usuario + "\nestá realizando um pedido de acesso remoto a essa máquina. \n\nLiberar acesso?", "Pedido de acesso remoto.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

                                    string DadosPacote = null;
                                    Pacote_AcessoRemoto_Resposta PCR = new Pacote_AcessoRemoto_Resposta();

                                    if (Resposta == DialogResult.Yes)
                                    {

                                        //------Pacote que vai para o remetente-----------
                                        PCR.ChaveAR = _Auth.GetAutenticacao.ChaveAR;
                                        PCR.Resposta = "OK_2001";

                                        //------Pacote base que chega no servidor WEB
                                        PAR.Resposta = Converter_JSON_String.SerializarPacote(PCR);

                                        DadosPacote = Converter_JSON_String.SerializarPacote(PAR);
                                        //Obtém o Barramento de escrita com a cliente
                                        ObterResposta.ContentLength64 = DadosPacote.Length;
                                        ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                                        ObterResposta.Close();

                                        //Atribui as classes de controle e autenticação para o WEBSOCKET
                                        ControleAC.SetBuffer = 5000;
                                        ControleAC.Autenticador = _Auth;
                                        ControleAC.Gerenciador_Cliente = _GerenciadorCliente;


                                    }
                                    else
                                    {

                                        PCR.Resposta = "NO_2001";
                                        PAR.Resposta = Converter_JSON_String.SerializarPacote(PCR);
                                        DadosPacote = Converter_JSON_String.SerializarPacote(PAR);
                                        //Obtém o Barramento de escrita com a cliente
                                        ObterResposta.ContentLength64 = DadosPacote.Length;
                                        ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                                        ObterResposta.Close();
                                    }


                                    break;

                                case TipoPacote.Echo:
                                    Pacote_PingReplay RPY = new Pacote_PingReplay();
                                    DadosPacote = Converter_JSON_String.SerializarPacote(RPY);
                                    //Obtém o Barramento de escrita com a cliente
                                    ObterResposta.ContentLength64 = DadosPacote.Length;
                                    ObterResposta.OutputStream.Write(ASCIIEncoding.Unicode.GetBytes(DadosPacote), 0, DadosPacote.Length);
                                    ObterResposta.Close();
                                    break;

                                case TipoPacote.AcessoRemoto: //Acesso remoto via WEBSOCKET

                                    break;

                                default:
                                    P_Error = new Pacote_Error();
                                    P_Error.Error = true;
                                    P_Error.Mensagem = "Esse tipo de pacote não é permitido.";
                                    P_Error.Numero = DadosExcecao.HResult;

                                    DadosPacote = Converter_JSON_String.SerializarPacote(P_Error);
                                    ObterResposta.ContentLength64 = DadosPacote.Length;
                                    ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                                    ObterResposta.Close();
                                    break;
                            }
                            _Conteudo.Close();
                            //Server.Close();


                        }
                        catch (Exception e)
                        {
                            TratadorErros(e, this.GetType().Name);

                            P_Error = new Pacote_Error();
                            P_Error.Error = Excecao;
                            P_Error.Mensagem = DadosExcecao.Message;
                            P_Error.Numero = DadosExcecao.HResult;

                            string Erros = Converter_JSON_String.SerializarPacote(P_Error);
                            ObterResposta.ContentLength64 = Erros.Length;
                            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(Erros), 0, Erros.Length);
                            ObterResposta.Close();

                            Excecao = false;
                            DadosExcecao = null;
                        }
                        break;

                    default:

                        break;
                }
                



                /**
                 * Coloca o servidor em listen novamente;
                 */

                //HttpListenerWebSocketContext WebSocket_CORAC =  await aceita.AcceptWebSocketAsync(null);


                //WebSocket Obter_Contexto_WEBSOCKET = WebSocket_CORAC.WebSocket;
                //Uri URi = Aceitar.Request.Url;
                //switch (URi.LocalPath)
                //{
                //    case "/AcessoRemoto/":
                //        Thread AC = new Thread(ControleAC.Iniciar_Begin_AcessoRemoto);
                //        AC.Name = "AcessoRemoto";
                //        AC.Start(Aceitar);
                //        break;

                //    default:

                //        break;
                //}

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
