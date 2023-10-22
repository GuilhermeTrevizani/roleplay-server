using AltV.Net.Data;
using Roleplay.Factories;
using Roleplay.Models;
using System.Text.Json;

namespace Roleplay.Commands.Faction
{
    public class Faction
    {
        [Command("f", "/f (mensagem)", GreedyArg = true)]
        public static async void CMD_f(MyPlayer player, string mensagem)
        {
            if (!player.Character.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção.");
                return;
            }

            if (player.Faction.BlockedChat)
            {
                player.SendMessage(MessageType.Error, "Chat da facção está bloqueado.");
                return;
            }

            if (player.User.FactionChatToggle)
            {
                player.SendMessage(MessageType.Error, "Você está com o chat da facção desabilitado.");
                return;
            }

            var message = $"(( {player.FactionRank.Name} {player.Character.Name} [{player.SessionId}]: {mensagem} ))";
            var color = $"#{player.Faction.ChatColor}";
            foreach (var target in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId && !x.User.FactionChatToggle))
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

            player.Faction.BlockedChat = !player.Faction.BlockedChat;
            player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} {(!player.Faction.BlockedChat ? "des" : string.Empty)}bloqueou o chat da facção.");
        }

        [Command("faccao")]
        public static async Task CMD_faccao(MyPlayer player)
        {
            if (!player.Character.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção.");
                return;
            }

            var htmlRanks = Functions.GetFactionRanksHTML(player.Faction.Id);
            var htmlMembers = await Functions.GetFactionMembersHTML(player.Faction.Id);

            var ranksJson = JsonSerializer.Serialize(
                Global.FactionsRanks
                .Where(x => x.FactionId == player.Character.FactionId)
                .OrderBy(x => x.Position)
            );
            var flagsJson = JsonSerializer.Serialize(
                player.Faction.GetFlags()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("ShowFaction",
                htmlRanks,
                htmlMembers,
                ranksJson,
                flagsJson,
                JsonSerializer.Serialize(player.Faction),
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

            player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} saiu da facção.");
            await player.GravarLog(LogType.Faction, $"/sairfaccao {player.Character.FactionId}", null);
            await player.RemoveFromFaction();
            await player.Save();
        }

        [Command("arsenal")]
        public static void CMD_arsenal(MyPlayer player)
        {
            if (!player.FactionFlags.Contains(FactionFlag.Armory))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsArmories.FirstOrDefault(x =>
                player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                && x.FactionId == player.Character.FactionId
                && x.Dimension == player.Dimension);
            if (factionArmory == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum arsenal da sua facção.");
                return;
            }

            var html = Functions.GetFactionArmoriesWeaponsHTML(factionArmory.Id, false);

            player.Emit("ShowFactionArmory",
                false,
                html,
                JsonSerializer.Serialize(player.Faction),
                player.Faction.Government);
        }

        [Command("drughouse")]
        public static void CMD_drughouse(MyPlayer player)
        {
            if (!player.FactionFlags.Contains(FactionFlag.DrugHouse))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionDrugHouse = Global.FactionsDrugsHouses.FirstOrDefault(x =>
                player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                && x.FactionId == player.Character.FactionId
                && x.Dimension == player.Dimension);
            if (factionDrugHouse == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhuma drug house da sua facção.");
                return;
            }

            var html = Functions.GetFactionsDrugsHousesItemsHTML(factionDrugHouse.Id, false);

            player.Emit("ShowFactionDrugHouse",
                false,
                html,
                JsonSerializer.Serialize(player.Faction));
        }
    }
}