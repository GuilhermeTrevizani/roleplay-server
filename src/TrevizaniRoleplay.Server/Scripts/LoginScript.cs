using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class LoginScript : IScript
    {
        [AsyncClientEvent(nameof(ValidateDiscordToken))]
        public static async Task ValidateDiscordToken(MyPlayer player, string token)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var res = await httpClient.GetFromJsonAsync<DiscordResponse>("https://discordapp.com/api/users/@me");
                if (res == null)
                {
                    player.Emit("Server:MostrarErro", "Usuário do Discord não encontrado.");
                    return;
                }
                res.Username = res.Discriminator == "0" ? res.Username : $"{res.Username}#{res.Discriminator}";

                await using var context = new DatabaseContext();
                var user = context.Users.FirstOrDefault(x => x.DiscordId == res.Id);
                if (user == null)
                {
                    var hasUsers = context.Users.Any();
                    user = new();
                    user.Create(res.Id, res.Username, res.Global_Name, player.RealIp, player.HardwareIdHash, player.HardwareIdExHash,
                        hasUsers ? UserStaff.None : UserStaff.Founder, hasUsers ? "[]" : Functions.Serialize(Enum.GetValues<StaffFlag>()));
                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();
                }
                else
                {
                    var banishment = await context.Banishments
                        .Include(x => x.StaffUser)
                        .FirstOrDefaultAsync(x => x.UserId == user.Id);
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

                            player.Emit("Server:BaseHTML", Functions.GetBaseHTML("Banimento", $"Você está banido{strBan}<br/>Data: <strong>{banishment.Date}</strong><br/>Motivo: <strong>{banishment.Reason}</strong><br/>Staffer: <strong>{banishment.StaffUser!.Name}</strong>"));
                            return;
                        }
                    }

                    if (Global.AllPlayers.Any(x => x.User?.Id == user.Id))
                    {
                        player.Emit("Server:MostrarErro", "Usuário já está logado.");
                        return;
                    }
                }

                player.User = user;
                player.User.UpdateLastAccess(player.RealIp, player.HardwareIdHash, player.HardwareIdExHash, res.Username, res.Global_Name);
                context.Users.Update(player.User);
                await context.SaveChangesAsync();
                player.StaffFlags = Functions.Deserialize<List<StaffFlag>>(player.User.StaffFlagsJSON);

                if (player.User.AnsweredQuestions)
                {
                    await ListarPersonagens(player);
                }
                else
                {
                    var perguntas = Global.Questions.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
                    var respostas = Global.QuestionsAnswers.OrderBy(x => Guid.NewGuid()).ToList();
                    player.Emit("Server:ExibirPerguntas",
                        Functions.Serialize(perguntas.Select(x => new
                        {
                            x.Id,
                            x.Name,
                            x.CorrectQuestionAnswerId,
                            Answers = respostas.Where(y => y.QuestionId == x.Id)
                        })));
                }
            }
            catch (Exception ex)
            {
                player.Emit("Server:MostrarErro", ex.Message);
                Alt.LogError(ex.Message);
            }
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
            player.User.SetAnsweredQuestions();
            context.Users.Update(player.User);
            await context.SaveChangesAsync();

            player.Emit("Server:ListarPersonagens", player.User.Name,
                Functions.Serialize((await context.Characters
                    .Where(x => x.UserId == player.User.Id
                        && x.NameChangeStatus != CharacterNameChangeStatus.Done
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
                    opcoes = $"<button class='btn btn-primary btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem(`{x.Id}`, false);'>LOGAR</button>";
                else
                    opcoes = $"<button class='btn btn-dark btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem(`{x.Id}`, false);'>REFAZER APLICAÇÃO</button>";
            }
            opcoes += x.NameChangeStatus == CharacterNameChangeStatus.Allowed && u.NameChanges > 0 && string.IsNullOrWhiteSpace(x.RejectionReason) && x.EvaluatorStaffUserId.HasValue ? $" <button class='btn btn-dark btn-sm mar-sm-top btn-selecionarpersonagem{x.Id}' onclick='selecionarPersonagem(`{x.Id}`, true);'>ALTERAR NOME</button>" : string.Empty;
            opcoes += $" <button class='btn btn-danger btn-sm mar-sm-top' onclick='deletarPersonagem(`{x.Id}`);' style='background-color:#d12c0f;color:#fff;'>DELETAR</button>";
            return opcoes;
        }

        [AsyncClientEvent(nameof(SelecionarPersonagem))]
        public async Task SelecionarPersonagem(MyPlayer player, string idString, bool namechange)
        {
            try
            {
                var id = idString.ToGuid();
                await using var context = new DatabaseContext();
                var character = await context.Characters
                    .Where(x => x.Id == id && x.UserId == player.User.Id)
                    .FirstOrDefaultAsync();
                if (character == null)
                {
                    player.Emit("Server:MostrarErro", "Personagem não encontrado.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(character.RejectionReason) || namechange)
                {
                    var evaluatorStaffUser = await context.Users.FirstOrDefaultAsync(x => x.Id == character.EvaluatingStaffUserId);
                    var nome = character.Name.Split(' ');
                    player.Emit("Server:CriarPersonagem", character.Id, nome.FirstOrDefault(), nome.LastOrDefault(),
                        character.Sex == CharacterSex.Man ? "M" : "F", character.BirthdayDate.ToShortDateString(),
                        character.History, character.RejectionReason, evaluatorStaffUser!.Name);
                    return;
                }

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
                        player.Emit("Server:MostrarErro", $"Você está banido{strBan}<br/>Data: <strong>{banishment.Date}</strong><br/>Motivo: <strong>{banishment.Reason}</strong><br/>Staffer: <strong>{banishment.StaffUser!.Name}</strong>", $".btn-selecionarpersonagem{character.Id}");
                        return;
                    }
                }

                player.Character = character;
                player.Character.UpdateLastAccess(player.RealIp, player.HardwareIdHash, player.HardwareIdExHash);

                player.IPLs = Functions.Deserialize<List<string>>(player.Character.IPLsJSON);
                player.SetarIPLs();
                player.Model = player.Character.Model;
                player.SetWeather((uint)Global.WeatherInfo.WeatherType);
                player.Wounds = Functions.Deserialize<List<Wound>>(player.Character.WoundsJSON);
                player.Items = await context.CharactersItems.Where(x => x.CharacterId == player.Character.Id).ToListAsync();
                player.FactionFlags = Functions.Deserialize<List<FactionFlag>>(player.Character.FactionFlagsJSON);
                await player.SetarRoupas();

                if (!string.IsNullOrWhiteSpace(player.Character.PersonalizationJSON))
                    player.Personalization = Functions.Deserialize<Personalization>(player.Character.PersonalizationJSON);

                if (player.Personalization.Structure.Count == 0)
                    player.Personalization.Structure = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

                if (player.Personalization.OpacityOverlays.Count == 0)
                    player.Personalization.OpacityOverlays =
                    [
                        new(0),
                        new(3),
                        new(6),
                        new(7),
                        new(9),
                        new(11)
                    ];

                if (player.Personalization.ColorOverlays.Count == 0)
                    player.Personalization.ColorOverlays =
                    [
                        new(4),
                        new(5),
                        new(8)
                    ];

                player.ClearBloodDamage();
                player.SetarPersonalizacao(player.Personalization);

                await player.GravarLog(LogType.Entrance, string.Empty, null);

                player.SetDateTime(DateTime.Now.AddHours(2));
                player.Emit("SyncWeather", Global.WeatherInfo.WeatherType.ToString().ToUpper());

                if (player.Character.PersonalizationStep != CharacterPersonalizationStep.Ready)
                {
                    player.SetPosition(new Position(402.83078f, -996.9758f, -99.01465f), player.SessionId, true);
                    player.Invincible = true;
                }
                else
                {
                    await player.Spawnar();
                }

                player.Emit("Server:SelecionarPersonagem",
                    (int)player.Character.PersonalizationStep,
                    (int)player.Character.Sex,
                    Functions.Serialize(player.Personalization));
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(CriarPersonagem))]
        public async Task CriarPersonagem(MyPlayer player, string idString, string nome, string sobrenome, string sex, string dataNascimento, string history)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(sex)
                    || string.IsNullOrWhiteSpace(dataNascimento) || string.IsNullOrWhiteSpace(history))
                {
                    player.Emit("Server:MostrarErro", "Verifique se todos os campos foram preenchidos corretamente.");
                    return;
                }

                history = history.Trim();
                if (history.Length < 500 || history.Length > 4096)
                {
                    player.Emit("Server:MostrarErro", $"História deve possuir entre 500 até 4096 caracteres. Quantidade atual de caracteres: {history.Length}.");
                    return;
                }

                var nomeCompleto = $"{nome.Trim()} {sobrenome.Trim()}";
                if (nomeCompleto.Length > 25)
                {
                    player.Emit("Server:MostrarErro", "Nome do personagem não pode possuir mais que 25 caracteres.");
                    return;
                }

                if (!DateTime.TryParse(dataNascimento, out DateTime birthdayDate) || birthdayDate == DateTime.MinValue)
                {
                    player.Emit("Server:MostrarErro", "Data de Nascimento não foi informada corretamente.");
                    return;
                }

                var anos = (DateTime.Now.Date - birthdayDate).TotalDays / 365;
                if (anos < 18 || anos > 90)
                {
                    player.Emit("Server:MostrarErro", "Personagem precisa ter entre 18 e 90 anos.");
                    return;
                }

                var isNew = true;
                Character? oldCharacter = null;
                await using var context = new DatabaseContext();
                var id = idString.ToGuid();
                if (id.HasValue)
                {
                    oldCharacter = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
                    if (oldCharacter == null)
                    {
                        player.Emit("Server:MostrarErro", "Personagem antigo não encontrado.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(oldCharacter.RejectionReason))
                    {
                        if (Global.Vehicles.Any(x => x.VehicleDB.CharacterId == oldCharacter.Id))
                        {
                            player.Emit("Server:MostrarErro", "Não é possível prosseguir pois você possui veículos spawnados.");
                            return;
                        }
                    }
                    else
                    {
                        isNew = false;
                    }
                }

                if (await context.Characters.AnyAsync(x => x.Name == nomeCompleto && x.Id != id))
                {
                    player.Emit("Server:MostrarErro", $"Personagem {nomeCompleto} já existe.");
                    return;
                }

                var character = new Character();
                if (isNew)
                {
                    character.Create(nomeCompleto, birthdayDate, history, sex == "M" ? CharacterSex.Man : CharacterSex.Woman,
                        player.User.Id, player.RealIp, (uint)(sex == "M" ? PedModel.FreemodeMale01 : PedModel.FreemodeFemale01),
                        player.HardwareIdHash, player.HardwareIdExHash, player.MaxHealth,
                        player.User.Staff == UserStaff.Founder ? player.User.Id : null);
                    await context.Characters.AddAsync(character);
                }
                else
                {
                    character.SetBankAndSavings(oldCharacter!.Bank, oldCharacter.Savings);
                    context.Characters.Update(character);
                }

                await context.SaveChangesAsync();

                if (oldCharacter != null)
                {
                    await player.GravarLog(LogType.NameChange, $"{oldCharacter.Name} [{oldCharacter.Id}] > {character.Name} [{character.Id}]", null);

                    await context.Properties.Where(x => x.CharacterId == oldCharacter.Id).ExecuteUpdateAsync(x => x.SetProperty(y => y.CharacterId, character.Id));
                    await context.Vehicles.Where(x => x.CharacterId == oldCharacter.Id).ExecuteUpdateAsync(x => x.SetProperty(y => y.CharacterId, character.Id));
                    await context.CharactersItems.Where(x => x.CharacterId == oldCharacter.Id).ExecuteUpdateAsync(x => x.SetProperty(y => y.CharacterId, character.Id));
                    await context.Companies.Where(x => x.CharacterId == oldCharacter.Id).ExecuteUpdateAsync(x => x.SetProperty(y => y.CharacterId, character.Id));
                    await context.CompaniesCharacters.Where(x => x.CharacterId == oldCharacter.Id).ExecuteUpdateAsync(x => x.SetProperty(y => y.CharacterId, character.Id));

                    foreach (var property in Global.Properties.Where(x => x.CharacterId == oldCharacter.Id))
                        property.SetOwner(character.Id);

                    oldCharacter.SetNameChangeStatus(CharacterNameChangeStatus.Done);
                    context.Characters.Update(oldCharacter);
                    await context.SaveChangesAsync();

                    player.User.RemoveNameChange();
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

        [AsyncClientEvent(nameof(DeletarPersonagem))]
        public async Task DeletarPersonagem(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character != null)
            {
                character.Delete();
                context.Characters.Update(character);
                await context.SaveChangesAsync();
                await player.GravarLog(LogType.CharacterDelete, id.ToString(), null);
            }
            await ListarPersonagens(player);
        }

        [AsyncClientEvent(nameof(PunicoesAdministrativas))]
        public async Task PunicoesAdministrativas(MyPlayer player)
        {
            await using var context = new DatabaseContext();
            var punicoes = await context.Punishments
                .Include(x => x.Character)
                .Include(x => x.StaffUser)
                .Where(x => x.Character!.UserId == player.User.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            player.Emit("Server:PunicoesAdministrativas", player.User.Name, DateTime.Now.ToString(),
                Functions.Serialize(punicoes.Select(x => new
                {
                    Character = x.Character!.Name,
                    Date = x.Date.ToString(),
                    Type = x.Type.ToString(),
                    Duration = x.Type == PunishmentType.Ban ? (x.Duration > 0 ? $"{x.Duration} dia{(x.Duration != 1 ? "s" : string.Empty)}" : "Permanente") : string.Empty,
                    Staffer = x.StaffUser!.Name,
                    x.Reason,
                })));
        }
    }
}