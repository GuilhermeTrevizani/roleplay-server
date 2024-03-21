using AltV.Net;
using AltV.Net.Enums;
using System.Numerics;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class SpotlightScript : IScript
    {
        [Command("spotlight", Aliases = ["holofote"])]
        public static void CMD_spotlight(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var veh = (MyVehicle)player.Vehicle;
            if (veh == null || player.Seat != 1)
            {
                player.SendMessage(MessageType.Error, "Você não é o motorista de um veículo.");
                return;
            }

            if (!veh.VehicleDB.Model.Equals(VehicleModel.Police.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && !veh.VehicleDB.Model.Equals(VehicleModel.Police2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && !veh.VehicleDB.Model.Equals(VehicleModel.Police3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && !veh.VehicleDB.Model.Equals(VehicleModel.Police4.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo policial com holofote.");
                return;
            }

            veh.SpotlightActive = !veh.SpotlightActive;
            player.SendMessage(MessageType.Success, $"Você {(!veh.SpotlightActive ? "des" : string.Empty)}ligou o holofote do veículo.");
            player.Emit("Spotlight:Toggle", veh.SpotlightActive);
            if (!veh.SpotlightActive)
            {
                var spotlight = Global.Spotlights.FirstOrDefault(x => x.Id == player.Vehicle.Id);
                if (spotlight != null)
                {
                    Global.Spotlights.Remove(spotlight);
                    Alt.EmitAllClients("Spotlight:Remove", spotlight.Id);
                }
            }
        }

        [ClientEvent(nameof(SpotlightAdd))]
        public static void SpotlightAdd(MyPlayer player, Vector3 position, Vector3 direction,
            float distance, float brightness, float hardness, float radius, float falloff,
            bool helicoptero)
        {
            if (!helicoptero)
            {
                var veh = (MyVehicle)player.Vehicle;
                if (!(veh?.SpotlightActive ?? false))
                    return;
            }

            var helilight = Global.Spotlights.FirstOrDefault(x => x.Id == player.Vehicle.Id);
            if (helilight == null)
            {
                helilight = new Spotlight
                {
                    Id = player.Vehicle.Id,
                    Position = position,
                    Direction = direction,
                    Player = player.SessionId,
                    Distance = distance,
                    Brightness = brightness,
                    Hardness = hardness,
                    Radius = radius,
                    Falloff = falloff,
                };
                Global.Spotlights.Add(helilight);
            }
            else
            {
                if (helilight.Player != player.SessionId)
                {
                    player.SendMessage(MessageType.Error, "Outro jogador está com a luz do helicóptero ativa.", notify: true);
                    player.Emit("Spotlight:Cancel");
                    return;
                }

                helilight.Position = position;
                helilight.Direction = direction;
            }
            Alt.EmitAllClients("Spotlight:Add", helilight.Id, helilight.Position, helilight.Direction,
                helilight.Distance, helilight.Brightness, helilight.Hardness, helilight.Radius, helilight.Falloff);
        }

        [ClientEvent(nameof(SpotlightRemove))]
        public static void SpotlightRemove(MyPlayer player)
        {
            var spotlight = Global.Spotlights.FirstOrDefault(x => x.Id == player.Vehicle.Id && x.Player == player.SessionId);
            if (spotlight != null)
            {
                Global.Spotlights.Remove(spotlight);
                Alt.EmitAllClients("Spotlight:Remove", spotlight.Id);
            }
        }

        [ClientEvent(nameof(HelicamToggle))]
        public static void HelicamToggle(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var veh = (MyVehicle)player.Vehicle;
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo.");
                return;
            }

            if (!veh.VehicleDB.Model.Equals(VehicleModel.Polmav.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && !veh.VehicleDB.Model.Equals(VehicleModelMods.LSPDHELI.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                player.SendMessage(MessageType.Error, "Você não está em um helicóptero policial.");
                return;
            }

            player.Emit("Helicam:Toggle");
        }
    }
}