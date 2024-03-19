using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffBlipScript : IScript
    {
        [Command("blips")]
        public static void CMD_blips(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffBlips", false, GetBlipsHTML());
        }

        [ClientEvent(nameof(StaffBlipGoto))]
        public static void StaffBlipGoto(MyPlayer player, int id)
        {
            var blip = Global.Blips.FirstOrDefault(x => x.Id == id);
            if (blip == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(blip.PosX, blip.PosY, blip.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffBlipSave))]
        public static async Task StaffBlipSave(MyPlayer player, int id, string name, Vector3 pos, int type, int color)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (type < 1 || type > 744)
            {
                player.EmitStaffShowMessage("Tipo deve ser entre 1 e 744.");
                return;
            }

            if (color < 1 || color > 85)
            {
                player.EmitStaffShowMessage("Cor deve ser entre 1 e 85.");
                return;
            }

            var blip = new Blip();
            if (id > 0)
                blip = Global.Blips.FirstOrDefault(x => x.Id == id);

            blip.Name = name;
            blip.PosX = pos.X;
            blip.PosY = pos.Y;
            blip.PosZ = pos.Z;
            blip.Type = Convert.ToUInt16(type);
            blip.Color = Convert.ToByte(color);

            await using var context = new DatabaseContext();

            if (blip.Id == 0)
                await context.Blips.AddAsync(blip);
            else
                context.Blips.Update(blip);

            await context.SaveChangesAsync();

            blip.CreateIdentifier();

            if (id == 0)
                Global.Blips.Add(blip);

            player.EmitStaffShowMessage($"Blip {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Blip | {Functions.Serialize(blip)}", null);

            var html = GetBlipsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Blips)))
                target.Emit("StaffBlips", true, html);
        }

        [AsyncClientEvent(nameof(StaffBlipRemove))]
        public static async Task StaffBlipRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Id == id);
            if (blip != null)
            {
                await using var context = new DatabaseContext();
                context.Blips.Remove(blip);
                await context.SaveChangesAsync();
                Global.Blips.Remove(blip);
                blip.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Blip | {Functions.Serialize(blip)}", null);
            }

            player.EmitStaffShowMessage($"Blip {id} excluído.");

            var html = GetBlipsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Blips)))
                target.Emit("StaffBlips", true, html);
        }

        private static string GetBlipsHTML()
        {
            var html = string.Empty;
            if (Global.Blips.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='5'>Não há blips criados.</td></tr>";
            }
            else
            {
                foreach (var blip in Global.Blips.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{blip.Id}</td>
                        <td>{blip.Name}</td>
                        <td>{blip.Type}</td>
                        <td>{blip.Color}</td>
                        <td>X: {blip.PosX} | Y: {blip.PosY} | Z: {blip.PosZ}</td>
                        <td class='text-center'>
                            <input id='json{blip.Id}' type='hidden' value='{Functions.Serialize(blip)}' />
                            <button onclick='ir({blip.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='editar({blip.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {blip.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}