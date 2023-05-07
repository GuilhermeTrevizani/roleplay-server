using AltV.Net;
using AltV.Net.Enums;
using Roleplay.Factories;
using Roleplay.Models;
using System.Linq;
using System.Numerics;

namespace Roleplay.Scripts
{
    public class SpotlightScript : IScript
    {
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

            if (veh.VehicleDB.Model.ToUpper() != VehicleModel.Polmav.ToString().ToUpper() 
                && veh.VehicleDB.Model.ToUpper() != VehicleModelMods.LSPDHELI.ToString().ToUpper())
            {
                player.SendMessage(MessageType.Error, "Você não está em um helicóptero policial.");
                return;
            }

            player.Emit("Helicam:Toggle");
        }
    }
}