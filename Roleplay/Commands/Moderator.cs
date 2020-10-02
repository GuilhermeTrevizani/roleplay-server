using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands
{
    public class Moderator
    {
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
            pos.X += Global.DistanciaRP;
            p.SetPosition(pos, false);
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
            pos.X += Global.DistanciaRP;
            target.SetPosition(pos, false);
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
            pos.X += Global.DistanciaRP;
            target.SetPosition(pos, false);
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

            if (p.UsuarioBD.TogChatStaff)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está com o chat da staff desabilitado.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.UsuarioBD.Staff != TipoStaff.Nenhum && !x.UsuarioBD.TogChatStaff))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"(( {Functions.ObterDisplayEnum(p.UsuarioBD.Staff)} {p.UsuarioBD.Nome}: {mensagem} ))", "#33EE33");
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

            if (target.UsuarioBD.Staff <= p.UsuarioBD.Staff)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

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
            pos.X += Global.DistanciaRP;
            p.SetPosition(pos, false);
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
            pos.X += Global.DistanciaRP;
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

            p.EmTrabalhoAdministrativo = !p.EmTrabalhoAdministrativo;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.EmTrabalhoAdministrativo ? "entrou em" : "saiu de")} serviço administrativo.");
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

        [Command("spec", "/spec (ID ou nome)")]
        public void CMD_spec(IPlayer player, string idNome)
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

            if (target.PosicaoSpec.HasValue)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador está observando outro jogador.");
                return;
            }

            if (!p.PosicaoSpec.HasValue)
            {
                p.PosicaoSpec = player.Position;
                p.DimensaoSpec = player.Dimension;
                p.IPLsSpec = p.IPLs;

                foreach (var x in Global.PersonagensOnline.Where(x => x.IDSpec == p.ID))
                    x.Unspectate();
            }

            player.CurrentWeapon = (uint)WeaponModel.Fist;
            p.LimparIPLs();
            p.IPLs = target.IPLs;
            p.SetarIPLs();
            p.IDSpec = target.ID;
            player.Dimension = target.Player.Dimension;
            p.SetPosition(new Position(target.Player.Position.X, target.Player.Position.Y, target.Player.Position.Z + 5), true);
            player.SetSyncedMetaData("nametag", string.Empty);
            player.Emit("SpectatePlayer", target.Player);
        }

        [Command("specoff")]
        public void CMD_specoff(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!p.PosicaoSpec.HasValue)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está observando um jogador.");
                return;
            }

            p.Unspectate();
        }

        [Command("apm", "/apm (ID ou nome) (mensagem)", GreedyArg = true)]
        public void CMD_apm(IPlayer player, string idNome, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Moderator || !p.EmTrabalhoAdministrativo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando ou não está em serviço administrativo.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"(( APM para {target.Nome} [{target.ID}]: {mensagem} ))", Global.CorCelularSecundaria);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"(( APM de {{{p.CorStaff}}}{p.Nome} [{p.ID}]{{{Global.CorCelularSecundaria}}}: {mensagem} ))", Global.CorCelular);
        }
    }
}