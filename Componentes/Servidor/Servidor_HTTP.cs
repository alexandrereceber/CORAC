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
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;
using System.Threading.Tasks;

namespace ServerClienteOnline.Server
{
    class Servidor_HTTP: Tratador_Erros, IDisposable, IServidor
    {
        HttpListener Servidor;
        private bool Active = false;

        public __Autenticacao TEndPoint { get; set; }
        List<string> Prefixos= new List<string>();

        private IRuntime _CMDs;
        private IAuthHTML _Auth;
        private IGClienteHTML _GerenciadorCliente;

         public Servidor_HTTP()
        {
            try
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }
        }

        /**
            <summary>
                Verifica se os serviços auxiliares estão configurados.
            </summary>
         */
        private void Checked_Servicos_Auxiliares()
        {
            try
            {
                if (_CMDs == null) throw new Exception("Nenhum tratador de comados foi identificado.");
                if (_Auth == null) throw new Exception("Nenhum processador de autenticação foi identificado.");
                if (_GerenciadorCliente == null) throw new Exception("Nenhum gerenciado de processo foi identificado.");
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }

        }
        /**
       * Data: 20/04/2019
       * Propriedade que atribui uma classe que executará comandos e retornará uma reposta.
       * Return: IRuntime
       */
        public IRuntime AtribuirExecutor
        {
            set { _CMDs = value; }
        }


        /**
          * Data: 20/04/2019
          * Propriedade que atribui uma classe que executará comandos e retornará uma reposta.
          * Return: IGCliente
          */
        public IGClienteHTML Gerenciador_Cliente
        {
            set { _GerenciadorCliente = value; }
        }

        /**
         * <summary>
         * 
         * </summary>
         */
        public void AddPrefixos(string Protocolo = "http", string Dominio = "*", string Pasta = null, int PORT = 80)
        {
            Protocolo = Protocolo == null ? "http" : Protocolo;
            Dominio = Dominio == null ? "*" : Dominio;
            Pasta = Pasta == null ? "" : Pasta;

            Prefixos.Add(Protocolo + "://" + Dominio + ":" + PORT + "/" + Pasta);
        }

        private void PrefixoServidor()
        {
            if (Prefixos.Count == 0) throw new Exception("Não foram definidos nenhum prefixo.");

            foreach (var i in Prefixos)
            {
                Servidor.Prefixes.Add(i);
            }
        }

        /*Dá início ao servidor*/
        public bool StartServidor()
        {
            try
            {
                Checked_Servicos_Auxiliares();

                Servidor = new HttpListener();
                //Servidor.Prefixes.Add("http://*:"+ Port +"/");
                PrefixoServidor();
                //Servidor.IgnoreWriteExceptions = true;
                Servidor.Start();
                
                IAsyncResult AceitarCliente = Servidor.BeginGetContext(new AsyncCallback(IniciarConversa), Servidor);
                Active = true;
                return true; 
            }
            catch (Exception e)
            {
                Active = false;

                TratadorErros(e, this.GetType().Name);
                return false;
            }

            // Note: The GetContext method blocks while waiting for a request. 
            //HttpListenerContext context = Servidor.GetContext();

            //HttpListenerRequest request = context.Request;
            //// Obtain a response object.
            //HttpListenerResponse response = context.Response;
            //// Construct a response.
            //string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            //// Get a response stream and write the response to it.
            //response.ContentLength64 = buffer.Length;
            //System.IO.Stream output = response.OutputStream;
            //output.Write(buffer, 0, buffer.Length);
            //// You must close the output stream.
            //output.Close();

        }

        /**
          * Data: 27/03/2019
          * Dá início à comunicação, criando uma thread para cada cliente
          * Return: string
          */
        private void IniciarConversa(IAsyncResult s)
        {
            Thread Criar;
            try
            {

                HttpListener Server = (HttpListener)s.AsyncState;
                HttpListenerContext aceita = Server.EndGetContext(s);

                /*Habilita possibilidade de vários tipos de redirecionamentos.*/
                switch (aceita.Request.RawUrl)
                {
                    case "/Pacotes/":
                        Criar = new Thread(Receber_Dados_Responder);
                        Criar.Start(aceita);

                        break;

                    default:
                        Criar = new Thread(Resposta_Padrao);
                        Criar.Start(aceita);

                        break;
                }


                /*Inicia novamente o estado de escuta com o fim de ouvir outros clientes*/
                Server.BeginGetContext(new AsyncCallback(IniciarConversa), Server);

            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

            }


        }

        /**
          * Data: 02/04/2019
          * Propriedade que atribui uma classe para autenticação de máquinas.
          * Return: IAuth
          */
        public IAuthHTML Autenticador
        {
            set { _Auth = value; }
        }


