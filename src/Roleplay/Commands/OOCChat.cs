using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class OOCChat
    {
        [Command("b", "/b (mensagem)", GreedyArg = true)]
        public static void CMD_b(MyPlayer player, string mensagem) => player.SendMessageToNearbyPlayers(mensagem, MessageCategory.ChatOOC, player.Dimension > 0 ? 5.0f : 10.0f);

        [Command("pm", "/pm (ID ou nome) (mensagem)", GreedyArg = true)]
        public static async Task CMD_pm(MyPlayer player, string idNome, string mensagem)
        {
            if (player.User.PMToggle && !player.OnAdminDuty)
            {
                player.SendMessage(MessageType.Error, "Você está com as mensagens privadas desabilitadas.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (target.User.PMToggle && !player.OnAdminDuty)
            {
                player.SendMessage(MessageType.Error, "Jogador está com as mensagens privadas desabilitadas.");
                return;
            }

            var nome = player.OnAdminDuty ?
                $"{{{player.StaffColor}}}{player.User.Name} [{player.SessionId}]"
                :
                $"{player.ICName} [{player.SessionId}]";

            var nomeTarget = target.OnAdminDuty ?
                $"{{{target.StaffColor}}}{target.User.Name} [{target.SessionId}]"
                :
                $"{target.ICName} [{target.SessionId}]";

            player.SendMessage(MessageType.None, $"(( PM para {nomeTarget}{{{Global.CELLPHONE_SECONDARY_COLOR}}}: {mensagem} ))", Global.CELLPHONE_SECONDARY_COLOR);
            target.SendMessage(MessageType.None, $"(( PM de {nome}{{{Global.CELLPHONE_MAIN_COLOR}}}: {mensagem} ))", Global.CELLPHONE_MAIN_COLOR);
            await player.GravarLog(LogType.MensagensPrivadas, mensagem, target);
        }
    }
}