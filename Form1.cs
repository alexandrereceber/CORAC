using RegistroWindows;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ServerClienteOnline.Utilidades;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Net.Http;

using ServerClienteOnline.Conectividade;
using ServerClienteOnline.TratadorDeErros;
using ServerClienteOnline.Server;
using ServerClienteOnline.MetodosAutenticacao;
using ServerClienteOnline.Gerenciador.ClientesConectados;
using ServerClienteOnline.WMIs;

using Power_Shell.AmbienteExecucao;
using CamadaDeDados.RESTFormat;
using CORAC.Chat;
using System.Management.Automation;

namespace CORAC
{

    public partial class CORAC_TPrincipal : Form
    {
        private RegistroWin32 ChavesCORAC;
        private List<KeyValuePair<string, string>> KeysValues;
        /**
         * Gerente de autenticação do servidor WEBPowershell
         */
        private GerenciadorClientes GerenteClientes = new GerenciadorClientes();
        private Ambiente_PowerShell AbrirComando = null;
        private Servidor_HTTP ServidorWEB_Local = null;
        private Servidor_WEBSOCKET ServidorWEB_Socket = null;

        private Autenticador_WEB Autent_WEB = null;
        private RegistroCORAC Registro_Corac = new RegistroCORAC();

        //Form CaixaDialog = new Chat_CORAC();

        private bool ArmazenarAlteracoesCampos(string Chave, string Valor)
        {
            int count = 0;
            try
            {
                if (KeysValues == null) KeysValues = new List<KeyValuePair<string, string>>();
                if(KeysValues.Count > 0)
                {
                    foreach(KeyValuePair<string, string> ParChvVlr in KeysValues)
                    {
                        if(ParChvVlr.Key == Chave)
                        {
                            if (ParChvVlr.Value == Valor) return true;
                            KeysValues.RemoveAt(count);
                            KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                            return true;
                        }
                        count++;
                    }
                    KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                }
                else
                {
                    KeysValues.Add(new KeyValuePair<string, string>(Chave, Valor));
                }
            }catch(Exception E)
            {
                return false;
            }


            return true;

        }

        private void ObterConfiguracoes()
        {
            ChavesCORAC = new RegistroWin32();
            ChavesCORAC.SetTratador_Erros(TipoSaidaErros.ComponenteAndFile);

            if (!ChavesCORAC.Existe_Chave_CORAC())
            {
                if (!ChavesCORAC.Criar_Chaves_Campos_CORAC())
                {
                    MessageBox.Show("Não foi possível obter acesso ao registro. A aplicações está terminando!");
                    Application.Exit();
                }
                else
                {
                    ChavesCORAC.LocalMachine_CamposChave("software\\CORAC");
                }

            }
            else
            {
                ChavesCORAC.LocalMachine_CamposChave("software\\CORAC");
            }
        }

        private void CheckComponentes()
        {

        }

        /**
         * <summary>
            Verifica se existe saída para internet.
            <para>Data</para>
         * </summary>
         */
        private async Task<bool> Verificar_Conectividade()
        {
            
            //Image CopiaImagem = picture_Internet_Status.Image;
            picture_Internet_Status.SizeMode = PictureBoxSizeMode.CenterImage;
            picture_Internet_Status.Image = Properties.Resources.Wait;

            if (await Conexoes.VerificarConectividade())
            {
                //Bitmap Internet_ON = Change_Color(CopiaImagem, Vermelho, Azul);
                picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Internet_Status.Tag = "O acesso à internet está OK.";
                picture_Internet_Status.Image = Properties.Resources.Internet_Cor_fw;
                return true;
            }
            else
            {

                picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Internet_Status.Tag = "Falha ao acessa internet.";
                picture_Internet_Status.Image = Properties.Resources.Internet_Cinza_fw;
                return false;
            }


        }

       
        /**
         * <summary>
            Verifica se existe atualizações do sistema CORAC, esta verificação é realizada através da internet em um site próprio.
         * </summary>
         */
        private async Task<bool> Verificar_Atualizacoes()
        {

            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);

            try
            {
                picture_Atualizacoes_CORAC.Image = Properties.Resources.Wait;
                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;
                
                if (!await Conexoes.VerificarConectividade()) throw new Exception("Sem conectividade");

                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));

