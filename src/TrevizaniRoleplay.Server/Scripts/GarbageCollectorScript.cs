using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class GarbageCollectorScript : IScript
    {
        [Command("pegarlixo")]
        public static void CMD_pegarlixo(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.GarbageCollector || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            if (player.GarbageBagObject != null)
            {
                player.SendMessage(MessageType.Error, "Você está segurando um saco de lixo.");
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

            player.GarbageBagObject = (MyObject)Alt.CreateObject("ng_proc_binbag_01a", player.Position, Rotation.Zero);
            player.GarbageBagObject.AttachToEntity(player, "SKEL_R_Hand", "", new Position(0, -0.1f, -0.4f), Rotation.Zero, false, false);
            player.CollectingSpot = ponto;
            player.SendMessageToNearbyPlayers($"pega um saco de lixo.", MessageCategory.Ame, 5);
        }

        [Command("colocarlixo")]
        public static void CMD_colocarlixo(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.GarbageCollector || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            if (player.GarbageBagObject == null || player.CollectingSpot == null)
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

        [ClientEvent(nameof(SoltarSacoLixo))]
        public void SoltarSacoLixo(MyPlayer player, float x, float y, float z)
        {
            if (player.GarbageBagObject == null || player.CollectingSpot == null)
            {
                player.SendMessage(MessageType.Error, "Você não está segurando um saco de lixo");
                return;
            }

            if (player.Position.Distance(new Position(x, y, z)) > 2)
            {
                player.SendMessage(MessageType.Error, "Você não está na parte de trás de um caminhão de lixo.");
                return;
            }

            player.GarbageBagObject.Destroy();
            player.GarbageBagObject = null;
            player.CollectingSpot.RemoveIdentifier();
            player.CollectSpots.Remove(player.CollectingSpot);
            player.CollectingSpot = null;
            player.SendMessageToNearbyPlayers($"coloca um saco de lixo no caminhão.", MessageCategory.Ame, 5);

            var multiplier = player.Character.DrugItemCategory switch
            {
                ItemCategory.Cocaine or ItemCategory.Crack => 2,
                ItemCategory.Metanfetamina => 2.7,
                _ => 1,
            };

            var extraPayment = Convert.ToInt32(Math.Abs(Global.Parameter.ExtraPaymentGarbagemanValue * multiplier));
            player.ExtraPayment += extraPayment;

            if (player.CollectSpots.Count == 0)
            {
                player.SendMessage(MessageType.Success, "Você realizou todas as coletas. Retorne ao centro de reciclagem e saia de serviço.");
                return;
            }
        }
    }
}