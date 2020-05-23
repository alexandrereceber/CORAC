using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;
using Newtonsoft.Json;

namespace ServerClienteOnline.Gerenciador.ClientesConectados
{
    class GerenciadorClientes : Tratador_Erros, IGClienteHTML, IServidor
    {


        /**
         * Data: 22/03/2019
         * Organiza todos os clientes conectador e autenticados do sistema.
         */
        private List<KeyValuePair<Pacote_Auth, EndPoint>> ListaClientes_Conectados;


        /**
          * Data: 27/02/2019
          * Adiciona em uma lista os clientes que se concectaram ao servidor.
          * Return: ParametrosInicializacao
          */
        public bool ConectarCliente(EndPoint Client, Pacote_Auth Autenticacao)
        {
            try
            {
                if(ListaClientes_Conectados != null)
                {
                    foreach (var i in ListaClientes_Conectados)
                    {
                        IPEndPoint IPOrigem = (IPEndPoint)Client;
                        IPEndPoint IPDestino = (IPEndPoint)i.Value;
                        Pacote_Auth User = i.Key;
                        if (IPOrigem.Address.Equals(IPDestino.Address))
                        {
                            if(Autenticacao.Senha == User.Senha)
                            {
                                i.Key.ChaveAR = Autenticacao.ChaveAR;
                            }
                        }
                    }
                }
                else
                {
                    ListaClientes_Conectados = new List<KeyValuePair<Pacote_Auth, EndPoint>>();
                    ListaClientes_Conectados.Add(new KeyValuePair<Pacote_Auth, EndPoint>(Autenticacao, Client));
                }

                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return false;
            }
        }

        /**
  * Data: 27/02/2019
  * Adiciona em uma lista os clientes que se concectaram ao servidor.
  * Return: ParametrosInicializacao
  */
        public bool Validar_Chave_AR(string Chave)
        {
            try
            {
                if (ListaClientes_Conectados != null)
                {
                    foreach (var i in ListaClientes_Conectados)
                    {
                        if (i.Key.ChaveAR == Chave)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
                return false;
            }
        }

        public bool _OAuth(string Chave)
        {
            byte[] BaseByte = Convert.FromBase64String(Chave);
            string StringLogin = ASCIIEncoding.UTF8.GetString(BaseByte);

            Pacote_Login Pct = JsonConvert.DeserializeObject<Pacote_Login>(StringLogin);

            if (ListaClientes_Conectados.Count > 0)
                foreach (var i in ListaClientes_Conectados)
                {

                }
            
            else return false;

            return true;
        }

        /**
          * Data: 27/02/2019
          * Remove os clientes, que desconectaram, da lista de clientes conectados.
          * Return: void
          */
        private void Desconectar_Cliente(string IP, string Porta)
        {
            try
            {
                if (ListaClientes_Conectados.Count > 0)
                    foreach (var i in ListaClientes_Conectados)
                    {
                        System.Net.IPEndPoint Endereco = ((System.Net.IPEndPoint)i.Value);
                        string IPC = Endereco.Address.ToString();
                        string PortaC = Endereco.Port.ToString();

                        if ((IPC == IP) & (PortaC == Porta))
                        {
                            ListaClientes_Conectados.Remove(i);
                            break;
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
          * Remove os clientes, que desconectaram, da lista de clientes conectados.
          * Return: void
          */
        public void Desconectar_Cliente(EndPoint Client)
        {

            try
            {
                foreach (var i in ListaClientes_Conectados)
                {
                    IPEndPoint IPOrigem = (IPEndPoint)Client;
                    IPEndPoint IPDestino = (IPEndPoint)i.Value;

                    if (IPOrigem.Address == IPDestino.Address)
                    {
                        ListaClientes_Conectados.Remove(i);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);
            }

        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool StartServidor()
        {
            return true;
        }

        public bool StopServidor()
        {
            ListaClientes_Conectados = null;
            return true;
        }

        public bool StatusServidor()
        {
            return true;
        }

        public bool ConectarCliente(EndPoint Client, bool Autenticacao)
        {
            throw new NotImplementedException();
        }
    }
}
