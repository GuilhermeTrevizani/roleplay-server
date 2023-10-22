using AltV.Net.Data;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands.Staff
{
    public class GameAdministrator
    {
        [Command("ban", "/ban (ID ou nome) (dias [0 para permanente]) (motivo)", GreedyArg = true)]
        public static async Task CMD_ban(MyPlayer player, string idNome, int dias, string motivo)
        {
            if (player.User.Staff < UserStaff.GameAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (target.User.Staff >= player.User.Staff)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var ban = new Banishment
            {
                ExpirationDate = dias > 0 ? DateTime.Now.AddDays(dias) : null,
                Reason = motivo,
                CharacterId = target.Character.Id,
                UserId = target.User.Id,
                StaffUserId = player.User.Id,
            };

            await context.Banishments.AddAsync(ban);

            await context.Punishments.AddAsync(new Punishment
            {
                Duration = dias,
                Reason = motivo,
                CharacterId = target.Character.Id,
                Type = PunishmentType.Ban,
                StaffUserId = player.User.Id,
            });
            await context.SaveChangesAsync();

            await target.Save();
            var strBan = dias == 0 ? "permanentemente" : $"por {dias} dia{(dias > 1 ? "s" : string.Empty)}";
            await Functions.SendStaffMessage($"{player.User.Name} baniu {target.Character.Name} ({target.User.Name}) {strBan}. Motivo: {motivo}", false);
            target.Kick($"{player.User.Name} baniu você {strBan}. Motivo: {motivo}");
        }

        [Command("pos", "/pos (x) (y) (z)")]
        public static async Task CMD_pos(MyPlayer player, float x, float y, float z)
        {
            if (player.User.Staff < UserStaff.GameAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.SetPosition(new Position(x, y, z), 0, false);
            await player.GravarLog(LogType.Staff, $"/pos {x} {y} {z}", null);
        }

        [Command("o", "/o (mensagem)", GreedyArg = true)]
        public static async Task CMD_o(MyPlayer player, string mensagem)
        {
            if (player.User.Staff < UserStaff.GameAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            foreach (var x in Global.Players)
                x.SendMessage(MessageType.None, $"(( [{Global.SERVER_INITIALS}] {{{player.StaffColor}}}{player.User.Name}{{#FFFFFF}}: {mensagem} ))");

            await player.GravarLog(LogType.GlobalOOCChat, mensagem, null);
        }

        [Command("waypoint")]
        public static void CMD_waypoint(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.GameAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Waypoint");
        }
    }
}