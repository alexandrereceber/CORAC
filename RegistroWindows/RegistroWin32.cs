using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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

    class RegistroWin32:IDisposable
    {
        private RegistryKey Corrente_User, LocalMachine;

        //Lista contendo o valores de configuração do sistemaCORAC
        private List<CamposCORAC> CORAC_Registro = new List<CamposCORAC>();

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
                return false;

            }
        }

        /**
         * Busca os campos de uma determinada chave, dentro da chave Raíz CurrenteUser.
         */
        public List<KeyValuePair<string, string>> CurrentUSer_CamposChave(string Chave)
        {
            List<KeyValuePair<string, string>> KeysValues = new List<KeyValuePair<string, string>>();
            try
            {
                RegistryKey SubChave = LocalMachine.OpenSubKey(Chave, true);
                string[] Campos = SubChave.GetValueNames();
                foreach (string i in Campos)
                {
                    object Vlr = SubChave.GetValue(i);
                    KeysValues.Add(new KeyValuePair<string, string>(i, (string)Vlr));
                }

                return KeysValues;
            }
            catch (Exception e)
            {

                return null;
            }

        }
        /**
         * Obtém os campos da chave indicada e retorna em um lista.
         */
        public List<KeyValuePair<string, string>> LocalMachine_CamposChave(string Chave)
        {
            List<KeyValuePair<string, string>> KeysValues = new List<KeyValuePair<string, string>>();
            try
            {
                RegistryKey SubChave = LocalMachine.OpenSubKey(Chave, true);
                string[] Campos = SubChave.GetValueNames();
                foreach(string i in Campos)
                {
                    object Vlr = SubChave.GetValue(i);
                    KeysValues.Add(new KeyValuePair<string, string>(i, (string)Vlr));
                }

                return KeysValues;
            }
            catch (Exception e)
            {

                return null;
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

            }
        }

        public void Dispose()
        {
            LocalMachine.Close();
            Corrente_User.Close();
        }
    }
}
