using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace Roleplay
{
    public class Server : AsyncResource
    {
        Timer TimerSegundo { get; set; }
        Timer TimerSalvar { get; set; }
        BackgroundWorker BackgroundWorkerSegundo { get; set; }

        public override void OnStart()
        {
            Alt.OnPlayerConnect += OnPlayerConnect;
            Alt.OnPlayerDisconnect += OnPlayerDisconnect;
            Alt.OnPlayerDead += OnPlayerDead;
            Alt.OnWeaponDamage += OnWeaponDamage;
            Alt.OnPlayerDamage += OnPlayerDamage;
            Alt.OnPlayerEnterVehicle += OnPlayerEnterVehicle;
            Alt.OnPlayerLeaveVehicle += OnPlayerLeaveVehicle;
            Alt.OnClient<IPlayer, string>("OnPlayerChat", OnPlayerChat);
            Alt.OnClient<IPlayer, string, string, string, string>("RegistrarUsuario", RegistrarUsuario);
            Alt.OnClient<IPlayer, string, string>("EntrarUsuario", EntrarUsuario);
            Alt.OnClient<IPlayer>("ListarPersonagens", ListarPersonagens);
            Alt.OnClient<IPlayer, int, string, string, string, string, string>("CriarPersonagem", CriarPersonagem);
            Alt.OnClient<IPlayer, int, bool>("SelecionarPersonagem", SelecionarPersonagem);
            Alt.OnClient<IPlayer>("ListarPlayers", ListarPlayers);
            Alt.OnClient<IPlayer, int, string, int, int, int, int, int, int>("ComprarVeiculo", ComprarVeiculo);
            Alt.OnClient<IPlayer, string>("ComprarConveniencia", ComprarConveniencia);
            Alt.OnClient<IPlayer, string, int>("AdicionarContatoCelular", AdicionarContatoCelular);
            Alt.OnClient<IPlayer, int>("RemoverContatoCelular", RemoverContatoCelular);
            Alt.OnClient<IPlayer, int>("PagarMulta", PagarMulta);
            Alt.OnClient<IPlayer, int, uint>("PegarItemArmario", PegarItemArmario);
            Alt.OnClient<IPlayer, int, string, int>("EntregarArma", EntregarArma);
            Alt.OnClient<IPlayer, string, string, string>("AtualizarInformacoes", AtualizarInformacoes);
            Alt.OnClient<IPlayer, IVehicle, string, object>("SetVehicleMeta", SetVehicleMeta);
            Alt.OnClient<IPlayer>("DevolverItensArmario", DevolverItensArmario);
            Alt.OnClient<IPlayer, int, int>("SpawnarVeiculoFaccao", SpawnarVeiculoFaccao);
            Alt.OnClient<IPlayer, string, string, int, int, bool>("ConfirmarLojaRoupas", ConfirmarLojaRoupas);
            Alt.OnClient<IPlayer, string>("EnviarEmailConfirmacao", EnviarEmailConfirmacao);
            Alt.OnClient<IPlayer, string>("ValidarTokenConfirmacao", ValidarTokenConfirmacao);
            Alt.OnClient<IPlayer>("ExibirPerguntas", ExibirPerguntas);
            Alt.OnClient<IPlayer, string>("ValidarPerguntas", ValidarPerguntas);
            Alt.OnClient<IPlayer, string, string>("EnviarEmailAlterarSenha", EnviarEmailAlterarSenha);
            Alt.OnClient<IPlayer, string, int, bool>("ConfirmarPersonalizacao", ConfirmarPersonalizacao);
            Alt.OnClient<IPlayer, int>("DeletarPersonagem", DeletarPersonagem);
            Alt.OnClient<IPlayer, bool>("Chatting", Chatting);
            Alt.OnClient<IPlayer>("EquiparColeteArmario", EquiparColeteArmario);
            Alt.OnClient<IPlayer, uint, uint>("PegarComponenteArmario", PegarComponenteArmario);
            Alt.OnClient<IPlayer, int>("LigarContatoCelular", LigarContatoCelular);
            Alt.OnClient<IPlayer, int>("EnviarLocalizacaoContatoCelular", EnviarLocalizacaoContatoCelular);
            Alt.OnClient<IPlayer, int>("AbastecerVeiculo", AbastecerVeiculo);
            Alt.OnClient<IPlayer>("OnPlayerConnectLogin", OnPlayerConnectLogin);
            Alt.OnClient<IPlayer>("PunicoesAdministrativas", PunicoesAdministrativas);
            Alt.OnClient<IPlayer, string>("AlterarEmail", AlterarEmail);
            Alt.OnClient<IPlayer, string, string, string>("AlterarSenha", AlterarSenha);
            Alt.OnClient<IPlayer, int, int, int, bool>("UsarATM", UsarATM);
            Alt.OnClient<IPlayer, int, int, int, int, int, int, int, int>("PintarVeiculo", PintarVeiculo);
            Alt.OnClient<IPlayer, int>("SpawnarVeiculo", SpawnarVeiculo);
            Alt.OnClient<IPlayer>("VenderVeiculo", VenderVeiculo);
            Alt.OnClient<IPlayer, float, float, float>("SoltarSacoLixo", SoltarSacoLixo);
            Alt.OnClient<IPlayer, string>("MDCPesquisarPessoa", MDCPesquisarPessoa);
            Alt.OnClient<IPlayer, string>("MDCPesquisarVeiculo", MDCPesquisarVeiculo);
            Alt.OnClient<IPlayer, string>("MDCPesquisarPropriedade", MDCPesquisarPropriedade);
            Alt.OnClient<IPlayer, int>("MDCRastrearVeiculo", MDCRastrearVeiculo);
            Alt.OnClient<IPlayer, int, int, string, string>("MDCAdicionarBOLO", MDCAdicionarBOLO);
            Alt.OnClient<IPlayer, int, string>("MDCRemoverBOLO", MDCRemoverBOLO);
            Alt.OnClient<IPlayer, int>("MDCRastrear911", MDCRastrear911);
            Alt.OnClient<IPlayer, int, string, int, string, string>("MDCMultar", MDCMultar);
            Alt.OnClient<IPlayer, int>("MDCRevogarLicencaMotorista", MDCRevogarLicencaMotorista);

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture =
                  CultureInfo.GetCultureInfo("pt-BR");

            var config = JsonConvert.DeserializeObject<Configuracao>(File.ReadAllText("settings.json"));
            Global.MaxPlayers = config.MaxPlayers;
            Global.ConnectionString = $"Server={config.DBHost};Database={config.DBName};Uid={config.DBUser};Password={config.DBPassword}";
            Global.VehicleInfos = JsonConvert.DeserializeObject<List<VehicleInfo>>(File.ReadAllText("vehicles.json"));
            Global.Development = config.Development;
            Global.TokenBot = config.TokenBot;
            Global.CanalAnuncios = config.CanalAnuncios;
            Global.CanalAnunciosGovernamentais = config.CanalAnunciosGovernamentais;
            Global.EmailHost = config.EmailHost;
            Global.EmailAddress = config.EmailAddress;
            Global.EmailName = config.EmailName;
            Global.EmailPassword = config.EmailPassword;
            Global.EmailPort = config.EmailPort;

            using (var context = new DatabaseContext())
            {
                context.Database.ExecuteSqlRaw("UPDATE Personagens SET Online=0");
                Console.WriteLine("Status online dos personagens limpo");

                Global.Parametros = context.Parametros.FirstOrDefault();
                Console.WriteLine("Parametros carregados");

                Global.Blips = context.Blips.ToList();
                Console.WriteLine($"Blips: {Global.Blips.Count}");

                Global.Faccoes = context.Faccoes.ToList();
                Console.WriteLine($"Faccoes: {Global.Faccoes.Count}");

                Global.Ranks = context.Ranks.ToList();
                Console.WriteLine($"Ranks: {Global.Ranks.Count}");

                Global.Propriedades = context.Propriedades.ToList();
                foreach (var x in Global.Propriedades)
                    x.CriarIdentificador();
                Console.WriteLine($"Propriedades: {Global.Propriedades.Count}");

                Global.Precos = context.Precos.ToList();
                Console.WriteLine($"Precos: {Global.Precos.Count}");

                Global.Pontos = context.Pontos.ToList();
                foreach (var x in Global.Pontos)
                    x.CriarIdentificador();
                Console.WriteLine($"Pontos: {Global.Pontos.Count}");

                Global.Armarios = context.Armarios.ToList();
                foreach (var x in Global.Armarios)
                    x.CriarIdentificador();
                Console.WriteLine($"Armarios: {Global.Armarios.Count}");

                Global.ArmariosItens = context.ArmariosItens.ToList();
                Console.WriteLine($"ArmariosItens: {Global.ArmariosItens.Count}");

                context.Database.ExecuteSqlRaw("UPDATE SOSs SET DataResposta = now(), TipoResposta = 3 WHERE DataResposta is null");
                Console.WriteLine("SOSs limpos");

                Global.Perguntas = context.Perguntas.ToList();
                Console.WriteLine($"Perguntas: {Global.Perguntas.Count}");

                Global.Respostas = context.Respostas.ToList();
                Console.WriteLine($"Respostas: {Global.Respostas.Count}");

                Global.ArmariosComponentes = context.ArmariosComponentes.ToList();
                Console.WriteLine($"ArmariosComponentes: {Global.ArmariosComponentes.Count}");
            }

            foreach (var c in Global.Concessionarias)
                Functions.CriarTextDraw($"{c.Nome}\n~w~Use /comprar", c.PosicaoCompra, 10, 0.4f, 4, Global.RgbaPrincipal, 0);
            Console.WriteLine($"Concessionarias: {Global.Concessionarias.Count}");

            foreach (var c in Global.Empregos)
            {
                var nome = Functions.ObterDisplayEnum(c.Tipo);
                Functions.CriarTextDraw($"Emprego de {nome}\n~w~Use /emprego para se tornar um {nome.ToLower()}", c.Posicao, 10, 0.4f, 4, Global.RgbaPrincipal, 0);
                Functions.CriarTextDraw($"Aluguel de Veículos {nome}\n~w~Use /valugar ou /vestacionar", c.PosicaoAluguel, 10, 0.4f, 4, Global.RgbaPrincipal, 0);
            }
            Console.WriteLine($"Empregos: {Global.Empregos.Count}");

            Functions.CriarTextDraw("Prisão\n~w~Use /prender", Global.PosicaoPrisao, 10, 0.4f, 4, Global.RgbaPrincipal, 0);

            Global.VoiceChannel = Alt.CreateVoiceChannel(true, 20);

            TimerSegundo = new Timer(1000);
            TimerSegundo.Elapsed += TimerSegundo_Elapsed;
            TimerSegundo.Start();

            TimerSalvar = new Timer(60000);
            TimerSalvar.Elapsed += TimerSalvar_Elapsed;
            TimerSalvar.Start();

            BackgroundWorkerSegundo = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            BackgroundWorkerSegundo.DoWork += BackgroundWorkerSegundo_DoWork;

            if (!string.IsNullOrWhiteSpace(Global.TokenBot))
                MainAsync();
        }

        public async void MainAsync()
        {
            using var services = ConfigureServices();
            Global.DiscordClient = services.GetRequiredService<DiscordSocketClient>();

            Global.DiscordClient.Log += LogAsync;

            await Global.DiscordClient.LoginAsync(TokenType.Bot, Global.TokenBot);
            await Global.DiscordClient.StartAsync();

            await Task.Delay(System.Threading.Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }

        private void BackgroundWorkerSegundo_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var p in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
            {
                p.Player.SetDateTime(DateTime.Now);

                var dif = DateTime.Now - p.DataUltimaVerificacao;
                if (dif.TotalMinutes >= 1)
                {
                    p.DataUltimaVerificacao = DateTime.Now;
                    p.TempoConectado++;

                    if (p.EmTrabalhoAdministrativo)
                        p.UsuarioBD.TempoTrabalhoAdministrativo++;

                    if (p.TempoConectado % 60 == 0)
                    {
                        var porcentagemImpostoPropriedade = 0.0015M;
                        var porcentagemImpostoVeiculo = 0.001M;
                        var porcentagemPoupanca = 0.001M;

                        if ((p.UsuarioBD.DataExpiracaoVIP ?? DateTime.MinValue) >= DateTime.Now)
                        {
                            switch (p.UsuarioBD.VIP)
                            {
                                case TipoVIP.Bronze:
                                    porcentagemImpostoPropriedade = 0.0013M;
                                    porcentagemImpostoVeiculo = 0.0007M;
                                    porcentagemPoupanca = 0.00125M;
                                    break;
                                case TipoVIP.Prata:
                                    porcentagemImpostoPropriedade = 0.001M;
                                    porcentagemImpostoVeiculo = 0.0005M;
                                    porcentagemPoupanca = 0.00150M;
                                    break;
                                case TipoVIP.Ouro:
                                    porcentagemImpostoPropriedade = 0.0008M;
                                    porcentagemImpostoVeiculo = 0.0003M;
                                    porcentagemPoupanca = 0.00175M;
                                    break;
                            }
                        }

                        var salario = 0;

                        var valorImpostoPropriedade = 0;
                        var valorImpostoVeiculo = 0;

                        using var context = new DatabaseContext();
                        var veiculos = context.Veiculos.AsQueryable().Where(x => x.Personagem == p.Codigo && !x.VendidoFerroVelho).ToList();
                        if (p.Propriedades.Count > 0 || veiculos.Count > 0)
                        {
                            foreach (var x in p.Propriedades)
                                valorImpostoPropriedade += Convert.ToInt32(Convert.ToDecimal(x.Valor) * porcentagemImpostoPropriedade);

                            foreach (var x in veiculos)
                                valorImpostoVeiculo += Convert.ToInt32(Convert.ToDecimal(Global.Precos.FirstOrDefault(y => y.Veiculo && y.Nome.ToLower() == x.Modelo.ToLower())?.Valor ?? 0) * porcentagemImpostoVeiculo);
                        }

                        var poupanca = 0;
                        var salarioEmprego = 0;
                        if (p.Faccao > 0)
                        {
                            salario += p.RankBD.Salario;
                        }
                        else if (p.Emprego > 0)
                        {
                            salarioEmprego = Global.Precos.FirstOrDefault(x => x.Tipo == TipoPreco.Empregos && x.Nome.ToLower() == p.Emprego.ToString().ToLower())?.Valor ?? 0;
                            salario += salarioEmprego;
                            salario += p.PagamentoExtra;
                        }

                        if (p.Poupanca > 0)
                        {
                            poupanca = Convert.ToInt32(Convert.ToDecimal(p.Poupanca) * porcentagemPoupanca);
                            p.Poupanca += poupanca;
                            if (p.Poupanca > 1000000)
                            {
                                p.Banco += p.Poupanca - 50000;
                                p.Poupanca = 50000;
                            }
                        }

                        salario *= Global.Parametros.Paycheck;

                        salario -= valorImpostoPropriedade;
                        salario -= valorImpostoVeiculo;

                        p.Banco += salario;
                        if (salario != 0)
                        {
                            Functions.EnviarMensagem(p.Player, TipoMensagem.Titulo, $"Pagamento de {p.Nome} {(Global.Parametros.Paycheck > 1 ? $"(PAYCHECK {Global.Parametros.Paycheck}x)" : string.Empty)}");

                            if (p.Faccao > 0 && p.RankBD.Salario > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Salário {p.FaccaoBD.Nome}: {{{Global.CorSucesso}}}+ ${p.RankBD.Salario:N0}");

                            if (salarioEmprego > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Salário Emprego: {{{Global.CorSucesso}}}+ ${salarioEmprego:N0}");

                            if (p.PagamentoExtra > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Extra Emprego: {{{Global.CorSucesso}}}+ ${p.PagamentoExtra:N0}");

                            if (valorImpostoPropriedade > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Imposto Propriedades: {{{Global.CorErro}}}- ${valorImpostoPropriedade:N0}");

                            if (valorImpostoVeiculo > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Imposto Veículos: {{{Global.CorErro}}}- ${valorImpostoVeiculo:N0}");

                            Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Total: {{{(salario >= 0 ? Global.CorSucesso : Global.CorErro)}}}${salario:N0}");

                            if (poupanca > 0)
                                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Poupança: {{{Global.CorSucesso}}}+ ${poupanca:N0} (${p.Poupanca:N0})");
                        }

                        p.PagamentoExtra = 0;
                    }
                }
            }

            foreach (var x in Global.Veiculos)
            {
                var dif = DateTime.Now - x.DataUltimaVerificacao;
                if (dif.TotalMinutes >= 1)
                {
                    x.DataUltimaVerificacao = DateTime.Now;

                    if (x.DataExpiracaoAluguel.HasValue)
                    {
                        if (x.DataExpiracaoAluguel.Value < DateTime.Now)
                        {
                            if (x.Vehicle.Driver != null)
                            {
                                x.Vehicle.Driver.Emit("vehicle:setVehicleEngineOn", x.Vehicle, false);
                                Functions.EnviarMensagem(x.Vehicle.Driver, TipoMensagem.Erro, "O aluguel do veículo expirou. Use /valugar para alugar novamente por uma hora.");
                            }

                            x.NomeEncarregado = string.Empty;
                            x.DataExpiracaoAluguel = null;
                        }
                    }

                    if (x.Vehicle.EngineOn && x.Combustivel > 0 && !Global.VeiculosSemCombustivel.Contains(x.Info?.Class ?? string.Empty))
                    {
                        x.Combustivel--;
                        x.Vehicle.SetSyncedMetaData("combustivel", x.CombustivelHUD);
                        if (x.Combustivel == 0)
                        {
                            if (x.Vehicle.Driver != null)
                                x.Vehicle.Driver.Emit("vehicle:setVehicleEngineOn", x.Vehicle, false);
                        }
                    }
                }
            }
        }

        private void OnPlayerLeaveVehicle(IVehicle vehicle, IPlayer player, byte seat)
        {
            var p = Functions.ObterPersonagem(player);
            p.StopAnimation();

            if (vehicle.Model == (uint)VehicleModel.Thruster)
            {
                AltAsync.Do(async () =>
                {
                    await Task.Delay(2000);
                    vehicle.Remove();
                });
            }
        }

        private void OnPlayerEnterVehicle(IVehicle vehicle, IPlayer player, byte seat)
        {
            var p = Functions.ObterPersonagem(player);
            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == vehicle);
            if (veh == null)
                return;

            if (vehicle.EngineOn)
            {
                if (veh.Combustivel == 0)
                {
                    player.Emit("vehicle:setVehicleEngineOn", vehicle, false);
                }
                else if (veh.Emprego != TipoEmprego.Nenhum && !veh.DataExpiracaoAluguel.HasValue)
                {
                    player.Emit("vehicle:setVehicleEngineOn", vehicle, false);
                    if (player.Seat == 0)
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "O aluguel do veículo expirou. Use /valugar para alugar novamente por uma hora.");
                }
            }
            else
            {
                if (veh.Info?.Class == "CYCLE")
                    player.Emit("vehicle:setVehicleEngineOn", vehicle, true);
            }

            if (veh.Emprego != TipoEmprego.Nenhum && veh.NomeEncarregado == p.Nome && veh.DataExpiracaoAluguel.HasValue)
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"O aluguel do veículo irá expirar em {veh.DataExpiracaoAluguel}.");
        }

        public override void OnStop()
        {
            TimerSegundo?.Stop();
            TimerSalvar?.Stop();
            BackgroundWorkerSegundo.CancelAsync();
            foreach (var p in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
                Functions.SalvarPersonagem(p);
        }

        private void OnPlayerConnect(IPlayer player, string reason)
        {
            player.SetDateTime(DateTime.Now);
            player.SetWeather(Global.Parametros.Weather);
            player.Spawn(new Position(-430.0717f, 1039.26f, 372.1287f));

            using var context = new DatabaseContext();

            if (!Functions.VerificarBanimento(player, context.Banimentos.FirstOrDefault(x => x.SocialClub == (long)player.SocialClubId
                && x.HardwareIdHash == (long)player.HardwareIdHash
                && x.HardwareIdExHash == (long)player.HardwareIdExHash)))
                return;

            OnPlayerConnectLogin(player);
        }

        private void OnPlayerConnectLogin(IPlayer player)
        {
            // começar a usar localstorage ao invés desse context
            using var context = new DatabaseContext();
            player.Emit("Server:Login", context.Usuarios.FirstOrDefault(x => x.SocialClubRegistro == (long)player.SocialClubId
                && x.HardwareIdHashRegistro == (long)player.HardwareIdHash
                && x.HardwareIdExHashRegistro == (long)player.HardwareIdExHash)?.Nome ?? string.Empty);
        }

        private void OnPlayerDisconnect(IPlayer player, string reason)
        {
            if (Global.VoiceChannel.HasPlayer(player))
                Global.VoiceChannel.RemovePlayer(player);

            var p = Functions.ObterPersonagem(player);
            if (p?.Codigo > 0)
            {
                foreach (var x in Global.PersonagensOnline.Where(x => x.Player.Dimension == player.Dimension && player.Position.Distance(x.Player.Position) <= 20))
                    Functions.EnviarMensagem(x.Player, TipoMensagem.Erro, $"(( {p.NomeIC} [{p.ID}] saiu do servidor. ))");

                if (p.PosicaoSpec.HasValue)
                    p.Unspectate();

                foreach (var x in Global.PersonagensOnline.Where(x => x.IDSpec == p.ID))
                    x.Unspectate();

                Functions.GravarLog(TipoLog.Saida, reason, p, null);
                Functions.SalvarPersonagem(p, false);
            }

            Global.PersonagensOnline.RemoveAll(x => x.Player == player);
        }

        private void OnPlayerDead(IPlayer player, IEntity killer, uint weapon)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Codigo ?? 0) == 0)
                return;

            Functions.GravarLog(TipoLog.Morte, JsonConvert.SerializeObject(p.Ferimentos), p,
                killer is IPlayer playerKiller ? Functions.ObterPersonagem(playerKiller) : null);

            p.SetarFerido(true);
        }

        private void OnPlayerChat(IPlayer player, string message)
        {
            // se contem <script>, travar
            if (message[0] != '/')
            {
                Functions.EnviarMensagemChat(Functions.ObterPersonagem(player), message, TipoMensagemJogo.ChatICNormal);
                return;
            }

            try
            {
                var split = message.Split(" ");
                var cmd = split[0].Replace("/", string.Empty).Trim().ToLower();
                var method = Assembly.GetExecutingAssembly().GetTypes()
                    .SelectMany(x => x.GetMethods())
                    .Where(x => x.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0
                    && (x.GetCustomAttribute<CommandAttribute>().Command.ToLower() == cmd
                        || x.GetCustomAttribute<CommandAttribute>().Alias.ToLower() == cmd))
                    .FirstOrDefault();
                if (method == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"O comando {{{Global.CorPrincipal}}}{message}{{#FFFFFF}} não existe. Digite {{{Global.CorPrincipal}}}/ajuda{{#FFFFFF}} para visualizar os comandos disponíveis.");
                    return;
                }

                var methodParams = method.GetParameters();
                var obj = Activator.CreateInstance(method.DeclaringType);
                var command = method.GetCustomAttribute<CommandAttribute>();

                var arr = new List<object>();

                var list = methodParams.ToList();
                foreach (var x in list)
                {
                    var index = list.IndexOf(x);
                    if (index == 0)
                    {
                        arr.Add(player);
                    }
                    else
                    {
                        if (split.Length <= index)
                            continue;

                        var p = split[index];

                        if (x.ParameterType == typeof(int))
                        {
                            int.TryParse(p, out int val);
                            if (val == 0 && p != "0")
                                continue;

                            arr.Add(val);
                        }
                        else if (x.ParameterType == typeof(string))
                        {
                            if (string.IsNullOrWhiteSpace(p))
                                continue;

                            if (command.GreedyArg && index + 1 == list.Count)
                                p = string.Join(" ", split.Skip(index).Take(split.Length - index));

                            arr.Add(p);
                        }
                        else if (x.ParameterType == typeof(float))
                        {
                            float.TryParse(p, out float val);
                            if (val == 0 && p != "0")
                                continue;

                            arr.Add(val);
                        }
                        else if (x.ParameterType == typeof(byte))
                        {
                            byte.TryParse(p, out byte val);
                            if (val == 0 && p != "0")
                                continue;

                            arr.Add(val);
                        }
                    }
                }

                if (methodParams.Length != arr.Count)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Os parâmetros do comando não foram informados corretamente. Use: {{{Global.CorPrincipal}}}{command.HelpText}");
                    return;
                }

                method.Invoke(obj, arr.ToArray());
            }
            catch (Exception ex)
            {
                Functions.RecuperarErro(ex);
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não foi possível interpretar o comando.");
            }
        }

        private bool OnWeaponDamage(IPlayer player, IEntity target, uint weapon, ushort damage, Position shotOffset, BodyPart bodyPart)
        {
            if (!(target is IPlayer playerTarget))
                return true;

            var p = Functions.ObterPersonagem(playerTarget);
            if (p == null)
                return false;

            if (p.TimerFerido != null)
            {
                if (p.TipoFerido == 1)
                {
                    Functions.EnviarMensagem(playerTarget, TipoMensagem.Erro, "Você perdeu a consciência.");
                    playerTarget.Emit("Server:ToggleFerido", 2);
                    playerTarget.SetSyncedMetaData("ferido", 2);
                    p.TipoFerido = 2;
                }
                return false;
            }

            if (!p.Algemado)
                p.StopAnimation();

            var attacker = Functions.ObterPersonagem(player);

            p.Ferimentos.Add(new Personagem.Ferimento()
            {
                Data = DateTime.Now,
                Arma = weapon,
                Dano = damage,
                BodyPart = (sbyte)bodyPart,
                Attacker = attacker != null ? $"{attacker.Codigo} - {attacker.Nome}" : string.Empty,
            });

            return true;
        }

        private void OnPlayerDamage(IPlayer player, IEntity attacker, uint weapon, ushort damage)
        {
            if (Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().Any(x => (uint)x == weapon))
                return;

            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.TimerFerido != null)
            {
                if (p.TipoFerido == 1)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você levou PK e perdeu a consciência.");
                    player.Emit("Server:ToggleFerido", 2);
                    player.SetSyncedMetaData("ferido", 2);
                    p.TipoFerido = 2;
                }
                return;
            }

            p.StopAnimation();

            var ferimento = new Personagem.Ferimento()
            {
                Data = DateTime.Now,
                Arma = weapon,
                Dano = damage,
            };

            Personagem pAttacker = null;
            if (attacker is IPlayer playerAttacker)
                pAttacker = Functions.ObterPersonagem(playerAttacker);
            else if (attacker is IVehicle vehicleAttacker)
                pAttacker = Functions.ObterPersonagem(vehicleAttacker.Driver);

            if (pAttacker != null)
                ferimento.Attacker = $"{pAttacker.Codigo} - {pAttacker.Nome}";

            p.Ferimentos.Add(ferimento);
        }

        private void TimerSegundo_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!BackgroundWorkerSegundo.IsBusy)
                BackgroundWorkerSegundo.RunWorkerAsync();
        }

        private void TimerSalvar_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var x in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
                Functions.SalvarPersonagem(x);
        }

        #region Server
        private void EntrarUsuario(IPlayer player, string usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
            {
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            var senhaCriptografada = Functions.Criptografar(senha);
            using var context = new DatabaseContext();
            var user = context.Usuarios.FirstOrDefault(x => x.Nome.ToLower() == usuario.ToLower() && x.Senha == senhaCriptografada);
            if (user == null)
            {
                Functions.GravarLog(TipoLog.LoginFalha, $"Usuário: {usuario}", null, null);
                player.Emit("Server:MostrarErro", "Usuário ou senha inválidos.");
                return;
            }

            if (!Functions.VerificarBanimento(player, context.Banimentos.FirstOrDefault(x => x.Usuario == user.Codigo)))
                return;

            if (Global.PersonagensOnline.Any(x => x?.UsuarioBD?.Nome == usuario))
            {
                player.Emit("Server:MostrarErro", "Usuário já está logado.");
                return;
            }

            user.DataUltimoAcesso = DateTime.Now;
            user.IPUltimoAcesso = Functions.ObterIP(player);
            user.SocialClubUltimoAcesso = (long)player.SocialClubId;
            user.HardwareIdHashUltimoAcesso = (long)player.HardwareIdHash;
            user.HardwareIdExHashUltimoAcesso = (long)player.HardwareIdExHash;
            context.Usuarios.Update(user);
            context.SaveChanges();

            Global.PersonagensOnline.Add(new Personagem()
            {
                UsuarioBD = user,
                Player = player,
            });

            if (!string.IsNullOrWhiteSpace(user.TokenConfirmacao))
            {
                player.Emit("Server:ConfirmacaoRegistro", user.Nome, user.Email);
                return;
            }

            ListarPersonagens(player);
        }

        private void ListarPersonagens(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);

            static int ObterSlots(Usuario u)
            {
                if ((u.DataExpiracaoVIP ?? DateTime.MinValue) < DateTime.Now)
                    return 2;

                return u.VIP switch
                {
                    TipoVIP.Bronze => 3,
                    TipoVIP.Prata => 4,
                    TipoVIP.Ouro => 5,
                    _ => 2,
                };
            }

            using var context = new DatabaseContext();
            player.Emit("Server:ListarPersonagens", p.UsuarioBD.Nome,
                JsonConvert.SerializeObject(context.Personagens.AsQueryable()
                    .Where(x => x.Usuario == p.UsuarioBD.Codigo && x.StatusNamechange != TipoStatusNamechange.Realizado && !x.DataExclusao.HasValue)
                    .ToList()
                    .Select(x => new
                    {
                        x.Codigo,
                        x.Nome,
                        Status = ObterStatusListarPersonagens(x),
                        Opcoes = ObterOpcoesListarPersonagens(x, p.UsuarioBD),
                    })),
                    ObterSlots(p.UsuarioBD));
        }

        private string ObterStatusListarPersonagens(Personagem x)
        {
            var span = $@"<span class=""label"" style=""background-color:{Global.CorSucesso};"">Vivo</span>";
            if (x.DataMorte.HasValue)
                span = $@"<span class=""label"" style=""background-color:{Global.CorErro};"">Morto ({x.MotivoMorte})</span>";
            else if ((x.DataTerminoPrisao ?? DateTime.MinValue) > DateTime.Now)
                span = $@"<span class=""label"" style=""background-color:{Global.CorErro};"">Preso até {x.DataTerminoPrisao}</span>";
            else if (!string.IsNullOrWhiteSpace(x.MotivoRejeicao))
                span = $@"<span class=""label"" style=""background-color:{Global.CorErro};"">Rejeitado</span>";
            else if (x.UsuarioStaffAvaliador == 0)
                span = $@"<span class=""label"" style=""background-color:#f0972b;"">Aguardando Avaliação</span>";
            return span;
        }

        private string ObterOpcoesListarPersonagens(Personagem x, Usuario u)
        {
            var opcoes = string.Empty;
            if (!x.DataMorte.HasValue && x.UsuarioStaffAvaliador != 0 && (x.DataTerminoPrisao ?? DateTime.MinValue) < DateTime.Now)
            {
                if (string.IsNullOrWhiteSpace(x.MotivoRejeicao))
                    opcoes = $"<button id='btn-selecionarpersonagem{x.Codigo}' class='btn btn-primary' onclick='selecionarPersonagem({x.Codigo}, false);'>LOGAR</button>";
                else
                    opcoes = $"<button id='btn-selecionarpersonagem{x.Codigo}' class='btn btn-dark' onclick='selecionarPersonagem({x.Codigo}, false);'>REFAZER APLICAÇÃO</button>";
            }
            opcoes += x.StatusNamechange == TipoStatusNamechange.Liberado && u.PossuiNamechange && string.IsNullOrWhiteSpace(x.MotivoRejeicao) && x.UsuarioStaffAvaliador != 0 ? $" <button id='btn-selecionarpersonagem{x.Codigo}' class='btn btn-dark' onclick='selecionarPersonagem({x.Codigo}, true);'>ALTERAR NOME</button>" : string.Empty;
            opcoes += $" <button class='btn btn-danger' onclick='deletarPersonagem({x.Codigo});' style='background-color:#d12c0f;color:#fff;'>DELETAR</button>";
            return opcoes;
        }

        private void SelecionarPersonagem(IPlayer player, int id, bool namechange)
        {
            var p = Functions.ObterPersonagem(player);

            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == id && x.Usuario == p.UsuarioBD.Codigo);
            if (!string.IsNullOrWhiteSpace(personagem.MotivoRejeicao) || namechange)
            {
                var staffer = context.Usuarios.FirstOrDefault(x => x.Codigo == personagem.UsuarioStaffAvaliador);
                var nome = personagem.Nome.Split(' ');
                player.Emit("Server:CriarPersonagem", personagem.Codigo, nome.FirstOrDefault(), nome.LastOrDefault(), p.PersonalizacaoDados.sex == 1 ? "M" : "F", personagem.DataNascimento.ToShortDateString(), personagem.Historia, personagem.MotivoRejeicao, staffer.Nome);
                return;
            }

            personagem.IPUltimoAcesso = Functions.ObterIP(player);
            personagem.SocialClubUltimoAcesso = (long)player.SocialClubId;
            personagem.ID = Functions.ObterNovoID();
            personagem.Online = true;
            personagem.HardwareIdHashUltimoAcesso = (long)player.HardwareIdHash;
            personagem.HardwareIdExHashUltimoAcesso = (long)player.HardwareIdExHash;
            personagem.DataTerminoPrisao = null;
            context.Personagens.Update(personagem);
            context.SaveChanges();

            var user = p.UsuarioBD;
            var index = Global.PersonagensOnline.IndexOf(p);
            Global.PersonagensOnline[index] = personagem;
            Global.PersonagensOnline[index].Player = player;
            Global.PersonagensOnline[index].UsuarioBD = user;
            p = personagem;

            foreach (var x in Global.Blips)
                x.CriarIdentificador(player);

            foreach (var x in Global.TextDraws)
                x.CriarIdentificador(player);

            p.IPLs = JsonConvert.DeserializeObject<List<string>>(p.IPL);
            p.SetarIPLs();
            player.SetDateTime(DateTime.Now);
            player.Model = (uint)p.Skin;
            p.SetDinheiro();
            player.SetWeather(Global.Parametros.Weather);
            p.DataUltimaVerificacao = DateTime.Now;
            p.PersonalizacaoDados = JsonConvert.DeserializeObject<Personagem.Personalizacao>(p.InformacoesPersonalizacao);
            p.Contatos = JsonConvert.DeserializeObject<List<Personagem.Contato>>(p.InformacoesContatos);
            p.Roupas = JsonConvert.DeserializeObject<List<Personagem.Vestimenta>>(p.InformacoesRoupas);
            p.Acessorios = JsonConvert.DeserializeObject<List<Personagem.Vestimenta>>(p.InformacoesAcessorios);
            p.Ferimentos = JsonConvert.DeserializeObject<List<Personagem.Ferimento>>(p.InformacoesFerimentos);
            foreach (var x in JsonConvert.DeserializeObject<List<Personagem.Arma>>(p.InformacoesArmas))
                p.DarArma((WeaponModel)x.Codigo, x.Municao, x.Pintura, x.Componentes, false);

            if (Global.PersonagensOnline.Count(x => x.Codigo > 0) > Global.Parametros.RecordeOnline)
            {
                Global.Parametros.RecordeOnline = Global.PersonagensOnline.Count;
                context.Parametros.Update(Global.Parametros);
                context.SaveChanges();
                Functions.EnviarMensagemStaff($"O novo recorde de jogadores online é: {Global.Parametros.RecordeOnline}.", true);
            }

            Functions.GravarLog(TipoLog.Entrada, string.Empty, p, null);

            if (personagem.EtapaPersonalizacao != TipoEtapaPersonalizacao.Concluido)
            {
                player.Dimension = p.ID;
                p.SetPosition(new Position(402.84396f, -996.9758f, -99.01465f), false);
            }
            else
            {
                p.Spawnar();
            }

            if (!Global.VoiceChannel.HasPlayer(player))
                Global.VoiceChannel.AddPlayer(player);

            player.Emit("Server:SelecionarPersonagem", p.InformacoesPersonalizacao, p.InformacoesRoupas, p.InformacoesAcessorios, p.Roupa, (int)p.EtapaPersonalizacao);
        }

        private void RegistrarUsuario(IPlayer player, string usuario, string email, string senha, string senha2)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(senha2))
            {
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            if (usuario.Contains(" "))
            {
                player.Emit("Server:MostrarErro", "Usuário não pode ter espaços.");
                return;
            }

            if (usuario.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Usuário não pode ter mais que 25 caracteres.");
                return;
            }

            if (email.Length > 100)
            {
                player.Emit("Server:MostrarErro", "E-mail não pode ter mais que 100 caracteres.");
                return;
            }

            if (senha != senha2)
            {
                player.Emit("Server:MostrarErro", "Senhas não são iguais.");
                return;
            }

            if (!Functions.ValidarEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                if (context.Usuarios.Any(x => x.Nome.ToLower() == usuario.ToLower()))
                {
                    player.Emit("Server:MostrarErro", $"Usuário {usuario} já existe.");
                    return;
                }

                if (context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower()))
                {
                    player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                    return;
                }

                var user = new Usuario()
                {
                    Nome = usuario,
                    Email = email,
                    Senha = Functions.Criptografar(senha),
                    SocialClubRegistro = (long)player.SocialClubId,
                    SocialClubUltimoAcesso = (long)player.SocialClubId,
                    IPRegistro = Functions.ObterIP(player),
                    IPUltimoAcesso = Functions.ObterIP(player),
                    HardwareIdHashRegistro = (long)player.HardwareIdHash,
                    HardwareIdExHashRegistro = (long)player.HardwareIdExHash,
                    HardwareIdHashUltimoAcesso = (long)player.HardwareIdHash,
                    HardwareIdExHashUltimoAcesso = (long)player.HardwareIdExHash,
                    TokenConfirmacao = string.IsNullOrWhiteSpace(Global.EmailHost) ? string.Empty : new Random().Next(111111, 999999).ToString(),
                };
                context.Usuarios.Add(user);
                context.SaveChanges();

                Functions.EnviarEmail(user.Email, "Confirmação de E-mail", $"Seu token de confirmação é <strong>{user.TokenConfirmacao}</strong>.");
            }

            EntrarUsuario(player, usuario, senha);
        }

        private void CriarPersonagem(IPlayer player, int codigo, string nome, string sobrenome, string sexo, string dataNascimento, string historia)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null || p.ID > 0)
                return;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(sexo)
                || string.IsNullOrWhiteSpace(dataNascimento) || string.IsNullOrWhiteSpace(historia))
            {
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            historia = historia.Trim();
            if (historia.Length < 500)
            {
                player.Emit("Server:MostrarErro", $"História deve possuir mais que 500 caracteres ({historia.Length} de 500).");
                return;
            }

            var nomeCompleto = $"{nome.Trim()} {sobrenome.Trim()}";
            if (nomeCompleto.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Nome do personagem não pode possuir mais que 25 caracteres.");
                return;
            }

            DateTime.TryParse(dataNascimento, out DateTime dtNascimento);
            if (dtNascimento == DateTime.MinValue)
            {
                player.Emit("Server:MostrarErro", "Data de Nascimento não foi informada corretamente.");
                return;
            }

            var anos = (DateTime.Now.Date - dtNascimento).TotalDays / 365;
            if (anos < 18 || anos > 90)
            {
                player.Emit("Server:MostrarErro", "Personagem precisa ter entre 18 e 90 anos.");
                return;
            }

            Personagem personagemAntigo = null;
            using var context = new DatabaseContext();
            if (codigo > 0)
            {
                var per = context.Personagens.AsNoTracking().FirstOrDefault(x => x.Codigo == codigo);
                if (string.IsNullOrWhiteSpace(per.MotivoRejeicao))
                {
                    personagemAntigo = per;
                    codigo = 0;
                }
            }

            if (context.Personagens.Any(x => x.Nome.ToLower() == nomeCompleto.ToLower() && x.Codigo != codigo))
            {
                player.Emit("Server:MostrarErro", $"Personagem {nomeCompleto} já existe.");
                return;
            }

            p.Acessorios = new List<Personagem.Vestimenta>();
            p.Roupas = new List<Personagem.Vestimenta>();
            p.PersonalizacaoDados.sex = sexo == "M" ? 1 : 0;
            p.PersonalizacaoDados.structure = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            p.PersonalizacaoDados.opacityOverlays = new List<Personagem.Personalizacao.OpacityOverlay> { new Personagem.Personalizacao.OpacityOverlay(0), new Personagem.Personalizacao.OpacityOverlay(3), new Personagem.Personalizacao.OpacityOverlay(6), new Personagem.Personalizacao.OpacityOverlay(7), new Personagem.Personalizacao.OpacityOverlay(9), new Personagem.Personalizacao.OpacityOverlay(11) };
            p.PersonalizacaoDados.colorOverlays = new List<Personagem.Personalizacao.ColorOverlay> { new Personagem.Personalizacao.ColorOverlay(4), new Personagem.Personalizacao.ColorOverlay(5), new Personagem.Personalizacao.ColorOverlay(8) };
            for (var i = 1; i <= 10; i++)
            {
                p.Acessorios.Add(new Personagem.Vestimenta(i, 0, -1));
                p.Acessorios.Add(new Personagem.Vestimenta(i, 1, -1));
                p.Acessorios.Add(new Personagem.Vestimenta(i, 2, -1));
                p.Acessorios.Add(new Personagem.Vestimenta(i, 6, -1));
                p.Acessorios.Add(new Personagem.Vestimenta(i, 7, -1));
                p.Roupas.Add(new Personagem.Vestimenta(i, 1));
                p.Roupas.Add(new Personagem.Vestimenta(i, 3));
                p.Roupas.Add(new Personagem.Vestimenta(i, 4));
                p.Roupas.Add(new Personagem.Vestimenta(i, 5));
                p.Roupas.Add(new Personagem.Vestimenta(i, 6));
                p.Roupas.Add(new Personagem.Vestimenta(i, 7));
                p.Roupas.Add(new Personagem.Vestimenta(i, 8));
                p.Roupas.Add(new Personagem.Vestimenta(i, 9));
                p.Roupas.Add(new Personagem.Vestimenta(i, 10));
                p.Roupas.Add(new Personagem.Vestimenta(i, 11));
            }

            var personagem = new Personagem()
            {
                Codigo = codigo,
                Nome = nomeCompleto,
                Usuario = p.UsuarioBD.Codigo,
                DataNascimento = dtNascimento,
                SocialClubRegistro = (long)player.SocialClubId,
                SocialClubUltimoAcesso = (long)player.SocialClubId,
                IPRegistro = Functions.ObterIP(player),
                IPUltimoAcesso = Functions.ObterIP(player),
                Skin = (long)(sexo == "M" ? PedModel.FreemodeMale01 : PedModel.FreemodeFemale01),
                InformacoesPersonalizacao = JsonConvert.SerializeObject(p.PersonalizacaoDados),
                HardwareIdHashRegistro = (long)player.HardwareIdHash,
                HardwareIdExHashRegistro = (long)player.HardwareIdExHash,
                HardwareIdHashUltimoAcesso = (long)player.HardwareIdHash,
                HardwareIdExHashUltimoAcesso = (long)player.HardwareIdExHash,
                Historia = historia,
                Vida = player.MaxHealth,
                InformacoesRoupas = JsonConvert.SerializeObject(p.Roupas),
                InformacoesAcessorios = JsonConvert.SerializeObject(p.Acessorios),
            };

            if (personagemAntigo != null)
            {
                personagem.Dinheiro = personagemAntigo.Dinheiro;
                personagem.Banco = personagemAntigo.Banco;
                personagem.Poupanca = personagemAntigo.Poupanca;
                personagem.InformacoesArmas = personagemAntigo.InformacoesArmas;
            }

            if (codigo == 0)
                context.Personagens.Add(personagem);
            else
                context.Personagens.Update(personagem);

            context.SaveChanges();

            if (personagemAntigo != null)
            {
                Functions.GravarLog(TipoLog.Namechange, string.Empty, personagemAntigo, personagem);

                context.Database.ExecuteSqlRaw($"UPDATE Propriedades SET Personagem = {personagem.Codigo} WHERE Personagem = {personagemAntigo.Codigo}");
                context.Database.ExecuteSqlRaw($"UPDATE Veiculos SET Personagem = {personagem.Codigo} WHERE Personagem = {personagemAntigo.Codigo}");

                foreach (var x in Global.Propriedades.Where(x => x.Personagem == personagemAntigo.Codigo))
                    x.Personagem = personagem.Codigo;

                foreach (var x in Global.Veiculos.Where(x => x.Personagem == personagemAntigo.Codigo))
                    x.Personagem = personagem.Codigo;

                personagemAntigo.StatusNamechange = TipoStatusNamechange.Realizado;
                context.Personagens.Update(personagemAntigo);

                p.UsuarioBD.PossuiNamechange = false;
                context.Usuarios.Update(p.UsuarioBD);
                context.SaveChanges();
            }

            Functions.EnviarMensagemStaff("Uma nova aplicação de personagem foi recebida.", true);

            ListarPersonagens(player);
        }

        private void ListarPlayers(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var personagens = Global.PersonagensOnline
                .Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido)
                .OrderBy(x => x.ID == p.ID ? 0 : 1).ThenBy(x => x.ID)
                .Select(x => new { x.ID, Nome = x.NomeIC, x.Player.Ping }).ToList();

            var duty = Global.PersonagensOnline.Where(x => x.EmTrabalho);
            player.Emit("Server:ListarPlayers", Global.NomeServidor, JsonConvert.SerializeObject(personagens),
                $"Policiais: {duty.Count(x => x.FaccaoBD?.Tipo == TipoFaccao.Policial)} | Médicos: {duty.Count(x => x.FaccaoBD?.Tipo == TipoFaccao.Medica)} | Taxistas: {duty.Count(x => x.Emprego == TipoEmprego.Taxista)} | Mecânicos: {duty.Count(x => x.Emprego == TipoEmprego.Mecanico)}");
        }

        private void ComprarVeiculo(IPlayer player, int tipo, string veiculo, int r1, int g1, int b1, int r2, int g2, int b2)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (string.IsNullOrWhiteSpace(veiculo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Verifique se todos os campos foram preenchidos corretamente.", notify: true);
                return;
            }

            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == (TipoPreco)tipo && x.Nome.ToLower() == veiculo.ToLower());
            if (preco == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está disponível para compra.", notify: true);
                return;
            }

            if (p.Dinheiro < preco.Valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente.", notify: true);
                return;
            }

            var restricao = Functions.VerificarRestricaoVeiculo(veiculo);
            if (restricao.Item2 > p.UsuarioBD.VIP || (p.UsuarioBD.VIP != TipoVIP.Nenhum && (p.UsuarioBD.DataExpiracaoVIP ?? DateTime.MinValue) < DateTime.Now))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"O veículo é restrito para VIP {restricao.Item2}.", notify: true);
                return;
            }

            var concessionaria = Global.Concessionarias.FirstOrDefault(x => x.Tipo == (TipoPreco)tipo);

            var veh = new Veiculo()
            {
                Personagem = p.Codigo,
                Cor1R = r1,
                Cor1G = g1,
                Cor1B = b1,
                Cor2R = r2,
                Cor2G = g2,
                Cor2B = b2,
                Modelo = veiculo,
                Placa = Functions.GerarPlacaVeiculo(),
                PosX = concessionaria.PosicaoSpawn.X,
                PosY = concessionaria.PosicaoSpawn.Y,
                PosZ = concessionaria.PosicaoSpawn.Z,
                RotX = concessionaria.RotacaoSpawn.X,
                RotY = concessionaria.RotacaoSpawn.Y,
                RotZ = concessionaria.RotacaoSpawn.Z,
            };
            veh.Combustivel = veh.TanqueCombustivel;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Add(veh);
                context.SaveChanges();
            }

            p.Dinheiro -= preco.Valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou {veh.Modelo.ToUpper()} por ${preco.Valor:N0}. Use /vlista para spawnar.");
            player.Emit("Server:CloseView");
        }

        private void ComprarConveniencia(IPlayer player, string nome)
        {
            var p = Functions.ObterPersonagem(player);

            var preco = Global.Precos.FirstOrDefault(x => x.Nome == nome && x.Tipo == TipoPreco.Conveniencia);
            if (p.Dinheiro < preco.Valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente.", notify: true);
                return;
            }

            string strMensagem = string.Empty;
            switch (nome)
            {
                case "Celular":
                    if (p.Celular > 0)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um celular.", notify: true);
                        return;
                    }

                    using (var context = new DatabaseContext())
                    {
                        do
                        {
                            p.Celular = new Random().Next(1111111, 9999999);
                            if (p.Celular == 5555555 || p.Celular == 7777777 || context.Personagens.Any(x => x.Celular == p.Celular))
                                p.Celular = 0;

                        } while (p.Celular == 0);
                    }

                    strMensagem = $"Você comprou um celular. Seu número é: {p.Celular}.";
                    break;
                case "Rádio Comunicador":
                    if (p.CanalRadio > -1)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um rádio comunicador.", notify: true);
                        return;
                    }

                    p.CanalRadio = p.CanalRadio2 = p.CanalRadio3 = 0;

                    strMensagem = $"Você comprou um rádio comunicador.";
                    break;
                case "Peça Veicular":
                    p.PecasVeiculares++;

                    strMensagem = $"Você comprou uma peça veicular.";
                    break;
                case "Galão de Combustível":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.JerryCan))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um galão de combustível.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.JerryCan, 2000);
                    strMensagem = $"Você comprou um galão de gasolina.";
                    break;
                case "Soco Inglês":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.BrassKnuckles))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um soco inglês.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.BrassKnuckles, 1);
                    strMensagem = $"Você comprou um soco inglês.";
                    break;
                case "Garrafa":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.BrokenBottle))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui uma garrafa.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.BrokenBottle, 1);
                    strMensagem = $"Você comprou uma garrafa.";
                    break;
                case "Pé de Cabra":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.Crowbar))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um pé de cabra.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.Crowbar, 1);
                    strMensagem = $"Você comprou um pé de cabra.";
                    break;
                case "Taco de Golfe":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.GolfClub))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um taco de golfe.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.GolfClub, 1);
                    strMensagem = $"Você comprou um taco de golfe.";
                    break;
                case "Martelo":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.Hammer))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um martelo.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.Hammer, 1);
                    strMensagem = $"Você comprou um martelo.";
                    break;
                case "Chave de Grifo":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.PipeWrench))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui uma chave de grifo.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.PipeWrench, 1);
                    strMensagem = $"Você comprou uma chave de grifo.";
                    break;
                case "Taco de Baseball":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.BaseballBat))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um taco de baseball.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.BaseballBat, 1);
                    strMensagem = $"Você comprou um taco de baseball.";
                    break;
                case "Bola de Baseball":
                    if (p.Armas.Any(x => x.Codigo == (long)WeaponModel.Baseball))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui uma bola de baseball.", notify: true);
                        return;
                    }

                    p.DarArma(WeaponModel.Baseball, 1);
                    strMensagem = $"Você comprou uma bola de baseball.";
                    break;
                case "Máscara":
                    if (p.Mascara > 0)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui uma máscara.", notify: true);
                        return;
                    }

                    using (var context = new DatabaseContext())
                    {
                        do
                        {
                            p.Mascara = new Random().Next(1, int.MaxValue);
                            if (context.Personagens.Any(x => x.Mascara == p.Mascara))
                                p.Mascara = 0;

                        } while (p.Mascara == 0);
                    }

                    strMensagem = $"Você comprou uma máscara.";
                    break;
            }

            p.Dinheiro -= preco.Valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, strMensagem, notify: true);
        }

        private void AdicionarContatoCelular(IPlayer player, string nome, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (string.IsNullOrWhiteSpace(nome) || celular == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Verifique se os campos foram preenchidos corretamente.", notify: true);
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome não pode ter mais que 25 caracteres.", notify: true);
                return;
            }

            var contato = p.Contatos.FirstOrDefault(x => x.Celular == celular);
            if (contato == null)
            {
                p.Contatos.Add(new Personagem.Contato()
                {
                    Nome = nome,
                    Celular = celular
                });
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Contato {celular} adicionado.", notify: true);
                player.Emit("Server:AtualizarCelular", p.Celular, JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()));
            }
            else
            {
                contato.Nome = nome;
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Contato {celular} editado.", notify: true);
                player.Emit("Server:AtualizarCelular", p.Celular, JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()));
            }
        }

        private void RemoverContatoCelular(IPlayer player, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.Contatos.RemoveAll(x => x.Celular == celular);
            player.Emit("Server:AtualizarCelular", p.Celular, JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()));
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Contato {celular} removido.", notify: true);
        }

        private void PagarMulta(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            using (var context = new DatabaseContext())
            {
                var multa = context.Multas.FirstOrDefault(x => x.Codigo == codigo);
                if (p.Dinheiro < multa.Valor)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente (${multa.Valor:N0}).", notify: true);
                    return;
                }

                multa.DataPagamento = DateTime.Now;
                context.Multas.Update(multa);
                context.SaveChanges();

                p.Dinheiro -= multa.Valor;
                p.SetDinheiro();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pagou a multa {codigo}.");
            player.Emit("Server:CloseView");
        }

        private void PegarItemArmario(IPlayer player, int armario, uint weapon)
        {
            var arma = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == weapon);
            if ((arma?.Estoque ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O item não possui estoque.", notify: true);
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p.Rank < arma.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para pegar o item.", notify: true);
                return;
            }

            static bool VerificarArmaBranca(WeaponModel wp)
            {
                if (wp == WeaponModel.AntiqueCavalryDagger || wp == WeaponModel.BaseballBat || wp == WeaponModel.BrokenBottle
                    || wp == WeaponModel.Crowbar || wp == WeaponModel.Fist || wp == WeaponModel.Flashlight
                    || wp == WeaponModel.GolfClub || wp == WeaponModel.Hammer || wp == WeaponModel.Hatchet
                    || wp == WeaponModel.BrassKnuckles || wp == WeaponModel.Knife || wp == WeaponModel.Machete
                    || wp == WeaponModel.Switchblade || wp == WeaponModel.Nightstick || wp == WeaponModel.PipeWrench
                    || wp == WeaponModel.BattleAxe || wp == WeaponModel.PoolCue || wp == WeaponModel.StoneHatchet)
                    return true;

                return false;
            }

            if (p.Armas.Any(x => x.Codigo == weapon && (x.Municao > 0 || VerificarArmaBranca((WeaponModel)x.Codigo))))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui esse item.", notify: true);
                return;
            }

            if (p.FaccaoBD.Tipo == TipoFaccao.Criminosa)
            {
                var preco = Global.Precos.FirstOrDefault(x => x.Tipo == TipoPreco.Armas && x.Nome.ToLower() == ((WeaponModel)weapon).ToString().ToLower());
                if (preco != null)
                {
                    if (p.Dinheiro < preco.Valor)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente para fabricar esse item (${preco.Valor:N0}).", notify: true);
                        return;
                    }

                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();
                }
            }

            p.DarArma((WeaponModel)weapon, arma.Municao, arma.Pintura, arma.Componentes);

            arma.Estoque--;
            using var context = new DatabaseContext();
            context.ArmariosItens.Update(arma);
            context.SaveChanges();

            var componentes = Global.ArmariosComponentes.Where(x => x.Codigo == armario).OrderBy(x => x.Arma).ThenBy(x => x.Componente)
            .Select(x => new
            {
                Arma = ((WeaponModel)x.Arma).ToString(),
                Componente = Global.WeaponComponents.FirstOrDefault(y => y.Weapon == (WeaponModel)x.Arma && y.Hash == x.Componente)?.Name ?? string.Empty,
                ItemArma = x.Arma,
                ItemComponente = x.Componente,
            }).ToList();

            var itens = Global.ArmariosItens.Where(x => x.Codigo == armario).OrderBy(x => x.Rank).ThenBy(x => x.Arma)
            .Select(x => new
            {
                Arma = ((WeaponModel)x.Arma).ToString(),
                Item = x.Arma,
                x.Municao,
                x.Estoque,
                Rank = Global.Ranks.FirstOrDefault(y => y.Faccao == p.Faccao && y.Codigo == x.Rank).Nome,
                Preco = $"${Global.Precos.FirstOrDefault(y => y.Tipo == TipoPreco.Armas && y.Nome.ToLower() == ((WeaponModel)x.Arma).ToString().ToLower())?.Valor ?? 0:N0}",
            }).ToList();

            player.Emit("Server:AtualizarArmario", JsonConvert.SerializeObject(itens), JsonConvert.SerializeObject(componentes));
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você equipou {(WeaponModel)weapon}.", notify: true);
            Functions.GravarLog(TipoLog.Arma, $"/armario {JsonConvert.SerializeObject(arma)}", p, null);
        }

        private void EntregarArma(IPlayer player, int codigo, string weapon, int municao)
        {
            var p = Functions.ObterPersonagem(player);

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == codigo);
            if (target == null)
                return;

            uint.TryParse(weapon, out uint arma);
            var wep = p.Armas.FirstOrDefault(x => x.Codigo == arma);

            p.Armas.RemoveAll(x => x.Codigo == arma);
            player.Emit("RemoveWeapon", weapon);
            target.DarArma((WeaponModel)arma, municao, wep.Pintura, wep.Componentes);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você entregou {(WeaponModel)arma} com {municao} de munição para {target.NomeIC}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} te entregou {(WeaponModel)arma} com {municao} de munição.");
            Functions.GravarLog(TipoLog.Arma, $"/entregararma {arma}", p, target);
        }

        private void AtualizarInformacoes(IPlayer player, string areaName, string zoneName, string armas)
        {
            var p = Functions.ObterPersonagem(player);
            p.AreaName = $"{areaName} - {zoneName}";
            if (player.Dimension != 0)
            {
                var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == player.Dimension);
                if (prop != null)
                    p.AreaName = prop.Endereco;
            }

            var weapons = (armas ?? string.Empty).Split(";").Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Personagem.Arma()
                {
                    Codigo = long.Parse(x.Split("|")[0]),
                    Municao = int.Parse(x.Split("|")[1]),
                });

            var armasRemover = new List<Personagem.Arma>();
            foreach (var x in p.Armas)
            {
                var wep = weapons.FirstOrDefault(y => y.Codigo == x.Codigo);
                if (wep == null)
                    armasRemover.Add(x);
                else
                    x.Municao = wep?.Municao ?? 0;
            }

            // Feito para armas jogáveis
            foreach (var x in armasRemover)
                p.Armas.Remove(x);
        }

        private void SetVehicleMeta(IPlayer player, IVehicle vehicle, string meta, object value) => vehicle.SetStreamSyncedMetaData(meta, value);

        private void DevolverItensArmario(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);

            foreach (var x in p.Armas)
                player.Emit("RemoveWeapon", x.Codigo);
            p.Armas = new List<Personagem.Arma>();

            player.Armor = 0;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você devolveu seus itens no armário.", notify: true);
        }

        private void SpawnarVeiculoFaccao(IPlayer player, int codigoPonto, int veiculo)
        {
            var p = Functions.ObterPersonagem(player);

            if (Global.Veiculos.Any(x => x.Codigo == veiculo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo já está spawnado.", notify: true);
                return;
            }

            using var context = new DatabaseContext();
            var veh = context.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);
            veh.PosX = player.Position.X;
            veh.PosY = player.Position.Y;
            veh.PosZ = player.Position.Z;
            veh.NomeEncarregado = p.Nome;

            var ponto = Global.Pontos.FirstOrDefault(x => x.Codigo == codigoPonto);
            if (ponto.Tipo == TipoPonto.SpawnVeiculosFaccao)
            {
                var rot = JsonConvert.DeserializeObject<Rotation>(ponto.Configuracoes);
                veh.RotX = rot.Roll;
                veh.RotY = rot.Pitch;
                veh.RotZ = rot.Yaw;
            }

            veh.Spawnar();
            veh.Vehicle.LockState = VehicleLockState.Unlocked;
            player.Emit("setPedIntoVehicle", veh.Vehicle, -1);
            player.Emit("Server:CloseView");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você spawnou o veículo {veiculo}.", notify: true);
        }

        private void ConfirmarLojaRoupas(IPlayer player, string strRoupas, string strAcessorios, int roupa, int tipo, bool sucesso)
        {
            var p = Functions.ObterPersonagem(player);

            if (sucesso)
            {
                p.Roupa = roupa;
                p.InformacoesRoupas = strRoupas;
                p.Roupas = JsonConvert.DeserializeObject<List<Personagem.Vestimenta>>(p.InformacoesRoupas);
                p.InformacoesAcessorios = strAcessorios;
                p.Acessorios = JsonConvert.DeserializeObject<List<Personagem.Vestimenta>>(p.InformacoesAcessorios);
            }

            if (tipo == 0)
            {
                p.EtapaPersonalizacao = TipoEtapaPersonalizacao.Concluido;
                using var context = new DatabaseContext();
                context.Personagens.Update(p);
                context.SaveChanges();
                p.Spawnar();
            }
            else if (tipo == 1 && sucesso)
            {
                p.Dinheiro -= Global.Parametros.ValorRoupas;
                p.SetDinheiro();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pagou ${Global.Parametros.ValorRoupas:N0} na loja de roupas.");
            }

            player.Emit("Server:SelecionarPersonagem", p.InformacoesPersonalizacao, p.InformacoesRoupas, p.InformacoesAcessorios, p.Roupa, (int)p.EtapaPersonalizacao);
        }

        private void EnviarEmailConfirmacao(IPlayer player, string email)
        {
            if (email.Length > 100)
            {
                player.Emit("Server:MostrarErro", "E-mail não pode ter mais que 100 caracteres.");
                return;
            }

            if (!Functions.ValidarEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            using var context = new DatabaseContext();
            if (context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower() && x.Codigo != p.UsuarioBD.Codigo))
            {
                player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                return;
            }

            p.UsuarioBD.Email = email;
            context.Usuarios.Update(p.UsuarioBD);
            context.SaveChanges();

            Functions.EnviarEmail(email, "Confirmação de E-mail", $"Seu token de confirmação é <strong>{p.UsuarioBD.TokenConfirmacao}</strong>.");
            player.Emit("Server:MostrarSucesso", "E-mail com o token de confirmação enviado.");
        }

        private void ValidarTokenConfirmacao(IPlayer player, string token)
        {
            var p = Functions.ObterPersonagem(player);

            if (p.UsuarioBD.TokenConfirmacao != token)
            {
                player.Emit("Server:MostrarErro", "Token de confirmação incorreto.");
                return;
            }

            using var context = new DatabaseContext();
            p.UsuarioBD.TokenConfirmacao = string.Empty;
            context.Usuarios.Update(p.UsuarioBD);
            context.SaveChanges();

            ListarPersonagens(player);
        }

        private void ExibirPerguntas(IPlayer player)
        {
            var perguntas = Global.Perguntas.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
            var respostas = Global.Respostas.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var x in perguntas)
                x.Respostas = respostas.Where(y => y.Pergunta == x.Codigo).ToList();
            player.Emit("Server:ExibirPerguntas", JsonConvert.SerializeObject(perguntas));
        }

        private void ValidarPerguntas(IPlayer player, string strPerguntas)
        {
            var perguntas = JsonConvert.DeserializeObject<List<Pergunta>>(strPerguntas);
            var qtdAcertos = perguntas.Count(x => x.RespostaCorreta == x.RespostaSelecionada);
            if (qtdAcertos < perguntas.Count)
            {
                player.Emit("Server:MostrarErro", $"Você não acertou todas as perguntas. Acertos: {qtdAcertos} de {perguntas.Count}.");
                return;
            }

            player.Emit("Server:RegistrarUsuario");
        }

        private void EnviarEmailAlterarSenha(IPlayer player, string usuario, string email)
        {
            using var context = new DatabaseContext();
            var user = context.Usuarios.AsQueryable().Where(x => x.Nome.ToLower() == usuario.ToLower() && x.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (user != null)
            {
                var senha = Functions.GerarStringAleatoria(10);
                user.Senha = Functions.Criptografar(senha);
                context.Usuarios.Update(user);
                context.SaveChanges();

                Functions.EnviarEmail(email, "Recuperação da Senha", $"Sua nova senha é <strong>{senha}</strong>. Lembre-se de alterar em seu próximo acesso.");
            }
            else
            {
                Functions.GravarLog(TipoLog.EsqueciMinhaSenhaFalha, $"Usuário: {usuario} | E-mail: {email}", null, null);
            }

            player.Emit("Server:MostrarSucesso", "Caso o usuário e o e-mail correspondam, o e-mail será enviado. Verifique também sua caixa de lixo eletrônico.");
        }

        private void ConfirmarPersonalizacao(IPlayer player, string strPersonalizacao, int tipo, bool sucesso)
        {
            var p = Functions.ObterPersonagem(player);

            if (sucesso)
            {
                p.InformacoesPersonalizacao = strPersonalizacao;
                p.PersonalizacaoDados = JsonConvert.DeserializeObject<Personagem.Personalizacao>(p.InformacoesPersonalizacao);
            }

            if (tipo == 0)
            {
                p.EtapaPersonalizacao = TipoEtapaPersonalizacao.Roupas;
                using var context = new DatabaseContext();
                context.Personagens.Update(p);
                context.SaveChanges();
            }
            else if (sucesso)
            {
                p.Dinheiro -= Global.Parametros.ValorBarbearia;
                p.SetDinheiro();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pagou ${Global.Parametros.ValorBarbearia:N0} na barbearia.");
            }

            player.Emit("Server:SelecionarPersonagem", p.InformacoesPersonalizacao, p.InformacoesRoupas, p.InformacoesAcessorios, p.Roupa, (int)p.EtapaPersonalizacao);
        }

        private void DeletarPersonagem(IPlayer player, int codigo)
        {
            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == codigo);
            personagem.DataExclusao = DateTime.Now;
            context.Personagens.Update(personagem);
            context.SaveChanges();

            Functions.GravarLog(TipoLog.ExclusaoPersonagem, string.Empty, personagem, null);
            ListarPersonagens(player);
        }

        private void Chatting(IPlayer player, bool chatting) => player.SetSyncedMetaData("chatting", chatting);

        private void EquiparColeteArmario(IPlayer player)
        {
            player.Armor = 100;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você equipou um colete.", notify: true);
        }

        private void PegarComponenteArmario(IPlayer player, uint arma, uint componente)
        {
            var p = Functions.ObterPersonagem(player);
            p.Dinheiro -= Global.Parametros.ValorComponentes;
            p.SetDinheiro();

            var pArma = p.Armas.FirstOrDefault(x => x.Codigo == arma);
            var componentes = JsonConvert.DeserializeObject<List<uint>>(pArma.Componentes);
            componentes.Add(componente);
            pArma.Componentes = JsonConvert.SerializeObject(componentes);

            var wc = Global.WeaponComponents.FirstOrDefault(x => x.Weapon == (WeaponModel)arma && x.Hash == componente);
            player.AddWeaponComponent(arma, componente);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você equipou {wc?.Name} na arma {(WeaponModel)arma}.", notify: true);
        }

        private void LigarContatoCelular(IPlayer player, int celular) => Functions.LigarCelular(player, celular.ToString());

        private void EnviarLocalizacaoContatoCelular(IPlayer player, int celular) => Functions.EnviarLocalizacaoCelular(player, celular.ToString());

        private void AbastecerVeiculo(IPlayer player, int veiculo)
        {
            var p = Functions.ObterPersonagem(player);

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);

            var combustivelNecessario = veh.TanqueCombustivel - veh.Combustivel;
            var valor = combustivelNecessario * Global.Parametros.ValorCombustivel;
            if (valor > p.Dinheiro && veh.Faccao == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente (${valor:N0}).");
                return;
            }

            player.Emit("Server:freezeEntityPosition", true);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Aguarde 5 segundos.");
            AltAsync.Do(async () =>
            {
                await Task.Delay(5000);
                veh.Combustivel = veh.TanqueCombustivel;
                veh.Vehicle.SetSyncedMetaData("combustivel", veh.CombustivelHUD);
                if (veh.Faccao == 0)
                {
                    p.Dinheiro -= valor;
                    p.SetDinheiro();
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você abasteceu {combustivelNecessario} litro{(combustivelNecessario > 1 ? "s" : string.Empty)} de combustível por ${valor:N0}.");
                }
                else
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você abasteceu {combustivelNecessario} litro{(combustivelNecessario > 1 ? "s" : string.Empty)} de combustível.");
                }
                Functions.SendMessageToNearbyPlayers(player, "abastece o veículo.", TipoMensagemJogo.Ame, 10);
                player.Emit("Server:freezeEntityPosition", false);
            });
        }

        private void PunicoesAdministrativas(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);

            using var context = new DatabaseContext();
            var sql = $@"SELECT pun.*, per.Nome NomePersonagem, usu.Nome NomeUsuarioStaff
            FROM Punicoes pun
            INNER JOIN Personagens per ON pun.Personagem = per.Codigo
            INNER JOIN Usuarios usu ON pun.UsuarioStaff = usu.Codigo
            WHERE per.Usuario = {p.UsuarioBD.Codigo}
            ORDER BY pun.Codigo DESC";
            var punicoes = context.PunicoesAdministrativas.FromSqlRaw(sql).ToList();

            player.Emit("Server:PunicoesAdministrativas", p.UsuarioBD.Nome, DateTime.Now.ToString(),
                JsonConvert.SerializeObject(punicoes.Select(x => new
                {
                    Personagem = x.NomePersonagem,
                    Data = x.Data.ToString(),
                    Tipo = x.Tipo.ToString(),
                    Duracao = x.Tipo == TipoPunicao.Ban ? (x.Duracao > 0 ? $"{x.Duracao} dia{(x.Duracao != 1 ? "s" : string.Empty)}" : "Permanente") : string.Empty,
                    Staffer = x.NomeUsuarioStaff,
                    x.Motivo,
                })));
        }

        private void AlterarEmail(IPlayer player, string email)
        {
            if (email.Length > 100)
            {
                player.Emit("Server:MostrarErro", "E-mail não pode ter mais que 100 caracteres.");
                return;
            }

            if (!Functions.ValidarEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            using var context = new DatabaseContext();
            if (context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower()))
            {
                player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                return;
            }

            p.UsuarioBD.TokenConfirmacao = new Random().Next(111111, 999999).ToString();
            p.UsuarioBD.Email = email;
            context.Usuarios.Update(p.UsuarioBD);
            context.SaveChanges();

            Functions.EnviarEmail(email, "Confirmação de E-mail", $"Você alterou seu e-mail. Seu token de confirmação é <strong>{p.UsuarioBD.TokenConfirmacao}</strong>.");
            player.Emit("Server:ConfirmacaoRegistro", p.UsuarioBD.Nome, p.UsuarioBD.Email);
        }

        private void AlterarSenha(IPlayer player, string senhaAntiga, string novaSenha, string novaSenha2)
        {
            if (string.IsNullOrWhiteSpace(senhaAntiga) || string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(novaSenha2))
            {
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            if (novaSenha != novaSenha2)
            {
                player.Emit("Server:MostrarErro", "Novas senhas não são iguais.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (Functions.Criptografar(senhaAntiga) != p.UsuarioBD.Senha)
            {
                player.Emit("Server:MostrarErro", "Sua senha atual não confere.");
                return;
            }

            using var context = new DatabaseContext();
            p.UsuarioBD.Senha = Functions.Criptografar(novaSenha);
            context.Usuarios.Update(p.UsuarioBD);
            context.SaveChanges();

            player.Emit("Server:MostrarSucesso", "Sua senha foi alterada.");
        }

        private void UsarATM(IPlayer player, int tipo, int target, int valor, bool sucesso)
        {
            if (!sucesso)
            {
                if (tipo == 1)
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM ou não possui um celular.");
                else
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM.");

                return;
            }

            if (tipo == 1)
                Functions.CMDTransferir(player, target, valor);
            else
                Functions.CMDSacar(player, valor);
        }

        private void PintarVeiculo(IPlayer player, int veiculo, int tipo, int r1, int g1, int b1, int r2, int g2, int b2)
        {
            player.Emit("Server:CloseView");

            var p = Functions.ObterPersonagem(player);
            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);
            if (tipo == 1)
            {
                if (veh == null)
                    return;

                player.Emit("Server:freezeEntityPosition", true);
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Aguarde 5 segundos.");
                AltAsync.Do(async () =>
                {
                    await Task.Delay(5000);
                    await veh.Vehicle.SetPrimaryColorRgbAsync(new Rgba((byte)r1, (byte)g1, (byte)b1, 255));
                    await veh.Vehicle.SetSecondaryColorRgbAsync(new Rgba((byte)r2, (byte)g2, (byte)b2, 255));
                    p.PecasVeiculares--;
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você pintou o veículo e usou uma peça veicular.");
                    Functions.SendMessageToNearbyPlayers(player, "pinta o veículo.", TipoMensagemJogo.Ame, 10);
                    player.Emit("Server:freezeEntityPosition", false);
                });
            }
            else
            {
                if (veh != null)
                    return;

                using (var context = new DatabaseContext())
                    veh = context.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Cores do veículo {veh.Codigo} alteradas para {r1} {g1} {b1} {r2} {g2} {b2}.");
                Functions.GravarLog(TipoLog.Staff, $"/evehcor {veh.Codigo} {r1} {g1} {b1} {r2} {g2} {b2}", p, null);
            }

            using (var context = new DatabaseContext())
            {
                veh.Cor1R = r1;
                veh.Cor1G = g1;
                veh.Cor1B = b1;
                veh.Cor2R = r2;
                veh.Cor2G = g2;
                veh.Cor2B = b2;
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }
        }

        private void SpawnarVeiculo(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (Global.Veiculos.Any(x => x.Codigo == codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo já está spawnado.", notify: true);
                return;
            }

            using var context = new DatabaseContext();
            var veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh.ValorApreensao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo está apreendido.", notify: true);
                return;
            }

            veh.Spawnar();
            player.Emit("Server:SetWaypoint", veh.PosX, veh.PosY);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você spawnou seu veículo.", notify: true);
            player.Emit("Server:CloseView");
        }

        private void VenderVeiculo(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            var veh = Global.Veiculos.FirstOrDefault(x => x.Personagem == p.Codigo && x.Vehicle == player.Vehicle);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um veículo seu.");
                return;
            }

            var valor = Convert.ToInt32((Global.Precos.FirstOrDefault(x => x.Veiculo && x.Nome.ToLower() == veh.Modelo.ToLower())?.Valor ?? 0) / 2);
            p.Dinheiro += valor;
            p.SetDinheiro();

            veh.VendidoFerroVelho = true;
            veh.Despawnar();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você vendeu seu veículo {veh.Codigo} para o ferro velho por ${valor:N0}.");
            Functions.GravarLog(TipoLog.Venda, $"/vvender {veh.Codigo} {valor}", p, null);
        }

        private void SoltarSacoLixo(IPlayer player, float x, float y, float z)
        {
            if (player.Position.Distance(new Position(x, y, z)) > 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está na parte de trás de um caminho de lixo.");
                return;
            }

            player.Emit("Server:SoltarSacoLixo");
            var p = Functions.ObterPersonagem(player);
            player.Emit("blip:remove", p.PontoColetando.Codigo * -1);
            player.Emit("marker:remove", p.PontoColetando.Codigo * -1);
            p.PontosColeta.Remove(p.PontoColetando);
            p.ItensColetados++;
            p.PontoColetando = new Ponto();
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você colocou um saco de lixo no caminhão.");

            if (p.PontosColeta.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você realizou todas as coletas. Retorne ao centro de reciclagem e saia de serviço.");
                return;
            }
        }

        private void MDCPesquisarPessoa(IPlayer player, string pesquisa)
        {
            AltAsync.Do(async () =>
            {
                var html = string.Empty;
                using var context = new DatabaseContext();
                var per = await context.Personagens.AsQueryable().FirstOrDefaultAsync(x => x.Nome.ToLower() == pesquisa.ToLower());
                if (per == null)
                {
                    html = $@"<div class='alert alert-danger'>Nenhuma pessoa foi encontrada com a pesquisa <strong>{pesquisa}</strong>.</div>";
                }
                else
                {
                    var propriedades = Global.Propriedades.Where(x => x.Personagem == per.Codigo).ToList();

                    var veiculos = await context.Veiculos.AsQueryable().Where(x => x.Personagem == per.Codigo && !x.VendidoFerroVelho)
                    .OrderByDescending(x => x.Codigo).ToListAsync();

                    var multas = await context.Multas.AsQueryable().Where(x => x.PersonagemMultado == per.Codigo)
                        .Include(x => x.PersonagemPolicialBD).OrderByDescending(x => x.Codigo).ToListAsync();

                    var prisoes = await context.Prisoes.AsQueryable().Where(x => x.Preso == per.Codigo)
                        .Include(x => x.PolicialBD).OrderByDescending(x => x.Codigo).ToListAsync();

                    var confiscos = await context.Confiscos.AsQueryable().Where(x => x.Personagem == per.Codigo)
                        .Include(x => x.PersonagemPolicialBD).OrderByDescending(x => x.Codigo).ToListAsync();

                    var emprego = Functions.ObterDisplayEnum(per.Emprego);
                    if (per.Faccao > 0)
                        emprego = $"{Global.Ranks.FirstOrDefault(x => x.Codigo == per.Rank && x.Faccao == per.Faccao)?.Nome ?? string.Empty} - {Global.Faccoes.FirstOrDefault(x => x.Codigo == per.Faccao)?.Nome ?? string.Empty}";

                    var botaoRevogarLicenca = $@"<button onclick='revogarLicenca({per.Codigo},""{per.Nome}"");' type='button' class='btn btn-xs btn-dark'>Revogar Licença de Motorista</button>";
                    var licencaMotorista = $"<span class='label' style='background-color:{Global.CorSucesso};'>VÁLIDA</span>";
                    if (per.PolicialRevogacaoLicencaMotorista > 0)
                    {
                        var policial = await context.Personagens.AsQueryable().FirstOrDefaultAsync(x => x.Codigo == per.PolicialRevogacaoLicencaMotorista);
                        licencaMotorista = $"<span class='label' style='background-color:{Global.CorErro};'>REVOGADA POR {policial?.Nome?.ToUpper() ?? string.Empty}</span>";
                        botaoRevogarLicenca = string.Empty;
                    }
                    else if (!per.DataValidadeLicencaMotorista.HasValue)
                    {
                        licencaMotorista = $"<span class='label' style='background-color:{Global.CorErro};'>NÃO POSSUI</span>";
                        botaoRevogarLicenca = string.Empty;
                    }
                    else if (per.DataValidadeLicencaMotorista?.Date < DateTime.Now.Date)
                    {
                        licencaMotorista = $"<span class='label' style='background-color:{Global.CorErro};'>VENCIDA</span>";
                        botaoRevogarLicenca = string.Empty;
                    }

                    var strBolo = string.Empty;
                    var botaoBolo = $@"<button onclick='adicionarBOLO(1, {per.Codigo},""{per.Nome}"");' type='button' class='btn btn-xs btn-dark'>Adicionar APB</button>";

                    var bolo = await context.Procurados.AsQueryable().Where(x => x.Personagem == per.Codigo && !x.DataExclusao.HasValue).Include(x => x.PersonagemPolicialBD).FirstOrDefaultAsync();
                    if (bolo != null)
                    {
                        botaoBolo = $@"<button onclick='removerBOLO({bolo.Codigo},""{per.Nome}"");' type='button' class='btn btn-xs btn-dark'>Remover APB</button>";
                        strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>APB</span> {bolo.Motivo}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PersonagemPolicialBD?.Nome ?? string.Empty}</strong> em <strong>{bolo.Data}</strong>.</p>
                                </div>
                            </div>
                        </div>";
                    }

                    html = $@"<div class='row'>
                                <div class='col-md-6'>
                                    <h3>{per.Nome}</h3>
                                </div>
                                <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                                    <button onclick='multar({per.Codigo}, ""{per.Nome}"");' type='button' class='btn btn-xs btn-dark'>Multar</button>
                                    {botaoBolo}
                                    {botaoRevogarLicenca}
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-md-3'>
                                    <p>Data de Nascimento: <strong>{per.DataNascimento.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - per.DataNascimento).TotalDays / 365):N0} anos)</strong></p>
                                </div>
                                <div class='col-md-2'>
                                    <p>Celular: <strong>{(per.Celular == 0 ? "N/A" : per.Celular.ToString())}</strong></p>
                                </div>
                                <div class='col-md-4'>
                                    <p>Emprego: <strong>{emprego}</strong></p>
                                </div>
                                <div class='col-md-3'>
                                    <p>Licença de Motorista: {licencaMotorista}</p>
                                </div>
                            </div>
                            {strBolo}";

                    if (propriedades.Count > 0)
                    {
                        html += $@"<p class='text-main text-semibold'>Propriedades</p>";
                        foreach (var x in propriedades)
                            html += $@"<p>Nº {x.Codigo} - {x.Endereco}</p>";
                    }

                    if (veiculos.Count > 0)
                    {
                        html += $@"<p class='text-main text-semibold'>Veículos</p>";
                        foreach (var x in veiculos)
                            html += $@"<p>Modelo: <strong>{x.Modelo.ToUpper()}</strong> Placa: <strong>{x.Placa}</strong></p>";
                    }

                    if (multas.Count > 0)
                    {
                        html += $@"<p class='text-main text-semibold'>Multas</p>";
                        foreach (var x in multas)
                            html += $@"<p>Data: <strong>{x.Data}</strong> Valor: <strong>${x.Valor:N0}</strong> Policial: <strong>{x.PersonagemPolicialBD?.Nome ?? string.Empty}</strong> Motivo: <strong>{x.Motivo}</strong> Status: <strong>{(x.DataPagamento.HasValue ? $"<span class='label' style='background-color:{Global.CorSucesso};'>PAGA</span>" : $"<span class='label' style='background-color:{Global.CorErro};'>PENDENTE</span>")}</strong> <button onclick='$.alert(""{x.Descricao}"");' class='btn btn-dark btn-xs'>Detalhes</button></p>";
                    }

                    if (prisoes.Count > 0)
                    {
                        html += $@"<p class='text-main text-semibold'>Prisões</p>";
                        foreach (var x in prisoes)
                            html += $@"<p>Data: <strong>{x.Data}</strong> Tempo: <strong>{Convert.ToInt32((x.Termino - x.Data).TotalMinutes)} minutos</strong> Policial: <strong>{x.PolicialBD?.Nome ?? string.Empty}</strong></p> <button onclick='$.alert(""<p>{x.Descricao}</p><p>{x.Crimes}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button></p>";
                    }

                    if (confiscos.Count > 0)
                    {
                        html += $@"<p class='text-main text-semibold'>Confiscos</p>";
                        foreach (var x in confiscos)
                        {
                            html += $@"<p>Data: <strong>{x.Data}</strong> Policial: <strong>{x.PersonagemPolicialBD?.Nome ?? string.Empty}</strong> <button onclick='$.alert(""{x.Descricao}"");' class='btn btn-dark btn-xs'>Detalhes</button></p>";
                            var armas = JsonConvert.DeserializeObject<List<Personagem.Arma>>(x.Armas);
                            html += "<p>Armas</p>";
                            foreach (var y in armas)
                                html += $@"<p>Arma: <strong>{(WeaponModel)y.Codigo}</strong> Munição: <strong>{y.Municao}</strong></p>";
                        }
                    }
                }

                player.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa", html);
            });
        }

        private void MDCPesquisarVeiculo(IPlayer player, string pesquisa)
        {
            AltAsync.Do(async () =>
            {
                var html = string.Empty;
                using var context = new DatabaseContext();
                var veh = await context.Veiculos.AsQueryable().FirstOrDefaultAsync(x => x.Placa.ToLower() == pesquisa.ToLower());
                if (veh == null)
                {
                    html = $@"<div class='alert alert-danger'>Nenhum veículo foi encontrado com a pesquisa <strong>{pesquisa}</strong>.</div>";
                }
                else
                {
                    var strBolo = string.Empty;
                    var botaoBolo = string.Empty;
                    var proprietario = $"Emprego de {Functions.ObterDisplayEnum(veh.Emprego)}";
                    if (veh.Personagem > 0)
                    {
                        botaoBolo = $@"<button onclick='adicionarBOLO(2, {veh.Codigo},""{pesquisa}"");' type='button' class='btn btn-xs btn-dark'>Adicionar BOLO</button>";
                        if (veh.VendidoFerroVelho)
                        {
                            proprietario = "FERRO VELHO";
                        }
                        else
                        {
                            var per = await context.Personagens.AsQueryable().FirstOrDefaultAsync(x => x.Codigo == veh.Personagem);
                            proprietario = per?.Nome ?? string.Empty;
                        }

                        var bolo = await context.Procurados.AsQueryable().Where(x => x.Veiculo == veh.Codigo && !x.DataExclusao.HasValue).Include(x => x.PersonagemPolicialBD).FirstOrDefaultAsync();
                        if (bolo != null)
                        {
                            botaoBolo = $@"<button onclick='removerBOLO({bolo.Codigo},""{pesquisa}"");' type='button' class='btn btn-xs btn-dark'>Remover BOLO</button>";
                            strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>BOLO</span> {bolo.Motivo}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PersonagemPolicialBD?.Nome ?? string.Empty}</strong> em <strong>{bolo.Data}</strong>.</p>
                                </div>
                            </div>
                            </div>";
                        }
                    }
                    else if (veh.Faccao > 0)
                    {
                        proprietario = Global.Faccoes.FirstOrDefault(x => x.Codigo == veh.Faccao)?.Nome ?? string.Empty;
                    }

                    html += $@"<div class='row'>
                    <div class='col-md-6'>
                        <h3>{veh.Placa}</h3>
                    </div>
                    <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                        {botaoBolo}
                        {(veh.Faccao > 0 || veh.Emprego != TipoEmprego.Nenhum ? $"<button onclick='rastrearVeiculo({veh.Codigo});' type='button' class='btn btn-xs btn-dark'>Rastrear</button>" : string.Empty)}
                    </div>
                    </div>
                    <div class='row'>
                        <div class='col-md-4'>
                            <p>Modelo: <strong>{veh.Modelo.ToUpper()}</strong></p>
                        </div>
                        <div class='col-md-4'>
                            <p>Proprietário: <strong>{proprietario}</strong></p>
                        </div>
                        <div class='col-md-4'>
                            <p>Apreendido: <strong>{(veh.ValorApreensao > 0 ? $"SIM (${veh.ValorApreensao:N0})" : "NÃO")}</strong></p>
                        </div>
                    </div>
                    {strBolo}";
                }

                player.Emit("Server:AtualizarMDC", "btn-pesquisarveiculo", "div-pesquisarveiculo", html);
            });
        }

        private void MDCPesquisarPropriedade(IPlayer player, string pesquisa)
        {
            AltAsync.Do(async () =>
            {
                var html = string.Empty;
                var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo.ToString() == pesquisa);
                if (prop == null)
                {
                    html = $@"<div class='alert alert-danger'>Nenhuma propriedade foi encontrada com a pesquisa <strong>{pesquisa}</strong>.</div>";
                }
                else
                {
                    var proprietario = "N/A";
                    if (prop.Personagem > 0)
                    {
                        using var context = new DatabaseContext();
                        var per = await context.Personagens.AsQueryable().FirstOrDefaultAsync(x => x.Codigo == prop.Personagem);
                        proprietario = per?.Nome ?? string.Empty;
                    }

                    html = $@"<h3>Propriedade Nº {prop.Codigo}</h3>
                    <div class='row'>
                        <div class='col-md-6'>
                            <p>Endereço: <strong>{prop.Endereco}</strong></p>
                        </div>
                        <div class='col-md-4'>
                            <p>Proprietário: <strong>{proprietario}</strong></p>
                        </div>
                        <div class='col-md-2'>
                            <p>Valor: <strong>${prop.Valor:N0}</strong></p>
                        </div>
                    </div>";
                }

                player.Emit("Server:AtualizarMDC", "btn-pesquisarpropriedade", "div-pesquisarpropriedade", html);
            });
        }

        private void MDCRastrearVeiculo(IPlayer player, int codigo)
        {
            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O veículo não foi encontrado.", notify: true);
                return;
            }

            player.Emit("Server:SetWaypoint", veh.Vehicle.Position.X, veh.Vehicle.Position.Y);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "A posição do veículo foi marcada no seu GPS.", notify: true);
        }

        private void MDCAdicionarBOLO(IPlayer player, int tipo, int codigo, string motivo, string pesquisa)
        {
            AltAsync.Do(async () =>
            {
                var p = Functions.ObterPersonagem(player);
                var bolo = new Procurado
                {
                    PersonagemPolicial = p.Codigo,
                    Motivo = motivo,
                };

                if (tipo == 1)
                    bolo.Personagem = codigo;
                else
                    bolo.Veiculo = codigo;

                using var context = new DatabaseContext();
                await context.Procurados.AddAsync(bolo);
                await context.SaveChangesAsync();

                if (tipo == 1)
                    MDCPesquisarPessoa(player, pesquisa);
                else
                    MDCPesquisarVeiculo(player, pesquisa);

                var procurados = await Functions.ObterHTMLProcurados();
                player.Emit("Server:AtualizarMDC", string.Empty, "tab-apb", procurados.Item1);
                player.Emit("Server:AtualizarMDC", string.Empty, "tab-bolo", procurados.Item2);
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"{(tipo == 1 ? "APB" : "BOLO")} adicionado.", notify: true);
            });
        }

        private void MDCRemoverBOLO(IPlayer player, int codigo, string pesquisa)
        {
            AltAsync.Do(async () =>
            {
                var p = Functions.ObterPersonagem(player);
                using var context = new DatabaseContext();
                var bolo = await context.Procurados.AsQueryable().FirstOrDefaultAsync(x => x.Codigo == codigo);
                bolo.PersonagemPolicialExclusao = p.Codigo;
                bolo.DataExclusao = DateTime.Now;
                context.Procurados.Update(bolo);
                await context.SaveChangesAsync();
                if (bolo.Personagem > 0)
                    MDCPesquisarPessoa(player, pesquisa);
                else
                    MDCPesquisarVeiculo(player, pesquisa);

                var procurados = await Functions.ObterHTMLProcurados();
                player.Emit("Server:AtualizarMDC", string.Empty, "tab-apb", procurados.Item1);
                player.Emit("Server:AtualizarMDC", string.Empty, "tab-bolo", procurados.Item2);
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"{(bolo.Personagem > 0 ? "APB" : "BOLO")} removido.", notify: true);
            });
        }

        private void MDCRastrear911(IPlayer player, int codigo)
        {
            var ligacao911 = Global.Ligacoes911.FirstOrDefault(x => x.Codigo == codigo);
            player.Emit("Server:SetWaypoint", ligacao911.PosX, ligacao911.PosY);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"A localização da ligação de emergência #{codigo} foi marcada no seu GPS.", notify: true);
        }

        private void MDCMultar(IPlayer player, int codigo, string nome, int valor, string motivo, string descricao)
        {
            AltAsync.Do(async () =>
            {
                if (valor <= 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido.", notify: true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não foi informado.", notify: true);
                    return;
                }

                if (motivo.Length > 255)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres.", notify: true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(descricao))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Descrição não foi informada.", notify: true);
                    return;
                }

                var p = Functions.ObterPersonagem(player);
                using (var context = new DatabaseContext())
                {
                    await context.Multas.AddAsync(new Multa()
                    {
                        Motivo = motivo,
                        PersonagemMultado = codigo,
                        PersonagemPolicial = p.Codigo,
                        Valor = valor,
                        Descricao = descricao,
                    });
                    await context.SaveChangesAsync();
                }

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {nome} por ${valor:N0}. Motivo: {motivo}", notify: true);

                var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == codigo);
                if ((target?.Celular ?? 0) > 0)
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {target.ObterNomeContato(911)}: Você recebeu uma multa de ${valor:N0}. Motivo: {motivo}", Global.CorCelular);

                MDCPesquisarPessoa(player, nome);
            });
        }

        private void MDCRevogarLicencaMotorista(IPlayer player, int codigo)
        {
            AltAsync.Do(async () =>
            {
                var p = Functions.ObterPersonagem(player);
                using var context = new DatabaseContext();

                var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == codigo);
                if (target != null)
                    target.PolicialRevogacaoLicencaMotorista = p.Codigo;

                var per = await context.Personagens.AsQueryable().FirstOrDefaultAsync(x => x.Codigo == codigo);
                per.PolicialRevogacaoLicencaMotorista = p.Codigo;
                context.Personagens.Update(per);
                await context.SaveChangesAsync();
            });
        }
        #endregion
    }
}