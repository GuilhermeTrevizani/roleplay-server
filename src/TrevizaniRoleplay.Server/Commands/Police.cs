using AltV.Net;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Commands
{
    public class Police
    {
        [Command("prender", "/prender (ID ou nome) (minutos)")]
        public static async Task CMD_prender(MyPlayer player, string idOrName, int minutes)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.Prison
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de prisão.");
                return;
            }

            var target = player.GetCharacterByIdOrName(idOrName, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            if (minutes <= 0)
            {
                player.SendMessage(MessageType.Error, "Minutos inválidos.");
                return;
            }

            await using var context = new DatabaseContext();
            var jail = new Jail();
            jail.Create(target.Character.Id, player.Character.Id, player.Character.FactionId!.Value, minutes);
            await context.Jails.AddAsync(jail);

            await context.SaveChangesAsync();

            target.Character.SetJailFinalDate(jail.EndDate);

            await target.Save();
            Functions.SendFactionTypeMessage(FactionType.Police, $"{player.FactionRank!.Name} {player.Character.Name} prendeu {target.Character.Name} por {minutes} minuto{(minutes > 1 ? "s" : string.Empty)}.", true, true);
            await target.ListarPersonagens("Prisão", $"{player.Character.Name} prendeu você por {minutes} minuto{(minutes > 1 ? "s" : string.Empty)}.");
        }

        [Command("algemar", "/algemar (ID ou nome)")]
        public static void CMD_algemar(MyPlayer player, string idOrName)
        {
            if (player.Faction?.Type != FactionType.Police)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial.");
                return;
            }

            var target = player.GetCharacterByIdOrName(idOrName, false);
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
        public static async Task CMD_apreender(MyPlayer player, int value, string reason)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.VehicleSeizure
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está em ponto de apreensão de veículos.");
                return;
            }

            if (value <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor da apreensão deve ser maior que 0.");
                return;
            }

            if (reason.Length > 255)
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
            var seizedVehicle = new SeizedVehicle();
            seizedVehicle.Create(veh.VehicleDB.Id, player.Character.Id, value, reason, player.Character.FactionId!.Value);
            await context.SeizedVehicles.AddAsync(seizedVehicle);
            await context.SaveChangesAsync();

            veh.VehicleDB.SetSeizedValue(value);

            await veh.Estacionar(player);

            player.SendFactionMessage($"{player.FactionRank!.Name} {player.Character.Name} apreendeu o veículo de placa {veh.VehicleDB.Plate.ToUpper()} por ${value:N0}.");
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

            var newSpot = new Spot();
            newSpot.Create(SpotType.GarbageCollector, pos.X, pos.Y, pos.Z, 0, 0, 0);

            var blip = (MyBlip)Alt.CreateBlip(false, 4, new Position(newSpot.PosX, newSpot.PosY, newSpot.PosZ), new MyPlayer[] { player });
            blip.Sprite = 225;
            blip.Name = "Radar";
            blip.Color = 59;
            blip.ShortRange = false;
            blip.ScaleXY = new Vector2(0.5f, 0.5f);
            blip.Display = 2;
            blip.SpotId = newSpot.Id;

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(player, MarkerType.MarkerHalo, pos, Global.MainRgba);
            //marker.Scale = new Vector3(10, 10, 10);
            //marker.SpotId = newSpot.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 10, 3);
            colShape.PoliceOfficerCharacterId = player.Character.Id;
            colShape.MaxSpeed = velocidade;
            colShape.SpotId = newSpot.Id;

            player.RadarSpot = newSpot;

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

            player.RadarSpot?.RemoveIdentifier();
            player.RadarSpot = null;

            player.SendMessage(MessageType.Success, $"Você removeu o radar.");
        }

        [Command("confisco")]
        public static void CMD_confisco(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.Confiscation
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de confisco.");
                return;
            }

            var itemsJSON = Functions.Serialize(
                player.Items.Select(x => new
                {
                    x.Id,
                    Name = $"{x.Quantity}x {x.GetName()} {(!string.IsNullOrWhiteSpace(x.Extra) ? $"[{x.GetExtra().Replace("<br/>", ", ")}]" : string.Empty)}",
                    x.Quantity
                })
            );

            player.Emit("PoliceConfiscationShow", itemsJSON);
        }
    }
}