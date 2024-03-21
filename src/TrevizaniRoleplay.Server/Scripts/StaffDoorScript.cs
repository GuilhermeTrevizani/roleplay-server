using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffDoorScript : IScript
    {
        [Command("portas")]
        public static void CMD_portas(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffDoors", false, GetDoorsHTML());
        }

        [ClientEvent(nameof(StaffDoorGoto))]
        public static void StaffDoorGoto(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(door.PosX, door.PosY, door.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffDoorRemove))]
        public static async Task StaffDoorRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            await using var context = new DatabaseContext();
            context.Doors.Remove(door);
            await context.SaveChangesAsync();
            Global.Doors.Remove(door);
            door.SetLocked(false);
            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {door.Id} excluída.");

            var html = GetDoorsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Porta | {Functions.Serialize(door)}", null);
        }

        [AsyncClientEvent(nameof(StaffDoorSave))]
        public static async Task StaffDoorSave(MyPlayer player, string idString, string name, long hash, Vector3 pos,
            string factionIdString, string companyIdString, bool locked)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionId = new Guid(factionIdString);
            if (!string.IsNullOrWhiteSpace(factionIdString) && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            var companyId = new Guid(companyIdString);
            if (!string.IsNullOrWhiteSpace(companyIdString) && !Global.Companies.Any(x => x.Id == companyId))
            {
                player.EmitStaffShowMessage($"Empresa {factionId} não existe.");
                return;
            }

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            var door = new Door();
            if (isNew)
            {
                door.Create(name, hash, pos.X, pos.Y, pos.Z, factionId, companyId, locked);
            }
            else
            {
                door.Update(name, hash, pos.X, pos.Y, pos.Z, factionId, companyId, locked);
                door = Global.Doors.FirstOrDefault(x => x.Id == id);
                if (door == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.Doors.AddAsync(door);
            else
                context.Doors.Update(door);

            await context.SaveChangesAsync();

            if (isNew)
                Global.Doors.Add(door);

            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Porta | {Functions.Serialize(door)}", null);

            var html = GetDoorsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);
        }

        private static string GetDoorsHTML()
        {
            var html = string.Empty;
            if (Global.Doors.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='8'>Não há portas criadas.</td></tr>";
            }
            else
            {
                foreach (var door in Global.Doors.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{door.Id}</td>
                        <td>{door.Name}</td>
                        <td>{door.Hash}</td>
                        <td>X: {door.PosX} | Y: {door.PosY} | Z: {door.PosZ}</td>
                        <td>{door.FactionId}</td>
                        <td>{door.CompanyId}</td>
                        <td class='text-center'>{(door.Locked ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{door.Id}' type='hidden' value='{Functions.Serialize(door)}' />
                            <button onclick='goto({door.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({door.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {door.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}