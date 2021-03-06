﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Security.Principal;
using System.Net;
using System.Net.Http;
using CamadaDeDados.RESTFormat;
using Newtonsoft.Json;

namespace ServerClienteOnline.MetodosAutenticacao
{
    class Autenticador_livre : IAuth, IServidor
    {
        private bool Active = false;

        Pacote_Auth Livre = new Pacote_Auth();
        public Autenticador_livre()
        {
            Livre.Autenticado = true;
            
        }

        public Pacote_Auth AutenticarUsuario(Pacote_Auth Pacote_Auth)
        {
            Livre.Token = Convert.ToString(GetHashCode());
            return Livre;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public bool StartServidor()
        {
            return true; ;
        }

        public bool StatusServidor()
        {
            return true; ;
        }

        public bool StopServidor()
        {
            return true; 
        }
    }

    class Autenticador_Local : Tratador_Erros ,IAuth, IServidor
    {
        private SecureString CrypSenha = new SecureString();

        private TError SaidaError = new TError { Error = false };

        Pacote_Auth Local = new Pacote_Auth();
        public Autenticador_Local()
        {
            
        }

        public Pacote_Auth AutenticarUsuario(Pacote_Auth Pacote_Auths)
        {
            if(Pacote_Auths.DominioCliente == Pacote_Auths.DominioServidor)
            {
                if (Pacote_Auths.TEndPointClient != Pacote_Auths.TEndPointServer)
                {
                    Local = Pacote_Auths;
                    if (!Local.Autenticado)
                    {
                    }
                    else
                    {

                    }
                    Pacote_Auths.Autenticado = true;
                }
                else
                {
                    Pacote_Auths.Autenticado = false;
                    Pacote_Auths.Error = true;
                    Pacote_Auths.EMensagem = "Atualmente o servidor não é um controlador. Favor direcionar para o controlador da sua rede.";
                }
            }
            else
            {
                Pacote_Auths.Autenticado = false;
                Pacote_Auths.Error = true;
                Pacote_Auths.EMensagem = "Atualmente este servidor não responde por esse domínio. Favor direcionar para o controlador da sua rede.";
            }

            return Pacote_Auths;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool StartServidor()
        {
            throw new NotImplementedException();
        }

        public bool StopServidor()
        {
            throw new NotImplementedException();
        }

        public bool StatusServidor()
        {
            throw new NotImplementedException();
        }
    }

    class Autenticador_WEB : Tratador_Erros, IServidor, IAuthHTML
    {
        private Uri _Servidor;
        private Pacote_Error Error = null;
        private Pacote_Auth Pacote_Autenticacao = null;

        /**
         * <summary>
         * Endereço do servidor de autenticação WEB.
         * <para><paramref name="Servidor"/>Uri do servidor de autenticação.</para>
         * <para>void</para>
         * </summary>
         */
        public void Endereco_Autenticacao(Uri PathURI, string Pasta)
        {
            string pth = PathURI.Scheme + "://" + PathURI.Host + ":" + PathURI.Port + Pasta;
            _Servidor =  new Uri(pth);
            
        }

        public async Task<bool> CheckServidorOnline()
        {
            try
            {
                HttpClient URL = new HttpClient();
                var pairs = new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>("login", "abc")
                                        };

                var content = new FormUrlEncodedContent(pairs);

                URL.Timeout = TimeSpan.FromSeconds(3);

                Task<HttpResponseMessage> Conteudo;

                Conteudo = URL.PostAsync(_Servidor, content);
                await Task.WhenAll(Conteudo);

                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);
                return false;
            }
        }

        public Pacote_Error Get_Error_Servidor_WEB { get { return Error; } }
        public Pacote_Auth GetAutenticacao { get { return Pacote_Autenticacao; } }

        public async Task<bool> HTML_AutenticarUsuario(Pacote_Auth Pacote_AuthWEB)
        {

            try
            {
                HttpClient URL = new HttpClient();
                List<KeyValuePair<string, string>> pairs = Pacote_AuthWEB.ListarAtributos();

                var content = new FormUrlEncodedContent(pairs);

                URL.Timeout = TimeSpan.FromSeconds(3);

                Task<HttpResponseMessage> Conteudo = URL.PostAsync(_Servidor, content);
                await Task.WhenAll(Conteudo);

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    string Dados = await Conteudo.Result.Content.ReadAsStringAsync();
                    Pacote_Base PB = JsonConvert.DeserializeObject<Pacote_Base>(Dados);
                    if(PB.Pacote != TipoPacote.Error)
                    {
                        Pacote_Autenticacao = JsonConvert.DeserializeObject<Pacote_Auth>(PB.Conteudo); ;
                        Pacote_Autenticacao.Autenticado = true;
                        Pacote_Autenticacao.Error = false;


                        return true;
                    }
                    else
                    {
                        Error = JsonConvert.DeserializeObject<Pacote_Error>(PB.Conteudo);
                        return false;

                    }
                }
                else
                {
                    Pacote_AuthWEB.Autenticado = false;
                    Pacote_AuthWEB.Error = true;
                    Error = new Pacote_Error();
                    Error.Error = true;
                    Error.Mensagem = "A conexão com o URi não foi estabelecida com sucesso!";

                    return false;
                }

               
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);
                //Classe Tratador de erros tem métodos para tratar erro a esse nível
                return false;
            }
        }

        public bool StartServidor()
        {
            throw new NotImplementedException();
        }

        public bool StatusServidor()
        {
            throw new NotImplementedException();
        }

        public bool StopServidor()
        {
            throw new NotImplementedException();
        }

        public bool HTML_Autenticado(string Pacote_Auth)
        {
            throw new NotImplementedException();
        }
    }
}
