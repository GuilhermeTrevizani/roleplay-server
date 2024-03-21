using AltV.Net;
using AltV.Net.Async;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class InventoryScript : IScript
    {
        [ClientEvent(nameof(ShowInventory))]
        public static void ShowInventory(MyPlayer player) => player.ShowInventory(player);

        [AsyncClientEvent(nameof(MoveItem))]
        public static async Task MoveItem(MyPlayer player, string id, short slot)
        {
            try
            {
                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null || item.Slot == slot || slot == 0)
                    return;

                var oldSlot = Convert.ToInt32(item.Slot);

                if (player.Items.Any(x => x.Slot == slot))
                {
                    player.EmitShowMessage($"Este slot já está sendo usado.");
                    player.Emit("Inventory:MoveItem", id, oldSlot, slot);
                    return;
                }

                if (slot < 0)
                {
                    var allowedSlots = item.Category switch
                    {
                        ItemCategory.Weapon => [-1, -2, -3],
                        ItemCategory.Cloth3 => [-101],
                        ItemCategory.Cloth11 => [-102],
                        ItemCategory.Cloth8 => [-103],
                        ItemCategory.Cloth4 => [-104],
                        ItemCategory.Cloth6 => [-105],
                        ItemCategory.Cloth7 => [-106],
                        ItemCategory.Cloth10 => [-107],
                        ItemCategory.Cloth9 => [-108],
                        ItemCategory.Cloth1 => [-109],
                        ItemCategory.Accessory0 => [-110],
                        ItemCategory.Accessory1 => [-111],
                        ItemCategory.Accessory2 => [-112],
                        ItemCategory.Accessory6 => [-113],
                        ItemCategory.Accessory7 => [-114],
                        ItemCategory.Cloth5 => [-115],
                        ItemCategory.WalkieTalkie => [-116],
                        ItemCategory.Cellphone => [-117],
                        _ => Array.Empty<int>(),
                    };

                    if (!allowedSlots.Contains(slot))
                    {
                        player.EmitShowMessage($"Este item não pode ser colocado neste slot.");
                        player.Emit("Inventory:MoveItem", id, oldSlot, slot);
                        return;
                    }

                    if (item.Category == ItemCategory.Weapon)
                    {
                        if (player.Items.Any(x => x.Category == item.Category && x.Type == item.Type && x.Slot < 0 && x.Id != item.Id))
                        {
                            player.Emit("Server:MostrarErro", $"Uma arma deste tipo já está equipada.");
                            player.Emit("Inventory:MoveItem", id, oldSlot, slot);
                            return;
                        }
                    }
                }

                item.SetSlot(slot);

                await using var context = new DatabaseContext();
                context.CharactersItems.Update(item);
                await context.SaveChangesAsync();

                player.ShowInventory(player, update: true);

                if (item.GetIsCloth())
                {
                    await player.SetarRoupas();
                }
                else if (item.Category == ItemCategory.Weapon)
                {
                    if (oldSlot < 0 && slot > 0)
                    {
                        if (player.CurrentWeapon == item.Type)
                            player.RemoveAllWeapons(true);
                    }
                }
                else if (item.Category == ItemCategory.WalkieTalkie)
                {
                    if (oldSlot < 0 && slot > 0)
                        player.RadioCommunicatorItem = new();
                    else if (slot < 0)
                        player.RadioCommunicatorItem = Functions.Deserialize<WalkieTalkieItem>(item.Extra);
                }
                else if (item.Category == ItemCategory.Cellphone)
                {
                    if (oldSlot < 0 && slot > 0)
                    {
                        player.Cellphone = 0;
                        player.CellphoneItem = new CellphoneItem();
                        await player.EndCellphoneCall();
                    }
                    else if (slot < 0)
                    {
                        player.Cellphone = item.Type;
                        player.CellphoneItem = Functions.Deserialize<CellphoneItem>(item.Extra);
                    }

                    player.ToggleViewCellphone();
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(GiveItem))]
        public static async Task GiveItem(MyPlayer player, string id, int quantity, int targetId)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode entregar um item ferido/algemado.");
                    return;
                }

                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (!Functions.CanDropItem(player.Character.Sex, player.Faction, item))
                {
                    player.Emit("Server:MostrarErro", "Você não pode entregar este item.");
                    return;
                }

                if (quantity <= 0 || quantity > item.Quantity)
                {
                    player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                    return;
                }

                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == targetId && x.Character.Id != player.Character.Id);
                if (target == null)
                {
                    player.Emit("Server:MostrarErro", "Jogador inválido.");
                    return;
                }

                if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
                {
                    player.Emit("Server:MostrarErro", "Jogador não está próximo de você.");
                    return;
                }

                if (item.GetIsCloth() && player.Character.Sex != target.Character.Sex)
                {
                    player.Emit("Server:MostrarErro", "Você só pode entregar roupas para um jogador do seu sexo.");
                    return;
                }

                var itemTarget = new CharacterItem();
                itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                var res = await target.GiveItem(itemTarget);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.Emit("Server:MostrarErro", res);
                    return;
                }

                if (item.GetIsStack())
                    await player.RemoveStackedItem(item.Category, item.Quantity);
                else
                    await player.RemoveItem(item);

                await player.GravarLog(LogType.DeliverItem, Functions.Serialize(itemTarget), target);

                player.SendMessageToNearbyPlayers($"entrega {quantity:N0}x {itemTarget.GetName()} para {target.ICName}.", MessageCategory.Ame, 5);
                var msgSucesso = $"Você entregou {quantity:N0}x {itemTarget.GetName()} para {target.ICName}.";
                player.Emit("Server:MostrarErro", msgSucesso);
                target.SendMessage(MessageType.Success, $"{player.ICName} entregou para você {quantity:N0}x {itemTarget.GetName()}.");
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ClientEvent(nameof(DropItem))]
        public static void DropItem(MyPlayer player, string id, int quantity)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode dropar um item ferido/algemado.");
                    return;
                }

                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (!Functions.CanDropItem(player.Character.Sex, player.Faction, item)
                    || string.IsNullOrWhiteSpace(item.GetObjectName()))
                {
                    player.Emit("Server:MostrarErro", "Você não pode dropar este item.");
                    return;
                }

                if (quantity <= 0 || quantity > item.Quantity)
                {
                    player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                    return;
                }

                if (player.IsInVehicle)
                {
                    player.Emit("Server:MostrarErro", "Você não pode dropar um item dentro de um veículo.");
                    return;
                }

                player.DropItem = item;
                player.DropItemQuantity = quantity;
                player.Emit("DropObject", item.GetObjectName(), 0);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ClientEvent(nameof(CancelDropItem))]
        public static void CancelDropItem(MyPlayer player)
        {
            try
            {
                player.DropItem = null;
                player.DropItemQuantity = 0;
                player.SendMessage(MessageType.Success, "Você cancelou o drop do item.");
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(ConfirmDropItem))]
        public static async Task ConfirmDropItem(MyPlayer player, Vector3 position, Vector3 rotation)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.SendMessage(MessageType.Error, "Você não pode dropar um item ferido/algemado.");
                    return;
                }

                if (player.DropItem == null || !player.Items.Any(x => x.Id == player.DropItem.Id))
                    return;

                if (position.X == 0 || position.Y == 0 || position.Z == 0)
                {
                    player.SendMessage(MessageType.Error, "Não foi possível recuperar a posição do item.");
                    return;
                }

                var item = new Item();
                item.Create(player.DropItem.Category, player.DropItem.Type, player.DropItemQuantity, player.DropItem.Extra);
                item.SetPosition(player.Dimension, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z);

                await using var context = new DatabaseContext();
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                if (player.DropItem.GetIsStack())
                    await player.RemoveStackedItem(player.DropItem.Category, player.DropItem.Quantity);
                else
                    await player.RemoveItem(player.DropItem);

                Global.Items.Add(item);
                item.CreateObject();

                player.DropItem = null;
                player.DropItemQuantity = 0;

                player.SendMessageToNearbyPlayers($"dropa {item.Quantity:N0}x {item.GetName()} no chão.", MessageCategory.Ame, 5);
                await player.GravarLog(LogType.DropItem, Functions.Serialize(item), null);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(RobItem))]
        public static async Task RobItem(MyPlayer player, string id, int quantity)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode roubar um item ferido/algemado.");
                    return;
                }

                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == player.InventoryTargetId);
                if (target == null)
                    return;

                if (player.Faction?.Type == FactionType.Police)
                {
                    if (!player.OnDuty)
                    {
                        player.Emit("Server:MostrarErro", "Você não está em serviço.");
                        return;
                    }
                }
                else
                {
                    if (Global.SpawnedPlayers.Count(x => x.OnDuty && x.Faction?.Type == FactionType.Police) < 2)
                    {
                        player.Emit("Server:MostrarErro", "É necessário 2 policiais em serviço para roubar.");
                        return;
                    }
                }

                var item = target.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (!Functions.CanDropItem(target.Character.Sex, target.Faction, item))
                {
                    player.Emit("Server:MostrarErro", "Você não pode pegar este item.");
                    return;
                }

                if (quantity <= 0 || quantity > item.Quantity)
                {
                    player.SendMessage(MessageType.Error, $"Quantidade deve ser entre 1 e {item.Quantity}.");
                    return;
                }

                if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
                {
                    player.Emit("Server:MostrarErro", "Jogador não está próximo de você.");
                    return;
                }

                if (item.GetIsCloth() && player.Character.Sex != target.Character.Sex)
                {
                    player.Emit("Server:MostrarErro", "Você só pode pegar roupas de um jogador do seu sexo.");
                    return;
                }

                var itemTarget = new CharacterItem();
                itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                var res = await player.GiveItem(itemTarget);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.Emit("Server:MostrarErro", res);
                    return;
                }

                if (item.GetIsStack())
                    await player.RemoveStackedItem(item.Category, quantity);
                else
                    await player.RemoveItem(item);

                await player.GravarLog(LogType.StealItem, Functions.Serialize(itemTarget), target);

                player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {itemTarget.GetName()} de {target.ICName}.", MessageCategory.Ame, 5);
                var msgSucesso = $"Você pegou {quantity:N0}x {itemTarget.GetName()} de {target.ICName}.";
                player.Emit("Server:MostrarErro", msgSucesso);
                target.SendMessage(MessageType.Success, $"{player.ICName} pegou {quantity:N0}x {itemTarget.GetName()} de você.");
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(DiscardItem))]
        public static async Task DiscardItem(MyPlayer player, string id, int quantity)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode descartar um item ferido/algemado.");
                    return;
                }

                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (item.Category == ItemCategory.Weapon)
                {
                    player.Emit("Server:MostrarErro", $"Você não pode descartar uma arma.");
                    return;
                }

                if (quantity <= 0 || quantity > item.Quantity)
                {
                    player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                    return;
                }

                if (player.IsInVehicle)
                {
                    player.Emit("Server:MostrarErro", "Você não pode descartar um item dentro de um veículo.");
                    return;
                }

                if (item.GetIsStack())
                    await player.RemoveStackedItem(item.Category, quantity);
                else
                    await player.RemoveItem(item);

                var itemTarget = new CharacterItem();
                itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                await player.GravarLog(LogType.DiscardItem, Functions.Serialize(itemTarget), null);

                player.ShowInventory(player, update: true);

                player.SendMessageToNearbyPlayers($"descarta {quantity:N0}x {item.GetName()}.", MessageCategory.Ame, 5);
                player.Emit("Server:MostrarErro", $"Você descartou {quantity:N0}x {item.GetName()}.");
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(StoreItem))]
        public static async Task StoreItem(MyPlayer player, string id, int quantity)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode armazenar um item ferido/algemado.");
                    return;
                }

                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (!Functions.CanDropItem(player.Character.Sex, player.Faction, item))
                {
                    player.Emit("Server:MostrarErro", "Você não pode armazenar este item.");
                    return;
                }

                if (quantity <= 0 || quantity > item.Quantity)
                {
                    player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                    return;
                }

                if (player.InventoryShowType == InventoryShowType.Property)
                {
                    var prop = Global.Properties.FirstOrDefault(x => x.Id == player.InventoryRightTargetId);
                    if (prop == null)
                        return;

                    if (prop.Items.Count(x => x.Slot > 0)
                        + ((!item.GetIsStack() || !prop.Items.Any(x => x.Category == item.Category)) ? 1 : 0)
                        > Global.QUANTIDADE_SLOTS_INVENTARIO)
                    {
                        player.Emit("Server:MostrarErro", $"Não é possível prosseguir pois os novos itens ultrapassarão a quantidade de slots do armazenamento ({Global.QUANTIDADE_SLOTS_INVENTARIO}).");
                        return;
                    }

                    PropertyItem? it = null;
                    await using var context = new DatabaseContext();
                    if (item.GetIsStack())
                    {
                        it = prop.Items.FirstOrDefault(x => x.Category == item.Category);
                        if (it != null)
                        {
                            it.SetQuantity(it.Quantity + quantity);
                            context.PropertiesItems.Update(it);
                        }
                    }

                    if (it == null)
                    {
                        it = new PropertyItem();
                        it.Create(item.Category, item.Type, quantity, item.Extra);
                        it.SetPropertyId(player.InventoryRightTargetId!.Value);
                        it.SetSlot(Convert.ToByte(Enumerable.Range(1, Global.QUANTIDADE_SLOTS_INVENTARIO)
                            .FirstOrDefault(i => !prop.Items.Any(x => x.Slot == i))));

                        await context.PropertiesItems.AddAsync(it);
                        prop.Items.Add(it);
                    }

                    await context.SaveChangesAsync();
                    if (item.GetIsStack())
                        await player.RemoveStackedItem(item.Category, quantity);
                    else
                        await player.RemoveItem(item);
                    await player.GravarLog(LogType.PutPropertyItem, $"{prop.Id} | {Functions.Serialize(item)}", null);

                    player.SendMessageToNearbyPlayers($"armazena {quantity:N0}x {item.GetName()} na propriedade.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você armazenou {quantity}x {item.GetName()} na propriedade {prop.Id}.");

                    prop.ShowInventory(player, true);
                }
                else
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    if (veh == null)
                        return;

                    if (veh.VehicleDB.Items.Count(x => x.Slot > 0)
                        + ((!item.GetIsStack() || !veh.VehicleDB.Items.Any(x => x.Category == item.Category)) ? 1 : 0)
                        > Global.QUANTIDADE_SLOTS_INVENTARIO)
                    {
                        player.Emit("Server:MostrarErro", $"Não é possível prosseguir pois os novos itens ultrapassarão a quantidade de slots do armazenamento ({Global.QUANTIDADE_SLOTS_INVENTARIO}).");
                        return;
                    }

                    VehicleItem? it = null;
                    await using var context = new DatabaseContext();
                    if (item.GetIsStack())
                    {
                        it = veh.VehicleDB.Items.FirstOrDefault(x => x.Category == item.Category);
                        if (it != null)
                        {
                            it.SetQuantity(it.Quantity + quantity);
                            context.VehiclesItems.Update(it);
                        }
                    }

                    if (it == null)
                    {
                        it = new VehicleItem();
                        it.Create(item.Category, item.Type, quantity, item.Extra);
                        it.SetVehicleId(player.InventoryRightTargetId!.Value);
                        it.SetSlot(Convert.ToByte(Enumerable.Range(1, Global.QUANTIDADE_SLOTS_INVENTARIO)
                            .FirstOrDefault(i => !veh.VehicleDB.Items.Any(x => x.Slot == i))));

                        await context.VehiclesItems.AddAsync(it);
                        veh.VehicleDB.Items.Add(it);
                    }

                    await context.SaveChangesAsync();
                    if (item.GetIsStack())
                        await player.RemoveStackedItem(item.Category, quantity);
                    else
                        await player.RemoveItem(item);
                    await player.GravarLog(LogType.PutVehicleItem, $"{veh.VehicleDB.Id} | {Functions.Serialize(item)}", null);

                    player.SendMessageToNearbyPlayers($"armazena {quantity:N0}x {item.GetName()} no veículo.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você armazenou {quantity:N0}x {item.GetName()} no veículo {veh.VehicleDB.Id}.");

                    veh.ShowInventory(player, true);
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(MoveRightItem))]
        public static async Task MoveRightItem(MyPlayer player, string id, short slot)
        {
            try
            {
                slot -= 1000;

                if (player.InventoryShowType == InventoryShowType.Property)
                {
                    var prop = Global.Properties.FirstOrDefault(x => x.Id == player.InventoryRightTargetId);
                    if (prop == null)
                        return;

                    var item = prop.Items!.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null || item.Slot == slot)
                        return;

                    item.SetSlot(Convert.ToByte(slot));

                    await using var context = new DatabaseContext();
                    context.PropertiesItems.Update(item);
                    await context.SaveChangesAsync();

                    prop.ShowInventory(player, true);
                }
                else
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    if (veh == null)
                        return;

                    var item = veh.VehicleDB.Items.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null || item.Slot == slot)
                        return;

                    item.SetSlot(Convert.ToByte(slot));

                    await using var context = new DatabaseContext();
                    context.VehiclesItems.Update(item);
                    await context.SaveChangesAsync();

                    veh.ShowInventory(player, true);
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(GetItem))]
        public static async Task GetItem(MyPlayer player, string id, int quantity)
        {
            try
            {
                if (player.Character.Wound != CharacterWound.None || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode pegar um item ferido/algemado.");
                    return;
                }
                if (player.IsInVehicle)
                {
                    player.Emit("Server:MostrarErro", "Você não pode pegar um item dentro de um veículo.");
                    return;
                }

                if (player.InventoryShowType == InventoryShowType.Property)
                {
                    var prop = Global.Properties.FirstOrDefault(x => x.Id == player.InventoryRightTargetId);
                    if (prop == null)
                        return;

                    var item = prop.Items.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null)
                        return;

                    if (quantity <= 0 || quantity > item.Quantity)
                    {
                        player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                        return;
                    }

                    if (item.GetIsCloth())
                    {
                        var extra = Functions.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sex != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var itemTarget = new CharacterItem();
                    itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                    var res = await player.GiveItem(itemTarget);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    item.SetQuantity(item.Quantity - quantity);
                    if (item.Quantity == 0)
                    {
                        await using var context = new DatabaseContext();
                        context.PropertiesItems.Remove(item);
                        await context.SaveChangesAsync();
                        prop.Items.Remove(item);
                    }

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.GetName()} da propriedade.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity:N0}x {item.GetName()} da propriedade {prop.Id}.");

                    await player.GravarLog(LogType.GetPropertyItem, $"{prop.Id} | {Functions.Serialize(item)}", null);

                    prop.ShowInventory(player, true);
                }
                else if (player.InventoryShowType == InventoryShowType.Vehicle)
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    if (veh == null)
                        return;

                    var item = veh.VehicleDB.Items.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null)
                        return;

                    if (quantity <= 0 || quantity > item.Quantity)
                    {
                        player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                        return;
                    }

                    if (item.GetIsCloth())
                    {
                        var extra = Functions.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sex != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var itemTarget = new CharacterItem();
                    itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                    var res = await player.GiveItem(itemTarget);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    item.SetQuantity(item.Quantity - quantity);
                    if (item.Quantity == 0)
                    {
                        await using var context = new DatabaseContext();
                        context.VehiclesItems.Remove(item);
                        await context.SaveChangesAsync();
                        veh.VehicleDB.Items.Remove(item);
                    }

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.GetName()} do veículo.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity:N0}x {item.GetName()} do veículo {veh.VehicleDB.Id}.");

                    await player.GravarLog(LogType.GetVehicleItem, $"{veh.VehicleDB.Id} | {Functions.Serialize(item)}", null);

                    veh.ShowInventory(player, true);
                }
                else
                {
                    var item = Global.Items.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null)
                    {
                        player.Emit("Server:MostrarErro", "Você não está próximo deste item.");
                        return;
                    }

                    if (quantity <= 0 || quantity > item.Quantity)
                    {
                        player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                        return;
                    }

                    if (item.GetIsCloth())
                    {
                        var extra = Functions.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sex != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var itemTarget = new CharacterItem();
                    itemTarget.Create(item.Category, item.Type, quantity, item.Extra);

                    var res = await player.GiveItem(itemTarget);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    await using var context = new DatabaseContext();
                    item.SetQuantity(item.Quantity - quantity);
                    if (item.Quantity == 0)
                    {
                        item.DeleteObject();
                        context.Items.Remove(item);
                    }
                    else
                    {
                        context.Items.Update(item);
                    }
                    await context.SaveChangesAsync();

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.GetName()} do chão.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity}x {item.GetName()} do chão.");
                    await player.GravarLog(LogType.GetGroundItem, Functions.Serialize(itemTarget), null);
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ClientEvent(nameof(EquipWeapon))]
        public static void EquipWeapon(MyPlayer player, int slot)
        {
            try
            {
                player.RemoveAllWeapons(true);

                if (slot == 0)
                    return;

                var item = player.Items.FirstOrDefault(x => x.Category == ItemCategory.Weapon && x.Slot == slot);
                if (item == null)
                    return;

                if (item.Type == player.CurrentWeapon)
                    return;

                player.GiveEquippedWeapon(item);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [Command("usardroga", "/usardroga (nome) (quantidade)")]
        public static async void CMD_usardroga(MyPlayer player, string name, int quantity)
        {
            if (!Enum.TryParse(name, true, out ItemCategory itemCategory))
            {
                player.SendMessage(MessageType.Error, $"Droga {name} não existe.");
                return;
            }

            if (!Functions.CheckIfIsDrug(itemCategory))
            {
                player.SendMessage(MessageType.Error, $"Droga {name} não existe.");
                return;
            }

            if (quantity <= 0)
            {
                player.SendMessage(MessageType.Error, $"Quantidade deve ser maior que 0.");
                return;
            }

            var item = player.Items.FirstOrDefault(x => x.Category == itemCategory && x.Quantity >= quantity);
            if (item == null)
            {
                player.SendMessage(MessageType.Error, $"Você não possui essa quantidade de {itemCategory.GetDisplay()}.");
                return;
            }

            if (player.Character.DrugItemCategory.HasValue && player.Character.DrugItemCategory != item.Category)
            {
                player.SendMessage(MessageType.Error, $"Você está sob efeito de {player.Character.DrugItemCategory.GetDisplay()}. Não é possível usar {item.Category.GetDisplay()}.");
                return;
            }

            if (!player.Character.DrugItemCategory.HasValue)
            {
                switch (item.Category)
                {
                    case ItemCategory.Weed:
                        player.Health = (ushort)(player.Health + 25);
                        break;
                    case ItemCategory.Cocaine:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Crack:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Heroin:
                        player.Health = (ushort)(player.Health + 80);
                        break;
                    case ItemCategory.MDMA:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Xanax:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Oxycontin:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                }
            }

            await player.RemoveStackedItem(item.Category, quantity);

            player.Character.UseDrug(item.Category, quantity);

            player.SendMessage(MessageType.Success, $"Você usou {quantity}x {player.Character.DrugItemCategory!.GetDisplay()} e seu limiar da morte está em {player.Character.ThresoldDeath}/100.");
            player.SetupDrugTimer(true);

            if (player.Character.ThresoldDeath == 100)
            {
                player.Health = 0;
                player.SendMessage(MessageType.Error, "Você atingiu 100 da limiar de morte e sofreu uma overdose.");
            }
        }
    }
}