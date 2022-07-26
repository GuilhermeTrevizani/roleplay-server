using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class ICChat
    {
        [Command("me", "/me (mensagem)", GreedyArg = true)]
        public static void CMD_me(MyPlayer player, string mensagem) => player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Me, player.Dimension > 0 ? 7.5f : 20.0f);

        [Command("do", "/do (mensagem)", GreedyArg = true)]
        public static void CMD_do(MyPlayer player, string mensagem) => player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Do, player.Dimension > 0 ? 7.5f : 20.0f);

        [Command("g", "/g (mensagem)", GreedyArg = true)]
        public static void CMD_g(MyPlayer player, string mensagem) => player.SendMessageToNearbyPlayers(mensagem, MessageCategory.ChatICGrito, 30.0f);

        [Command("baixo", "/baixo (mensagem)", GreedyArg = true)]
        public static void CMD_baixo(MyPlayer player, string mensagem) => player.SendMessageToNearbyPlayers(mensagem, MessageCategory.ChatICBaixo, player.Dimension > 0 ? 3.75f : 5);

        [Command("s", "/s (ID ou nome) (mensagem)", GreedyArg = true)]
        public static void CMD_s(MyPlayer player, string idNome, string mensagem)
        {
            if (player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_GRAVEMENTE_FERIDO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            mensagem = Functions.CheckFinalDot(mensagem);

            player.SendMessage(MessageType.None, $"{player.ICName} sussurra: {mensagem}", Global.CELLPHONE_SECONDARY_COLOR);
            target.SendMessage(MessageType.None, $"{player.ICName} sussurra: {mensagem}", Global.CELLPHONE_MAIN_COLOR);
            player.SendMessageToNearbyPlayers($"sussurra algo para {target.ICName}.", MessageCategory.Ame, 5);
        }

        [Command("ame", "/ame (mensagem)", GreedyArg = true)]
        public static void CMD_ame(MyPlayer player, string mensagem)
        {
            var msgTotal = $"* {player.ICName} {mensagem}";
            if (msgTotal.Length > 99)
            {
                player.SendMessage(MessageType.Error, $"Mensagem deve ter no máximo {99 - $"* {player.ICName} ".Length} caracteres.");
                return;
            }

            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Ame, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("ado", "/ado (mensagem)", GreedyArg = true)]
        public static void CMD_ado(MyPlayer player, string mensagem)
        {
            var msgTotal = $"* {mensagem} (( {player.ICName} ))";
            if (msgTotal.Length > 99)
            {
                player.SendMessage(MessageType.Error, $"Mensagem deve ter no máximo {99 - $"*  (( {player.ICName} ))".Length} caracteres.");
                return;
            }

            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Ado, player.Dimension > 0 ? 7.5f : 20.0f);
        }
    }
}