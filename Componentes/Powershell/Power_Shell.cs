using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Collections.ObjectModel;
using ServerClienteOnline.Interfaces;
using Microsoft.PowerShell;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.WMIs;
using System.IO;
using Newtonsoft.Json;
using CamadaDeDados.RESTFormat;
using Newtonsoft.Json.Linq;
using CORAC;
using System.Threading;

/**
 * CORAC - Controle Operacional Remoto de Acesso Centralizado
 **/
namespace Power_Shell.AmbienteExecucao
{
    /**
        <summary>
            <para>Data: 23/06/2020</para>
            <para>Representa as informações básica que serão informadas ao sistema de gereciamento central WEB CORAC.</para>
            <para>Return: </para>            
        </summary>
     */
    //class InformacoesGerais
    //{
    //    public string Usuario { get; }
    //    public string PlacaMae { get; }
    //    public string NumeroSerie { get; }
    //    public string Versao { get; }
    //    public string SO { get; }
    //    public string SOCaption { get; }
    //    public string Processador { get; }
    //    public int Memoria { get; }
    //    public int Antivirus { get; }
    //    public InformacoesGerais()
    //    {
    //        //Usuario = ((string)Get_WMI.Obter_Atributo("Win32_ComputerSystem", "Username")).Replace("\\","-");
    //        PlacaMae = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "Manufacturer");
    //        NumeroSerie = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "SerialNumber");
    //        Versao = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "Version");
    //        SOCaption = Get_WMI.Obter_Atributo("Win32_OperatingSystem", "Caption");
    //        SO = Get_WMI.Obter_Atributo("Win32_OperatingSystem", "OSArchitecture");
    //        Memoria = (int)Get_WMI.Obter_Atributo("Win32_OperatingSystem", "TotalVisibleMemorySize");
    //        Processador = Get_WMI.Obter_Atributo("Win32_Processor", "Name");
            
    //    }


    //}

    public struct Erros
    {
        public bool error;
        public string Mensagem;
        public string Trilha;
    }

    /**
        <summary>
            <para>Data: 23/06/2020</para>
            <para>Representa o ambiente de execução poweshell.</para>
            <para>Return: </para>            
        </summary>
     */
    class Ambiente_PowerShell : Tratador_Erros, IRuntime, IServidor, IDisposable
    {
        private SecureString CrypSenha = new SecureString();
        private PSCredential Credenciais;

        private PowerShell Servidor = null;
        private string LinhaComando;
        private TiposSaidas TSaida = TiposSaidas.HTML;

        private Collection<PSObject> OutComandos_Script;
        private Collection<PSObject> OutComandos_Unico;

        private string PathCORACWEB = null;
        private List<KeyValuePair<string, string>> FunctionsBD = null; 
        private List<KeyValuePair<string, string>> FunctionsBDCMPExcluidos = null;

        private bool Active = false;

        string Result;

        public string Comando {
            get { return LinhaComando; }
            set { var LinhaComando = value; }
        }


        public Ambiente_PowerShell()
        {
        }
        ~Ambiente_PowerShell()
        {
            Servidor = null;
            FunctionsBD = null;
            FunctionsBDCMPExcluidos = null;
            OutComandos_Script = null;
        }

