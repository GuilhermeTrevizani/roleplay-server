using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace Roleplay
{
    public class Server : AsyncResource
    {
        Timer TimerPrincipal { get; set; }

        public override void OnStart()
        {
            Alt.OnPlayerConnect += OnPlayerConnect;
            Alt.OnPlayerDisconnect += OnPlayerDisconnect;
            Alt.OnPlayerDead += OnPlayerDead;
            Alt.OnWeaponDamage += OnWeaponDamage;
            Alt.OnPlayerDamage += OnPlayerDamage;
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
            Alt.OnClient<IPlayer>("ConfirmarLojaRoupas", ConfirmarLojaRoupas);
            Alt.OnClient<IPlayer, string>("EnviarEmailConfirmacao", EnviarEmailConfirmacao);
            Alt.OnClient<IPlayer, string>("ValidarTokenConfirmacao", ValidarTokenConfirmacao);
            Alt.OnClient<IPlayer>("ExibirPerguntas", ExibirPerguntas);
            Alt.OnClient<IPlayer, string>("ValidarPerguntas", ValidarPerguntas);
            Alt.OnClient<IPlayer, string, string>("EnviarEmailAlterarSenha", EnviarEmailAlterarSenha);
            Alt.OnClient<IPlayer, int, string, string, string>("AlterarSenhaRecuperacao", AlterarSenhaRecuperacao);
            Alt.OnClient<IPlayer, string, bool, bool>("ConfirmarPersonalizacao", ConfirmarPersonalizacao);
            Alt.OnClient<IPlayer, int>("DeletarPersonagem", DeletarPersonagem);
            Alt.OnClient<IPlayer, bool>("Chatting", Chatting);
            Alt.OnClient<IPlayer>("EquiparColeteArmario", EquiparColeteArmario);

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture =
                  CultureInfo.GetCultureInfo("pt-BR");

            var config = JsonConvert.DeserializeObject<Configuracao>(File.ReadAllText("settings.json"));
            Global.MaxPlayers = config.MaxPlayers;
            Global.ConnectionString = $"Server={config.DBHost};Database={config.DBName};Uid={config.DBUser};Password={config.DBPassword}";

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
            }

            foreach (var c in Global.Concessionarias)
                Functions.CriarTextDraw($"{c.Nome}\n~w~Use /comprar", c.PosicaoCompra, 10, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);
            Console.WriteLine($"Concessionarias: {Global.Concessionarias.Count}");

            foreach (var c in Global.Empregos)
            {
                var nome = Functions.ObterDisplayEnum(c.Tipo);
                Functions.CriarTextDraw($"Emprego de {nome}\n~w~Use /emprego para se tornar um {nome.ToLower()}", c.Posicao, 10, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);
            }
            Console.WriteLine($"Empregos: {Global.Empregos.Count}");

            Functions.CriarTextDraw("Prisão\n~w~Use /prender", Global.PosicaoPrisao, 10, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);

            Global.TACVoice = new List<IVoiceChannel>
            {
                Alt.CreateVoiceChannel(false, 0),
                Alt.CreateVoiceChannel(false, 0),
                Alt.CreateVoiceChannel(false, 0),
                Alt.CreateVoiceChannel(false, 0),
                Alt.CreateVoiceChannel(false, 0),
            };

            Global.GlobalVoice = Alt.CreateVoiceChannel(false, 10);

            TimerPrincipal = new Timer(60000);
            TimerPrincipal.Elapsed += TimerPrincipal_Elapsed;
            TimerPrincipal.Start();
        }

        public override void OnStop()
        {
            TimerPrincipal?.Stop();
            foreach (var p in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
                Functions.SalvarPersonagem(p);
        }

        private void OnPlayerConnect(IPlayer player, string reason)
        {
            player.SetDateTime(DateTime.Now);
            player.SetWeather(Global.Weather);
            player.Spawn(new Position(0f, 0f, 0f));

            using var context = new DatabaseContext();

            if (!Functions.VerificarBanimento(player, context.Banimentos.FirstOrDefault(x => (x.SocialClub == (long)player.SocialClubId && x.SocialClub != 0)
                || x.HardwareIdHash == (long)player.HardwareIdHash
                || x.HardwareIdExHash == (long)player.HardwareIdExHash)))
                return;

            player.Emit("Server:Login", context.Usuarios.FirstOrDefault(x => (x.SocialClubRegistro == (long)player.SocialClubId && x.SocialClubRegistro != 0)
                || x.HardwareIdHashRegistro == (long)player.HardwareIdHash
                || x.HardwareIdExHashRegistro == (long)player.HardwareIdExHash)?.Nome ?? string.Empty);
        }

        private void OnPlayerDisconnect(IPlayer player, string reason)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Codigo > 0)
            {
                Functions.GravarLog(TipoLog.Saida, reason, p, null);
                Functions.SalvarPersonagem(p, false);
            }

            Global.PersonagensOnline.RemoveAll(x => x.Player?.HardwareIdHash == player.HardwareIdHash);
        }

        private void OnPlayerDead(IPlayer player, IEntity killer, uint weapon)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            Functions.GravarLog(TipoLog.Morte, JsonConvert.SerializeObject(p.Ferimentos), p,
                killer is IPlayer playerKiller ? Functions.ObterPersonagem(playerKiller) : null);

            Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você foi gravemente ferido e perdeu a consciência! Os médicos deverão chegar em até 3 minutos.");

            player.SetSyncedMetaData("ferido", true);

            AltAsync.Do(async () =>
            {
                await Task.Delay(5000);
                await player.SpawnAsync(player.Position);
                p.StopAnimation();
                p.PlayAnimation("misslamar1dead_body", "dead_idle", (int)AnimationFlags.Loop);
            });

            p.TimerFerido?.Stop();
            p.TimerFerido = new TagTimer(180000)
            {
                Tag = p.Codigo,
            };
            p.TimerFerido.Elapsed += TimerFerido_Elapsed;
            p.TimerFerido.Start();
        }

        private void TimerFerido_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timer = (TagTimer)sender;
            timer.ElapsedCount++;

            var p = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == (int)timer.Tag);
            if (p == null)
            {
                timer?.Stop();
                return;
            }

            Functions.EnviarMensagem(p.Player, TipoMensagem.Erro, "Digite /aceitartratamento para que você receba os cuidados dos médicos.");
            Functions.EnviarMensagem(p.Player, TipoMensagem.Erro, "Digite /aceitarck para aplicar CK no seu personagem. ESSA OPERAÇÃO É IRREVERSÍVEL.");
            timer?.Stop();
        }

        private void OnPlayerChat(IPlayer player, string message)
        {
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
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"O comando {{{Global.CorAmarelo}}}{message}{{#FFFFFF}} não existe. Digite {{{Global.CorAmarelo}}}/ajuda{{#FFFFFF}} para visualizar os comandos disponíveis.");
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
                    }
                }

                if (methodParams.Length != arr.Count)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Os parâmetros do comando não foram informados corretamente. Use: {{{Global.CorAmarelo}}}{command.HelpText}");
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
            if (!(target is IPlayer))
                return true;

            var p = Functions.ObterPersonagem((IPlayer)target);
            if (p == null)
                return false;

            p.Ferimentos.Add(new Personagem.Ferimento()
            {
                Data = DateTime.Now,
                Arma = weapon,
                Dano = damage,
                BodyPart = (sbyte)bodyPart,
                CodigoAttacker = Functions.ObterPersonagem(player)?.Codigo ?? 0,
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

            var ferimento = new Personagem.Ferimento()
            {
                Data = DateTime.Now,
                Arma = weapon,
                Dano = damage,
            };

            if (attacker is IPlayer playerAttacker)
                ferimento.CodigoAttacker = Functions.ObterPersonagem(playerAttacker)?.Codigo ?? 0;
            else if (attacker is IVehicle vehicleAttacker)
                ferimento.CodigoAttacker = Functions.ObterPersonagem(vehicleAttacker.Driver)?.Codigo ?? 0;

            p.Ferimentos.Add(ferimento);
        }

        private void TimerPrincipal_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var p in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
                Functions.SalvarPersonagem(p);
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
            var user = context.Usuarios.FirstOrDefault(x => x.Nome == usuario && x.Senha == senhaCriptografada);
            if (user == null)
            {
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

            using var context = new DatabaseContext();
            player.Emit("Server:ListarPersonagens", p.UsuarioBD.Nome,
                JsonConvert.SerializeObject(context.Personagens
                    .Where(x => x.Usuario == p.UsuarioBD.Codigo && x.StatusNamechange != TipoStatusNamechange.Realizado && !x.DataExclusao.HasValue)
                    .OrderByDescending(x => x.Codigo)
                    .ToList()
                    .Select(x => new
                    {
                        x.Codigo,
                        x.Nome,
                        Status = ObterStatusListarPersonagens(x),
                        Opcoes = ObterOpcoesListarPersonagens(x, p.UsuarioBD),
                    })),
                    Global.Parametros.SlotsPersonagens);
        }

        private string ObterStatusListarPersonagens(Personagem x)
        {
            var span = $@"<span style=""color:#1de312;"">Vivo</span>";

            if (x.DataMorte.HasValue)
                span = $@"<span style=""color:#d12c0f;"">Morto ({x.MotivoMorte})</span>";
            else if ((x.DataTerminoPrisao ?? DateTime.MinValue) > DateTime.Now)
                span = $@"<span style=""color:#d12c0f;"">Preso até {x.DataTerminoPrisao}</span>";
            else if (!string.IsNullOrWhiteSpace(x.MotivoRejeicao))
                span = $@"<span style=""color:#d12c0f;"">Rejeitado</span>";
            else if (x.UsuarioStaffAvaliador == 0)
                span = $@"<span style=""color:#e69215;"">Aguardando Avaliação</span>";

            return span;
        }

        private string ObterOpcoesListarPersonagens(Personagem x, Usuario u)
        {
            var opcoes = string.Empty;
            if (!x.DataMorte.HasValue && x.UsuarioStaffAvaliador != 0 && (x.DataTerminoPrisao ?? DateTime.MinValue) < DateTime.Now)
            {
                if (string.IsNullOrWhiteSpace(x.MotivoRejeicao))
                    opcoes = $"<button onclick='selecionarPersonagem({x.Codigo}, false);'>LOGAR</button>";
                else
                    opcoes = $"<button onclick='selecionarPersonagem({x.Codigo}, false);'>REFAZER APLICAÇÃO</button>";
            }
            opcoes += x.StatusNamechange == TipoStatusNamechange.Liberado && u.PossuiNamechange && string.IsNullOrWhiteSpace(x.MotivoRejeicao) && x.UsuarioStaffAvaliador != 0 ? $" <button onclick='selecionarPersonagem({x.Codigo}, true);'>TROCAR NOME</button>" : string.Empty;
            opcoes += $" <button onclick='deletarPersonagem({x.Codigo});' style='background-color:#d12c0f;color:#fff;'>DELETAR</button>";
            return opcoes;
        }

        private void SelecionarPersonagem(IPlayer player, int id, bool namechange)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null || p?.ID > 0)
                return;

            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == id && x.Usuario == p.UsuarioBD.Codigo);
            if (!string.IsNullOrWhiteSpace(personagem.MotivoRejeicao) || namechange)
            {
                var staffer = context.Usuarios.FirstOrDefault(x => x.Codigo == personagem.UsuarioStaffAvaliador);
                var nome = personagem.Nome.Split(' ');
                player.Emit("Server:CriarPersonagem", personagem.Codigo, nome.FirstOrDefault(), nome.LastOrDefault(), p.PersonalizacaoDados.sex == 1 ? "H" : "M", personagem.DataNascimento.ToShortDateString(), personagem.Historia, personagem.MotivoRejeicao, staffer.Nome);
                return;
            }

            personagem.DataUltimoAcesso = DateTime.Now;
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

            player.Dimension = (int)p.Dimensao;
            p.IPLs = JsonConvert.DeserializeObject<List<string>>(p.IPL);
            p.SetarIPLs();
            player.SetDateTime(DateTime.Now);
            player.Health = (ushort)(p.Vida + 100);
            player.Armor = (ushort)p.Colete;
            player.Model = (uint)p.Skin;
            p.SetDinheiro();
            player.SetWeather(Global.Weather);

            p.Contatos = JsonConvert.DeserializeObject<List<Personagem.Contato>>(p.InformacoesContatos);

            var roupas = JsonConvert.DeserializeObject<List<Personagem.Roupa>>(p.InformacoesRoupas);
            foreach (var x in roupas)
                p.SetClothes(x.Slot, x.Drawable, x.Texture);

            var acessorios = JsonConvert.DeserializeObject<List<Personagem.Roupa>>(p.InformacoesAcessorios);
            foreach (var x in acessorios)
                p.SetAccessories(x.Slot, x.Drawable, x.Texture);

            p.Armas = JsonConvert.DeserializeObject<List<Personagem.Arma>>(p.InformacoesArmas);
            foreach (var x in p.Armas)
            {
                player.GiveWeapon((WeaponModel)x.Codigo, x.Municao, false);
                player.SetWeaponTintIndex((WeaponModel)x.Codigo, (byte)x.Pintura);
                foreach (var c in JsonConvert.DeserializeObject<List<uint>>(x.Componentes))
                    player.AddWeaponComponent((WeaponModel)x.Codigo, c);
            }

            p.PersonalizacaoDados = JsonConvert.DeserializeObject<Personagem.Personalizacao>(p.InformacoesPersonalizacao);

            if (Global.PersonagensOnline.Count(x => x.Codigo > 0) > Global.Parametros.RecordeOnline)
            {
                Global.Parametros.RecordeOnline = Global.PersonagensOnline.Count;
                context.Parametros.Update(Global.Parametros);
                context.SaveChanges();

                foreach (var u in Global.PersonagensOnline)
                    Functions.EnviarMensagem(u.Player, TipoMensagem.Nenhum, $"O novo recorde de jogadores online é: {{{Global.CorSucesso}}}{Global.Parametros.RecordeOnline}{{#FFFFFF}}.");
            }

            Functions.GravarLog(TipoLog.Entrada, string.Empty, p, null);

            if (personagem.EtapaPersonalizacao != TipoEtapaPersonalizacao.Concluido)
            {
                player.Dimension = p.ID;
                player.Spawn(new Position(402.84396f, -996.9758f, -99.01465f));
                player.Rotation = new Position(0f, 0f, -3.017908f);
            }
            else
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Olá {{{Global.CorAmarelo}}}{p.UsuarioBD.Nome}{{#FFFFFF}}, que bom te ver por aqui! Seu último login foi em {{{Global.CorAmarelo}}}{personagem.DataUltimoAcesso}{{#FFFFFF}}.");
                player.SetSyncedMetaData("nametag", p.Nome);
                player.Emit("nametags:Config", true);
                player.Emit("chat:activateTimeStamp", p.UsuarioBD.TimeStamp);
                player.Spawn(new Position(p.PosX, p.PosY, p.PosZ));
                player.Rotation = new Position(p.RotX, p.RotY, p.RotZ);
                Global.GlobalVoice.AddPlayer(player);
                Global.GlobalVoice.MutePlayer(player);
            }

            player.Emit("Server:SelecionarPersonagem", p.InformacoesPersonalizacao, (int)personagem.EtapaPersonalizacao);
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
                if (context.Usuarios.Any(x => (x.SocialClubRegistro == (long)player.SocialClubId && x.SocialClubRegistro != 0)
                    || x.HardwareIdHashRegistro == (long)player.HardwareIdHash
                    || x.HardwareIdExHashRegistro == (long)player.HardwareIdExHash))
                {
                    player.Emit("Server:MostrarErro", $"Você já possui um usuário.");
                    return;
                }

                if (context.Usuarios.Any(x => x.Nome == usuario))
                {
                    player.Emit("Server:MostrarErro", $"Usuário {usuario} já existe.");
                    return;
                }

                if (context.Usuarios.Any(x => x.Email == email))
                {
                    player.Emit("Server:MostrarErro", $"Email {email} já está sendo utilizado.");
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
                    TokenConfirmacao = new Random().Next(111111, 999999).ToString(),
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

            var nomeCompleto = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome)} {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sobrenome)}";
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

            if (context.Personagens.Any(x => x.Nome == nomeCompleto && x.Codigo != codigo))
            {
                player.Emit("Server:MostrarErro", $"Personagem {nomeCompleto} já existe.");
                return;
            }

            p.PersonalizacaoDados.sex = sexo == "M" ? 1 : 0;
            p.PersonalizacaoDados.structure = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            p.PersonalizacaoDados.opacityOverlays = new List<Personagem.Personalizacao.OpacityOverlay> { new Personagem.Personalizacao.OpacityOverlay(0), new Personagem.Personalizacao.OpacityOverlay(3), new Personagem.Personalizacao.OpacityOverlay(6), new Personagem.Personalizacao.OpacityOverlay(7), new Personagem.Personalizacao.OpacityOverlay(9), new Personagem.Personalizacao.OpacityOverlay(11) };
            p.PersonalizacaoDados.colorOverlays = new List<Personagem.Personalizacao.ColorOverlay> { new Personagem.Personalizacao.ColorOverlay(4), new Personagem.Personalizacao.ColorOverlay(5), new Personagem.Personalizacao.ColorOverlay(8) };

            p.Contatos = new List<Personagem.Contato>()
            {
                new Personagem.Contato()
                {
                    Celular = 911,
                    Nome = "Central de Emergência",
                },
                new Personagem.Contato()
                {
                    Celular = 5555555,
                    Nome = "Dowtown Cab Co.",
                },
            };
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
                InformacoesContatos = JsonConvert.SerializeObject(p.Contatos),
            };

            if (personagemAntigo != null)
            {
                personagem.Dinheiro = personagemAntigo.Dinheiro;
                personagem.Banco = personagemAntigo.Banco;
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

                var propriedades = Global.Propriedades.Where(x => x.Personagem == personagemAntigo.Codigo);
                foreach (var x in propriedades)
                    x.Personagem = personagem.Codigo;

                var veiculos = Global.Veiculos.Where(x => x.Personagem == personagemAntigo.Codigo);
                foreach (var x in veiculos)
                    x.Personagem = personagem.Codigo;

                personagemAntigo.StatusNamechange = TipoStatusNamechange.Realizado;
                context.Personagens.Update(personagemAntigo);

                p.UsuarioBD.PossuiNamechange = false;
                context.Usuarios.Update(p.UsuarioBD);
                context.SaveChanges();
            }

            ListarPersonagens(player);
        }

        private void ListarPlayers(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var personagens = Global.PersonagensOnline.Where(x => x.ID > 0).OrderBy(x => x.ID == p.ID ? 0 : 1).ThenBy(x => x.ID)
                .Select(x => new { x.ID, x.Nome, x.Player.Ping }).ToList();

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
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == (TipoPreco)tipo && x.Nome.ToLower() == veiculo.ToLower());
            if (preco == null)
            {
                player.Emit("Server:MostrarErro", "Veículo não está disponível para compra.");
                return;
            }

            if (p.Dinheiro < preco.Valor)
            {
                player.Emit("Server:MostrarErro", "Você não possui dinheiro suficiente.");
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

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Add(veh);
                context.SaveChanges();
            }

            p.Dinheiro -= preco.Valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou {veh.Modelo.ToUpper()} por ${preco.Valor:N0}. Use /vspawn {veh.Codigo} para spawnar.");
            player.Emit("Server:CloseView");
        }

        private void ComprarConveniencia(IPlayer player, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var preco = Global.Precos.FirstOrDefault(x => x.Nome == nome && x.Tipo == TipoPreco.Conveniencia);
            if (p.Dinheiro < preco.Valor)
            {
                player.Emit("Server:MostrarErro", "Você não possui dinheiro suficiente.");
                return;
            }

            string strMensagem = string.Empty;
            switch (nome)
            {
                case "Celular":
                    if (p?.Celular > 0)
                    {
                        player.Emit("Server:MostrarErro", "Você já possui um celular.");
                        return;
                    }

                    using (var context = new DatabaseContext())
                    {
                        do
                        {
                            p.Celular = new Random().Next(1111111, 9999999);
                            if (p.Celular == 5555555 || context.Personagens.Any(x => x.Celular == p.Celular))
                                p.Celular = 0;

                        } while (p.Celular == 0);
                    }

                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();

                    strMensagem = $"Você comprou um celular! Seu número é: {p.Celular}.";
                    break;
                case "Rádio Comunicador":
                    if (p?.CanalRadio > -1)
                    {
                        player.Emit("Server:MostrarErro", "Você já possui um rádio comunicador.");
                        return;
                    }

                    p.CanalRadio = p.CanalRadio2 = p.CanalRadio3 = 0;
                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();

                    strMensagem = $"Você comprou um rádio comunicador.";
                    break;
            }

            player.Emit("Server:MostrarSucesso", strMensagem);
        }

        private void AdicionarContatoCelular(IPlayer player, string nome, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (string.IsNullOrWhiteSpace(nome) || celular == 0)
            {
                player.Emit("Server:MostrarErro", "Verifique se os campos foram preenchidos corretamente.");
                return;
            }

            if (nome.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Nome não pode ter mais que 25 caracteres.");
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
                player.Emit("Server:AtualizarCelular", JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()), $"Contato {celular} adicionado com sucesso.");
            }
            else
            {
                contato.Nome = nome;
                player.Emit("Server:AtualizarCelular", JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()), $"Contato {celular} editado com sucesso.");
            }
        }

        private void RemoverContatoCelular(IPlayer player, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.Contatos.RemoveAll(x => x.Celular == celular);
            player.Emit("Server:AtualizarCelular", JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()), $"Celular {celular} removido dos contatos.");
        }

        private void PagarMulta(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            using (var context = new DatabaseContext())
            {
                var multa = context.Multas.FirstOrDefault(x => x.Codigo == codigo);
                if (p.Dinheiro < multa.Valor)
                {
                    player.Emit("Server:MostrarErro", "Você não possui dinheiro suficiente.");
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
                player.Emit("Server:MostrarErro", $"O item não possui estoque.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p.Rank < arma.Rank)
            {
                player.Emit("Server:MostrarErro", $"Você não possui autorização para pegar o item.");
                return;
            }

            if (p.FaccaoBD.Tipo == TipoFaccao.Criminosa)
            {
                var preco = Global.Precos.FirstOrDefault(x => x.Tipo == TipoPreco.Armas && x.Nome.ToLower() == ((WeaponModel)weapon).ToString().ToLower());
                if (preco != null)
                {
                    if (p.Dinheiro < preco.Valor)
                    {
                        player.Emit("Server:MostrarErro", $"Você não possui dinheiro suficiente para fabricar esse item (${preco.Valor:N0}).");
                        return;
                    }

                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();
                }
            }

            player.GiveWeapon(weapon, arma.Municao, false);
            player.SetWeaponTintIndex(weapon, (byte)arma.Pintura);
            var componentes = JsonConvert.DeserializeObject<List<uint>>(arma.Componentes);
            foreach (var x in componentes)
                player.AddWeaponComponent(weapon, x);

            p.Armas.Add(new Personagem.Arma()
            {
                Codigo = arma.Arma,
                Municao = arma.Municao,
                Pintura = arma.Pintura,
                Componentes = arma.Componentes,
            });

            arma.Estoque--;
            using var context = new DatabaseContext();
            context.ArmariosItens.Update(arma);
            context.SaveChanges();

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

            player.Emit("Server:AtualizarArmario", armario, p.FaccaoBD.Nome, JsonConvert.SerializeObject(itens), p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica, p.FaccaoBD.Tipo == TipoFaccao.Policial, $"Você equipou {(WeaponModel)weapon}.");
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

            player.Emit("RemoveWeapon", arma);
            p.Armas.Remove(wep);

            wep.Codigo = target.Codigo;
            target.Player.GiveWeapon(arma, municao, true);
            target.Player.SetWeaponTintIndex(arma, (byte)wep.Pintura);
            var componentes = JsonConvert.DeserializeObject<List<uint>>(wep.Componentes);
            foreach (var x in componentes)
                target.Player.AddWeaponComponent(arma, x);
            target.Armas.Add(wep);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu {(WeaponModel)arma} com {municao} de munição para {target.NomeIC}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} te deu {(WeaponModel)arma} com {municao} de munição.");
            Functions.GravarLog(TipoLog.Arma, $"/entregararma {arma}", p, target);
        }

        private void AtualizarInformacoes(IPlayer player, string areaName, string zoneName, string armas)
        {
            var p = Functions.ObterPersonagem(player);
            p.AreaName = areaName;
            p.ZoneName = zoneName;
            p.StringArmas = armas;
        }

        private void SetVehicleMeta(IPlayer player, IVehicle vehicle, string meta, object value) => vehicle.SetStreamSyncedMetaData(meta, value);

        private void DevolverItensArmario(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);

            foreach (var x in p.Armas)
                player.Emit("RemoveWeapon", (uint)x.Codigo);

            p.Armas = new List<Personagem.Arma>();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você devolveu seus itens no armário.", notify: true);
            Functions.GravarLog(TipoLog.Arma, $"/armario DevolverItensArmario", p, null);
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

        private void ConfirmarLojaRoupas(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            p.Dinheiro -= Global.Parametros.ValorRoupas;
            p.SetDinheiro();
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pagou ${Global.Parametros.ValorRoupas:N0} na loja de roupas.");
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
            var user = context.Usuarios.Where(x => x.Nome == usuario && x.Email == email).FirstOrDefault();
            if (user != null)
            {
                user.TokenSenha = Functions.GerarStringAleatoria(25);
                context.Usuarios.Update(user);
                context.SaveChanges();

                Functions.EnviarEmail(email, "Recuperação da Senha", $"Seu token para alteração de senha é <strong>{user.TokenSenha}</strong>.");
            }

            player.Emit("Server:EsqueciMinhaSenha", user?.Codigo ?? 0);
        }

        private void AlterarSenhaRecuperacao(IPlayer player, int codigo, string token, string senha, string senha2)
        {
            using var context = new DatabaseContext();
            var user = context.Usuarios.Where(x => x.Codigo == codigo && x.TokenSenha == token && !string.IsNullOrWhiteSpace(x.TokenSenha)).FirstOrDefault();
            if (user == null)
            {
                player.Emit("Server:Login");
                return;
            }

            if (senha != senha2)
            {
                player.Emit("Server:MostrarErro", "Senhas não são iguais.");
                return;
            }

            user.TokenSenha = string.Empty;
            user.Senha = Functions.Criptografar(senha);
            context.Usuarios.Update(user);
            context.SaveChanges();

            player.Emit("Server:MostrarSucesso", "Sua senha foi alterada com sucesso.");
        }

        private void ConfirmarPersonalizacao(IPlayer player, string strPersonalizacao, bool barbearia, bool sucesso)
        {
            var p = Functions.ObterPersonagem(player);

            if (sucesso)
            {
                p.InformacoesPersonalizacao = strPersonalizacao;
                p.PersonalizacaoDados = JsonConvert.DeserializeObject<Personagem.Personalizacao>(p.InformacoesPersonalizacao);
            }

            if (!barbearia)
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

            player.Emit("Server:SelecionarPersonagem", p.InformacoesPersonalizacao, (int)p.EtapaPersonalizacao);
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
        #endregion
    }
}