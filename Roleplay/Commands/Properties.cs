using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Models;
using System.Linq;

namespace Roleplay.Commands
{
    public class Properties
    {
        [Command("entrar")]
        public void CMD_entrar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Constants.DistanciaRP)
                .OrderBy(x => player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma entrada.");
                return;
            }

            if (!prox.Aberta)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "A porta está trancada.");
                return;
            }

            p.IPLs = Functions.ObterIPLsPorInterior(prox.Interior);
            p.SetarIPLs();
            player.Dimension = prox.Codigo;
            player.Position = new Position(prox.SaidaPosX, prox.SaidaPosY, prox.SaidaPosZ);
        }

        [Command("sair")]
        public void CMD_sair(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Dimension == x.Codigo
                    && player.Position.Distance(new Position(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)) <= Constants.DistanciaRP)
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

            p.LimparIPLs();
            player.Dimension = 0;
            player.Position = new Position(prox.EntradaPosX, prox.EntradaPosY, prox.EntradaPosZ);
        }

        [Command("pvender", "/pvender (ID ou nome) (valor)")]
        public void CMD_pvender(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => x.Personagem == p.Codigo && player.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= Constants.DistanciaRP)
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

            if (player.Position.Distance(target.Player.Position) > Constants.DistanciaRP || player.Dimension != target.Player.Dimension)
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
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} ofereceu para você a propriedade {prox.Codigo} por ${valor:N0}. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.Venda, $"/pvender {prox.Codigo} {valor}", p, target);
        }
    }
}