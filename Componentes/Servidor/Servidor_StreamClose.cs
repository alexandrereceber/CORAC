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
using ServerClienteOnline.TratadorDeErros;

namespace ServerClienteOnline.Server
{
    using Interfaces;

    //using Utilidades;

    public class Server_StreamClose : Tratador_Erros, IServidor
    {
        TcpListener Servidor;
        private IPAddress IPEscutar;
        private int PORT;
        private int TotalConexoes = 10;
        private string EDominio = null; //Endereço do servidor que responderá essa estação.

        private string NomeLocalMaquina;
        private IPHostEntry IPsHost;

        /*Informa se ocorreram erros durate a execução da classe*/
        private TError SaidaError = new TError { Error = false };

        public __Autenticacao TEndPoint { get; set; }

        private IRuntime _CMDs;
        private IAuth _Auth;
        private IGClienteHTML _GerenciadorCliente;
        private IAcesso_Remoto _AcessoRemoto;

        private bool Active = false;

        Pacote_Base Base;
        /**
          * Data: 27/02/2019
          * Construtor da classe que configura os principais variáveis da classe;
          * Return: void
          */
        public Server_StreamClose(int port = 0, string IP = null)
        {
            try
            {
                PORT = port == 0 ? 80 : port ;
                NomeLocalMaquina = Dns.GetHostName(); //Obtém o nome da máquina local
                IPsHost = Dns.GetHostEntry(NomeLocalMaquina); //Obtém uma lista de endereços de ips locais
                TEndPoint = __Autenticacao.Cliente; /*Se não for, explicitamente informado o tipo de Serviço, será utilizado o serviço de Cliente*/

                if (IP == null)
                {
                    IPEscutar = IPsHost.AddressList[2]; //Falta implementar a seleção de qual ip será utilizado quando existir mais IP

                }
                else
                {
                    IPEscutar = IPAddress.Parse(IP);
                    

                    bool Test = false;

                    foreach ( IPAddress Ips in IPsHost.AddressList)
                    {
                        
                        if(Ips.Equals(IPEscutar))
                        {
                            Test = true;
                        }
                    }

                    if (!Test)
                    {
                        throw new Exception("O ip informado não coincide com os IPs desta máquina.");
                    }
                }
            }
            catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);;
            }

        }


        /**
          * Data: 22/03/2019
          * Relaciona a máquina a um domínio.
          * Return: void
          */
        public void Dominio(string DOM)
        {
            DOM = DOM.Replace(" ", "");
            EDominio = DOM;
        }
        /**
          * <summary>
          * Promove a estação a um controlador.
          * </summary>
          */
        public void PromoverControlador()
        {
            TEndPoint = __Autenticacao.Servidor;
        }

        /**
         * <summary>
         * Promove a estação a um utilizador.
         * </summary>
         */
        public void PromoverUtilizador()
        {
            TEndPoint = __Autenticacao.Cliente;
        }

        /**
          * Data: 27/02/2019
          * Inicia o servirdor no modo de Escuta
          * Return: void
          */
        public bool StartServidor()
        {

            try
            {
                /*Verifica se um domínio válido foi especificado.*/
                if (EDominio == null) { throw new Exception("Nenhum domínio válido foi especificdo."); }

                Servidor = new TcpListener(IPEscutar, PORT);
                Servidor.Start(TotalConexoes);

                IAsyncResult AceitarCliente = Servidor.BeginAcceptTcpClient(new AsyncCallback(IniciarConversa), Servidor);
                Active = true;

                return true;

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                Active = false;

                return false;
            }

        }

        /**
          * Data: 27/02/2019
          * Propriedade que atribui uma classe que executará comandos e retornará uma reposta.
          * Return: IRuntime
          */
        public IRuntime AtribuirExecutor
        {
            set { _CMDs = value; }
        }

        /**
          * Data: 23/03/2019
          * Propriedade que atribui uma classe para autenticação de máquinas.
          * Return: IAuth
          */
        public IAuth Autenticador
        {
            set { _Auth = value; }
        }

        /**
          * Data: 23/03/2019
          * Propriedade que atribui uma classe que executará comandos e retornará uma reposta.
          * Return: IGCliente
          */
        public IGClienteHTML Gerenciador_Cliente
        {
            set { _GerenciadorCliente = value; }
        }

        public IAcesso_Remoto AcessoRemoto
        {
            set
            {
                _AcessoRemoto = value;
            }
        }

        /**
          * Data: 27/02/2019
          * Dá início à comunicação, criando uma thread para cada cliente
          * Return: string
          */
        private void IniciarConversa(IAsyncResult s)
        {
            Thread Criar;
            try
            {
 
                TcpListener Server = (TcpListener)s.AsyncState;
                TcpClient aceita = Server.EndAcceptTcpClient(s);
                
                Criar = new Thread(Receber_Dados_Responder);
                Criar.Start(aceita);

                /*Inicia novamente o estado de escuta com o fim de ouvir outros clientes*/
                Server.BeginAcceptTcpClient(new AsyncCallback(IniciarConversa), Server);
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }


        }

        /**
          * Data: 27/02/2019
          * Implementação de melhoras para o futuro. Mas a ideia geral é a de que o cliente que está conectando, seja avaliado
          * pelos parâmetros que foram enviados.
          * Return: string
          */  
        private string TratarInicializacao(string Pct)
        {
            try
            {
                Pacote_Inicializacao Init = DeserializarPacote(Pct);

                Pacote_Inicializacao PACT = new Pacote_Inicializacao();
                PACT.EnderecoIP = IPEscutar.ToString();
                PACT.PORT = PORT;

                return SerializarPacote(PACT);
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);;
                return null;
            }
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
                string Pct = JsonConvert.SerializeObject(Conteudo);
                Pacote_Base PctBase = new Pacote_Base();
                PctBase.Pacote = Conteudo.GetTipoPacote();
                PctBase.Conteudo = Pct;

                string SerializarPacote = JsonConvert.SerializeObject(PctBase);
                return SerializarPacote;

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);;
                return null;
            }

        }

        /**
          * Data: 22/03/2019
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
                TratadorErros(e, this.GetType().Name);;
                return false;
            }
            

        }

        /**
         * Data: 27/02/2019
         * Método que cria os barramentos de leitura e escrita, dá início ao diálogo inicial com os clientes e 
         * e dá início à conversa contínua com os cliente.
         * Return: void
         */
        private void Receber_Dados_Responder(object Dados)
        {
            StreamReader BarramentoLeitura;
            StreamWriter BarramentoEscrita;
            Pacote_Error P_Error;

            try
            {
                TcpClient Clients = (TcpClient)Dados;

                Func<string, bool> Falha = (G) => {
                    if (G == null)
                    {
                        EndPoint Fl = Clients.Client.RemoteEndPoint;
                        throw new Exception("Ocorreu uma falha no Servidor: " + Fl.ToString());
                    }
                    else { return true; }
                };

                using (NetworkStream Brrm = Clients.GetStream())
                {
                    //Barramento de Leitura, após a abertura do socket
                    BarramentoLeitura = new StreamReader(Brrm);
                    //Obtém o Barramento de escrita com a cliente
                    BarramentoEscrita = new StreamWriter(Brrm);

                    string DadosLidos = BarramentoLeitura.ReadLine();
                    Falha(DadosLidos);//Verifica se o barramento é nulo, caracterizando uma falha na comunicação.

                    Pacote_PingEcho Echo = DeserializarPacote(DadosLidos);
                    /*Verifica se, cliente e servidor estão em sintonia, ocorreu algum erro*/
                    if (Echo.GetTipoPacote() != TipoPacote.Echo) {
                        Pacote_PingReplay Replay = new Pacote_PingReplay();
                        Replay.Validado = false;
                        Replay.ObterTempo();

                        /*Confirma o recebimento, do cliente, inicialização.*/
                        BarramentoEscrita.WriteLine(SerializarPacote(Replay));
                        BarramentoEscrita.Flush();

                        throw new Exception("Falha de comunicação inicial: Cliente");
                    }
                    else
                    {
                        Pacote_PingReplay Replay = new Pacote_PingReplay();
                        Replay.Validado = true;
                        Replay.ObterTempo();

                        /*Confirma o recebimento, do cliente, inicialização.*/
                        BarramentoEscrita.WriteLine(SerializarPacote(Replay));
                        BarramentoEscrita.Flush();
                    }

                    DadosLidos = BarramentoLeitura.ReadLine();
                    Falha(DadosLidos);//Verifica se o barramento é nulo, caracterizando uma falha na comunicação.

                    string EnviarDadosCliente = TratarInicializacao(DadosLidos);


                    /*Informa ao cliente que os parâmetros de inicialização foram recebidos com sucesso.*/
                    BarramentoEscrita.WriteLine(EnviarDadosCliente);
                    BarramentoEscrita.Flush();

                    /*Processo de leitura do pacote de autenticação.*/
                    DadosLidos = BarramentoLeitura.ReadLine();
                    Falha(DadosLidos);//Verifica se o barramento é nulo, caracterizando uma falha na comunicação.

                    Pacote_Auth PTC = DeserializarPacote(DadosLidos); //Transforma string em um objeto da classe Pacote_Auth
                    PTC.TEndPointServer = TEndPoint;
                    PTC.DominioServidor = EDominio;
                    Pacote_Auth _Autenticado = _Auth?.AutenticarUsuario(PTC);
                    if (_Autenticado != null)
                    {
                        _GerenciadorCliente?.ConectarCliente(Clients.Client.RemoteEndPoint, _Autenticado);
                        EnviarDadosCliente = SerializarPacote(_Autenticado);
                        BarramentoEscrita.WriteLine(EnviarDadosCliente);
                        BarramentoEscrita.Flush();


                        //Leitura do barramento de dados do próximo tipo de pacote
                        DadosLidos = BarramentoLeitura.ReadLine();
                        Falha(DadosLidos);//Verifica se o barramento é nulo, caracterizando uma falha na comunicação.

                        dynamic QTP = DeserializarPacote(DadosLidos);
                        switch (Base.Pacote)
                        {
                            case TipoPacote.Comando:
                                Pacote_Comando CMM = (Pacote_Comando)QTP; //Transforma string em um objeto da classe Pacote_Auth
                                string Executar = _ResponderRequisicao(CMM);

                                if (Excecao)
                                {
                                    P_Error = new Pacote_Error();
                                    P_Error.Error = Excecao;
                                    P_Error.Mensagem = DadosExcecao.Message;

                                    Executar = SerializarPacote(P_Error);
                                    Excecao = false;
                                    DadosExcecao = null;
                                }

                                Pacote_Comando PCT = new Pacote_Comando();
                                PCT.Resposta = Executar;
                                string DadosPacote = SerializarPacote(PCT);
                                //Obtém o Barramento de escrita com a cliente
                                BarramentoEscrita.WriteLine(DadosPacote);
                                BarramentoEscrita.Flush();

                                break;

                            case TipoPacote.Echo:
                                Pacote_PingReplay RPY = new Pacote_PingReplay();
                                DadosPacote = SerializarPacote(RPY);
                                //Obtém o Barramento de escrita com a cliente
                                BarramentoEscrita.WriteLine(DadosPacote);
                                BarramentoEscrita.Flush();
                                break;

                            case TipoPacote.File:
                                Pacote_File FILE = (Pacote_File)QTP;
                                string PathFile = FILE.Path;

                                int ReceiveBufferClienteSize = FILE.ReceiveBufferCLiente; //Armazena o tamanho do buffer de recepção do cliente;
                                int SendBufferClienteSize = FILE.SendBufferCliente; //Armazena o tamanho do buffer de envio do cliente;

                                int ReceiveBufferServerSize = Clients.ReceiveBufferSize; //Armazena o tamanho do buffer de recepção do cliente;
                                int SendBufferServerSize = Clients.SendBufferSize; //Armazena o tamanho do buffer de envio do cliente;

                                //Enviando dados dos buffers do servidor
                                FILE.ReceiveBufferServidor = ReceiveBufferServerSize; //Envia para o server a capacidade de recepção
                                FILE.SendBufferServidor = SendBufferServerSize;//envia para o server capacidade o tamanho de envio;

                                int BufferTransmissao = ReceiveBufferClienteSize > SendBufferServerSize ? SendBufferServerSize : ReceiveBufferClienteSize;

                                FILE.Existe = File.Exists(PathFile);
                                if (FILE.Existe)
                                {
                                    using (FileStream EnviarArquivo = new FileStream(FILE.Path, FileMode.Open))
                                    {
                                        long TamanhoArquivoEnviar = EnviarArquivo.Length;
                                        FILE.Lenght = TamanhoArquivoEnviar;

                                        DadosPacote = SerializarPacote(FILE);
                                        BarramentoEscrita.WriteLine(DadosPacote);
                                        BarramentoEscrita.Flush();

                                        byte[] T = new byte[BufferTransmissao];
                                        using (BinaryWriter EVFILE = new BinaryWriter(Brrm))
                                        {
                                            int LoopEnvio = 0;
                                            do
                                            {
                                                int TotalEnviado = EnviarArquivo.Read(T, 0, BufferTransmissao);
                                                EVFILE.Write(T);
                                                LoopEnvio += TotalEnviado;
                                            } while (LoopEnvio <= TamanhoArquivoEnviar);

                                            EnviarArquivo.Close();
                                        }
                                    }
                                }
                                else
                                {

                                }


                                break;

                            default:
                                    P_Error = new Pacote_Error();
                                    P_Error.Error = true;
                                    P_Error.Mensagem = "Esse tipo de pacote não existe.";
                                    BarramentoEscrita.WriteLine(SerializarPacote(P_Error));
                                    BarramentoEscrita.Flush();
                                break;
                        }




                    }
                    else
                    {   //Não existe mecanismo de autenticação.
                        _Autenticado = new Pacote_Auth {
                                                            Autenticado = false,
                                                            Error = true,
                                                            EMensagem = "O servidor não está configurado com nenhum processo de autenticação."
                                                        };
                        EnviarDadosCliente = SerializarPacote(_Autenticado);
                        BarramentoEscrita.WriteLine(EnviarDadosCliente);
                        BarramentoEscrita.Flush();
                    }
                    

                    
                    BarramentoLeitura.Close();
                    BarramentoEscrita.Close();
                    //Desconectar_Cliente("10.56.34.222","80");
                    Clients.Close();
                }
            }
            catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);;
            }
            

        }

        /**
         * Data: 27/02/2019
         * Método para chamar, de uma classe informada, a execução e resposta por streamReader
         * Return: False
         */
        private string _ResponderRequisicao(Pacote_Comando pct)
        {

            try
            {
                bool DadosEnviar = _CMDs.Route(pct);
                if (DadosEnviar) return _CMDs.Get_Resultado(); else return null;

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);;
                return null;
            }
            
        }

        public bool StopServidor()
        {
            try { Servidor.Stop(); Active = false; return true; } catch(Exception e) { TratadorErros(e, this.GetType().Name); Active = false; return false; }
            
        }

        public bool StatusServidor()
        {
            return Active;
        }
    }


}
