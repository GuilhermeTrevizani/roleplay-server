using AltV.Net;
using AltV.Net.Async;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Text.Json;

namespace Roleplay.Scripts
{
    public class FactionArmoryScript : IScript
    {
        [AsyncClientEvent(nameof(FactionArmoryEquipWeapon))]
        public async Task FactionArmoryEquipWeapon(MyPlayer player, int id)
        {
            var weapon = Global.FactionsArmoriesWeapons.FirstOrDefault(x => x.Id == id);
            if ((weapon?.Quantity ?? 0) == 0)
            {
                player.EmitStaffShowMessage("O item não possui estoque.");
                return;
            }

            var preco = 0;
            if (player.Faction.Type == FactionType.Criminal)
            {
                preco = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.Armas && x.Name.ToLower() == weapon.Weapon.ToString().ToLower())?.Value ?? 0));
                if (player.Money < preco)
                {
                    player.EmitStaffShowMessage(string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, preco));
                    return;
                }
            }

            var res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)weapon.Weapon)
            {
                Extra = JsonSerializer.Serialize(new WeaponItem
                {
                    Ammo = weapon.Ammo,
                    TintIndex = weapon.TintIndex,
                    Components = JsonSerializer.Deserialize<List<uint>>(weapon.ComponentsJSON),
                }),
            });

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            if (preco > 0)
            {
                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = preco
                });
            }

            weapon.Quantity--;
            await using var context = new DatabaseContext();
            context.FactionsArmoriesWeapons.Update(weapon);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você equipou {weapon.Weapon}.");
            await player.GravarLog(LogType.Arma, $"Pegar Item Arsenal {JsonSerializer.Serialize(weapon)}", null);

            var html = Functions.GetFactionArmoriesWeaponsHTML(weapon.FactionArmoryId, false);
            foreach (var target in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("ShowFactionArmory", true, html);
        }

        [AsyncClientEvent(nameof(FactionArmoryGiveBack))]
        public async Task FactionArmoryGiveBack(MyPlayer player)
        {
            player.Armor = 0;
            await player.RemoveItem(player.Items.Where(x => x.Category == ItemCategory.Weapon));
            await player.GravarLog(LogType.Faction, $"Devolver Itens Arsenal", null);
            player.EmitStaffShowMessage($"Você devolveu seus itens no arsenal.");
        }

        [ClientEvent(nameof(FactionArmoryEquipArmor))]
        public async Task FactionArmoryEquipArmor(MyPlayer player)
        {
            player.Armor = 100;
            await player.GravarLog(LogType.Faction, $"Pegar Colete Arsenal", null);
            player.EmitStaffShowMessage($"Você equipou um colete.");
        }
    }
}