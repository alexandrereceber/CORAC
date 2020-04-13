using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ServerClienteOnline.Utilidades;


namespace RegistroWindows
{
    struct CamposCORAC
    {
        public string Path_Update_CORAC;
        public string Type_Autentication;
        public string Username;
        public string Password;
        public string Path_Server_CORAC;
        public string IP_Listen_Host_CORAC;
        public string Porta_Listen_Host_CORAC;
        public string IP_Listen_AR;
        public string Porta_Listen_AR;

    }

    class RegistroWin32: Tratador_Erros, IDisposable
    {
        private RegistryKey Corrente_User, LocalMachine;

        //Lista contendo o valores de configuração do sistemaCORAC
        List<KeyValuePair<string, string>> KeysValues;

        public RegistroWin32()
        {
            Corrente_User = Registry.CurrentUser;
            LocalMachine = Registry.LocalMachine;
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
                    CORAC.Close();
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
                    CORAC.SetValue(CPMIndividual.Name, "");
                }

                CORAC.Close();
                return true;
            }
            catch (Exception e)
            {
                TratadorErros(e, GetType().Name);
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
            }
        }

        public void Dispose()
        {
            LocalMachine.Close();
            Corrente_User.Close();
        }
    }
}
