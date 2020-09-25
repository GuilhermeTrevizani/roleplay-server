using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands
{
    public class Taxi 
    {
        [Command("taxiduty", "/taxiduty")]
        public void CMD_taxiduty(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é um taxista.");
                return;
            }

            p.EmTrabalho = !p.EmTrabalho;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.EmTrabalho ? "entrou em" : "saiu de")} serviço como taxista.");
        }

        [Command("taxicha", "/taxicha")]
        public void CMD_taxicha(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista.");
                return;
            }

            if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi.");
                return;
            }

            var chamadas = Global.PersonagensOnline.Where(x => x.AguardandoTipoServico == (int)TipoEmprego.Taxista).OrderBy(x => x.Codigo).ToList();
            if (chamadas.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada para taxistas.");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Chamadas Aguardando Taxistas");
            foreach (var c in chamadas)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Chamada #{c.Codigo}");
        }

        [Command("taxiac", "/taxiac (chamada)")]
        public void CMD_taxiac(IPlayer player, int chamada)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista.");
                return;
            }

            if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi.");
                return;
            }
            
            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == chamada && x.AguardandoTipoServico == (int)TipoEmprego.Taxista);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada com esse código.");
                return;
            }

            player.Emit("Server:SetWaypoint", target.Player.Position.X, target.Player.Position.Y);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você está atendendo a chamada {chamada} e a localização do solicitante foi marcada em seu GPS.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {p.ObterNomeContato(5555555)}: Nosso taxista {p.Nome} está atendendo sua chamada! Placa: {player.Vehicle.NumberplateText}", Constants.CorCelular);
        }
    }
}