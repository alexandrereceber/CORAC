﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;

namespace ServerClienteOnline.Gerenciador.ClientesConectados
{
    class GerenciadorClientes : Tratador_Erros, IGCliente, IGClienteHTML, IServidor
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
                foreach (var i in ListaClientes_Conectados)
                {
                    IPEndPoint IPOrigem = (IPEndPoint)Client;
                    IPEndPoint IPDestino = (IPEndPoint)i.Value;

                    if (IPOrigem.Address.Equals(IPDestino.Address))
                    {
                        return false;
                    }
                }
                ListaClientes_Conectados.Add(new KeyValuePair<Pacote_Auth, EndPoint>(Autenticacao, Client));

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
            ListaClientes_Conectados = new List<KeyValuePair<Pacote_Auth, EndPoint>>();
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
