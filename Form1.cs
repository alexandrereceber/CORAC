﻿using RegistroWindows;
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
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

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

        private Assinatura Sign;
        public static int TimeSleep = 20000;
        public static List<string> MsgIniciar = new List<string>();
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
            

            if (await Conexoes.VerificarConectividade())
            {
                MsgIniciar.Add("O acesso à internet está OK." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                 return true;
            }
            else
            {
                MsgIniciar.Add("Falha ao acessa internet." + " - Tempo: " + DateTime.Now.ToString() + "\n");
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

                string Path_Config = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                string ConfPath = Regex.Replace(Path_Config, "/Checked/", "/Update/", RegexOptions.IgnoreCase);
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


                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Update_System_256px, Vermelho, Azul);
                    picture_Internet_Status.Tag = "";
                    picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Color_fw;
                    MsgIniciar.Add("Repositório localizado." + " - Tempo: " + DateTime.Now.ToString() + "\n");
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
                MsgIniciar.Add("Ocorreu uma falha no repositório." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                return false;
            }
        }

        private async Task<bool> Verificar_Atualizacoes_Botao()
        {

            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);

            try
            {
                picture_Atualizacoes_CORAC.Image = Properties.Resources.Wait;
                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

                string Path_Config = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                string ConfPath = Regex.Replace(Path_Config, "/Checked/", "/Update/", RegexOptions.IgnoreCase);
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


                picture_Atualizacoes_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Update_System_256px, Vermelho, Azul);
                    picture_Internet_Status.Tag = "";
                    picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Color_fw;
                    MessageBox.Show("Repositório localizado." + " - Tempo: " + DateTime.Now.ToString(), "Atualização CORAC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    picture_Atualizacoes_CORAC.Image = Properties.Resources.Atualizacoes_Cinza_fw;
                    MessageBox.Show("O sistema de atualização não responde." + " - Tempo: " + DateTime.Now.ToString(), "Atualização CORAC", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Ocorreu uma falha no repositório." + " - Tempo: " + DateTime.Now.ToString(), "Atualização CORAC", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

            while (true)
            {
                try
                {
                    string Path_Config = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                    string ConfPath = Regex.Replace(Path_Config, "/Checked/", "/ConfCORAC/", RegexOptions.IgnoreCase);
                    Uri EndURI = new Uri(ConfPath);

                    HttpClient URL = new HttpClient();
                    var pairs = new List<KeyValuePair<string, string>> { };

                    var content = new FormUrlEncodedContent(pairs);
                    URL.Timeout = TimeSpan.FromSeconds(30);

                    Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                    await Task.WhenAll(Conteudo);


                    if (Conteudo.Result.IsSuccessStatusCode)
                    {
                        string Conf = await Conteudo.Result.Content.ReadAsStringAsync();
                        Conexoes.Config = JsonConvert.DeserializeObject<ConfiguracoesCORAC>(Conf);
                        MsgIniciar.Add("As configurações dos serviços CORAC foram carregados com sucesso." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                        return true;
                    }
                    else
                    {
                        MsgIniciar.Add("As configurações dos serviços CORAC não foram carregados." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                        Thread.Sleep(TimeSleep);
                    }

                }
                catch (Exception E)
                {
                    Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                    Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                    Gerar_Arquivo.TratadorErros(E, GetType().Name);

                    MsgIniciar.Add("Ocorreu erros ao buscar as configurações dos serviços CORAC." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                    Thread.Sleep(TimeSleep);
                }
            }
        }



        private async Task<bool> AutenticarUsuarioCORAC()
        {
            while (true)
            {
                try
                {
                    string Path_Config = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                    string ConfPath = Regex.Replace(Path_Config, "/Checked/", "/Validar/", RegexOptions.IgnoreCase);
                    Uri EndURI = new Uri(ConfPath);

                    LogarSistema Logar = new LogarSistema(EndURI);
                    await Logar.Logar(PerfilCORAC.UsuarioCORAC, PerfilCORAC.SenhaCORAC);
                    if (Logar.is_Logado())
                    {
                        PerfilCORAC.ChaveLogin = Logar.getChaveSessao();
                        PerfilCORAC.isLogon = true;
                        MsgIniciar.Add("A autenticação do usuário CORAC WEB foi realizada com sucesso!." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                        
                        return true;
                    }
                    else
                    {
                        if (Logar.GetInforError().Error)
                        {
                            MsgIniciar.Add(Logar.GetInforError().Mensagem + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            PerfilCORAC.isLogon = false;
                        }
                        else
                        {
                            MsgIniciar.Add("Error na autenticação do usuário CORAC. Favor entrar em contato com o departamento de tecnologia." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            PerfilCORAC.isLogon = false;
                        }

                        Thread.Sleep(TimeSleep);
                    }

                }
                catch (Exception E)
                {
                    MsgIniciar.Add("Servidor ou página inacessível." + " - Tempo: " + DateTime.Now.ToString() + "\n" + "Error: " + E.Message);
                    PerfilCORAC.isLogon = false;
                    Thread.Sleep(TimeSleep);
                }
            }
        }
        private async Task<bool> ObterAssinatura_Externa()
        {
            while (true)
            {
                try
                {
                    string SerieHD = Get_WMI.Obter_Atributo("win32_logicaldisk", "VolumeSerialNumber");
                    SerieHD = CalculaHash(SerieHD);

                    Uri EndURI = new Uri("http://192.168.15.10/CORAC/ValidarAAExterno/" + SerieHD + "/" + Dns.GetHostName() + "/" + Sign.Empresa + "/");

                    HttpClient URL = new HttpClient();
                    var pairs = new List<KeyValuePair<string, string>> { };

                    var content = new FormUrlEncodedContent(pairs);

                    URL.Timeout = TimeSpan.FromSeconds(30);

                    Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                    await Task.WhenAll(Conteudo);

                    if (Conteudo.Result.IsSuccessStatusCode)
                    {
                        string Dados = await Conteudo.Result.Content.ReadAsStringAsync();
                        AssinaturaExterna Signature = JsonConvert.DeserializeObject<AssinaturaExterna>(Dados);

                        if(Signature.Tempo.Date < DateTime.Now)
                        {
                            return true;
                        }

                        if (Signature.Maquina == Dns.GetHostName() && Signature.Signature == SerieHD && Signature.Valida == true)
                        {
                            MsgIniciar.Add("A assinatura externa CORAC está OK." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            Sign.Valida = true;
                            return true;
                        }
                        else
                        {
                            MsgIniciar.Add("A assinatura externa CORAC inacessível ou inválida!" + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            Sign.Valida = false;

                            Thread.Sleep(TimeSleep);
                        }

                    }
                    else
                    {
                        MsgIniciar.Add("Error ao buscar assinatura externa. Aguarde!" + " - Tempo: " + DateTime.Now.ToString() + "\n");
                        Thread.Sleep(TimeSleep);
                    }

                }
                catch (Exception E)
                {
                    MsgIniciar.Add("Assinatura externa: Internet, servidor ou página inacessível." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                    Thread.Sleep(TimeSleep);
                }
            }
        }
        /**
            <summary>
                Método utilizado na guia configurações
            </summary>
         */
        private async Task<bool> ObterAssinatura(string PathCORAC)
        {
            try
            {
                if (!await Conexoes.VerificarConectividade())
                {
                    MessageBox.Show("Não há conectividade!", "Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("Sem conectividade");
                }
                Uri EndURI = new Uri(PathCORAC);

                HttpClient URL = new HttpClient();
                var pairs = new List<KeyValuePair<string, string>> { };

                var content = new FormUrlEncodedContent(pairs);

                URL.Timeout = TimeSpan.FromSeconds(30);

                Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                await Task.WhenAll(Conteudo);

                if (Conteudo.Result.IsSuccessStatusCode)
                {
                    string Dados = await Conteudo.Result.Content.ReadAsStringAsync();
                    Assinatura Sign = JsonConvert.DeserializeObject<Assinatura>(Dados);
                    if (Sign.Sistema == "CORAC" && Sign.Signacture == "a4b315c63dca8337dc70ef6a336310f4")
                    {
                        Sign.Valida = true;
                        return true;
                    }
                    else
                    {
                        Sign.Valida = false;
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

        private async Task<bool> ObterAssinatura()
        {
            while (true)
            {
                try
                {
                    Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));

                    HttpClient URL = new HttpClient();
                    var pairs = new List<KeyValuePair<string, string>> { };

                    var content = new FormUrlEncodedContent(pairs);

                    URL.Timeout = TimeSpan.FromSeconds(30);

                    Task<HttpResponseMessage> Conteudo = URL.PostAsync(EndURI, content);
                    await Task.WhenAll(Conteudo);

                    if (Conteudo.Result.IsSuccessStatusCode)
                    {
                        string Dados = await Conteudo.Result.Content.ReadAsStringAsync();
                        Sign = JsonConvert.DeserializeObject<Assinatura>(Dados);
                        if (Sign.Sistema == "CORAC" && Sign.Signacture == "a4b315c63dca8337dc70ef6a336310f4")
                        {
                            MsgIniciar.Add("A assinatura CORAC está OK." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            Sign.Valida = true;
                            return true;
                        }
                        else
                        {
                            MsgIniciar.Add("A assinatura CORAC inacessível ou inválida!" + " - Tempo: " + DateTime.Now.ToString() + "\n");
                            Sign.Valida = false;

                            Thread.Sleep(TimeSleep);
                        }

                    }
                    else
                    {
                        Thread.Sleep(TimeSleep);
                    }

                }
                catch (Exception E)
                {
                    MsgIniciar.Add("Servidor ou página inacessível." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                    Thread.Sleep(TimeSleep);
                }
            }
        }

        public string CalculaHash(string Senha)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Senha);
                byte[] hash = md5.ComputeHash(inputBytes);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString(); // Retorna senha criptografada 
            }
            catch (Exception)
            {
                return null; // Caso encontre erro retorna nulo
            }
        }

        /**
         * <summary>
            Verifica se o software CORAC está licenciado. Esta verificação ocorre através da internet em umm site próprio do CORAC.
            Obs. IMPORTANTE: ESSE MÉTODO ENVIA UM INFORMAÇÃO DE QUE A MÁQUINA ESTÁ LIGADA. MAS NO FUTURO SERÁ CRIADO UM SERVIÇO PARA REALIZAR ESSA OPERAÇÃO, FICANDO A MESMA OBSOLETA.
         * </summary>
         */
        private async Task<bool> Verificar_Registro()
        {
            //Color Vermelho = Color.FromArgb(255, 255, 0, 0);
            //Color Azul = Color.FromArgb(255, 0, 1, 255);
            while (true)
            {
                try
                {
                    String Estado = "";

                    string NomeEstacao = Dns.GetHostName();

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
                                string Registro = (string)Dados.Value[0][2];
                                string SerieHD = Get_WMI.Obter_Atributo("win32_logicaldisk", "VolumeSerialNumber");
                                SerieHD = CalculaHash(SerieHD);

                                MsgIniciar.Add("Chave de Registro: " + SerieHD + " - Tempo: " + DateTime.Now.ToString() + "\n");

                                if (Registro == SerieHD)
                                {
                                    /**
                                    * Informando que a máquina está ligada
                                    */
                                    string getChave = Dados.Value[0][0].Value<string>();
                                    /*
                                     * Registro encontrado e equipamento ativado
                                     */
                                    Registro_Corac.Status = StatusRegistro.Habilitado;
                                    Registro_Corac.Chave_BD = getChave;
                                    MsgIniciar.Add("Equipamento e registro encontrados e ativados." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                                    FiltrosB.Clear();
                                    return true;
                                }
                                else
                                {
                                    MsgIniciar.Add("Equipamento com registro inválido!" + " - Tempo: " + DateTime.Now.ToString() + "\n");
                                    Thread.Sleep(TimeSleep);

                                }


                            }
                            else
                            {
                                MsgIniciar.Add("Equipamento encontrado e desativado. Requisitar habilitação em sua unidade de Tecnologia." + " - Tempo: " + DateTime.Now.ToString() + "\n");
                                FiltrosB.Clear();
                                Thread.Sleep(TimeSleep);
                            }

                        }
                        else
                        {
                            int Chassi = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "ChassisTypes")[0];
                            string SerieHD = Get_WMI.Obter_Atributo("win32_logicaldisk", "VolumeSerialNumber");
                            SerieHD = CalculaHash(SerieHD);


                            List<KeyValuePair<string, string>> IDados = new List<KeyValuePair<string, string>>();
                            IDados.Add(new KeyValuePair<string, string>("Tipo", Convert.ToString(Chassi)));
                            IDados.Add(new KeyValuePair<string, string>("Nome", NomeEstacao));
                            IDados.Add(new KeyValuePair<string, string>("Registro", SerieHD));

                            BuscarRegistro_CORAC.sendTabela = "334644edbd3aecbe746b32f4f2e8e5fb";
                            BuscarRegistro_CORAC.setDadosInserir(IDados);

                            Boolean Adicionar = await BuscarRegistro_CORAC.InserirDadosTabela();
                            if (Adicionar)
                            {
                                MsgIniciar.Add("O agente autônomo foi adicionado com sucesso, favor entrar em contato com administrador para habilitação. Nome: " + NomeEstacao + " - Tempo: " + DateTime.Now.ToString());

                            }
                            else
                            {
                                MsgIniciar.Add("O agente autônomo não pôde ser adicionado. Nome: " + NomeEstacao + " - Tempo: " + DateTime.Now.ToString() + "\n");

                            }

                            Thread.Sleep(TimeSleep);

                        }

                    }
                    else
                    {
                        MsgIniciar.Add("O Site do CORAC devolveu um erro." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                        FiltrosB.Clear();
                        Thread.Sleep(TimeSleep);

                    }


                }
                catch (Exception E)
                {
                    Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                    Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                    Gerar_Arquivo.TratadorErros(E, GetType().Name);


                    MsgIniciar.Add("Error reportado pelo tratador de erros." + " - Tempo: " + DateTime.Now.ToString() + "\n");

                    Thread.Sleep(TimeSleep);

                }
            }
            
        }

        private async Task<bool> Verificar_Registro_Botao()
        {
            try
            {
                pictureBox_Registro_CORAC.Image = Properties.Resources.Wait;
                pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;

                if (Sign.Valida)
                {
                    String Estado = "";

                    string NomeEstacao = Dns.GetHostName();



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
                                /*
                                 * Registro encontrado e equipamento ativado
                                 */
                                Registro_Corac.Status = StatusRegistro.Habilitado;
                                Registro_Corac.Chave_BD = getChave;
                                MessageBox.Show("Registro encontrado e equipamento ativado. Requisitar habilitação em sua unidade Tecnologia." + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                FiltrosB.Clear();
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Registro encontrado e equipamento desativado. Requisitar habilitação em sua unidade de Tecnologia." + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                FiltrosB.Clear();
                                return false;

                            }

                        }
                        else
                        {
                            int Chassi = Get_WMI.Obter_Atributo("Win32_SystemEnclosure", "ChassisTypes")[0];
                            //string SerieHD = Get_WMI.Obter_Atributo("win32_logicaldisk", "VolumeSerialNumber");

                            List<KeyValuePair<string, string>> IDados = new List<KeyValuePair<string, string>>();
                            IDados.Add(new KeyValuePair<string, string>("Tipo", Convert.ToString(Chassi)));
                            IDados.Add(new KeyValuePair<string, string>("Nome", NomeEstacao));

                            BuscarRegistro_CORAC.sendTabela = "334644edbd3aecbe746b32f4f2e8e5fb";
                            BuscarRegistro_CORAC.setDadosInserir(IDados);

                            Boolean Adicionar = await BuscarRegistro_CORAC.InserirDadosTabela();
                            if (Adicionar)
                            {
                                MessageBox.Show("O agente autônomo foi adicionado com sucesso, favor entrar em contato com administrador para habilitação. Nome: " + NomeEstacao + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;

                            }
                            else
                            {
                                MessageBox.Show("O agente autônomo não pôde ser adicionado. Nome: " + NomeEstacao + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;

                            }

                        }

                    }
                    else
                    {
                        MessageBox.Show("O Site do CORAC devolveu um erro." + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        FiltrosB.Clear();
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("A assinatura não é válida." + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
            catch (Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                MessageBox.Show("Error reportado pelo tratador de erros." + " - Tempo: " + DateTime.Now.ToString(), "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Error);


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


                bool Server_WEBSOCKT = await Task.Run(ServidorWEB_Socket.StopServidor);

                //------------------------------------------------------------------------------------------------------

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_WEBSOCKT)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Azul, Vermelho);
                    pictureBox_AcessoRemoto.Tag = "O serviço de acesso remoto foi interrompido com sucesso!";
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Cinza_fw;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Vermelho, Azul);
                    pictureBox_AcessoRemoto.Tag = "Ocorreu um erro ao parar o serviço de acesso remoto.";
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
                    pictureBox_Powershell.Tag = "O serviço de powershell foi parado corretamento.";

                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Cinza_fw ;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Vermelho, Azul);
                    pictureBox_Powershell.Tag = "Ocorreu um erro ao parar o serviço de powershell.";
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
                AbrirComando.setPath_CORACWEB = (string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC");
                AbrirComando.StartServidor();

                Autent_WEB = new Autenticador_WEB();
                Uri EndURI = new Uri((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC"));
                Autent_WEB.Endereco_Autenticacao(EndURI, "/CORAC/ValidarLoginAA/");

                //-------------------SERVIDOR DE HTTP-------------------------------------------------------------------

                string EndString = Conexoes.EnderecoHttpListen_Powershell();
                int EndPorta = Conexoes.Config.Servicos.PowerShell;

                ServidorWEB_Local = new Servidor_HTTP();
                
                ServidorWEB_Local.SetTratador_Erros(TipoSaidaErros.Arquivo);

                ServidorWEB_Local.AddPrefixos(null, EndString, "CORAC/SYNCPCT/", EndPorta);

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
                    CORAC_TPrincipal.MsgIniciar.Add("Serviço powershell carregado corretamente.\n");
                    pictureBox_Powershell.Image = Properties.Resources.Powershell_Color_fw;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_PS_Core_128px, Azul, Vermelho);
                    pictureBox_Powershell.Tag = "Ocorreu um erro no serviço de powershell desta estação.";
                    CORAC_TPrincipal.MsgIniciar.Add("Ocorreu um erro no serviço de powershell desta estação.\n");
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
                Autent_WEB.Endereco_Autenticacao(EndURI, "/CORAC/ValidarLoginAA/");


                //-------------------SERVIDOR DE HTTP-------------------------------------------------------------------

                string EndString = Conexoes.EnderecoHttpListen_AcessoRemoto();
                int EndPorta = Conexoes.Config.Servicos.AR;

                ServidorWEB_Socket = new Servidor_WEBSOCKET();

                ServidorWEB_Socket.SetTratador_Erros(TipoSaidaErros.Arquivo);

                ServidorWEB_Socket.AddPrefixos(null, EndString, "CORAC/AcessoRemoto/", EndPorta);
                ServidorWEB_Socket.AddPrefixos(null, EndString, "CORAC/AA_AcessoRemoto_SYN/", EndPorta);

                ServidorWEB_Socket.Autenticador = Autent_WEB;
                ServidorWEB_Socket.Gerenciador_Cliente = GerenteClientes;

                
                bool Server_WEBSOCKET = await Task.Run(ServidorWEB_Socket.StartServidor);

                //------------------------------------------------------------------------------------------------------

                pictureBox_AcessoRemoto.SizeMode = PictureBoxSizeMode.StretchImage;

                if (Server_WEBSOCKET)
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Vermelho, Azul);
                    pictureBox_AcessoRemoto.Tag = "Serviço de acesso remoto foi carregado corretamente.\n";
                    CORAC_TPrincipal.MsgIniciar.Add("Serviço de acesso remoto foi carregado corretamente.\n");
                    pictureBox_AcessoRemoto.Image = Properties.Resources.AcessoRemoto_Color_fw ;
                    return true;
                }
                else
                {
                    //Bitmap Internet_ON = Change_Color(Properties.Resources.Status_AcessoRemoto_128px, Azul, Vermelho);
                    pictureBox_AcessoRemoto.Tag = "Ocorreu um erro ao iniciar o serviço de acesso remoto desta estação.\n";
                    CORAC_TPrincipal.MsgIniciar.Add("Ocorreu um erro ao iniciar o serviço de acesso remoto desta estação.\n");
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
            button_VerificarInternet.Enabled = false;
            picture_Internet_Status.Image = Properties.Resources.Wait;
            picture_Internet_Status.SizeMode = PictureBoxSizeMode.CenterImage;

            if (await Verificar_Conectividade())
            {
                button_VerificarInternet.Enabled = true;
                picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                picture_Internet_Status.Image = Properties.Resources.Internet_Cor_fw;

                Task Atualizar, 
                    Assinatura, 
                    Assinatura_Externa,
                    Registro, 
                    BuscarConfiguracoes,
                    Powerhell_WEB, 
                    Acesso_Remoto, 
                    LogonUserCorac;

                int Atualizar_ID = 0,
                    Assinatura_ID = 0,
                    Assinatura_Externa_ID = 0,
                    BuscarConfiguracoes_ID = 0,
                    Registro_ID = 0,
                    Powerhell_WEB_ID = 0,
                    Acesso_Remoto_ID = 0,
                    LogonUserCorac_ID = 0,
                    SPower = 0, SAR = 0;
                List<Task> Servicos = new List<Task>();

                Atualizar = Task.Run(Verificar_Atualizacoes);
                Atualizar_ID = Atualizar.Id;

                LogonUserCorac = Task.Run(AutenticarUsuarioCORAC);
                LogonUserCorac_ID = LogonUserCorac.Id;

                Assinatura = Task.Run(ObterAssinatura);
                Assinatura_ID = Assinatura.Id;
                

                Servicos.Add(Atualizar);
                Servicos.Add(LogonUserCorac);
                Servicos.Add(Assinatura);

                while (Servicos.Count > 0)
                {
                    /*Coleção de serviços CORAC*/
                    Task Tarefa = await Task.WhenAny(Servicos);

                    if (Tarefa.Id == Atualizar_ID)
                    {
                        Servicos.Remove(Tarefa);
                        button_AtualizacoesCORAC.Enabled = true;

                    }

                    else if (Tarefa.Id == LogonUserCorac_ID)
                    {
                        Servicos.Remove(Tarefa);

                    }
                    else if (Tarefa.Id == Assinatura_Externa_ID)
                    {
                        Servicos.Remove(Tarefa);

                        Task<bool> ResultRegistro = (Task<bool>)Tarefa;
                        if (ResultRegistro.Result)
                        {
                            pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.CenterImage;
                            pictureBox_Registro_CORAC.Image = Properties.Resources.Wait;

                            Registro = Task.Run(Verificar_Registro);
                            Registro_ID = Registro.Id;
                            Servicos.Add(Registro);

                        }

                    }
                    else if (Tarefa.Id == Assinatura_ID)
                    {
                        Servicos.Remove(Tarefa);

                        Task<bool> ResultRegistro = (Task<bool>)Tarefa;
                        if (ResultRegistro.Result)
                        {
                            Assinatura_Externa = Task.Run(ObterAssinatura_Externa);
                            Assinatura_Externa_ID = Assinatura_Externa.Id;
                            Servicos.Add(Assinatura_Externa);
                        }

                    }
                    else if (Tarefa.Id == Registro_ID)
                    {
                        Servicos.Remove(Tarefa);
                        Task<bool> ResultConfigServ = (Task<bool>)Tarefa;
                        if (ResultConfigServ.Result)
                        {

                            BuscarConfiguracoes = Task.Run(Buscar_ConfiguracoesCORAC);
                            BuscarConfiguracoes_ID = BuscarConfiguracoes.Id;
                            Servicos.Add(BuscarConfiguracoes);

                        }
                        

                    }
                    else if (Tarefa.Id == BuscarConfiguracoes_ID)
                    {
                        Servicos.Remove(Tarefa);

                        Task<bool> ResultConfig = (Task<bool>)Tarefa;
                        if (ResultConfig.Result)
                        {
                            pictureBox_Registro_CORAC.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox_Registro_CORAC.Image = Properties.Resources.Registro_Color_fw;

                            Powerhell_WEB = Task.Run(Iniciar_Servidor_PowerShell);
                            Powerhell_WEB_ID = Powerhell_WEB.Id;
                            Servicos.Add(Powerhell_WEB);

                            Acesso_Remoto= Task.Run(Iniciar_Servidor_AcessoRemoto);
                            Acesso_Remoto_ID = Acesso_Remoto.Id;
                            Servicos.Add(Acesso_Remoto);
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

            }

            return true;

        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public CORAC_TPrincipal()
        {

            ObterConfiguracoes();

            if((string)ChavesCORAC.Obter_ConteudoCampo("Path_ServerWEB_CORAC") == "")
            {
                InitializeComponent();
                Control[] TStatus = tabCORAC.Controls.Find("Tab_Status", false);
                Control[] TLog = tabCORAC.Controls.Find("Tab_Log", false);
                Control[] TMsg = tabCORAC.Controls.Find("tab_Mensagens", false);


                tabCORAC.Controls.Remove(TStatus[0]);
                tabCORAC.Controls.Remove(TLog[0]);
                tabCORAC.Controls.Remove(TMsg[0]);
                if (!IsAdministrator())
                {
                    
                }
                MessageBox.Show("Endereço do servidor CORAC WEB está vazio!\nFavor entrar em contato com o departamento de tecnologia.", "Servidor CORAC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                InitializeComponent();
                Loaders();
            }


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
            try
            {
                await SairSistema();
            }
            catch(Exception E)
            {

            }
            Application.Exit();
            
        }

        private async Task<bool> SairSistema()
        {

            try
            {
                List<KeyValuePair<string, string>> KDados = new List<KeyValuePair<string, string>>();
                KDados.Add(new KeyValuePair<string, string>("0", Registro_Corac.Chave_BD));

                List<KeyValuePair<string, string>> ADados = new List<KeyValuePair<string, string>>();
                ADados.Add(new KeyValuePair<string, string>("SPowershell", "0"));
                ADados.Add(new KeyValuePair<string, string>("SAcessoRemoto", "0"));
                ADados.Add(new KeyValuePair<string, string>("SChat", "0"));
                
                await AtualizarTabelas_CORAC("334644edbd3aecbe746b32f4f2e8e5fb", KDados, ADados);
                return true;

            }
            catch (Exception e)
            {
                //Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                //Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.ShowWindow);
                //Gerar_Arquivo.TratadorErros(e, GetType().Name);
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
                if (KeysValues == null) MessageBox.Show("Nenhuma alteração foi identificada.", "Alterações",MessageBoxButtons.OK,MessageBoxIcon.Error);
                else
                {
                    DialogResult Resposta = MessageBox.Show("Tem certeza que deseja realizar essa operação?", "Salvar configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (Resposta == DialogResult.Yes)
                    {

                        if (KeysValues.Count > 0)
                        {
                            if (ChavesCORAC.Gravar_ConteudoCampo(TipoChave.LocalMachine, "software\\CORAC", ref KeysValues))
                            {
                                MessageBox.Show("O dados foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ObterConfiguracoes();
                            }
                            else
                            {
                                MessageBox.Show("O dados não foram salvos com sucesso!", "Salvar alterações", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            //KeysValues.Clear();
                        }
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
                button_Servidor_WEB.Enabled = false;
                SalvaConfiguracoes.Enabled = false;
                pictureBox_Servidor_WEB.Image = Properties.Resources.Wait;

                if (await ObterAssinatura(textBox_Path_ServerWEB_CORAC.Text))
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.Acepty;
                    SalvaConfiguracoes.Enabled = true;
                }
                else
                {
                    pictureBox_Servidor_WEB.Image = Properties.Resources.No_Acepty;
                }
                button_Servidor_WEB.Enabled = true;

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
                button_VerificarInternet.Enabled = false;
                picture_Internet_Status.Image = Properties.Resources.Wait;
                picture_Internet_Status.SizeMode = PictureBoxSizeMode.CenterImage;

                if (await Conexoes.VerificarConectividade())
                {
                    picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                    picture_Internet_Status.Image = Properties.Resources.Internet_Cor_fw;
                    MessageBox.Show("O acesso à internet está OK.", "Internet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    picture_Internet_Status.SizeMode = PictureBoxSizeMode.StretchImage;
                    picture_Internet_Status.Image = Properties.Resources.Internet_Cinza_fw;
                    MessageBox.Show("Não há conectividade!.", "Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                button_VerificarInternet.Enabled = true;


            }
            finally
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
                await Verificar_Atualizacoes_Botao();
            }finally
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
            Status_Informacao.Text = (string)(sender as PictureBox).Tag;
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
            //Console.WriteLine("Relogio");
        }

        private void CORAC_TPrincipal_Shown(object sender, EventArgs e)
        {
            Relógio.Start();
        }

        private async void onNetwork_Tick(object sender, EventArgs e)
        {
            if(await Conexoes.VerificarConectividade())
            {
                Loaders();
                onNetwork.Stop();
            }
            else
            {
                MsgIniciar.Add("A conectividade ainda não foi reestabelecida. Verificação: " + DateTime.Now.ToString()+ "\n");

            }
        }

        private void textBox_Path_ServerWEB_CORAC_TextChanged(object sender, EventArgs e)
        {

        }

        private void onConnect_Tick(object sender, EventArgs e)
        {

        }

        private void reiniciarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult Restart = MessageBox.Show("Tem certeza que deseja reiniciar a aplicação?", "Reiniciar aplicação", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(Restart == DialogResult.Yes)
                Application.Restart();
        }

        private async void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult Restart = MessageBox.Show("Tem certeza que deseja sair da aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Restart == DialogResult.Yes)
            {
                try
                {
                    await SairSistema();
                }
                finally
                {

                }
                Application.Exit();

            }
        }

        private void Reiniciar_Click(object sender, EventArgs e)
        {
            DialogResult Restart = MessageBox.Show("Tem certeza que deseja reiniciar a aplicação?", "Reiniciar aplicação", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (Restart == DialogResult.Yes)
                Application.Restart();
        }

        private void pictureBox_AcessoRemoto_MouseEnter(object sender, EventArgs e)
        {
            Status_Informacao.Text = (string)(sender as PictureBox).Tag;
        }

        private void MenuPrincipal_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabCORAC_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl TabSelecionada = (TabControl)sender;
            if(TabSelecionada.SelectedIndex == 2)
            {
                MSGInfo.Clear();
                foreach(string Msgs in MsgIniciar)
                {
                    MSGInfo.AppendText(Msgs + "\n");
                }
            }
        }

        private void picture_Internet_Status_MouseLeave(object sender, EventArgs e)
        {
            Status_Informacao.Text = "";
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
            catch(Exception E)
            {
                Tratador_Erros Gerar_Arquivo = new Tratador_Erros();
                Gerar_Arquivo.SetTratador_Erros(TipoSaidaErros.Arquivo);
                Gerar_Arquivo.TratadorErros(E, GetType().Name);

                return false;
            }


            
        }
        private async void Admin_CriarUsuario_Click(object sender, EventArgs e)
        {
            if (IsAdministrator())
            {
                PCriar_Usuario.Image = Properties.Resources.Wait;
                Admin_CriarUsuario.Enabled = false;

                bool VAdmin = await Task.Run(() => Verify_Counta());

                if (VAdmin)
                {

                    bool VConta = await Task.Run(() => CriarConta_CORAC());

                    if (VConta)
                    {
                        MessageBox.Show("Usuário CORAC criado com sucesso!", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível criar o usuário CORAC, favor verificar os logs de erro.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Você não é administrador, favor elevar os privilégio!", "Usuário CORAC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                PCriar_Usuario.Image = null;
                Admin_CriarUsuario.Enabled = true;
            }
            else
            {
                MessageBox.Show("Você não é administrador, favor elevar os privilégios!", "Usuário CORAC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

        }

        private void Start_Win_Click(object sender, EventArgs e)
        {

            if (IsAdministrator())
            {
                try
                {
                    List<KeyValuePair<string, string>> CR = new List<KeyValuePair<string, string>>();
                    CR.Add(new KeyValuePair<string, string>("CORAC", "\"%ProgramFiles(x86)%\\CORAC\\CORAC.exe\""));
                    RegistroWin32 Criar_Start = new RegistroWin32();
                    if (Criar_Start.Gravar_ConteudoCampo(TipoChave.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", ref CR))
                    {
                        MessageBox.Show("Inicialização automática configurada!", "Start Automático", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("Inicialização automática não foi configurada!", "Start Automático", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Você não é administrador, favor elevar os privilégios!", "Start Automático", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

    }






}
