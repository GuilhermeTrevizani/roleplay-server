using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffInfoScript : IScript
    {
        [Command("ainfos")]
        public static void CMD_ainfos(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffInfos", false, Functions.GetInfosHTML(null));
        }

        [AsyncClientEvent(nameof(StaffInfoSave))]
        public static async Task StaffInfoSave(MyPlayer player, int days, string message)
        {
            var maxDays = player.User.VIP switch
            {
                UserVIP.Gold => int.MaxValue,
                UserVIP.Silver => 15,
                UserVIP.Bronze => 5,
                _ => 3,
            };

            if (days > maxDays)
            {
                player.EmitStaffShowMessage($"O máximo de dias permitido para seu nível de VIP é de {maxDays}.");
                return;
            }

            var info = new Info
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z - 0.7f,
                Dimension = player.Dimension,
                ExpirationDate = DateTime.Now.AddDays(days),
                UserId = player.User.Id,
                Message = message,
            };

            await using var context = new DatabaseContext();
            await context.Infos.AddAsync(info);
            await context.SaveChangesAsync();

            info.User = player.User;

            Global.Infos.Add(info);
            info.CreateIdentifier();

            player.EmitStaffShowMessage($"Info {info.Id} criada.", true);
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoGoto))]
        public static void StaffInfoGoto(MyPlayer player, int id)
        {
            var info = Global.Infos.FirstOrDefault(x => x.Id == id);
            if (info == null)
                return;

            if (player.User.Staff < UserStaff.Moderator)
            {
                player.EmitShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.SetPosition(new Position(info.PosX, info.PosY, info.PosZ), info.Dimension, false);
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoRemove))]
        public static async Task StaffInfoRemove(MyPlayer player, int id)
        {
            var info = Global.Infos.FirstOrDefault(x => x.Id == id);
            if (info == null)
                return;

            if (player.User.Id != info.UserId
                && player.User.Staff < UserStaff.Moderator)
            {
                player.EmitShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            info.RemoveIdentifier();
            Global.Infos.Remove(info);
            context.Infos.Remove(info);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Info {id} removida.");
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(info.UserId == player.User.Id ? player.User.Id : null));
        }
    }
}