using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffTruckerLocationScript : IScript
    {
        [Command("acaminhoneiro")]
        public static void CMD_acaminhoneiro(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffTruckerLocations", false, Functions.GetTruckerLocationsHTML());
        }

        [ClientEvent(nameof(StaffTruckerLocationGoto))]
        public static void StaffTruckerLocationGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
            if (truckerLocation == null)
                return;

            player.LimparIPLs();

            player.SetPosition(new Position(truckerLocation.PosX, truckerLocation.PosY, truckerLocation.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationRemove))]
        public static async Task StaffTruckerLocationRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
            if (truckerLocation == null)
                return;

            await using var context = new DatabaseContext();
            context.TruckerLocations.Remove(truckerLocation);
            context.TruckerLocationsDeliveries.RemoveRange(Global.TruckerLocationsDeliveries.Where(x => x.TruckerLocationId == id));
            await context.SaveChangesAsync();
            Global.TruckerLocations.Remove(truckerLocation);
            Global.TruckerLocationsDeliveries.RemoveAll(x => x.TruckerLocationId == truckerLocation.Id);
            truckerLocation.RemoveIdentifier();

            player.EmitStaffShowMessage($"Localização de caminhoneiro {truckerLocation.Id} excluída.");

            var html = Functions.GetTruckerLocationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Localização de Caminhoneiro | {Functions.Serialize(truckerLocation)}", null);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationSave))]
        public static async Task StaffTruckerLocationSave(MyPlayer player, int id, string name, Vector3 pos,
            int deliveryValue, int loadWaitTime, int unloadWaitTime, string allowedVehiclesJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (deliveryValue <= 0)
            {
                player.EmitStaffShowMessage($"Valor por Entrega deve ser maior que 0.");
                return;
            }

            if (loadWaitTime <= 0)
            {
                player.EmitStaffShowMessage($"Valor por Entrega deve ser maior que 0.");
                return;
            }

            if (unloadWaitTime <= 0)
            {
                player.EmitStaffShowMessage($"Tempo de Espera por Entrega deve ser maior que 0.");
                return;
            }

            var allowedVehicles = Functions.Deserialize<List<string>>(allowedVehiclesJSON.ToUpper());
            if (allowedVehicles.Count == 0)
            {
                player.EmitStaffShowMessage($"Veículos Permitidos devem ser informados.");
                return;
            }

            foreach (var allowedVehicle in allowedVehicles)
            {
                if (!Enum.TryParse(allowedVehicle, true, out VehicleModel v1) && !Enum.TryParse(allowedVehicle, true, out VehicleModelMods v2))
                {
                    player.EmitStaffShowMessage($"Veículo {allowedVehicle} não existe.");
                    return;
                }
            }

            var truckerLocation = new TruckerLocation();
            if (id > 0)
                truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);

            truckerLocation.Name = name;
            truckerLocation.PosX = pos.X;
            truckerLocation.PosY = pos.Y;
            truckerLocation.PosZ = pos.Z;
            truckerLocation.DeliveryValue = deliveryValue;
            truckerLocation.LoadWaitTime = loadWaitTime;
            truckerLocation.UnloadWaitTime = unloadWaitTime;
            truckerLocation.AllowedVehiclesJSON = Functions.Serialize(allowedVehicles);

            await using var context = new DatabaseContext();

            if (truckerLocation.Id == 0)
                await context.TruckerLocations.AddAsync(truckerLocation);
            else
                context.TruckerLocations.Update(truckerLocation);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.TruckerLocations.Add(truckerLocation);

            truckerLocation.CreateIdentifier();

            player.EmitStaffShowMessage($"Localização de caminhoneiro {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Localização de Caminhoneiro | {Functions.Serialize(truckerLocation)}", null);

            var html = Functions.GetTruckerLocationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);
        }

        [ClientEvent(nameof(StaffTruckerLocationsDeliveriesShow))]
        public static void StaffTruckerLocationsDeliveriesShow(MyPlayer player, int truckerLocationId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffTruckerLocationsDeliveries",
                false,
                Functions.GetTruckerLocationsDeliverysHTML(truckerLocationId),
                truckerLocationId);
        }

        [ClientEvent(nameof(StaffTruckerLocationDeliveryGoto))]
        public static void StaffTruckerLocationDeliveryGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == id);
            if (truckerLocationDelivery == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(truckerLocationDelivery.PosX, truckerLocationDelivery.PosY, truckerLocationDelivery.PosZ), 0, false);
        }


        [AsyncClientEvent(nameof(StaffTruckerLocationDeliverySave))]
        public static async Task StaffTruckerLocationDeliverySave(MyPlayer player,
            int truckerLocationDeliveryId,
            int truckerLocationId,
            Vector3 pos)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = new TruckerLocationDelivery();
            if (truckerLocationDeliveryId > 0)
                truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == truckerLocationDeliveryId);

            truckerLocationDelivery.TruckerLocationId = truckerLocationId;
            truckerLocationDelivery.PosX = pos.X;
            truckerLocationDelivery.PosY = pos.Y;
            truckerLocationDelivery.PosZ = pos.Z;

            await using var context = new DatabaseContext();

            if (truckerLocationDelivery.Id == 0)
                await context.TruckerLocationsDeliveries.AddAsync(truckerLocationDelivery);
            else
                context.TruckerLocationsDeliveries.Update(truckerLocationDelivery);

            await context.SaveChangesAsync();

            if (truckerLocationDeliveryId == 0)
                Global.TruckerLocationsDeliveries.Add(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega {(truckerLocationDeliveryId == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Entrega Localização de Caminhoneiro | {Functions.Serialize(truckerLocationDelivery)}", null);

            var html = Functions.GetTruckerLocationsDeliverysHTML(truckerLocationId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationDeliveryRemove))]
        public static async Task StaffTruckerLocationDeliveryRemove(MyPlayer player, int truckerLocationDeliveryId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == truckerLocationDeliveryId);
            if (truckerLocationDelivery == null)
                return;

            await using var context = new DatabaseContext();
            context.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);
            await context.SaveChangesAsync();
            Global.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega da Localização de Caminhoneiro {truckerLocationDelivery.Id} excluída.");

            await player.GravarLog(LogType.Staff, $"Remover Entrega da Localização de Caminhoneiro | {Functions.Serialize(truckerLocationDelivery)}", null);

            var html = Functions.GetTruckerLocationsDeliverysHTML(truckerLocationDelivery.TruckerLocationId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }
    }
}