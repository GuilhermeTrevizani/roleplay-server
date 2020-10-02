using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands
{
    public class Mechanic
    {
        [Command("reparar")]
        public void CMD_reparar(IPlayer player)
        {
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

            Alt.EmitAllClients("vehicle:setVehicleFixed", veh.Vehicle);
            p.PecasVeiculares--;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você reparou o veículo e usou uma peça veicular.");
        }

        [Command("pintar")]
        public void CMD_pintar(IPlayer player)
        {
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