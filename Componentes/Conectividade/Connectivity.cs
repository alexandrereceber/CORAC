using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

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

        /**
         * <summary>
         * Verifica a conectividade do host para a rede local ou internet.
         * <para>
         * <paramref name="Endereco"/> Informando um endereço, o método tentará realizar a verificação da conectividade até o endereço.
         * Caso não seja informado um endereço o sistema tentará acesso ao www.google.com.br. Poderá ser tanto um IP quanto um endereço de domínio.
         * </para>
         * </summary>
         */
        public static bool VerificarConectividade(string Endereco = null)
        {
            try
            {
                if (!CXRedeDisponivel()) return false;
                Ping Enviar = new Ping();

                PingOptions Opcoes = new PingOptions();
                Opcoes.DontFragment = true;

                Endereco = Endereco == null ? "www.google.com.br": Endereco;
                PingReply Replay = Enviar.Send(Endereco);

                if (Replay.Status == IPStatus.Success) return true; else return false;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public static bool CXRedeDisponivel()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

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
    }
}
