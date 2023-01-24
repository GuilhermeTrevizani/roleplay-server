using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class LoginScript : IScript
    {
        [AsyncClientEvent(nameof(EntrarUsuario))]
        public static async Task EntrarUsuario(MyPlayer player, string usuario, string senha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
                {
                    player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                    return;
                }

                var senhaCriptografada = Functions.Encrypt(senha);
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.Name.ToLower() == usuario.ToLower() && x.Password == senhaCriptografada);
                if (user == null)
                {
                    await player.GravarLog(LogType.LoginFalha, $"Usuário: {usuario}", null);
                    player.Emit("Server:MostrarErro", "Usuário ou senha inválidos.");
                    return;
                }

                var banishment = await context.Banishments.Where(x => x.UserId == user.Id).Include(x => x.StaffUser).FirstOrDefaultAsync();
                if (banishment != null)
                {
                    if (banishment.ExpirationDate.HasValue && DateTime.Now > banishment.ExpirationDate)
                    {
                        context.Banishments.Remove(banishment);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var strBan = !banishment.ExpirationDate.HasValue ? " permanentemente." : $". Seu banimento expira em: {banishment.ExpirationDate?.ToString()}";

                        player.Emit("Server:BaseHTML", Functions.GetBaseHTML("Banimento", $"Você está banido{strBan}<br/>Data: <strong>{banishment.Date}</strong><br/>Motivo: <strong>{banishment.Reason}</strong><br/>Staffer: <strong>{banishment.StaffUser.Name}</strong>"));
                        return;
                    }
                }

                if (Global.Players.Any(x => x.User.Id == user.Id))
                {
                    player.Emit("Server:MostrarErro", "Usuário já está logado.");
                    return;
                }

                player.User = user;
                player.User.LastAccessDate = DateTime.Now;
                player.User.LastAccessIp = player.RealIp;
                player.User.LastAccessHardwareIdHash = player.HardwareIdHash;
                player.User.LastAccessHardwareIdExHash = player.HardwareIdExHash;
                context.Users.Update(player.User);
                await context.SaveChangesAsync();

                player.StaffFlags = JsonSerializer.Deserialize<List<StaffFlag>>(player.User.StaffFlagsJSON);

                await VerificarRegistro(player);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        private static async Task VerificarRegistro(MyPlayer player)
        {
            if (!string.IsNullOrWhiteSpace(player.User.EmailConfirmationToken))
            {
                player.Emit("Server:ConfirmacaoEmail", player.User.Name, player.User.Email);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Global.DiscordBotToken) && !string.IsNullOrWhiteSpace(player.User.DiscordConfirmationToken))
            {
                player.Emit("Server:ConfirmacaoDiscord", player.User.Name, player.User.DiscordConfirmationToken);
                return;
            }

            await ListarPersonagens(player);
        }

        [AsyncClientEvent(nameof(VerificarConfirmacaoDiscord))]
        public static async Task VerificarConfirmacaoDiscord(MyPlayer player)
        {
            using var context = new DatabaseContext();
            var usuario = await context.Users.FirstOrDefaultAsync(x => x.Id == player.User.Id);
            player.User.Discord = usuario.Discord;
            player.User.DiscordConfirmationToken = usuario.DiscordConfirmationToken;

            if (string.IsNullOrWhiteSpace(player.User.DiscordConfirmationToken))
            {
                await ListarPersonagens(player);
                return;
            }

            player.Emit("Server:MostrarErro", "Você ainda não vinculou nenhuma conta do Discord.");
        }

        [AsyncClientEvent(nameof(ListarPersonagens))]
        public static async Task ListarPersonagens(MyPlayer player, string alerta = "")
        {
            var slots = 2;

            if ((player.User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now)
            {
                slots = 2;
            }
            else
            {
                slots = player.User.VIP switch
                {
                    UserVIP.Bronze => 3,
                    UserVIP.Silver => 4,
                    UserVIP.Gold => 5,
                    _ => 2,
                };
            }

            await using var context = new DatabaseContext();
            player.Emit("Server:ListarPersonagens", player.User.Name,
                JsonSerializer.Serialize((await context.Characters
                    .Where(x => x.UserId == player.User.Id
                        && x.NameChangeStatus != CharacterNameChangeStatus.Realizado
                        && !x.DeletedDate.HasValue)
                    .ToListAsync())
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        Status = ObterStatusListarPersonagens(x),
                        Options = ObterOpcoesListarPersonagens(x, player.User),
                    })),
                    slots,
                    alerta);
        }

        private static string ObterStatusListarPersonagens(Character x)
        {
            var span = $@"<span class=""label"" style=""background-color:{Global.SUCCESS_COLOR};"">Vivo</span>";
            if (x.CKAvaliation)
                span = $@"<span class=""label"" style=""background-color:{Global.ERROR_COLOR};"">Avaliação de CK</span>";
            else if (x.DeathDate.HasValue)
                span = $@"<span class=""label"" style=""background-color:{Global.ERROR_COLOR};"">Morto ({x.DeathReason})</span>";
            else if ((x.JailFinalDate ?? DateTime.MinValue) > DateTime.Now)
                span = $@"<span class=""label"" style=""background-color:{Global.ERROR_COLOR};"">Preso até {x.JailFinalDate}</span>";
            else if (!string.IsNullOrWhiteSpace(x.RejectionReason))
                span = $@"<span class=""label"" style=""background-color:{Global.ERROR_COLOR};"">Rejeitado</span>";
            else if (!x.EvaluatorStaffUserId.HasValue)
                span = $@"<span class=""label"" style=""background-color:#f0972b;"">Aguardando Avaliação</span>";
            return span;
        }

        private static string ObterOpcoesListarPersonagens(Character x, User u)
        {
            var opcoes = string.Empty;
            if (!x.CKAvaliation && !x.DeathDate.HasValue && x.EvaluatorStaffUserId.HasValue && (x.JailFinalDate ?? DateTime.MinValue) < DateTime.Now)
            {
                if (string.IsNullOrWhiteSpace(x.RejectionReason))
                    opcoes = $"<button class='btn btn-primary btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem({x.Id}, false);'>LOGAR</button>";
                else
                    opcoes = $"<button class='btn btn-dark btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem({x.Id}, false);'>REFAZER APLICAÇÃO</button>";
            }
            opcoes += x.NameChangeStatus == CharacterNameChangeStatus.Liberado && u.NameChanges > 0 && string.IsNullOrWhiteSpace(x.RejectionReason) && x.EvaluatorStaffUserId.HasValue ? $" <button class='btn btn-dark btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem({x.Id}, true);'>ALTERAR NOME</button>" : string.Empty;
            opcoes += $" <button class='btn btn-danger btn-sm mar-sm-top' onclick='deletarPersonagem({x.Id});' style='background-color:#d12c0f;color:#fff;'>DELETAR</button>";
            return opcoes;
        }

        [AsyncClientEvent(nameof(SelecionarPersonagem))]
        public async Task SelecionarPersonagem(MyPlayer player, int id, bool namechange)
        {
            try
            {
                await using var context = new DatabaseContext();
                var character = await context.Characters.Where(x => x.Id == id && x.UserId == player.User.Id)
                    .Include(x => x.EvaluatorStaffUser)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(character.RejectionReason) || namechange)
                {
                    var nome = character.Name.Split(' ');
                    player.Emit("Server:CriarPersonagem", character.Id, nome.FirstOrDefault(), nome.LastOrDefault(),
                        character.Sex == CharacterSex.Man ? "M" : "F", character.BirthdayDate.ToShortDateString(),
                        character.History, character.RejectionReason, character.EvaluatorStaffUser.Name);
                    return;
                }

                character.EvaluatorStaffUser = null;

                var banishment = await context.Banishments.Where(x => x.CharacterId == character.Id).Include(x => x.StaffUser).FirstOrDefaultAsync();
                if (banishment != null)
                {
                    if (banishment.ExpirationDate.HasValue && DateTime.Now > banishment.ExpirationDate)
                    {
                        context.Banishments.Remove(banishment);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var strBan = !banishment.ExpirationDate.HasValue ? " permanentemente." : $". Seu banimento expira em: {banishment.ExpirationDate?.ToString()}";
                        player.Emit("Server:MostrarErro", $"Você está banido{strBan}<br/>Data: <strong>{banishment.Date}</strong><br/>Motivo: <strong>{banishment.Reason}</strong><br/>Staffer: <strong>{banishment.StaffUser.Name}</strong>", $".btn-selecionarpersonagem{character.Id}");
                        return;
                    }
                }

                player.Character = character;
                player.Character.LastAccessIp = player.RealIp;
                player.Character.LastAccessHardwareIdHash = player.HardwareIdHash;
                player.Character.LastAccessHardwareIdExHash = player.HardwareIdExHash;
                player.Character.JailFinalDate = null;

                player.IPLs = JsonSerializer.Deserialize<List<string>>(player.Character.IPLsJSON);
                player.SetarIPLs();
                player.Model = (uint)player.Character.Model;
                player.SetWeather((uint)Global.WeatherInfo.WeatherType);
                player.Ferimentos = JsonSerializer.Deserialize<List<MyPlayer.Ferimento>>(player.Character.WoundsJSON);
                player.Items = (await context.CharactersItems.Where(x => x.CharacterId == player.Character.Id).ToListAsync()).Select(x => new CharacterItem(x)).ToList();
                player.Character.Mask = 0;
                player.FactionFlags = JsonSerializer.Deserialize<List<FactionFlag>>(player.Character.FactionFlagsJSON);
                await player.SetarRoupas();

                if (!string.IsNullOrWhiteSpace(player.Character.PersonalizationJSON))
                    player.Personalization = JsonSerializer.Deserialize<MyPlayer.Personalizacao>(player.Character.PersonalizationJSON);

                if (!player.Personalization.Structure.Any())
                    player.Personalization.Structure = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                if (!player.Personalization.OpacityOverlays.Any())
                    player.Personalization.OpacityOverlays = new List<MyPlayer.Personalizacao.OpacityOverlay>
                    {
                        new MyPlayer.Personalizacao.OpacityOverlay(0),
                        new MyPlayer.Personalizacao.OpacityOverlay(3),
                        new MyPlayer.Personalizacao.OpacityOverlay(6),
                        new MyPlayer.Personalizacao.OpacityOverlay(7),
                        new MyPlayer.Personalizacao.OpacityOverlay(9),
                        new MyPlayer.Personalizacao.OpacityOverlay(11)
                    };

                if (!player.Personalization.ColorOverlays.Any())
                    player.Personalization.ColorOverlays = new List<MyPlayer.Personalizacao.ColorOverlay>
                    {
                        new MyPlayer.Personalizacao.ColorOverlay(4),
                        new MyPlayer.Personalizacao.ColorOverlay(5),
                        new MyPlayer.Personalizacao.ColorOverlay(8)
                    };

                player.ClearBloodDamage();
                player.SetarPersonalizacao(player.Personalization);

                var qtdOnline = Global.Players.Count(x => x.Character.Id > 0);
                if (qtdOnline > Global.Parameter.MaxCharactersOnline)
                {
                    Global.Parameter.MaxCharactersOnline = qtdOnline;
                    context.Parameters.Update(Global.Parameter);
                    await context.SaveChangesAsync();
                    await Functions.SendStaffMessage($"O novo recorde de jogadores online é: {Global.Parameter.MaxCharactersOnline}.", true, true);
                }

                await player.GravarLog(LogType.Entrada, string.Empty, null);

                player.SetDateTime(DateTime.Now.AddHours(2));
                player.Emit("SyncWeather", Global.WeatherInfo.WeatherType.ToString().ToUpper());
                player.Invincible = true;
                player.Frozen = true;

                if (player.Character.PersonalizationStep != CharacterPersonalizationStep.Ready)
                    player.SetPosition(new Position(402.84396f, -996.9758f, -99.01465f), player.SessionId, true);
                else
                    await player.Spawnar();

                player.Emit("Server:SelecionarPersonagem",
                    (int)player.Character.PersonalizationStep,
                    (int)player.Character.Sex,
                    JsonSerializer.Serialize(player.Personalization));
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(RegistrarUsuario))]
        public async Task RegistrarUsuario(MyPlayer player, string usuario, string email, string senha, string senha2)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(senha2))
            {
                player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                return;
            }

            if (usuario.Contains(' '))
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

            if (!Functions.CheckEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            await using var context = new DatabaseContext();
            if (await context.Users.AnyAsync(x => x.Name.ToLower() == usuario.ToLower()))
            {
                player.Emit("Server:MostrarErro", $"Usuário {usuario} já existe.");
                return;
            }

            if (await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                return;
            }

            var user = new User()
            {
                Name = usuario,
                Email = email,
                Password = Functions.Encrypt(senha),
                RegisterIp = player.RealIp,
                LastAccessIp = player.RealIp,
                RegisterHardwareIdHash = player.HardwareIdHash,
                RegisterHardwareIdExHash = player.HardwareIdExHash,
                LastAccessHardwareIdHash = player.HardwareIdHash,
                LastAccessHardwareIdExHash = player.HardwareIdExHash,
                EmailConfirmationToken = string.IsNullOrWhiteSpace(Global.EmailHost) ? string.Empty : Functions.GenerateRandomString(6),
                DiscordConfirmationToken = Functions.GenerateRandomString(6),
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            _ = Functions.SendEmail(user.Email, "Confirmação de E-mail", $"Seu token de confirmação é <strong>{user.EmailConfirmationToken}</strong>.");

            await EntrarUsuario(player, usuario, senha);
        }

        [AsyncClientEvent(nameof(CriarPersonagem))]
        public async Task CriarPersonagem(MyPlayer player, int id, string nome, string sobrenome, string sexo, string dataNascimento, string historia)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(sexo)
                    || string.IsNullOrWhiteSpace(dataNascimento) || string.IsNullOrWhiteSpace(historia))
                {
                    player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                    return;
                }

                historia = historia.Trim();
                if (historia.Length < 500 || historia.Length > 4096)
                {
                    player.Emit("Server:MostrarErro", $"História deve possuir entre 500 até 4096 caracteres. Quantidade atual de caracteres: {historia.Length}.");
                    return;
                }

                var nomeCompleto = $"{nome.Trim()} {sobrenome.Trim()}";
                if (nomeCompleto.Length > 25)
                {
                    player.Emit("Server:MostrarErro", "Nome do personagem não pode possuir mais que 25 caracteres.");
                    return;
                }

                if (!DateTime.TryParse(dataNascimento, out DateTime dtNascimento) || dtNascimento == DateTime.MinValue)
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

                Character oldCharacter = null;
                await using var context = new DatabaseContext();
                if (id > 0)
                {
                    oldCharacter = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
                    if (string.IsNullOrWhiteSpace(oldCharacter.RejectionReason))
                    {
                        if (Global.Vehicles.Any(x => x.Vehicle.CharacterId == oldCharacter.Id))
                        {
                            player.Emit("Server:MostrarErro", "Não é possível prosseguir pois você possui veículos spawnados.");
                            return;
                        }

                        id = 0;
                    }
                }

                if (await context.Characters.AnyAsync(x => x.Name == nomeCompleto && x.Id != id))
                {
                    player.Emit("Server:MostrarErro", $"Personagem {nomeCompleto} já existe.");
                    return;
                }

                var personagem = new Character()
                {
                    Id = id,
                    Name = nomeCompleto,
                    UserId = player.User.Id,
                    BirthdayDate = dtNascimento,
                    RegisterIp = player.RealIp,
                    LastAccessIp = player.RealIp,
                    Model = sexo == "M" ? PedModel.FreemodeMale01 : PedModel.FreemodeFemale01,
                    RegisterHardwareIdHash = player.HardwareIdHash,
                    RegisterHardwareIdExHash = player.HardwareIdExHash,
                    LastAccessHardwareIdHash = player.HardwareIdHash,
                    LastAccessHardwareIdExHash = player.HardwareIdExHash,
                    History = historia,
                    Health = player.MaxHealth,
                    EvaluatorStaffUserId = player.User.Id == 1 ? 1 : null,
                    Sex = sexo == "M" ? CharacterSex.Man : CharacterSex.Woman,
                };

                if (oldCharacter != null)
                {
                    personagem.Bank = oldCharacter.Bank;
                    personagem.Savings = oldCharacter.Savings;
                }

                if (id == 0)
                    await context.Characters.AddAsync(personagem);
                else
                    context.Characters.Update(personagem);

                await context.SaveChangesAsync();

                if (oldCharacter != null)
                {
                    await player.GravarLog(LogType.NameChange, $"{oldCharacter.Name} [{oldCharacter.Id}] > {personagem.Name} [{personagem.Id}]", null);

                    await context.Database.ExecuteSqlRawAsync($"UPDATE {nameof(context.Properties)} SET {nameof(Property.CharacterId)} = {personagem.Id} WHERE {nameof(Property.CharacterId)} = {oldCharacter.Id}");
                    await context.Database.ExecuteSqlRawAsync($"UPDATE {nameof(context.Vehicles)} SET {nameof(Entities.Vehicle.CharacterId)} = {personagem.Id} WHERE {nameof(Entities.Vehicle.CharacterId)} = {oldCharacter.Id}");
                    await context.Database.ExecuteSqlRawAsync($"UPDATE {nameof(context.CharactersItems)} SET {nameof(CharacterItem.CharacterId)} = {personagem.Id} WHERE {nameof(CharacterItem.CharacterId)} = {oldCharacter.Id}");

                    foreach (var x in Global.Properties.Where(x => x.CharacterId == oldCharacter.Id))
                        x.CharacterId = personagem.Id;

                    oldCharacter.NameChangeStatus = CharacterNameChangeStatus.Realizado;
                    context.Characters.Update(oldCharacter);
                    await context.SaveChangesAsync();

                    player.User.NameChanges--;
                    context.Users.Update(player.User);
                    await context.SaveChangesAsync();
                }

                await Functions.SendStaffMessage("Uma nova aplicação de personagem foi recebida.", true, true);

                await ListarPersonagens(player);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(EnviarEmailConfirmacao))]
        public async Task EnviarEmailConfirmacao(MyPlayer player, string email)
        {
            if (email.Length > 100)
            {
                player.Emit("Server:MostrarErro", "E-mail não pode ter mais que 100 caracteres.");
                return;
            }

            if (!Functions.CheckEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            await using var context = new DatabaseContext();
            if (await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower() && x.Id != player.User.Id))
            {
                player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                return;
            }

            player.User.Email = email;
            context.Users.Update(player.User);
            await context.SaveChangesAsync();

            await Functions.SendEmail(email, "Confirmação de E-mail", $"Seu token de confirmação é <strong>{player.User.EmailConfirmationToken}</strong>.");
            player.Emit("Server:MostrarSucesso", "E-mail com o token de confirmação enviado.");
        }

        [AsyncClientEvent(nameof(ValidarTokenConfirmacao))]
        public async Task ValidarTokenConfirmacao(MyPlayer player, string token)
        {
            if (player.User.EmailConfirmationToken != token)
            {
                player.Emit("Server:MostrarErro", "Token de confirmação incorreto.");
                return;
            }

            using var context = new DatabaseContext();
            player.User.EmailConfirmationToken = string.Empty;
            context.Users.Update(player.User);
            await context.SaveChangesAsync();

            await VerificarRegistro(player);
        }

        [ClientEvent(nameof(ExibirPerguntas))]
        public void ExibirPerguntas(MyPlayer player)
        {
            var perguntas = Global.Questions.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
            var respostas = Global.QuestionsAnswers.OrderBy(x => Guid.NewGuid()).ToList();
            player.Emit("Server:ExibirPerguntas",
                JsonSerializer.Serialize(perguntas.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.CorrectQuestionAnswerId,
                    Answers = respostas.Where(y => y.QuestionId == x.Id)
                })));
        }

        [AsyncClientEvent(nameof(DeletarPersonagem))]
        public async Task DeletarPersonagem(MyPlayer player, int codigo)
        {
            await using var context = new DatabaseContext();
            var personagem = await context.Characters.FirstOrDefaultAsync(x => x.Id == codigo);
            personagem.DeletedDate = DateTime.Now;
            context.Characters.Update(personagem);
            await context.SaveChangesAsync();

            await player.GravarLog(LogType.ExclusaoPersonagem, codigo.ToString(), null);
            await ListarPersonagens(player);
        }

        [AsyncClientEvent(nameof(PunicoesAdministrativas))]
        public async Task PunicoesAdministrativas(MyPlayer player)
        {
            await using var context = new DatabaseContext();
            var punicoes = await context.Punishments
                .Include(x => x.Character)
                .Include(x => x.StaffUser)
                .Where(x => x.Character.UserId == player.User.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            player.Emit("Server:PunicoesAdministrativas", player.User.Name, DateTime.Now.ToString(),
                JsonSerializer.Serialize(punicoes.Select(x => new
                {
                    Character = x.Character.Name,
                    Date = x.Date.ToString(),
                    Type = x.Type.ToString(),
                    Duration = x.Type == PunishmentType.Ban ? (x.Duration > 0 ? $"{x.Duration} dia{(x.Duration != 1 ? "s" : string.Empty)}" : "Permanente") : string.Empty,
                    Staffer = x.StaffUser.Name,
                    x.Reason,
                })));
        }

        [AsyncClientEvent(nameof(AlterarEmail))]
        public async Task AlterarEmail(MyPlayer player, string email)
        {
            if (email.Length > 100)
            {
                player.Emit("Server:MostrarErro", "E-mail não pode ter mais que 100 caracteres.");
                return;
            }

            if (!Functions.CheckEmail(email))
            {
                player.Emit("Server:MostrarErro", "E-mail não está um formato válido.");
                return;
            }

            using var context = new DatabaseContext();
            if (await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                player.Emit("Server:MostrarErro", $"E-mail {email} já está sendo utilizado.");
                return;
            }

            player.User.EmailConfirmationToken = string.IsNullOrWhiteSpace(Global.EmailHost) ? string.Empty : Functions.GenerateRandomString(6);
            player.User.Email = email;
            context.Users.Update(player.User);
            await context.SaveChangesAsync();

            await Functions.SendEmail(email, "Confirmação de E-mail", $"Você alterou seu e-mail. Seu token de confirmação é<strong>{player.User.EmailConfirmationToken}</strong>.");

            if (!string.IsNullOrWhiteSpace(Global.EmailHost))
            {
                await VerificarRegistro(player);
                return;
            }

            player.Emit("Server:MostrarErro", "Você alterou seu e-mail.");
        }

        [AsyncClientEvent(nameof(AlterarSenha))]
        public async Task AlterarSenha(MyPlayer player, string senhaAntiga, string novaSenha, string novaSenha2)
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

            if (Functions.Encrypt(senhaAntiga) != player.User.Password)
            {
                player.Emit("Server:MostrarErro", "Sua senha atual não confere.");
                return;
            }

            using var context = new DatabaseContext();
            player.User.Password = Functions.Encrypt(novaSenha);
            context.Users.Update(player.User);
            await context.SaveChangesAsync();

            player.Emit("Server:MostrarSucesso", "Sua senha foi alterada.");
        }

        [AsyncClientEvent(nameof(EnviarEmailTokenRedefinirSenha))]
        public async Task EnviarEmailTokenRedefinirSenha(MyPlayer player, string email)
        {
            await using var context = new DatabaseContext();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                user.ResetPasswordToken = Functions.GenerateRandomString(6);
                context.Users.Update(user);
                await context.SaveChangesAsync();

                _ = Functions.SendEmail(email, "Recuperação da Senha", $"Seu token para redefinir a senha é <strong>{user.ResetPasswordToken}</strong>.");
            }
            else
            {
                await player.GravarLog(LogType.EsqueciMinhaSenhaFalha, $"E-mail: {email}", null);
            }

            player.Emit("Server:RedefinirSenha", email, user?.Id ?? 0);
        }

        [AsyncClientEvent(nameof(RedefinirSenha))]
        public async Task RedefinirSenha(MyPlayer player, int codigo, string token)
        {
            using var context = new DatabaseContext();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == codigo && x.ResetPasswordToken == token);
            if (user != null)
            {
                var senha = Functions.GenerateRandomString(10);
                user.ResetPasswordToken = string.Empty;
                user.Password = Functions.Encrypt(senha);
                context.Users.Update(user);
                await context.SaveChangesAsync();

                _ = Functions.SendEmail(user.Email, "Recuperação da Senha", $"Sua nova senha é <strong>{senha}</strong>.");
            }

            player.Emit("Server:MostrarSucesso", "Caso o e-mail e o token correspondam, um e-mail será enviado contendo sua nova senha. Verifique também sua caixa de lixo eletrônico.");
        }
    }
}