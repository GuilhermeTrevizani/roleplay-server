using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class ChatIC
    {
        [Command("me", "/me (mensagem)", GreedyArg = true)]
        public void CMD_me(IPlayer player, string mensagem) => Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Me, player.Dimension > 0 ? 7.5f : 20.0f);

        [Command("do", "/do (mensagem)", GreedyArg = true)]
        public void CMD_do(IPlayer player, string mensagem) => Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Do, player.Dimension > 0 ? 7.5f : 20.0f);

        [Command("g", "/g (mensagem)", GreedyArg = true)]
        public void CMD_g(IPlayer player, string mensagem) => Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICGrito, 30.0f);

        [Command("baixo", "/baixo (mensagem)", GreedyArg = true)]
        public void CMD_baixo(IPlayer player, string mensagem) => Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICBaixo, player.Dimension > 0 ? 3.75f : 5);

        [Command("s", "/s (ID ou nome) (mensagem)", GreedyArg = true)]
        public void CMD_s(IPlayer player, string idNome, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Constants.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{p.NomeIC} sussura: {mensagem}", Constants.CorCelularSecundaria);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"{p.NomeIC} sussura: {mensagem}", Constants.CorCelular);
            Functions.SendMessageToNearbyPlayers(player, $"sussurra algo para {target.NomeIC}.", TipoMensagemJogo.Ame, 5, true);
        }

        [Command("ame", "/ame (mensagem)", GreedyArg = true)]
        public void CMD_ame(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);

            var msgTotal = $"* {p.NomeIC} {mensagem}";
            if (msgTotal.Length > 99)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Mensagem deve ter no máximo {99 - $"* {p.NomeIC} ".Length} caracteres.");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Ame, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("ado", "/ado (mensagem)", GreedyArg = true)]
        public void CMD_ado(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);

            var msgTotal = $"* {mensagem} (( {p.NomeIC} ))";
            if (msgTotal.Length > 99)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Mensagem deve ter no máximo {99 - $"*  (( {p.NomeIC} ))".Length} caracteres.");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Ado, player.Dimension > 0 ? 7.5f : 20.0f);
        }
    }
}