using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Commands
{
    public class Government
    {
        [Command("m", "/m (mensagem)", GreedyArg = true)]
        public static void CMD_m(MyPlayer player, string mensagem)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma governamental ou não está em serviço.");
                return;
            }

            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Megafone, 55.0f);
        }

        [Command("gov", "/gov (mensagem)", GreedyArg = true)]
        public static async Task CMD_gov(MyPlayer player, string message)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma governamental ou não está em serviço.");
                return;
            }

            if (!player.FactionFlags.Contains(FactionFlag.GovernmentAdvertisement))
            {
                player.SendMessage(Models.MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            message = Functions.CheckFinalDot(message);
            foreach (var target in Global.SpawnedPlayers)
            {
                target.SendMessage(Models.MessageType.None, $"[{player.Faction.Name}] {{#FFFFFF}}{message}", $"#{player.Faction.Color}");

                if (target.User.Staff != UserStaff.None)
                    target.SendMessage(Models.MessageType.None, $"{player.Character.Name} [{player.SessionId}] ({player.User.Name}) enviou o anúncio governamental.", Global.STAFF_COLOR);
            }

            await player.GravarLog(LogType.GovernmentAdvertisement, message, null);

            if (Global.DiscordClient == null
                || Global.DiscordClient.GetChannel(Global.GovernmentAnnouncementDiscordChannel) is not SocketTextChannel channel)
                return;

            var cor = ColorTranslator.FromHtml($"#{player.Faction.Color}");
            var embedBuilder = new EmbedBuilder
            {
                Title = player.Faction.Name,
                Description = message,
                Color = new Discord.Color(cor.R, cor.G, cor.B),
            };
            embedBuilder.WithFooter($"Enviado em {DateTime.Now}.");

            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("fspawn")]
        public static async Task CMD_fspawn(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção habilitada ou não está em serviço.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.FactionVehicleSpawn
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                return;
            }

            await using var context = new DatabaseContext();
            var veiculos = (await context.Vehicles.Where(x => x.FactionId == player.Character.FactionId && !x.Sold).ToListAsync())
                .OrderBy(x => Convert.ToInt32(Global.Vehicles.Any(y => y.VehicleDB.Id == x.Id)))
                .ThenBy(x => x.Model)
                .ThenBy(x => x.Plate)
                .Select(x => new
                {
                    x.Id,
                    Model = x.Model.ToUpper(),
                    Name = Alt.GetVehicleModelInfo(x.Model).Title,
                    LiveryName = x.GetLiveryName(),
                    x.Plate,
                    InChargeCharacterName = Global.Vehicles.FirstOrDefault(y => y.VehicleDB.Id == x.Id)?.NomeEncarregado ?? "N/A",
                }).ToList();

            if (veiculos.Count == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Sua facção não possui veículos para spawnar.");
                return;
            }

            player.Emit("Server:SpawnarVeiculosFaccao", ponto.Id, player.Faction.Name, Functions.Serialize(veiculos));
        }

        [Command("uniforme")]
        public static void CMD_uniforme(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false))
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental.");
                return;
            }

            if (!Global.Spots.Any(x => x.Type == SpotType.Uniform
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhum ponto de uniforme.");
                return;
            }

            player.Invincible = true;
            player.Emit("AbrirLojaRoupas", (int)player.Character.Sex, 2, (int)player.Faction.Type);
        }

        [Command("freparar")]
        public static async Task CMD_freparar(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x == player.Vehicle && x.Driver == player && x.VehicleDB.FactionId == player.Character.FactionId);
            if (veh == null)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está dirigindo um veículo que pertence a sua facção.");
                return;
            }

            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.FactionVehicleSpawn && veh.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(Models.MessageType.Success, $"Aguarde 5 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new CancellationTokenSource();
            await Task.Delay(5000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    veh = await veh.Reparar();
                    player.ToggleGameControls(true);
                    player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} reparou o veículo {veh.VehicleDB.Model.ToUpper()} {veh.VehicleDB.Plate}.");
                    await player.GravarLog(LogType.FactionVehicleRepair, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("ex", "/ex (extra)")]
        public static void CMD_ex(MyPlayer player, byte extra)
        {
            if (!player.IsInVehicle)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está dentro de um veículo.");
                return;
            }

            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            if (!Global.Spots.Any(x => x.Type == SpotType.FactionVehicleSpawn
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                return;
            }

            player.Vehicle.ToggleExtra(extra, player.Vehicle.IsExtraOn(extra));
            player.SendMessage(Models.MessageType.Success, $"Você alterou extra {extra} do veículo.");
        }

        [Command("rapel")]
        public static void CMD_rapel(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var veh = (MyVehicle)player.Vehicle;
            var modelo = veh?.VehicleDB?.Model?.ToUpper();
            if (player.Seat != 3 && player.Seat != 4
                || !modelo.Equals(VehicleModel.Polmav.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && !modelo.Equals(VehicleModelMods.LSPDHELI.ToString(), StringComparison.CurrentCultureIgnoreCase)
            )
            {
                player.SendMessage(Models.MessageType.Error, "Você não está nos assentos traseiros de um helicóptero apropriado.");
                return;
            }

            player.Emit("TaskRappelFromHeli");
        }

        [Command("mostrardistintivo", "/mostrardistintivo (ID ou nome)")]
        public static void CMD_mostrardistintivo(MyPlayer player, string idNome)
        {
            if (player.Character.Badge == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Você não possui um distintivo.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(Models.MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            target.SendMessage(Models.MessageType.None, $"{{#{player.Faction.Color}}}Distintivo #{player.Character.Badge} de {player.Character.Name}");
            target.SendMessage(Models.MessageType.None, $"{player.Faction.Name} - {player.FactionRank.Name}");
            player.SendMessageToNearbyPlayers(player == target ? "olha seu próprio distintivo." : $"mostra seu distintivo para {target.ICName}.", MessageCategory.Ame, 10);
        }

        [Command("hq", "/hq (mensagem)", GreedyArg = true)]
        public static async Task CMD_hq(MyPlayer player, string message)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            if (!player.FactionFlags.Contains(FactionFlag.HQ))
            {
                player.SendMessage(Models.MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var channel = player.Faction.Type == FactionType.Police ? 911 : 931;
            var slotPl = 0;
            if (player.RadioCommunicatorItem.Channel1 == channel)
                slotPl = 1;
            else if (player.RadioCommunicatorItem.Channel2 == channel)
                slotPl = 2;
            else if (player.RadioCommunicatorItem.Channel3 == channel)
                slotPl = 3;
            else if (player.RadioCommunicatorItem.Channel4 == channel)
                slotPl = 4;
            else if (player.RadioCommunicatorItem.Channel5 == channel)
                slotPl = 5;

            if (slotPl == 0)
            {
                player.SendMessage(Models.MessageType.Error, $"Você não configurou o canal {channel}");
                return;
            }

            message = Functions.CheckFinalDot(message);
            Functions.SendRadioMessage(channel, $"Dispatcher: {message}");

            await player.GravarLog(LogType.Faction, $"/hq {message}", null);
        }

        [Command("br", "/br (tipo). Use /br 0 para visualizar os tipos.")]
        public static void CMD_br(MyPlayer player, int type)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var furnitures = Global.Furnitures.Where(x => x.Category.Equals("barreiras", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (type == 0)
            {
                player.SendMessage(Models.MessageType.Title, "Lista de barreiras");
                foreach (var furniture in furnitures)
                    player.SendMessage(Models.MessageType.None, $"{furnitures.IndexOf(furniture) + 1} - {furniture.Name}");
            }
            else
            {
                if (type > furnitures.Count)
                {
                    player.SendMessage(Models.MessageType.Error, $"Tipo deve ser entre 1 e {furnitures.Count}.");
                    return;
                }

                player.DropFurniture = furnitures[type - 1];
                player.Emit("DropObject", player.DropFurniture.Model, 1);
            }
        }

        [Command("rb")]
        public static async Task CMD_rb(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var barrier = Global.Objects.Where(x =>
                x.CharacterId == player.Character.Id
                && x.Dimension == player.Dimension
                && player.Position.Distance(x.Position) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(x.Position));
            if (barrier == null)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhuma barreira criada por você.");
                return;
            }

            barrier.Destroy();
            player.SendMessageToNearbyPlayers($"retira a barreira do chão.", MessageCategory.Ame, 5);
            await player.GravarLog(LogType.Faction, $"/rb | X: {barrier.Position.X} Y: {barrier.Position.Y} Z: {barrier.Position.Z}", null);
        }

        [Command("rball")]
        public static async Task CMD_rball(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            if (!player.FactionFlags.Contains(FactionFlag.RemoveAllBarriers))
            {
                player.SendMessage(Models.MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var barriers = Global.Objects.Where(x => x.FactionId == player.Faction.Id).ToList();
            if (barriers.Count == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Não há barreiras criadas pela facção.");
                return;
            }

            foreach (var barrier in barriers)
                barrier.Destroy();

            player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} removeu todas as barreiras da facção.");
            await player.GravarLog(LogType.Faction, $"/rball", null);
        }

        [Command("rballme")]
        public static async Task CMD_rballme(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var barriers = Global.Objects.Where(x => x.CharacterId == player.Character.Id).ToList();
            if (barriers.Count == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Não há barreiras criadas por você.");
                return;
            }

            foreach (var barrier in barriers)
                barrier.Destroy();

            player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} removeu todas as suas barreiras.");
            await player.GravarLog(LogType.Faction, $"/rballme", null);
        }
    }
}