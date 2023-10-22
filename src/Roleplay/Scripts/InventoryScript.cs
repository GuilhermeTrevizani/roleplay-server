using AltV.Net;
using AltV.Net.Async;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Numerics;
using System.Text.Json;

namespace Roleplay.Scripts
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
                        ItemCategory.Weapon => new int[] { -1, -2, -3 },
                        ItemCategory.Cloth3 => new int[] { -101 },
                        ItemCategory.Cloth11 => new int[] { -102 },
                        ItemCategory.Cloth8 => new int[] { -103 },
                        ItemCategory.Cloth4 => new int[] { -104 },
                        ItemCategory.Cloth6 => new int[] { -105 },
                        ItemCategory.Cloth7 => new int[] { -106 },
                        ItemCategory.Cloth10 => new int[] { -107 },
                        ItemCategory.Cloth9 => new int[] { -108 },
                        ItemCategory.Cloth1 => new int[] { -109 },
                        ItemCategory.Accessory0 => new int[] { -110 },
                        ItemCategory.Accessory1 => new int[] { -111 },
                        ItemCategory.Accessory2 => new int[] { -112 },
                        ItemCategory.Accessory6 => new int[] { -113 },
                        ItemCategory.Accessory7 => new int[] { -114 },
                        ItemCategory.Cloth5 => new int[] { -115 },
                        ItemCategory.WalkieTalkie => new int[] { -116 },
                        ItemCategory.Cellphone => new int[] { -117 },
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

                item.Slot = slot;

                await using var context = new DatabaseContext();
                context.CharactersItems.Update(item);
                await context.SaveChangesAsync();

                player.ShowInventory(player, update: true);

                if (item.IsCloth)
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
                        player.RadioCommunicatorItem = JsonSerializer.Deserialize<RadioCommunicatorItem>(item.Extra);
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
                        player.CellphoneItem = JsonSerializer.Deserialize<CellphoneItem>(item.Extra);
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
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

                var target = Global.Players.FirstOrDefault(x => x.SessionId == targetId
                    && x.Character.Id > 0
                    && x.Character.Id != player.Character.Id);
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

                if (item.IsCloth && player.Character.Sex != target.Character.Sex)
                {
                    player.Emit("Server:MostrarErro", "Você só pode entregar roupas para um jogador do seu sexo.");
                    return;
                }

                var itemTarget = new CharacterItem(item.Category, item.Type)
                {
                    Quantity = quantity,
                    Extra = item.Extra,
                };

                var res = await target.GiveItem(itemTarget);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.Emit("Server:MostrarErro", res);
                    return;
                }

                await player.RemoveItem(item.IsStack ?
                    new CharacterItem(item.Category)
                    {
                        Quantity = quantity,
                    }
                    :
                    item);

                await player.GravarLog(LogType.EntregarItem, JsonSerializer.Serialize(itemTarget), target);

                player.SendMessageToNearbyPlayers($"entrega {quantity:N0}x {itemTarget.Name} para {target.ICName}.", MessageCategory.Ame, 5);
                var msgSucesso = $"Você entregou {quantity:N0}x {itemTarget.Name} para {target.ICName}.";
                player.Emit("Server:MostrarErro", msgSucesso);
                target.SendMessage(MessageType.Success, $"{player.ICName} entregou para você {quantity:N0}x {itemTarget.Name}.");
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode dropar um item ferido/algemado.");
                    return;
                }

                var item = player.Items.FirstOrDefault(x => x.Id == new Guid(id));
                if (item == null)
                    return;

                if (!Functions.CanDropItem(player.Character.Sex, player.Faction, item)
                    || string.IsNullOrWhiteSpace(item.ObjectName))
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
                player.Emit("DropObject", item.ObjectName, 0);
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
                {
                    player.SendMessage(MessageType.Error, "Você não pode dropar um item ferido/algemado.");
                    return;
                }

                if (!player.Items.Any(x => x.Id == player.DropItem?.Id))
                    return;

                if (position.X == 0 || position.Y == 0 || position.Z == 0)
                {
                    player.SendMessage(MessageType.Error, "Não foi possível recuperar a posição do item.");
                    return;
                }

                var item = new Item(player.DropItem.Category, player.DropItem.Type)
                {
                    Dimension = player.Dimension,
                    PosX = position.X,
                    PosY = position.Y,
                    PosZ = position.Z,
                    RotR = rotation.X,
                    RotP = rotation.Y,
                    RotY = rotation.Z,
                    Extra = player.DropItem.Extra,
                    Quantity = player.DropItemQuantity,
                };

                await using var context = new DatabaseContext();
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                await player.RemoveItem(player.DropItem.IsStack ?
                    new CharacterItem(player.DropItem.Category)
                    {
                        Quantity = player.DropItemQuantity,
                    }
                    :
                    player.DropItem);

                Global.Items.Add(item);
                item.CreateObject();

                player.DropItem = null;
                player.DropItemQuantity = 0;

                player.SendMessageToNearbyPlayers($"dropa {item.Quantity:N0}x {item.Name} no chão.", MessageCategory.Ame, 5);
                await player.GravarLog(LogType.DroparItem, JsonSerializer.Serialize(item), null);
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
                {
                    player.Emit("Server:MostrarErro", "Você não pode roubar um item ferido/algemado.");
                    return;
                }

                var target = Global.Players.FirstOrDefault(x => x.Character.Id == player.InventoryTargetId);
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
                    if (Global.Players.Count(x => x.OnDuty && x.Faction?.Type == FactionType.Police) < 2)
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

                if (item.IsCloth && player.Character.Sex != target.Character.Sex)
                {
                    player.Emit("Server:MostrarErro", "Você só pode pegar roupas de um jogador do seu sexo.");
                    return;
                }

                var itemTarget = new CharacterItem(item.Category, item.Type)
                {
                    Quantity = quantity,
                    Extra = item.Extra,
                };

                var res = await player.GiveItem(itemTarget);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.Emit("Server:MostrarErro", res);
                    return;
                }

                await target.RemoveItem(item.IsStack ?
                    new CharacterItem(item.Category)
                    {
                        Quantity = quantity,
                    }
                    :
                    item);

                await player.GravarLog(LogType.RoubarItem, JsonSerializer.Serialize(itemTarget), target);

                player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {itemTarget.Name} de {target.ICName}.", MessageCategory.Ame, 5);
                var msgSucesso = $"Você pegou {quantity:N0}x {itemTarget.Name} de {target.ICName}.";
                player.Emit("Server:MostrarErro", msgSucesso);
                target.SendMessage(MessageType.Success, $"{player.ICName} pegou {quantity:N0}x {itemTarget.Name} de você.");
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
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

                await player.RemoveItem(item.IsStack ?
                    new CharacterItem(item.Category)
                    {
                        Quantity = quantity,
                    }
                    :
                    item);

                var itemTarget = new CharacterItem(item.Category, item.Type)
                {
                    Quantity = quantity,
                    Extra = item.Extra,
                };
                await player.GravarLog(LogType.DescartarItem, JsonSerializer.Serialize(itemTarget), null);

                player.ShowInventory(player, update: true);

                player.SendMessageToNearbyPlayers($"descarta {quantity:N0}x {item.Name}.", MessageCategory.Ame, 5);
                player.Emit("Server:MostrarErro", $"Você descartou {quantity:N0}x {item.Name}.");
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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
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
                        + ((!item.IsStack || !prop.Items.Any(x => x.Category == item.Category)) ? 1 : 0)
                        > Global.QUANTIDADE_SLOTS_INVENTARIO)
                    {
                        player.Emit("Server:MostrarErro", $"Não é possível prosseguir pois os novos itens ultrapassarão a quantidade de slots do armazenamento ({Global.QUANTIDADE_SLOTS_INVENTARIO}).");
                        return;
                    }

                    PropertyItem it = null;
                    await using var context = new DatabaseContext();
                    if (item.IsStack)
                    {
                        it = prop.Items.FirstOrDefault(x => x.Category == item.Category);
                        if (it != null)
                        {
                            it.Quantity += quantity;
                            context.PropertiesItems.Update(it);
                        }
                    }

                    if (it == null)
                    {
                        it = new PropertyItem(item.Category, item.Type)
                        {
                            Quantity = quantity,
                            PropertyId = player.InventoryRightTargetId,
                            Extra = item.Extra,
                            Slot = Convert.ToByte(Enumerable.Range(1, Global.QUANTIDADE_SLOTS_INVENTARIO)
                                .FirstOrDefault(i => !prop.Items.Any(x => x.Slot == i))),
                        };

                        await context.PropertiesItems.AddAsync(it);
                        prop.Items.Add(it);
                    }

                    await context.SaveChangesAsync();
                    await player.RemoveItem(item.IsStack ?
                    new CharacterItem(item.Category)
                    {
                        Quantity = quantity,
                    }
                    :
                    item);

                    await player.GravarLog(LogType.ArmazenarItemPropriedade, $"{prop.Id} | {JsonSerializer.Serialize(item)}", null);

                    player.SendMessageToNearbyPlayers($"armazena {quantity:N0}x {item.Name} na propriedade.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você armazenou {quantity}x {item.Name} na propriedade {prop.Id}.");

                    prop.ShowInventory(player, true);
                }
                else
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    if (veh == null)
                        return;

                    if (veh.Itens.Count(x => x.Slot > 0)
                        + ((!item.IsStack || !veh.Itens.Any(x => x.Category == item.Category)) ? 1 : 0)
                        > Global.QUANTIDADE_SLOTS_INVENTARIO)
                    {
                        player.Emit("Server:MostrarErro", $"Não é possível prosseguir pois os novos itens ultrapassarão a quantidade de slots do armazenamento ({Global.QUANTIDADE_SLOTS_INVENTARIO}).");
                        return;
                    }

                    VehicleItem it = null;
                    await using var context = new DatabaseContext();
                    if (item.IsStack)
                    {
                        it = veh.Itens.FirstOrDefault(x => x.Category == item.Category);
                        if (it != null)
                        {
                            it.Quantity += quantity;
                            context.VehiclesItems.Update(it);
                        }
                    }

                    if (it == null)
                    {
                        it = new VehicleItem(item.Category, item.Type)
                        {
                            Quantity = quantity,
                            VehicleId = player.InventoryRightTargetId,
                            Extra = item.Extra,
                            Slot = Convert.ToByte(Enumerable.Range(1, Global.QUANTIDADE_SLOTS_INVENTARIO)
                                .FirstOrDefault(i => !veh.Itens.Any(x => x.Slot == i))),
                        };

                        await context.VehiclesItems.AddAsync(it);
                        veh.Itens.Add(it);
                    }

                    await context.SaveChangesAsync();
                    await player.RemoveItem(item.IsStack ?
                    new CharacterItem(item.Category)
                    {
                        Quantity = quantity,
                    }
                    :
                    item);

                    await player.GravarLog(LogType.ArmazenarItemVeiculo, $"{veh.VehicleDB.Id} | {JsonSerializer.Serialize(item)}", null);

                    player.SendMessageToNearbyPlayers($"armazena {quantity:N0}x {item.Name} no veículo.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você armazenou {quantity:N0}x {item.Name} no veículo {veh.VehicleDB.Id}.");

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
                    var item = prop.Items.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null || item.Slot == slot)
                        return;

                    item.Slot = Convert.ToByte(slot);

                    await using var context = new DatabaseContext();
                    context.PropertiesItems.Update(item);
                    await context.SaveChangesAsync();

                    prop.ShowInventory(player, true);
                }
                else
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    var item = veh.Itens.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null || item.Slot == slot)
                        return;

                    item.Slot = Convert.ToByte(slot);

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
                if (player.Character.Wound != CharacterWound.Nenhum || player.Cuffed)
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

                    if (item.IsCloth)
                    {
                        var extra = JsonSerializer.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sexo != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var personagemItem = new CharacterItem(item.Category, item.Type)
                    {
                        Extra = item.Extra,
                        Quantity = quantity,
                    };

                    var res = await player.GiveItem(personagemItem);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    item.Quantity -= quantity;
                    if (item.Quantity == 0)
                    {
                        await using var context = new DatabaseContext();
                        context.PropertiesItems.Remove(item);
                        await context.SaveChangesAsync();
                        prop.Items.Remove(item);
                    }

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.Name} da propriedade.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity:N0}x {item.Name} da propriedade {prop.Id}.");

                    await player.GravarLog(LogType.PegarItemPropriedade, $"{prop.Id} | {JsonSerializer.Serialize(item)}", null);

                    prop.ShowInventory(player, true);
                }
                else if (player.InventoryShowType == InventoryShowType.Vehicle)
                {
                    var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == player.InventoryRightTargetId);
                    if (veh == null)
                        return;

                    var item = veh.Itens.FirstOrDefault(x => x.Id == new Guid(id));
                    if (item == null)
                        return;

                    if (quantity <= 0 || quantity > item.Quantity)
                    {
                        player.Emit("Server:MostrarErro", $"Quantidade deve ser entre 1 e {item.Quantity}.");
                        return;
                    }

                    if (item.IsCloth)
                    {
                        var extra = JsonSerializer.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sexo != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var personagemItem = new CharacterItem(item.Category, item.Type)
                    {
                        Extra = item.Extra,
                        Quantity = quantity,
                    };

                    var res = await player.GiveItem(personagemItem);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    item.Quantity -= quantity;
                    if (item.Quantity == 0)
                    {
                        await using var context = new DatabaseContext();
                        context.VehiclesItems.Remove(item);
                        await context.SaveChangesAsync();
                        veh.Itens.Remove(item);
                    }

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.Name} do veículo.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity:N0}x {item.Name} do veículo {veh.VehicleDB.Id}.");

                    await player.GravarLog(LogType.PegarItemVeiculo, $"{veh.VehicleDB.Id} | {JsonSerializer.Serialize(item)}", null);

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

                    if (item.IsCloth)
                    {
                        var extra = JsonSerializer.Deserialize<ClotheAccessoryItem>(item.Extra);
                        if (extra.Sexo != player.Character.Sex)
                        {
                            player.Emit("Server:MostrarErro", $"O sexo desta peça de roupa não é o mesmo que o seu.");
                            return;
                        }
                    }

                    var personagemItem = new CharacterItem(item.Category, item.Type)
                    {
                        Extra = item.Extra,
                        Quantity = quantity,
                    };

                    var res = await player.GiveItem(personagemItem);

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.Emit("Server:MostrarErro", res);
                        return;
                    }

                    await using var context = new DatabaseContext();
                    item.Quantity -= quantity;
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

                    player.SendMessageToNearbyPlayers($"pega {quantity:N0}x {item.Name} do chão.", MessageCategory.Ame, 5);
                    player.Emit("Server:MostrarErro", $"Você pegou {quantity}x {item.Name} do chão.");
                    await player.GravarLog(LogType.PegarItemChao, JsonSerializer.Serialize(personagemItem), null);
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
    }
}