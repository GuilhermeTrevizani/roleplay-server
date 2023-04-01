using AltV.Net;
using AltV.Net.Data;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using Roleplay.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Roleplay.Commands.Job
{
    public class Trucker
    {
        [Command("rotas")]
        public static void CMD_rotas(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Trucker || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um caminhoneiro ou não está em serviço.");
                return;
            }

            var html = string.Empty;
            if (!Global.TruckerLocations.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há rotas criadas.</td></tr>";
            }
            else
            {
                foreach (var truckerLocation in Global.TruckerLocations.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{truckerLocation.Id}</td>
                        <td>{truckerLocation.Name}</td>
                        <td>${truckerLocation.DeliveryValue:N0}</td>
                        <td>{string.Join(", ", truckerLocation.AllowedVehicles)}</td>
                        <td class='text-center'>
                            <button onclick='track({truckerLocation.PosX.ToString().Replace(".", string.Empty).Replace(",", ".")}, {truckerLocation.PosY.ToString().Replace(".", string.Empty).Replace(",", ".")})' type='button' class='btn btn-dark btn-sm'>RASTREAR</button>
                        </td>
                    </tr>";
            }

            player.Emit("TruckerLocations", html);
        }

        [Command("carregarcaixas")]
        public static async Task CMD_carregarcaixas(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Trucker || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um caminhoneiro ou não está em serviço.");
                return;
            }

            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.CollectSpots.Count > 0)
            {
                player.SendMessage(MessageType.Error, "O veículo já está carregado.");
                return;
            }

            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (truckerLocation == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhuma rota.");
                return;
            }

            if (!truckerLocation.AllowedVehicles.Contains(veh.Vehicle.Model.ToUpper()))
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo permitido para esta rota.");
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde {truckerLocation.LoadWaitTime} segundo{(truckerLocation.LoadWaitTime != 1 ? "s" : string.Empty)}. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(truckerLocation.LoadWaitTime * 1000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                foreach (var delivery in Global.TruckerLocationsDeliveries.Where(x => x.TruckerLocationId == truckerLocation.Id))
                {
                    var spot = new Spot
                    {
                        PosX = delivery.PosX,
                        PosY = delivery.PosY,
                        PosZ = delivery.PosZ,
                    };

                    spot.Blip = Alt.CreateBlip(player, 4, new Position(spot.PosX, spot.PosY, spot.PosZ));
                    spot.Blip.Sprite = 1;
                    spot.Blip.Name = "Ponto de Entrega";
                    spot.Blip.Color = 2;
                    spot.Blip.ShortRange = false;
                    spot.Blip.ScaleXY = new Vector2(0.5f, 0.5f);
                    spot.Blip.Display = 2;

                    spot.Marker = MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleSkinny,
                        new Position(spot.PosX, spot.PosY, spot.PosZ - 0.95f),
                        new Vector3(5, 5, 5),
                        Global.MainRgba,
                        player: player.Id);

                    veh.CollectSpots.Add(spot);
                }

                veh.TruckerLocation = truckerLocation;

                player.SendMessage(MessageType.Success, "Você carregou seu veículo. Ao chegar em um ponto de entrega use /entregarcaixas. Você somente receberá o seu pagamento se concluir todas as entregas.");
                player.ToggleGameControls(true);
                player.CancellationTokenSourceAcao = null;
            });
        }

        [Command("cancelarcaixas")]
        public static void CMD_cancelarcaixas(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Trucker || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um caminhoneiro ou não está em serviço.");
                return;
            }

            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.CollectSpots.Count == 0 || veh.TruckerLocation == null)
            {
                player.SendMessage(MessageType.Error, "O veículo não está carregado.");
                return;
            }

            if (player.Position.Distance(new Position(veh.TruckerLocation.PosX, veh.TruckerLocation.PosY, veh.TruckerLocation.PosZ)) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, "Você não está no local que carregou o veículo.");
                return;
            }

            foreach (var collectSpot in veh.CollectSpots)
            {
                collectSpot.Blip.Destroy();
                collectSpot.Marker.Destroy();
            }
            veh.CollectSpots = new List<Spot>();
            veh.TruckerLocation = null;
            player.ExtraPayment = 0;

            player.SendMessage(MessageType.Success, "Você descarregou seu veículo.");
        }

        [Command("entregarcaixas")]
        public static async Task CMD_entregarcaixas(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Trucker || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não é um caminhoneiro ou não está em serviço.");
                return;
            }

            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.CollectSpots.Count == 0 || veh.TruckerLocation == null)
            {
                player.SendMessage(MessageType.Error, "O veículo não está carregado.");
                return;
            }

            var spot = veh.CollectSpots.FirstOrDefault(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (spot == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de entrega.");
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde {veh.TruckerLocation.UnloadWaitTime} segundo{(veh.TruckerLocation.UnloadWaitTime != 1 ? "s" : string.Empty)}. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(veh.TruckerLocation.UnloadWaitTime * 1000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                spot.Blip.Destroy();
                spot.Marker.Destroy();
                veh.CollectSpots.Remove(spot);

                var multiplier = player.Character.DrugItemCategory switch
                {
                    ItemCategory.Heroin => 1.5,
                    ItemCategory.Crack => 2,
                    _ => 1,
                };

                player.ExtraPayment += Convert.ToInt32(Math.Truncate(veh.TruckerLocation.DeliveryValue * multiplier));

                player.SendMessage(MessageType.Success, $"Você realizou uma entrega.");

                if (veh.CollectSpots.Count == 0)
                {
                    player.Character.ExtraPayment += player.ExtraPayment;
                    veh.TruckerLocation = null;
                    player.SendMessage(MessageType.Success, $"Você realizou todas as entregas e a rota foi concluída. ${player.ExtraPayment:N0} foram adicionados no seu próximo pagamento.");
                    player.ExtraPayment = 0;
                }

                player.ToggleGameControls(true);
                player.CancellationTokenSourceAcao = null;
            });
        }
    }
}