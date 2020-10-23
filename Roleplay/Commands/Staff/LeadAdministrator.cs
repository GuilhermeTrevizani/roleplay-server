using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands.Staff
{
    public class LeadAdministrator
    {
        [Command("ck", "/ck (ID ou nome) (motivo)", GreedyArg = true)]
        public void CMD_ck(IPlayer player, string idNome, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo deve ter até 255 caracteres.");
                return;
            }

            target.DataMorte = DateTime.Now;
            target.MotivoMorte = motivo;
            Functions.SalvarPersonagem(target, false);
            Functions.EnviarMensagemStaff($"{p.UsuarioBD.Nome} aplicou CK no personagem {target.Nome}. Motivo: {motivo}", false);
            target.Player.Kick($"{p.UsuarioBD.Nome} aplicou CK no seu personagem. Motivo: {motivo}");

            Functions.GravarLog(TipoLog.Staff, $"/ck {motivo}", p, target);
        }

        [Command("unck", "/unck (personagem)")]
        public void CMD_unck(IPlayer player, int personagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            using var context = new DatabaseContext();
            var per = context.Personagens.FirstOrDefault(x => x.Codigo == personagem);
            if (per == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {personagem} não existe.");
                return;
            }

            per.DataMorte = null;
            per.MotivoMorte = string.Empty;
            context.Personagens.Update(per);
            context.SaveChanges();
            Functions.EnviarMensagemStaff($"{p.UsuarioBD.Nome} removeu o CK do personagem {per.Nome}.", true);
            Functions.GravarLog(TipoLog.Staff, $"/unck {personagem}", p, null);
        }

        [Command("tempo", "/tempo (tempo)")]
        public void CMD_tempo(IPlayer player, int tempo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (tempo < 0 || tempo > 14)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tempo deve ser entre 0 e 14.");
                return;
            }

            Global.Parametros.Weather = (WeatherType)tempo;
            Functions.EnviarMensagemStaff($"{p.UsuarioBD.Nome} alterou o tempo para {Global.Parametros.Weather}.", true);
            foreach (var x in Global.PersonagensOnline)
                x.Player.SetWeather(Global.Parametros.Weather);

            using (var context = new DatabaseContext())
            {
                context.Parametros.Update(Global.Parametros);
                context.SaveChanges();
            }

            Functions.GravarLog(TipoLog.Staff, $"/tempo {tempo}", p, null);
        }

        [Command("bloquearnc", "/bloquearnc (ID ou nome)")]
        public void CMD_bloquearnc(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.StatusNamechange = target.StatusNamechange == TipoStatusNamechange.Liberado ? TipoStatusNamechange.Bloqueado : TipoStatusNamechange.Liberado;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(target.StatusNamechange == TipoStatusNamechange.Liberado ? "des" : string.Empty)}bloqueou a troca de nome de {target.Nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/bloquearnc", p, target);
        }

        [Command("limparchatgeral")]
        public void CMD_limparchatgeral(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
            {
                x.Player.Emit("chat:clearMessages");
                Functions.EnviarMensagem(x.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} limpou o chat de todos.", notify: true);
            }

            Functions.GravarLog(TipoLog.Staff, $"/limparchatgeral", p, null);
        }

        [Command("areparar", "/areparar (veículo)")]
        public void CMD_areparar(IPlayer player, int veiculo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está spawnado.");
                return;
            }

            Functions.EnviarMensagemStaff($"{p.UsuarioBD.Nome} reparou o veículo {veh.Codigo}.", true);
            veh.Reparar();

            Functions.GravarLog(TipoLog.Staff, $"/areparar {veiculo}", p, null);
        }
    }
}