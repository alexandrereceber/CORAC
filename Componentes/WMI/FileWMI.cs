using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ServerClienteOnline.WMIs
{
    class Get_WMI
    {
        /**
         * Função com retorno em dynamic, pois poderá retornar string, integer, array e outros.
         * Ex.: É o retorno em array da propriedade chassi. A conversão deve ser feita na variável que receberá o retorno;
         */
        public static dynamic Obter_Atributo(string Classe_WMI, string Nome)
        {
            dynamic Result = null;

            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");

            //create object query
            ObjectQuery query = new ObjectQuery("SELECT * FROM " + Classe_WMI);

            //create object searcher
            ManagementObjectSearcher searcher =
                                    new ManagementObjectSearcher(scope, query);

            //get a collection of WMI objects
            ManagementObjectCollection queryCollection = searcher.Get();

            //enumerate the collection.
            foreach (ManagementObject m in queryCollection)
            {
                Result = m[Nome];
                return Result;
            }

            return false;

        }
    }
}