        public bool AdicionarScriptsAmbienteExecucao()
        {
            if(Servidor != null)
            {
                foreach(KeyValuePair<string, string> i in FunctionsBD)
                {
                    Servidor.AddScript(i.Value);

                }
                try
                {
                    Collection<PSObject> Resultado = Servidor.Invoke();


                    CORAC_TPrincipal.MsgIniciar.Add("As funções foram carregadas com sucesso no ambiente powershell." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                }
                catch (Exception e)
                {
                    CORAC_TPrincipal.MsgIniciar.Add("Error Powershell: " + e.Message);
                }

            }
            return true;

        }
        public async void LoadsFunctionsBD()
        {
            while (true)
            {
                if (PerfilCORAC.isLogon)
                {
                    Uri EndURI = new Uri(PathCORACWEB);
                    string pth = EndURI.Scheme + "://" + EndURI.Host + ":" + EndURI.Port + "/CORAC/ControladorTabelas/";
                    Tabelas BuscarRegistro_CORAC = new Tabelas(pth, PerfilCORAC.ChaveLogin);

                    List<KeyValuePair<int, string[]>> FiltrosB = new List<KeyValuePair<int, string[]>>();
                    FiltrosB.Add(new KeyValuePair<int, string[]>(0, new string[4] { "6", "=", "Load", "0" }));
                    BuscarRegistro_CORAC.setFiltros(TipoFiltro.Buscar, FiltrosB);
                    BuscarRegistro_CORAC.sendTabela = "5ca5579ec4bd2e5ca5d9608be68ae733";

                    await BuscarRegistro_CORAC.SelectTabelaJSON();

                    if (!BuscarRegistro_CORAC.getError)
                    {

                        JProperty Dados = BuscarRegistro_CORAC.getDados().ResultadoDados;
                        if (Dados.Value.HasValues)
                        {
                            /**
                             * Destroy as variável de memoria da tabelaHTML;
                             */
                            BuscarRegistro_CORAC.Dispose();
                            CORAC_TPrincipal.MsgIniciar.Add("As funções foram carregadas da base de dados CORAC." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                            FunctionsBD = new List<KeyValuePair<string, string>>();
                            FunctionsBDCMPExcluidos = new List<KeyValuePair<string, string>>();

                            foreach (JArray i in Dados.Value)
                            {
                                if((int)i[8] == 1)
                                {
                                    FunctionsBD.Add(new KeyValuePair<string, string>((string)i[2], (string)i[3]));
                                    if ((string)i[6] != "")
                                        FunctionsBDCMPExcluidos.Add(new KeyValuePair<string, string>((string)i[2], (string)i[7]));
                                }
                            }

                            AdicionarScriptsAmbienteExecucao();
                            break;
                        }
                        else
                        {
                            CORAC_TPrincipal.MsgIniciar.Add("Não foi encontrada nenhuma função a ser carregada." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            Thread.Sleep(CORAC_TPrincipal.TimeSleep);
                        }
                    }
                    else
                    {
                        CORAC_TPrincipal.MsgIniciar.Add("Error: " + BuscarRegistro_CORAC.GetInfoError().Mensagem + " - Tempo: " + DateTime.Now.ToString() + "\n");
                        Thread.Sleep(CORAC_TPrincipal.TimeSleep);
                    }
                }
                else
                {
                    CORAC_TPrincipal.MsgIniciar.Add("Chave de acesso ao CORAC WEB não foi carregada." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                    Thread.Sleep(CORAC_TPrincipal.TimeSleep);
                }

            }

        }

        public string setPath_CORACWEB { set { PathCORACWEB = value; } }
        /**
            <summary>
                <para>Data: 23/06/2020</para>
                <para>Inicia o serviço de powershell.</para>
                <para>Return: </para>            
            </summary>
         */
        public bool StartServidor()
        {
            try
            {
                TSaida_Error = TipoSaidaErros.Arquivo;
                Servidor = PowerShell.Create();

                Task BuscarFunctions = new Task(LoadsFunctionsBD);
                BuscarFunctions.Start();

                Active = true;
                return true;
            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                Active = false;
                return false;
            }
        }


        /**
         * <summary>
         * Gera a saída para dentro da variável Resulta.
         * <para>Método Síncrono</para>
         * </summary>
         */
        private bool gerarSaida()
        {
            try
            {
                foreach (var i in OutComandos_Script)
                {
                    Result += (i.BaseObject.ToString()).Replace("\r", "").Replace("\n", "");
                }
                //Result = "[" + Result + "]";
                return true;
            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                return false;
            }


        }

        /**
         * <summary>
         * Executa comandos de powershell.
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
         * <para>Método Assíncrono</para>
         * </summary>
         */
        public bool BeginInvoke_ExecutarScript(string Comando)
        {
            string output;
            try
            {
                LinhaComando = Comando;

                switch (TSaida)
                {
                    case TiposSaidas.HTML:
                        output = "|ConvertTo-Html";
                        break;

                    case TiposSaidas.CVS:
                        output = "|ConvertTo-CVS";
                        break;

                    case TiposSaidas.JSON:
                        output = "|ConvertTo-Json";
                        break;

                    case TiposSaidas.XML:
                        output = "|ConvertTo-XML";
                        break;

                    case TiposSaidas.TXT:
                        output = "";
                        break;

                    default:
                        output = "ConvertTo-Html";
                        break;
                }

                LinhaComando += output;
                Servidor.AddScript(LinhaComando);

                PSDataCollection<PSObject> SaidaDados = new PSDataCollection<PSObject>();
                SaidaDados.DataAdded += new EventHandler<DataAddedEventArgs>(BeginInvoke_Saida);

                IAsyncResult Resultado = Servidor.BeginInvoke<PSObject, PSObject>(null, SaidaDados);

                Servidor.InvocationStateChanged += new EventHandler<PSInvocationStateChangedEventArgs>(BeginInvoke_Concluido);
                return true;

            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                return false;
            }
        }


        /**
         * <summary>
         * Gera a saída em texto.
         * <para>Método Síncrono</para>
         * </summary>
         */
        public void BeginInvoke_Saida(object sender, DataAddedEventArgs e)
        {
            try
            {
                PSDataCollection<PSObject> Conversor = (PSDataCollection<PSObject>)sender;
                Collection<PSObject> OutComandos_Script = Conversor.ReadAll();

                foreach (var i in OutComandos_Script)
                {
                    Result += i.BaseObject.ToString(); //pode gerr conflitos em outros geradores, cuidado!
                }

            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
            }


        }

        /**
         * <summary>
         * Executa o método após a execução do comando, powershell, ter sido concluído.
         * <para><paramref name="sender"/> - Instância do método que chamou esse método.</para>
         * <para><paramref name="e"/> - Parâmetro que representa a invocação do método.</para>
         * <para>Método Assíncrono</para>
         * </summary>
         */
        public void BeginInvoke_Concluido(object sender, PSInvocationStateChangedEventArgs e)
        {
            try
            {
                //string Result = "";

            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
            }


        }

        public PSCredential GetCredencial()
        {
            return Credenciais;
        }

        /**
         * <summary>
         * Gera a saída em texto.
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
         * <para>Método Síncrono</para>
         * </summary>
         */
        public void setCredenciais(string Usuario, string Senha)
        {
            try
            {
                foreach (char i in Senha)
                {
                    CrypSenha.AppendChar(i);
                }

                Credenciais = new PSCredential(Usuario, CrypSenha);

            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
            }
        }
        /**
          * <summary>
          * <para>Data Criação: 31/03/2020</para>
          * Método síncrono, utilizado para realizar a execução de um comando ou script, powershell em Collection.
          * <para><paramref name="Comando"/> - Comandoou script que será executado no espaço do powershell</para>
          * <para>Método Síncrono</para>
          * <para>Retorno: boolean</para>
          * </summary>
          */
        public bool ExecutarScript_BD(string Comando)
        {
            try
            {

                if (PerfilCORAC.isLogon)
                {

                    Uri EndURI = new Uri(PathCORACWEB);
                    string pth = EndURI.Scheme + "://" + EndURI.Host + ":" + EndURI.Port + "/CORAC/ControladorTabelas/";
                    Tabelas BuscarRegistro_CORAC = new Tabelas(pth, PerfilCORAC.ChaveLogin);

                    List<KeyValuePair<int, string[]>> FiltrosB = new List<KeyValuePair<int, string[]>>();
                    FiltrosB.Add(new KeyValuePair<int, string[]>(0, new string[4] { "6", "=", "Runtime", "0" }));
                    FiltrosB.Add(new KeyValuePair<int, string[]>(0, new string[4] { "2", "=", Comando, "1" }));
                    BuscarRegistro_CORAC.setFiltros(TipoFiltro.Buscar, FiltrosB);
                    BuscarRegistro_CORAC.sendTabela = "5ca5579ec4bd2e5ca5d9608be68ae733";

                    Task<bool> BuscarScriptBD = BuscarRegistro_CORAC.SelectTabelaJSON();
                    BuscarScriptBD.Wait();

                    if (!BuscarRegistro_CORAC.getError)
                    {

                        JProperty Dados = BuscarRegistro_CORAC.getDados().ResultadoDados;
                        if (Dados.Value.HasValues)
                        {
                            /**
                             * Destroy as variável de memoria da tabelaHTML;
                             */
                            BuscarRegistro_CORAC.Dispose();

                            JArray LoadBD = (JArray)Dados.Value;
                            string LoadScriptBD = (string)LoadBD[0][3];

                            Servidor.AddScript(LoadScriptBD);
                            return true;

                        }
                        else
                        {
                            Result = "Não foi encontrada nenhuma função a ser carregada." + " - Tempo: " + DateTime.Now.ToString();
                            return false;
                        }
                    }
                    else
                    {
                        Result = "Error: " + BuscarRegistro_CORAC.GetInfoError().Mensagem + " - Tempo: " + DateTime.Now.ToString();
                        return false;
                    }
                }
                else
                {
                    Result  = "Chave de acesso ao CORAC WEB não foi carregada." + " - Tempo: " + DateTime.Now.ToString();
                    return false;
                }


            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                Result = ex.Message;

                return false;
            }
        }
       
        /**
          * <summary>
          * <para>Data Criação: 31/03/2020</para>
          * Método síncrono para gerar a saída em formato JSON, baseado ou não em uma matrix de campos e uma outra de exclusão.
          * <paramref name="Objeto"/>Representa, em Collection Object, uma saída de comando enviada para o ambiente powershell.
          * <paramref name="Campos"/> Representa os campos que serão gerados como saída.
          * <paramref name="Excluidos"/> Representa os campos que serão excluídos da montagem json.
          * <para>A saída é enviada para a variável result que é uma variável global dentro dessa classe</para>
          * <para>Método Síncrono</para>
          * <para>Retorno: boolean</para>
          * </summary>
          * */
        private bool Transfor_JSON(ref Collection<PSObject> Objeto, string Campos = null, string Excluidos = null)
        {
            if (Objeto[0].BaseObject.GetType().Name == "String") { Result = (string)Objeto[0].BaseObject; return true; };
            string Linha = "";
            string _JSON = "";
            string[] _Campos;
            string[] _Excluidos;

            char[] Separador = new char[] { ',' };
            bool Lista_Incluidos = false;
            bool Lista_Excluidos = false;

            _Campos = Campos !=null ? Campos.Split(Separador, StringSplitOptions.None): null;
            _Excluidos = Excluidos != null ? Excluidos.Split(Separador, StringSplitOptions.None) : null;

            foreach (var i in Objeto)
            {
                int count = 0;
                foreach(var ii in i.Properties)
                {
                    Lista_Incluidos = _Campos != null ? _Campos.Contains<string>(ii.Name) : true;
                    Lista_Excluidos = _Excluidos != null ? _Excluidos.Contains<string>(ii.Name) : false;

                    if (!Lista_Incluidos | Lista_Excluidos) continue;
                    try
                    {
                        if(ii.Value != null)
                        {
                            if (ii.Value.GetType().Name == "String")
                                Linha += (count == 0 ? "" : ", ") + "\"" + ii.Name + "\"" + " : " + "\"" + ii.Value.ToString().Replace("\\", "#") + "\"";
                            else
                                Linha += (count == 0 ? "" : ", ") + "\"" + ii.Name + "\"" + " : " + "\"" + ii.Value + "\"";
                        }
                        else
                        {
                            Linha += (count == 0 ? "" : ", ") + "\"" + ii.Name + "\"" + " : " + "\"" + ii.Value + "\"";
                        }

                    }
                    catch (Exception e)
                    {
                        Linha += (count == 0 ? "" : ", ") + "\"" + ii.Name + "\"" + " : \"Error\"";
                    }
                    count = 1;
                }
                _JSON += "{ " + Linha + " }";
                Linha = "";
            }
            Result = "[" + _JSON + "]";
            return true;
        }


        /**
          * <summary>
          * <para>Data Criação: 09/07/2020</para>
          * Busca os campos de um determinado comando que deverão ser excluídos na criação do json. Obs: Seu uso se dá muitas vezes por ter campos incompatível com json enviando caracteres que confundem o json, principalmente no PHP.
          * <paramref name="Campos"/> Representa os campos que serão gerados como saída.
          * <para>Método Síncrono</para>
          * <para>Retorno: string</para>
          * </summary>
          * */
        private string ObterCMPExcluidosBD(string Comando)
        {
            if(FunctionsBDCMPExcluidos != null)
                if(FunctionsBDCMPExcluidos.Count > 0)
                    foreach(KeyValuePair<string, string> i in FunctionsBDCMPExcluidos)
                    {
                        if(i.Key.Equals(Comando,StringComparison.OrdinalIgnoreCase))
                        {
                            return i.Value;
                        }
                    }
            return "";
        }

        /**
          * <summary>
          * <para>Data Criação: 09/07/2020</para>
          * Busca os campos de um determinado comando que deverão ser excluídos na criação do json.
          * <paramref name="Camando"/> Representa o comando que deverá ser executado no ambiente powershell.
          * <paramref name="Parametros"/> Representa os campos que serão gerados como saída json.
          * <para>Método Síncrono</para>
          * <para>Retorno: string</para>
          * </summary>
          * */
        private bool OtherCommands(string Comando, string[] Parametros)
        {
            try
            {
                int i = 1;
                string TCampos = null;
                string ExcluidosBD = ObterCMPExcluidosBD(Comando);

                string Excluidos = "Path,Options,ClassPath,Properties,SystemProperties,Qualifiers,Scope,__NAMESPACE,__PATH,__RELPATH" + (ExcluidosBD == "" ? "" : ("," + ExcluidosBD));

                if (Parametros.Contains<string>("-campos"))
                {
                    for (i = 1; i <= Parametros.Length; i++)
                    {
                        if (Parametros[i] == "-campos") { TCampos = Parametros[i + 1]; break; } else TCampos = null;
                    }
                }

                Servidor.AddScript(Comando);
                OutComandos_Script = Servidor.Invoke();
                Servidor.Commands.Clear();

                if (OutComandos_Script.Count == 0) return false;

                switch (TSaida)
                {
                    case TiposSaidas.HTML:
                        break;

                    case TiposSaidas.CVS:
                        break;

                    case TiposSaidas.JSON:
                        return Transfor_JSON(ref OutComandos_Script, TCampos, Excluidos);


                    case TiposSaidas.XML:
                        break;

                    case TiposSaidas.TXT:
                        break;

                    default:
                        break;
                }

                return false;
            }catch(Exception e)
            {
                SetTratador_Erros(TipoSaidaErros.Arquivo);
                TratadorErros(e, this.GetType().Name);
                return false;
            }
            
        }
        /**
          * <summary>
          * <para>Data Criação: 31/03/2020</para>
          * <para>Método síncrono que verifica se existe uma personalização da saída de comandos powershell.</para>
          * <paramref name="CMD"/> Representa o pacote de comando enviado pelo cliente para execução no ambiente powershell.
          * <para>Retorno: Boolean</para>
          * </summary>
          * */
        private bool Get_Comandos(Pacote_Comando CMD = null)
        {

            string Comando = CMD?.Comando;
            
            if(Comando != null && Comando != "")
            {
                char[] Separador = new char[]{' '};
                string[] Partes_Comando = Comando.Split(Separador,StringSplitOptions.None);
                Partes_Comando[0] = Partes_Comando[0].Replace("-", "_");
                //System.Reflection.MethodInfo Metodo = GetType().GetMethod(Partes_Comando[0]);

                if (CMD.Modo == "RUNTIME")
                {
                    bool Encontrou = ExecutarScript_BD(Partes_Comando[0]);
                    if (!Encontrou)
                    {
                        return false;
                    }
                }

                return OtherCommands(Comando, Partes_Comando);

                //switch (Partes_Comando[0])
                //{
                //    case "get_process":
                //        if (get_process(Partes_Comando))
                //            return true;
                //        else
                //            return false;

                //    case "get_childrem":
                //        if (get_childrem(Partes_Comando))
                //            return true;
                //        else
                //            return false;


                //    default:
                //        return OtherCommands(Comando, Partes_Comando);
                //}
            }
            else
            {
                return false;
            }

        }

        private string Get_ScriptCompleto(Pacote_Comando Script = null)
        {
            return "";
        }
        public bool Route(Pacote_Comando PCT)
        {
            TSaida = PCT.Formato;
            
            if (Get_Comandos(PCT))
                return true;
            else return false;

        }

        public void Dispose()
        {
            Servidor.Stop();
        }

        public bool StopServidor()
        {
            try { Servidor.Stop(); Active = false; return true; } catch (Exception e) { TratadorErros(e, this.GetType().Name); Active = false; return false; }

        }

        public bool StatusServidor()
        {
            return Active;
        }

        public string Get_Resultado()
        {
            string Saida = Result.Replace("\r","").Replace("\n","").Replace("\\","#");
            Result = null;
            return Saida;
        }
    }
}
