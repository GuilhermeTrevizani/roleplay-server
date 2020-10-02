using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Commands
{
    public class Faction
    {
        [Command("f", "/f (mensagem)", GreedyArg = true)]
        public void CMD_f(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção.");
                return;
            }

            if (p.FaccaoBD.ChatBloqueado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Chat da facção está bloqueado.");
                return;
            }

            if (p.UsuarioBD.TogChatFaccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está com o chat da facção desabilitado.");
                return;
            }

            Functions.EnviarMensagemFaccao(p, $"(( {p.RankBD.Nome} {p.Nome} [{p.ID}]: {mensagem} ))");
        }

        [Command("membros")]
        public void CMD_membros(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção.");
                return;
            }

            var html = $@"<input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os membros...' /><br/>
            <table class='table table-bordered table-striped'>
                <thead>
                    <tr class='bg-dark'>
                        <th>Rank</th>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>OOC</th>
                        {(p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica ? "<th>Status</th>" : string.Empty)}
                        {(p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica || p.FaccaoBD.Tipo == TipoFaccao.Governo ? "<th>Distintivo</th>" : string.Empty)}
                    </tr>
                </thead>
                <tbody>";

            var players = Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao).OrderByDescending(x => x.Rank).ThenBy(x => x.Nome);
            foreach (var x in players)
            {
                var status = x.EmTrabalho ? $"<span class='label' style='background-color:{Global.CorSucesso}'>EM SERVIÇO</span>" : $"<span class='label' style='background-color:{Global.CorErro}'>FORA DE SERVIÇO</span>";
                html += $@"<tr class='pesquisaitem'><td>{x.RankBD.Nome}</td><td>{x.ID}</td><td>{x.Nome}</td><td>{x.UsuarioBD.Nome}</td>{(p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica ? $"<td>{status}</td>" : string.Empty)}{(p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica || p.FaccaoBD.Tipo == TipoFaccao.Governo ? $"<td>{x.Distintivo}</td>" : string.Empty)}</tr>";
            }

            html += $@"
                </tbody>
            </table>";

            player.Emit("Server:BaseHTML", Functions.GerarBaseHTML($"{p.FaccaoBD.Nome} • Membros Online", html));
        }

        [Command("blockf")]
        public void CMD_blockf(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            p.FaccaoBD.ChatBloqueado = !p.FaccaoBD.ChatBloqueado;
            Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} {(!p.FaccaoBD.ChatBloqueado ? "des" : string.Empty)}bloqueou o chat da facção.");
        }

        [Command("convidar", "/convidar (ID ou nome)")]
        public void CMD_convidar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            if (p.FaccaoBD.Slots > 0)
            {
                using var context = new DatabaseContext();
                var qtdMembros = context.Personagens.Count(x => x.Faccao == p.Faccao && !x.DataMorte.HasValue && !x.DataExclusao.HasValue);
                if (qtdMembros >= p.FaccaoBD.Slots)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção atingiu o máximo de slots ({p.FaccaoBD.Slots}).");
                    return;
                }
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador já está em uma facção.");
                return;
            }

            var rank = Global.Ranks.Where(x => x.Faccao == p.Faccao).Min(x => x.Codigo);
            var convite = new Convite()
            {
                Tipo = TipoConvite.Faccao,
                Personagem = p.Codigo,
                Valor = new string[] { p.Faccao.ToString(), rank.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == TipoConvite.Faccao);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você convidou {target.Nome} para a facção.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} convidou você para a facção {p.FaccaoBD.Nome}. (/ac {(int)convite.Tipo} para aceitar ou /rc {(int)convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.FaccaoGestor, "/convidar", p, target);
        }

        [Command("rank", "/rank (ID ou nome) (rank)")]
        public void CMD_rank(IPlayer player, string idNome, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção.");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu.");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == p.Faccao && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} não existe.");
                return;
            }

            if (rank >= p.FaccaoBD.RankGestor && p.Rank < p.FaccaoBD.RankLider)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Somente o líder da facção pode alterar o rank de um jogador para gestor.");
                return;
            }

            target.Rank = rank;
            Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} alterou o rank de {target.Nome} para {rk.Nome}.");
            Functions.GravarLog(TipoLog.FaccaoGestor, $"/rank {rank}", p, target);
        }

        [Command("expulsar", "/expulsar (ID ou nome)")]
        public void CMD_expulsar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção.");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu.");
                return;
            }

            if (target.FaccaoBD.Tipo == TipoFaccao.Policial)
            {
                foreach (var x in p.Armas)
                    p.Player.Emit("RemoveWeapon", x.Codigo);
                target.Armas = new List<Personagem.Arma>();
                target.Player.Armor = 0;
            }

            target.Faccao = target.Rank = target.Distintivo = 0;
            target.EmTrabalho = false;
            Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} expulsou {target.Nome} da facção.");
            Functions.GravarLog(TipoLog.FaccaoGestor, "/expulsar", p, target);
        }

        [Command("m", "/m (mensagem)", GreedyArg = true)]
        public void CMD_m(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Megafone, 55.0f);
        }

        [Command("duty")]
        public void CMD_duty(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial && p?.FaccaoBD?.Tipo != TipoFaccao.Medica && p?.Emprego == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou médica e não possui um emprego.");
                return;
            }

            p.EmTrabalho = !p.EmTrabalho;

            if (p?.Faccao == 0)
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.EmTrabalho ? "entrou em" : "saiu de")} serviço.");
            else
                Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} {(p.EmTrabalho ? "entrou em" : "saiu de")} serviço.");
        }

        [Command("sairfaccao")]
        public void CMD_sairfaccao(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção.");
                return;
            }

            if (p.FaccaoBD.Tipo == TipoFaccao.Policial)
            {
                foreach (var x in p.Armas)
                    p.Player.Emit("RemoveWeapon", x.Codigo);
                p.Armas = new List<Personagem.Arma>();
                player.Armor = 0;
            }

            Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} saiu da facção.");
            p.Faccao = p.Rank = p.Distintivo = 0;
            p.EmTrabalho = false;
        }

        [Command("multar", "/multar (ID ou nome) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multar(IPlayer player, string idNome, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido.");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                context.Multas.Add(new Multa()
                {
                    Motivo = motivo,
                    PersonagemMultado = target.Codigo,
                    PersonagemPolicial = p.Codigo,
                    Valor = valor,
                });
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {target.Nome} por ${valor:N0}. Motivo: {motivo}");
            if (target.Celular > 0)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {target.ObterNomeContato(911)}: Você recebeu uma multa de ${valor:N0}. Motivo: {motivo}", Global.CorCelular);
        }

        [Command("multaroff", "/multar (nome completo) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multaroff(IPlayer player, string nomeCompleto, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido.");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres.");
                return;
            }

            nomeCompleto = nomeCompleto.Replace("_", " ");

            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Nome.ToLower() == nomeCompleto.ToLower() && !x.Online);
            if (personagem == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {nomeCompleto} não encontrado ou está online.");
                return;
            }

            context.Multas.Add(new Multa()
            {
                Motivo = motivo,
                PersonagemMultado = personagem.Codigo,
                PersonagemPolicial = p.Codigo,
                Valor = valor,
            });
            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {personagem.Nome} por ${valor:N0}. Motivo: {motivo}");
        }

        [Command("prender", "/prender (ID ou nome) (minutos)")]
        public void CMD_prender(IPlayer player, string idNome, int minutos)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (player.Position.Distance(Global.PosicaoPrisao) > Global.DistanciaRP)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no local que as prisões são efetuadas.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            if (minutos <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Minutos inválidos.");
                return;
            }

            target.DataTerminoPrisao = DateTime.Now.AddMinutes(minutos);

            using (var context = new DatabaseContext())
            {
                context.Prisoes.Add(new Prisao()
                {
                    Preso = target.Codigo,
                    Policial = p.Codigo,
                    Termino = target.DataTerminoPrisao.Value,
                });
                context.SaveChanges();
            }

            Functions.SalvarPersonagem(target, false);
            Functions.EnviarMensagemTipoFaccao(TipoFaccao.Policial, $"{p.RankBD.Nome} {p.Nome} prendeu {target.Nome} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.", true, true);
            target.Player.Kick($"{p.Nome} prendeu você por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.");
        }

        [Command("algemar", "/algemar (ID ou nome)")]
        public void CMD_algemar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.TimerFerido != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador está ferido.");
                return;
            }

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            target.Algemado = !target.Algemado;

            if (target.Algemado)
            {
                target.PlayAnimation("mp_arresting", "idle", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você algemou {target.NomeIC}.");
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} algemou você.");
            }
            else
            {
                target.StopAnimation();

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você desalgemou {target.NomeIC}.");
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} desalgemou você.");
            }
        }

        [Command("gov", "/gov (mensagem)", GreedyArg = true)]
        public void CMD_gov(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"{p.FaccaoBD.Nome}: {{#FFFFFF}}{mensagem}", $"#{p.FaccaoBD.Cor}");
        }

        [Command("armario")]
        public void CMD_armario(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção.");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP && x.Faccao == p.Faccao && x.Dimensao == player.Dimension);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum armário da sua facção.");
                return;
            }

            var componentes = Global.ArmariosComponentes.Where(x => x.Codigo == armario.Codigo).OrderBy(x => x.Arma).ThenBy(x => x.Componente)
            .Select(x => new
            {
                Arma = ((WeaponModel)x.Arma).ToString(),
                Componente = Global.WeaponComponents.FirstOrDefault(y => y.Weapon == (WeaponModel)x.Arma && y.Hash == x.Componente)?.Name ?? string.Empty,
                ItemArma = x.Arma,
                ItemComponente = x.Componente,
            }).ToList();

            var itens = Global.ArmariosItens.Where(x => x.Codigo == armario.Codigo).OrderBy(x => x.Rank).ThenBy(x => x.Arma)
            .Select(x => new
            {
                Arma = ((WeaponModel)x.Arma).ToString(),
                Item = x.Arma,
                x.Municao,
                x.Estoque,
                Rank = Global.Ranks.FirstOrDefault(y => y.Faccao == p.Faccao && y.Codigo == x.Rank).Nome,
                Preco = $"${Global.Precos.FirstOrDefault(y => y.Tipo == TipoPreco.Armas && y.Nome.ToLower() == ((WeaponModel)x.Arma).ToString().ToLower())?.Valor ?? 0:N0}",
            }).ToList();

            if (itens.Count == 0 && componentes.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O armário não possui itens.");
                return;
            }

            player.Emit("Server:AbrirArmario", armario.Codigo, p.FaccaoBD.Nome,
                JsonConvert.SerializeObject(itens), JsonConvert.SerializeObject(componentes),
                p.FaccaoBD.Tipo == TipoFaccao.Policial || p.FaccaoBD.Tipo == TipoFaccao.Medica, p.FaccaoBD.Tipo == TipoFaccao.Policial,
                $"${Global.Parametros.ValorComponentes:N0}");
        }

        [Command("curar", "/curar (ID ou nome)")]
        public void CMD_curar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Medica || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção médica ou não está em serviço.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension || (target.Ferimentos.Count == 0 && !target.Player.IsDead && target.TimerFerido == null && target.Player.Health == target.Player.MaxHealth))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo ou não está ferido.");
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

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você curou {target.NomeIC}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} curou você.");
        }

        [Command("fspawn")]
        public void CMD_fspawn(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.FaccaoBD?.Tipo != TipoFaccao.Policial && p?.FaccaoBD?.Tipo != TipoFaccao.Medica) || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var ponto = Global.Pontos.FirstOrDefault(x => x.Tipo == TipoPonto.SpawnVeiculosFaccao && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP);
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                return;
            }

            using var context = new DatabaseContext();
            var veiculos = context.Veiculos.Where(x => x.Faccao == p.Faccao).ToList()
                .OrderBy(x => Convert.ToInt32(Global.Veiculos.Any(y => y.Codigo == x.Codigo))).ThenBy(x => x.Modelo).ThenBy(x => x.Placa)
                .Select(x => new
                {
                    x.Codigo,
                    Modelo = x.Modelo.ToUpper(),
                    x.Placa,
                    Encarregado = Global.Veiculos.FirstOrDefault(y => y.Codigo == x.Codigo)?.NomeEncarregado ?? "N/A",
                }).ToList();

            if (veiculos.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Sua facção não possui veículos para spawnar.");
                return;
            }

            player.Emit("Server:SpawnarVeiculosFaccao", ponto.Codigo, p.FaccaoBD.Nome, JsonConvert.SerializeObject(veiculos));
        }

        [Command("ate", "/ate (código)")]
        public void CMD_ate(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.FaccaoBD?.Tipo != TipoFaccao.Policial && p?.FaccaoBD?.Tipo != TipoFaccao.Medica) || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            var ligacao911 = Global.Ligacoes911.FirstOrDefault(x => x.ID == codigo);
            if (ligacao911 == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Nenhuma ligação 911 encontrada com o código {codigo}.");
                return;
            }

            player.Emit("Server:SetWaypoint", ligacao911.PosX, ligacao911.PosY);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"A localização da ligação de emergência #{codigo} foi marcada no seu GPS.", notify: true);
        }

        [Command("apreender", "/apreender (placa) (valor) (motivo)", GreedyArg = true)]
        public void CMD_apreender(IPlayer player, string placa, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var ponto = Global.Pontos.FirstOrDefault(x => x.Tipo == TipoPonto.ApreensaoVeiculos && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP);
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em ponto de apreensão de veículos.");
                return;
            }

            if (valor < 1 || valor > 5000)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor da apreensão deve ser entre 1 e 5000.");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Placa.ToUpper() == placa.ToUpper());
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não existe veículo com a placa: {placa.ToUpper()}");
                return;
            }

            if (veh.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo pertence a uma facção.");
                return;
            }

            if (player.Position.Distance(new Position(veh.Vehicle.Position.X, veh.Vehicle.Position.Y, veh.Vehicle.Position.Z)) > Global.DistanciaRP)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo do veículo.");
                return;
            }

            using var context = new DatabaseContext();
            veh.ValorApreensao = valor;
            veh.PosX = ponto.PosX;
            veh.PosY = ponto.PosY;
            veh.PosZ = ponto.PosZ;
            var rot = JsonConvert.DeserializeObject<Rotation>(ponto.Configuracoes);
            veh.RotX = rot.Roll;
            veh.RotY = rot.Pitch;
            veh.RotZ = rot.Yaw;
            context.Veiculos.Update(veh);

            context.Apreensoes.Add(new Apreensao()
            {
                Veiculo = veh.Codigo,
                Motivo = motivo,
                PersonagemPolicial = p.Codigo,
                Valor = valor,
            });
            context.SaveChanges();

            veh.Despawnar();

            Functions.EnviarMensagemFaccao(p, $"{p.RankBD.Nome} {p.Nome} apreendeu o veículo de placa {placa.ToUpper()} por ${valor:N0}.");
        }

        [Command("uniforme")]
        public void CMD_uniforme(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial && p?.FaccaoBD?.Tipo != TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção governamental.");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.Uniforme && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de uniformes.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(p.RoupasCivil))
            {
                var roupas = JsonConvert.DeserializeObject<List<Personagem.Roupa>>(p.RoupasCivil);
                foreach (var x in roupas)
                    p.SetClothes(x.Slot, x.Drawable, x.Texture);

                p.RoupasCivil = string.Empty;
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você retirou seu uniforme.", notify: true);
                return;
            }

            p.RoupasCivil = JsonConvert.SerializeObject(p.Roupas);

            p.SetClothes(3, 0, 0);
            p.SetClothes(7, 0, 0);

            if (p.PersonalizacaoDados.sex == 1)
            {
                if (p.FaccaoBD.Tipo == TipoFaccao.Policial)
                {
                    p.SetClothes(8, 58, 0);
                    p.SetClothes(6, 25, 0);
                    p.SetClothes(4, 35, 0);
                    p.SetClothes(11, 55, 0);
                }
            }
            else
            {
                if (p.FaccaoBD.Tipo == TipoFaccao.Policial)
                {
                    p.SetClothes(8, 35, 0);
                    p.SetClothes(6, 25, 0);
                    p.SetClothes(4, 34, 0);
                    p.SetClothes(11, 48, 0);
                }
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você colocou seu uniforme.", notify: true);
        }

        [Command("mdc")]
        public void CMD_mdc(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.MDC && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP)
                && !(Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle)?.Faccao == p.Faccao
                    && (p.Player.Seat == 1 || p.Player.Seat == 2)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em ponto de MDC ou em um veículo policial nos bancos dianteiros.");
                return;
            }

            player.Emit("Server:AbrirMDC", p.FaccaoBD.Nome);
            Functions.SendMessageToNearbyPlayers(player, "abre o MDC.", TipoMensagemJogo.Ame, 10);
        }

        [Command("tac", "/tac (canal [0-5])", Alias = "t")]
        public void CMD_tac(IPlayer player, int canal)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            if (canal < 0 || canal > 5)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal deve ser entre 0 e 5.");
                return;
            }

            if (p.TimerFerido != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está gravamente ferido.");
                return;
            }

            if (canal == 0)
            {
                foreach (var x in Global.TACVoice)
                    if (x.HasPlayer(player))
                        x.RemovePlayer(player);

                Global.GlobalVoice.MutePlayer(player);
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu do TAC.", notify: true);
                return;
            }

            foreach (var x in Global.TACVoice)
            {
                if (x.HasPlayer(player))
                {
                    if (canal == Global.TACVoice.IndexOf(x) + 1)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você já está no TAC {canal}.");
                        return;
                    }
                    else
                    {
                        x.RemovePlayer(player);
                    }
                }
            }

            Global.TACVoice[canal - 1].AddPlayer(player);
            Global.GlobalVoice.UnmutePlayer(player);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você entrou no TAC {canal}.", notify: true);
        }

        [Command("confiscar", "/confiscar (ID ou nome)")]
        public void CMD_confiscar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            foreach (var x in target.Armas)
                target.Player.Emit("RemoveWeapon", x.Codigo);

            target.Armas = new List<Personagem.Arma>();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você confiscou as armas de {target.NomeIC}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} confiscou suas armas.");
        }

        [Command("distintivo", "/distintivo (ID ou nome) (distintivo)")]
        public void CMD_distintivo(IPlayer player, string idNome, int distintivo)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.FaccaoBD?.Tipo != TipoFaccao.Policial && p?.FaccaoBD?.Tipo != TipoFaccao.Medica && p?.FaccaoBD?.Tipo != TipoFaccao.Governo) || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção.");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu.");
                return;
            }

            using var context = new DatabaseContext();
            var per = context.Personagens.FirstOrDefault(x => x.Faccao == p.Faccao && x.Distintivo == distintivo && !x.DataMorte.HasValue && !x.DataExclusao.HasValue);
            if (per != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Distintivo {distintivo} está sendo usado por {per.Nome}.");
                return;
            }

            target.Distintivo = distintivo;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o distintivo de {target.Nome} para {distintivo}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu distintivo para {distintivo}.");

            Functions.GravarLog(TipoLog.FaccaoGestor, $"/distintivo {distintivo}", p, target);
        }

        [Command("mostrardistintivo", "/mostrardistintivo (ID ou nome)")]
        public void CMD_mostrardistintivo(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Distintivo == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um distintivo.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"{{#{p.FaccaoBD.Cor}}}Distintivo #{p.Distintivo} de {p.Nome}");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"{p.FaccaoBD.Nome} - {p.RankBD.Nome}");
            Functions.SendMessageToNearbyPlayers(player, p == target ? "olha seu próprio distintivo." : $"mostra seu distintivo para {target.NomeIC}.", TipoMensagemJogo.Ame, 10);
        }
    }
}