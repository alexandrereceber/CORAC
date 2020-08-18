using RegistroWindows;
using ServerClienteOnline.Utilidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC
{
    [RunInstaller(true)]
    public partial class InstCORAC : System.Configuration.Install.Installer
    {
        public InstCORAC()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            Task<bool> Resultado = Task.Run(() => Admin_CriarUsuario());
            Resultado.Wait();
            if (Resultado.Result)
            {
                base.Install(stateSaver);

            }
            else
            {
                base.Rollback(stateSaver);
            }
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                AddInitialAutomatic();
            }
            catch (Exception e)
            {
                MessageBox.Show("Falha ao atualizar o arquivo de configuração da aplicação: " + e.Message);
                base.Rollback(savedState);
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        private void AddInitialAutomatic()
        {
            if (IsAdministrator())
            {
                string PathServidor = Context.Parameters["PATHSERVIDORCORAC"];
                List<KeyValuePair<string, string>> CR = new List<KeyValuePair<string, string>>();
                CR.Add(new KeyValuePair<string, string>("Path_ServerWEB_CORAC", PathServidor));
                RegistroWin32 Criar_Start = new RegistroWin32();
                if (!Criar_Start.Gravar_ConteudoCampo(TipoChave.LocalMachine, "SOFTWARE\\CORAC", ref CR))
                {
                    throw new Exception("Falha no registro. Chave: SOFTWARE\\CORAC");
                }

                CR.Clear();
                CR = new List<KeyValuePair<string, string>>();
                CR.Add(new KeyValuePair<string, string>("CORAC", "\"%ProgramFiles(x86)%\\CORAC\\CORAC.exe\""));

                if (!Criar_Start.Gravar_ConteudoCampo(TipoChave.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", ref CR))
                {
                    throw new Exception("Falha no registro. Chave: SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }


            }
            else
            {
                throw new Exception("Não há privilégios suficiente para essa operação.");
            }
        }
        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private bool Verify_Counta()
        {
            try
            {
                PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Machine);
                UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, "CORAC");
                if (oUserPrincipal != null)
                {
                    oUserPrincipal.Delete();
                    return true;
                }
                else return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        private async Task<bool> Admin_CriarUsuario()
        {
            if (IsAdministrator())
            {

                bool VExistConta = await Task.Run(() => Verify_Counta());

                if (VExistConta)
                {

                    bool VConta = await Task.Run(() => CriarConta_CORAC());

                    if (VConta)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível criar o usuário CORAC.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Não foi possível apagar a conta CORAC existente. Favor excluí-la para continuar.", "Usuário CORAC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Você não é administrador, favor elevar os privilégios!", "Usuário CORAC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;

            }

        }

        private bool CriarConta_CORAC()
        {
            try
            {
                PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Machine);

                UserPrincipal oUserPrincipal = new UserPrincipal(oPrincipalContext);
                oUserPrincipal.Name = "CORAC";
                oUserPrincipal.SetPassword("Al$#09005811");
                oUserPrincipal.PasswordNeverExpires = true;
                oUserPrincipal.UserCannotChangePassword = true;
                oUserPrincipal.PasswordNotRequired = true;

                //User Log on Name
                //oUserPrincipal.UserPrincipalName = sUserName;
                oUserPrincipal.Save();

                GroupPrincipal oGruopAdministrators = GroupPrincipal.FindByIdentity(oPrincipalContext, "administradores");
                oGruopAdministrators.Members.Add(oUserPrincipal);
                oGruopAdministrators.Save();

                return true;
            }
            catch (Exception E)
            {
                return false;
            }



        }
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistroWin32 Excluir_Chave = new RegistroWin32();
            Excluir_Chave.DeleteValorChave(TipoChave.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "CORAC");
            Delete_Counta();
        }

        private void Delete_Counta()
        {
            try
            {
                PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Machine);
                UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, "CORAC");
                if (oUserPrincipal != null)
                {
                    oUserPrincipal.Delete();
                }

            }
            catch (Exception e)
            {

            }

        }

    }

}
