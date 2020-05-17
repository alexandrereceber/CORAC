﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerClienteOnline.Interfaces;
using System.IO;

namespace ServerClienteOnline.Utilidades
{
    public class ParametrosInicializacao
    {
        [JsonProperty("Maquina")]
        public string Maquina { get; set; }

        [JsonProperty("IP")]
        public string IP { get; set; }

        [JsonProperty("Criptografia")]
        public string Criptografia { get; set; }

        [JsonProperty("TipoCriptografia")]
        public string TipoCriptografia { get; set; }
    }
    [Serializable]
    public class Pacote_Base
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Base;

        [JsonProperty("Conteudo")]
        public string Conteudo = "SubPacote";

        [JsonProperty("Remetente")]
        public Remetente Remetente = Remetente.Cplusplus;


    }

    public class Pacote_Chave
    {
        [JsonProperty("enviarChaves")]
        public string enviarChaves { set; get; }
    }

    [Serializable]
    public class Pacote_Inicializacao : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Inicializacao;

        [JsonProperty("EnderecoIP")]
        public string EnderecoIP { get; set; }

        [JsonProperty("PORT")]
        public int PORT { get; set; }

        [JsonProperty("Criptografia")]
        public string Criptografia { get; set; }

        [JsonProperty("TipoCriptografia")]
        public string TipoCriptografia { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }
    }
    public class Pacote_PingEcho : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Echo;

        [JsonProperty("TempoInicio")]
        public DateTime TempoInicio { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public void ObterTempo(int i = 0)
        {
            TempoInicio = DateTime.Now;
        }


        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    public class Pacote_Error : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Error;

        [JsonProperty("Error")]
        public bool Error { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Numero")]
        public int Numero { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Mensagem;
        }
    }

    /**
    * Pacote referente ao acesso remoto à máquina do agente autônomo.
    */
    public class Pacote_AcessoRemoto : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Comando;

        [JsonProperty("Tipo")]
        public TiposRequisicaoAR Tipo { get; set; } //Poder do tipo liberação

        [JsonProperty("Resposta")]
        public string Resposta { get; set; }

        [JsonProperty("Formato")]
        public TiposSaidas Formato = TiposSaidas.JSON;

        [JsonProperty("Chave")]
        public string Chave = "";

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Resposta;
        }

        public string GetChave()
        {
            return Chave;
        }
    }

    public class Pacote_AcessoRemoto_Resposta : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Comando;

        [JsonProperty("Tipo")]
        public TiposRequisicaoAR Tipo = TiposRequisicaoAR.Resposta; //Poder do tipo liberação

        [JsonProperty("Resposta")]
        public string Resposta { get; set; }

        [JsonProperty("ChaveAR")]
        public string ChaveAR { get; set; }

        [JsonProperty("Formato")]
        public TiposSaidas Formato = TiposSaidas.JSON;

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetChave()
        {
            return "";
        }
        public string GetResultado()
        {
            return Resposta;
        }

    }

    public class Pacote_PingReplay : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Replay;

        [JsonProperty("Validado")]
        public bool Validado = false;

        [JsonProperty("TempoInicio")]
        public DateTime TempoFim { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public void ObterTempo()
        {
            TempoFim = DateTime.Now;
        }

        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    public class Pacote_Auth : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Auth;

        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        [JsonProperty("Habilitado")]
        public bool Habilitado { get; set; }

        [JsonProperty("Dominio")]
        public string Dominio { get; set; }

        [JsonProperty("Senha")]
        public string Senha { get; set; }

        [JsonProperty("Autenticado")]
        public bool Autenticado = false;

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("ChaveAR")]
        public string ChaveAR { get; set; }

        [JsonProperty("TempoSessao")]
        public string TempoSessao { get; set; }

        [JsonProperty("RenovarSessao")]
        public bool RenovarSessao { get; set; }

        [JsonProperty("Error")]
        public bool Error { get; set; }

        [JsonProperty("EMensagem")]
        public string EMensagem { get; set; }

        [JsonProperty("TEndPointClient")]
        public __Autenticacao TEndPointClient { get; set; } /*Envia o tipo de servidor que a estação está atualmente.*/

        [JsonProperty("TEndPointServer")]
        public __Autenticacao TEndPointServer { get; set; } /*Usado pelo servidor*/

        [JsonProperty("DominioCliente")]
        public string DominioCliente { get; set; } /*Usado pelo servidor*/

        [JsonProperty("Autenticacao")]
        public string Autenticacao { get; set; } /*Método de autenticação que será utilizado pelo servidor WEB ldap, ad*/

        [JsonProperty("Dispositivo")]
        public string Dispositivo { get; set; } /*Usado pelo servidor*/

        [JsonProperty("DominioServidor")]
        public string DominioServidor { get; set; } /*Usado pelo servidor*/

        [JsonProperty("Servico")]
        public TipoServico Servico { get; set; } /*Qual serviço requisitou a autenticação: Powershell, Acesso Remoto, Chat*/

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }

        public List<KeyValuePair<string, string>> ListarAtributos()
        {
            List<KeyValuePair<string, string>> Lista = new List<KeyValuePair<string, string>>();
            System.Reflection.PropertyInfo[] Propriedades = GetType().GetProperties();

            foreach (System.Reflection.PropertyInfo i in Propriedades)
            {
                string Valor = Convert.ToString(i.GetValue(this, null));
                Lista.Add(new KeyValuePair<string, string>(i.Name, Valor));
            }


            return Lista;
        }
    }

    public class Pacote_Comando : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Comando;

        [JsonProperty("Comando")]
        public string Comando { get; set; }

        [JsonProperty("Resposta")]
        public string Resposta { get; set; }

        [JsonProperty("Formato")]
        public TiposSaidas Formato = TiposSaidas.JSON;

        [JsonProperty("Chave")]
        public string Chave = "";

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Resposta;
        }

        public string GetChave()
        {
            return Chave;
        }
    }

    public class Pacote_File : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.File;


        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Destino")]
        public string Destino { get; set; }

        [JsonProperty("Lenght")]
        public long Lenght { get; set; }

        [JsonProperty("Existe")]
        public bool Existe { get; set; }

        [JsonProperty("ReceiveBufferCLiente")]
        public int ReceiveBufferCLiente { get; set; }

        [JsonProperty("SendBufferCliente")]
        public int SendBufferCliente { get; set; }

        [JsonProperty("ReceiveBufferServidor")]
        public int ReceiveBufferServidor { get; set; }

        [JsonProperty("SendBufferServidor")]
        public int SendBufferServidor { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }

    }

    public class Pacote_SystemFile : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.FileSystem;

        [JsonProperty("Conteudo")]
        public string Origem { get; set; }
        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }
    }

    public class Pacote_Login : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Login;

        [JsonProperty("Active")]
        public string Origem { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Tusuario")]
        public string Tusuario { get; set; }

        [JsonProperty("Tempo")]
        public string Tempo { get; set; }

        [JsonProperty("ID")]
        public string ID { get; set; }


        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }
    }

    public enum TiposRequisicaoAR{
        Pedido_Acesso = 0,
        Resposta = 1
    }
    public enum TipoServico
    {
        Powershell = 0,
        AcessoRemoto = 1,
        Chat = 2
    }

    public enum TipoSaidaErros {
                                    ShowWindow = 0,
                                    EventWindow = 1,
                                    Console = 2,
                                    Arquivo = 3,
                                    Componente = 4,
                                    ComponenteAndFile = 5
                                };
    public enum TipoChave
    {
        LocalMachine = 0,
        CurrenteUser = 1
    }
    /**
     * <summary>
     * Formato do pacote será transmitido entre os serviços de Cliente/Serviço.
     * </summary>
     */
    public enum TipoPacote     {
                                    Base = 0,
                                    Echo = 1,
                                    Replay = 2,
                                    Comando = 3,
                                    File = 4,
                                    FileSystem = 5,
                                    Auth = 6,
                                    Inicializacao = 7,
                                    Error = 8,
                                    Login = 9,
                                    AcessoRemoto = 10,
                                    AcessoRemoto_Resposta = 11

    };

    public enum TiposSaidas { TXT = 0, JSON = 1, XML = 2, CVS = 3, HTML = 4 }
    /**
     * <summary>
     * Informa ao servidor qual a fonte de dados que será utilizada para autenticar o usuário antes de respondê-lo.
     * <para>LDAP - Autenticação será realizada pelo servidor de domínio da instituição</para>
     * <para>LocalHost - Autenticação será realizada utilizando-se os usuário local da máquina onde o server esta rodando</para>
     * <para>BancoDados - Autenticação será via página de PHP via json em um banco de dados local internet</para>
     * <para>Google - Autenticação será realizada pelo google</para>
     * <para>Livre - Não realização autenticação, ficando o servidor respondendo à qualquer requisiçaõ.</para>
     * * </summary>
     */
    public enum Autenticacao   { 
                                    LDAP = 0,
                                    LocalHost = 1,
                                    BancoDados = 2,
                                    Google = 3,
                                    Livre = 4
                                };

    public struct TError
    {
        public bool Error;
        public string Mensagem;
        public object Pacote;
    }

    public enum __Autenticacao
    {
        Cliente     = 0x00000A,
        Servidor    = 0xFFFFFF
    }

    struct CamposCORAC
    {
        public string Username;
        public string Password;
        public string Path_ServerWEB_CORAC;

    }

    enum StatusRegistro
    {
        Habilitado = 0,
        Desabilitado =1
    }

    public enum Remetente
    {
        PHP = 0,
        Cplusplus = 1
    }

    class RegistroCORAC
    {
        public string Registro;
        public StatusRegistro Status;
        public string Chave_BD = null;
    }
}