using AltV.Net;
using AltV.Net.Async;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class CompanyScript : IScript
    {
        [AsyncClientEvent(nameof(CompanySave))]
        public static async Task CompanySave(MyPlayer player, int id, string color, int blipType, int blipColor)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company?.CharacterId != player.Character.Id)
                return;

            if (color.Length != 6)
            {
                player.EmitStaffShowMessage("Cor deve possuir 6 caracteres.");
                return;
            }

            if (blipType < 1 || blipType > 744)
            {
                player.EmitStaffShowMessage("Tipo do Blip deve ser entre 1 e 744.");
                return;
            }

            if (blipColor < 1 || blipColor > 85)
            {
                player.EmitStaffShowMessage("Cor do Blip deve ser entre 1 e 85.");
                return;
            }

            company.Color = color;
            company.BlipType = Convert.ToUInt16(blipType);
            company.BlipColor = Convert.ToByte(blipColor);

            await using var context = new DatabaseContext();
            context.Companies.Update(company);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Empresa editada.", true);

            await player.GravarLog(LogType.Empresa, $"Gravar | {JsonSerializer.Serialize(company)}", null);
            player.Emit("Companies", true, Functions.GetCompaniesByCharacterHTML(player));
        }

        [AsyncClientEvent(nameof(CompanyEmployees))]
        public static async Task CompanyEmployees(MyPlayer player, int id)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);

            var companyFlagsJSON = JsonSerializer.Serialize(
                Enum.GetValues(typeof(CompanyFlag)).Cast<CompanyFlag>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("Server:CloseView");
            player.Emit("CompanyCharacters", false,
                await Functions.GetCompanyCharactersHTML(company.Id),
                companyCharacter?.FlagsJSON ?? "[]", company.CharacterId == player.Character.Id,
                company.Id, company.Name, companyFlagsJSON);
        }

        [AsyncClientEvent(nameof(CompanyOpenClose))]
        public static async Task CompanyOpenClose(MyPlayer player, int id)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            if (company.Color.Length != 6)
            {
                player.EmitStaffShowMessage("Cor deve possuir 6 caracteres.");
                return;
            }

            if (company.BlipType < 1 || company.BlipType > 744)
            {
                player.EmitStaffShowMessage("Tipo do Blip deve ser entre 1 e 744.");
                return;
            }

            if (company.BlipColor < 1 || company.BlipColor > 85)
            {
                player.EmitStaffShowMessage("Cor do Blip deve ser entre 1 e 85.");
                return;
            }

            company.ToggleOpen();

            await player.GravarLog(LogType.Empresa, $"{(company.Open ? "Abrir" : "fechou")} Empresa {id}", null);
            player.EmitStaffShowMessage($"Você {(company.Open ? "abriu" : "fechou")} a empresa {company.Name}.", true);
            player.Emit("Companies", true, Functions.GetCompaniesByCharacterHTML(player));
        }

        [AsyncClientEvent(nameof(CompanyAnnounce))]
        public static async Task CompanyAnnounce(MyPlayer player, int id, string message)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            if (!company.Open)
            {
                player.EmitStaffShowMessage("A empresa está fechada.");
                return;
            }

            await company.Announce(player, message);
            player.EmitStaffShowMessage(null, true);
        }

        [AsyncClientEvent(nameof(CompanyCharacterInvite))]
        public static async Task CompanyCharacterInvite(MyPlayer player, int companyId, int characterSessionId)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == companyId);
            if (company == null)
                return;

            var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);
            if (company.CharacterId != player.Character.Id
                && !(companyCharacter?.Flags?.Contains(CompanyFlag.InviteCharacter) ?? false))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.SessionId == characterSessionId);
            if (target == null)
            {
                player.EmitStaffShowMessage($"Nenhum personagem online com o ID {characterSessionId}.");
                return;
            }

            if (company.CharacterId == target.Character.Id || company.Characters.Any(x => x.CharacterId == target.Character.Id))
            {
                player.EmitStaffShowMessage("Personagem já está nessa empresa.");
                return;
            }

            var invite = new Invite()
            {
                Type = InviteType.Company,
                SenderCharacterId = player.Character.Id,
                Value = new string[] { company.Id.ToString() },
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.Company);
            target.Invites.Add(invite);

            player.EmitStaffShowMessage($"Você convidou {target.Character.Name} para {company.Name}.", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} convidou você para a empresa {company.Name}. (/ac {(int)invite.Type} para aceitar ou /rc {(int)invite.Type} para recusar)");

            await player.GravarLog(LogType.Empresa, $"Convidar Empresa {companyId}", target);
        }

        [AsyncClientEvent(nameof(CompanyCharacterSave))]
        public static async Task CompanyCharacterSave(MyPlayer player, int companyId, int characterId, string flagsJSON)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == companyId);
            if (company == null)
                return;

            var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);
            if (company.CharacterId != player.Character.Id
                && !(companyCharacter?.Flags?.Contains(CompanyFlag.EditCharacter) ?? false))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var companyCharacterTarget = company.Characters.FirstOrDefault(x => x.CharacterId == characterId);
            if (companyCharacterTarget == null)
            {
                player.EmitStaffShowMessage($"Personagem {characterId} não está na empresa {companyId}.");
                return;
            }

            await using var context = new DatabaseContext();

            var companyFlags = JsonSerializer.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();
            companyCharacterTarget.FlagsJSON = JsonSerializer.Serialize(companyFlags);
            context.CompaniesCharacters.Update(companyCharacterTarget);
            await context.SaveChangesAsync();

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == characterId);
            if (target != null)
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na empresa {company.Name}.");

            player.EmitStaffShowMessage($"Você alterou as informações do personagem {characterId} na empresa.", true);
            await player.GravarLog(LogType.Faction, $"Salvar Funcionário Empresa {companyId} {characterId} {flagsJSON}", target);

            player.Emit("CompanyCharacters", true,
                await Functions.GetCompanyCharactersHTML(company.Id),
                companyCharacter?.FlagsJSON ?? "[]", company.CharacterId == player.Character.Id);
        }

        [AsyncClientEvent(nameof(CompanyCharacterRemove))]
        public static async Task CompanyCharacterRemove(MyPlayer player, int companyId, int characterId)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == companyId);
            if (company == null)
                return;

            var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);
            if (company.CharacterId != player.Character.Id
                && !(companyCharacter?.Flags?.Contains(CompanyFlag.RemoveCharacter) ?? false))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var companyCharacterTarget = company.Characters.FirstOrDefault(x => x.CharacterId == characterId);
            if (companyCharacterTarget == null)
            {
                player.EmitStaffShowMessage($"Personagem {characterId} não está na empresa {companyId}.");
                return;
            }

            await using var context = new DatabaseContext();
            context.CompaniesCharacters.Remove(companyCharacterTarget);
            await context.SaveChangesAsync();
            company.Characters.Remove(companyCharacterTarget);

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == characterId);
            if (target != null)
                target.SendMessage(MessageType.Success, $"{player.User.Name} expulsou você da empresa {company.Name}.");

            player.EmitStaffShowMessage($"Você expulsou o personagem {characterId} da empresa.", true);
            await player.GravarLog(LogType.Empresa, $"Expulsar Empresa {companyId} {characterId}", target);

            player.Emit("CompanyCharacters", true,
                await Functions.GetCompanyCharactersHTML(company.Id),
                companyCharacter?.FlagsJSON ?? "[]", company.CharacterId == player.Character.Id);
        }
    }
}