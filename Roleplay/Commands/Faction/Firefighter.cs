using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands.Faction
{
    public class Firefighter
    {
        [Command("curar", "/curar (ID ou nome)")]
        public static void CMD_curar(MyPlayer player, string idNome)
        {
            if (player.Faction?.Type != FactionType.Firefighter || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção médica ou não está em serviço.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension || !target.Ferido)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo ou não está ferido.");
                return;
            }

            target.Curar();
            player.SendMessage(MessageType.Success, $"Você curou {target.ICName}.");
            target.SendMessage(MessageType.Success, $"{player.ICName} curou você.");
        }
    }
}