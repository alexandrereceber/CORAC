using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

/**
 * Usa como Referência as bibliotecas
 * Newtonsoft.Json
 * System.Net.Http
 * */
namespace CamadaDeDados.RESTFormat
{
    /**
     * Struct que representa a mensage de erro que foi gerada no servidor REST.
     */
struct InforError
    {
        public string Modo;
        public Boolean Error;
        public int Codigo;
        public string Mensagem;
        public string Tracer;
        public string File;

    }
    /**
     * Pacote que contém todos os dados do último login realizado como: Usuário e senha, tentativas de acesso, chave de sessão
     */
    struct PLogin
    {
        public Boolean Error;
        public string Modo;
        public string Chave;
        public string TipoUsuario;
        public int Tentativas;
        public string Header;
    }
    /**
     * Classe que realiza a autenticação no sistema através da internet.
     */
    class LogarSistema : HttpClient
    {
        private readonly Uri Dados_Endereco = null;
        private string Chave_Sessao = null;
        private bool isLogado = false;
        private InforError InfoError;
        private PLogin PacoteLogin;

        CancellationToken TokenCancel;

        private HttpResponseMessage ChamadaREST = null;
        private string ResultSET = null;
        private JObject ResultSETJSON;

        /**
         * Representa o nome do usuário que esta tentando logar no sistema.
         */
        private string Usuario = null;

        /**
         * Representa a respectiva senha do usuário que esta tentando logar no sistema.
         */
        private string Senha = null;