        private void Resposta_Padrao(object Dados)
        {

            HttpListenerContext Conexao = Dados as HttpListenerContext;
            HttpListenerRequest ObterConteudo = Conexao.Request;
            StreamReader _Conteudo = new StreamReader(ObterConteudo.InputStream);

            HttpListenerResponse ObterResposta = Conexao.Response;
            string RPadrao = "Nenhuma página encontrada";
            ObterResposta.ContentLength64 = RPadrao.Length;
            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(RPadrao), 0, RPadrao.Length);
        }
        /**
         * Data: 27/03/2019
         * Método que recebe a requisição WEB do cliente, trata e a responde.
         */
        private void Receber_Dados_Responder(object Dados)
        {
            Pacote_Error P_Error;

            HttpListenerContext Conexao = Dados as HttpListenerContext;
            HttpListenerRequest ObterConteudo = Conexao.Request;
            StreamReader _Conteudo = new StreamReader(ObterConteudo.InputStream);

            HttpListenerResponse ObterResposta = Conexao.Response;

            try
            {

                string DadosPacote;

                string __Conteudo = _Conteudo.ReadLine();
                /*-----------------------Processamento----------------------------*/

                Converter_JSON_String.DeserializarPacote(__Conteudo, out Pacote_Base Base, out object QTP);

                switch (Base.Pacote)
                    {
                        case TipoPacote.Comando:
                        Pacote_Comando CMM = (Pacote_Comando)QTP; //Transforma string em um objeto da classe Pacote_Auth

                        //_GerenciadorCliente?._OAuth(CMM.Chave);

                        bool _Autenticado = (bool)_Auth.HTML_Autenticado(CMM.Chave);

                        if (!_Autenticado) throw new Exception("Usuário não autenticado ou bloqueado.");

                        //Qualifica o tipo de serviço que requisitou a autenticação;
                        _Auth.GetAutenticacao.Servico = TipoServico.AcessoRemoto;

                        _GerenciadorCliente?.ConectarCliente(Conexao.Request.RemoteEndPoint, _Auth.GetAutenticacao);

                        string Executar = _ResponderRequisicao(CMM);

                        if (Excecao)
                        {
                            throw new Exception("Ocorreram erros ao executar o comando!");
                        }

                        if(Executar != null)
                        {
                            Pacote_Comando PCT = new Pacote_Comando();
                            PCT.Resposta = Executar;
                            DadosPacote = Converter_JSON_String.SerializarPacote(PCT);
                            //Obtém o Barramento de escrita com a cliente
                            ObterResposta.ContentLength64 = DadosPacote.Length;
                            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                            ObterResposta.Close();
                        }
                        else
                        {
                            P_Error = new Pacote_Error();
                            P_Error.Error = true;
                            P_Error.Mensagem = "Esse comando não existe.";
                            P_Error.Numero = 50000;

                            DadosPacote = Converter_JSON_String.SerializarPacote(P_Error);
                            ObterResposta.ContentLength64 = DadosPacote.Length;
                            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                            ObterResposta.Close();
                        }


                        break;

                    case TipoPacote.Echo:
                            Pacote_PingReplay RPY = new Pacote_PingReplay();
                            DadosPacote = Converter_JSON_String.SerializarPacote(RPY);
                            //Obtém o Barramento de escrita com a cliente
                            ObterResposta.ContentLength64 = DadosPacote.Length;
                            ObterResposta.OutputStream.Write(ASCIIEncoding.Unicode.GetBytes(DadosPacote), 0, DadosPacote.Length);
                            ObterResposta.Close();
                            break;

                        
                        default:
                            P_Error = new Pacote_Error();
                            P_Error.Error = true;
                            P_Error.Mensagem = "Esse tipo de pacote não existe.";
                            P_Error.Numero = DadosExcecao.HResult;

                            DadosPacote = Converter_JSON_String.SerializarPacote(P_Error);
                            ObterResposta.ContentLength64 = DadosPacote.Length;
                            ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(DadosPacote), 0, DadosPacote.Length);
                            ObterResposta.Close();
                            break;
                    }
                
            }
            catch (Exception e)
            {
                TratadorErros(e, this.GetType().Name);

                P_Error = new Pacote_Error();
                P_Error.Error = Excecao;
                P_Error.Mensagem = DadosExcecao.Message;
                P_Error.Numero = DadosExcecao.HResult;

                string Erros = Converter_JSON_String.SerializarPacote(P_Error);
                ObterResposta.ContentLength64 = Erros.Length;
                ObterResposta.OutputStream.Write(ASCIIEncoding.UTF8.GetBytes(Erros), 0, Erros.Length);
                ObterResposta.Close();

                Excecao = false;
                DadosExcecao = null;
            }


        }

        /**
         * Data: 03/03/2020
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
                TratadorErros(e, this.GetType().Name); ;
                return null;
            }

        }

        public bool StopServidor()
        {
            try { Servidor.Stop(); Active = false; return true; } catch (Exception e) { TratadorErros(e, this.GetType().Name); Active = false; return false; }

        }

        public bool StatusServidor()
        {
             return Active;
        }
    }
}
