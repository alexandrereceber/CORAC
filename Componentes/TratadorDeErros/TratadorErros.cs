using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerClienteOnline.Utilidades;
using System.IO;

namespace ServerClienteOnline.TratadorDeErros
{
    public abstract class Tratador_Erros : IDisposable
    {
        protected bool Excecao = false;
        protected Exception DadosExcecao = null;
        protected TipoSaidaErros TSaida_Error { get; set; }

        string H = null,
                Mensagem = null,
                HelpLink = null,
                Source = null,
                StackTrace = null;
        public bool GetError()
        {
            return Excecao;
        }

        public string getH { get { return H; } }
        public Exception GetException()
        {
            return DadosExcecao;
        }

        public void SetTratador_Erros(TipoSaidaErros T)
        {
            TSaida_Error = T;
        }

        public TipoSaidaErros ObterTipo_SaidaErro()
        {
            return TSaida_Error;
        }
        /**
         * Data: 27/02/2019
         * Trata de todos os erros dentro da classe.
         * Return: False
         */
        protected void TratadorErros(Exception e, string NomeClasse)
        {
            Excecao = true;
            DadosExcecao = e;



            int Hresult = e.HResult;
            /**
             * Função que personaliza as mensagens enviadas.
             */
            switch (Hresult)
            {
                case 33:
                    Mensagem = e.Message;       //Mensagem personalizada
                    HelpLink = e.HelpLink;      //Mensagem personalizada
                    Source = e.Source;          //Mensagem personalizada
                    StackTrace = e.StackTrace;  //Mensagem personalizada

                    break;

                default:
                    Mensagem = e.Message;
                    HelpLink = e.HelpLink;
                    Source = e.Source;
                    StackTrace = e.StackTrace;

                    break;
            }
            H = "<div>" +
                    "<table>" +
                        "<tr><td>HResult: </td><td>" + Convert.ToString(e.HResult) + "</td></tr>" +
                        "<tr><td>Data: </td><td>" + DateTime.Now + "</td></tr>" +
                        "<tr><td>HelpLink: </td><td>" + HelpLink + "</td></tr>" +
                        "<tr><td>Source: </td><td>" + Source + "</td></tr>" +
                        "<tr><td>StackTracer: </td><td>" + StackTrace + "</td></tr>" +
                        "<tr><td>Mensagem: </td><td>" + Mensagem + "</td></tr>" +
                    "</table>" +
               "</div>";


            switch (TSaida_Error)
            {
                case TipoSaidaErros.ShowWindow:
                    System.Windows.Forms.MessageBox.Show(H);
                    break;

                case TipoSaidaErros.EventWindow:
                    System.Diagnostics.EventLog EventosLogos = new System.Diagnostics.EventLog();
                    EventosLogos.Source = "application";
                    EventosLogos.WriteEntry(H, System.Diagnostics.EventLogEntryType.Error, 5000);
                    break;

                case TipoSaidaErros.Console:
                    Console.WriteLine(H);
                    break;

                case TipoSaidaErros.Arquivo:
                    File.AppendAllText(".\\" + NomeClasse + ".html", H);

                    break;
                    //Observação: Não há a necessidade de criar as opções componente e componenteAndFile, uma vez que será implementado na classe que gerou a excessão
                    //via componente.
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
