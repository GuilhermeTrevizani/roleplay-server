using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffVehicleScript : IScript
    {
        [Command("veiculos")]
        public static async Task CMD_veiculos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffVehicles", false, await GetVehiclesHTML());
        }

        [Command("atunar")]
        public static void CMD_atunar(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            Functions.CMDTuning(player, null, true);
        }

        [AsyncClientEvent(nameof(StaffVehicleSave))]
        public static async Task StaffVehicleSave(MyPlayer player, string idString, string model, int type, string typeId,
            int livery, int color1R, int color1G, int color1B, int color2R, int color2G, int color2B)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!byte.TryParse(livery.ToString(), out byte bLivery) || bLivery == 0)
            {
                player.EmitStaffShowMessage($"Livery deve ser entre 1 e 255.");
                return;
            }

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            await using var context = new DatabaseContext();
            Vehicle? vehicle = null;
            if (isNew)
            {
                if (!Enum.TryParse(model, true, out VehicleModel v1) && !Enum.TryParse(model, true, out VehicleModelMods v2))
                {
                    player.EmitStaffShowMessage($"Modelo {model} não existe.");
                    return;
                }

                vehicle = new();

                var government = false;
                if (type == 1) // Faction
                {
                    var factionId = new Guid(typeId);
                    var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);
                    if (!(faction?.Government ?? false))
                    {
                        player.EmitStaffShowMessage($"Facção {typeId} não é do tipo governamental.");
                        return;
                    }

                    government = true;
                    vehicle.SetFaction(factionId);
                }
                else if (type == 2) // Job
                {
                    var job = Global.Jobs.FirstOrDefault(x => x.CharacterJob == (CharacterJob)Convert.ToByte(typeId));
                    if (job == null)
                    {
                        player.EmitStaffShowMessage($"Emprego {typeId} não existe.");
                        return;
                    }

                    vehicle.SetJob(job.CharacterJob);
                    color1R = job.VehicleColor.R;
                    color1G = job.VehicleColor.G;
                    color1B = job.VehicleColor.B;
                    color2R = job.VehicleColor.R;
                    color2G = job.VehicleColor.G;
                    color2B = job.VehicleColor.B;
                }
                else if (type == 3) // Faction Gift
                {
                    var characterId = new Guid(typeId);
                    var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == characterId);
                    if (character == null)
                    {
                        player.EmitStaffShowMessage($"Personagem {typeId} não existe.");
                        return;
                    }

                    vehicle.SetOwner(characterId);
                    vehicle.SetStaffGift();
                }

                vehicle.Create(model, await Functions.GenerateVehiclePlate(government),
                    Convert.ToByte(color1R), Convert.ToByte(color1G), Convert.ToByte(color1B),
                    Convert.ToByte(color2R), Convert.ToByte(color2G), Convert.ToByte(color2B));
                vehicle.ChangePosition(player.Position.X, player.Position.Y, player.Position.Z,
                    player.Rotation.Roll, player.Rotation.Pitch, player.Rotation.Yaw);
                vehicle.SetFuel(vehicle.GetMaxFuel());
                vehicle.SetLivery(bLivery);
                if (vehicle.CharacterId.HasValue)
                    vehicle.SetLockNumber(await context.Vehicles.OrderByDescending(x => x.LockNumber).Select(x => x.LockNumber).FirstOrDefaultAsync() + 1);
            }
            else
            {
                if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
                {
                    player.EmitStaffShowMessage($"Veículo {id} está spawnado.");
                    return;
                }

                vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                if (vehicle == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                vehicle.SetLivery(bLivery);
                vehicle.SetColor(Convert.ToByte(color1R), Convert.ToByte(color1G), Convert.ToByte(color1B),
                    Convert.ToByte(color2R), Convert.ToByte(color2G), Convert.ToByte(color2B));
            }

            if (isNew)
                await context.Vehicles.AddAsync(vehicle);
            else
                context.Vehicles.Update(vehicle);

            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Veículo {(isNew ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Veículo | {Functions.Serialize(vehicle)}", null);

            var html = await GetVehiclesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Vehicles)))
                target.Emit("StaffVehicles", true, html);
        }

        [AsyncClientEvent(nameof(StaffVehicleRemove))]
        public static async Task StaffVehicleRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.EmitStaffShowMessage($"Veículo {id} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle != null)
            {
                vehicle.SetSold();
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync();
                await player.GravarLog(LogType.Staff, $"Remover Veículo | {Functions.Serialize(vehicle)}", null);
            }

            player.EmitStaffShowMessage($"Veículo {id} excluído.");

            var html = await GetVehiclesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Vehicles)))
                target.Emit("StaffVehicles", true, html);
        }

        private static async Task<string> GetVehiclesHTML()
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var vehicles = await context.Vehicles
                .Where(x => !x.Sold && (
                    x.Job != CharacterJob.None
                    || x.FactionId.HasValue
                    || (x.CharacterId.HasValue && x.StaffGift)))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            if (vehicles.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='6'>Não há veículos criados para facções, aluguel de empregos ou benefícios de personagens de facções.</td></tr>";
            }
            else
            {
                foreach (var vehicle in vehicles)
                    html += $@"<tr class='pesquisaitem'>
                        <td>{vehicle.Id}</td>
                        <td>{vehicle.Model}</td>
                        <td>{vehicle.Job.GetDisplay()}</td>
                        <td>{vehicle.FactionId}</td>
                        <td>{vehicle.CharacterId}</td>
                        <td class='text-center'>
                            <input id='json{vehicle.Id}' type='hidden' value='{Functions.Serialize(vehicle)}' />
                            <button onclick='edit({vehicle.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {vehicle.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}