using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using ServerClienteOnline.Utilidades;
using ServerClienteOnline.Interfaces;
using ServerClienteOnline.TratadorDeErros;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace ServerClienteOnline.AcessoRemoto.Server
{

    public class Server_AcessoRemoto :Tratador_Erros, IDisposable, IAcesso_Remoto, IServidor
    {
       

        public void Dispose()
        {

        }

        public bool StartServidor()
        {
            throw new NotImplementedException();
        }

        public bool StatusServidor()
        {
            throw new NotImplementedException();
        }

        public bool StopServidor()
        {
            throw new NotImplementedException();
        }
    }


}
