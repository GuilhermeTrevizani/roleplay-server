using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands.Job
{
    public class Lixeiro
    {
        [Command("pegarlixo")]
        public void CMD_pegarlixo(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego != TipoEmprego.Lixeiro || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            var ponto = p.PontosColeta
               .Where(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP)
               .OrderBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)))
               .FirstOrDefault();
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de coleta.");
                return;
            }

            player.Emit("Server:PegarSacoLixo");
            p.PontoColetando = ponto;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você pegou um saco de lixo.");
        }

        [Command("colocarlixo")]
        public void CMD_colocarlixo(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego != TipoEmprego.Lixeiro || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é um lixeiro ou não está em serviço.");
                return;
            }

            if (p.PontoColetando.Codigo == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está segurando um saco de lixo.");
                return;
            }

            var veh = Global.Veiculos
                .Where(x => (x.Vehicle.Model == (uint)VehicleModel.Trash || x.Vehicle.Model == (uint)VehicleModel.Trash2)
                && player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= 15)
                .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum caminhão de lixo.");
                return;
            }

            player.Emit("Server:VerificarSoltarSacoLixo", veh.Vehicle);
        }
    }
}