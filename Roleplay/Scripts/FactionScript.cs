using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class FactionScript : IScript
    {
        [AsyncClientEvent(nameof(FactionRankSave))]
        public static async Task FactionRankSave(MyPlayer player, int factionId, int factionRankId, string name)
        {
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionRank = new FactionRank();
            if (factionRankId > 0)
                factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);

            factionRank.FactionId = factionId;
            factionRank.Name = name;

            await using var context = new DatabaseContext();

            if (factionRank.Id == 0)
                await context.FactionsRanks.AddAsync(factionRank);
            else
                context.FactionsRanks.Update(factionRank);

            await context.SaveChangesAsync();

            if (factionRankId == 0)
                Global.FactionsRanks.Add(factionRank);

            player.EmitStaffShowMessage($"Rank {(factionRankId == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Faction, $"Gravar Rank | {JsonSerializer.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = JsonSerializer.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );

            foreach (var target in Global.Players.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionRankRemove))]
        public static async Task FactionRankRemove(MyPlayer player, int factionId, int factionRankId)
        {
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

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

            await player.GravarLog(LogType.Faction, $"Remover Rank | {JsonSerializer.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = JsonSerializer.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );

            foreach (var target in Global.Players.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionRankOrder))]
        public static async Task FactionRankOrder(MyPlayer player, int factionId, string ranksJSON)
        {
            if (!player.IsFactionLeader || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();

            var ranks = JsonSerializer.Deserialize<List<FactionRank>>(ranksJSON);
            foreach (var rank in ranks)
            {
                var factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == rank.Id);
                factionRank.Position = rank.Position;
                context.FactionsRanks.Update(factionRank);
            }

            await context.SaveChangesAsync();
            player.EmitStaffShowMessage("Ranks ordenados.");

            await player.GravarLog(LogType.Faction, $"Ordenar Ranks | {ranksJSON}", null);

            var html = Functions.GetFactionRanksHTML(factionId);

            var ranksJson = JsonSerializer.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );

            foreach (var target in Global.Players.Where(x => x.Character.FactionId == factionId))
                target.Emit("ShowFactionUpdateRanks", html, ranksJson);
        }

        [AsyncClientEvent(nameof(FactionMemberInvite))]
        public static async Task FactionMemberInvite(MyPlayer player, int factionId, int characterSessionId)
        {
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

            var target = Global.Players.FirstOrDefault(x => x.SessionId == characterSessionId);
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
                Value = new string[] { faction.Id.ToString(), rank.Id.ToString() },
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.Faccao);
            target.Invites.Add(convite);

            player.EmitStaffShowMessage($"Você convidou {target.Character.Name} para a facção.", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} convidou você para a facção {faction.Name}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");

            await player.GravarLog(LogType.Faction, $"Convidar Facção {factionId}", target);
        }

        [AsyncClientEvent(nameof(FactionMemberSave))]
        public static async Task FactionMemberSave(MyPlayer player, int factionId, int characterId, int factionRankId,
            int badge, string flagsJSON)
        {
            if (!player.FactionFlags.Contains(FactionFlag.EditMember) || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

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

            if (character.FactionRankId >= player.Character.FactionRankId
                && character.Id != player.Character.Id)
            {
                player.EmitStaffShowMessage($"Jogador possui um rank igual ou maior que o seu.");
                return;
            }

            var factionFlags = JsonSerializer.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                target.Character.FactionRankId = factionRankId;
                target.Character.Badge = badge;
                target.FactionFlags = factionFlags;
                target.Character.FactionFlagsJSON = JsonSerializer.Serialize(target.FactionFlags);
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na facção.");
                await target.Save();
            }
            else
            {
                character.FactionRankId = factionRankId;
                character.Badge = badge;
                character.FactionFlagsJSON = JsonSerializer.Serialize(factionFlags);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você alterou as informações de {character.Name} na facção.", true);
            await player.GravarLog(LogType.Faction, $"Salvar Membro Facção {factionId} {characterId} {factionRankId} {badge} {flagsJSON}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetFaction in Global.Players.Where(x => x.Character.FactionId == factionId))
                targetFaction.Emit("ShowFactionUpdateMembers", html,
                targetFaction.Character.FactionFlagsJSON,
                targetFaction.IsFactionLeader);
        }

        [AsyncClientEvent(nameof(FactionMemberRemove))]
        public static async Task FactionMemberRemove(MyPlayer player, int factionId, int characterId)
        {
            if (!player.FactionFlags.Contains(FactionFlag.RemoveMember) || player.Character.FactionId != factionId)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

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

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == character.Id);
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
                    character.Badge = 0;
                    character.Armor = 0;

                    var items = (await context.CharactersItems.Where(x => x.CharacterId == character.Id).ToListAsync()).Select(x => new CharacterItem(x)).ToList();
                    items = items.Where(x => !Functions.CanDropItem(character.Sex, faction, x)).ToList();
                    context.CharactersItems.RemoveRange(items);
                }

                character.FactionFlagsJSON = "[]";
                character.FactionId = character.FactionRankId = null;
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você expulsou {character.Name} da facção.", true);
            await player.GravarLog(LogType.Faction, $"Expulsar Facção {factionId} {characterId}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetFaction in Global.Players.Where(x => x.Character.FactionId == factionId))
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
            if (position.X == 0 || position.Y == 0 || position.Z == 0)
            {
                player.SendMessage(MessageType.Error, "Não foi possível recuperar a posição do item.");
                return;
            }

            var barrier = (MyObject)Alt.CreateNetworkObject(player.DropFurniture.Model, position, rotation);
            barrier.Dimension = player.Dimension;
            barrier.Frozen = true;
            barrier.Collision = true;
            barrier.CharacterId = player.Character.Id;
            barrier.FactionId = player.Faction.Id;

            await player.GravarLog(LogType.Faction, $"/br {JsonSerializer.Serialize(player.DropFurniture)}", null);
            player.SendMessageToNearbyPlayers($"dropa uma barreira no chão.", MessageCategory.Ame, 5);
            player.DropFurniture = null;
        }
    }
}