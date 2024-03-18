using AltV.Net;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class WalkieTalkieScript : IScript
    {
        [Command("canal", "/canal (slot [1-5]) (canal)")]
        public static async Task CMD_canal(MyPlayer player, int slot, int canal)
        {
            var item = player.Items.FirstOrDefault(x => x.Category == ItemCategory.WalkieTalkie && x.Slot < 0);
            if (item == null)
            {
                player.SendMessage(MessageType.Error, "Você não possui um rádio comunicador equipado.");
                return;
            }

            if (slot < 1 || slot > 5)
            {
                player.SendMessage(MessageType.Error, "Slot deve ser entre 1 e 5.");
                return;
            }

            if (canal < 0)
            {
                player.SendMessage(MessageType.Error, "Canal inválido.");
                return;
            }

            if (canal >= 911 && canal <= 930 && player.Faction?.Type != FactionType.Police)
            {
                player.SendMessage(MessageType.Error, "Canal 911 até 930 é reservado para facções policiais.");
                return;
            }

            if (canal >= 931 && canal <= 950 && player.Faction?.Type != FactionType.Firefighter)
            {
                player.SendMessage(MessageType.Error, "Canal 931 até 950 é reservado para facções de bombeiros.");
                return;
            }

            if (canal == 999 && !(player.Faction?.Government ?? false))
            {
                player.SendMessage(MessageType.Error, "Canal 999 é reservado para facções governamentais.");
                return;
            }

            var extra = Functions.Deserialize<WalkieTalkieItem>(item.Extra);

            if (slot == 1)
                player.RadioCommunicatorItem.Channel1 = extra.Channel1 = canal;
            else if (slot == 2)
                player.RadioCommunicatorItem.Channel2 = extra.Channel2 = canal;
            else if (slot == 3)
                player.RadioCommunicatorItem.Channel3 = extra.Channel3 = canal;
            else if (slot == 4)
                player.RadioCommunicatorItem.Channel4 = extra.Channel4 = canal;
            else if (slot == 5)
                player.RadioCommunicatorItem.Channel5 = extra.Channel5 = canal;

            item.Extra = Functions.Serialize(extra);
            await using var context = new DatabaseContext();
            context.CharactersItems.Update(item);
            await context.SaveChangesAsync();

            if (canal == 0)
                player.SendMessage(MessageType.Success, $"Você desativou seu canal de rádio do slot {slot}.");
            else
                player.SendMessage(MessageType.Success, $"Você alterou seu canal de rádio do slot {slot} para {canal}.");
        }

        [Command("r", "/r (mensagem)", GreedyArg = true)]
        public static void CMD_r(MyPlayer player, string mensagem) => player.SendRadioMessage(1, mensagem);

        [Command("r2", "/r2 (mensagem)", GreedyArg = true)]
        public static void CMD_r2(MyPlayer player, string mensagem) => player.SendRadioMessage(2, mensagem);

        [Command("r3", "/r3 (mensagem)", GreedyArg = true)]
        public static void CMD_r3(MyPlayer player, string mensagem) => player.SendRadioMessage(3, mensagem);

        [Command("r4", "/r4 (mensagem)", GreedyArg = true)]
        public static void CMD_r4(MyPlayer player, string mensagem) => player.SendRadioMessage(4, mensagem);

        [Command("r5", "/r5 (mensagem)", GreedyArg = true)]
        public static void CMD_r5(MyPlayer player, string mensagem) => player.SendRadioMessage(5, mensagem);
    }
}