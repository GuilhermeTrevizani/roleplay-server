using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
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

            player.Emit("StaffInfos", false, GetInfosHTML(null));
        }

        [Command("infos")]
        public static void CMD_infos(MyPlayer player)
        {
            player.Emit("StaffInfos", false, GetInfosHTML(player.User.Id));
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

            var info = new Info();
            info.Create(player.Position.X, player.Position.Y, player.Position.Z - 0.7f, player.Dimension, days, player.User.Id, message);

            await using var context = new DatabaseContext();
            await context.Infos.AddAsync(info);
            await context.SaveChangesAsync();

            info.User = player.User;

            Global.Infos.Add(info);
            info.CreateIdentifier();

            player.EmitStaffShowMessage($"Info {info.Id} criada.", true);
            player.Emit("StaffInfos", true, GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoGoto))]
        public static void StaffInfoGoto(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var info = Global.Infos.FirstOrDefault(x => x.Id == id);
            if (info == null)
                return;

            if (player.User.Staff < UserStaff.Moderator)
            {
                player.EmitShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.SetPosition(new Position(info.PosX, info.PosY, info.PosZ), info.Dimension, false);
            player.Emit("StaffInfos", true, GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoRemove))]
        public static async Task StaffInfoRemove(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
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
            player.Emit("StaffInfos", true, GetInfosHTML(info.UserId == player.User.Id ? player.User.Id : null));
        }

        private static string GetInfosHTML(Guid? userId)
        {
            var html = string.Empty;
            var infos = Global.Infos;
            if (userId.HasValue)
                infos = infos.Where(x => x.UserId == userId).ToList();

            if (infos.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='8'>Não há informações criadas.</td></tr>";
            }
            else
            {
                foreach (var info in infos.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{info.Id}</td>
                        <td>{info.Date}</td>
                        <td>{info.ExpirationDate}</td>
                        <td>{info.User!.Name} [{info.UserId}]</td>
                        <td>X: {info.PosX} | Y: {info.PosY} | Z: {info.PosZ}</td>
                        <td>{info.Dimension}</td>
                        <td>{info.Message}</td>
                        <td class='text-center'>
                            {(!userId.HasValue ? $"<button onclick='goto(`{info.Id}`)' type='button' class='btn btn-dark btn-sm'>IR</button>" : string.Empty)}
                            <button onclick='remove(this, `{info.Id}`)' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}