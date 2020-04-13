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

using System.Xml;

namespace ServerClienteOnline.Cliente
{
    class Cliente_StreamOpen : IDisposable
    {
        private IPEndPoint IPServer;
        private int PORT;
        private string ParametrosInicializacao;
        ParametrosInicializacao ParamIniciais_Server;

        private bool Error = false;
        private Exception DadosError;

        private TcpClient ClientToServer;
        private NetworkStream BarramentoComunicacao;
        private BinaryReader BarramentoLeitura;
        private BinaryWriter BarramentoEscrita;

        public TipoSaidaErros TSaida_Error { get; set; }

        private bool Encriptar { get; set; }
        
        public Cliente_StreamOpen(string IP, int port)
        {
            try
            {
                IPAddress IPS = IPAddress.Parse(IP);
                PORT = port;
                IPServer = new IPEndPoint(IPS, PORT);

            }catch(Exception e)
            {
                TratadorErros(e);
            }
        }

        public void GetParamentrosIniciais()
        {
            try
            {
                ParametrosInicializacao Cliente = new ParametrosInicializacao();
                Cliente.Maquina = Dns.GetHostName();

                ParametrosInicializacao = JsonConvert.SerializeObject(Cliente);
            }
            catch (Exception e)
            {
                TratadorErros(e);
            }

        }

        private void TratarParametrosInicializacao(string Pmt)
        {
            try
            {
                ParamIniciais_Server = JsonConvert.DeserializeObject<ParametrosInicializacao>(Pmt);
                if (1 == 1)
                {

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                TratadorErros(e);
            }

        }
        public bool Connectar_Servidor()
        {

            string DadosLidos;
            try
            {
                TSaida_Error = TipoSaidaErros.ShowWindow;
                /**
                 * Seta os parâmetros de inicialização
                 */
                GetParamentrosIniciais();

                ClientToServer = new TcpClient();
                ClientToServer.Connect(IPServer);
                if (ClientToServer.Connected)
                {
                    BarramentoComunicacao = ClientToServer.GetStream();
                    BarramentoEscrita = new BinaryWriter(BarramentoComunicacao);
                    BarramentoLeitura = new BinaryReader(BarramentoComunicacao);

                    byte[] entrada = new byte[ClientToServer.ReceiveBufferSize];

                    BarramentoEscrita.Write(ASCIIEncoding.UTF8.GetBytes("Cliente enviando dados"));
                    BarramentoLeitura.Read(entrada, 0, ClientToServer.ReceiveBufferSize);

                    Console.WriteLine(ASCIIEncoding.UTF8.GetString(entrada));
                    System.Windows.Forms.MessageBox.Show(ASCIIEncoding.UTF8.GetString(entrada));
                    entrada = new byte[ClientToServer.ReceiveBufferSize];

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                TratadorErros(e);
                return false;
            }

        }
        public  string EnviarReceberAsync(string Dados)
        {
            try
            {
                BarramentoEscrita.Write(Dados);

                byte[] entrada = new byte[ClientToServer.ReceiveBufferSize];
                BarramentoLeitura.Read(entrada, 0, ClientToServer.ReceiveBufferSize);
                Console.WriteLine(ASCIIEncoding.UTF8.GetString(entrada));
                var o =ClientToServer.SendBufferSize;
                return ASCIIEncoding.UTF8.GetString(entrada);


            }
            catch (Exception e)
            {
                TratadorErros(e);
                return "false";
            }
            
        }

        private void TratadorErros(Exception e)
        {
            Error = true;
            DadosError = e;

            switch (e.HResult)
            {
                case 33:

                    break;

                default:
                    Console.WriteLine(e.Message);
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    break;
            }
        }
        public void FecharConexao()
        {
            BarramentoEscrita.Write("1000DX");
            BarramentoEscrita.Flush();
            ClientToServer.Client.Close();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


}
