using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
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

            player.Emit("StaffVehicles", false, await Functions.GetVehiclesHTML());
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
        public static async Task StaffVehicleSave(MyPlayer player, string idString, string model, int type, string typeIdString,
            int livery, int color1R, int color1G, int color1B, int color2R, int color2G, int color2B)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var typeId = new Guid(typeIdString);
            var id = new Guid(idString);
            if (id > 0 && Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.EmitStaffShowMessage($"Veículo {id} está spawnado.");
                return;
            }

            if (id == 0 && !Enum.TryParse(model, true, out VehicleModel v1) && !Enum.TryParse(model, true, out VehicleModelMods v2))
            {
                player.EmitStaffShowMessage($"Modelo {model} não existe.");
                return;
            }

            if (!byte.TryParse(livery.ToString(), out byte bLivery) || bLivery == 0)
            {
                player.EmitStaffShowMessage($"Livery deve ser entre 1 e 255.");
                return;
            }

            await using var context = new DatabaseContext();

            var vehicle = new Vehicle();
            if (id > 0)
                vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);

            var government = false;
            if (type == 1) // Faction
            {
                var faction = Global.Factions.FirstOrDefault(x => x.Id == typeId);
                if (!(faction?.Government ?? false))
                {
                    player.EmitStaffShowMessage($"Facção {typeId} não é do tipo governamental.");
                    return;
                }

                government = true;
                vehicle.FactionId = typeId;
            }
            else if (type == 2) // Job
            {
                var job = Global.Jobs.FirstOrDefault(x => x.CharacterJob == (CharacterJob)typeId);
                if (job == null)
                {
                    player.EmitStaffShowMessage($"Emprego {typeId} não existe.");
                    return;
                }

                vehicle.Job = job.CharacterJob;
                vehicle.Color1R = job.VehicleColor.R;
                vehicle.Color1G = job.VehicleColor.G;
                vehicle.Color1B = job.VehicleColor.B;
                vehicle.Color2R = job.VehicleColor.R;
                vehicle.Color2G = job.VehicleColor.G;
                vehicle.Color2B = job.VehicleColor.B;
            }
            else if (type == 3) // Faction Gift
            {
                var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == typeId);
                if (character == null)
                {
                    player.EmitStaffShowMessage($"Personagem {typeId} não existe.");
                    return;
                }

                vehicle.CharacterId = typeId;
                vehicle.FactionGift = true;
            }

            vehicle.Livery = bLivery;

            if (type != 2)
            {
                vehicle.Color1R = Convert.ToByte(color1R);
                vehicle.Color1G = Convert.ToByte(color1G);
                vehicle.Color1B = Convert.ToByte(color1B);
                vehicle.Color2R = Convert.ToByte(color2R);
                vehicle.Color2G = Convert.ToByte(color2G);
                vehicle.Color2B = Convert.ToByte(color2B);
            }

            if (vehicle.Id == 0)
            {
                vehicle.Model = model;
                vehicle.PosX = player.Position.X;
                vehicle.PosY = player.Position.Y;
                vehicle.PosZ = player.Position.Z;
                vehicle.RotR = player.Rotation.Roll;
                vehicle.RotP = player.Rotation.Pitch;
                vehicle.RotY = player.Rotation.Yaw;
                vehicle.Fuel = vehicle.GetMaxFuel();
                vehicle.Plate = await Functions.GenerateVehiclePlate(government);
                vehicle.LockNumber = await context.Vehicles.OrderByDescending(x => x.LockNumber).Select(x => x.LockNumber).FirstOrDefaultAsync() + 1;
                await context.Vehicles.AddAsync(vehicle);
            }
            else
            {
                context.Vehicles.Update(vehicle);
            }

            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Veículo {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Veículo | {Functions.Serialize(vehicle)}", null);

            var html = await Functions.GetVehiclesHTML();
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

            var id = new Guid(idString);
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

            var html = await Functions.GetVehiclesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Vehicles)))
                target.Emit("StaffVehicles", true, html);
        }
    }
}