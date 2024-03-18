using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class CrackDenScript : IScript
    {
        [AsyncClientEvent(nameof(CrackDenSellItem))]
        public async Task CrackDenSellItem(MyPlayer player, int id, int quantity)
        {
            if (quantity <= 0)
            {
                player.EmitStaffShowMessage("Quantidade deve ser maior que 0.");
                return;
            }

            var item = Global.CrackDensItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
                return;

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == item.CrackDenId);
            if (crackDen == null)
            {
                player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                return;
            }

            if (crackDen.CooldownDate > DateTime.Now)
            {
                player.EmitStaffShowMessage($"A boca de fumo está com o cooldown ativo. Será liberada novamente {crackDen.CooldownDate}.");
                return;
            }

            if (!player.Items.Any(x => x.Category == item.ItemCategory && x.Quantity >= quantity))
            {
                player.EmitStaffShowMessage($"Você não possui {quantity}x {item.ItemCategory.GetDisplay()}.");
                return;
            }

            var allowedQuantity = crackDen.CooldownQuantityLimit - crackDen.Quantity;
            if (crackDen.Quantity + quantity > crackDen.CooldownQuantityLimit)
            {
                player.EmitStaffShowMessage($"A quantidade selecionada para venda ultrapassa o limite da boca de fumo. Quantidade restante: {allowedQuantity:N0}.");
                return;
            }

            var nowDate = DateTime.Now;

            var initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                Global.Parameter.InitialTimeCrackDen, 0, 0);

            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                Global.Parameter.EndTimeCrackDen, 0, 0);

            if (Global.Parameter.EndTimeCrackDen < Global.Parameter.InitialTimeCrackDen && nowDate.Hour >= Global.Parameter.InitialTimeCrackDen)
                endDate = endDate.AddDays(1);
            else
                initialDate = initialDate.AddDays(-1);

            if (nowDate < initialDate || nowDate > endDate)
            {
                player.EmitStaffShowMessage($"Você está fora do horário de funcionamento das bocas de fumo ({Global.Parameter.InitialTimeCrackDen} - {Global.Parameter.EndTimeCrackDen}).");
                return;
            }

            var value = item.Value * quantity;

            var res = await player.GiveItem(new CharacterItem(ItemCategory.Money) { Quantity = value });

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            await player.RemoveStackedItem(item.ItemCategory, quantity);

            crackDen.Quantity += quantity;
            if (crackDen.Quantity >= crackDen.CooldownQuantityLimit)
            {
                crackDen.CooldownDate = DateTime.Now.AddHours(crackDen.CooldownHours);
                crackDen.Quantity = 0;
            }

            await using var context = new DatabaseContext();
            context.CrackDens.Update(crackDen);
            await context.SaveChangesAsync();

            await context.CrackDensSells.AddAsync(new CrackDenSell
            {
                CrackDenId = item.CrackDenId,
                CharacterId = player.Character.Id,
                ItemCategory = item.ItemCategory,
                Quantity = quantity,
                Value = item.Value,
            });
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você vendeu {quantity}x {item.ItemCategory.GetDisplay()} por ${value:N0}.", true);
            await player.GravarLog(LogType.Drug, $"Vender Boca de Fumo {item.CrackDenId} {item.ItemCategory} {quantity}", null);

            player.Emit("ShowCrackDen", true, Functions.GetCrackDensItemsHTML(item.CrackDenId, false));
        }

        [AsyncClientEvent(nameof(CrackDenShowSells))]
        public async Task CrackDenShowSells(MyPlayer player, int id)
        {
            await player.GravarLog(LogType.ViewCrackDenSales, id.ToString(), null);

            await using var context = new DatabaseContext();
            var sells = await context.CrackDensSells.Where(x => x.CrackDenId == id)
                .Include(x => x.Character).ThenInclude(x => x.User)
                .OrderByDescending(x => x.Id)
                .Take(50)
                .Select(x => new
                {
                    x.Date,
                    Character = $"{x.Character.Name} [{x.CharacterId}] ({x.Character.User.Name})",
                    Item = x.ItemCategory.GetDisplay(),
                    Quantity = x.Quantity.ToString("N0"),
                    Value = $"${x.Value:N0}",
                    Total = $"${x.Quantity * x.Value:N0}"
                })
                .ToListAsync();

            var html = $@"<div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:auto;'>
                <table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Personagem</th>
                        <th>Item</th>
                        <th>Quantidade</th>
                        <th>Valor Unitário</th>
                        <th>Total</th>
                    </tr>
                </thead>
                <tbody>";

            if (sells.Count != 0)
            {
                foreach (var x in sells)
                    html += $@"<tr><td>{x.Date}</td><td>{x.Character}</td><td>{x.Item}</td><td>{x.Quantity}</td><td>{x.Value}</td><td>{x.Total}</td></tr>";
            }
            else
            {
                html += $"<tr><td class='text-center' colspan='6'>Nenhuma venda foi efetuada nessa boca de fumo.</td></tr>";
            }

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"50 últimas vendas da Boca de Fumo {id}", html));
        }
    }
}