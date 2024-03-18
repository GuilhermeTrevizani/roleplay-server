using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffGiveItemScript : IScript
    {
        [Command("daritem")]
        public static void CMD_daritem(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.GiveItem))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            static string GetDefaultExtra(BaseItem baseItem)
            {
                if (baseItem.Category == ItemCategory.Weapon)
                    return Functions.Serialize(new WeaponItem());

                if (baseItem.GetIsCloth())
                    return Functions.Serialize(new ClotheAccessoryItem());

                return string.Empty;
            }

            var categories = new List<CategoryResponse>();

            foreach (var category in Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>())
            {
                var item = new CharacterItem();
                item.Create(category, 0, 1, string.Empty);

                categories.Add(new()
                {
                    ID = (int)category,
                    Name = category.GetDisplay(),
                    Extra = GetDefaultExtra(item),
                    HasType = !(item.GetIsStack()
                        || category == ItemCategory.WalkieTalkie
                        || category == ItemCategory.Cellphone
                        || category == ItemCategory.Microphone),
                    IsStack = item.GetIsStack(),
                });
            }

            player.Emit("Staff:GiveItem", Functions.Serialize(categories));
        }

        [AsyncClientEvent(nameof(StaffGiveItem))]
        public static async Task StaffGiveItem(MyPlayer player, int category, string type, string extra, int quantity, int targetId)
        {
            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == targetId);
            if (target == null)
            {
                player.EmitStaffShowMessage("Jogador inválido.");
                return;
            }

            if (!Enum.IsDefined(typeof(ItemCategory), Convert.ToByte(category)))
            {
                player.EmitStaffShowMessage($"Categoria {category} não existe.");
                return;
            }

            if (quantity <= 0)
            {
                player.EmitStaffShowMessage("Quantidade deve ser maior que 0.");
                return;
            }

            var item = new CharacterItem();
            item.Create((ItemCategory)category, 0, quantity, extra);

            _ = uint.TryParse(type, out uint realType);

            if (item.Category == ItemCategory.Weapon)
            {
                if (!Enum.TryParse(type, true, out WeaponModel wep))
                {
                    player.EmitStaffShowMessage($"Arma {type} não existe.");
                    return;
                }

                if (!Enum.IsDefined(typeof(WeaponModel), wep))
                {
                    player.EmitStaffShowMessage($"Arma {type} não existe.");
                    return;
                }

                realType = (uint)wep;

                try
                {
                    var itemExtra = Functions.Deserialize<WeaponItem>(extra);
                    if (itemExtra.Ammo <= 0)
                    {
                        player.EmitStaffShowMessage("Ammo deve ser maior que 0.");
                        return;
                    }

                    if (itemExtra.TintIndex < 0 || itemExtra.TintIndex > 7)
                    {
                        player.EmitStaffShowMessage("TintIndex deve ser entre 0 e 7.");
                        return;
                    }

                    foreach (var c in itemExtra.Components)
                    {
                        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Weapon == wep && x.Hash == c);
                        if (comp == null)
                        {
                            player.EmitStaffShowMessage($"Componente {c} não existe.");
                            return;
                        }
                    }
                }
                catch
                {
                    player.EmitStaffShowMessage("Extra do item não foi informado corretamente.");
                    return;
                }
            }
            else if (item.GetIsCloth())
            {
                try
                {
                    var itemExtra = Functions.Deserialize<ClotheAccessoryItem>(extra);

                    var cloth = (item.Category switch
                    {
                        ItemCategory.Cloth1 => target.Character.Sex == CharacterSex.Man ? Global.Clothes1Male : Global.Clothes1Female,
                        ItemCategory.Cloth3 => target.Character.Sex == CharacterSex.Man ? Global.Clothes3Male : Global.Clothes3Female,
                        ItemCategory.Cloth4 => target.Character.Sex == CharacterSex.Man ? Global.Clothes4Male : Global.Clothes4Female,
                        ItemCategory.Cloth5 => target.Character.Sex == CharacterSex.Man ? Global.Clothes5Male : Global.Clothes5Female,
                        ItemCategory.Cloth6 => target.Character.Sex == CharacterSex.Man ? Global.Clothes6Male : Global.Clothes6Female,
                        ItemCategory.Cloth7 => target.Character.Sex == CharacterSex.Man ? Global.Clothes7Male : Global.Clothes7Female,
                        ItemCategory.Cloth8 => target.Character.Sex == CharacterSex.Man ? Global.Clothes8Male : Global.Clothes8Female,
                        ItemCategory.Cloth9 => target.Character.Sex == CharacterSex.Man ? Global.Clothes9Male : Global.Clothes9Female,
                        ItemCategory.Cloth10 => target.Character.Sex == CharacterSex.Man ? Global.Clothes10Male : Global.Clothes10Female,
                        ItemCategory.Cloth11 => target.Character.Sex == CharacterSex.Man ? Global.Clothes11Male : Global.Clothes11Female,
                        ItemCategory.Accessory0 => target.Character.Sex == CharacterSex.Man ? Global.Accessories0Male : Global.Accessories0Female,
                        ItemCategory.Accessory1 => target.Character.Sex == CharacterSex.Man ? Global.Accessories1Male : Global.Accessories1Female,
                        ItemCategory.Accessory2 => target.Character.Sex == CharacterSex.Man ? Global.Accessories2Male : Global.Accessories2Female,
                        ItemCategory.Accessory6 => target.Character.Sex == CharacterSex.Man ? Global.Accessories6Male : Global.Accessories6Female,
                        ItemCategory.Accessory7 => target.Character.Sex == CharacterSex.Man ? Global.Accessories7Male : Global.Accessories7Female,
                        _ => [],
                    }).FirstOrDefault(x => x.Drawable == realType && x.DLC == itemExtra.DLC);

                    if (cloth == null)
                    {
                        player.EmitStaffShowMessage($"Tipo {realType} com a DLC {itemExtra.DLC} não existe.");
                        return;
                    }

                    if (cloth.MaxTexture > 0 && itemExtra.Texture > cloth.MaxTexture)
                    {
                        player.EmitStaffShowMessage($"Texture deve ser entre 0 e {cloth.MaxTexture}.");
                        return;
                    }

                    itemExtra.Sex = target.Character.Sex;
                    extra = Functions.Serialize(itemExtra);
                }
                catch
                {
                    player.EmitStaffShowMessage("Extra do item não foi informado corretamente.");
                    return;
                }
            }
            else if (item.Category == ItemCategory.WalkieTalkie)
            {
                extra = Functions.Serialize(new WalkieTalkieItem());
            }
            else if (item.Category == ItemCategory.Cellphone)
            {
                extra = Functions.Serialize(new CellphoneItem { Contacts = Functions.GetDefaultsContacts() });
                realType = await Functions.GetNewCellphoneNumber();
            }

            item.Create((ItemCategory)category, realType, quantity, extra);

            var res = await target.GiveItem(item);

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            await Functions.SendStaffMessage($"{player.User.Name} deu {item.Quantity}x {item.GetName()} para {target.Character.Name}.", true);
            player.EmitStaffShowMessage($"Você deu {item.Quantity}x {item.GetName()} para {target.Character.Name}.");
            target.SendMessage(MessageType.Success, $"{player.User.Name} deu {item.Quantity}x {item.GetName()} para você.");

            await player.GravarLog(LogType.GiveItem, Functions.Serialize(item), target);
        }

    }
}