using AltV.Net.Elements.Entities;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Commands
{
    public class GameAdministrator
    {
        [Command("o", "/o (mensagem)", GreedyArg = true)]
        public void CMD_o(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline)
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"(( {p.UsuarioBD.Nome}: {mensagem} ))", "#AAC4E5");
        }

        [Command("checar", "/checar (ID ou nome)")]
        public void CMD_checar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            Functions.MostrarStats(player, target);
        }

        [Command("ban", "/ban (ID ou nome) (dias) (motivo)", GreedyArg = true)]
        public void CMD_ban(IPlayer player, string idNome, int dias, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.UsuarioBD.Staff <= p.UsuarioBD.Staff)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                var ban = new Banimento()
                {
                    Data = DateTime.Now,
                    Expiracao = null,
                    Motivo = motivo,
                    Usuario = target.UsuarioBD.Codigo,
                    SocialClub = target.UsuarioBD.SocialClubRegistro,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                    HardwareIdHash = target.HardwareIdHashUltimoAcesso,
                    HardwareIdExHash = target.HardwareIdExHashUltimoAcesso,
                };

                if (dias > 0)
                    ban.Expiracao = DateTime.Now.AddDays(dias);

                context.Banimentos.Add(ban);

                context.Punicoes.Add(new Punicao()
                {
                    Data = DateTime.Now,
                    Duracao = dias,
                    Motivo = motivo,
                    Personagem = target.Codigo,
                    Tipo = TipoPunicao.Ban,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();
            }

            var strBan = dias == 0 ? "permanentemente" : $"por {dias} dia{(dias > 1 ? "s" : string.Empty)}";
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você baniu {target.UsuarioBD.Nome} ({target.Nome}) {strBan}. Motivo: {motivo}");
            Functions.SalvarPersonagem(target, false);
            target.Player.Kick($"{p.UsuarioBD.Nome} baniu você {strBan}. Motivo: {motivo}");
        }

        [Command("banoff", "/banoff (personagem) (dias) (motivo)", GreedyArg = true)]
        public void CMD_banoff(IPlayer player, int personagem, int dias, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
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

            var user = context.Usuarios.FirstOrDefault(x => x.Codigo == per.Usuario);

            if (user.Staff <= p.UsuarioBD.Staff)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var ban = new Banimento()
            {
                Data = DateTime.Now,
                Expiracao = null,
                Motivo = motivo,
                Usuario = user.Codigo,
                SocialClub = user.SocialClubRegistro,
                UsuarioStaff = p.UsuarioBD.Codigo,
            };

            if (dias > 0)
                ban.Expiracao = DateTime.Now.AddDays(dias);

            context.Banimentos.Add(ban);

            context.Punicoes.Add(new Punicao()
            {
                Data = DateTime.Now,
                Duracao = dias,
                Motivo = motivo,
                Personagem = per.Codigo,
                Tipo = TipoPunicao.Ban,
                UsuarioStaff = p.UsuarioBD.Codigo,
            });
            context.SaveChanges();

            var strBan = dias == 0 ? "permanentemente" : $"por {dias} dia{(dias > 1 ? "s" : string.Empty)}";
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você baniu {user.Nome} ({per.Nome}) {strBan}. Motivo: {motivo}");
        }

        [Command("unban", "/unban (usuario)")]
        public void CMD_unban(IPlayer player, int usuario)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                var ban = context.Banimentos.FirstOrDefault(x => x.Usuario == usuario);
                if (ban == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Usuário {usuario} não está banido.");
                    return;
                }

                context.Banimentos.Remove(ban);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você desbaniu {usuario}.");
            Functions.GravarLog(TipoLog.Staff, $"/unban {usuario}", p, null);
        }

        [Command("checaroff", "/checaroff (código ou nome do personagem)", GreedyArg = true)]
        public void CMD_checaroff(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            int.TryParse(idNome, out int codigo);
            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == codigo || x.Nome.ToLower() == idNome.ToLower());
            if (personagem == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum personagem encontrado através da pesquisa: {idNome}.");
                return;
            }

            personagem.UsuarioBD = context.Usuarios.FirstOrDefault(x => x.Codigo == personagem.Usuario);
            Functions.MostrarStats(player, personagem);
        }

        [Command("reviver", "/reviver (ID ou nome)")]
        public void CMD_reviver(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            if (target.Ferimentos.Count == 0 && !target.Player.IsDead && target.TimerFerido == null && target.Player.Health == target.Player.MaxHealth)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está ferido.");
                return;
            }

            target.Player.SetSyncedMetaData("ferido", false);
            if (target.TimerFerido != null)
            {
                target.SetPosition(target.Player.Position, true);
                target.StopAnimation();
            }
            target.Ferimentos = new List<Personagem.Ferimento>();
            target.Player.Emit("Server:CurarPersonagem");
            target.Player.Health = target.Player.MaxHealth;
            target.TimerFerido?.Stop();
            target.TimerFerido = null;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você reviveu {target.Nome}.", notify: true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} reviveu você.", notify: true);

            Functions.GravarLog(TipoLog.Staff, $"/reviver", p, target);
        }
    }
}