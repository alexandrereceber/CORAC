using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerClienteOnline.TratadorDeErros;

namespace CORAC
{

    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form CORAC = new CORAC_TPrincipal();
                Application.Run();
            }catch(Exception e)
            {
                Tratador_Erros Erros = new Tratador_Erros();
                Erros.SetTratador_Erros(ServerClienteOnline.Utilidades.TipoSaidaErros.Arquivo);
                Erros.TratadorErros(e, "MAIN");
            }
        }
    }
}
