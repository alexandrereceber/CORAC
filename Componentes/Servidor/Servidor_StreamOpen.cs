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
using ServerClienteOnline.TratadorDeErros;


namespace ServerClienteOnline.Server
{
    using Interfaces;
    using Utilidades;

    public class Server_STREAMOpen : Tratador_Erros ,IDisposable
    {

        private IPAddress IPEscutar;
        private int PORT;
        private int TotalConexoes = 10;

        private string NomeLocalMaquina;
        private IPHostEntry IPsHost;

        //private List<KeyValuePair<ParametrosInicializacao, EndPoint>> ListaClientes_Conectados = new List<KeyValuePair<ParametrosInicializacao, EndPoint>>();
        /*Informa se ocorreram erros durate a execução da classe*/

        private IRuntime _CMDs;

        /**
          * Data: 27/02/2019
          * Construtor da classe que configura os principais variáveis da classe;
          * Return: void
          */
        public Server_STREAMOpen(int port = 0, string IP = null)
        {
            try
            {
                TSaida_Error = TipoSaidaErros.ShowWindow;

                PORT = port == 0 ? 80 : port ;
                NomeLocalMaquina = Dns.GetHostName(); //Obtém o nome da máquina local
                IPsHost = Dns.GetHostEntry(NomeLocalMaquina); //Obtém uma lista de endereços de ips locais

                if (IP == null)
                {
                    IPEscutar = IPsHost.AddressList[1]; //Falta implementar a seleção de qual ip será utilizado quando existir mais IP
                }
                else
                {
                    IPEscutar = IPAddress.Parse(IP);
                    bool Test = false;
                    foreach( IPAddress Ips in IPsHost.AddressList)
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
                TratadorErros(e, this.GetType().Name);

            }

        }

        /**
          * Data: 27/02/2019
          * Inicia o servirdor no modo de Escuta
          * Return: void
          */
        public void StartServidor()
        {

            try
            {
                TcpListener Servidor = new TcpListener(IPEscutar, PORT);
                Servidor.Start(TotalConexoes);

                IAsyncResult AceitarCliente = Servidor.BeginAcceptTcpClient(new AsyncCallback(IniciarConversa), Servidor);
                
            }
            catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);
            }

        }

        /**
          * Data: 27/02/2019
          * Propriedade qye atribui uma classe à instância para futuro uso.
          * Return: IRuntime
          */
        public IRuntime AtribuirExecutor
        {
            set { _CMDs = value; }
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
                
                Criar = new Thread(Servidor_ADHOC);
                Criar.Start(aceita);

                /*Inicia novamente o estado de escuta com o fim de ouvir outros clientes*/
                Server.BeginAcceptTcpClient(new AsyncCallback(IniciarConversa), Server);
            }
            catch (Exception e)
            {
                                TratadorErros(e, this.GetType().Name);;
            }


        }

        /**
          * Data: 27/02/2019
          * Implementação de melhoras para o futuro. Mas a ideia geral é a de que o cliente que está conectando, seja avaliado
          * pelos parâmetros que foram enviados.
          * Return: string
          */  
        private string TratarInicializacao(ParametrosInicializacao Cliente)
        {
            try
            {

                /*Irá validar as informações e responder para o cliente*/
                if (1 == 1)
                {
                    ParametrosInicializacao Server = new ParametrosInicializacao();
                    Server.Maquina = Dns.GetHostName();

                    return JsonConvert.SerializeObject(Server);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                                TratadorErros(e, this.GetType().Name);;
                return null;
            }
        }


        /**
          * Data: 27/02/2019
          * Adiciona em uma lista os clientes que se concectaram ao servidor.
          * Return: ParametrosInicializacao
          */
        private ParametrosInicializacao ConectarCliente(string Pmt, EndPoint Client)
        {
            ParametrosInicializacao PIL = JsonConvert.DeserializeObject<ParametrosInicializacao>(Pmt);

            return PIL;
        }

        /**
         * Data: 27/02/2019
         * Método que criar um barramento temporário e após a execução dos comando fecha todos os canais.
         * Return: void
         */
        private void Servidor_ADHOC(object Dados)
        {
            BinaryReader BarramentoLeitura;
            BinaryWriter BarramentoEscrita;

            try
            {
                TcpClient Clients = (TcpClient)Dados;
                
                using (NetworkStream Brrm = Clients.GetStream())
                {
                    BarramentoLeitura = new BinaryReader(Brrm);
                    BarramentoEscrita = new BinaryWriter(Brrm);

                    byte[] entrada = new byte[Clients.Available];
                    int count = 0;
                    bool RecebendoDadosLoop = true;
                    while (true)
                    {
                        BarramentoLeitura.Read(entrada, 0, Clients.Available);
                        if (count == 0)
                            count++;
                        else
                            if (RecebendoDadosLoop) { RecebendoDadosLoop = false; continue; } else RecebendoDadosLoop = true;

                        Console.WriteLine(ASCIIEncoding.UTF8.GetString(entrada));
                        entrada = ASCIIEncoding.UTF8.GetBytes("O servidor recebeu os dados" + count);
                        count++;
                        BarramentoEscrita.Write(entrada);
                        entrada = new byte[Clients.SendBufferSize];
                    }
                }
            }
            catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);
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
                TratadorErros(e, this.GetType().Name);
                return null;
            }
            
        }

        public void Dispose()
        {

        }
    }


}
