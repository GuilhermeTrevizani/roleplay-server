using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class ChatOOC
    {
        [Command("b", "/b (mensagem)", GreedyArg = true)]
        public void CMD_b(IPlayer player, string mensagem) => Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatOOC, player.Dimension > 0 ? 5.0f : 10.0f);

        [Command("pm", "/pm (ID ou nome) (mensagem)", GreedyArg = true)]
        public void CMD_pm(IPlayer player, string idNome, string mensagem)
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

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"(( PM para {target.Nome} [{target.ID}]: {mensagem} ))", Constants.CorCelularSecundaria);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"(( PM de {p.Nome} [{p.ID}]: {mensagem} ))", Constants.CorCelular);
        }
    }
}