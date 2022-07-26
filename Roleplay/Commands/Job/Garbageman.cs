using AltV.Net.Data;
using AltV.Net.Enums;
using Roleplay.Factories;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands.Job
{
    public class Garbageman
    {
        [Command("pegarlixo")]
        public static void CMD_pegarlixo(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Garbageman || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            var ponto = player.CollectSpots
               .Where(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE)
               .OrderBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)))
               .FirstOrDefault();
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de coleta.");
                return;
            }

            player.Emit("Server:PegarSacoLixo");
            player.CollectingSpot = ponto;
            player.SendMessageToNearbyPlayers($"pega um saco de lixo.", MessageCategory.Ame, 5);
        }

        [Command("colocarlixo")]
        public static void CMD_colocarlixo(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Garbageman || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            if (player.CollectingSpot.PosX == 0)
            {
                player.SendMessage(MessageType.Error, "Você não está segurando um saco de lixo.");
                return;
            }

            var veh = Global.Vehicles
                .Where(x => (x.Model == (uint)VehicleModel.Trash || x.Model == (uint)VehicleModel.Trash2)
                && player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 15)
                .OrderBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)))
                .FirstOrDefault();
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum caminhão de lixo.");
                return;
            }

            player.Emit("Server:VerificarSoltarSacoLixo", veh);
        }
    }
}