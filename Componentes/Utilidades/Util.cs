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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ServerClienteOnline.Utilidades
{
    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe que representa a inicialização da conversa entre serviços, passando informações da máquina e parâmetros de comunicação.</para>
            <para>Return: ParametrosInicializacao</para>            
        </summary>
    */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe utilizada em todas as comunicações entre clientes e servidores. Principal função é transportar todas as outras classes.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe que transporta a chave de autenticação que foi realizada pelo usuário do sistema.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_Chave
    {
        [JsonProperty("enviarChaves")]
        public string enviarChaves { set; get; }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe utilizada na inicialização da comunicação entre clientes e servidores com propósito de troca de parâmetros inciciais de comunicação.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Pedi resposta a uma outra instância do sistema CORAC desktop.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe que informa que ocorreram erros durante a execução..</para>
            <para>Return: </para>            
        </summary>
     */
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
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações sobre a inicialização do acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
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
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe que transporta as imagens das telas do agente autônomo.</para>
            <para>Return: </para>            
        </summary>
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Realiza o pedido de fechamento da caixa de diálogo do acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_UserCloseDialog : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.User_CloseDialog;

        [JsonProperty("Close")]
        public WebSocketState Close { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Error")]
        public bool Error = false;
        public string GetResultado()
        {
            return Mensagem;
        }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Requisita um pedido de fechamento da conexão estabelecida entre os agentes.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_CloseConection : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Close_Connection;

        [JsonProperty("Close")]
        public WebSocketState Close { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Error")]
        public bool Error = false;
        public string GetResultado()
        {
            return Mensagem;
        }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe que captura as imagens dos dispositivos de imagem.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_FrameTelas : ITipoPacote
    {
        /**
            <summary>
                <para>Data: 22/06/2020</para>
                <para>SubClasse que mantém informações sobre os dispositivos de imagem.</para>
                <para>Return: </para>            
            </summary>
         */
        public class Tela 
        {
            [JsonProperty("Monitor")]
            public string Monitor { get; set; }

            [JsonProperty("Primary")]
            public bool Primary { get; set; }

            [JsonProperty("Screen")]
            public string Screen { get; set; }

            internal Bitmap TelaMonitor = null;
            internal Size T = new Size { };
            internal Point P = new Point { };

            internal Graphics CopyTela = null;

        }

        Tela[] FrameTelas;
        ConfigImagem_Monitor Configuracoes_Gerais = new ConfigImagem_Monitor();
        bool Confirm_GerarTela = true;
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

        public bool GerarTelas()
        {
            //var LogueUI = Process.GetProcessesByName("LogonUI").Length;
            //if(LogueUI == 0)
            //{
            try
            {
                MemoryStream TransformImg = new MemoryStream();

                foreach (Tela TL in FrameTelas)
                {
                    if (Configuracoes_Gerais.Primary == TL.Monitor)
                    {
                        TL.Primary = true;
                        TL.CopyTela.CopyFromScreen(TL.P, new Point { X = 0, Y = 0 }, TL.T);
                        TransformImg = new MemoryStream();
                        TL.TelaMonitor.Save(TransformImg, Configuracoes_Gerais.TiposImagems);
                        TL.Screen = Convert.ToBase64String(TransformImg.ToArray());

                    }
                    else
                    {
                        TL.Primary = false;
                        TL.CopyTela.CopyFromScreen(TL.P, new Point { X = 0, Y = 0 }, TL.T);
                        TransformImg = new MemoryStream();
                        TL.TelaMonitor.Save(TransformImg, Configuracoes_Gerais.TiposImagems);
                        TL.Screen = Convert.ToBase64String(TransformImg.ToArray());

                    }
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
                
            //}
            
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Requisita o pedido de acesso para o WebSocket.</para>
            <para>Return: </para>            
        </summary>
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações dos eventos do mouse enviados do cliente de acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_EventMouse: ITipoPacote
    {
        public TipoPacote Pacote = TipoPacote.EventMouse;

        public bool altKey { get; set; }
        public string Screen { get; set; }
        public bool bubbles { get; set; }
        public int button { get; set; }
        public int buttons { get; set; }
        public bool cancelBubble { get; set; }
        public bool cancelable { get; set; }
        public int clientX { get; set; }
        public int clientY { get; set; }
        public bool composed { get; set; }
        public bool ctrlKey { get; set; }
        public bool defaultPrevented { get; set; }
        public int detail { get; set; }
        public int eventPhase { get; set; }
        public bool isTrusted { get; set; }
        public int layerX { get; set; }
        public int layerY { get; set; }
        public int deltaX { get; set; }
        public int deltaY { get; set; }
        public bool metaKey { get; set; }
        public int movementX { get; set; }
        public int movementY { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public int pageX { get; set; }
        public int pageY { get; set; }
        public int screenX { get; set; }
        public int screenY { get; set; }
        public bool shiftKey { get; set; }
        public string type { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public string GetResultado()
        {
            throw new NotImplementedException();
        }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações do deslocamento do mouse enviados do cliente de acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_MouseRemoto : ITipoPacote
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        enum SendInputEventType : int
        {
            /// <summary>
            /// Contains Mouse event data
            /// </summary>
            InputMouse,
            /// <summary>
            /// Contains Keyboard event data
            /// </summary>
            InputKeyboard,
            /// <summary>
            /// Contains Hardware event data
            /// </summary>
            InputHardware
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MouseInputData
        {
            /// <summary>
            /// The x value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            public int dx;
            /// <summary>
            /// The y value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            public int dy;
            /// <summary>
            /// Wheel event data, X buttons
            /// </summary>
            public uint mouseData;
            /// <summary>
            /// ORable field with the various flags about buttons and nature of event
            /// </summary>
            public MouseEventFlags dwFlags;
            /// <summary>
            /// The timestamp for the event, if zero then the system will provide
            /// </summary>
            public uint time;
            /// <summary>
            /// Additional data obtained by calling app via GetMessageExtraInfo
            /// </summary>
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            /// <summary>
            /// The Mouse Input Data
            /// </summary>
            [FieldOffset(0)]
            public MouseInputData mi;

            /// <summary>
            /// The Keyboard input data
            /// </summary>
            [FieldOffset(0)]
            public KEYBDINPUT ki;

            /// <summary>
            /// The hardware input data
            /// </summary>
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            /// <summary>
            /// The actual data type contained in the union Field
            /// </summary>
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            MOUSEEVENTF_WHEEL = 0x0800
        }

        bool Botao1_press = false;
        bool Botao2_press = false;

        public Pacote_MouseRemoto()
        {

        }

        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.MouseRemoto;

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }
        private void Mouse_Click(Pacote_EventMouse Click)
        {
            INPUT structInput = new INPUT();
            structInput.type = SendInputEventType.InputMouse;
            structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP;
            structInput.mkhi.mi.dx = 500;
            structInput.mkhi.mi.dy = 500;
            uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
        }
        private void Mouse_PressButton(ref Pacote_EventMouse Mouse)
        {

            if (Mouse.buttons == 1)
            {
                if (Botao1_press == false)
                {
                    Botao1_press = true;
                    INPUT structInput = new INPUT();
                    structInput.type = SendInputEventType.InputMouse;
                    structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN;
                    structInput.mkhi.mi.dx = 500;
                    structInput.mkhi.mi.dy = 500;
                    uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));

                }

            }
            else if(Mouse.buttons == 0)
            {
                if(Botao1_press == true)
                {
                    Botao1_press = false;
                    INPUT structInput = new INPUT();
                    structInput.type = SendInputEventType.InputMouse;
                    structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTUP;
                    structInput.mkhi.mi.dx = 500;
                    structInput.mkhi.mi.dy = 500;
                    uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                }

            }
            else if (Mouse.buttons == 2)
            {
                if (Botao2_press == false)
                {
                    Botao2_press = true;
                    INPUT structInput = new INPUT();
                    structInput.type = SendInputEventType.InputMouse;
                    structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.RIGHTDOWN;
                    structInput.mkhi.mi.dx = 500;
                    structInput.mkhi.mi.dy = 500;
                    uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                }

            }

        }
        private void Mouse_Move(Pacote_EventMouse Move)
        {
            Mouse_PressButton(ref Move);
            Cursor.Position = new Point(Move.offsetX, Move.offsetY);

            //foreach (Screen i in Screen.AllScreens)
            //{
            //    if (i.DeviceName.Contains(Move.Screen))
            //    {
            //        if (i.Primary)
            //        {
            //            Cursor.Position = new Point(Move.offsetX, Move.offsetY);
            //            break;
            //        }
            //        else
            //        {
            //            Cursor.Position = new Point(i.Bounds.Width + Move.offsetX, i.Bounds.Height + Move.offsetY);
            //            break;
            //        }

            //    }
            //}
        }

        private void Mouse_ContextMenu(Pacote_EventMouse Move)
        {
            if(Move.button == 2)
            {
                Botao2_press = false;
                INPUT structInput = new INPUT();
                structInput.type = SendInputEventType.InputMouse;
                structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP;
                structInput.mkhi.mi.dx = 500;
                structInput.mkhi.mi.dy = 500;
                uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
            }

        }

        private void Mouse_Whell(Pacote_EventMouse Whell)
        {

                INPUT structInput = new INPUT();
                structInput.type = SendInputEventType.InputMouse;
                structInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_WHEEL;
                structInput.mkhi.mi.mouseData = (uint)(Whell.deltaY == -1 ? Whell.deltaX: Whell.deltaY);
                structInput.mkhi.mi.dx = 500;
                structInput.mkhi.mi.dy = 500;
                uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));

        }
        public bool Gerar_EventoMouse(Pacote_EventMouse Evt)
        {
            Console.WriteLine(Evt.type);

            switch (Evt.type)
            {
                case "click":
                    Mouse_Click(Evt);
                    return true;

                case "mousemove":
                    Mouse_Move(Evt);
                    return true;

                case "contextmenu":
                    Mouse_ContextMenu(Evt);
                    break;

                case "wheel":
                    Mouse_Whell(Evt);
                    break; 

                default:
                    Console.WriteLine("Pacote de eventos de mouse não identificado");
                    return false;
            }


            Console.WriteLine(Evt.type);
            return true;
        }

     }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações dos eventos de tecla do tecado enviados do cliente de acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_TecladoRemoto : ITipoPacote
    {
        const string SHIFT = "+", CTRL = "^", ALT = "%";
        private string TECLAS = "{SHIFT}{CTRL}{ALT}{key}";
        private Dictionary<int, string> MapsTeclas = new Dictionary<int, string>();

        public Pacote_TecladoRemoto()
        {
            MapsTeclas.Add(37, "LEFT");
            MapsTeclas.Add(38, "UP");
            MapsTeclas.Add(39, "RIGHT");
            MapsTeclas.Add(40, "DOWN");

            MapsTeclas.Add(33, "PGUP");
            MapsTeclas.Add(34, "PGDN");

            MapsTeclas.Add(187, "ADD"); 
            MapsTeclas.Add(107, "ADD");
            MapsTeclas.Add(189, "SUBTRACT");
            MapsTeclas.Add(109, "SUBTRACT");
            MapsTeclas.Add(56, "MULTIPLY");
            MapsTeclas.Add(106, "MULTIPLY");
            MapsTeclas.Add(111, "DIVIDE");

        }

        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.TecladoRemoto;

        [JsonProperty("altKey")]
        public bool altKey { get; set; }

        [JsonProperty("bubbles")]
        public bool bubbles { get; set; }

        [JsonProperty("cancelBubble")]
        public bool cancelBubble { get; set; }

        [JsonProperty("cancelable")]
        public bool cancelable { get; set; }

        [JsonProperty("charCode")]
        public int charCode { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("composed")]
        public bool composed { get; set; }

        [JsonProperty("ctrlKey")]
        public bool ctrlKey { get; set; }

        [JsonProperty("defaultPrevented")]
        public bool defaultPrevented { get; set; }

        [JsonProperty("detail")]
        public int detail { get; set; }

        [JsonProperty("eventPhase")]
        public int eventPhase { get; set; }

        [JsonProperty("isComposing")]
        public bool isComposing { get; set; }

        [JsonProperty("isTrusted")]
        public bool isTrusted { get; set; }

        [JsonProperty("key")]
        public string key { get; set; }

        [JsonProperty("keyCode")]
        public int keyCode { get; set; }

        [JsonProperty("metaKey")]
        public bool metaKey { get; set; }

        [JsonProperty("repeat")]
        public bool repeat { get; set; }

        [JsonProperty("returnValue")]
        public bool returnValue { get; set; }

        [JsonProperty("shiftKey")]
        public bool shiftKey { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }

        public bool ChamarTeclas()
        {
            if (key == "Dead") return false;
            switch (keyCode)
            {
                case 16:
                case 18:
                case 17:
                case 91:
                case 219:
                case 222:
                case 179:
                case 177:
                case 176:
                    return false;

            }
            if(key != "?" && keyCode != 32)
            {
                bool ACH = MapsTeclas.TryGetValue(keyCode, out string Tecla);

                key = ACH == true ? Tecla : key;

                if (shiftKey) TECLAS = TECLAS.Replace("{SHIFT}", SHIFT); else TECLAS = TECLAS.Replace("{SHIFT}", "");
                if (altKey) TECLAS = TECLAS.Replace("{ALT}", ALT); else TECLAS = TECLAS.Replace("{ALT}", "");
                if (ctrlKey) TECLAS = TECLAS.Replace("{CTRL}", CTRL); else TECLAS = TECLAS.Replace("{CTRL}", "");

                TECLAS = TECLAS.Replace("key", key);
                Console.WriteLine(TECLAS);
                SendKeys.SendWait(TECLAS);
                return true;
            }
            else
            {
                SendKeys.SendWait(key);
                return true;
            }

        }

    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações, requisitadas pelo cliente, sobre os dispositivos de imagem do servidor.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_AcessoRemoto_Resposta : ITipoPacote
    {

        /**
            <summary>
                <para>Data: 22/06/2020</para>
                <para>Transporta informações sobre os dispositivos de imagem do servidor.</para>
                <para>Return: </para>            
            </summary>
         */
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

        [JsonProperty("Displays")]
        public List<Display> Displays = null;

        public Pacote_AcessoRemoto_Resposta()
        {
            foreach (Screen s in Screen.AllScreens)
            {
                Displays = GetConfig_Video();
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Resposta ao pedido de informações sobre o agente.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações sobre autenticação.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta pedido de executação de comandos do powershell.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_Comando : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Comando;

        [JsonProperty("Comando")]
        public string Comando { get; set; }

        [JsonProperty("Tipo")]
        public String Tipo { get; set; }

        [JsonProperty("Modo")]
        public String Modo { get; set; }

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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações sobre um determinado arquivo.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações sobre uma estrutura de diretório.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_SystemFile : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.FileSystem;

        [JsonProperty("Conteudo")]
        public string Conteudo { get; set; }
        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return "";
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações sobre o usuário e senha para autentição no sistema CORAC WEB.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta mensagem do chat do cliente.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_ChatUser : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Chat_User;

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Hora")]
        public string Hora { get { return DateTime.Now.ToShortTimeString(); } }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Mensagem de que o cliente esta digitando na caixa de mensagem.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_ChatDigitando : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Chat_Digitando;

        [JsonProperty("Digitando")]
        public bool Digitando { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta mensagem do atendente.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_ChatSuporte : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Chat_Suporte;

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("Hora")]
        public string Hora { get { return DateTime.Now.ToShortTimeString(); } }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Transporta informações de autenticãção para a execução do programa de instalação de softwares do sistema CORAC.</para>
            <para>Return: </para>            
        </summary>
     */
    public class Pacote_Credencial : ITipoPacote
    {
        [JsonProperty("Pacote")]
        public TipoPacote Pacote = TipoPacote.Credencial;

        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        [JsonProperty("Senha")]
        public string Senha { get; set; }

        [JsonProperty("Dominio")]
        public string Dominio { get; set; }

        public TipoPacote GetTipoPacote()
        {
            return Pacote;
        }

        public string GetResultado()
        {
            return Pacote.ToString();
        }
    }

    /**
        <summary>
            <para>Data: 23/06/2020</para>
            <para>Transporta informações sobre as configurações do CORAC desktop.</para>
            <para>Return: </para>            
        </summary>
     */
    class ConfiguracoesCORAC
    {
        /**
            <summary>
                <para>Data: 23/06/2020</para>
                <para>Transporta informações de configurações dos serviços disponíveis no CORAC desktop.</para>
                <para>Return: </para>            
            </summary>
         */
        public class Services
        {
            public int PowerShell { set; get; }
            public int AR { set; get; }

        }

        public Services Servicos { get; set; }
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumeração dos tipos de requisição do acesso remoto.</para>
            <para>Return: </para>            
        </summary>
     */
    public enum TiposRequisicaoAR
    {
        Pedido_Acesso = 0,
        Resposta = 1
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos de serviços existentes no servidor.</para>
            <para>Return: </para>            
        </summary>
     */
    public enum TipoServico
    {
        Powershell = 0,
        AcessoRemoto = 1,
        Chat = 2
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera do tipos de mecanismos de acesso que podem ser utilizados.</para>
            <para>Return: </para>            
        </summary>
     */
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

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos de chaves de registro que o agente autônomo reconhece.</para>
            <para>Return: </para>            
        </summary>
     */
    public enum TipoChave
    {
        LocalMachine = 0,
        CurrenteUser = 1
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos de pacotes reconhecidos pelo sistema CORAC desktop.</para>
            <para>Return: </para>            
        </summary>
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
        Close_Connection = 15,
        TecladoRemoto = 16,
        MouseRemoto = 17,
        EventMouse = 18,
        Confirmacao = 19,
        User_CloseDialog = 20,
        Chat_User = 21,
        Chat_Suporte = 22,
        Chat_Digitando = 23,
        Credencial = 24

    };

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos de saída de informações.</para>
            <para>Return: </para>            
        </summary>
     */
    public enum TiposSaidas { TXT = 0, JSON = 1, XML = 2, CVS = 3, HTML = 4 }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Struct sobre informações de erros.</para>
            <para>Return: </para>            
        </summary>
     */
    public struct TError
    {
        public bool Error;
        public string Mensagem;
        public object Pacote;
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos agentes de autenticação existente.</para>
            <para>Return: </para>            
        </summary>
     */
    public enum __Autenticacao
    {
        Cliente = 0x00000A,
        Servidor = 0xFFFFFF
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Campos existentes no registro windows.</para>
            <para>Return: </para>            
        </summary>
     */
    struct CamposCORAC
    {
        public string Path_ServerWEB_CORAC;

    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os estados do registro de máquina no CORAC WEB.</para>
            <para>Return: </para>            
        </summary>
     */
    enum StatusRegistro
    {
        Habilitado = 0,
        Desabilitado = 1
    }

    /**
    <summary>
        <para>Data: 22/06/2020</para>
        <para>Enumera os tipos de linguagens que são reconhecidas pelo sistema.</para>
        <para>Return: </para>            
    </summary>
 */
    public enum Remetente
    {
        PHP = 0,
        Cplusplus = 1,
        Javascript = 2
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Enumera os tipos de imagem que poderão ser utilizadas no sistema.</para>
            <para>Return: </para>            
        </summary>
     */
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
        public StatusRegistro Status = StatusRegistro.Desabilitado;
        public string Chave_BD = null;
    }

    /**
        <summary>
            <para>Data: 22/06/2020</para>
            <para>Classe estática que convert string em classes reconhecidas pelo sitema.</para>
            <para>Return: </para>            
        </summary>
     */
    class Converter_JSON_String
    {
        public static string SerializarPacote(ITipoPacote Conteudo)
        {
            string SubPct = JsonConvert.SerializeObject(Conteudo);

            Pacote_Base PctBase = new Pacote_Base();
            PctBase.Pacote = Conteudo.GetTipoPacote();
            PctBase.Conteudo = SubPct;

            string SerializarPacote = JsonConvert.SerializeObject(PctBase);
            return SerializarPacote + "                    ";

        }

        /**
           * Data: 02/04/2019
           * Transforma uma string em pacote para acesso.
           * Return: string
           */
        public static void DeserializarPacote(string Pacote, out Pacote_Base out_Base, out object Saida)
        {
            try
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

                    case TipoPacote.TecladoRemoto:
                        Pacote_TecladoRemoto Teclado_Remoto = JsonConvert.DeserializeObject<Pacote_TecladoRemoto>(Base.Conteudo);
                        Saida = Teclado_Remoto;
                        break;

                    case TipoPacote.EventMouse:
                        Pacote_EventMouse Mouse_Remoto = JsonConvert.DeserializeObject<Pacote_EventMouse>(Base.Conteudo);
                        Saida = Mouse_Remoto;
                        break;

                    case TipoPacote.Chat_Suporte:
                        Pacote_ChatSuporte Pacote_Suporte = JsonConvert.DeserializeObject<Pacote_ChatSuporte>(Base.Conteudo);
                        Saida = Pacote_Suporte;
                        break;

                    case TipoPacote.Chat_Digitando:
                        Pacote_ChatDigitando Pacote_Dig = JsonConvert.DeserializeObject<Pacote_ChatDigitando>(Base.Conteudo);
                        Saida = Pacote_Dig;
                        break;

                    case TipoPacote.Credencial:
                        Pacote_Credencial Pacote_Credential = JsonConvert.DeserializeObject<Pacote_Credencial>(Base.Conteudo);
                        Saida = Pacote_Credential;
                        break;

                    default:
                        throw new Exception("Tentativa de envio de pacote não reconhecida pelo sistema.");


                }

                out_Base = Base;
            }catch(Exception e)
            {
                Pacote_Error PERR = new Pacote_Error();
                PERR.Error = true;
                PERR.Mensagem = e.Message;
                PERR.Numero = e.HResult;
                Saida = PERR;
                out_Base = new Pacote_Base();
                out_Base.Pacote = TipoPacote.Error;

                Console.Write(e.StackTrace);
            }
            
        }

     }

    //class Mensagem
    //{
    //    public static List<KeyValuePair<string, string>> ProgressoInicializacao = new List<KeyValuePair<string, string>>();
    //}

    class Assinatura
    {
        [JsonProperty("Sistema")]
        public string Sistema { get; set; }

        [JsonProperty("Signacture")]
        public string Signacture { get; set; }

        public bool Valida = false;
    }

    class PerfilCORAC
    {
        public static string UsuarioCORAC   = "CORAC@corax.com";
        public static string SenhaCORAC     = "Al$#2810";
        public static string ChaveLogin     = "";
        public static bool isLogon          = false;
    }
}
