using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffFactionStorageScript : IScript
    {
        [Command("aarmazenamentos")]
        public static void CMD_aarmazenamentos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFactionsStorages", false, GetFactionsStoragesHTML());
        }

        [ClientEvent(nameof(StaffFactionStorageGoto))]
        public static void StaffFactionStorageGoto(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var factionStorage = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
            if (factionStorage == null)
                return;

            player.LimparIPLs();

            if (factionStorage.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Number == factionStorage.Dimension)!.Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(factionStorage.PosX, factionStorage.PosY, factionStorage.PosZ), factionStorage.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffFactionStorageRemove))]
        public static async Task StaffFactionStorageRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var factionStorage = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
            if (factionStorage == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionStorages.Remove(factionStorage);
            context.FactionStoragesItems.RemoveRange(Global.FactionsStoragesItems.Where(x => x.FactionStorageId == id));
            await context.SaveChangesAsync();
            Global.FactionsStorages.Remove(factionStorage);
            Global.FactionsStoragesItems.RemoveAll(x => x.FactionStorageId == factionStorage.Id);
            factionStorage.RemoveIdentifier();

            player.EmitStaffShowMessage($"Arsenal {factionStorage.Id} excluído.");

            var html = GetFactionsStoragesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
                target.Emit("StaffFactionsStorages", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Arsenal | {Functions.Serialize(factionStorage)}", null);
        }

        [AsyncClientEvent(nameof(StaffFactionStorageSave))]
        public static async Task StaffFactionStorageSave(MyPlayer player, string idString, string factionIdString, Vector3 pos, int dimension)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionId = factionIdString.ToGuid();
            if (!Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            var id = idString.ToGuid();
            var isNew = !id.HasValue;
            var factionStorage = new FactionStorage();
            if (isNew)
            {
                factionStorage.Create(factionId!.Value, pos.X, pos.Y, pos.Z, dimension);
            }
            else
            {
                factionStorage = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
                if (factionStorage == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                factionStorage.Update(factionId!.Value, pos.X, pos.Y, pos.Z, dimension);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.FactionStorages.AddAsync(factionStorage);
            else
                context.FactionStorages.Update(factionStorage);

            await context.SaveChangesAsync();

            if (isNew)
                Global.FactionsStorages.Add(factionStorage);

            factionStorage.CreateIdentifier();

            player.EmitStaffShowMessage($"Arsenal {(isNew ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Arsenal | {Functions.Serialize(factionStorage)}", null);

            var html = GetFactionsStoragesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
                target.Emit("StaffFactionsStorages", true, html);
        }

        //[ClientEvent(nameof(StaffFactionsStoragesWeaponsShow))]
        //public static void StaffFactionsStoragesWeaponsShow(MyPlayer player, string idString)
        //{
        //    if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
        //    {
        //        player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
        //        return;
        //    }

        //    player.Emit("Server:CloseView");

        //    var id = idString.ToGuid();
        //    player.Emit("StaffFactionsStoragesWeapons",
        //        false,
        //        GetFactionStoragesWeaponsHTML(id),
        //        idString);
        //}

        //[AsyncClientEvent(nameof(StaffFactionStorageWeaponSave))]
        //public static async Task StaffFactionStorageWeaponSave(MyPlayer player, string factionStorageItemIdString, string factionStorageIdString, string weapon,
        //    int ammo, int quantity, int tintIndex, string componentsJSON)
        //{
        //    if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
        //    {
        //        player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
        //        return;
        //    }

        //    if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
        //    {
        //        player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
        //        return;
        //    }

        //    if (quantity < 0)
        //    {
        //        player.EmitStaffShowMessage($"Estoque deve ser maior ou igual a 0.");
        //        return;
        //    }

        //    if (!Enum.TryParse(weapon, true, out WeaponModel wep) || wep == 0)
        //    {
        //        player.EmitStaffShowMessage($"Arma {weapon} não existe.");
        //        return;
        //    }

        //    if (ammo <= 0)
        //    {
        //        player.EmitStaffShowMessage($"Munição deve ser maior que 0.");
        //        return;
        //    }

        //    if (tintIndex < 0 || tintIndex > 7)
        //    {
        //        player.EmitStaffShowMessage($"Pintura deve ser entre 0 e 7.");
        //        return;
        //    }

        //    var realComponents = new List<uint>();
        //    var components = Functions.Deserialize<List<string>>(componentsJSON);
        //    foreach (var component in components)
        //    {
        //        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.Equals(component, StringComparison.CurrentCultureIgnoreCase) && x.Weapon == wep);
        //        if (comp == null)
        //        {
        //            player.EmitStaffShowMessage($"Componente {component} não existe para a arma {wep}.");
        //            return;
        //        }

        //        if (realComponents.Contains(comp.Hash))
        //        {
        //            player.EmitStaffShowMessage($"Componente {component} foi inserido na lista mais de uma vez.");
        //            return;
        //        }

        //        realComponents.Add(comp.Hash);
        //    }

        //    var factionStorageWeapon = new FactionStorageItem();
        //    if (factionStorageWeaponId > 0)
        //        factionStorageWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == factionStorageWeaponId);

        //    factionStorageWeapon.FactionStorageId = factionStorageId;
        //    factionStorageWeapon.Model = wep;
        //    factionStorageWeapon.Ammo = ammo;
        //    factionStorageWeapon.Quantity = quantity;
        //    factionStorageWeapon.TintIndex = Convert.ToByte(tintIndex);
        //    factionStorageWeapon.ComponentsJSON = Functions.Serialize(realComponents);

        //    await using var context = new DatabaseContext();

        //    if (factionStorageWeapon.Id == 0)
        //        await context.FactionStoragesItems.AddAsync(factionStorageWeapon);
        //    else
        //        context.FactionStoragesItems.Update(factionStorageWeapon);

        //    await context.SaveChangesAsync();

        //    if (factionStorageWeaponId == 0)
        //        Global.FactionsStoragesItems.Add(factionStorageWeapon);

        //    player.EmitStaffShowMessage($"Arma {(factionStorageWeaponId == 0 ? "criada" : "editada")}.", true);

        //    await player.GravarLog(LogType.Staff, $"Gravar Arma Arsenal | {Functions.Serialize(factionStorageWeapon)}", null);

        //    var html = GetFactionStoragesWeaponsHTML(factionStorageId);
        //    foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
        //        target.Emit("StaffFactionsStoragesWeapons", true, html);
        //}

        //[AsyncClientEvent(nameof(StaffFactionStorageWeaponRemove))]
        //public static async Task StaffFactionStorageWeaponRemove(MyPlayer player, string idString)
        //{
        //    if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
        //    {
        //        player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
        //        return;
        //    }

        //    var id = idString.ToGuid();
        //    var factionStorageWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == id);
        //    if (factionStorageWeapon == null)
        //        return;

        //    await using var context = new DatabaseContext();
        //    context.FactionStoragesItems.Remove(factionStorageWeapon);
        //    await context.SaveChangesAsync();
        //    Global.FactionsStoragesItems.Remove(factionStorageWeapon);

        //    player.EmitStaffShowMessage($"Arma do Arsenal {factionStorageWeapon.Id} excluída.");

        //    await player.GravarLog(LogType.Staff, $"Remover Arma Arsenal | {Functions.Serialize(factionStorageWeapon)}", null);

        //    var html = GetFactionStoragesWeaponsHTML(factionStorageWeapon.FactionStorageId);
        //    foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
        //        target.Emit("StaffFactionsStoragesWeapons", true, html);
        //}

        private static string GetFactionsStoragesHTML()
        {
            var html = string.Empty;
            if (Global.FactionsStorages.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='5'>Não há armazenamentos criados.</td></tr>";
            }
            else
            {
                foreach (var factionStorage in Global.FactionsStorages.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{factionStorage.Id}</td>
                        <td>{factionStorage.FactionId}</td>
                        <td>X: {factionStorage.PosX} | Y: {factionStorage.PosY} | Z: {factionStorage.PosZ}</td>
                        <td>{factionStorage.Dimension}</td>
                        <td class='text-center'>
                            <input id='json{factionStorage.Id}' type='hidden' value='{Functions.Serialize(factionStorage)}' />
                            <button onclick='goto(`{factionStorage.Id}`)' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit(`{factionStorage.Id}`)' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editItems(`{factionStorage.Id}`)' type='button' class='btn btn-dark btn-sm'>ITENS</button>
                            <button onclick='remove(this, `{factionStorage.Id}`)' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        //private static string GetFactionStoragesWeaponsHTML(Guid factionStorageId)
        //{
        //    var html = string.Empty;
        //    var factionsStoragesWeapons = Global.FactionsStoragesItems.Where(x => x.FactionStorageId == factionStorageId);
        //    if (!factionsStoragesWeapons.Any())
        //    {
        //        html = "<tr><td class='text-center' colspan='8'>Não há armas criadas.</td></tr>";
        //    }
        //    else
        //    {
        //        var factionStorage = Global.FactionsStorages.FirstOrDefault(x => x.Id == factionStorageId);
        //        var faction = Global.Factions.FirstOrDefault(x => x.Id == factionStorage.FactionId);

        //        foreach (var factionStorageWeapon in factionsStoragesWeapons)
        //        {
        //            var realComponents = new List<string>();
        //            foreach (var component in Functions.Deserialize<List<uint>>(factionStorageWeapon.ComponentsJSON))
        //            {
        //                var comp = Global.WeaponComponents.FirstOrDefault(x => x.Hash == component && x.Weapon.ToString() == factionStorageWeapon.Model);
        //                if (comp != null)
        //                    realComponents.Add(comp.Name);
        //            }

        //            html += $@"<tr class='pesquisaitem'>
        //                <td>{factionStorageWeapon.Id}</td>
        //                <td>{factionStorageWeapon.Model}</td>
        //                <td>{factionStorageWeapon.Ammo}</td>
        //                <td>{factionStorageWeapon.Quantity}</td>
        //                <td>{factionStorageWeapon.TintIndex}</td>
        //                {(!faction.Government && !staff ? $"<td>{Global.Prices.FirstOrDefault(y => y.Type == PriceType.Weapons && y.Name.Equals(factionStorageWeapon.Model.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0:N0}</td>" : string.Empty)}
        //                <td>{string.Join(", ", realComponents)}</td>
        //                <td class='text-center'>
        //                    <input id='json{factionStorageWeapon.Id}' type='hidden' value='{Functions.Serialize(new
        //            {
        //                Weapon = factionStorageWeapon.Model.ToString(),
        //                factionStorageWeapon.Ammo,
        //                factionStorageWeapon.Quantity,
        //                factionStorageWeapon.TintIndex,
        //                Components = realComponents,
        //            })}' />
        //                    <button onclick='edit({factionStorageWeapon.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
        //                    <button onclick='remove(this, {factionStorageWeapon.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
        //                </td>
        //            </tr>";
        //        }
        //    }
        //    return html;
        //}
    }
}