using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class CrackDenScript : IScript
    {
        [AsyncClientEvent(nameof(CrackDenSellItem))]
        public async Task CrackDenSellItem(MyPlayer player, string idString, int quantity)
        {
            if (quantity <= 0)
            {
                player.EmitStaffShowMessage("Quantidade deve ser maior que 0.");
                return;
            }

            var id = idString.ToGuid();
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

            var characterItem = new CharacterItem();
            characterItem.Create(ItemCategory.Money, 0, value, null);
            var res = await player.GiveItem(characterItem);

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            await player.RemoveStackedItem(item.ItemCategory, quantity);

            crackDen.AddQuantity(quantity);

            await using var context = new DatabaseContext();
            context.CrackDens.Update(crackDen);
            await context.SaveChangesAsync();

            var crackDenSell = new CrackDenSell();
            crackDenSell.Create(item.CrackDenId, player.Character.Id, item.ItemCategory, quantity, item.Value);
            await context.CrackDensSells.AddAsync(crackDenSell);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você vendeu {quantity}x {item.ItemCategory.GetDisplay()} por ${value:N0}.", true);
            await player.GravarLog(LogType.Drug, $"Vender Boca de Fumo {item.CrackDenId} {item.ItemCategory} {quantity}", null);

            player.Emit("ShowCrackDen", true, GetCrackDensItemsHTML(item.CrackDenId));
        }

        [AsyncClientEvent(nameof(CrackDenShowSells))]
        public async Task CrackDenShowSells(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            await player.GravarLog(LogType.ViewCrackDenSales, id.ToString(), null);

            await using var context = new DatabaseContext();
            var sells = await context.CrackDensSells.Where(x => x.CrackDenId == id)
                .Include(x => x.Character).ThenInclude(x => x.User)
                .OrderByDescending(x => x.Id)
                .Take(50)
                .Select(x => new
                {
                    x.Date,
                    Character = $"{x.Character!.Name} [{x.CharacterId}] ({x.Character.User!.Name})",
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

        [Command("bocafumo")]
        public static void CMD_bocafumo(MyPlayer player)
        {
            var crackDen = Global.CrackDens.FirstOrDefault(x =>
                player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                && x.Dimension == player.Dimension);
            if (crackDen == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhuma boca de fumo.");
                return;
            }

            var html = GetCrackDensItemsHTML(crackDen.Id);

            player.Emit("ShowCrackDen",
                false,
                html,
                crackDen.Id);
        }

        private static string GetCrackDensItemsHTML(Guid crackDenId)
        {
            var html = string.Empty;
            var items = Global.CrackDensItems.Where(x => x.CrackDenId == crackDenId);
            if (!items.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há itens criados.</td></tr>";
            }
            else
            {
                foreach (var item in items)
                {
                    html += $@"<tr class='pesquisaitem'>
                        <td>{item.Id}</td>
                        <td>{item.ItemCategory.GetDisplay()}</td>
                        <td>${item.Value:N0}</td>
                        <td class='text-center'>
                            <button onclick='sell({item.Id})' type='button' class='btn btn-dark btn-sm'>VENDER</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
    }
}