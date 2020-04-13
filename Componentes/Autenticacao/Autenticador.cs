using System;
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
        private string _Servidor;

        /**
         * <summary>
         * Endereço do servidor de autenticação WEB.
         * <para><paramref name="Servidor"/>Uri do servidor de autenticação.</para>
         * <para>void</para>
         * </summary>
         */
        public void Endereco_Autenticacao(string Servidor)
        {
            _Servidor =  Servidor;
        }

        public bool CheckServidor()
        {
            try
            {
                WebRequest Status_Servidor =  WebRequest.Create(_Servidor);
                return true;
            }
            catch(Exception e)
            {
                TratadorErros(e, GetType().Name);
                return false;
            }
        }
        public bool HTML_Autenticado(string Pacote_Auth)
        {
            throw new NotImplementedException();
        }

        public Pacote_Auth HTML_AutenticarUsuario(Pacote_Auth Pacote_Auth)
        {
            throw new NotImplementedException();
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
    }
}