                HttpClient URL = new HttpClient();
                var pairs = new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>("login", "abc")
                                        };

                var content = new FormUrlEncodedContent(pairs);
                URL.Timeout = TimeSpan.FromSeconds(30);
                
                Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                await Task.WhenAll(Conteudo);


                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Update_System_256px, Vermelho, Azul);
                    picture_Internet_Status.Tag = "";
                    picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Color_fw;

                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Update_System_256px, Azul, Vermelho);
                    picture_Internet_Status.Tag = "O sistema de atualização não responde.";
                    picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Cinza_fw;
                    return false;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                //Bitmap Internet_ON = Change_Color(Properties.Resources.Update_System_256px, Azul, Vermelho);

                picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Cinza_fw;

                return false;
            }
        }

        /**
        * <summary>
        * Criado: 23/06/2020 -
           Método que busca as configurações dos diversos componentes do desktop CORAC.
        * </summary>
        */
        private async Task<bool> Buscar_ConfiguracoesCORAC()
        {

            try
            {
                string Path_Config = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                string ConfPath = Regex.Replace(Path_Config, "/Checked/", "/ConfCORAC/", RegexOptions.IgnoreCase);
                Uri EndURI = new Uri(ConfPath);
                
                HttpClient URL = new HttpClient();
                var pairs = new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>("login", "abc")
                                        };

                var content = new FormUrlEncodedContent(pairs);
                URL.Timeout = TimeSpan.FromSeconds(30);

                Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                await Task.WhenAll(Conteudo);


                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    string Conf = await Conteudo.Result.Content.ReadAsStringAsync();
                    Conexoes.Config = JsonConvert.DeserializeObject<ConfiguracoesCORAC>(Conf);
                    Conexoes.ConfigLoad = true;
                    return true;
                }
                else
                {
                    Conexoes.ConfigLoad = false;
                    return false;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);
                
                Conexoes.ConfigLoad = false;

                return false;
            }
        }

        private async Task<bool> ObterAssinatura()
        {
            try
            {
                if (!await Conexoes.VerificarConectividade())
                {
                    MessageBox.Show("Não há conectividade!", "Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("Sem conectividade");
                }
                pictureBox_Servidor_WEB.Image = Properties.Resources.Wait;
                Uri EndURI = new Uri(textBox_Path_ServerWEB_CORAC.Text);

                HttpClient URL = new HttpClient();
                var pairs = new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>("login", "abc")
                                        };

                var content = new FormUrlEncodedContent(pairs);

                URL.Timeout = TimeSpan.FromSeconds(30);

                Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                await Task.WhenAll(Conteudo);

                pictureBox_Servidor_WEB.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    string Dados = await Conteudo.Result.Content.ReadAsStringAsync();
                    Assinatura Sign = JsonConvert.DeserializeObject<Assinatura>(Dados);
                    if (Sign.Sistema == "CORAC" && Sign.Signacture == "a4b315c63dca8337dc70ef6a336310f4")
                    {
                        return true;
                    }
                    else
                    {
                        return false; 
                    }

                }
                else
                {
                    return false;
                }

            }
            catch (Exception E)
            {
                return false;
            }
        }
        /**
         * <summary>
            Verifica se o software CORAC está licenciado. Esta verificação ocorre através da internet em umm site próprio do CORAC.
         * </summary>
         */
        private async Task<bool> Verificar_Registro()
        {
            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);
            
            try
            {
                String Estado = "";

                if (await ObterAssinatura())
                {
                    string NomeEstacao = Dns.GetHostName();

                    pictureBox_Registro_CORAC.Image = Properties.Resources.Wait;
                    pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

                    if (!await Conexoes.VerificarConectividade()) throw new Exception("Sem conectividade!");

                    Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
                    string pth = EndURI.Scheme + "://" + EndURI.Host + ":" + EndURI.Port + "/CORAC/ControladorTabelas/";
                    Tabelas BuscarRegistro_CORAC = new Tabelas(pth);

                    List<KeyValuePair<int, string[]>> FiltrosB = new List<KeyValuePair<int, string[]>>();
                    FiltrosB.Add(new KeyValuePair<int, string[]>(0, new string[4] { "1", "like", NomeEstacao, "1" }));
                    BuscarRegistro_CORAC.setFiltros(TipoFiltro.Buscar, FiltrosB);
                    BuscarRegistro_CORAC.sendTabela = "e78169c2553f6f5abe6e35fe042b792a";
                    await BuscarRegistro_CORAC.SelectTabelaJSON();

                    if (!BuscarRegistro_CORAC.getError)
                    {

                        JProperty Dados = BuscarRegistro_CORAC.getDados().ResultadoDados;
                        if (Dados.Value.HasValues)
                        {
                            string Status = (string)Dados.Value[0][3];
                            if (Status == "Ativado")
                            {
                                /**
                                 * Informando que a máquina está ligada
                                 */
                                string getChave = Dados.Value[0][0].Value<string>();
                                List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();

                                KDados.Add(new KeyValuePair<string, string>("0", getChave));
                                BuscarRegistro_CORAC.setKeyDadosAtualizar(KDados);

                                List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                                ADados.Add(new KeyValuePair<string, string>("Status", "Ligada"));

                                BuscarRegistro_CORAC.sendTabela = "334644edbd3aecbe746b32f4f2e8e5fb";
                                BuscarRegistro_CORAC.setDadosAtualizar(ADados);
                                Boolean Rst = await BuscarRegistro_CORAC.AtualizarDadosTabela();
                                if (Rst)
                                {
                                    Estado = "Mudança de status realizada com sucesso!";
                                }
                                else
                                {
                                    Estado = "Mudança de status não foi realizada com sucesso!";
                                }
                                /*
                                 * Registro encontrado e equipamento ativado
                                 */
                                Registro_Corac.Status = StatusRegistro.Habilitado;
                                Registro_Corac.Chave_BD = getChave;

                                bool Confg_CORAC = await Buscar_ConfiguracoesCORAC();
                                if (Confg_CORAC)
                                {

                                    if (!(ServidorWEB_Local == null))
                                    {
                                        if (ServidorWEB_Local.StatusServidor())
                                        {
                                            button_Start_PowerShellCORAC.Enabled = true;
                                            button_Start_AR_CORAC.Enabled = true;
                                        }

                                    }
                                    else
                                    {
                                        button_Start_PowerShellCORAC.Enabled = true;
                                        button_Start_AR_CORAC.Enabled = true;

                                    }
                                }
                                else
                                {
                                    Estado += ". As configurações dos serviços CORAC não foram encontradas!";
                                }


                                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                                //Bitmap Internet_ON = Change_Color(Properties.Resources.Registro_256px, Vermelho, Azul);
                                pictureBox_Registro_CORAC.Tag = "Registro encontrado e equipamento ativado. " + Estado;
                                pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Color_fw;
                                FiltrosB.Clear();
                                return true;
                            }
                            else
                            {
                                //Registro encontrado e equipamento desativado.
                                Registro_Corac.Status = StatusRegistro.Desabilitado;

                                //Bitmap Internet_ON = Change_Color(Properties.Resources.Registro_256px, Azul, Vermelho);
                                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox_Registro_CORAC.Tag = "Registro encontrado e equipamento desativado. Requisitar habilitação junto ao administrador da sua unidade.";
                                pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Color_fw;
                                FiltrosB.Clear();
                                return false;
                            }

                        }
                        else
                        {
                            int Chassi = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "ChassisTypes")[0];

                            List<KeyValuePair<string, string>> IDados = new List<KeyValuePair<string, string>>();
                            IDados.Add(new KeyValuePair<string, string>("Tipo", Convert.ToString(Chassi)));
                            IDados.Add(new KeyValuePair<string, string>("Nome", NomeEstacao));

                            BuscarRegistro_CORAC.sendTabela = "334644edbd3aecbe746b32f4f2e8e5fb";
                            BuscarRegistro_CORAC.setDadosInserir(IDados);

                            Boolean Adicionar = await BuscarRegistro_CORAC.InserirDadosTabela();
                            if (Adicionar)
                            {
                                pictureBox_Registro_CORAC.Tag = "O agente autônomo foi adicionado com sucesso, favor entrar em contato com administrador para habilitação. Nome: " + NomeEstacao;

                            }
                            else
                            {
                                pictureBox_Registro_CORAC.Tag = "O agente autônomo não pôde ser adicionado. Nome: " + NomeEstacao;

                            }

                            pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Cinza_fw;
                            return false;
                        }

                    }
                    else
                    {
                        //Mensagem de error no site
                        //Bitmap Internet_ON = Change_Color(Properties.Resources.Registro_256px, Azul, Vermelho);
                        pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;

                        pictureBox_Registro_CORAC.Tag = "O Site do CORAC devolveu um erro.";
                        pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Cinza_fw;
                        FiltrosB.Clear();

                        return false;
                    }
                }
                else
                {
                    pictureBox_Registro_CORAC.Tag = "Assinatura inacessível ou incorreta, favor contactar os administradores.";
                    return false;
                }
                

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);


                //Bitmap Internet_ON = Change_Color(Properties.Resources.Registro_256px, Azul , Vermelho);
                pictureBox_Registro_CORAC.Tag = "Error reportado pelo tratador de erros.";
                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Cinza_fw;

                return false;
            }
        }

        /**
  * <summary>
     Para oo serviço Acesso Remoto CORAC
  * </summary>
  */
        private async Task<bool> Stop_Servidor_AcessoRemoto()
        {
            pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.CenterImage;

            Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            Color Azul = Color.FromArgb(255, 0, 1, 255);
            try
            {

                pictureBox_AcessoRemoto.Image = Properties.Resources.Wait;
                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.CenterImage;

                //-------------------SERVIDOR HTTP-------------------------------------------------------------------


                bool Server_HTTP = await Task.Run(ServidorWEB_Socket.StopServidor);

                //------------------------------------------------------------------------------------------------------

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_HTTP)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Azul, Vermelho);
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Cinza_fw;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Vermelho, Azul);
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Color_fw;
                    return true;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;
                //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Vermelho, Azul);

                pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Color_fw; ;

                return true;

            }
        }


        /**
          * <summary>
             Para oo serviço PowerShell CORAC
          * </summary>
          */
        private async Task<bool> Stop_Servidor_PowerShell()
        {
            pictureBox_Powershell.SizeMode = PictureBoxSizeMode.CenterImage;

            Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            Color Azul = Color.FromArgb(255, 0, 1, 255);
            try
            {

                pictureBox_Powershell.Image = Properties.Resources.Wait;
                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.CenterImage;

                //------------------STOP SERVIDOR POWERSHELL-----------------------------------------------------------------


                AbrirComando.StopServidor();
                AbrirComando = null;


                //-------------------SERVIDOR HTTP-------------------------------------------------------------------


                bool Server_HTTP = await Task.Run(ServidorWEB_Local.StopServidor);

                //------------------------------------------------------------------------------------------------------

                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_HTTP)
                {
                   // Bitmap Internet_ON = Change_Color(Properties.Resources.po, Azul, Vermelho);
                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Cinza_fw ;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Vermelho, Azul);
                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Color_fw ;
                    return true;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.StretchImage;
                //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Vermelho, Azul);

                pictureBox_Powershell.Image = Properties.Resources.Powershell_Color_fw;

                return true;

            }
        }

        /**
          * <summary>
            Inicia o serviço CORAC.
          * </summary>
          */
        private async Task<bool> Iniciar_Servidor_PowerShell()
        {
            //pictureBox_Powershell.SizeMode = PictureBoxSizeMode.CenterImage;

            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);
            
            try
            {
                if (!(Registro_Corac.Status == StatusRegistro.Habilitado))
                {
                    throw new Exception("Agente autônomo não está habilitado a funcionar nesta estação. Contate o administrador.");
                }


                pictureBox_Powershell.Image = Properties.Resources.Wait;
                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.CenterImage;

                //------------------SERVIDOR POWERSHELL-----------------------------------------------------------------

                AbrirComando = new Ambiente_PowerShell();
                AbrirComando.StartServidor();

                Autent_WEB = new Autenticador_WEB();
                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
                Autent_WEB.Endereco_Autenticacao(EndURI, "/CORAC/ValidarLogin/");

                //-------------------SERVIDOR DE HTTP-------------------------------------------------------------------

                string EndString = Conexoes.EnderecoHttpListen_Powershell();
                int EndPorta = Conexoes.Config.Servicos.PowerShell;

                ServidorWEB_Local = new Servidor_HTTP();
                
                ServidorWEB_Local.SetTratador_Erros(TipoSaidaErros.Arquivo);

                ServidorWEB_Local.AddPrefixos(null, EndString, "Pacotes/", EndPorta);

                ServidorWEB_Local.AtribuirExecutor = AbrirComando;
                ServidorWEB_Local.Autenticador = Autent_WEB;
                ServidorWEB_Local.Gerenciador_Cliente = GerenteClientes;

                bool Server_HTTP = await Task.Run(ServidorWEB_Local.StartServidor);
                
                //------------------------------------------------------------------------------------------------------

                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_HTTP)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Vermelho, Azul);
                    pictureBox_Powershell.Tag = "Serviço powershell carregado corretamente.";
                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Color_fw;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Azul, Vermelho);
                    pictureBox_Powershell.Tag = "Ocorreu um erro no serviço de powershell desta estação.";
                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Cinza_fw; 
                    return false;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                pictureBox_Powershell.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox_Powershell.Tag = E.Message;
                //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Azul, Vermelho);
                pictureBox_Powershell.Image = Properties.Resources.Powershell_Cinza_fw; 

                return false;

            }
        }


        /**
          * <summary>
            Inicia o serviço de acesso remoto.
          * </summary>
          */
        private async Task<bool> Iniciar_Servidor_AcessoRemoto()
        {
            //CaixaDialog.Show();
            //pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.CenterImage;

            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);

            try
            {
                if (!(Registro_Corac.Status == StatusRegistro.Habilitado))
                {
                    throw new Exception("Agente autônomo não está habilitado a funcionar nesta estação. Contate o administrador.");
                }


                pictureBox_AcessoRemoto.Image = Properties.Resources.Wait;
                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.CenterImage;

                //------------------Serviço de Autenticação-----------------------------------------------------------------

                Autent_WEB = new Autenticador_WEB();
                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
                Autent_WEB.Endereco_Autenticacao(EndURI, "/CORAC/ValidarLogin/");


                //-------------------SERVIDOR DE HTTP-------------------------------------------------------------------

                string EndString = Conexoes.EnderecoHttpListen_AcessoRemoto();
                int EndPorta = Conexoes.Config.Servicos.AR;

                ServidorWEB_Socket = new Servidor_WEBSOCKET();

                ServidorWEB_Socket.SetTratador_Erros(TipoSaidaErros.Arquivo);

                ServidorWEB_Socket.AddPrefixos(null, EndString, "CORAC/AcessoRemoto/", EndPorta);
                ServidorWEB_Socket.AddPrefixos(null, EndString, "AA_AcessoRemoto_SYN/", EndPorta);

                ServidorWEB_Socket.Autenticador = Autent_WEB;
                ServidorWEB_Socket.Gerenciador_Cliente = GerenteClientes;

                
                bool Server_HTTP = await Task.Run(ServidorWEB_Socket.StartServidor);

                //------------------------------------------------------------------------------------------------------

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_HTTP)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Vermelho, Azul);
                    pictureBox_AcessoRemoto.Tag = "Serviço de powershell instanciado corretamente.";
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Color_fw ;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Azul, Vermelho);
                    pictureBox_AcessoRemoto.Tag = "Ocorreu um erro no serviço de powershell desta estação.";
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Cinza_fw;
                    return false;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox_AcessoRemoto.Tag = E.Message;
                //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Azul, Vermelho);
                pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Cinza_fw;

                return false;

            }
        }
        private async Task<bool> Loaders()
        {
            

            if (await Verificar_Conectividade())
            {
                Task Atualizar, Registro, Powerhell_WEB, Acesso_Remoto;
                int Atualizar_ID = 0, Registro_ID = 0, Powerhell_WEB_ID = 0, Acesso_Remoto_ID = 0, SPower = 0, SAR = 0;
                List<Task> Servicos = new List<Task>();

                Atualizar = Task.Run(Verificar_Atualizacoes);
                Atualizar_ID = Atualizar.Id;

                Registro = Task.Run(Verificar_Registro);
                Registro_ID = Registro.Id;

                Servicos.Add(Atualizar);
                Servicos.Add(Registro);

                while (Servicos.Count > 0)
                {
                    Task Tarefa = await Task.WhenAny(Servicos);
                    if (Tarefa.Id == Atualizar_ID)
                    {
                        Servicos.Remove(Tarefa);
                        button_AtualizacoesCORAC.Enabled = true;

                    }
                    else if (Tarefa.Id == Registro_ID)
                    {

                        Servicos.Remove(Tarefa);

                        Task<bool> Assinatura = (Task<bool>)Tarefa;

                        if (Assinatura.Result)
                        {
                            button_RegistroMaquina.Enabled = true;
                            if (Conexoes.ConfigLoad)
                            {
                                if (Registro_Corac.Status == StatusRegistro.Habilitado)
                                {
                                    //-------------------------Serviço de PowerShell-----------------------------
                                    Powerhell_WEB = Task.Run(Iniciar_Servidor_PowerShell);
                                    Powerhell_WEB_ID = Powerhell_WEB.Id;
                                    //-------------------------FIM-----------------------------------------------

                                    //-------------------------Serviço de ACESSO REMOTO--------------------------
                                    Acesso_Remoto = Task.Run(Iniciar_Servidor_AcessoRemoto);
                                    Acesso_Remoto_ID = Acesso_Remoto.Id;
                                    //-------------------------FIM-----------------------------------------------

                                    Servicos.Add(Acesso_Remoto);
                                    Servicos.Add(Powerhell_WEB);
                                }
                                else
                                {
                                    pictureBox_Powershell.Tag = "Favor verificar o registro da máquina!";
                                }
                            }
                            else
                            {
                                ConfiguracoesCORAC.Start();
                            }
                        }
                        else
                        {
                            onAssinatura.Start();
                        }




                    }
                    else if (Tarefa.Id == Powerhell_WEB_ID)
                    {

                        Servicos.Remove(Tarefa);
                        Task<bool> Result = (Task<bool>)Tarefa;
                        if (Result.Result)
                        {
                            SPower = 1;
                            button_Start_PowerShellCORAC.Enabled = false;
                            button_Stop_PowerShellCORAC.Enabled = true;
                        }
                        else
                        {
                            SPower = 0;
                            button_Start_PowerShellCORAC.Enabled = true;
                            button_Stop_PowerShellCORAC.Enabled = false;
                        }

                    }
                    else if (Tarefa.Id == Acesso_Remoto_ID)
                    {

                        Servicos.Remove(Tarefa);
                        Task<bool> Result = (Task<bool>)Tarefa;
                        if (Result.Result)
                        {
                            SAR = 1; //Indica que o serviço foi carregado com sucesso!
                            button_Start_AR_CORAC.Enabled = false;
                            button_Stop_AR_CORAC.Enabled = true;
                        }
                        else
                        {
                            SAR = 0;
                            button_Start_AR_CORAC.Enabled = true;
                            button_Stop_AR_CORAC.Enabled = false;
                        }

                    }
                }

                if (Registro_Corac.Status == StatusRegistro.Habilitado)
                {
                    List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                    KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                    List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                    ADados.Add(new KeyValuePair<string, string>("SPowershell", Convert.ToString(SPower)));
                    ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", Convert.ToString(SAR)));
                    ADados.Add(new KeyValuePair<string, string>("SChat", "0"));

                    await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                }

            }
            else
            {
                onNetwork.Start();
                //button_AtualizacoesCORAC.Enabled = true;
                //button_RegistroMaquina.Enabled = true;
            }

            button_VerificarInternet.Enabled = true;

           
            return true;

        }

        public CORAC_TPrincipal()
        {

            ObterConfiguracoes();

            InitializeComponent();

            Loaders();
            //Application.ApplicationExit += SairSistema;
        }

        private void textBox_BD_CORAC_Enter(object sender, EventArgs e)
        {
            Status_Informacao.Text = "Endereço WEB do servidor de banco de dados do CORAC.";

        }


        private void toolStripMenuItem1_MAN_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void CORAC_TPrincipal_Load(object sender, EventArgs e)
        {
            try
            {

                this.MaximizeBox = false;
                this.MinimizeBox = false;
                
                textBox_Path_ServerWEB_CORAC.Text = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);
            }


        }

        private void CORAC_TPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Relógio.Stop();
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                Notificacao.Visible = false;
            }

        }

        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await SairSistema();
            Application.Exit();
            
        }

        private async Task<bool> SairSistema()
        {
                List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                ADados.Add(new KeyValuePair<string, string>("SPowershell", "0"));
                ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", "0"));
                ADados.Add(new KeyValuePair<string, string>("SChat", "0"));
            try
            {
                await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                return true;

            }
            catch (Exception e)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(e, GetType().Name);
                return false;

            }
            finally
            {

            }
        }
        private void SalvaConfiguracoes_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult Resposta = MessageBox.Show("Tem certeza que deseja realizar essa operação?", "Salvar configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Resposta == DialogResult.Yes)
                {
                    if (KeysValues == null) MessageBox.Show("Nenhuma alteração foi identificada.", "Alterações");
                    if (KeysValues.Count > 0)
                    {
                        if(ChavesCORAC.Gravar_ConteudoCampo(TipoChave.LocalMachine, "software\\CORAC", ref KeysValues))
                        {
                            MessageBox.Show("O dados foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ObterConfiguracoes();                   }
                        else
                        {
                            MessageBox.Show("O dados não foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //KeysValues.Clear();
                    }
                }
            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);
            }

        }

        private void textBox_Path_ServerWEB_CORAC_Leave(object sender, EventArgs e)
        {
            TextBox T = sender as TextBox;

            if (T.Modified)
            {
                ArmazenarAlteracoesCampos((string)T.Tag, T.Text);
            }
        }


        private async void button_Servidor_WEB_Click(object sender, EventArgs e)
        {
            if (textBox_Path_ServerWEB_CORAC.Text.Length > 0)
            {

                if(await ObterAssinatura())
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.Acepty;
                }
                else
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.No_Acepty;
                }
            }
            else
            {
                MessageBox.Show("Endereço inválido!", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void textBox_Path_ServerWEB_CORAC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox T = (TextBox)sender;
            if (T.Modified)
            {
                Regex CampoCORACAtualiza = new Regex("^http[s]?://");
                if (!CampoCORACAtualiza.IsMatch(T.Text))
                {
                    e.Cancel = true;
                    MessageBox.Show("O campo precisa iniciar com http(s)://", "Endereço WEB", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
        }

        private async void button_VerificarInternet_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Conectividade();
            }finally
            {
                B.Enabled = true;
            }
        }

        private async void button_AtualizacoesCORAC_Click(object sender, EventArgs e)
        {
            
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Atualizacoes();
            }finally
            {
                B.Enabled = true;
            }
        }

        private async void button_RegistroMaquina_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            B.Enabled = false;
            try
            {
                await Verificar_Registro();
            }
            finally
            {
                B.Enabled = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string[] ArquivosLOG = Directory.GetFiles(".\\", "*.html");
            listBox_FILE_LOG.Items.Clear();
            foreach(string i in ArquivosLOG)
            {
                listBox_FILE_LOG.Items.Add(i);
            }
        }

        private async void listBox_FILE_LOG_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox L = (ListBox)sender;

            try
            {
                L.Enabled = false;
                if (L.SelectedItem as string != "")
                {
                    using (FileStream Abrir_File = File.OpenRead(L.SelectedItem as string))
                    {
                        byte[] LetTudo = new byte[Abrir_File.Length];
                        await Abrir_File.ReadAsync(LetTudo, 0, (int)Abrir_File.Length);
                        webBrowser1.DocumentText = ASCIIEncoding.UTF8.GetString(LetTudo);
                    }

                }
            }
            catch(Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);
            }
            finally
            {
                L.Enabled = true;

            }

        }

        private async void button_PowerShellCORAC_Click(object sender, EventArgs e)
        {
            Button T = (Button)sender;
            T.Enabled = false;

            try
            {
                bool ServerCORAC = await Task.Run(Iniciar_Servidor_PowerShell);
                if (ServerCORAC)
                {
                    if (Registro_Corac.Status == StatusRegistro.Habilitado)
                    {
                        List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                        KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                        List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                        ADados.Add(new KeyValuePair<string, string>("SPowershell", "1"));

                        await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                    }

                    T.Enabled = false;
                    button_Stop_PowerShellCORAC.Enabled = true;
                }
                else
                {
                    T.Enabled = true;
                    button_Stop_PowerShellCORAC.Enabled = false;
                }
            }catch(Exception E)
            {
                T.Enabled = true;
            }

        }

        private async void button_Stop_PowerShellCORAC_Click(object sender, EventArgs e)
        {
            Button T = (Button)sender;
            T.Enabled = false;
            try
            {
                bool ServerCORAC = await Task.Run(Stop_Servidor_PowerShell);
                if (ServerCORAC)
                {
                    T.Enabled = false;
                    if(Registro_Corac.Status == StatusRegistro.Habilitado)
                    {
                        List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                        KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                        List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                        ADados.Add(new KeyValuePair<string, string>("SPowershell", "0"));
                        await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                    }

                    button_Start_PowerShellCORAC.Enabled = true;

                }
                else
                {
                    T.Enabled = true;
                }
            }
            catch (Exception E)
            {
                T.Enabled = true;
            }
        }


        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        private void PictureBox_Registro_CORAC_MouseEnter(object sender, EventArgs e)
        {
            Status_Informacao.Text = (string)(sender as PictureBox).Tag;
        }

        private void Notificacao_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();

        }

        private void PictureBox_Registro_CORAC_MouseLeave(object sender, EventArgs e)
        {
            Status_Informacao.Text = "";
        }

        private void Picture_Internet_Status_MouseEnter(object sender, EventArgs e)
        {
            Status_Informacao.Text = (string)(sender as PictureBox).Tag;
        }

        private void Picture_Atualizacoes_CORAC_MouseEnter(object sender, EventArgs e)
        {
            
        }


        private async Task<bool> AtualizarTabelas_CORAC(string Tabela, List<KeyValuePair<string, string>> KDados, List<KeyValuePair<string, string>> ADados)
        {
            Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
            string pth = EndURI.Scheme + "://" + EndURI.Host + ":" + EndURI.Port + "/CORAC/ControladorTabelas/";
            Tabelas BuscarRegistro_CORAC = new Tabelas(pth);

            //List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();

            //KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));
            BuscarRegistro_CORAC.setKeyDadosAtualizar(KDados);

            //List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
            //ADados.Add(new KeyValuePair<string, string>("SPowershell", "0"));


            BuscarRegistro_CORAC.sendTabela = Tabela; /*"334644edbd3aecbe746b32f4f2e8e5fb"*/;
            BuscarRegistro_CORAC.setDadosAtualizar(ADados);

            return await BuscarRegistro_CORAC.AtualizarDadosTabela();
        }

        private void pictureBox_Powershell_MouseEnter(object sender, EventArgs e)
        {
            Status_Informacao.Text = (string)(sender as PictureBox).Tag;
        }
        private async void button_Start_AR_CORAC_Click(object sender, EventArgs e)
        {

            Button T = (Button)sender;
            T.Enabled = false;

            try
            {
                bool ServerCORAC = await Task.Run(Iniciar_Servidor_AcessoRemoto);
                if (ServerCORAC)
                {

                    if (Registro_Corac.Status == StatusRegistro.Habilitado)
                    {
                        List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                        KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                        List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                        ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", "1"));

                        await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                    }

                    T.Enabled = false;
                    button_Stop_AR_CORAC.Enabled = true;
                }
                else
                {
                    T.Enabled = true;
                    button_Stop_AR_CORAC.Enabled = false;
                }
            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                T.Enabled = true;
            }
        }

        private async void button_Stop_AR_CORAC_Click(object sender, EventArgs e)
        {
            Button T = (Button)sender;
            T.Enabled = false;
            try
            {
                bool ServerCORAC = await Task.Run(Stop_Servidor_AcessoRemoto);
                if (ServerCORAC)
                {
                    T.Enabled = false;
                    if (Registro_Corac.Status == StatusRegistro.Habilitado)
                    {
                        List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                        KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                        List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                        ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", "0"));
                        await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                    }

                    button_Start_AR_CORAC.Enabled = true;

                }
                else
                {
                    T.Enabled = true;
                }
            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                T.Enabled = true;
            }
        }

        private void pictureBox_Registro_CORAC_Click(object sender, EventArgs e)
        {

        }

        private async void ConfiguracoesCORAC_Tick(object sender, EventArgs e)
        {
            if (await Buscar_ConfiguracoesCORAC())
            {
                //-------------------------------------------------------------------------------------------------
                try
                {
                    bool ServerCORAC = await Task.Run(Iniciar_Servidor_PowerShell);
                    if (ServerCORAC)
                    {
                        if (Registro_Corac.Status == StatusRegistro.Habilitado)
                        {
                            List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                            KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                            List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                            ADados.Add(new KeyValuePair<string, string>("SPowershell", "1"));

                            await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                        }
                        button_Stop_PowerShellCORAC.Enabled = true;

                    }
                    else
                    {
                        button_Stop_PowerShellCORAC.Enabled = false;
                    }


                }
                catch (Exception E)
                {
                    Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                    Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                    Gerar_Arquivo.TratadorErros(E, GetType().Name);

                    Status_Informacao.Text = "Ocorreram erros ao acessar o repositório de configurações do CORAC.";
                }

                //-------------------------------------------------------------------------------------------------

                try
                {
                    bool ServerCORAC = await Task.Run(Iniciar_Servidor_AcessoRemoto);
                    if (ServerCORAC)
                    {

                        if (Registro_Corac.Status == StatusRegistro.Habilitado)
                        {
                            List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                            KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                            List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                            ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", "1"));

                            await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                        }

                        button_Stop_AR_CORAC.Enabled = true;
                    }
                    else
                    {
                        button_Stop_AR_CORAC.Enabled = false;
                    }

                    ConfiguracoesCORAC.Stop();

                }
                catch (Exception E)
                {
                    Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                    Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                    Gerar_Arquivo.TratadorErros(E, GetType().Name);

                    Status_Informacao.Text = "Ocorreram erros ao acessar o repositório de configurações do CORAC.";
                }

                //-------------------------------------------------------------------------------------------------

                ConfiguracoesCORAC.Stop();
                Status_Informacao.Text = "Repositório respondendo: Resposta: " + DateTime.Now.ToString();
            }
            else
            {
                Status_Informacao.Text = "O repositório de configurações está inacessível. Verificação: " + DateTime.Now.ToString();
            }

        }

        private void pictureBox_Powershell_MouseLeave(object sender, EventArgs e)
        {
            Status_Informacao.Text = "";
        }

        private void pictureBox_AcessoRemoto_MouseLeave(object sender, EventArgs e)
        {
            Status_Informacao.Text = "";
        }

        private void Relógio_Tick(object sender, EventArgs e)
        {
            Data_Sistema_TLPrincipal.Text = DateTime.Now.ToString();
            Console.WriteLine("Relogio");
        }

        private void CORAC_TPrincipal_Shown(object sender, EventArgs e)
        {
            Relógio.Start();
        }

        private async void onAssinatura_Tick(object sender, EventArgs e)
        {
            bool Assinatura = await ObterAssinatura();
            if (Assinatura)
            {
                await Verificar_Registro();
                onAssinatura.Stop();
            }
            else
            {
                Status_Informacao.Text = "A assinatura CORAC não está acessível ou não corresponde à correta, favor entrar em contato com os administradores. Verificação: " + DateTime.Now.ToString();
            }
        }

        private async void onNetwork_Tick(object sender, EventArgs e)
        {
            if(await Conexoes.VerificarConectividade())
            {
                Loaders();
                onConnect.Stop();
            }
            else
            {
                Status_Informacao.Text = "A conectividade não foi reestabelecida. Verificação: " + DateTime.Now.ToString();

            }
        }

        private void textBox_Path_ServerWEB_CORAC_TextChanged(object sender, EventArgs e)
        {

        }

    }






}
