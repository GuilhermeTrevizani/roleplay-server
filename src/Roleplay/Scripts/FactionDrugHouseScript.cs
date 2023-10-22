using AltV.Net;
using AltV.Net.Async;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Scripts
{
    public class FactionDrugHouseScript : IScript
    {
        [AsyncClientEvent(nameof(FactionDrugHouseBuyItem))]
        public async Task FactionDrugHouseBuyItem(MyPlayer player, int id, int quantity)
        {
            if (quantity <= 0)
            {
                player.EmitStaffShowMessage("Quantidade deve ser maior que 0.");
                return;
            }

            var item = Global.FactionsDrugsHousesItems.FirstOrDefault(x => x.Id == id);
            if ((item?.Quantity ?? 0) - quantity < 0)
            {
                player.EmitStaffShowMessage("O item não possui estoque.");
                return;
            }

            var value = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.Drogas && x.Name.ToLower() == item.ItemCategory.ToString().ToLower())?.Value ?? 0))
                * quantity;
            if (player.Money < value)
            {
                player.EmitStaffShowMessage(string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value));
                return;
            }

            var res = await player.GiveItem(new CharacterItem(item.ItemCategory) { Quantity = quantity });

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });

            item.Quantity -= quantity;

            await using var context = new DatabaseContext();
            context.FactionsDrugsHousesItems.Update(item);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você comprou {quantity:N0}x {Functions.GetEnumDisplay(item.ItemCategory)} por ${value:N0}.", true);
            await player.GravarLog(LogType.Droga, $"Comprar Drug House {item.FactionDrugHouseId} {item.ItemCategory} {quantity} {value}", null);

            var html = Functions.GetFactionsDrugsHousesItemsHTML(item.FactionDrugHouseId, false);
            foreach (var target in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("ShowFactionDrugHouse", true, html);
        }
    }
}