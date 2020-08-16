using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System.Collections.Generic;
using System.Linq;
using static Roleplay.Constants;

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
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            if (p.FaccaoBD.IsChatBloqueado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Chat da facção está bloqueado!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"(( {p.RankBD.Nome} {p.Nome} [{p.ID}]: {mensagem} ))", $"#{p.FaccaoBD.Cor}");
        }

        [Command("membros")]
        public void CMD_membros(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            var html = $@"<div class='box-header with-border'>
                <h3>{p.FaccaoBD.Nome} • Membros Online<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os membros...' />
            <br/><table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Rank</th>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>OOC</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>";

            var players = Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao).OrderByDescending(x => x.Rank).ThenBy(x => x.Nome);
            foreach (var x in players)
            {
                var status = x.IsEmTrabalho ? "<span style='color:#6EB469'>EM SERVIÇO</span>" : "<span style='color:#FF6A4D'>FORA DE SERVIÇO</span>";
                html += $@"<tr class='pesquisaitem'><td>{x.RankBD.Nome}</td><td>{x.ID}</td><td>{x.Nome}</td><td>{x.UsuarioBD.Nome}</td><td>{status}</td></tr>";
            }

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("blockf")]
        public void CMD_blockf(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(p.FaccaoBD)].IsChatBloqueado = !p.FaccaoBD.IsChatBloqueado;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!p.FaccaoBD.IsChatBloqueado ? "des" : string.Empty)}bloqueou o chat da facção!");
        }

        [Command("convidar", "/convidar (ID ou nome)")]
        public void CMD_convidar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador já está em uma facção!");
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
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção!");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu!");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == p.Faccao && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} não existe!");
                return;
            }

            if (rank >= p.FaccaoBD.RankGestor && p.Rank < p.FaccaoBD.RankLider)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Somente o líder da facção pode alterar o rank de um jogador para gestor!");
                return;
            }

            target.Rank = rank;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o rank de {target.Nome} para {rk.Nome} ({rk.Codigo}).");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu rank para {rk.Nome} ({rk.Codigo}).");

            Functions.GravarLog(TipoLog.FaccaoGestor, $"/rank {rank}", p, target);
        }

        [Command("expulsar", "/expulsar (ID ou nome)")]
        public void CMD_expulsar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção!");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu!");
                return;
            }

            target.Faccao = 0;
            target.Rank = 0;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você demitiu {target.Nome} da facção.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} demitiu você da facção.");

            Functions.GravarLog(TipoLog.FaccaoGestor, "/demitir", p, target);
        }

        [Command("m", "/m (mensagem)", GreedyArg = true)]
        public void CMD_m(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
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
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou médica e não possui um emprego!");
                return;
            }

            p.IsEmTrabalho = !p.IsEmTrabalho;

            if (p?.Faccao == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.IsEmTrabalho ? "entrou em" : "saiu de")} serviço!");
            }
            else
            {
                foreach (var pl in Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao))
                    Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"{p.RankBD.Nome} {p.Nome} {(p.IsEmTrabalho ? "entrou em" : "saiu de")} serviço!", $"#{p.FaccaoBD.Cor}");
            }
        }

        [Command("sairfaccao")]
        public void CMD_sairfaccao(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            p.Faccao = p.Rank = 0;
            p.IsEmTrabalho = false;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu da facção.");
        }

        [Command("multar", "/multar (ID ou nome) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multar(IPlayer player, string idNome, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres!");
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
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {target.ObterNomeContato(911)}: Você recebeu uma multa de ${valor:N0}. Motivo: {motivo}", CorCelular);
        }

        [Command("multaroff", "/multar (nome completo) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multaroff(IPlayer player, string nomeCompleto, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres!");
                return;
            }

            nomeCompleto = nomeCompleto.Replace("_", " ");

            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Nome.ToLower() == nomeCompleto.ToLower() && !x.Online);
            if (personagem == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {nomeCompleto} não encontrado ou está online!");
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

        [Command("prender", "/prender (ID ou nome) (cela [1-3]) (minutos)")]
        public void CMD_prender(IPlayer player, string idNome, int cela, int minutos)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            if (player.Position.Distance(Constants.PosicaoPrisao) > 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no local que as prisões são efetuadas!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.TempoPrisao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador já está preso!");
                return;
            }

            if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            if (minutos <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Minutos inválidos!");
                return;
            }

            var pos = new Position();
            switch (cela)
            {
                case 1:
                    pos = new Position(460.4085f, -994.0992f, 25);
                    break;
                case 2:
                    pos = new Position(460.4085f, -997.7994f, 25);
                    break;
                case 3:
                    pos = new Position(460.4085f, -1001.342f, 25);
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cela deve ser entre 1 e 3!");
                    break;
            }

            using (var context = new DatabaseContext())
            {
                context.Prisoes.Add(new Prisao()
                {
                    Preso = target.Codigo,
                    Policial = p.Codigo,
                    Tempo = minutos,
                    Cela = cela,
                });
                context.SaveChanges();
            }

            target.Player.Position = pos;
            target.TempoPrisao = minutos;
            Functions.EnviarMensagemTipoFaccao(TipoFaccao.Policial, $"{p.RankBD.Nome} {p.Nome} prendeu {target.Nome} na cela {cela} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.", true, true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"{p.Nome} prendeu você na cela {cela} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.");
        }

        [Command("algemar", "/algemar (ID ou nome)")]
        public void CMD_algemar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            target.Algemado = !target.Algemado;

            if (target.Algemado)
            {
                target.PlayAnimation("mp_arresting", "idle", (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl));

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
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.Codigo > 0))
            {
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, p.FaccaoBD.Nome, $"#{p.FaccaoBD.Cor}");
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem);
            }
        }

        [Command("armario")]
        public void CMD_armario(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2 && x.Faccao == p.Faccao);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum armário da sua facção!");
                return;
            }

            var itens = Global.ArmariosItens.Where(x => x.Codigo == armario.Codigo).OrderBy(x => x.Rank).ThenBy(x => x.Arma)
            .Select(x => new
            {
                Arma = ((WeaponModel)x.Arma).ToString(),
                Item = x.Arma,
                x.Municao,
                x.Estoque,
                Rank = Global.Ranks.FirstOrDefault(y => y.Faccao == p.Faccao && y.Codigo == x.Rank).Nome,
            }).ToList();
            if (itens.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O armário não possui itens!");
                return;
            }

            player.Emit("Server:AbrirArmario", armario.Codigo, p.FaccaoBD.Nome, JsonConvert.SerializeObject(itens));
        }

        [Command("pegarcolete")]
        public void CMD_pegarcolete(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            var armario = Global.Armarios.FirstOrDefault(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2 && x.Faccao == p.Faccao);
            if (armario == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum armário da sua facção!");
                return;
            }

            player.Armor = 100;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você pegou colete.");
        }

        [Command("curar", "/curar (ID ou nome)")]
        public void CMD_curar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != TipoFaccao.Medica || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção médica ou não está em serviço!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension || target.TimerFerido == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você ou jogador não está ferido!");
                return;
            }

            target.TimerFerido?.Stop();
            target.TimerFerido = null;
            target.Ferimentos = new List<Ferimento>();
            target.Armas = new List<PersonagemArma>();
            target.Player.RemoveAllWeapons();
            target.Player.Emit("Server:SelecionarPersonagem");
            target.Player.Spawn(target.Player.Position);
            target.Player.Health = 200;
            target.Player.Armor = 0;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você curou {target.NomeIC}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} curou você.");
        }
    }
}