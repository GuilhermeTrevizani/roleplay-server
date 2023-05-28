using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using AltV.Net.Shared.Enums;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Commands.Faction
{
    public class Police
    {
        [Command("prender", "/prender (ID ou nome) (minutos)")]
        public static async Task CMD_prender(MyPlayer player, string idNome, int minutos)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.Prisao
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de prisão.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            if (minutos <= 0)
            {
                player.SendMessage(MessageType.Error, "Minutos inválidos.");
                return;
            }

            target.Character.JailFinalDate = DateTime.Now.AddMinutes(minutos);

            await using var context = new DatabaseContext();
            await context.Jails.AddAsync(new Jail
            {
                CharacterId = target.Character.Id,
                PoliceOfficerCharacterId = player.Character.Id,
                EndDate = target.Character.JailFinalDate.Value,
            });
            await context.SaveChangesAsync();

            await target.Save();
            Functions.SendFactionTypeMessage(FactionType.Police, $"{player.FactionRank.Name} {player.Character.Name} prendeu {target.Character.Name} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.", true, true);
            await target.ListarPersonagens("Prisão", $"{player.Character.Name} prendeu você por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.");
        }

        [Command("algemar", "/algemar (ID ou nome)")]
        public static void CMD_algemar(MyPlayer player, string idNome)
        {
            if (player.Faction?.Type != FactionType.Police)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            target.Cuffed = !target.Cuffed;

            if (target.Cuffed)
            {
                target.PlayAnimation("mp_arresting", "idle", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), freeze: true);
                player.SendMessage(MessageType.Success, $"Você algemou {target.ICName}.");
                target.SendMessage(MessageType.Success, $"{player.ICName} algemou você.");
            }
            else
            {
                target.StopAnimation();
                player.SendMessage(MessageType.Success, $"Você desalgemou {target.ICName}.");
                target.SendMessage(MessageType.Success, $"{player.ICName} desalgemou você.");
            }
        }

        [Command("apreender", "/apreender (valor) (motivo)", GreedyArg = true)]
        public static async Task CMD_apreender(MyPlayer player, int valor, string motivo)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.ApreensaoVeiculos
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está em ponto de apreensão de veículos.");
                return;
            }

            if (valor <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor da apreensão deve ser maior que 0.");
                return;
            }

            if (motivo.Length > 255)
            {
                player.SendMessage(MessageType.Error, "Motivo não pode ter mais que 255 caracteres.");
                return;
            }

            var veiculo = (MyVehicle)player.Vehicle;
            if (veiculo == null)
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo.");
                return;
            }

            var veh = (MyVehicle)veiculo.Attached;
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está rebocando nenhum veículo.");
                return;
            }

            if (veh.VehicleDB.FactionId.HasValue || veh.VehicleDB.Job > 0)
            {
                player.SendMessage(MessageType.Error, "Veículo pertence a uma facção ou um emprego.");
                return;
            }

            await using var context = new DatabaseContext();
            await context.SeizedVehicles.AddAsync(new SeizedVehicle
            {
                VehicleId = veh.VehicleDB.Id,
                Reason = motivo,
                PoliceOfficerCharacterId = player.Character.Id,
                Value = valor,
                FactionId = player.Character.FactionId ?? 0,
            });
            await context.SaveChangesAsync();

            veh.VehicleDB.SeizedValue = valor;

            await veh.Estacionar(player);

            player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} apreendeu o veículo de placa {veh.VehicleDB.Plate.ToUpper()} por ${valor:N0}.");
        }

        [Command("radar", "/radar (velocidade)")]
        public static void CMD_radar(MyPlayer player, int velocidade)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (player.RadarSpot != null)
            {
                player.SendMessage(MessageType.Error, "Você já possui um radar ativo.");
                return;
            }

            var pos = player.Position;
            pos.Z -= player.IsInVehicle ? 0.45f : 0.95f;

            player.RadarSpot = new()
            {
                PosX = pos.X,
                PosY = pos.Y,
                PosZ = pos.Z,
                Blip = Alt.CreateBlip(player, 4, pos)
            };
            player.RadarSpot.Blip.Sprite = 225;
            player.RadarSpot.Blip.Name = "Radar";
            player.RadarSpot.Blip.Color = 59;
            player.RadarSpot.Blip.ShortRange = false;
            player.RadarSpot.Blip.ScaleXY = new Vector2(0.5f, 0.5f);
            player.RadarSpot.Blip.Display = 2;

            player.RadarSpot.Marker = Alt.CreateMarker(player, MarkerType.MarkerHalo, pos, Global.MainRgba);
            player.RadarSpot.Marker.Scale = new Vector3(10, 10, 10);

            player.RadarSpot.ColShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 10, 3);
            player.RadarSpot.ColShape.PoliceOfficerCharacterId = player.Character.Id;
            player.RadarSpot.ColShape.MaxSpeed = velocidade;

            player.SendMessage(MessageType.Success, $"Você criou um radar com a velocidade {velocidade}.");
        }

        [Command("radaroff")]
        public static void CMD_radaroff(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (player.RadarSpot == null)
            {
                player.SendMessage(MessageType.Error, "Você não possui um radar ativo.");
                return;
            }

            player.RadarSpot?.ColShape?.Destroy();
            player.RadarSpot?.Blip?.Destroy();
            player.RadarSpot?.Marker?.Destroy();
            player.RadarSpot = null;

            player.SendMessage(MessageType.Success, $"Você removeu o radar.");
        }

        [Command("spotlight", Aliases = new string[] { "holofote" })]
        public static void CMD_spotlight(MyPlayer player)
        {
            player.SendMessage(MessageType.Error, "Este comando está inativo.");
            return;

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

            if (veh.VehicleDB.Model.ToUpper() != VehicleModel.Police.ToString().ToUpper()
                && veh.VehicleDB.Model.ToUpper() != VehicleModel.Police2.ToString().ToUpper()
                && veh.VehicleDB.Model.ToUpper() != VehicleModel.Police3.ToString().ToUpper()
                && veh.VehicleDB.Model.ToUpper() != VehicleModel.Police4.ToString().ToUpper()
                && veh.VehicleDB.Model.ToUpper() != VehicleModelMods.PSCOUT.ToString().ToUpper()
                && veh.VehicleDB.Model.ToUpper() != VehicleModelMods.POLICE42.ToString().ToUpper())
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

        [Command("confisco")]
        public static void CMD_confisco(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.Confisco
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de confisco.");
                return;
            }

            var itemsJSON = JsonSerializer.Serialize(
                player.Items.Select(x => new
                {
                    x.Id,
                    Name = $"{x.Quantity}x {x.Name} {(!string.IsNullOrWhiteSpace(x.Extra) ? $"[{Functions.GetItemExtra(x).Replace("<br/>", ", ")}]" : string.Empty)}",
                    x.Quantity
                })
            );

            player.Emit("PoliceConfiscationShow", itemsJSON);
        }
    }
}