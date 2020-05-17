using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ServerClienteOnline.Utilidades;
using System.Net;
using System.Net.Sockets;

namespace ServerClienteOnline.Interfaces
{

    /**
     * <summary>Interface que recebe pacote para tratamento.</summary>
     */
    public interface IRuntime : IDisposable
    {
        bool Route(Pacote_Comando pct);
        string Get_Resultado();
    }

    /**
     * <summary>Interface para autenticação de máquinas.</summary>
     */
    public interface IAuth : IDisposable
    {
        Pacote_Auth AutenticarUsuario(Pacote_Auth Pacote_Auth);
    }

    /**
 * <summary>Interface para autenticação de máquinas HTML.</summary>
 */
    public interface IAuthHTML : IDisposable
    {
        Task<bool> HTML_AutenticarUsuario(Pacote_Auth Pacote_Auth);
        bool HTML_Autenticado(string Chave_Autenticar);
        Pacote_Auth GetAutenticacao { get;}
    }

    /**
 * <summary>Interface para gerenciamento de clientes HTML conectados.</summary>
 */
    public interface IGClienteHTML : IDisposable
    {
        bool ConectarCliente(EndPoint Client, Pacote_Auth Autenticacao);
        bool _OAuth(string Chave);
        bool Validar_Chave_AR(string Chave);
    }
    /**
     * <summary>Interface para tratamento de pacotes.</summary>
     */
    public interface ITipoPacote
    {
        TipoPacote GetTipoPacote();
        string GetResultado();
    }

    /**
     * <summary>Interface acesso remoto.</summary>
     */
     public interface IAcesso_Remoto
    {

    }

    /**
     * <summary>Tipo de pacote do acesso remoto</summary>
     */
    public interface IPacote_AR
    {
        TipoPacote Obter_Tipo_Pacote_AR();
    }

    public interface IServidor
    {
        bool StartServidor();
        bool StopServidor();
        bool StatusServidor();
    }

}
