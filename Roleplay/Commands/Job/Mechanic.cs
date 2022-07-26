using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands.Job
{
    public class Mechanic
    {
        [Command("tunarcomprar")]
        public static void CMD_tunarcomprar(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Mechanic)
            {
                player.SendMessage(MessageType.Error, "Você não é um mecânico.");
                return;
            }

            if (!player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em serviço.");
                return;
            }

            Functions.CMDTuning(player, null, false);
        }
    }
}