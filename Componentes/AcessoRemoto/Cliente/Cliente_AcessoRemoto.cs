using System;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.IO;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.TratadorDeErros;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerClienteOnline.AcessoRemoto.Cliente
{

        class Cliente_AcessoRemoto : Tratador_Erros,IDisposable
    {

        private IPEndPoint IPServer;
        private int PORT;
        private string EDominio = null; //Endereço do servidor que responderá essa estação.

        Pacote_Auth Authic = new Pacote_Auth();


        private TcpClient ClientToServer;
        private NetworkStream BarramentoDados;
        
        Task TReceber, TEnviar;


        public Cliente_AcessoRemoto(string IP, int port)
        {
            try
            {
                IPAddress IPS = IPAddress.Parse(IP);
                PORT = port;
                IPServer = new IPEndPoint(IPS, PORT);

            }catch(Exception e)
            {
                TratadorErros(e, this.GetType().Name);
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

            try
            {
                /**
                 * Seta os parâmetros de inicialização
                 */
                ClientToServer = new TcpClient();
                ClientToServer.Connect(IPServer);
                if (ClientToServer.Connected)
                {
                    BarramentoDados = ClientToServer.GetStream();
                    BinaryFormatter Serial = new BinaryFormatter();

                    BinaryWriter MM = new BinaryWriter(BarramentoDados);
                    MemoryStream pp = new MemoryStream();

                    byte[] Entr = new byte[pp.Length];
                    pp.Position = 0;
                    pp.Read(Entr, 0, (int)pp.Length);
                    MM.Write(Entr);
                    MM.Flush();
                    return true;
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

        public async void Receber_Pacotes()
        {
            BinaryReader BarramentoLeitura = new BinaryReader(BarramentoDados);
            
        }

        public async void Enviar_Pacotes(string Dados)
        {
                BinaryWriter BarramentoEscrita = new BinaryWriter(BarramentoDados);
                BarramentoEscrita.Write(Dados);
        }


        public void FecharConexao()
        {
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
