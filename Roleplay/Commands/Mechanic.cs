using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Commands
{
    public class Mechanic
    {
        [Command("reparar")]
        public void CMD_reparar(IPlayer player)
        {
            if (player.IsInVehicle)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você deve estar fora do veículo.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p.PecasVeiculares == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui peças veiculares.");
                return;
            }

            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= Global.DistanciaRP
                   && x.Vehicle.Dimension == player.Dimension)
                   .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                   .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo.");
                return;
            }

            player.Emit("Server:freezeEntityPosition", true);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Aguarde 5 segundos.");
            AltAsync.Do(async () =>
            {
                await Task.Delay(5000);
                Alt.EmitAllClients("vehicle:setVehicleFixed", veh.Vehicle);
                p.PecasVeiculares--;
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você consertou o veículo e usou uma peça veicular.");
                player.Emit("Server:freezeEntityPosition", false);
                Functions.SendMessageToNearbyPlayers(player, "conserta o veículo.", TipoMensagemJogo.Ame, 10);
            });
        }

        [Command("pintar")]
        public void CMD_pintar(IPlayer player)
        {
            if (player.IsInVehicle)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você deve estar fora do veículo.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p.PecasVeiculares == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui peças veiculares.");
                return;
            }

            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= Global.DistanciaRP
                   && x.Vehicle.Dimension == player.Dimension)
                   .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                   .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo.");
                return;
            }

            player.Emit("Server:PintarVeiculo", veh.Codigo, 1);
        }
    }
}