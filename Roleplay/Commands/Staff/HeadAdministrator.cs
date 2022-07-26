using Roleplay.Factories;
using Roleplay.Models;
using System.Text.Json;

namespace Roleplay.Commands.Staff
{
    public class HeadAdministrator
    {
        [Command("parametros")]
        public static void CMD_parametros(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.HeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffParameters", JsonSerializer.Serialize(Global.Parameter));
        }
    }
}