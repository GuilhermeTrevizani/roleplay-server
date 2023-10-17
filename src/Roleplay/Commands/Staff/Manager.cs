using AltV.Net;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands.Staff
{
    public class Manager
    {
        [Command("gmx")]
        public static async Task CMD_gmx(MyPlayer player)
        {
            if (player.User?.Staff < UserStaff.Manager)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            foreach (var x in Global.Vehicles)
                await x.Estacionar(player);

            foreach (var x in Global.Players.Where(x => x.Character.PersonalizationStep == CharacterPersonalizationStep.Ready))
            {
                x.SendMessage(MessageType.None, $"[{Global.SERVER_INITIALS}] {player.User.Name} salvou as informações do servidor.", Global.STAFF_COLOR);
                await x.Save();
            }

            player.SendMessage(MessageType.Success, "As informações do servidor foram salvas.");
            await player.GravarLog(LogType.Staff, "/gmx", null);
        }

        [Command("vip", "/vip (usuário) (nível) (meses)")]
        public static async Task CMD_vip(MyPlayer player, string usuario, int nivelVip, int meses)
        {
            if (player.User?.Staff < UserStaff.Manager)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.GetValues(typeof(UserVIP)).Cast<UserVIP>().Any(x => (int)x == nivelVip))
            {
                player.SendMessage(MessageType.Error, $"VIP {nivelVip} não existe.");
                return;
            }

            await using var context = new DatabaseContext();
            var user = context.Users.FirstOrDefault(x => x.Name.ToLower() == usuario.ToLower());
            if (user == null)
            {
                player.SendMessage(MessageType.Error, $"Usuário {usuario} não existe.");
                return;
            }

            var vip = (UserVIP)nivelVip;
            user.VIP = vip;
            user.VIPValidDate = ((user.VIPValidDate ?? DateTime.Now) > DateTime.Now && user.VIP == vip ? user.VIPValidDate.Value : DateTime.Now).AddMonths(meses);
            user.NameChanges += vip switch
            {
                UserVIP.Gold => 4,
                UserVIP.Silver => 3,
                _ => 2,
            };

            user.ForumNameChanges += vip switch
            {
                UserVIP.Gold => 2,
                _ => 1,
            };

            user.PlateChanges += vip switch
            {
                UserVIP.Gold => 2,
                UserVIP.Silver => 1,
                _ => 0,
            };

            var target = Global.Players.FirstOrDefault(x => x.User.Id == user.Id);
            if (target != null)
            {
                target.User.VIP = user.VIP;
                target.User.VIPValidDate = user.VIPValidDate;
                target.User.NameChanges = user.NameChanges;
                target.User.ForumNameChanges = user.ForumNameChanges;
                target.User.PlateChanges = user.PlateChanges;
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou seu nível VIP para {Functions.GetEnumDisplay(vip)} expirando em {user.VIPValidDate}.");
            }
            else
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            await player.GravarLog(LogType.Staff, $"/vip {user.Id} {vip} {meses}", target);
            player.SendMessage(MessageType.Success, $"Você alterou o nível VIP de {user.Name} para {Functions.GetEnumDisplay(vip)} expirando em {user.VIPValidDate}.");
        }

        [Command("password", "/password (senha)")]
        public static async Task CMD_password(MyPlayer player, string password)
        {
            if (player.User?.Staff < UserStaff.Manager)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            Alt.Core.SetPassword(password);
            await player.GravarLog(LogType.Staff, $"/password {password}", null);
            player.SendMessage(MessageType.Success, $"Você alterou a senha do servidor.");
        }
    }
}