        /**
         * Construtor da classe que recebe, como parâmetro, o endereço do servidor que criará a saída de dados.
         */
        public LogarSistema(string End)
        {
            Regex Valid_End = new Regex(@"\bhttp://\b",RegexOptions.IgnoreCase);
            if (!Valid_End.IsMatch(End))
            {
                End = "http://" + End;
            }
            Dados_Endereco = new Uri(End);
        }
        /**
         * Autentica o usuário no sistema.
         */ 
        public async Task<bool> Logar(string US, string PS)
        {
            Usuario = US;
            Senha = PS;

            KeyValuePair<string, string>[] LogIN = new KeyValuePair<string, string>[3];
            LogIN[0] = new KeyValuePair<string, string>("sendUsuario", Usuario);
            LogIN[1] = new KeyValuePair<string, string>("sendSenha", Senha);
            LogIN[2] = new KeyValuePair<string, string>("sendDispositivo", "Movel");
            FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(LogIN);

            TokenCancel = new CancellationToken();
            try
            {
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    isLogado = false;
                    InfoError = new InforError{
                                                    Modo = ResultSETJSON["Modo"].Value<string>(),
                                                    Codigo = ResultSETJSON["Codigo"].Value<int>(),
                                                    Error = ResultSETJSON["Error"].Value<Boolean>(),
                                                    Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                                                    File = ResultSETJSON["File"].Value<string>(),
                                                    Tracer = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    isLogado = true;
                    Chave_Sessao = ResultSETJSON["Chave"].Value<string>();

                    PacoteLogin = new PLogin {
                                                Error = ResultSETJSON["Error"].Value<Boolean>(),
                                                Modo = ResultSETJSON["Modo"].Value<string>(),
                                                Chave = ResultSETJSON["Chave"].Value<string>(),
                                                TipoUsuario = ResultSETJSON["TipoUsuario"].Value<string>(),
                                                Tentativas = ResultSETJSON["Tentativas"].Value<int>(),
                                                Header = ResultSETJSON["Header"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return true;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /**
         * Retorna uma struct dos dados dos erros ocorridos no servidor.
         */
        public InforError GetInforError()
        {
            return InfoError;
        }
        /**
         * Retorna a chave da sessão. Se o retorno for null é porque não houve login ou ocorreram erros.
         */
        public string getChaveSessao()
        {
            return Chave_Sessao;
        }
        /**
         * Retorna os dados, em formato struct, do login.
         */
        public PLogin getPacoteLogin()
        {
            return PacoteLogin;
        }
        /**
         * Informa se existe alguém logado no sistema.
         */
        public Boolean is_Logado()
        {
            return isLogado;
        }

    }
    /**
     * Struct que conterá os dados da tabela, gerados no servidor, para uso do sistema.
     */
    struct PacoteSelecionarDados
    {
        public JProperty Modo;
        public JProperty Error;
        public JProperty NomeTabela;
        public JProperty ResultadoDados;
        public JProperty Campos;
        public JProperty ChavesPrimarias;
        public JProperty Paginacao;
        public JProperty InfoPaginacao;
        public JProperty Botoes;
        public JProperty ContadorLinha;
        public JProperty OrdemBy;
        public JProperty Filtros;
        public JProperty ShowColumnsIcones;
        public JProperty Formato;
        public JProperty Indexador;
        public JProperty TempoTotal;
        
    }

    struct PacoteInserirDados
    {
        public JProperty Modo;
        public JProperty Error;
        public JProperty lastID;
        public JProperty TempoTotal;

    }
    struct PacoteExcluirDados
    {
        public JProperty Modo;
        public JProperty Error;
        public JProperty TempoTotal;

    }

    struct PacoteAtualizarDados
    {
        public JProperty Modo;
        public JProperty Error;
        public JProperty TempoTotal;

    }
    /**
     * Tipos de Filtros existentes e possíveis de se usar.
     */
    enum TipoFiltro
    {
        Padrao = 0,
        Buscar = 1,
        Campos = 2
    }
    /**
     * Classe que busca dos dados das tabelas nos bancos de dados.
     */
    class Tabelas : HttpClient
    {
        const string 
            Select  = "ab58b01839a6d92154c615db22ea4b8f", 
            Insert  = "5a59ffc82a16fc2b17daa935c1aed3e9", 
            Update  = "1b24931707c03902dad1ae4b42266fd6", 
            Delete  = "1570ef32c1a283e79add799228203571";

        private readonly Uri Dados_Endereco = null;
        private readonly string Chave_Sessao;

        private string NomeTabela = null;
        public string sendTabela { set { NomeTabela = value; } }

        private List<KeyValuePair<string, string>> ListaFiltrosPadroes = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> ConjuntoDadosInserir = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> ConjuntoKey = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> ConjuntoDadosAtualizar = new List<KeyValuePair<string, string>>();

        private Boolean Status = false;
        private Boolean is_error = false;
        public  Boolean getError { get { return is_error; } }
        private InforError InfoError;
        private PacoteSelecionarDados   PacoteTBL;
        private PacoteInserirDados      PacoteIDados;
        private PacoteExcluirDados      PacoteEDados;
        private PacoteAtualizarDados    PacoteADados;

        private List<KeyValuePair<string, string>> EnviarParametros = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> EnviarParametrosSelect = new List<KeyValuePair<string, string>>();
        CancellationToken TokenCancel;

        private HttpResponseMessage ChamadaREST = null;
        private string ResultSET = null;
        private JObject ResultSETJSON;
        public Tabelas(string End, string Chave = null)
        {
            Chave_Sessao = Chave;

            Regex Valid_End = new Regex(@"\bhttp://\b", RegexOptions.IgnoreCase);
            if (!Valid_End.IsMatch(End))
            {
                End = "http://" + End;
            }
            Dados_Endereco = new Uri(End);

        }

        private string getMD5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /**
         * Adicionar filtros de busca ao sistema que podem ser os padrões, buscas e por campos.
         * Ex. de filtro padrão.
         * Filtros padrões são aqueles em que o sistema já vai predefinido, mesmo antes o usuário escolher qualquer
         * filtragem, esse filtro vem depois do filtro padrão dentro da classe PHP que acontece antes mesmo desse filtro.
         * 
         *      List<KeyValuePair<int, string[]>> Filtros = new List<KeyValuePair<int, string[]>>();
         *      Filtros.Add(new KeyValuePair<int, string[]>(0, new string[4] { "1", "like", "%02%", "1" }));
         *      Filtros.Add(new KeyValuePair<int, string[]>(1, new string[4] { "2", "like",  "%8%", "1" }));
         *      KeyValuePair<int, string[]> ft = new KeyValuePair<int, string[]>(0 , new string[4]{"1","like","%02%","1"});

         */
        public void setFiltros(TipoFiltro Tipo, List<KeyValuePair<int, string[]>> Lst)
        {
            foreach (KeyValuePair<int, string[]> i in Lst)
            {
                ListaFiltrosPadroes.Add(new KeyValuePair<string, string>("sendFiltros[" + (int)Tipo + "][" + i.Key + "][0]", (string)i.Value.GetValue(0)));
                ListaFiltrosPadroes.Add(new KeyValuePair<string, string>("sendFiltros[" + (int)Tipo + "][" + i.Key + "][1]", (string)i.Value.GetValue(1)));
                ListaFiltrosPadroes.Add(new KeyValuePair<string, string>("sendFiltros[" + (int)Tipo + "][" + i.Key + "][2]", (string)i.Value.GetValue(2)));
                ListaFiltrosPadroes.Add(new KeyValuePair<string, string>("sendFiltros[" + (int)Tipo + "][" + i.Key + "][3]", (string)i.Value.GetValue(3)));
            }
            if (ListaFiltrosPadroes.Count > 0)
            {
                EnviarParametros.AddRange(ListaFiltrosPadroes);
            }

        }
        /**
         * Busca os dados de uma tabela mapeada no sistema.
         */
        public async Task<Boolean> SelectTabelaJSON()
        {
            /**
             * Limpa os parâmetros antes de adicionar outros
             */
            EnviarParametrosSelect.Clear();

            try
            {
                if (NomeTabela == null) { throw new Exception("Não foi definido nenhum nome de tabela."); }

                KeyValuePair<string, string> ChaveDaSessao  = new KeyValuePair<string, string>("enviarChaves", Chave_Sessao);
                KeyValuePair<string, string> Tabelas        = new KeyValuePair<string, string>("sendTabelas", NomeTabela);
                KeyValuePair<string, string> ModoOperacao   = new KeyValuePair<string, string>("sendModoOperacao", Select);
                KeyValuePair<string, string> Dispositivo    = new KeyValuePair<string, string>("sendDispositivo", "pc");

                EnviarParametrosSelect.Add(ChaveDaSessao);
                EnviarParametrosSelect.Add(Tabelas);
                EnviarParametrosSelect.Add(ModoOperacao);
                EnviarParametrosSelect.Add(Dispositivo);
                /**
                 * Copias os parâmentros gerais e mantem a variável EnviarParametrosSelect para ser usada pela instrução refreshDados()
                 */
                EnviarParametrosSelect.AddRange(EnviarParametros);

                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametrosSelect);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ListaFiltrosPadroes.Clear();
                EnviarParametros.Clear(); //Limpa todos os filtros.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                        {
                            Modo = ResultSETJSON["Modo"].Value<string>(),
                            Codigo = ResultSETJSON["Codigo"].Value<int>(),
                            Error = ResultSETJSON["Error"].Value<Boolean>(),
                            Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                            File = ResultSETJSON["File"].Value<string>(),
                            Tracer = ResultSETJSON["Tracer"].Value<string>()
                        };
                    ResultSETJSON = null;
                    PacoteTBL = new PacoteSelecionarDados
                    {
                        Error = new JProperty("Error",true)
                    };
                    return false;

                }
                else
                {
                    is_error = false;
                PacoteTBL = new PacoteSelecionarDados
                {
                        Modo = ResultSETJSON.Property("Modo"),
                        Error = ResultSETJSON.Property("Error"),
                        NomeTabela = ResultSETJSON.Property("NomeTabela"),
                        ResultadoDados = ResultSETJSON.Property("ResultDados"),
                        Campos = ResultSETJSON.Property("Campos"),
                        ChavesPrimarias = ResultSETJSON.Property("ChavesPrimarias"),
                        Paginacao = ResultSETJSON.Property("Paginacao"),
                        InfoPaginacao = ResultSETJSON.Property("InfoPaginacao"),
                        Botoes = ResultSETJSON.Property("Botoes"),
                        ContadorLinha = ResultSETJSON.Property("ContadorLinha"),
                        OrdemBy = ResultSETJSON.Property("OrdemBy"),
                        Filtros = ResultSETJSON.Property("Filtros"),
                        ShowColumnsIcones = ResultSETJSON.Property("ShowColumnsIcones"),
                        Formato = ResultSETJSON.Property("Formato"),
                        Indexador = ResultSETJSON.Property("Indexador"),
                        TempoTotal = ResultSETJSON.Property("TempoTotal")
                    };
                    
                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/SelectTabelaJson",
                    Tracer = ex.StackTrace
                };
                return false;
            }
        }
        /**
         * Armazena as chaves e todos os nome de campos e seus respectivos valores.
         */
        public void setDadosInserir(List<KeyValuePair<string, string>> Lst)
        {
            int TCampos = ConjuntoDadosInserir.Count;
            foreach (KeyValuePair<string, string> i in Lst)
            {
                ConjuntoDadosInserir.Add(new KeyValuePair<string, string>("sendCamposAndValores[" + TCampos + "][name]", i.Key));
                ConjuntoDadosInserir.Add(new KeyValuePair<string, string>("sendCamposAndValores[" + TCampos + "][value]", i.Value));
                TCampos++;
            }
            if (ConjuntoDadosInserir.Count > 0)
            {
                EnviarParametros.AddRange(ConjuntoDadosInserir);
            }

        }
        /**
         * Inserir dados na tabela informada.
         * List<KeyValuePair<string, string>> IDados = new List<KeyValuePair<string, string>>();
         * IDados.Add(new KeyValuePair<string, string>("MaquinaR", "rf061040555swtbos"));
         * IDados.Add(new KeyValuePair<string, string>("patri", "666666"));
         * tab.sendTabela = "64b99121f7e18c0f8586f30bf78062e0";
         * tab.setDadosInserir(IDados);
         * Boolean f = await tab.InserirDadosTabela();
        */
        public async Task<Boolean> InserirDadosTabela()
        {
            try
            {
                if (Status) { throw new Exception("Já existe um pedido sendo realizado, favor aguardar..."); }
                if (NomeTabela == null) { throw new Exception("Não foi definido nenhum nome de tabela."); }

                if (ConjuntoDadosInserir.Count == 0){ throw new Exception("Não há dados para serem enviados.");}

                /**
                 * Armazena o nome da tabela que será utilizada para a busca, inserção, delete e update dos dados.
                 */
                KeyValuePair<string, string> ChaveDaSessao = new KeyValuePair<string, string>("enviarChaves", Chave_Sessao);
                KeyValuePair<string, string> Tabelas = new KeyValuePair<string, string>("sendTabelas", NomeTabela);
                KeyValuePair<string, string> ModoOperacao = new KeyValuePair<string, string>("sendModoOperacao", Insert);
                KeyValuePair<string, string> Dispositivo = new KeyValuePair<string, string>("sendDispositivo", "Movel");
            

                EnviarParametros.Add(ChaveDaSessao);
                EnviarParametros.Add(Tabelas);
                EnviarParametros.Add(ModoOperacao);
                EnviarParametros.Add(Dispositivo);

                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametros);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ConjuntoDadosInserir.Clear();
                EnviarParametros.Clear(); //Limpa todos os filtros.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                    {
                        Modo = ResultSETJSON["Modo"].Value<string>(),
                        Codigo = ResultSETJSON["Codigo"].Value<int>(),
                        Error = ResultSETJSON["Error"].Value<Boolean>(),
                        Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                        File = ResultSETJSON["File"].Value<string>(),
                        Tracer = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    is_error = false;
                    PacoteIDados = new PacoteInserirDados
                    {
                        Modo = ResultSETJSON.Property("Modo"),
                        Error = ResultSETJSON.Property("Error"),
                        lastID = ResultSETJSON.Property("lastId"),
                        TempoTotal = ResultSETJSON.Property("TempoTotal")

                    };

                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/InserirDadosTabela()",
                    Tracer = ex.StackTrace
                };
                ResultSETJSON = null;
                return false;
            }
        }
        /**
         * Armazena as chaves e todos os nome de campos e seus respectivos valores.
         */
        public void setDadosExcluir(List<KeyValuePair<string, string>> Lst)
        {
            int TCampos = ConjuntoKey.Count;
            foreach (KeyValuePair<string, string> i in Lst)
            {
                ConjuntoKey.Add(new KeyValuePair<string, string>("sendChavesPrimarias[" + TCampos + "][0][0]", i.Key));
                ConjuntoKey.Add(new KeyValuePair<string, string>("sendChavesPrimarias[" + TCampos + "][0][1]", i.Value));
                TCampos++;
            }
            if (ConjuntoKey.Count > 0)
            {
                EnviarParametros.AddRange(ConjuntoKey);
            }

        }
        /**
         * Excluir dados da tabela através do envio das chaves via REST.
         */
        public async Task<Boolean> ExcluirDadosTabela()
        {
            try
            {
                if (Status) { throw new Exception("Já existe um pedido sendo realizado, favor aguardar..."); }

                if (NomeTabela == null) { throw new Exception("Não foi definido nenhum nome de tabela."); }

                if (ConjuntoKey.Count == 0) { throw new Exception("Não há dados para serem enviados."); }

                /**
                 * Armazena o nome da tabela que será utilizada para a busca, inserção, delete e update dos dados.
                 */
                KeyValuePair<string, string> ChaveDaSessao = new KeyValuePair<string, string>("enviarChaves", Chave_Sessao);
                KeyValuePair<string, string> Tabelas = new KeyValuePair<string, string>("sendTabelas", NomeTabela);
                KeyValuePair<string, string> ModoOperacao = new KeyValuePair<string, string>("sendModoOperacao", Delete);
                KeyValuePair<string, string> Dispositivo = new KeyValuePair<string, string>("sendDispositivo", "Movel");


                EnviarParametros.Add(ChaveDaSessao);
                EnviarParametros.Add(Tabelas);
                EnviarParametros.Add(ModoOperacao);
                EnviarParametros.Add(Dispositivo);

                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametros);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ConjuntoKey.Clear();
                EnviarParametros.Clear(); //Limpa todos os filtros.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                    {
                        Modo = ResultSETJSON["Modo"].Value<string>(),
                        Codigo = ResultSETJSON["Codigo"].Value<int>(),
                        Error = ResultSETJSON["Error"].Value<Boolean>(),
                        Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                        File = ResultSETJSON["File"].Value<string>(),
                        Tracer = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    is_error = false;
                    PacoteEDados = new PacoteExcluirDados
                    {
                        Modo = ResultSETJSON.Property("Modo"),
                        Error = ResultSETJSON.Property("Error"),
                        TempoTotal = ResultSETJSON.Property("TempoTotal")

                    };

                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/InserirDadosTabela()",
                    Tracer = ex.StackTrace
                };
                ResultSETJSON = null;
                return false;
            }
        }
        /**
         * Cria um conjunto de chaves para uso de atualização, exclusão.
         */
        public void setKeyDadosAtualizar(List<KeyValuePair<string, string>> Lst)
        {
            int TCampos = ConjuntoKey.Count;
            foreach (KeyValuePair<string, string> i in Lst)
            {
                ConjuntoKey.Add(new KeyValuePair<string, string>("sendChavesPrimarias[" + TCampos + "][0]", i.Key));
                ConjuntoKey.Add(new KeyValuePair<string, string>("sendChavesPrimarias[" + TCampos + "][1]", i.Value));
                TCampos++;
            }
            if (ConjuntoKey.Count > 0)
            {
                EnviarParametros.AddRange(ConjuntoKey);
            }

        }
        /**
         * Cria um conjunto de dados para atualização.
         */
        public void setDadosAtualizar(List<KeyValuePair<string, string>> Lst)
        {
            int TCampos = ConjuntoDadosAtualizar.Count;
            foreach (KeyValuePair<string, string> i in Lst)
            {
                ConjuntoDadosAtualizar.Add(new KeyValuePair<string, string>("sendCamposAndValores[" + TCampos + "][name]", i.Key));
                ConjuntoDadosAtualizar.Add(new KeyValuePair<string, string>("sendCamposAndValores[" + TCampos + "][value]", i.Value));
                TCampos++;
            }
            if (ConjuntoDadosAtualizar.Count > 0)
            {
                EnviarParametros.AddRange(ConjuntoDadosAtualizar);
            }

        }
        /**
         * Atualiza os dados da tabela através do envio das chaves e valores via REST.
         */
        public async Task<Boolean> AtualizarDadosTabela()
        {
            try
            {
                if (Status) { throw new Exception("Já existe um pedido sendo realizado, favor aguardar..."); }

                if (NomeTabela == null) { throw new Exception("Não foi definido nenhum nome de tabela."); }

                if (ConjuntoDadosAtualizar.Count == 0) { throw new Exception("Não há dados para serem enviados."); }
                if (ConjuntoKey.Count == 0) { throw new Exception("Não há chaves para serem enviados."); }

                /**
                 * Armazena o nome da tabela que será utilizada para a busca, inserção, delete e update dos dados.
                 */
                KeyValuePair<string, string> ChaveDaSessao = new KeyValuePair<string, string>("enviarChaves", Chave_Sessao);
                KeyValuePair<string, string> Tabelas = new KeyValuePair<string, string>("sendTabelas", NomeTabela);
                KeyValuePair<string, string> ModoOperacao = new KeyValuePair<string, string>("sendModoOperacao", Update);
                KeyValuePair<string, string> Dispositivo = new KeyValuePair<string, string>("sendDispositivo", "Movel");


                EnviarParametros.Add(ChaveDaSessao);
                EnviarParametros.Add(Tabelas);
                EnviarParametros.Add(ModoOperacao);
                EnviarParametros.Add(Dispositivo);

                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametros);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ConjuntoDadosAtualizar.Clear();
                EnviarParametros.Clear(); //Limpa todos os filtros.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                    {
                        Modo        = ResultSETJSON["Modo"].Value<string>(),
                        Codigo      = ResultSETJSON["Codigo"].Value<int>(),
                        Error       = ResultSETJSON["Error"].Value<Boolean>(),
                        Mensagem    = ResultSETJSON["Mensagem"].Value<string>(),
                        File        = ResultSETJSON["File"].Value<string>(),
                        Tracer      = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    is_error = false;
                    PacoteADados = new PacoteAtualizarDados
                    {
                        Modo        = ResultSETJSON.Property("Modo"),
                        Error       = ResultSETJSON.Property("Error"),
                        TempoTotal  = ResultSETJSON.Property("TempoTotal")

                    };

                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/InserirDadosTabela()",
                    Tracer = ex.StackTrace
                };
                ResultSETJSON = null;
                return false;
            }
        }
        /**
         * Atualiza as informações da última busca de dados no servidor.
         */
        public async Task<Boolean> RefreshSelectDados()
        {

            try
            {
                if (Status) { throw new Exception("Já existe um pedido sendo realizado, favor aguardar..."); }

                if (EnviarParametrosSelect.Count == 0) { throw new Exception("A instrução select não foi utilizada ainda."); }

                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametrosSelect);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                    {
                        Modo = ResultSETJSON["Modo"].Value<string>(),
                        Codigo = ResultSETJSON["Codigo"].Value<int>(),
                        Error = ResultSETJSON["Error"].Value<Boolean>(),
                        Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                        File = ResultSETJSON["File"].Value<string>(),
                        Tracer = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    is_error = false;
                    PacoteTBL = new PacoteSelecionarDados
                    {
                        Modo = ResultSETJSON.Property("Modo"),
                        Error = ResultSETJSON.Property("Error"),
                        NomeTabela = ResultSETJSON.Property("NomeTabela"),
                        ResultadoDados = ResultSETJSON.Property("ResultDados"),
                        Campos = ResultSETJSON.Property("Campos"),
                        ChavesPrimarias = ResultSETJSON.Property("ChavesPrimarias"),
                        Paginacao = ResultSETJSON.Property("Paginacao"),
                        InfoPaginacao = ResultSETJSON.Property("InfoPaginacao"),
                        Botoes = ResultSETJSON.Property("Botoes"),
                        ContadorLinha = ResultSETJSON.Property("ContadorLinha"),
                        OrdemBy = ResultSETJSON.Property("OrdemBy"),
                        Filtros = ResultSETJSON.Property("Filtros"),
                        ShowColumnsIcones = ResultSETJSON.Property("ShowColumnsIcones"),
                        Formato = ResultSETJSON.Property("Formato"),
                        Indexador = ResultSETJSON.Property("Indexador"),
                        TempoTotal = ResultSETJSON.Property("TempoTotal")
                    };

                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/SelectTabelaJson",
                    Tracer = ex.StackTrace
                };
                return false;
            }
        }

        public async Task<Boolean> TrocarPagina(int Pagina = 0)
        {

            try
            {
                if (Status) { throw new Exception("Já existe um pedido sendo realizado, favor aguardar..."); }

                if (EnviarParametrosSelect.Count == 0) { throw new Exception("A instrução select não foi utilizada ainda."); }

                Predicate<KeyValuePair<string, string>> FindExcluir = ExcluirChave;

                EnviarParametrosSelect.RemoveAll(FindExcluir);
                EnviarParametrosSelect.Add(new KeyValuePair<string, string>("sendPagina", Convert.ToString(Pagina)));
                FormUrlEncodedContent PacoteEnvio = new FormUrlEncodedContent(EnviarParametrosSelect);

                TokenCancel = new CancellationToken();

                Status = true; //Bloqueia as outras operações que envolvem busca de dados no servidor.
                ChamadaREST = await PostAsync(Dados_Endereco, PacoteEnvio, TokenCancel);
                ResultSET = await ChamadaREST.Content.ReadAsStringAsync();
                Status = false; //Desbloqueia o sistema para outras busca no servidor.

                ResultSETJSON = JObject.Parse(ResultSET);
                ResultSET = null;

                Boolean Errors = ResultSETJSON["Error"].Value<Boolean>();
                if (Errors)
                {
                    is_error = true;
                    InfoError = new InforError
                    {
                        Modo = ResultSETJSON["Modo"].Value<string>(),
                        Codigo = ResultSETJSON["Codigo"].Value<int>(),
                        Error = ResultSETJSON["Error"].Value<Boolean>(),
                        Mensagem = ResultSETJSON["Mensagem"].Value<string>(),
                        File = ResultSETJSON["File"].Value<string>(),
                        Tracer = ResultSETJSON["Tracer"].Value<string>()
                    };
                    ResultSETJSON = null;
                    return false;
                }
                else
                {
                    is_error = false;
                    PacoteTBL = new PacoteSelecionarDados
                    {
                        Modo = ResultSETJSON.Property("Modo"),
                        Error = ResultSETJSON.Property("Error"),
                        NomeTabela = ResultSETJSON.Property("NomeTabela"),
                        ResultadoDados = ResultSETJSON.Property("ResultDados"),
                        Campos = ResultSETJSON.Property("Campos"),
                        ChavesPrimarias = ResultSETJSON.Property("ChavesPrimarias"),
                        Paginacao = ResultSETJSON.Property("Paginacao"),
                        InfoPaginacao = ResultSETJSON.Property("InfoPaginacao"),
                        Botoes = ResultSETJSON.Property("Botoes"),
                        ContadorLinha = ResultSETJSON.Property("ContadorLinha"),
                        OrdemBy = ResultSETJSON.Property("OrdemBy"),
                        Filtros = ResultSETJSON.Property("Filtros"),
                        ShowColumnsIcones = ResultSETJSON.Property("ShowColumnsIcones"),
                        Formato = ResultSETJSON.Property("Formato"),
                        Indexador = ResultSETJSON.Property("Indexador"),
                        TempoTotal = ResultSETJSON.Property("TempoTotal")
                    };

                    ResultSETJSON = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                is_error = true;
                InfoError = new InforError
                {
                    Modo = "Local",
                    Codigo = 1,
                    Error = true,
                    Mensagem = ex.Message,
                    File = "CamadaDados.cs/SelectTabelaJson",
                    Tracer = ex.StackTrace
                };
                return false;
            }
        }

        private bool ExcluirChave(KeyValuePair<string, string> i)
        {
            Regex Regx = new Regex(@"\bsendPagina\b");
            return Regx.IsMatch(i.Key);
        }

        public PacoteSelecionarDados getDados()
        {
            return PacoteTBL;
        }
    }
}
