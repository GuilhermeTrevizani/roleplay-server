using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
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

            player.Emit("StaffTruckerLocations", false, GetTruckerLocationsHTML());
        }

        [ClientEvent(nameof(StaffTruckerLocationGoto))]
        public static void StaffTruckerLocationGoto(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
            if (truckerLocation == null)
                return;

            player.LimparIPLs();

            player.SetPosition(new Position(truckerLocation.PosX, truckerLocation.PosY, truckerLocation.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationRemove))]
        public static async Task StaffTruckerLocationRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
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

            var html = GetTruckerLocationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Localização de Caminhoneiro | {Functions.Serialize(truckerLocation)}", null);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationSave))]
        public static async Task StaffTruckerLocationSave(MyPlayer player, string idString, string name, Vector3 pos,
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

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            var truckerLocation = new TruckerLocation();
            if (isNew)
            {
                truckerLocation.Create(name, pos.X, pos.Y, pos.Z, deliveryValue, loadWaitTime, unloadWaitTime, Functions.Serialize(allowedVehicles));
            }
            else
            {
                truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
                if (truckerLocation == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                truckerLocation.Update(name, pos.X, pos.Y, pos.Z, deliveryValue, loadWaitTime, unloadWaitTime, Functions.Serialize(allowedVehicles));
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.TruckerLocations.AddAsync(truckerLocation);
            else
                context.TruckerLocations.Update(truckerLocation);

            await context.SaveChangesAsync();

            if (isNew)
                Global.TruckerLocations.Add(truckerLocation);

            truckerLocation.CreateIdentifier();

            player.EmitStaffShowMessage($"Localização de caminhoneiro {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Localização de Caminhoneiro | {Functions.Serialize(truckerLocation)}", null);

            var html = GetTruckerLocationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);
        }

        [ClientEvent(nameof(StaffTruckerLocationsDeliveriesShow))]
        public static void StaffTruckerLocationsDeliveriesShow(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            var id = idString.ToGuid();
            player.Emit("StaffTruckerLocationsDeliveries",
                false,
                GetTruckerLocationsDeliverysHTML(id.Value),
                idString);
        }

        [ClientEvent(nameof(StaffTruckerLocationDeliveryGoto))]
        public static void StaffTruckerLocationDeliveryGoto(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == id);
            if (truckerLocationDelivery == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(truckerLocationDelivery.PosX, truckerLocationDelivery.PosY, truckerLocationDelivery.PosZ), 0, false);
        }


        [AsyncClientEvent(nameof(StaffTruckerLocationDeliverySave))]
        public static async Task StaffTruckerLocationDeliverySave(MyPlayer player,
            string truckerLocationDeliveryIdString,
            string truckerLocationIdString,
            Vector3 pos)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDeliveryId = new Guid(truckerLocationDeliveryIdString);
            var truckerLocationId = new Guid(truckerLocationIdString);
            var isNew = string.IsNullOrWhiteSpace(truckerLocationDeliveryIdString);
            var truckerLocationDelivery = new TruckerLocationDelivery();
            if (isNew)
            {
                truckerLocationDelivery.Create(truckerLocationId, pos.X, pos.Y, pos.Z);
            }
            else
            {
                truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == truckerLocationDeliveryId);
                if (truckerLocationDelivery == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                truckerLocationDelivery.Update(truckerLocationId, pos.X, pos.Y, pos.Z);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.TruckerLocationsDeliveries.AddAsync(truckerLocationDelivery);
            else
                context.TruckerLocationsDeliveries.Update(truckerLocationDelivery);

            await context.SaveChangesAsync();

            if (isNew)
                Global.TruckerLocationsDeliveries.Add(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Entrega Localização de Caminhoneiro | {Functions.Serialize(truckerLocationDelivery)}", null);

            var html = GetTruckerLocationsDeliverysHTML(truckerLocationId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationDeliveryRemove))]
        public static async Task StaffTruckerLocationDeliveryRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == id);
            if (truckerLocationDelivery == null)
                return;

            await using var context = new DatabaseContext();
            context.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);
            await context.SaveChangesAsync();
            Global.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega da Localização de Caminhoneiro {truckerLocationDelivery.Id} excluída.");

            await player.GravarLog(LogType.Staff, $"Remover Entrega da Localização de Caminhoneiro | {Functions.Serialize(truckerLocationDelivery)}", null);

            var html = GetTruckerLocationsDeliverysHTML(truckerLocationDelivery.TruckerLocationId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }

        public static string GetTruckerLocationsHTML()
        {
            var html = string.Empty;
            if (Global.TruckerLocations.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='8'>Não há localizações de caminhoneiros criadas.</td></tr>";
            }
            else
            {
                foreach (var truckerLocation in Global.TruckerLocations.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{truckerLocation.Id}</td>
                        <td>{truckerLocation.Name}</td>
                        <td>X: {truckerLocation.PosX} | Y: {truckerLocation.PosY} | Z: {truckerLocation.PosZ}</td>
                        <td>${truckerLocation.DeliveryValue:N0}</td>
                        <td>{truckerLocation.LoadWaitTime}</td>
                        <td>{truckerLocation.UnloadWaitTime}</td>
                        <td>{string.Join(", ", truckerLocation.GetAllowedVehicles())}</td>
                        <td class='text-center'>
                            <input id='json{truckerLocation.Id}' type='hidden' value='{Functions.Serialize(truckerLocation)}' />
                            <button onclick='goto({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editDeliveries({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>ENTREGAS</button>
                            <button onclick='remove(this, {truckerLocation.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetTruckerLocationsDeliverysHTML(Guid truckerLocationId)
        {
            var html = string.Empty;
            var truckerLocationDeliveries = Global.TruckerLocationsDeliveries.Where(x => x.TruckerLocationId == truckerLocationId);
            if (!truckerLocationDeliveries.Any())
            {
                html = "<tr><td class='text-center' colspan='3'>Não há entregas criadas.</td></tr>";
            }
            else
            {
                foreach (var truckerLocationDelivery in truckerLocationDeliveries)
                {
                    html += $@"<tr class='pesquisaitem'>
                        <td>{truckerLocationDelivery.Id}</td>
                        <td>X: {truckerLocationDelivery.PosX} | Y: {truckerLocationDelivery.PosY} | Z: {truckerLocationDelivery.PosZ}</td>
                        <td class='text-center'>
                            <input id='json{truckerLocationDelivery.Id}' type='hidden' value='{Functions.Serialize(truckerLocationDelivery)}' />
                            <button onclick='goto({truckerLocationDelivery.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({truckerLocationDelivery.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {truckerLocationDelivery.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
    }
}