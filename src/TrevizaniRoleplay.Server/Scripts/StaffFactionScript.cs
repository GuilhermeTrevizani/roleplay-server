using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffFactionScript : IScript
    {
        [Command("faccoes")]
        public static void CMD_faccoes(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = Functions.Serialize(
                Enum.GetValues(typeof(FactionType))
                .Cast<FactionType>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("StaffFactions", false, GetFactionsHTML(), jsonTypes);
        }

        [AsyncClientEvent(nameof(StaffFactionSave))]
        public static async Task StaffFactionSave(MyPlayer player, string idString, string name, int type, string color,
            int slots, string chatColor)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(FactionType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            if (slots < 0)
            {
                player.EmitStaffShowMessage("Slots deve ser maior ou igual a zero.");
                return;
            }

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            var faction = new Faction();
            if (isNew)
            {
                faction.Create(name, (FactionType)type, color, slots, chatColor);
            }
            else
            {
                faction = Global.Factions.FirstOrDefault(x => x.Id == id);
                if (faction == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                faction.Update(name, (FactionType)type, color, slots, chatColor);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.Factions.AddAsync(faction);
            else
                context.Factions.Update(faction);

            await context.SaveChangesAsync();

            if (isNew)
                Global.Factions.Add(faction);

            player.EmitStaffShowMessage($"Facção {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Facção | {Functions.Serialize(faction)}", null);

            var html = GetFactionsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffFactions", true, html);
        }

        [ClientEvent(nameof(StaffFactionShowRanks))]
        public static void StaffFactionShowRanks(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");
            var id = idString.ToGuid();
            var htmlFactionRanks = Functions.GetFactionRanksHTML(id.Value);
            player.Emit("StaffShowFactionRanks",
                false,
                htmlFactionRanks,
                idString,
                Global.Factions.FirstOrDefault(x => x.Id == id)?.Name);
        }

        [AsyncClientEvent(nameof(StaffFactionRankSave))]
        public static async Task StaffFactionRankSave(MyPlayer player, string factionIdString, string factionRankIdString,
            string name, int salary)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (salary < 0)
            {
                player.EmitStaffShowMessage("Salário deve ser maior que 0.");
                return;
            }

            var factionId = factionIdString.ToGuid();
            var factionRankId = factionRankIdString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(factionRankIdString);
            var factionRank = new FactionRank();
            if (isNew)
            {
                factionRank.Create(factionId!.Value,
                    Global.FactionsRanks.Where(x => x.FactionId == factionId).Select(x => x.Position).DefaultIfEmpty(0).Max() + 1,
                    name, salary);
            }
            else
            {
                factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
                if (factionRank == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                factionRank.Update(name, salary);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.FactionsRanks.AddAsync(factionRank);
            else
                context.FactionsRanks.Update(factionRank);

            await context.SaveChangesAsync();

            if (isNew)
                Global.FactionsRanks.Add(factionRank);

            player.EmitStaffShowMessage($"Rank {(isNew ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Rank | {Functions.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId!.Value);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionRankRemove))]
        public static async Task StaffFactionRankRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == id);
            if (factionRank == null)
                return;

            await using var context = new DatabaseContext();

            if (await context.Characters.AnyAsync(x => x.FactionRankId == factionRank.Id))
            {
                player.EmitStaffShowMessage($"Não é possível remover o rank {factionRank.Id} pois existem personagens nele.");
                return;
            }

            context.FactionsRanks.Remove(factionRank);
            await context.SaveChangesAsync();
            Global.FactionsRanks.Remove(factionRank);

            player.EmitStaffShowMessage($"Rank {factionRank.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Rank | {Functions.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionRank.FactionId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionRankOrder))]
        public static async Task StaffFactionRankOrder(MyPlayer player, string idString, string ranksJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();

            var id = idString.ToGuid();
            var factionRanks = Global.FactionsRanks.Where(x => x.FactionId == id);
            var ranks = Functions.Deserialize<List<FactionRank>>(ranksJSON);
            foreach (var rank in ranks)
            {
                var factionRank = factionRanks.FirstOrDefault(x => x.Id == rank.Id);
                factionRank.SetPosition(rank.Position);
                context.FactionsRanks.Update(factionRank);
            }

            await context.SaveChangesAsync();
            player.EmitStaffShowMessage("Ranks ordenados.");

            await player.GravarLog(LogType.Faction, $"Ordenar Ranks | {ranksJSON}", null);

            var html = Functions.GetFactionRanksHTML(id.Value);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionShowMembers))]
        public static async Task StaffFactionShowMembers(MyPlayer player, string idString)
        {
            player.Emit("Server:CloseView");
            var id = idString.ToGuid();
            var ranksJson = Functions.Serialize(Global.FactionsRanks.Where(x => x.FactionId == id).OrderBy(x => x.Position));

            var faction = Global.Factions.FirstOrDefault(x => x.Id == id);

            var flagsJson = Functions.Serialize(
                faction.GetFlags()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            var htmlFactionMembers = await Functions.GetFactionMembersHTML(id.Value);
            player.Emit("StaffShowFactionMembers",
                false,
                htmlFactionMembers,
                idString,
                faction.Name,
                faction.Government,
                ranksJson,
                flagsJson);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberInvite))]
        public static async Task StaffFactionMemberInvite(MyPlayer player, string factionIdString, int characterSessionId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionId = factionIdString.ToGuid();
            var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);
            if (faction == null)
                return;

            await using var context = new DatabaseContext();
            if (faction.Slots > 0)
            {
                var members = await context.Characters.CountAsync(x => x.FactionId == faction.Id && !x.DeathDate.HasValue && !x.DeletedDate.HasValue);
                if (members >= faction.Slots)
                {
                    player.EmitStaffShowMessage($"Facção atingiu o máximo de slots ({faction.Slots}).");
                    return;
                }
            }

            var rank = Global.FactionsRanks.Where(x => x.FactionId == faction.Id).MinBy(x => x.Position);
            if (rank == null)
            {
                player.EmitStaffShowMessage("Facção não possui ranks.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == characterSessionId);
            if (target == null)
            {
                player.EmitStaffShowMessage($"Nenhum personagem online com o ID {characterSessionId}.");
                return;
            }

            if (target.Character.FactionId.HasValue)
            {
                player.EmitStaffShowMessage("Personagem já está em uma facção.");
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.Faccao,
                SenderCharacterId = player.Character.Id,
                Value = [faction.Id.ToString(), rank.Id.ToString()],
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.Faccao);
            target.Invites.Add(convite);

            player.EmitStaffShowMessage($"Você convidou {target.Character.Name} para a facção.", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} convidou você para a facção {faction.Name}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");

            await player.GravarLog(LogType.Staff, $"Convidar Facção {factionId}", target);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberSave))]
        public static async Task StaffFactionMemberSave(MyPlayer player, string factionIdString, string characterIdString, string factionRankIdString,
            int badge, string flagsJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var factionId = factionIdString.ToGuid();
            var characterId = characterIdString.ToGuid();
            var factionRankId = factionRankIdString.ToGuid();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == characterId);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Nenhum jogador encontrado com o ID {characterId}.");
                return;
            }

            if (character.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Jogador não pertence a esta facção.");
                return;
            }

            var rank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            if (rank?.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Rank {factionRankId} não existe na facção {factionId}.");
                return;
            }

            if (badge < 0)
            {
                player.EmitStaffShowMessage($"Distintivo deve ser maior que zero.");
                return;
            }

            if (badge > 0)
            {
                var characterTarget = await context.Characters.FirstOrDefaultAsync(x => x.FactionId == factionId
                    && x.Badge == badge
                    && !x.DeathDate.HasValue
                    && !x.DeletedDate.HasValue);
                if (characterTarget != null && characterTarget.Id != character.Id)
                {
                    player.EmitStaffShowMessage($"Distintivo {badge} está sendo usado por {characterTarget.Name}.");
                    return;
                }
            }

            var factionFlags = Functions.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                target.Character.UpdateFaction(factionRankId!.Value, Functions.Serialize(factionFlags), badge);
                target.FactionFlags = factionFlags;
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na facção.");
                await target.Save();
            }
            else
            {
                character.UpdateFaction(factionRankId!.Value, Functions.Serialize(factionFlags), badge);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você alterou as informações de {character.Name} na facção.", true);
            await player.GravarLog(LogType.Staff, $"Salvar Membro Facção {factionId} {characterId} {factionRankId} {badge} {flagsJSON}", target);

            var html = await Functions.GetFactionMembersHTML(factionId!.Value);
            foreach (var targetStaff in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                targetStaff.Emit("StaffShowFactionMembers", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberRemove))]
        public static async Task StaffFactionMemberRemove(MyPlayer player, string factionIdString, string characterIdString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var factionId = factionIdString.ToGuid();
            var characterId = characterIdString.ToGuid();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == characterId);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Nenhum jogador encontrado com o ID {characterId}.");
                return;
            }

            if (character.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Jogador não pertence a esta facção.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                await target.RemoveFromFaction();
                await target.Save();
                target.SendMessage(MessageType.Success, $"{player.User.Name} expulsou você da facção.");
            }
            else
            {
                var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);

                if (faction?.Government ?? false)
                {
                    var items = await context.CharactersItems.Where(x => x.CharacterId == character.Id).ToListAsync();
                    items = items.Where(x => !Functions.CanDropItem(character.Sex, faction, x)).ToList();
                    context.CharactersItems.RemoveRange(items);
                }

                character.ResetFaction();
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você expulsou {character.Name} da facção.", true);
            await player.GravarLog(LogType.Staff, $"Expulsar Facção {factionId} {characterId}", target);

            var html = await Functions.GetFactionMembersHTML(factionId!.Value);
            foreach (var targetStaff in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                targetStaff.Emit("StaffShowFactionMembers", true, html);
        }

        private static string GetFactionsHTML()
        {
            var html = string.Empty;
            if (Global.Factions.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='7'>Não há facções criadas.</td></tr>";
            }
            else
            {
                foreach (var faction in Global.Factions.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{faction.Id}</td>
                        <td>{faction.Name}</td>
                        <td>{faction.Type.GetDisplay()}</td>
                        <td><span style='color:#{faction.Color}'>#{faction.Color}</span></td>
                        <td><span style='color:#{faction.ChatColor}'>#{faction.ChatColor}</span></td>
                        <td>{faction.Slots}</td>
                        <td class='text-center'>
                            <input id='json{faction.Id}' type='hidden' value='{Functions.Serialize(faction)}' />
                            <button onclick='editar(`{faction.Id}`)' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='ranks(`{faction.Id}`)' type='button' class='btn btn-dark btn-sm'>RANKS</button>
                            <button onclick='members(`{faction.Id}`)' type='button' class='btn btn-dark btn-sm'>MEMBROS</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}