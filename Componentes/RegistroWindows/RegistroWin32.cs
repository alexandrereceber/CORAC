using System;
using System.Collections.Generic;
using Microsoft.Win32;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Utilidades;
using System.Windows.Forms;

namespace RegistroWindows

{
    
    class RegistroWin32: Tratador_Erros, IDisposable
    {
        private RegistryKey Corrente_User, LocalMachine;

        //Lista contendo o valores de configuração do sistemaCORAC

        List<KeyValuePair<string, string>> KeysValues;
        public WebBrowser Componente_Log { get; set; }
        public RegistroWin32()
        {
            Corrente_User = Registry.CurrentUser;
            LocalMachine = Registry.LocalMachine;
        }

        /**
         * <summary>
         * grava o valor de uma chave no registro do windows.
         * <para>
         * <paramref name="ListaCamposValores"/> - Contém um conjunto de chaves e valores que serão gravados no registro do windwos.
         * </para>
         * </summary>
         */
        public bool Gravar_ConteudoCampo(TipoChave TipoChave, string Chave, string Campo, string Valor)
        {
            try
            {
                if(TipoChave == TipoChave.LocalMachine)
                {
                    RegistryKey CORAC = LocalMachine.OpenSubKey(Chave);
                    CORAC.SetValue(Campo, Valor);
                    return true;
                }
                else
                {
                    RegistryKey CORAC = Corrente_User.OpenSubKey(Chave);
                    CORAC.SetValue(Campo, Valor);
                    return true;
                }

            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);

                if (GetError() && TSaida_Error == TipoSaidaErros.Componente || TSaida_Error == TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
                return false;

            }
        }


        /**
         * <summary>
         * Busca o valor de uma chave no registro do windows.
         * <para>
         * <paramref name="Campo"/> - Nome do campo para o qual se deseja buscar o seu valor no registro do windows.
         * </para>
         * </summary>
         */
        public object Obter_ConteudoCampo(string Campo)
        {
            if (KeysValues == null) return null;
            if (KeysValues?.Count == 0) return null;

            foreach (KeyValuePair<string, string> Position in KeysValues)
            {
                if (Position.Key == Campo) return Position.Value;
            }

            return null;
        }

        /**
         * <summary>
         * Verifica se a chave CORAC existe.
         * <para>return bool</para>
         * </summary>
         */
        public bool Existe_Chave_CORAC()
        {
            try
            {
                RegistryKey CORAC = LocalMachine.OpenSubKey("SOFTWARE\\CORAC");
                if (CORAC == null)
                {
                    //CORAC.Close();
                    return false;
                }
                else
                {
                    CORAC.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);

                if (GetError() && TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.Componente || TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
                return false;

            }
        }

        /**
         * <summary>
         * Cria os campos de configuração do sistema CORAC, baseado na STRUCT camposCORAC. Caminho da chave SOFTWARE/corac.
         * <para>eturn bools</para>
         * </summary>
         */
        public bool Criar_Chaves_Campos_CORAC()
        {
            try
            {
                RegistryKey CORAC = LocalMachine.CreateSubKey("SOFTWARE\\CORAC");
                
                CamposCORAC CMP = new CamposCORAC();
                System.Reflection.FieldInfo[] Campos = CMP.GetType().GetFields();
                foreach(System.Reflection.FieldInfo CPMIndividual in Campos)
                {
                    string Tipo = CPMIndividual.FieldType.Name;
                    Tipo = Tipo == "Boolean" ? "false" : "";
                    CORAC.SetValue(CPMIndividual.Name, Tipo);
                }

                CORAC.Close();
                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);

                if (GetError() && TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.Componente || TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
                return false;

            }
        }

        /**
         * Busca os campos de uma determinada chave, dentro da chave Raíz CurrenteUser.
         */
        public bool CurrentUSer_CamposChave(string Chave)
        {
            KeysValues = new List<KeyValuePair<string, string>>();
            try
            {
                RegistryKey SubChave = LocalMachine.OpenSubKey(Chave, true);
                string[] Campos = SubChave.GetValueNames();
                foreach (string i in Campos)
                {
                    object Vlr = SubChave.GetValue(i);
                    KeysValues.Add(new KeyValuePair<string, string>(i, (string)Vlr));
                }

                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);

                if (GetError() && TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.Componente || TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
                return false;
            }

        }
        /**
         * Obtém os campos da chave indicada e retorna em um lista.
         */
        public bool LocalMachine_CamposChave(string Chave)
        {
            KeysValues = new List<KeyValuePair<string, string>>();

            try
            {
                RegistryKey SubChave = LocalMachine.OpenSubKey(Chave, true);
                string[] Campos = SubChave.GetValueNames();
                foreach(string i in Campos)
                {
                    object Vlr = SubChave.GetValue(i);
                    KeysValues.Add(new KeyValuePair<string, string>(i, (string)Vlr));
                }

                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);
                if (GetError() && TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.Componente || TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
                return false;
            }

        }
        /**
         * Cria campos e valores dentro de uma subchave.
         */
        public void Criar_Chave(string Chave,RegistryKey ChaveRaiz, string Nome, object Valor)
        {
            try
            {
                ChaveRaiz = Corrente_User.OpenSubKey(Chave, true);
                ChaveRaiz.SetValue(Nome, Valor);
                ChaveRaiz.Close();

            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);
                if (GetError() && TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.Componente || TSaida_Error == ServerClienteOnline.Utilidades.TipoSaidaErros.ComponenteAndFile)
                {
                    Componente_Log.DocumentText += getH;
                }
            }
        }

        public void Dispose()
        {
            LocalMachine.Close();
            Corrente_User.Close();
        }
    }
}
