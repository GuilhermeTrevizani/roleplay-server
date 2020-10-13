using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands.Job
{
    public class Emprego 
    {
        [Command("sairemprego")]
        public void CMD_sairemprego(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego == TipoEmprego.Nenhum)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não tem um emprego.");
                return;
            }

            var emp = Global.Empregos.FirstOrDefault(x => x.Tipo == p.Emprego);
            if (player.Position.Distance(emp.Posicao) > Global.DistanciaRP)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está onde você pegou esse emprego.");
                return;
            }

            p.Emprego = TipoEmprego.Nenhum;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu do seu emprego.");
        }

        [Command("emprego")]
        public void CMD_emprego(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego != TipoEmprego.Nenhum)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já tem um emprego.");
                return;
            }

            if (p.FaccaoBD?.Governamental ?? false)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode pegar um emprego pois está em uma facção governamental.");
                return;
            }

            var emprego = TipoEmprego.Nenhum;
            foreach (var c in Global.Empregos)
            {
                if (emprego == TipoEmprego.Nenhum && player.Position.Distance(c.Posicao) <= Global.DistanciaRP)
                    emprego = c.Tipo;
            }

            if (emprego == TipoEmprego.Nenhum)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum local de emprego.");
                return;
            }

            p.Emprego = emprego;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pegou o emprego {Functions.ObterDisplayEnum(emprego)}.");
        }

        [Command("chamadas")]
        public void CMD_chamadas(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Emprego != TipoEmprego.Taxista && p?.Emprego != TipoEmprego.Mecanico) || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista ou mecânico.");
                return;
            }

            if (p.Emprego == TipoEmprego.Taxista)
            {
                if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi.");
                    return;
                }
            }

            var chamadas = Global.PersonagensOnline.Where(x => x.AguardandoTipoServico == (int)p.Emprego).OrderBy(x => x.Codigo).ToList();
            if (chamadas.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada.");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Chamadas Aguardando");
            foreach (var c in chamadas)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Chamada #{c.ID}");
        }

        [Command("atcha", "/atcha (chamada)")]
        public void CMD_atcha(IPlayer player, int chamada)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Emprego != TipoEmprego.Taxista && p?.Emprego != TipoEmprego.Mecanico) || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista ou mecânico.");
                return;
            }

            if (p.Emprego == TipoEmprego.Taxista)
            {
                if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi.");
                    return;
                }
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.ID == chamada && x.AguardandoTipoServico == (int)p.Emprego);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada com esse código.");
                return;
            }

            target.AguardandoTipoServico = 0;
            player.Emit("Server:SetWaypoint", target.Player.Position.X, target.Player.Position.Y);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você está atendendo a chamada {chamada} e a localização do solicitante foi marcada em seu GPS.");
            if (p.Emprego == TipoEmprego.Taxista)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {p.ObterNomeContato(5555555)}: Nosso taxista {p.Nome} está atendendo sua chamada. Placa: {player.Vehicle.NumberplateText}. Celular: {p.Celular}.", Global.CorCelular);
            else if (p.Emprego == TipoEmprego.Mecanico)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {p.ObterNomeContato(5555555)}: Nosso mecânico {p.Nome} está atendendo sua chamada. Celular: {p.Celular}.", Global.CorCelular);
        }
    }
}