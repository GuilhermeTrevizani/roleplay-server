using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Commands
{
    public class Staff
    {
        #region Moderator
        [Command("ir", "/ir (ID ou nome)")]
        public void CMD_ir(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            p.LimparIPLs();
            p.IPLs = target.IPLs;
            p.SetarIPLs();
            var pos = target.Player.Position;
            pos.X += 2;
            player.Position = pos;
            player.Dimension = target.Player.Dimension;
        }

        [Command("trazer", "/trazer (ID ou nome)")]
        public void CMD_trazer(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            target.LimparIPLs();
            target.IPLs = p.IPLs;
            target.SetarIPLs();
            var pos = player.Position;
            pos.X += 2;
            target.Player.Position = pos;
            target.Player.Dimension = player.Dimension;
        }

        [Command("tp", "/tp (ID ou nome) (ID ou nome)")]
        public void CMD_tp(IPlayer player, string idNome, string idNomeDestino)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            var targetDest = Functions.ObterPersonagemPorIdNome(player, idNomeDestino, false);
            if (targetDest == null)
                return;

            target.LimparIPLs();
            target.IPLs = targetDest.IPLs;
            target.SetarIPLs();
            var pos = targetDest.Player.Position;
            pos.X += 2;
            target.Player.Position = pos;
            target.Player.Dimension = targetDest.Player.Dimension;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} teleportou você para {targetDest.Nome}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você teleportou {target.Nome} para {targetDest.Nome}.");
        }

        [Command("vw", "/vw (ID ou nome) (vw)")]
        public void CMD_vw(IPlayer player, string idNome, int vw)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Dimension = vw;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua dimensão para {vw}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a dimensão de {target.Nome} para {vw}.");
        }

        [Command("a", "/a (mensagem)", GreedyArg = true)]
        public void CMD_a(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.UsuarioBD.Staff != TipoStaff.Nenhum))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"(( {Functions.ObterDisplayEnum(p.UsuarioBD.Staff)} {p.UsuarioBD.Nome}: {mensagem} ))", "#33EE33");
        }

        [Command("o", "/o (mensagem)", GreedyArg = true)]
        public void CMD_o(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline)
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"(( {p.UsuarioBD.Nome}: {mensagem} ))", "#AAC4E5");
        }

        [Command("kick", "/kick (ID ou nome) (motivo)", GreedyArg = true)]
        public void CMD_kick(IPlayer player, string idNome, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            using (var context = new DatabaseContext())
            {
                context.Punicoes.Add(new Punicao()
                {
                    Data = DateTime.Now,
                    Duracao = 0,
                    Motivo = motivo,
                    Personagem = target.Codigo,
                    Tipo = TipoPunicao.Kick,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você kickou {target.Nome}. Motivo: {motivo}");
            Functions.SalvarPersonagem(target, false);
            target.Player.Kick($"{p.UsuarioBD.Nome} kickou você. Motivo: {motivo}");
        }

        [Command("irveh", "/irveh (código)")]
        public void CMD_irveh(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está spawnado.");
                return;
            }

            p.LimparIPLs();
            var pos = veh.Vehicle.Position;
            pos.X += 5;
            player.Position = pos;
            player.Dimension = veh.Vehicle.Dimension;
        }

        [Command("trazerveh", "/trazerveh (código)")]
        public void CMD_trazerveh(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (p.Dimensao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não é possível usar esse comando em um interior.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está spawnado.");
                return;
            }

            var pos = player.Position;
            pos.X += 5;
            veh.Vehicle.Position = pos;
            veh.Vehicle.Dimension = player.Dimension;
        }

        [Command("aduty")]
        public void CMD_aduty(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            p.IsEmTrabalhoAdministrativo = !p.IsEmTrabalhoAdministrativo;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.IsEmTrabalhoAdministrativo ? "entrou em" : "saiu de")} serviço administrativo.");
        }

        [Command("listasos")]
        public void CMD_listasos(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (Global.SOSs.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhum SOS pendente.");
                return;
            }

            foreach (var x in Global.SOSs.OrderBy(x => x.Data))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"SOS de {x.NomePersonagem} [{x.IDPersonagem}] ({x.NomeUsuario}) | {x.Data}");
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, x.Mensagem);
            }
        }

        [Command("aj", "/aj (código)")]
        public void CMD_aj(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var sos = Global.SOSs.FirstOrDefault(x => x.IDPersonagem == codigo);
            if (sos == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"SOS {codigo} não existe.");
                return;
            }

            var target = sos.Verificar(p.Usuario);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador do SOS não está conectado.");
                return;
            }

            sos.DataResposta = DateTime.Now;
            sos.UsuarioStaff = p.UsuarioBD.Codigo;
            sos.TipoResposta = TipoRespostaSOS.Aceito;

            using var context = new DatabaseContext();
            context.SOSs.Update(sos);
            context.SaveChanges();
            Global.SOSs.Remove(sos);

            p.UsuarioBD.QuantidadeSOSAceitos++;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou o SOS de {sos.NomePersonagem} [{sos.IDPersonagem}] ({sos.NomeUsuario}).");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} aceitou seu SOS.");
        }

        [Command("rj", "/rj (código)")]
        public void CMD_rj(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var sos = Global.SOSs.FirstOrDefault(x => x.IDPersonagem == codigo);
            if (sos == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"SOS {codigo} não existe.");
                return;
            }

            var target = sos.Verificar(p.Usuario);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador do SOS não está conectado.");
                return;
            }

            sos.DataResposta = DateTime.Now;
            sos.UsuarioStaff = p.UsuarioBD.Codigo;
            sos.TipoResposta = TipoRespostaSOS.Rejeitado;

            using var context = new DatabaseContext();
            context.SOSs.Update(sos);
            context.SaveChanges();
            Global.SOSs.Remove(sos);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você rejeitou o SOS de {sos.NomePersonagem} [{sos.IDPersonagem}] ({sos.NomeUsuario}).");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} rejeitou seu SOS.");
        }
        #endregion Staff 1

        #region Game Administrator
        [Command("vida", "/vida (ID ou nome) (vida)")]
        public void CMD_vida(IPlayer player, string idNome, int vida)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (vida < 1 || vida > 100)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Vida deve ser entre 1 e 100.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Health = (ushort)(vida + 100);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua vida para {vida}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a vida de {target.Nome} para {vida}.");
            Functions.GravarLog(TipoLog.Staff, $"/vida {vida}", p, target);
        }

        [Command("colete", "/colete (ID ou nome) (colete)")]
        public void CMD_colete(IPlayer player, string idNome, int colete)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.GameAdministrator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (colete < 1 || colete > 100)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Colete deve ser entre 1 e 100.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Armor = (ushort)colete;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu colete para {colete}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o colete de {target.Nome} para {colete}.");
            Functions.GravarLog(TipoLog.Staff, $"/colete {colete}", p, target);
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

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
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
        #endregion Staff 2

        #region Lead Administrator
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

        [Command("acurar", "/acurar (ID ou nome)")]
        public void CMD_acurar(IPlayer player, string idNome)
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

            if (target.Ferimentos.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está ferido.");
                return;
            }

            target.Ferimentos = new List<Personagem.Ferimento>();
            target.Player.Emit("Server:CurarPersonagem");
            target.Player.Health = 200;

            if (target.TimerFerido != null)
            {
                target.TimerFerido?.Stop();
                target.TimerFerido = null;
                target.Player.Emit("player:toggleFreeze", false);
                target.StopAnimation();
                target.Player.Armor = 0;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você curou {target.Nome}.", notify: true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} curou você.", notify: true);

            Functions.GravarLog(TipoLog.Staff, $"/acurar", p, target);
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
        #endregion

        #region Manager
        [Command("gmx")]
        public void CMD_gmx(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.EtapaPersonalizacao == TipoEtapaPersonalizacao.Concluido))
            {
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} reiniciará o servidor.");
                Functions.SalvarPersonagem(pl);
            }
        }

        [Command("proximo")]
        public void CMD_proximo(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var isTemAlgoProximo = false;
            var distanceVer = 5f;

            foreach (var x in Global.Blips)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Blip {x.Codigo} | Inativo: {(x.Inativo ? "SIM" : "NÃO")}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Propriedades)
            {
                if (player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Propriedade {x.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Pontos)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Ponto {x.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Armarios)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Armário {x.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Veiculos)
            {
                if (player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Veículo {x.Codigo} | Modelo: {x.Modelo.ToUpper()}");
                    isTemAlgoProximo = true;
                }
            }

            if (!isTemAlgoProximo)
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum item.");
        }

        [Command("cblip", "/cblip (tipo) (cor) (nome)", GreedyArg = true)]
        public void CMD_cblip(IPlayer player, int tipo, int cor, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (tipo < 0 || tipo > 744)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 744.");
                return;
            }

            if (cor < 0 || cor > 85)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cor deve ser entre 1 e 85.");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            var blip = new Entities.Blip()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Cor = cor,
                Tipo = tipo,
                Nome = nome,
            };

            using (var context = new DatabaseContext())
            {
                context.Blips.Add(blip);
                context.SaveChanges();
            }

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                blip.CriarIdentificador(x.Player);

            Global.Blips.Add(blip);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} criado com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/cblip {blip.Codigo}", p, null);
        }

        [Command("rblip", "/rblip (código)")]
        public void CMD_rblip(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Codigo == codigo);
            if (blip == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Blip {codigo} não existe.");
                return;
            }

            using (var context = new DatabaseContext())
                context.Database.ExecuteSqlRaw($"DELETE FROM Blips WHERE Codigo = {codigo}");

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                blip.DeletarIdentificador(x.Player);

            Global.Blips.Remove(blip);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} removido com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rblip {blip.Codigo}", p, null);
        }

        [Command("cfac", "/cfac (tipo) (nome)", GreedyArg = true)]
        public void CMD_cfac(IPlayer player, int tipo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoFaccao), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido.");
                return;
            }

            var faccao = new Faccao()
            {
                Nome = nome,
                Cor = "FFFFFF",
                Tipo = (TipoFaccao)tipo,
            };

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Add(faccao);
                context.SaveChanges();
            }

            Global.Faccoes.Add(faccao);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você criou a facção {faccao.Codigo}.");

            Functions.GravarLog(TipoLog.Staff, $"/cfac {faccao.Codigo}", p, null);
        }

        [Command("efacnome", "/efacnome (código) (nome)", GreedyArg = true)]
        public void CMD_efacnome(IPlayer player, int codigo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Nome = nome;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o nome da facção {faccao.Codigo} para {nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/efacnome {faccao.Codigo} {nome}", p, null);
        }

        [Command("efactipo", "/efactipo (código) (tipo)")]
        public void CMD_efactipo(IPlayer player, int codigo, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoFaccao), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Tipo = (TipoFaccao)tipo;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o tipo da facção {faccao.Codigo} para {tipo}.");
            Functions.GravarLog(TipoLog.Staff, $"/efactipo {faccao.Codigo} {tipo}", p, null);
        }

        [Command("efaccor", "/efaccor (código) (cor)")]
        public void CMD_efaccor(IPlayer player, int codigo, string cor)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (cor.Length > 6)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cor deve ter até 6 caracteres.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Cor = cor;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a cor da facção {faccao.Codigo} para {cor}.");
            Functions.GravarLog(TipoLog.Staff, $"/efaccor {faccao.Codigo} {cor}", p, null);
        }

        [Command("efacrankgestor", "/efacrankgestor (código) (rank)")]
        public void CMD_efacrankgestor(IPlayer player, int codigo, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (rank <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank deve ser maior que 0.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankGestor = rank;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o rank gestor da facção {faccao.Codigo} para {rank}.");
            Functions.GravarLog(TipoLog.Staff, $"/efacrankgestor {faccao.Codigo} {rank}", p, null);
        }

        [Command("efacranklider", "/efacranklider (código) (rank)")]
        public void CMD_efacranklider(IPlayer player, int codigo, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (rank <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank deve ser maior que 0.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankLider = rank;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o rank líder da facção {faccao.Codigo} para {rank}.");
            Functions.GravarLog(TipoLog.Staff, $"/efacranklider {faccao.Codigo} {rank}", p, null);
        }

        [Command("rfac", "/rfac (código)")]
        public void CMD_rfac(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                if (context.Personagens.Any(x => x.Faccao == codigo))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não é possível remover a facção {codigo} pois existem personagens nela.");
                    return;
                }

                context.Database.ExecuteSqlRaw($"DELETE FROM Faccoes WHERE Codigo = {codigo}");
                context.Database.ExecuteSqlRaw($"DELETE FROM `Ranks` WHERE Faccao = {codigo}");
            }

            Global.Faccoes.Remove(faccao);
            Global.Ranks.RemoveAll(x => x.Faccao == codigo);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu a facção {faccao.Codigo}.");
            Functions.GravarLog(TipoLog.Staff, $"/rfac {faccao.Codigo}", p, null);
        }

        [Command("faccoes")]
        public void CMD_faccoes(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (Global.Faccoes.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não existe nenhuma facção.");
                return;
            }

            var html = $@"<div class='box-header with-border'>
                <h3>{Constants.NomeServidor} • Facções<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise as facções...' />
            <br/><table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>Tipo</th>
                        <th>Cor</th>
                        <th>Rank Gestor</th>
                        <th>Rank Líder</th>
                        <th>Slots</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in Global.Faccoes)
                html += $@"<tr class='pesquisaitem'><td>{x.Codigo}</td><td>{x.Nome}</td><td>{Functions.ObterDisplayEnum(x.Tipo)}</td><td><span style='color:#{x.Cor}'>#{x.Cor}</span></td><td>{Global.Ranks.FirstOrDefault(y => y.Faccao == x.Codigo && y.Codigo == x.RankGestor)?.Nome ?? string.Empty} [{x.RankGestor}]</td><td>{Global.Ranks.FirstOrDefault(y => y.Faccao == x.Codigo && y.Codigo == x.RankLider)?.Nome ?? string.Empty} [{x.RankLider}]</td><td>{x.Slots}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("crank", "/crank (facção) (salário) (nome)", GreedyArg = true)]
        public void CMD_crank(IPlayer player, int fac, int salario, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (!((int)p?.UsuarioBD?.Staff >= (int)TipoStaff.Manager || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (salario < 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Salário não pode ser negativo.");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe.");
                return;
            }

            var rank = new Rank()
            {
                Faccao = fac,
                Nome = nome,
                Salario = salario,
            };

            using (var context = new DatabaseContext())
            {
                var ranks = context.Ranks.Where(x => x.Faccao == fac).ToList();
                rank.Codigo = ranks.Count == 0 ? 1 : ranks.Max(x => x.Codigo) + 1;
                context.Ranks.Add(rank);
                context.SaveChanges();
            }

            Global.Ranks.Add(rank);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você criou o rank {rank.Nome} [{rank.Codigo}] da facção {faction.Nome} [{faction.Codigo}].");
            Functions.GravarLog(TipoLog.Staff, $"/crank {faction.Codigo} {rank.Codigo}", p, null);
        }

        [Command("rrank", "/rrank (facção) (código) (nome)")]
        public void CMD_rrank(IPlayer player, int fac, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (!((int)p?.UsuarioBD?.Staff >= (int)TipoStaff.Manager || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} da facção {fac} não existe.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                if (context.Personagens.Any(x => x.Faccao == fac && x.Rank == rank))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não é possível remover o rank {rank} da facção {fac} pois existem personagens nele.");
                    return;
                }

                context.Ranks.Remove(rk);
                context.SaveChanges();
            }

            Global.Ranks.Remove(rk);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu o rank {rk.Nome} [{rk.Codigo}] da facção {fac}.");
            Functions.GravarLog(TipoLog.Staff, $"/rrank {fac} {rk.Codigo}", p, null);
        }

        [Command("eranknome", "/eranknome (facção) (código) (nome)", GreedyArg = true)]
        public void CMD_eranknome(IPlayer player, int fac, int rank, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (!((int)p?.UsuarioBD?.Staff >= (int)TipoStaff.Manager || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} da facção {fac} não existe.");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            Global.Ranks[Global.Ranks.IndexOf(rk)].Nome = nome;

            using (var context = new DatabaseContext())
            {
                context.Ranks.Update(Global.Ranks[Global.Ranks.IndexOf(rk)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o nome do rank {rank} da facção {fac} para {nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/eranknome {fac} {rank} {nome}", p, null);
        }

        [Command("ranks", "/ranks (facção)")]
        public void CMD_ranks(IPlayer player, int fac)
        {
            var p = Functions.ObterPersonagem(player);
            if (!((int)p?.UsuarioBD?.Staff >= (int)TipoStaff.Manager || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe.");
                return;
            }

            var ranks = Global.Ranks.Where(x => x.Faccao == fac).OrderBy(x => x.Codigo).ToList();
            if (ranks.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não existe nenhum rank para a facção {fac}.");
                return;
            }

            var html = $@"<div class='box-header with-border'>
                <h3>{faction.Nome} • Ranks<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os ranks...' />
            <br/><table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>Salário</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in ranks)
                html += $@"<tr class='pesquisaitem'><td>{x.Codigo}</td><td>{x.Nome}</td><td>${x.Salario:N0}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("setstaff", "/setstaff (ID ou nome) (nível)")]
        public void CMD_setstaff(IPlayer player, string idNome, int staff)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.GetValues(typeof(TipoStaff)).Cast<TipoStaff>().Any(x => (int)x == staff))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Staff {staff} não existe.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            var stf = (TipoStaff)staff;
            target.UsuarioBD.Staff = stf;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu nível staff para {stf} [{staff}].");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o nível staff de {target.UsuarioBD.Nome} para {stf} [{staff}].");
            Functions.GravarLog(TipoLog.Staff, $"/staff {staff}", p, target);
        }

        [Command("lider", "/lider (ID ou nome) (facção)")]
        public void CMD_lider(IPlayer player, string idNome, int fac)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe.");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == faccao.RankLider);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank líder ({faccao.RankLider}) da facção {fac} não existe.");
                return;
            }

            target.Faccao = fac;
            target.Rank = faccao.RankLider;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} te deu a liderança da facção {faccao.Nome} [{faccao.Codigo}].");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu a liderança da facção {faccao.Nome} [{faccao.Codigo}] para {target.Nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/lider {fac}", p, target);
        }

        [Command("cprop", "/cprop (interior) (valor)")]
        public void CMD_cprop(IPlayer player, int interior, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), interior))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            var saida = Functions.ObterPosicaoPorInterior((TipoInterior)interior);

            var prop = new Propriedade()
            {
                Interior = (TipoInterior)interior,
                EntradaPosX = player.Position.X,
                EntradaPosY = player.Position.Y,
                EntradaPosZ = player.Position.Z,
                Valor = valor,
                SaidaPosX = saida.X,
                SaidaPosY = saida.Y,
                SaidaPosZ = saida.Z,
                Dimensao = player.Dimension,
            };

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Add(prop);
                context.SaveChanges();
            }

            prop.CriarIdentificador();

            Global.Propriedades.Add(prop);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} criada com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/cprop {prop.Codigo} {prop.Valor}", p, null);
        }

        [Command("rprop", "/rprop (código)")]
        public void CMD_rprop(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe.");
                return;
            }

            if (prop.Personagem > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} possui um dono.");
                return;
            }

            using (var context = new DatabaseContext())
                context.Database.ExecuteSqlRaw($"DELETE FROM Propriedades WHERE Codigo = {codigo}");

            prop.DeletarIdentificador();

            Global.Propriedades.Remove(prop);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} removida com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rprop {prop.Codigo}", p, null);
        }

        [Command("epropvalor", "/epropvalor (código) (valor)")]
        public void CMD_epropvalor(IPlayer player, int codigo, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].Valor = valor;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o valor da propriedade {prop.Codigo} para {valor}.");
            Functions.GravarLog(TipoLog.Staff, $"/epropvalor {prop.Codigo} {valor}", p, null);
        }

        [Command("epropint", "/epropint (código) (interior)")]
        public void CMD_epropint(IPlayer player, int codigo, int interior)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), interior))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido.");
                return;
            }

            var pos = Functions.ObterPosicaoPorInterior((TipoInterior)interior);
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].Interior = (TipoInterior)interior;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosX = pos.X;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosY = pos.Y;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosZ = pos.Z;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o interior da propriedade {prop.Codigo} para {interior}.");
            Functions.GravarLog(TipoLog.Staff, $"/epropinterior {prop.Codigo} {interior}", p, null);
        }

        [Command("eproppos", "/eproppos (código)")]
        public void CMD_eproppos(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe.");
                return;
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosX = player.Position.X;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosY = player.Position.Y;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosZ = player.Position.Z;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].Dimensao = player.Dimension;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a posição da propriedade {prop.Codigo} para sua posição atual (X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z} D: {player.Dimension}).");
            Functions.GravarLog(TipoLog.Staff, $"/eproppos {prop.Codigo} X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z} D: {player.Dimension}", p, null);
        }

        [Command("irprop", "/irprop (código)")]
        public void CMD_irprop(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe.");
                return;
            }

            p.LimparIPLs();
            player.Dimension = 0;
            player.Position = new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você foi até a propriedade {prop.Codigo}.");
        }

        [Command("irblip", "/irblip (código)")]
        public void CMD_irblip(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Codigo == codigo);
            if (blip == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Blip {codigo} não existe.");
                return;
            }

            p.LimparIPLs();
            player.Dimension = 0;
            player.Position = new Position(blip.PosX, blip.PosY, blip.PosZ);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você foi até o blip {blip.Codigo}.");
        }

        [Command("cpreco", "/cpreco (tipo) (valor) (nome)", GreedyArg = true)]
        public void CMD_cpreco(IPlayer player, int tipo, int valor, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoPreco), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            var tp = (TipoPreco)tipo;
            if (tp == TipoPreco.CarrosMotos)
            {
                if (!Enum.GetValues(typeof(VehicleModel)).Cast<VehicleModel>().Any(x => x.ToString().ToLower() == nome.ToLower()))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Modelo {nome} não existe.");
                    return;
                }
            }

            using (var context = new DatabaseContext())
            {
                var preco = Global.Precos.FirstOrDefault(x => x.Tipo == tp && x.Nome.ToLower() == nome.ToLower());
                if (preco == null)
                {
                    preco = new Preco()
                    {
                        Tipo = tp,
                        Nome = nome,
                        Valor = valor,
                    };
                    Global.Precos.Add(preco);
                    context.Precos.Add(preco);
                }
                else
                {
                    preco.Valor = valor;
                    context.Precos.Update(preco);
                }

                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {Functions.ObterDisplayEnum(tp)} ({tipo}) e nome {nome} criado/editado com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/cpreco {tipo} {nome} {valor}", p, null);
        }

        [Command("rpreco", "/rpreco (tipo) (nome)")]
        public void CMD_rpreco(IPlayer player, int tipo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var tp = (TipoPreco)tipo;
            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == tp && x.Nome.ToLower() == nome.ToLower());
            if (preco == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Preço com tipo {Functions.ObterDisplayEnum(tp)} ({tipo}) e nome {nome} não existe.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                context.Precos.Remove(preco);
                context.SaveChanges();
            }

            Global.Precos.Remove(preco);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {Functions.ObterDisplayEnum(tp)} ({tipo}) e nome {nome} removido com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rpreco {tipo} {nome}", p, null);
        }

        [Command("cponto", "/cponto (tipo)")]
        public void CMD_cponto(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoPonto), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido.");
                return;
            }

            var ponto = new Ponto()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Tipo = (TipoPonto)tipo,
            };

            if (ponto.Tipo == TipoPonto.SpawnVeiculosFaccao || ponto.Tipo == TipoPonto.ApreensaoVeiculos)
                ponto.Configuracoes = JsonConvert.SerializeObject(player.Rotation);

            using (var context = new DatabaseContext())
            {
                context.Pontos.Add(ponto);
                context.SaveChanges();
            }

            ponto.CriarIdentificador();

            Global.Pontos.Add(ponto);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} criado com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/cponto {ponto.Codigo}", p, null);
        }

        [Command("rponto", "/rponto (código)")]
        public void CMD_rponto(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var ponto = Global.Pontos.FirstOrDefault(x => x.Codigo == codigo);
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Ponto {codigo} não existe.");
                return;
            }
            using (var context = new DatabaseContext())
                context.Database.ExecuteSqlRaw($"DELETE FROM Pontos WHERE Codigo = {codigo}");

            ponto.DeletarIdentificador();

            Global.Pontos.Remove(ponto);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} removido com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rponto {ponto.Codigo}", p, null);
        }

        [Command("irponto", "/irponto (código)")]
        public void CMD_irponto(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var ponto = Global.Pontos.FirstOrDefault(x => x.Codigo == codigo);
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Ponto {codigo} não existe.");
                return;
            }

            p.LimparIPLs();
            player.Dimension = 0;
            player.Position = new Position(ponto.PosX, ponto.PosY, ponto.PosZ);
        }

        [Command("eranksalario", "/eranksalario (facção) (código) (salário)")]
        public void CMD_eranksalario(IPlayer player, int fac, int rank, int salario)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} da facção {fac} não existe.");
                return;
            }

            if (salario <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Salário deve ser maior que 0.");
                return;
            }

            Global.Ranks[Global.Ranks.IndexOf(rk)].Salario = salario;

            using (var context = new DatabaseContext())
            {
                context.Ranks.Update(Global.Ranks[Global.Ranks.IndexOf(rk)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o salário do rank {rank} da facção {fac} para ${salario:N0}.");
            Functions.GravarLog(TipoLog.Staff, $"/eranksalario {fac} {rank} {salario}", p, null);
        }

        [Command("eblipinativo", "/eblipinativo (código)")]
        public void CMD_eblipinativo(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Codigo == codigo);
            if (blip == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Blip {codigo} não existe.");
                return;
            }

            blip.Inativo = !blip.Inativo;

            using (var context = new DatabaseContext())
            {
                context.Blips.Update(blip);
                context.SaveChanges();
            }

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                blip.CriarIdentificador(x.Player);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(blip.Inativo ? "in" : string.Empty)}ativou o blip {blip.Codigo}.");
            Functions.GravarLog(TipoLog.Staff, $"/eblipinativo {blip.Codigo}", p, null);
        }

        [Command("carm", "/carm (facção)")]
        public void CMD_carm(IPlayer player, int faccao)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == faccao);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {faccao} não existe.");
                return;
            }

            var armario = new Armario()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Faccao = faccao,
                Dimensao = player.Dimension,
            };

            using (var context = new DatabaseContext())
            {
                context.Armarios.Add(armario);
                context.SaveChanges();
            }

            armario.CriarIdentificador();

            Global.Armarios.Add(armario);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Armário {armario.Codigo} criado com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/carm {armario.Codigo} {faccao}", p, null);
        }

        [Command("rarm", "/rarm (código)")]
        public void CMD_rarm(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => x.Codigo == codigo);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {codigo} não existe.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                context.Database.ExecuteSqlRaw($"DELETE FROM Armarios WHERE Codigo = {codigo}");
                context.Database.ExecuteSqlRaw($"DELETE FROM ArmariosItens WHERE Codigo = {codigo}");
            }

            armario.DeletarIdentificador();

            Global.Armarios.Remove(armario);
            Global.ArmariosItens.RemoveAll(x => x.Codigo == armario.Codigo);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Armário {armario.Codigo} removido com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rarm {armario.Codigo}", p, null);
        }

        [Command("earmfac", "/earmfac (código) (facção)")]
        public void CMD_earmfac(IPlayer player, int codigo, int faccao)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => x.Codigo == codigo);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {codigo} não existe.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == faccao);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {faccao} não existe.");
                return;
            }

            Global.Armarios[Global.Armarios.IndexOf(armario)].Faccao = faccao;

            using (var context = new DatabaseContext())
            {
                context.Armarios.Update(Global.Armarios[Global.Armarios.IndexOf(armario)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a facção do armário {armario.Codigo} para {faccao}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmariofac {armario.Codigo} {faccao}", p, null);
        }

        [Command("earmpos", "/earmpos (código)")]
        public void CMD_earmpos(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => x.Codigo == codigo);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {codigo} não existe.");
                return;
            }

            Global.Armarios[Global.Armarios.IndexOf(armario)].PosX = player.Position.X;
            Global.Armarios[Global.Armarios.IndexOf(armario)].PosY = player.Position.Y;
            Global.Armarios[Global.Armarios.IndexOf(armario)].PosX = player.Position.X;
            Global.Armarios[Global.Armarios.IndexOf(armario)].Dimensao = player.Dimension;

            using (var context = new DatabaseContext())
            {
                context.Armarios.Update(Global.Armarios[Global.Armarios.IndexOf(armario)]);
                context.SaveChanges();
            }

            Global.Armarios[Global.Armarios.IndexOf(armario)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a posição do armário {armario.Codigo} para sua posição atual (X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z} D: {player.Dimension}).");
            Functions.GravarLog(TipoLog.Staff, $"/earmpos {armario.Codigo} X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z} D: {player.Dimension}", p, null);
        }

        [Command("irarm", "/irarm (código)")]
        public void CMD_irarm(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => x.Codigo == codigo);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {codigo} não existe.");
                return;
            }

            p.LimparIPLs();
            player.Dimension = (int)armario.Dimensao;

            if (armario.Dimensao > 0)
            {
                p.IPLs = Functions.ObterIPLsPorInterior(Global.Propriedades.FirstOrDefault(x => x.Codigo == armario.Dimensao).Interior);
                p.SetarIPLs();
            }

            player.Position = new Position(armario.PosX, armario.PosY, armario.PosZ);
        }

        [Command("carmi", "/carmi (armário) (arma)")]
        public void CMD_carmi(IPlayer player, int armario, string arma)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var arm = Global.Armarios.FirstOrDefault(x => x.Codigo == armario);
            if (arm == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {armario} não existe.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            if (Global.ArmariosItens.Any(x => x.Codigo == armario && x.Arma == (long)wep))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} já existe no armário {armario}.");
                return;
            }

            var item = new ArmarioItem()
            {
                Codigo = armario,
                Arma = (long)wep,
            };

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Add(item);
                context.SaveChanges();
            }

            Global.ArmariosItens.Add(item);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Arma {wep} criada no armário {armario} com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/carmi {armario} {item.Arma}", p, null);
        }

        [Command("rarmi", "/rarmi (armário) (arma)")]
        public void CMD_rarmi(IPlayer player, int armario, string arma)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var arm = Global.Armarios.FirstOrDefault(x => x.Codigo == armario);
            if (arm == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {armario} não existe.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }
            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Remove(item);
                context.SaveChanges();
            }

            Global.ArmariosItens.Remove(item);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Arma {wep} removida do armário {armario} com sucesso.");
            Functions.GravarLog(TipoLog.Staff, $"/rarmi {armario} {item.Arma}", p, null);
        }

        [Command("earmimun", "/earmimun (armário) (arma) (munição)")]
        public void CMD_earmimun(IPlayer player, int armario, string arma, int municao)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            if (municao <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Munição inválida.");
                return;
            }

            Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)].Municao = municao;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Munição da arma {wep} no armário {armario} alterada para {municao}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmimun {armario} {item.Arma} {municao}", p, null);
        }

        [Command("earmirank", "/earmirank (armário) (arma) (rank)")]
        public void CMD_earmirank(IPlayer player, int armario, string arma, int rank)
        {
            var arm = Global.Armarios.FirstOrDefault(x => x.Codigo == armario);
            if (arm == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Armário {armario} não existe.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (!((int)p?.UsuarioBD?.Staff >= (int)TipoStaff.Manager || (p.Faccao == arm.Faccao && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            if (!Global.Ranks.Any(x => x.Faccao == arm.Faccao && x.Codigo == rank))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} não existe na facção {arm.Faccao}.");
                return;
            }

            Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)].Rank = rank;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Rank da arma {wep} no armário {armario} alterado para {rank}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmirank {armario} {item.Arma} {rank}", p, null);
        }

        [Command("earmiest", "/earmiest (armário) (arma) (estoque)")]
        public void CMD_earmiest(IPlayer player, int armario, string arma, int estoque)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            if (estoque < 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Estoque inválido.");
                return;
            }

            Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)].Estoque = estoque;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(Global.ArmariosItens[Global.ArmariosItens.IndexOf(item)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Estoque da arma {wep} no armário {armario} alterado para {estoque}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmiest {armario} {item.Arma} {estoque}", p, null);
        }

        [Command("cveh", "/cveh (modelo) (facção) (r1) (g1) (b1) (r2) (g2) (b2)")]
        public void CMD_cveh(IPlayer player, string modelo, int faccao, int r1, int g1, int b1, int r2, int g2, int b2)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.GetValues(typeof(VehicleModel)).Cast<VehicleModel>().Any(x => x.ToString().ToLower() == modelo.ToLower())
                && !Enum.GetValues(typeof(ModeloVeiculo)).Cast<ModeloVeiculo>().Any(x => x.ToString().ToLower() == modelo.ToLower()))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Modelo {modelo} não existe.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == faccao);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {faccao} não existe.");
                return;
            }

            var veiculo = new Veiculo()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                RotX = player.Rotation.Roll,
                RotY = player.Rotation.Pitch,
                RotZ = player.Rotation.Yaw,
                Faccao = faccao,
                Placa = Functions.GerarPlacaVeiculo(),
                Cor1R = r1,
                Cor1G = g1,
                Cor1B = b1,
                Cor2R = r2,
                Cor2G = g2,
                Cor2B = b2,
                Modelo = modelo,
            };

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Add(veiculo);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Veículo {veiculo.Codigo} criado.");
            Functions.GravarLog(TipoLog.Staff, $"/cveh {veiculo.Codigo} {modelo} {faccao} {r1} {g1} {b1} {r2} {g2} {b2}", p, null);
        }

        [Command("rveh", "/rveh (código)")]
        public void CMD_rveh(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} está spawnado.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
                if (veh.Faccao == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} não pertence a uma facção.");
                    return;
                }

                context.Veiculos.Remove(veh);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Veículo {veh.Codigo} removido.");
            Functions.GravarLog(TipoLog.Staff, $"/rveh {veh.Codigo}", p, null);
        }

        [Command("evehcor", "/evehcor (código) (r1) (g1) (b1) (r2) (g2) (b2)")]
        public void CMD_evehcor(IPlayer player, int codigo, int r1, int g1, int b1, int r2, int g2, int b2)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} está spawnado.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
                if (veh.Faccao == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} não pertence a uma facção.");
                    return;
                }

                veh.Cor1R = r1;
                veh.Cor1G = g1;
                veh.Cor1B = b1;
                veh.Cor2R = r2;
                veh.Cor2G = g2;
                veh.Cor2B = b2;
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Cores do veículo {veh.Codigo} alteradas para {r1} {g1} {b1} {r2} {g2} {b2}.");
            Functions.GravarLog(TipoLog.Staff, $"/evehcor {veh.Codigo} {r1} {g1} {b1} {r2} {g2} {b2}", p, null);
        }

        [Command("evehlivery", "/evehlivery (código) (livery)")]
        public void CMD_evehlivery(IPlayer player, int codigo, int livery)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} está spawnado.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
                if (veh.Faccao == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} não pertence a uma facção.");
                    return;
                }

                veh.Livery = livery;
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Livery do veículo {veh.Codigo} alterada para {livery}.");
            Functions.GravarLog(TipoLog.Staff, $"/evehlivery {veh.Codigo} {livery}", p, null);
        }

        [Command("earmipintura", "/earmipintura (armário) (arma) (pintura)")]
        public void CMD_earmipintura(IPlayer player, int armario, string arma, int pintura)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            if (pintura < 0 || pintura > 7)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Pintura deve ser entre 0 e 7.");
                return;
            }

            item.Pintura = pintura;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Pintura da arma {wep} no armário {armario} alterada para {pintura}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmipintura {armario} {item.Arma} {pintura}", p, null);
        }

        [Command("save")]
        public void CMD_save(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (player.IsInVehicle)
            {
                player.Emit("alt:log", $"POS: {player.Vehicle.Position.X.ToString().Replace(",", ".")}f, {player.Vehicle.Position.Y.ToString().Replace(",", ".")}f, {player.Vehicle.Position.Z.ToString().Replace(",", ".")}f");
                player.Emit("alt:log", $"ROT: {player.Vehicle.Rotation.Roll.ToString().Replace(",", ".")}f, {player.Vehicle.Rotation.Pitch.ToString().Replace(",", ".")}f, {player.Vehicle.Rotation.Yaw.ToString().Replace(",", ".")}f");
            }
            else
            {
                player.Emit("alt:log", $"POS: {player.Position.X.ToString().Replace(",", ".")}f, {player.Position.Y.ToString().Replace(",", ".")}f, {player.Position.Z.ToString().Replace(",", ".")}f");
                player.Emit("alt:log", $"ROT: {player.Rotation.Roll.ToString().Replace(",", ".")}f, {player.Rotation.Pitch.ToString().Replace(",", ".")}f, {player.Rotation.Yaw.ToString().Replace(",", ".")}f");
            }
        }

        [Command("pos", "/pos (x) (y) (z)")]
        public void CMD_pos(IPlayer player, float x, float y, float z)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            player.Position = new Position(x, y, z);

            Functions.GravarLog(TipoLog.Staff, $"/pos {x} {y} {z}", p, null);
        }

        [Command("carmicomp", "/carmicomp (armário) (arma) (componente)")]
        public void CMD_carmicomp(IPlayer player, int armario, string arma, string componente)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.ToLower() == componente.ToLower() && x.Weapon == wep);
            if (comp == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe para a arma {wep}.");
                return;
            }

            var componentes = JsonConvert.DeserializeObject<List<uint>>(item.Componentes);
            if (componentes.Contains(comp.Hash))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} já existe na arma {wep} do armário {armario}.");
                return;
            }

            componentes.Add(comp.Hash);
            item.Componentes = JsonConvert.SerializeObject(componentes);

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Componente {componente} adicionado na arma {wep} no armário {armario}.");
            Functions.GravarLog(TipoLog.Staff, $"/carmicomp {armario} {item.Arma} {componente}", p, null);
        }

        [Command("rarmicomp", "/rarmicomp (armário) (arma) (componente)")]
        public void CMD_rarmicomp(IPlayer player, int armario, string arma, string componente)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var item = Global.ArmariosItens.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {wep} não existe no armário {armario}.");
                return;
            }

            var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.ToLower() == componente.ToLower() && x.Weapon == wep);
            if (comp == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe para a arma {wep}.");
                return;
            }

            var componentes = JsonConvert.DeserializeObject<List<uint>>(item.Componentes);
            if (!componentes.Contains(comp.Hash))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe na arma {wep} do armário {armario}.");
                return;
            }

            componentes.Remove(comp.Hash);
            item.Componentes = JsonConvert.SerializeObject(componentes);

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Componente {componente} removido na arma {wep} no armário {armario}.");
            Functions.GravarLog(TipoLog.Staff, $"/rarmicomp {armario} {item.Arma} {componente}", p, null);
        }

        [Command("efacslots", "/efacslots (código) (slots)")]
        public void CMD_efacslots(IPlayer player, int codigo, int slots)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe.");
                return;
            }

            if (slots < 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Slots deve ser um valor positivo.");
                return;
            }

            faccao.Slots = slots;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(faccao);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a quantidade de slots da facção {faccao.Codigo} para {slots}.");
            Functions.GravarLog(TipoLog.Staff, $"/efacslots {faccao.Codigo} {slots}", p, null);
        }
        #endregion Staff 1337
    }
}