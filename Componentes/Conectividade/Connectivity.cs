using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Utilidades;

namespace ServerClienteOnline.Conectividade
{
    /**
     * <summary>
     * Struct que mantém uma descrição das interfaces do host.
     * <para> Tipo: Tipo de inteface: Ethernet, Wifi, etc...</para>
     * <para>Interface: Descrição da interface</para>
     * <para>IP: O edereço IPv4 ou IPv6</para>
     * <para>Link: Velocidade da interface</para>
     * <para>Status: Estado atual da interface</para>
     * </summary>
     */
    public struct ListaConexoesHost
    {
        public NetworkInterfaceType Tipo;
        public string Interface;
        public string IP;
        public long Link;
        public OperationalStatus Status;
    }
    class Conexoes
    {
        private static List<ListaConexoesHost> IPIdentificados;
        private static Exception Erros = null;
        public static ConfiguracoesCORAC Config { get; set; }
        public static string EndPadrao = "www.google.com.br";
        /**
         * <summary>
         * Verifica a conectividade do host com a rede local ou internet.
         * <para>
         * <paramref name="Endereco"/>: 
         *              Informando um endereço, o método tentará realizar a verificação da conectividade até o endereço.
         * Caso não seja informado um endereço o sistema tentará acesso ao www.google.com.br. Poderá ser tanto um IP quanto um endereço de domínio.
         * </para>
         * </summary>
         */
        public async static Task<bool> VerificarConectividade(string Endereco = null)
        {
            try
            {
                if (!CXRedeDisponivel()) return false;
                Ping Enviar = new Ping();

                PingOptions Opcoes = new PingOptions();
                Opcoes.DontFragment = true;
                byte[] Bufered = new byte[78];

                
                Endereco = Endereco == null ? EndPadrao: Endereco;
                PingReply Replay = await Enviar.SendPingAsync(Endereco, 1000, Bufered, Opcoes);

                if (Replay.Status == IPStatus.Success) return true; else return false;
            }
            catch (Exception e)
            {
                Erros = e;
                return false;
            }

        }
        /**
            <summary>
                Verifica se existe rede disponível no computador atual.
            </summary>
         */
        public static bool CXRedeDisponivel()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        /**
            <summary>
                Busca as interfaces de rede do computador e retorna informações em uma lista de conexões do host.
            </summary>
         */
        public static List<ListaConexoesHost> ObterIps()
        {
            IPIdentificados = new List<ListaConexoesHost>();

            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface Int in Interfaces)
            {
                  foreach( UnicastIPAddressInformation UniInform in Int.GetIPProperties().UnicastAddresses)
                {
                    IPIdentificados.Add(new ListaConexoesHost { Interface = Int.Name, Link = Int.Speed, Status = Int.OperationalStatus, Tipo = Int.NetworkInterfaceType, IP = UniInform.Address.ToString() });
                  
                }
            }
            return IPIdentificados;
            
        }
        /**
            <summary>
                Busca as interfaces de rede do computador e retorna informações em uma lista de conexões do host.
            </summary>
         */
        public static string EnderecoHttpListen_Powershell()
        {
            return "*";
        }

        /**
            <summary>
                Especifica a url que o sistema ficará escutando.
            </summary>
         */
        public static string EnderecoHttpListen_AcessoRemoto()
        {
            return "*";
        }

    }
}
