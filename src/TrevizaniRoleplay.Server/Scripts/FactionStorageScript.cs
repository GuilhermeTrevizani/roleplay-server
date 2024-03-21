using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json.Linq;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class FactionStorageScript : IScript
    {
        [Command("farmazenamento")]
        public static void CMD_farmazenamento(MyPlayer player)
        {
            if (!player.FactionFlags.Contains(FactionFlag.Storage))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsStorages.FirstOrDefault(x =>
                player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                && x.FactionId == player.Character.FactionId
                && x.Dimension == player.Dimension);
            if (factionArmory == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum arsenal da sua facção.");
                return;
            }

            var html = GetFactionArmoriesWeaponsHTML(factionArmory.Id);

            player.Emit("ShowFactionArmory",
                false,
                html,
                Functions.Serialize(player.Faction),
                player.Faction.Government);
        }

        [AsyncClientEvent(nameof(FactionArmoryEquipWeapon))]
        public async Task FactionArmoryEquipWeapon(MyPlayer player, string idString)
        {
            var id = new Guid(idString);
            var weapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == id);
            if (weapon == null || weapon.Quantity == 0)
            {
                player.EmitStaffShowMessage("O item não possui estoque.");
                return;
            }

            var preco = 0;
            if (player.Faction.Type == FactionType.Criminal)
            {
                preco = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.Weapons
                    && x.Name.ToLower() == weapon.Model.ToString().ToLower())?.Value ?? 0));
                if (player.Money < preco)
                {
                    player.EmitStaffShowMessage(string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, preco));
                    return;
                }

                // Preço será apenas na droga e na arma.
                // Se não achar preço, exibir mensagem que o preço não foi configurado

                var value = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.Drugs
                    && x.Name.Equals(item.ItemCategory.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0))
                    * quantity;
                if (player.Money < value)
                {
                    player.EmitStaffShowMessage(string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value));
                    return;
                }
            }

            var characterItem = new CharacterItem();
            characterItem.Create(ItemCategory.Weapon, (uint)weapon.Model, 1, Functions.Serialize(new WeaponItem
            {
                Ammo = weapon.Ammo,
                TintIndex = weapon.TintIndex,
                Components = Functions.Deserialize<List<uint>>(weapon.ComponentsJSON),
            }));
            var res = await player.GiveItem(characterItem);

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            if (preco > 0)
                await player.RemoveStackedItem(ItemCategory.Money, preco);

            weapon.Quantity--;
            await using var context = new DatabaseContext();
            context.FactionStoragesItems.Update(weapon);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você equipou {weapon.Model}.");
            await player.GravarLog(LogType.Weapon, $"Pegar Item Arsenal {Functions.Serialize(weapon)}", null);

            var html = GetFactionArmoriesWeaponsHTML(weapon.FactionStorageId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
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

        private static string GetFactionArmoriesWeaponsHTML(Guid factionArmoryId)
        {
            var html = string.Empty;
            var factionsArmoriesWeapons = Global.FactionsStoragesItems.Where(x => x.FactionStorageId == factionArmoryId);
            if (!factionsArmoriesWeapons.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há armas criadas.</td></tr>";
            }
            else
            {
                var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == factionArmoryId);
                var faction = Global.Factions.FirstOrDefault(x => x.Id == factionArmory.FactionId);

                foreach (var factionArmoryWeapon in factionsArmoriesWeapons)
                {
                    var realComponents = new List<string>();
                    foreach (var component in Functions.Deserialize<List<uint>>(factionArmoryWeapon.ComponentsJSON))
                    {
                        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Hash == component && x.Weapon.ToString() == factionArmoryWeapon.Model);
                        if (comp != null)
                            realComponents.Add(comp.Name);
                    }

                    html += $@"<tr class='pesquisaitem'>
                        <td>{factionArmoryWeapon.Id}</td>
                        <td>{factionArmoryWeapon.Model}</td>
                        <td>{factionArmoryWeapon.Ammo}</td>
                        <td>{factionArmoryWeapon.Quantity}</td>
                        <td>{factionArmoryWeapon.TintIndex}</td>
                        {(!faction.Government && !staff ? $"<td>{Global.Prices.FirstOrDefault(y => y.Type == PriceType.Weapons && y.Name.Equals(factionArmoryWeapon.Model.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0:N0}</td>" : string.Empty)}
                        <td>{string.Join(", ", realComponents)}</td>
                        <td class='text-center'>
                            <button onclick='equip({factionArmoryWeapon.Id})' type='button' class='btn btn-dark btn-sm'>EQUIPAR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
    }
}