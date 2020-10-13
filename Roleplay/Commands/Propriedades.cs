using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands
{
    public class Propriedades
    {
        [Command("entrar")]
        public void CMD_entrar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            var prox = Global.Propriedades
                .Where(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Global.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox != null)
            {
                if (!prox.Aberta)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "A porta está trancada.");
                    return;
                }

                p.IPLs = Functions.ObterIPLsPorInterior(prox.Interior);
                p.SetarIPLs();
                player.Dimension = prox.Codigo;
                p.SetPosition(new Position(prox.SaidaPosX, prox.SaidaPosY, prox.SaidaPosZ), false);
                return;
            }

            var proxEntrada = Global.Pontos
                .Where(x => x.Tipo == TipoPonto.Entrada && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)))
                .FirstOrDefault();
            if (proxEntrada != null)
            {
                if (string.IsNullOrWhiteSpace(proxEntrada.Configuracoes))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "O ponto de entrada não está configurado.");
                    return;
                }

                player.Dimension = 0;
                p.SetPosition(JsonConvert.DeserializeObject<Position>(proxEntrada.Configuracoes), false);
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma entrada.");
        }

        [Command("sair")]
        public void CMD_sair(IPlayer player)
        {
            var prox = Global.Propriedades
                .Where(x => player.Dimension == x.Codigo
                    && player.Position.Distance(new Position(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)) <= Global.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma saída.");
                return;
            }

            if (!prox.Aberta)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "A porta está trancada.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            p.LimparIPLs();
            player.Dimension = 0;
            p.SetPosition(new Position(prox.EntradaPosX, prox.EntradaPosY, prox.EntradaPosZ), false);
        }

        [Command("pvender", "/pvender (ID ou nome) (valor)")]
        public void CMD_pvender(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            var prox = Global.Propriedades
                .Where(x => x.Personagem == p.Codigo && player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Global.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();
            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade sua.");
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

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor não é válido.");
                return;
            }

            var convite = new Convite()
            {
                Tipo = TipoConvite.VendaPropriedade,
                Personagem = p.Codigo,
                Valor = new string[] { prox.Codigo.ToString(), valor.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == TipoConvite.VendaPropriedade);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você ofereceu sua propriedade {prox.Codigo} para {target.NomeIC} por ${valor:N0}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} ofereceu para você a propriedade {prox.Codigo} por ${valor:N0}. (/ac {(int)convite.Tipo} para aceitar ou /rc {(int)convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.Venda, $"/pvender {prox.Codigo} {valor}", p, target);
        }

        [Command("liberarprop")]
        public void CMD_liberarprop(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            var prop = Global.Propriedades
               .Where(x => x.Personagem != 0 && player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Global.DistanciaRP)
               .OrderBy(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
               .FirstOrDefault();
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de uma propriedade com dono.");
                return;
            }

            if (Global.PersonagensOnline.Any(x => x.Codigo == prop.Personagem))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O dono da propriedade está online.");
                return;
            }

            using var context = new DatabaseContext();
            var sql = $@"SELECT per.*
            FROM Personagens per
            INNER JOIN Usuarios usu ON per.Usuario = usu.Codigo
            WHERE per.Codigo = {prop.Personagem} 
            AND DATEDIFF(now(), per.DataUltimoAcesso) > (CASE usu.VIP WHEN 3 THEN 21 WHEN 2 THEN 14 ELSE 7 END)";
            var dono = context.Personagens.FromSqlRaw(sql).FirstOrDefault();
            if (dono == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O dono da propriedade não está inativo.");
                return;
            }

            prop.Personagem = 0;
            prop.CriarIdentificador();

            context.Propriedades.Update(prop);
            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"A propriedade {prop.Codigo} agora está disponível para compra.");
            Functions.GravarLog(TipoLog.Venda, $"/liberarprop {prop.Codigo}", p, null);
        }

        [Command("abandonar")]
        public void CMD_abandonar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            var prop = Global.Propriedades
                .Where(x => x.Personagem == p.Codigo && player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Global.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade sua.");
                return;
            }

            prop.Aberta = false;
            prop.Personagem = 0;
            prop.CriarIdentificador();

            using var context = new DatabaseContext();
            context.Propriedades.Update(prop);
            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você abandonou a propriedade {prop.Codigo}.");
            Functions.GravarLog(TipoLog.Venda, $"/abandonar {prop.Codigo}", p, null);
        }

        [Command("guardaroupas")]
        public void CMD_guardaroupas(IPlayer player)
        {
            var prox = Global.Propriedades.FirstOrDefault(x => player.Dimension == x.Codigo);
            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no interior de uma propriedade.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            player.Emit("AbrirLojaRoupas", p.InformacoesRoupas, p.InformacoesAcessorios, p.PersonalizacaoDados.sex, p.SlotsRoupas, p.Roupa, 3);
        }
    }
}