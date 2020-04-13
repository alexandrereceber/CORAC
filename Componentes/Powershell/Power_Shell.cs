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
using System.IO;

/**
 * CORAC - Controle Operacional Remoto de Acesso Centralizado
 **/
namespace Power_Shell.AmbienteExecucao
{
    public struct Erros
    {
        public bool error;
        public string Mensagem;
        public string Trilha;
    }

    class Ambiente_PowerShell : Tratador_Erros, IRuntime, IServidor
    {
        private SecureString CrypSenha = new SecureString();
        private PSCredential Credenciais;

        private PowerShell Servidor;
        private string LinhaComando;
        private TiposSaidas TSaida = TiposSaidas.HTML;
        private Collection<PSObject> OutComandos_Unico;

        private Collection<PSObject> OutComandos_Script;

        private Erros Erro = new Erros { error = false, Mensagem = "", Trilha = "" };
        private bool Active = false;

        string Result;

        public string Comando {
            get { return LinhaComando; }
            set { var LinhaComando = value; }
        }


        public Ambiente_PowerShell()
        {

        }

        public bool StartServidor()
        {
            try
            {
                TSaida_Error = TipoSaidaErros.ShowWindow;
                Servidor = PowerShell.Create();
                Active = true;
                return true;
            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                Active = true;
                return false;
            }
        }
        /**
         * <summary>
         * Configura o tipo de saída que será gerada pelo executor de comandos.
         * <para><paramref name="sda"/> - Parâmetro com entradas - TipoDaida: JSON; XML; HTML; CSV</para>
         * <para></para>
         * <para>Método Síncrono</para>
         * </summary>
         */
        public void tipoSaida(TiposSaidas sda)
        {
            TSaida = sda;
        }

        /**
         * <summary>
         * Executar apena um comando por vez, caso precise de parâmetros tem que adicioná-los.
         * <para></para>Retorna apenas true ou false. True para execução foi concluída com sucesso e false para algum tipo de erro.
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
         * <para><paramref name="Parametros"/> - Parametros que serão utilizados pelo comando.</para>
         * <para>Método Síncrono</para>
         * </summary>
         */
        public bool ExecutarComando(string Comando, List<string> Parametros = null)
        {
            try
            {
                Servidor.AddCommand(Comando);

                if (Parametros != null)
                {
                    foreach(var i in Parametros)
                    {
                        Servidor.AddParameter(i);
                    }
                }
                OutComandos_Unico = Servidor.Invoke();
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
         * Método assíncrono, utilizado para realizar a execução de um comando, powershell, e retornar de um dos formatos
         * existentes json, html, cvs, xml
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
         * <para>Método Síncrono</para>
         * </summary>
         */
        public bool ExecutarScript(string Comando)
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
                
                OutComandos_Script = Servidor.Invoke();
                
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
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
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
                Result = "[" + Result + "]";
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
         * <para><paramref name="Comando"/> - Comando que será executado no espaço do powershell</para>
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
        public Collection<PSObject> ExecutarScript_Personalizados(string Comando)
        {
            try
            {
                LinhaComando = Comando;
                Servidor.AddScript(LinhaComando);
                OutComandos_Script = Servidor.Invoke();

                return OutComandos_Script;
            }
            catch (Exception ex)
            {
                TratadorErros(ex, this.GetType().Name);
                return null;
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
                        Linha += (count == 0 ? "" : ", ") + "\"" + ii.Name + "\"" + " : " + "\"" + ii.Value + "\"";
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
            return false;
        }
        public bool get_childrem(string[] Parametros)
        {
            Collection<PSObject> Saida = ExecutarScript_Personalizados("Get-ChildItem");
            return false;
        }

        public bool get_process(string[] Parametros)
        {
            int i = 1;
            string TCampos = null;

            string Excluidos = "StartTime";

            if (Parametros.Contains<string>("-campos"))
            {
                for (i = 1; i <= Parametros.Length; i++)
                {
                    if (Parametros[i] == "-campos") { TCampos = Parametros[i + 1]; break; } else TCampos = null;
                }
            }


            

            Collection<PSObject> Saida = ExecutarScript_Personalizados("get-process");
            if (Saida == null) throw new Exception("A saída do powershell não retornou nenhum valor. get_process");

            switch (TSaida)
            {
                case TiposSaidas.HTML:
                    break;

                case TiposSaidas.CVS:
                    break;

                case TiposSaidas.JSON:
                    return Transfor_JSON(ref Saida, TCampos, Excluidos);

                case TiposSaidas.XML:
                    break;

                case TiposSaidas.TXT:
                    break;

                default:
                    break;
            }

            return true; ;
        }

        /**
          * <summary>
          * <para>Data Criação: 31/03/2020</para>
          * <para>Método síncrono que verifica se existe uma personalização da saída de comandos powershell.</para>
          * <paramref name="CMD"/> Representa o pacote de comando enviado pelo cliente para execução no ambiente powershell.
          * <para>Retorno: Boolean</para>
          * </summary>
          * */
        private bool Get_ComandosPersonalizados(Pacote_Comando CMD = null)
        {
            string Comando = CMD?.Comando;

            if(Comando != null && Comando != "")
            {
                char[] Separador = new char[]{' '};
                string[] Partes_Comando = Comando.Split(Separador,StringSplitOptions.None);
                Partes_Comando[0] = Partes_Comando[0].Replace("-", "_");
                System.Reflection.MethodInfo Metodo = GetType().GetMethod(Partes_Comando[0]);
                if(Metodo != null)
                {
                    switch (Partes_Comando[0])
                    {
                        case "get_process":
                            if (get_process(Partes_Comando))
                                return true;
                            else 
                                return false;

                        case "get_childrem":
                            if (get_childrem(Partes_Comando))
                                return true;
                            else
                                return false;
                        default:

                            break;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }

        }

        public bool Route(Pacote_Comando PCT)
        {
            TSaida = PCT.Formato;
            if (Get_ComandosPersonalizados(PCT)) return true;

            if ((ExecutarScript(PCT.Comando)) && (PCT.Comando != ""))
            {
                if (gerarSaida()) return true; else return false;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
            string Saida = Result;
            Result = null;
            return Saida;
        }
    }
}
