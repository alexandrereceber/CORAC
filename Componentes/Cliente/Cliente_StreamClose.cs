using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Interfaces;
using System.Windows.Forms;
using System.Xml;

namespace ServerClienteOnline.Cliente
{
    class Cliente_StreamClose : Tratador_Erros, IDisposable
    {

        private IPEndPoint IPServer;
        private int PORT;
        private string EDominio = null; //Endereço do servidor que responderá essa estação.

        Pacote_Auth Authic = new Pacote_Auth();

        private TcpClient ClientToServer;
        private NetworkStream BarramentoComunicacao;
        private StreamReader BarramentoLeitura;
        private StreamWriter BarramentoEscrita;

        public __Autenticacao TEndPoint { get; set; }

        private bool Encriptar { get; set; }

        public Cliente_StreamClose(string IP, int port)
        {
            try
            {
                IPAddress IPS = IPAddress.Parse(IP);
                PORT = port;
                IPServer = new IPEndPoint(IPS, PORT);
                TEndPoint = __Autenticacao.Cliente;

            }catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);
            }
        }

        /**
         * <summary>
         * Define o tipo de servidor que a estação esta funcionando.
         * </summary>
         */
        public void TipoServidor(__Autenticacao Auth)
        {
            TEndPoint = Auth;
        }

        public void Dominio(string DOM)
        {
            DOM = DOM.Replace(" ", "");
            EDominio = DOM;
        }
        /**
         * <summary>
         * Converte uma classe de pacote a uma string para envio no barramento escrita.
         * </summary>
         */
        public string SerializarPacote(ITipoPacote Conteudo)
        {
            try
            {
                string Pct = JsonConvert.SerializeObject(Conteudo);
                Pacote_Base PctBase = new Pacote_Base();
                PctBase.Pacote = Conteudo.GetTipoPacote();
                PctBase.Conteudo = Pct;

                string SerializarPacote = JsonConvert.SerializeObject(PctBase);
                return SerializarPacote;

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return null;
            }

        }

        /**
         * <summary>
         * Converte uma string em uma classe de pacote.
         * </summary>
         */
        private dynamic DeserializarPacote(string Pacote)
        {
            Pacote_Base Base = JsonConvert.DeserializeObject<Pacote_Base>(Pacote);
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

            }

            return null;
        }

        /**
         * <summary>
         * Converte uma string em um pacote de inicialização entre cliente e servidor.
         * </summary>
         */
        private void TratarParametrosInicializacao(string Pmt)
        {
            try
            {
                Pacote_Inicializacao PACT = DeserializarPacote(Pmt);
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
            }

        }

        public Pacote_Auth SetDadosAutenticacao {
            get {
                return Authic;
            }
            set {
                Authic = value;
            }
        }


        /**
         * <summary>
         * Realiza o pedido de conexão a um servidor remoto. Após a conexão será enviado ao servidor um pedido de confirmação de recebimento de uma string, o 
         * servidor deverá responder com uma string contendo os caracteres 200OK. Após essa confirmação o cliente envia um pacote contendo todas as informações
         * para realizar as trocas de informações.
         * <para>Assíncrono</para>
         * </summary>
         */
        public async Task<bool> Connectar_ServidorAsync()
        {

            string DadosLidos;
            try
            {
                /**
                 * Seta os parâmetros de inicialização
                 */
                //GetParamentrosIniciais();

                ClientToServer = new TcpClient();
                ClientToServer.Connect(IPServer);
                if (ClientToServer.Connected)
                {
                    BarramentoComunicacao = ClientToServer.GetStream();

                    BarramentoEscrita = new StreamWriter(BarramentoComunicacao);
                    BarramentoLeitura = new StreamReader(BarramentoComunicacao);

                    Pacote_PingEcho Echo = new Pacote_PingEcho();
                    Echo.ObterTempo();
                    BarramentoEscrita.WriteLine(SerializarPacote(Echo));
                    BarramentoEscrita.Flush();

                    DadosLidos = await BarramentoLeitura.ReadLineAsync();
                    Pacote_PingReplay Replay = DeserializarPacote(DadosLidos);
                    if (Replay.GetTipoPacote() != TipoPacote.Replay) throw new Exception("Falha de comunicação inicial: Server");

                    Pacote_Inicializacao Boot = new Pacote_Inicializacao();
                    Boot.EnderecoIP = IPServer.Address.ToString();
                    Boot.PORT = IPServer.Port;
                    BarramentoEscrita.WriteLine(SerializarPacote(Boot));
                    BarramentoEscrita.Flush();

                    DadosLidos = await BarramentoLeitura.ReadLineAsync();
                    /*Verifica se os parametros foram recebidos pelo servidor com sucesso.*/
                    TratarParametrosInicializacao(DadosLidos);

                    Authic.TEndPointClient = TEndPoint;
                    Authic.DominioCliente = EDominio;

                    BarramentoEscrita.WriteLine(SerializarPacote(Authic));
                    BarramentoEscrita.Flush();

                    DadosLidos = await BarramentoLeitura.ReadLineAsync();
                    Authic = DeserializarPacote(DadosLidos);
                    if (Authic.Error)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                FecharConexao();
                return false;
            }

        }

        /**
         * <summary>
         * Envia e recebe dados.
         * <para>Assíncrono</para>
         * </summary>
         */
        public async Task<string> Enviar_Receber_DadosAsync(ITipoPacote Pacote)
        {
            try
            {
                string Dados = SerializarPacote(Pacote);
                BarramentoEscrita.WriteLine(Dados);
                BarramentoEscrita.Flush();
                string DadosLidos = await BarramentoLeitura.ReadLineAsync();
                ITipoPacote PCT = DeserializarPacote(DadosLidos);
                return PCT.GetResultado();
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return "false";
            }
            
        }

        public async Task<bool> Enviar_Receber_ArquivoAsync(Pacote_File Pacote, ProgressBar Progresso = null)
        {
            try
            {
                Pacote.ReceiveBufferCLiente = ClientToServer.ReceiveBufferSize; //Envia para o server a capacidade de recepção
                Pacote.SendBufferCliente = ClientToServer.SendBufferSize; //envia para o server capacidade o tamanho de envio;

                int ReceiveBufferClienteSize = ClientToServer.ReceiveBufferSize; //Envia para o server a capacidade de recepção
                int SendBufferClienteSize = ClientToServer.SendBufferSize; //envia para o server capacidade o tamanho de envio;

                string Dados = SerializarPacote(Pacote);
                BarramentoEscrita.WriteLine(Dados);
                BarramentoEscrita.Flush();
                string DadosLidos = await BarramentoLeitura.ReadLineAsync();
                Pacote_File PCT = DeserializarPacote(DadosLidos);
                
                int TamanhoArquivo = (int)PCT.Lenght;

                int ReceiveBufferServerSize = PCT.ReceiveBufferServidor; //Armazena o tamanho do buffer de recepção do cliente;
                int SendBufferServerSize = PCT.SendBufferServidor; //Armazena o tamanho do buffer de envio do cliente;

                int BufferRecepcao = ReceiveBufferClienteSize > SendBufferServerSize ? SendBufferServerSize : ReceiveBufferClienteSize;

                if (PCT.Existe)
                {
                    byte[] Arquivo = new byte[BufferRecepcao];
                    using (BinaryReader RFile = new BinaryReader(ClientToServer.GetStream()))
                    {
                        using (FileStream Gravar = new FileStream(PCT.Destino, FileMode.Create))
                        {
                            int LoopRecepcao = 0;

                            do
                            {
                                int TotalRecebido = RFile.Read(Arquivo, 0, BufferRecepcao);
                                Gravar.Write(Arquivo, 0, TotalRecebido);
                                LoopRecepcao += TotalRecebido;
                                Progresso.Value = (LoopRecepcao / TamanhoArquivo)*100;
                            } while (LoopRecepcao <= TamanhoArquivo);
                            
                            Gravar.Close();
                        }
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return false;
            }

        }


        public void FecharConexao()
        {
            if (BarramentoEscrita != null | BarramentoLeitura != null)
            {
                BarramentoEscrita.Close();
                BarramentoLeitura.Close();
            }
            if (ClientToServer != null )
            {
                ClientToServer.Close();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


}
