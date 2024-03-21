using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class FactionScript : IScript
    {
        [Command("f", "/f (mensagem)", GreedyArg = true)]
        public static async void CMD_f(MyPlayer player, string mensagem)
        {
            if (!player.Character.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção.");
                return;
            }

            if (player.Faction!.BlockedChat)
            {
                player.SendMessage(MessageType.Error, "Chat da facção está bloqueado.");
                return;
            }

            if (player.User.FactionChatToggle)
            {
                player.SendMessage(MessageType.Error, "Você está com o chat da facção desabilitado.");
                return;
            }

            var message = $"(( {player.FactionRank!.Name} {player.Character.Name} [{player.SessionId}]: {mensagem} ))";
            var color = $"#{player.Faction.ChatColor}";
            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId && !x.User.FactionChatToggle))
                target.SendMessage(MessageType.None, message, color);

            await player.GravarLog(LogType.FactionChat, $"{player.Character.FactionId} | {mensagem}", null);
        }

        [Command("blockf")]
        public static void CMD_blockf(MyPlayer player)
        {
            if (!player.FactionFlags.Contains(FactionFlag.BlockChat))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Faction!.ToggleBlockedChat();
            player.SendFactionMessage($"{player.FactionRank!.Name} {player.Character.Name} {(!player.Faction.BlockedChat ? "des" : string.Empty)}bloqueou o chat da facção.");
        }

        [Command("faccao")]
        public static async Task CMD_faccao(MyPlayer player)
        {
            if (!player.Character.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção.");
                return;
            }

            var htmlRanks = Functions.GetFactionRanksHTML(player.Faction!.Id);
            var htmlMembers = await Functions.GetFactionMembersHTML(player.Faction.Id);

            var ranksJson = Functions.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );
            var flagsJson = Functions.Serialize(
                player.Faction.GetFlags()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("ShowFaction",
                htmlRanks,
                htmlMembers,
                ranksJson,
                flagsJson,
                Functions.Serialize(player.Faction),
                player.Faction.Government,
                player.Character.FactionFlagsJSON,
                player.IsFactionLeader
            );
        }

        [Command("sairfaccao")]
        public static async Task CMD_sairfaccao(MyPlayer player)
        {
            if (!player.Character.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção.");
                return;
            }

            player.SendFactionMessage($"{player.FactionRank!.Name} {player.Character.Name} saiu da facção.");
            await player.GravarLog(LogType.Faction, $"/sairfaccao {player.Character.FactionId}", null);
            await player.RemoveFromFaction();
            await player.Save();
        }

        [AsyncClientEvent(nameof(FactionRankSave))]
        public static async Task FactionRankSave(MyPlayer player, string factionIdString, string factionRankIdString, string name)
        {
            var factionId = new Guid(factionIdString);
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionRankId = new Guid(factionRankIdString);
            var isNew = string.IsNullOrWhiteSpace(factionRankIdString);
            var factionRank = new FactionRank();
            if (isNew)
            {
                factionRank.Create(factionId,
                    Global.FactionsRanks.Where(x => x.FactionId == factionId).Select(x => x.Position).DefaultIfEmpty(0).Max() + 1,
                    name, 0);
            }
            else
            {
                factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
                if (factionRank == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                factionRank.Update(name, 0);
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

            await player.GravarLog(LogType.Faction, $"Gravar Rank | {Functions.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = Functions.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionRankRemove))]
        public static async Task FactionRankRemove(MyPlayer player, string factionIdString, string factionRankIdString)
        {
            var factionId = new Guid(factionIdString);
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionRankId = new Guid(factionRankIdString);
            var factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            if (factionRank == null)
                return;

            await using var context = new DatabaseContext();

            if (await context.Characters.AnyAsync(x => x.FactionRankId == factionRankId))
            {
                player.EmitStaffShowMessage($"Não é possível remover o rank {factionRankId} pois existem personagens nele.");
                return;
            }

            context.FactionsRanks.Remove(factionRank);
            await context.SaveChangesAsync();
            Global.FactionsRanks.Remove(factionRank);

            player.EmitStaffShowMessage($"Rank {factionRank.Id} excluído.");

            await player.GravarLog(LogType.Faction, $"Remover Rank | {Functions.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = Functions.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionRankOrder))]
        public static async Task FactionRankOrder(MyPlayer player, string factionIdString, string ranksJSON)
        {
            var factionId = new Guid(factionIdString);
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();

            var ranks = Functions.Deserialize<List<FactionRank>>(ranksJSON);
            var factionRanks = Global.FactionsRanks.Where(x => x.FactionId == factionId);
            foreach (var rank in ranks)
            {
                var factionRank = factionRanks.FirstOrDefault(x => x.Id == rank.Id);
                factionRank?.SetPosition(rank.Position);
            }

            context.FactionsRanks.UpdateRange(factionRanks);
            await context.SaveChangesAsync();
            player.EmitStaffShowMessage("Ranks ordenados.");

            await player.GravarLog(LogType.Faction, $"Ordenar Ranks | {ranksJSON}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = Functions.Serialize(factionRanks.OrderBy(x => x.Position));

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionMemberInvite))]
        public static async Task FactionMemberInvite(MyPlayer player, string factionIdString, int characterSessionId)
        {
            var factionId = new Guid(factionIdString);
            if (!player.FactionFlags.Contains(FactionFlag.InviteMember) || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

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

            await player.GravarLog(LogType.Faction, $"Convidar Facção {factionId}", target);
        }

        [AsyncClientEvent(nameof(FactionMemberSave))]
        public static async Task FactionMemberSave(MyPlayer player, string factionIdString, string characterIdString, string factionRankIdString,
            int badge, string flagsJSON)
        {
            var factionId = new Guid(factionIdString);
            if (!player.FactionFlags.Contains(FactionFlag.EditMember) || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var characterId = new Guid(characterIdString);
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

            var factionRankId = new Guid(factionRankIdString);
            var rank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            if (rank?.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Rank {factionRankId} não existe na facção {factionId}.");
                return;
            }

            if (badge < 0)
            {
                player.EmitStaffShowMessage($"Distintivo deve ser mais que zero.");
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

            if (character.FactionRankId >= player.Character.FactionRankId && character.Id != player.Character.Id)
            {
                player.EmitStaffShowMessage($"Jogador possui um rank igual ou maior que o seu.");
                return;
            }

            var factionFlags = Functions.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                target.Character.UpdateFaction(factionRankId, Functions.Serialize(factionFlags), badge);
                target.FactionFlags = factionFlags;
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na facção.");
                await target.Save();
            }
            else
            {
                character.UpdateFaction(factionRankId, Functions.Serialize(factionFlags), badge);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você alterou as informações de {character.Name} na facção.", true);
            await player.GravarLog(LogType.Faction, $"Salvar Membro Facção {factionId} {characterId} {factionRankId} {badge} {flagsJSON}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetFaction in Global.SpawnedPlayers.Where(x => x.Character.FactionId == factionId))
                targetFaction.Emit("ShowFactionUpdateMembers", html,
                targetFaction.Character.FactionFlagsJSON,
                targetFaction.IsFactionLeader);
        }

        [AsyncClientEvent(nameof(FactionMemberRemove))]
        public static async Task FactionMemberRemove(MyPlayer player, string factionIdString, string characterIdString)
        {
            var factionId = new Guid(factionIdString);
            if (!player.FactionFlags.Contains(FactionFlag.RemoveMember) || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var characterId = new Guid(characterIdString);
            await using var context = new DatabaseContext();
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

            if (character.FactionRankId >= player.Character.FactionRankId
                && character.Id != player.Character.Id)
            {
                player.EmitStaffShowMessage($"Jogador possui um rank igual ou maior que o seu.");
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
            await player.GravarLog(LogType.Faction, $"Expulsar Facção {factionId} {characterId}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetFaction in Global.SpawnedPlayers.Where(x => x.Character.FactionId == factionId))
                targetFaction.Emit("ShowFactionUpdateMembers", html,
                targetFaction.Character.FactionFlagsJSON,
                targetFaction.IsFactionLeader);
        }

        [ClientEvent(nameof(CancelDropBarrier))]
        public static void CancelDropBarrier(MyPlayer player)
        {
            player.DropFurniture = null;
            player.SendMessage(MessageType.Success, "Você cancelou o drop da barreira.");
        }

        [AsyncClientEvent(nameof(ConfirmDropBarrier))]
        public static async Task ConfirmDropBarrier(MyPlayer player, Vector3 position, Vector3 rotation)
        {
            if (player.DropFurniture == null)
            {
                player.SendMessage(MessageType.Error, "Você não está dropando um objeto.");
                return;
            }

            if (position.X == 0 || position.Y == 0 || position.Z == 0)
            {
                player.SendMessage(MessageType.Error, "Não foi possível recuperar a posição do item.");
                return;
            }

            var barrier = (MyObject)Alt.CreateObject(player.DropFurniture.Model, position, rotation);
            barrier.Dimension = player.Dimension;
            barrier.Frozen = true;
            barrier.Collision = true;
            barrier.CharacterId = player.Character.Id;
            barrier.FactionId = player.Character.FactionId;

            await player.GravarLog(LogType.Faction, $"/br {Functions.Serialize(player.DropFurniture)}", null);
            player.SendMessageToNearbyPlayers($"dropa uma barreira no chão.", MessageCategory.Ame, 5);
            player.DropFurniture = null;
        }
    }
}