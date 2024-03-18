﻿using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
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

            player.Emit("StaffDoors", false, Functions.GetDoorsHTML());
        }

        [ClientEvent(nameof(StaffDoorGoto))]
        public static void StaffDoorGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(door.PosX, door.PosY, door.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffDoorRemove))]
        public static async Task StaffDoorRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            await using var context = new DatabaseContext();
            context.Doors.Remove(door);
            await context.SaveChangesAsync();
            Global.Doors.Remove(door);
            door.Locked = false;
            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {door.Id} excluída.");

            var html = Functions.GetDoorsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Porta | {Functions.Serialize(door)}", null);
        }

        [AsyncClientEvent(nameof(StaffDoorSave))]
        public static async Task StaffDoorSave(MyPlayer player, int id, string name, long hash, Vector3 pos,
            int factionId, int companyId, bool locked)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (factionId != 0 && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            if (companyId != 0 && !Global.Companies.Any(x => x.Id == companyId))
            {
                player.EmitStaffShowMessage($"Empresa {factionId} não existe.");
                return;
            }

            var door = new Door();
            if (id > 0)
                door = Global.Doors.FirstOrDefault(x => x.Id == id);

            door.Name = name;
            door.Hash = hash;
            door.PosX = pos.X;
            door.PosY = pos.Y;
            door.PosZ = pos.Z;
            door.FactionId = factionId == 0 ? null : factionId;
            door.CompanyId = companyId == 0 ? null : companyId;
            door.Locked = locked;

            await using var context = new DatabaseContext();

            if (door.Id == 0)
                await context.Doors.AddAsync(door);
            else
                context.Doors.Update(door);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Doors.Add(door);

            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Porta | {Functions.Serialize(door)}", null);

            var html = Functions.GetDoorsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);
        }
    }
}