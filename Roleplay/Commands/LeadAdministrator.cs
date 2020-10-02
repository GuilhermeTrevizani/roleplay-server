using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aplicou CK no personagem {target.Nome}. Motivo: {motivo}");
            Functions.SalvarPersonagem(target, false);
            target.Player.Kick($"{p.UsuarioBD.Nome} aplicou CK no seu personagem. Motivo: {motivo}");

            Functions.GravarLog(TipoLog.Staff, $"/ck {motivo}", p, target);
        }

        [Command("unck", "/unck (personagem)")]
        public void CMD_ck(IPlayer player, int personagem)
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu o CK do personagem {per.Nome}.");
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

            Global.Weather = (WeatherType)tempo;
            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                x.Player.SetWeather(Global.Weather);
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

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(target.StatusNamechange == TipoStatusNamechange.Liberado ? "des" : string.Empty)}bloqueou a troca de nome de {target.Nome}.", notify: true);

            Functions.GravarLog(TipoLog.Staff, $"/bloquearnc", p, target);
        }

        [Command("limparchat")]
        public void CMD_limparchat(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.LeadAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                x.Player.Emit("chat:clearMessages");

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você limpou o chat de todos os personagens.", notify: true);
            Functions.GravarLog(TipoLog.Staff, $"/limparchat", p, null);
        }
    }
}