using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
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

            await player.GravarLog(LogType.Company, $"Gravar | {Functions.Serialize(company)}", null);
            player.Emit("Companies", true, GetCompaniesByCharacterHTML(player));
        }

        [AsyncClientEvent(nameof(CompanyEmployees))]
        public static async Task CompanyEmployees(MyPlayer player, int id)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);

            var companyFlagsJSON = Functions.Serialize(
                Enum.GetValues(typeof(CompanyFlag)).Cast<CompanyFlag>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("Server:CloseView");
            player.Emit("CompanyCharacters", false,
                await GetCompanyCharactersHTML(company.Id),
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

            await player.GravarLog(LogType.Company, $"{(company.Open ? "Abrir" : "fechou")} Empresa {id}", null);
            player.EmitStaffShowMessage($"Você {(company.Open ? "abriu" : "fechou")} a empresa {company.Name}.", true);
            player.Emit("Companies", true, GetCompaniesByCharacterHTML(player));
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
            player.EmitStaffShowMessage(string.Empty, true);
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

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == characterSessionId);
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
                Value = [company.Id.ToString()],
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.Company);
            target.Invites.Add(invite);

            player.EmitStaffShowMessage($"Você convidou {target.Character.Name} para {company.Name}.", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} convidou você para a empresa {company.Name}. (/ac {(int)invite.Type} para aceitar ou /rc {(int)invite.Type} para recusar)");

            await player.GravarLog(LogType.Company, $"Convidar Empresa {companyId}", target);
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

            var companyFlags = Functions.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();
            companyCharacterTarget.FlagsJSON = Functions.Serialize(companyFlags);
            context.CompaniesCharacters.Update(companyCharacterTarget);
            await context.SaveChangesAsync();

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == characterId);
            target?.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na empresa {company.Name}.");

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

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == characterId);
            target?.SendMessage(MessageType.Success, $"{player.User.Name} expulsou você da empresa {company.Name}.");

            player.EmitStaffShowMessage($"Você expulsou o personagem {characterId} da empresa.", true);
            await player.GravarLog(LogType.Company, $"Expulsar Empresa {companyId} {characterId}", target);

            player.Emit("CompanyCharacters", true,
                await GetCompanyCharactersHTML(company.Id),
                companyCharacter?.FlagsJSON ?? "[]", company.CharacterId == player.Character.Id);
        }

        [Command("empresa")]
        public static void CMD_empresa(MyPlayer player)
        {
            if (player.Companies.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Você não está em nenhuma empresa.");
                return;
            }

            player.Emit("Companies", false, GetCompaniesByCharacterHTML(player));
        }

        private static string GetCompaniesByCharacterHTML(MyPlayer player)
        {
            var html = string.Empty;
            if (player.Companies.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='11'>Não há empresas criadas.</td></tr>";
            }
            else
            {
                foreach (var company in player.Companies.OrderByDescending(x => x.Id))
                {
                    var companyCharacter = company.Characters!.FirstOrDefault(x => x.CharacterId == player.Character.Id);

                    var htmlOptions = string.Empty;

                    if (company.CharacterId == player.Character.Id || (companyCharacter?.GetFlags()?.Contains(CompanyFlag.Open) ?? false))
                    {
                        htmlOptions += $@"<button onclick='openClose({company.Id})' type='button' class='btn btn-dark btn-sm'>ABRIR/FECHAR</button>";

                        if (company.GetIsOpen())
                            htmlOptions += $@" <button onclick='announce({company.Id})' type='button' class='btn btn-dark btn-sm'>ANUNCIAR</button>";
                    }

                    html += $@"<tr class='pesquisaitem'>
                        <td>{company.Id}</td>
                        <td>{company.Name}</td>
                        <td>${company.WeekRentValue:N0}</td>
                        <td>{company.RentPaymentDate}</td>
                        <td><span style='color:#{company.Color}'>#{company.Color}</span></td>
                        <td>{company.BlipType}</td>
                        <td>{company.BlipColor}</td>
                        <td class='text-center'>{(company.GetIsOpen() ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{company.Id}' type='hidden' value='{Functions.Serialize(company)}' />
                            {(company.CharacterId == player.Character.Id ? $"<button onclick='edit({company.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>" : string.Empty)}
                            <button onclick='employees({company.Id})' type='button' class='btn btn-dark btn-sm'>FUNCIONÁRIOS</button>
                            {htmlOptions}
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        private static async Task<string> GetCompanyCharactersHTML(Guid companyId)
        {
            await using var context = new DatabaseContext();
            var characters = (await context.CompaniesCharacters
                .Where(x => x.CompanyId == companyId)
                .Include(x => x.Character)
                .ThenInclude(x => x.User)
                .ToListAsync())
                .Select(x => new
                {
                    CompanyCharacter = x,
                    OnlineCharacter = Global.SpawnedPlayers.FirstOrDefault(y => y.Character.Id == x.CharacterId),
                })
                .OrderByDescending(x => x.OnlineCharacter != null)
                .ThenBy(x => x.CompanyCharacter.Character.Name);

            var html = string.Empty;
            if (!characters.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há funcionários na empresa.</td></tr>";
            }
            else
            {
                foreach (var character in characters)
                {
                    var online = character.OnlineCharacter != null ?
                        $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>ONLINE</span>"
                        :
                        $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>OFFLINE</span>";

                    html += $@"<tr class='pesquisaitem'>
                        <td>{character.CompanyCharacter.Character.Name} [{character.CompanyCharacter.Character.Id}]</td>
                        <td>{character.CompanyCharacter.Character.User.Name} [{character.CompanyCharacter.Character.UserId}]</td>
                        <td>{character.CompanyCharacter.Character.LastAccessDate}</td>
                        <td class='text-center'>{online}</td>
                        <td class='text-center tdOptions'>
                            <input id='jsonMember{character.CompanyCharacter.Character.Id}' type='hidden' value='{Functions.Serialize(new { character.CompanyCharacter.Character.Name, character.CompanyCharacter.FlagsJSON })}' />
                            <button onclick='edit({character.CompanyCharacter.Character.Id})' type='button' class='btn btn-dark btn-sm editMember'>EDITAR</button>
                            <button onclick='remove(this, {character.CompanyCharacter.Character.Id})' type='button' class='btn btn-danger btn-sm removeMember'>EXPULSAR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        [Command("alugarempresa")]
        public static async Task CMD_alugarempresa(MyPlayer player)
        {
            var company = Global.Companies.Where(x => !x.CharacterId.HasValue
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (company == null)
            {
                player.SendMessage(MessageType.Error, "Você não está perto de uma empresa disponível para alugar.");
                return;
            }

            if (player.Money < company.WeekRentValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, company.WeekRentValue));
                return;
            }

            await player.RemoveStackedItem(ItemCategory.Money, company.WeekRentValue);

            company.CharacterId = player.Character.Id;
            company.RentPaymentDate = DateTime.Now.AddDays(7);
            company.RemoveIdentifier();

            await using var context = new DatabaseContext();
            context.Companies.Update(company);
            await context.SaveChangesAsync();

            await player.GravarLog(LogType.Company, $"/alugarempresa {company.Id} {company.WeekRentValue}", null);

            player.SendMessage(MessageType.Success, $"Você alugou a empresa {company.Name} [{company.Id}] por 7 dias por ${company.WeekRentValue:N0}.");
            player.SendMessage(MessageType.Success, $"O próximo pagamento será em {company.RentPaymentDate} e será debitado da sua conta bancária. Se você não possuir este valor, a empresa será retirada do seu nome.");
        }
    }
}