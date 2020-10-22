using AltV.Net;
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

namespace Roleplay.Commands.Staff
{
    public class Manager
    {
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
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"{p.UsuarioBD.Nome} salvou as informações do servidor.", Global.CorErro);
                Functions.SalvarPersonagem(pl);
            }
        }

        [Command("proximo", Alias = "prox")]
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

            foreach (var x in Global.Pontos)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Ponto {x.Codigo} | Tipo: {(int)x.Tipo}");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} criado.");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} removido.");
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

            faccao.Nome = nome;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(faccao);
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

            faccao.Tipo = (TipoFaccao)tipo;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(faccao);
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

            faccao.Cor = cor;

            using (var context = new DatabaseContext())
            {
                context.Faccoes.Update(faccao);
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

            var html = $@"<input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise as facções...' /><br/>
            <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:hidden;'>
            <table class='table table-bordered table-striped'>
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

            player.Emit("Server:BaseHTML", Functions.GerarBaseHTML($"{Global.NomeServidor} • Faccções", html));
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
                var ranks = context.Ranks.AsQueryable().Where(x => x.Faccao == fac).ToList();
                rank.Codigo = ranks.Count == 0 ? 1 : ranks.Max(x => x.Codigo) + 1;
                context.Ranks.Add(rank);
                context.SaveChanges();
            }

            Global.Ranks.Add(rank);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você criou o rank {rank.Nome} [{rank.Codigo}] da facção {faction.Nome} [{faction.Codigo}].");
            Functions.GravarLog(TipoLog.Staff, $"/crank {faction.Codigo} {rank.Codigo}", p, null);
        }

        [Command("rrank", "/rrank (facção) (código)")]
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

            var html = $@"<input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os ranks...' /><br/>
            <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:hidden;'>
            <table class='table table-bordered table-striped'>
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

            player.Emit("Server:BaseHTML", Functions.GerarBaseHTML($"{faction.Nome} • Ranks", html));
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

            if (target.Usuario == 1 && p.Usuario != 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var stf = (TipoStaff)staff;
            target.UsuarioBD.Staff = stf;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu nível staff para {stf} [{staff}].");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o nível staff de {target.UsuarioBD.Nome} para {stf} [{staff}].");
            Functions.GravarLog(TipoLog.Staff, $"/setstaff {staff}", p, target);
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
            if (faccao == null && fac != 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe.");
                return;
            }

            if (fac != 0)
            {
                if (!Global.Ranks.Any(x => x.Faccao == fac && x.Codigo == faccao.RankLider))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank líder ({faccao.RankLider}) da facção {fac} não existe.");
                    return;
                }
            }

            target.Faccao = fac;
            target.Rank = faccao?.RankLider ?? 0;
            target.Emprego = TipoEmprego.Nenhum;
            target.EmTrabalho = false;

            if (fac != 0)
            {
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} te deu a liderança da facção {faccao.Nome} [{faccao.Codigo}].");
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu a liderança da facção {faccao.Nome} [{faccao.Codigo}] para {target.Nome}.");
            }
            else
            {
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} removeu você da liderança da facção.");
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu {target.Nome} da liderança da facção.");
            }

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
                Endereco = p.AreaName,
            };

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Add(prop);
                context.SaveChanges();
            }

            prop.CriarIdentificador();

            Global.Propriedades.Add(prop);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} criada.");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} removida.");
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

            prop.Valor = valor;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(prop);
                context.SaveChanges();
            }

            prop.CriarIdentificador();

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
            prop.Interior = (TipoInterior)interior;
            prop.SaidaPosX = pos.X;
            prop.SaidaPosY = pos.Y;
            prop.SaidaPosZ = pos.Z;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(prop);
                context.SaveChanges();
            }

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

            prop.EntradaPosX = player.Position.X;
            prop.EntradaPosY = player.Position.Y;
            prop.EntradaPosZ = player.Position.Z;
            prop.Dimensao = player.Dimension;
            prop.Endereco = p.AreaName;

            using (var context = new DatabaseContext())
            {
                context.Propriedades.Update(prop);
                context.SaveChanges();
            }

            prop.CriarIdentificador();

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
            player.Dimension = (int)prop.Dimensao;
            p.SetPosition(new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ), false);
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
            p.SetPosition(new Position(blip.PosX, blip.PosY, blip.PosZ), false);
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
            if (tp == TipoPreco.Armas)
            {
                if (!Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().Any(x => x.ToString().ToLower() == nome.ToLower()))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {nome} não existe.");
                    return;
                }
            }
            else if (tp == TipoPreco.Empregos || tp == TipoPreco.AluguelEmpregos)
            {
                if (!Global.Empregos.Any(x => x.Tipo.ToString().ToLower() == nome.ToLower()))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Emprego {nome} não existe.");
                    return;
                }
            }
            else if (tp != TipoPreco.Conveniencia)
            {
                if (!Enum.GetValues(typeof(VehicleModel)).Cast<VehicleModel>().Any(x => x.ToString().ToLower() == nome.ToLower())
                    && !Enum.GetValues(typeof(ModeloVeiculo)).Cast<ModeloVeiculo>().Any(x => x.ToString().ToLower() == nome.ToLower()))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {nome} não existe.");
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

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {Functions.ObterDisplayEnum(tp)} ({tipo}) e nome {nome} criado/editado.");
            Functions.GravarLog(TipoLog.Staff, $"/cpreco {tipo} {nome} {valor}", p, null);
        }

        [Command("rpreco", "/rpreco (tipo) (nome)", GreedyArg = true)]
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {Functions.ObterDisplayEnum(tp)} ({tipo}) e nome {nome} removido.");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} criado.");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} removido.");
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
            p.SetPosition(new Position(ponto.PosX, ponto.PosY, ponto.PosZ), false);
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Armário {armario.Codigo} criado.");
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Armário {armario.Codigo} removido.");
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

            p.SetPosition(new Position(armario.PosX, armario.PosY, armario.PosZ), false);
        }

        [Command("carmi", "/carmi (armário) (arma) (munição) (estoque)")]
        public void CMD_carmi(IPlayer player, int armario, string arma, int municao, int estoque)
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

            if (municao <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Munição inválida.");
                return;
            }

            if (estoque < 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Estoque inválido.");
                return;
            }

            var item = new ArmarioItem()
            {
                Codigo = armario,
                Arma = (long)wep,
                Municao = municao,
                Estoque = estoque
            };

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Add(item);
                context.SaveChanges();
            }

            Global.ArmariosItens.Add(item);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Arma {wep} com munição {municao} e estoque {estoque} adicionada no armário {armario}.");
            Functions.GravarLog(TipoLog.Staff, $"/carmi {armario} {item.Arma} {municao} {estoque}", p, null);
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
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Arma {wep} removida do armário {armario}.");
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

            item.Municao = municao;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
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

            item.Rank = rank;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
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

            item.Estoque = estoque;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Estoque da arma {wep} no armário {armario} alterado para {estoque}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmiest {armario} {item.Arma} {estoque}", p, null);
        }

        [Command("cveh", "/cveh (modelo) (facção)")]
        public void CMD_cveh(IPlayer player, string modelo, int faccao)
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
                Modelo = modelo,
            };
            veiculo.Combustivel = veiculo.TanqueCombustivel;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Add(veiculo);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Veículo {veiculo.Codigo} criado.");
            Functions.GravarLog(TipoLog.Staff, $"/cveh {veiculo.Codigo} {modelo} {faccao}", p, null);
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
                if ((veh?.Faccao ?? 0) == 0)
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

        [Command("evehcor", "/evehcor (código)")]
        public void CMD_evehcor(IPlayer player, int codigo)
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
                if ((veh?.Faccao ?? 0) == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} não pertence a uma facção.");
                    return;
                }
            }

            player.Emit("Server:PintarVeiculo", veh.Codigo, 2);
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
                if ((veh?.Faccao ?? 0) == 0)
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

            item.Pintura = (byte)pintura;

            using (var context = new DatabaseContext())
            {
                context.ArmariosItens.Update(item);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Pintura da arma {wep} no armário {armario} alterada para {pintura}.");
            Functions.GravarLog(TipoLog.Staff, $"/earmipintura {armario} {item.Arma} {pintura}", p, null);
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

        [Command("ccomp", "/ccomp (armário) (arma) (componente)")]
        public void CMD_ccomp(IPlayer player, int armario, string arma, string componente)
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

            var wc = Global.WeaponComponents.FirstOrDefault(x => x.Weapon == wep && x.Name.ToLower() == componente.ToLower());
            if (wc == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe.");
                return;
            }

            if (Global.ArmariosComponentes.Any(x => x.Codigo == armario && x.Arma == (long)wep && x.Componente == wc.Hash))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {wc.Name} da arma {wep} já existe no armário {armario}.");
                return;
            }

            var item = new ArmarioComponente()
            {
                Codigo = armario,
                Arma = (long)wep,
                Componente = wc.Hash,
            };

            using (var context = new DatabaseContext())
            {
                context.ArmariosComponentes.Add(item);
                context.SaveChanges();
            }

            Global.ArmariosComponentes.Add(item);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Componente {wc.Name} da arma {wep} adicionado no armário {armario}.");
            Functions.GravarLog(TipoLog.Staff, $"/ccomp {armario} {item.Arma} {item.Componente}", p, null);
        }

        [Command("rcomp", "/rcomp (armário) (arma) (componente)")]
        public void CMD_rcomp(IPlayer player, int armario, string arma, string componente)
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

            var wc = Global.WeaponComponents.FirstOrDefault(x => x.Weapon == wep && x.Name.ToLower() == componente.ToLower());
            if (wc == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe.");
                return;
            }

            var item = Global.ArmariosComponentes.FirstOrDefault(x => x.Codigo == armario && x.Arma == (long)wep && x.Componente == wc.Hash);
            if (item == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {wc.Name} da arma {wep} não existe no armário {armario}.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                context.ArmariosComponentes.Remove(item);
                context.SaveChanges();
            }

            Global.ArmariosComponentes.Remove(item);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Componente {wc.Name} da arma {wep} removida do armário {armario}.");
            Functions.GravarLog(TipoLog.Staff, $"/rcomp {armario} {item.Arma} {item.Componente}", p, null);
        }

        [Command("vip", "/vip (usuário) (nível) (meses)")]
        public void CMD_vip(IPlayer player, int usuario, int nivelVip, int meses)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Usuario != 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (!Enum.GetValues(typeof(TipoVIP)).Cast<TipoVIP>().Any(x => (int)x == nivelVip))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"VIP {nivelVip} não existe.");
                return;
            }

            using var context = new DatabaseContext();
            var user = context.Usuarios.FirstOrDefault(x => x.Codigo == usuario);
            if (user == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Usuário {usuario} não existe.");
                return;
            }

            var vip = (TipoVIP)nivelVip;
            user.VIP = vip;
            user.DataExpiracaoVIP = ((user.DataExpiracaoVIP ?? DateTime.Now) > DateTime.Now ? user.DataExpiracaoVIP.Value : DateTime.Now).AddMonths(meses);
            user.PossuiNamechange = user.PossuiNamechangeForum = true;
            if (vip == TipoVIP.Prata || vip == TipoVIP.Ouro)
                user.PossuiPlateChange = true;
            context.Usuarios.Update(user);
            context.SaveChanges();

            var target = Global.PersonagensOnline.FirstOrDefault(x => x?.Usuario == usuario);
            if (target != null)
            {
                target.UsuarioBD.VIP = user.VIP;
                target.UsuarioBD.DataExpiracaoVIP = user.DataExpiracaoVIP;
                target.UsuarioBD.PossuiNamechange = user.PossuiNamechange;
                target.UsuarioBD.PossuiNamechangeForum = user.PossuiNamechangeForum;
                target.UsuarioBD.PossuiPlateChange = user.PossuiPlateChange;
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu nível VIP para {vip} expirando em {user.DataExpiracaoVIP}.");
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o nível VIP de {user.Nome} para {vip} expirando em {user.DataExpiracaoVIP}.");
            Functions.GravarLog(TipoLog.Staff, $"/vip {usuario} {vip} {meses}", p, null);
        }

        [Command("ncforum", "/ncforum (usuário)")]
        public void CMD_ncforum(IPlayer player, int usuario)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            using var context = new DatabaseContext();
            var user = context.Usuarios.FirstOrDefault(x => x.Codigo == usuario);
            if (user == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Usuário {usuario} não existe.");
                return;
            }

            user.PossuiNamechangeForum = false;
            context.Usuarios.Update(user);
            context.SaveChanges();

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Usuario == usuario);
            if (target != null)
            {
                target.UsuarioBD.PossuiNamechangeForum = false;
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} debitou seu namechange do fórum.");
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você debitou o namechange do fórum de {target.UsuarioBD.Nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/ncforum {usuario}", p, null);
        }

        [Command("eponto", "/eponto (código)")]
        public void CMD_eponto(IPlayer player, int codigo)
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

            if (ponto.Tipo != TipoPonto.Entrada)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Somente pontos de entrada podem ser editados.");
                return;
            }

            ponto.Configuracoes = JsonConvert.SerializeObject(player.Position);

            using (var context = new DatabaseContext())
            {
                context.Pontos.Update(ponto);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} editado.");
            Functions.GravarLog(TipoLog.Staff, $"/eponto {ponto.Codigo}", p, null);
        }

        [Command("jetpack")]
        public void CMD_jetpack(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (player.IsInVehicle)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode executar esse comando em um veículo.");
                return;
            }

            if (player.Dimension == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode executar esse comando na dimensão padrão.");
                return;
            }

            var veh = Alt.CreateVehicle(VehicleModel.Thruster, player.Position, player.Rotation);
            veh.Dimension = player.Dimension;
            veh.ManualEngineControl = false;
            veh.EngineOn = true;
            player.Emit("setPedIntoVehicle", veh, -1);
            Functions.GravarLog(TipoLog.Staff, "/jetpack", p, null);
        }

        [Command("param", "/param (parâmetro) (valor)")]
        public void CMD_param(IPlayer player, string parametro, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser mair que 0.");
                return;
            }

            var prop = typeof(Parametro).GetProperties().FirstOrDefault(x => x.Name.ToLower() == parametro.ToLower()
                && x.Name.ToLower() != "codigo" && x.Name.ToLower() != "recordeonline");
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não existe nenhum parâmetro com o nome {parametro}.");
                return;
            }

            prop.SetValue(Global.Parametros, valor);
            using var context = new DatabaseContext();
            context.Database.ExecuteSqlRaw($"UPDATE Parametros SET {parametro.ToUpper()} = {valor}");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"O parâmetro {parametro.ToUpper()} foi alterado para {valor}.");
            Functions.GravarLog(TipoLog.Staff, $"/param {parametro} {valor}", p, null);
        }

        [Command("cvehemprego", "/cvehemprego (emprego)")]
        public void CMD_cvehemprego(IPlayer player, int emprego)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var emp = Global.Empregos.FirstOrDefault(x => x.Tipo == (TipoEmprego)emprego);
            if (emp == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Emprego {emprego} não existe.");
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
                Emprego = emp.Tipo,
                Placa = Functions.GerarPlacaVeiculo(),
                Modelo = emp.Veiculo.ToString(),
                Cor1R = emp.CorVeiculo.R,
                Cor1G = emp.CorVeiculo.G,
                Cor1B = emp.CorVeiculo.B,
                Cor2R = emp.CorVeiculo.R,
                Cor2G = emp.CorVeiculo.G,
                Cor2B = emp.CorVeiculo.B,
            };
            veiculo.Combustivel = veiculo.TanqueCombustivel;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Add(veiculo);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Veículo {veiculo.Codigo} criado.");
            Functions.GravarLog(TipoLog.Staff, $"/cvehemprego {veiculo.Codigo} {emprego}", p, null);
        }

        [Command("blackout")]
        public void CMD_blackout(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            Global.Parametros.Blackout = !Global.Parametros.Blackout;
            Functions.EnviarMensagemStaff($"{p.UsuarioBD.Nome} {(!Global.Parametros.Blackout ? "des" : string.Empty)}ativou o blackout.", true);
            foreach (var x in Global.PersonagensOnline)
                x.Player.Emit("Server:setArtificialLightsState", Global.Parametros.Blackout);

            using (var context = new DatabaseContext())
            {
                context.Parametros.Update(Global.Parametros);
                context.SaveChanges();
            }

            Functions.GravarLog(TipoLog.Staff, "/blackout", p, null);
        }

        [Command("dinheiro", "/dinheiro (ID ou nome) (valor)")]
        public void CMD_dinheiro(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if ((int)p?.UsuarioBD?.Staff < (int)TipoStaff.Manager)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Dinheiro += valor;
            target.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você setou ${valor:N0} para {target.Nome}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} setou ${valor:N0} para você.");

            Functions.GravarLog(TipoLog.Staff, $"/dinheiro {valor}", p, target);
        }
    }
}