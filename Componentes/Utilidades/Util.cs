using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerClienteOnline.Interfaces;
using System.IO;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Net.WebSockets;

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

    public class Pacote_AcessoRemoto_Config_INIT : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.AcessoRemoto_Config_INIT;

        [JsonProperty("Conteudo")]
        public string Conteudo { get; set; }

        [JsonProperty("DeviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

        [JsonProperty("Chave_AR")]
        public string Chave_AR { get; set; }

        [JsonProperty("Error")]
        public bool Error { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Conteudo;
        }
    }

    /**
     * Pacote vem do cliente, informando qual tela deseja visualizar
     */
    public class ConfigImagem_Monitor : ITipoPacote

    {
        public ConfigImagem_Monitor()
        {
            
        }

        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.FrameTelas;

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Primary")]       /*Informa o nome do display que deseja visualizar*/
        public string Primary = null;

        [JsonProperty("ThumbnailImage")] /*Tamanho da imagem miniaturizada*/
        public Size ThumbnailImage = new Size { Width = 300, Height = 300};

        [JsonProperty("FormatImagem")]
        public TiposImagem FormatImagem = TiposImagem.Jpeg;

        public ImageFormat TiposImagems = ImageFormat.Png;
        public string GetResultado()
        {
            return Mensagem;
        }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public void NormalizarTipo() /*Deve ser chamado logo após a instanciação da classe para normalizar o atributo*/
        {
            switch (FormatImagem)
            {
                case TiposImagem.Png:

                    break;

                default:
                    TiposImagems = ImageFormat.Png;
                    break;
            }
        }
    }

    public class Pacote_CloseConection : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Close_Connection;

        [JsonProperty("Close")]
        public WebSocketState Close { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        string ITipoPacote.GetResultado()
        {
            return Mensagem;
        }

        TipoPacote ITipoPacote.GetTipoPacote()
        {
            return Pacote;
        }
    }
    public class Pacote_FrameTelas : ITipoPacote
    {
        /**
         * Classe convertida em json e enviada para o cliente
         */
        public class Tela 
        {
            [JsonProperty("Monitor")]
            public string Monitor { get; set; }

            [JsonProperty("Primary")]
            public string Primary { get; set; }

            [JsonProperty("ThumbnailImage")]
            public string ThumbnailImage { get; set; }

            internal Bitmap TelaMonitor = null;
            internal Size T = new Size { };
            internal Point P = new Point { };

            internal Graphics CopyTela = null;

        }

        Tela[] FrameTelas;
        ConfigImagem_Monitor Configuracoes_Gerais = new ConfigImagem_Monitor();
        public Pacote_FrameTelas()
        {
            FrameTelas = new Tela[Screen.AllScreens.Length];
            for (int ii = 0; ii < Screen.AllScreens.Length; ii++)
            {
                FrameTelas[ii] = new Tela();
            }
            int Count = 0;
            foreach (Screen i in Screen.AllScreens)
            {
                string M = i.DeviceName.Replace("\\", "").Replace(".", "");
                if (i.Primary)
                {
                    Configuracoes_Gerais.Primary = M;
                }
                FrameTelas[Count].Monitor = M;
                FrameTelas[Count].TelaMonitor = new Bitmap(i.Bounds.Width, i.Bounds.Height);
                FrameTelas[Count].T = new Size { Width = i.Bounds.Width, Height = i.Bounds.Height };
                FrameTelas[Count].P = i.Bounds.Location;
                FrameTelas[Count].CopyTela = Graphics.FromImage(FrameTelas[Count].TelaMonitor);
                Count++;
            }
        }
        internal ConfigImagem_Monitor setConfiguracoesDisplay { set { Configuracoes_Gerais = value; } get { return Configuracoes_Gerais; } }

        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.FrameTelas;

        [JsonProperty("Error")]
        public bool Error { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Telas")]
        public Tela[] Telas { get { return FrameTelas; } }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Mensagem;
        }

        public void GerarTelas()
        {
            MemoryStream TransformImg = new MemoryStream();

            foreach (Tela TL in FrameTelas)
            {
                if(Configuracoes_Gerais.Primary == TL.Monitor)
                {
                    TL.CopyTela.CopyFromScreen(TL.P, new Point { X = 0, Y = 0 }, TL.T);
                    TransformImg = new MemoryStream();
                    TL.TelaMonitor.Save(TransformImg, Configuracoes_Gerais.TiposImagems);
                    TL.Primary = Convert.ToBase64String(TransformImg.ToArray());

                }
                else
                {
                    TL.CopyTela.CopyFromScreen(TL.P, new Point { X = 0, Y = 0 }, TL.T);
                    TransformImg = new MemoryStream();
                    TL.TelaMonitor.Save(TransformImg, Configuracoes_Gerais.TiposImagems);
                    TL.ThumbnailImage = Convert.ToBase64String(TransformImg.ToArray());

                }
            }
        }
    }

    /**
    * Pacote referente ao acesso remoto à máquina do agente autônomo.
    */
    public class Pacote_AcessoRemoto_SYN : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.AcessoRemoto_SYN;

        [JsonProperty("Tipo")]
        public TiposRequisicaoAR Tipo { get; set; } //Poder do tipo liberação

        [JsonProperty("Resposta")]
        public string Resposta { get; set; }

        [JsonProperty("Formato")]
        public TiposSaidas Formato = TiposSaidas.JSON;

        [JsonProperty("Chave")]
        public string Chave = "";

        [JsonProperty("Mecanismo")]
        public TipoMecanismo Mecanismo { set; get; }

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
        public class Display
        {
            [JsonProperty("DeviceName")]
            public string DeviceName { get; set; }

            [JsonProperty("Primary")]
            public bool Primary { get; set; }

            [JsonProperty("X")]
            public int X { get; set; }

            [JsonProperty("Y")]
            public int Y { get; set; }

            [JsonProperty("Width")]
            public int Width { get; set; }

            [JsonProperty("Height")]
            public int Height { get; set; }

            [JsonProperty("Top")]
            public int Top { get; set; }

            [JsonProperty("Right")]
            public int Right { get; set; }

            [JsonProperty("Left")]
            public int Left { get; set; }
        }

        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Comando;

        [JsonProperty("Tipo")]
        public TiposRequisicaoAR Tipo = TiposRequisicaoAR.Resposta;

        [JsonProperty("Resposta")]
        public string Resposta { get; set; }

        [JsonProperty("ChaveAR")] //Chave gerada para uma sessão.
        public string ChaveAR { get; set; }

        [JsonProperty("Formato")]
        public TiposSaidas Formato = TiposSaidas.JSON;

        [JsonProperty("Configuracoes")]
        public List<Display> Configuracoes = null;

        public Pacote_AcessoRemoto_Resposta()
        {
            foreach (Screen s in Screen.AllScreens)
            {
                Configuracoes = GetConfig_Video();
            }
        }

        public List<Display> GetConfig_Video()
        {
            List<Display> Config = new List<Display>();

            foreach (Screen s in Screen.AllScreens)
            {
                Display Dsp = new Display();
                Dsp.DeviceName = s.DeviceName.Replace("\\", "").Replace(".", "");
                Dsp.Primary = s.Primary;
                Dsp.Width = s.Bounds.Width;
                Dsp.Height = s.Bounds.Height;
                Dsp.Right = s.Bounds.Left;
                Dsp.Left = s.Bounds.Left;
                Dsp.Top = s.Bounds.Top;
                Dsp.X = s.Bounds.X;
                Dsp.Y = s.Bounds.Y;

                Config.Add(Dsp);
            }

            return Config;
        }

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

    public enum TiposRequisicaoAR
    {
        Pedido_Acesso = 0,
        Resposta = 1
    }
    public enum TipoServico
    {
        Powershell = 0,
        AcessoRemoto = 1,
        Chat = 2
    }

    public enum TipoMecanismo
    {
        Navegador = 0,
        Desktop = 1
    }
    public enum TipoSaidaErros
    {
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
    public enum TipoPacote
    {
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
        AcessoRemoto_Resposta = 11,
        AcessoRemoto_SYN = 12,
        AcessoRemoto_Config_INIT = 13,
        FrameTelas = 14,
        Close_Connection = 15

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
    public enum Autenticacao
    {
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
        Cliente = 0x00000A,
        Servidor = 0xFFFFFF
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
        Desabilitado = 1
    }

    public enum Remetente
    {
        PHP = 0,
        Cplusplus = 1,
        Javascript = 2
    }

    public enum TiposImagem
    {
        MemoryBmp = 0,
        Bmp = 1,
        Emf = 2,
        Wmf = 3,
        Gif = 4,
        Jpeg = 5,
        Png = 6,
        Tiff = 7,
        Icon = 8
    }
    class RegistroCORAC
    {
        public string Registro;
        public StatusRegistro Status;
        public string Chave_BD = null;
    }

    class Converter_JSON_String
    {
        public static string SerializarPacote(ITipoPacote Conteudo)
        {
            string SubPct = JsonConvert.SerializeObject(Conteudo);

            Pacote_Base PctBase = new Pacote_Base();
            PctBase.Pacote = Conteudo.GetTipoPacote();
            PctBase.Conteudo = SubPct;

            string SerializarPacote = JsonConvert.SerializeObject(PctBase);
            return SerializarPacote + "     ";

        }

        /**
           * Data: 02/04/2019
           * Transforma uma string em pacote para acesso.
           * Return: string
           */
        public static void DeserializarPacote(string Pacote, out Pacote_Base out_Base, out object Saida)
        {
            Pacote_Base Base = JsonConvert.DeserializeObject<Pacote_Base>(Pacote);

            switch (Base.Pacote)
            {
                case TipoPacote.Auth:
                    Pacote_Auth Auth = JsonConvert.DeserializeObject<Pacote_Auth>(Base.Conteudo);
                    Saida = Auth;
                    break;

                case TipoPacote.Comando:
                    Pacote_Comando Exec = JsonConvert.DeserializeObject<Pacote_Comando>(Base.Conteudo);
                    Saida = Exec;
                    break;

                case TipoPacote.File:
                    Pacote_File File = JsonConvert.DeserializeObject<Pacote_File>(Base.Conteudo);
                    Saida = File;
                    break;

                case TipoPacote.FileSystem:
                    Pacote_SystemFile FileSystem = JsonConvert.DeserializeObject<Pacote_SystemFile>(Base.Conteudo);
                    Saida = FileSystem;
                    break;

                case TipoPacote.Echo:
                    Pacote_PingEcho Ping = JsonConvert.DeserializeObject<Pacote_PingEcho>(Base.Conteudo);
                    Saida = Ping;
                    break;

                case TipoPacote.Replay:
                    Pacote_PingReplay Replay = JsonConvert.DeserializeObject<Pacote_PingReplay>(Base.Conteudo);
                    Saida = Replay;
                    break;

                case TipoPacote.Inicializacao:
                    Pacote_Inicializacao Inicializacao = JsonConvert.DeserializeObject<Pacote_Inicializacao>(Base.Conteudo);
                    Saida = Inicializacao;
                    break;

                case TipoPacote.AcessoRemoto_SYN:
                    Pacote_AcessoRemoto_SYN AcessoRemoto = JsonConvert.DeserializeObject<Pacote_AcessoRemoto_SYN>(Base.Conteudo);
                    Saida = AcessoRemoto;
                    break;

                case TipoPacote.AcessoRemoto_Config_INIT:
                    Pacote_AcessoRemoto_Config_INIT Configuracao_Inicial = JsonConvert.DeserializeObject<Pacote_AcessoRemoto_Config_INIT>(Base.Conteudo);
                    Saida = Configuracao_Inicial;
                    break;

                default:
                   throw new Exception("Tentativa de envio de pacote não reconhecida pelo sistema.");
                    

            }

            out_Base = Base;
        }

        internal static string SerializarPacote(Pacote_Base pct)
        {
            throw new NotImplementedException();
        }
    }
}
