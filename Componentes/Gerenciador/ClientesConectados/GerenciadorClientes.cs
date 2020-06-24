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
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Verifica se o cliente já esta autenticado pelo sistema CORAC.</para>
                <para>Return: ParametrosInicializacao</para>            
            </summary>
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
        /**
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Implementação incompleta.</para>
                <para>Return: bool</para>            
            </summary>
         */
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
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Remove da lista dos clientes conectados, através do endereço IP e Porta, sobre o cliente.</para>
                <para>Return: ParametrosInicializacao</para>            
            </summary>
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
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Remove da lista de clientes conectados, utilizando uma classe abstrata EndPoint.</para>
                <para>Return: void</para>            
            </summary>
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
        /**
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Verifica se o cliente já esta autenticado pelo sistema CORAC.</para>
                <para>Return: ParametrosInicializacao</para>            
            </summary>
         */
        public bool StartServidor()
        {
            return true;
        }
        /**
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Limpa a variável de gerenciamento.</para>
                <para>Return: bool</para>            
            </summary>
         */
        public bool StopServidor()
        {
            ListaClientes_Conectados = null;
            return true;
        }
        /**
            <summary>
                <para>Data: 27/02/2019</para>
                <para>Sem implementação para o método atual.</para>
                <para>Return: ParametrosInicializacao</para>            
            </summary>
         */
        public bool StatusServidor()
        {
            return true;
        }

    }
}
