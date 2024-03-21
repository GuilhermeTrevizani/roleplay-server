//using AltV.Net;
//using AltV.Net.Async;
//using AltV.Net.Data;
//using AltV.Net.Enums;
//using System.Numerics;
//using TrevizaniRoleplay.Domain.Entities;
//using TrevizaniRoleplay.Domain.Enums;
//using TrevizaniRoleplay.Server.Extensions;
//using TrevizaniRoleplay.Server.Factories;
//using TrevizaniRoleplay.Server.Models;

//namespace TrevizaniRoleplay.Server.Scripts
//{
//    public class StaffFactionStorageScript : IScript
//    {
//        [Command("aarmazenamento")]
//        public static void CMD_aarmazenamento(MyPlayer player)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            player.Emit("StaffFactionsArmories", false, GetFactionsArmoriesHTML());
//        }

//        [ClientEvent(nameof(StaffFactionArmoryGoto))]
//        public static void StaffFactionArmoryGoto(MyPlayer player, string idString)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            var id = new Guid(idString);
//            var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
//            if (factionArmory == null)
//                return;

//            player.LimparIPLs();

//            if (factionArmory.Dimension > 0)
//            {
//                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Number == factionArmory.Dimension).Interior);
//                player.SetarIPLs();
//            }

//            player.SetPosition(new Position(factionArmory.PosX, factionArmory.PosY, factionArmory.PosZ), factionArmory.Dimension, false);
//        }

//        [AsyncClientEvent(nameof(StaffFactionArmoryRemove))]
//        public static async Task StaffFactionArmoryRemove(MyPlayer player, string idString)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            var id = new Guid(idString);
//            var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
//            if (factionArmory == null)
//                return;

//            await using var context = new DatabaseContext();
//            context.FactionStorages.Remove(factionArmory);
//            context.FactionStoragesItems.RemoveRange(Global.FactionsStoragesItems.Where(x => x.FactionStorageId == id));
//            await context.SaveChangesAsync();
//            Global.FactionsStorages.Remove(factionArmory);
//            Global.FactionsStoragesItems.RemoveAll(x => x.FactionStorageId == factionArmory.Id);
//            factionArmory.RemoveIdentifier();

//            player.EmitStaffShowMessage($"Arsenal {factionArmory.Id} excluído.");

//            var html = GetFactionsArmoriesHTML();
//            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
//                target.Emit("StaffFactionsArmories", true, html);

//            await player.GravarLog(LogType.Staff, $"Remover Arsenal | {Functions.Serialize(factionArmory)}", null);
//        }

//        [AsyncClientEvent(nameof(StaffFactionArmorySave))]
//        public static async Task StaffFactionArmorySave(MyPlayer player, string idString, string factionIdString, Vector3 pos, int dimension)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            var factionId = new Guid(factionIdString);
//            if (!Global.Factions.Any(x => x.Id == factionId))
//            {
//                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
//                return;
//            }

//            var id = new Guid(idString);
//            var factionArmory = new FactionStorage();
//            if (id > 0)
//                factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);

//            factionArmory.FactionId = factionId;
//            factionArmory.PosX = pos.X;
//            factionArmory.PosY = pos.Y;
//            factionArmory.PosZ = pos.Z;
//            factionArmory.Dimension = dimension;

//            await using var context = new DatabaseContext();

//            if (factionArmory.Id == 0)
//                await context.FactionStorages.AddAsync(factionArmory);
//            else
//                context.FactionStorages.Update(factionArmory);

//            await context.SaveChangesAsync();

//            if (id == 0)
//                Global.FactionsStorages.Add(factionArmory);

//            factionArmory.CreateIdentifier();

//            player.EmitStaffShowMessage($"Arsenal {(id == 0 ? "criado" : "editado")}.", true);

//            await player.GravarLog(LogType.Staff, $"Gravar Arsenal | {Functions.Serialize(factionArmory)}", null);

//            var html = GetFactionsArmoriesHTML();
//            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
//                target.Emit("StaffFactionsArmories", true, html);
//        }

//        [ClientEvent(nameof(StaffFactionsArmorysWeaponsShow))]
//        public static void StaffFactionsArmorysWeaponsShow(MyPlayer player, string idString)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            player.Emit("Server:CloseView");

//            var id = new Guid(idString);
//            player.Emit("StaffFactionsArmoriesWeapons",
//                false,
//                GetFactionArmoriesWeaponsHTML(id),
//                idString);
//        }

//        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponSave))]
//        public static async Task StaffFactionArmoryWeaponSave(MyPlayer player, string factionStorageItemIdString, string factionStorageIdString, string weapon,
//            int ammo, int quantity, int tintIndex, string componentsJSON)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
//            {
//                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
//                return;
//            }

//            if (quantity < 0)
//            {
//                player.EmitStaffShowMessage($"Estoque deve ser maior ou igual a 0.");
//                return;
//            }

//            if (!Enum.TryParse(weapon, true, out WeaponModel wep) || wep == 0)
//            {
//                player.EmitStaffShowMessage($"Arma {weapon} não existe.");
//                return;
//            }

//            if (ammo <= 0)
//            {
//                player.EmitStaffShowMessage($"Munição deve ser maior que 0.");
//                return;
//            }

//            if (tintIndex < 0 || tintIndex > 7)
//            {
//                player.EmitStaffShowMessage($"Pintura deve ser entre 0 e 7.");
//                return;
//            }

//            var realComponents = new List<uint>();
//            var components = Functions.Deserialize<List<string>>(componentsJSON);
//            foreach (var component in components)
//            {
//                var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.Equals(component, StringComparison.CurrentCultureIgnoreCase) && x.Weapon == wep);
//                if (comp == null)
//                {
//                    player.EmitStaffShowMessage($"Componente {component} não existe para a arma {wep}.");
//                    return;
//                }

//                if (realComponents.Contains(comp.Hash))
//                {
//                    player.EmitStaffShowMessage($"Componente {component} foi inserido na lista mais de uma vez.");
//                    return;
//                }

//                realComponents.Add(comp.Hash);
//            }

//            var factionArmoryWeapon = new FactionStorageItem();
//            if (factionArmoryWeaponId > 0)
//                factionArmoryWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == factionArmoryWeaponId);

//            factionArmoryWeapon.FactionArmoryId = factionArmoryId;
//            factionArmoryWeapon.Model = wep;
//            factionArmoryWeapon.Ammo = ammo;
//            factionArmoryWeapon.Quantity = quantity;
//            factionArmoryWeapon.TintIndex = Convert.ToByte(tintIndex);
//            factionArmoryWeapon.ComponentsJSON = Functions.Serialize(realComponents);

//            await using var context = new DatabaseContext();

//            if (factionArmoryWeapon.Id == 0)
//                await context.FactionStoragesItems.AddAsync(factionArmoryWeapon);
//            else
//                context.FactionStoragesItems.Update(factionArmoryWeapon);

//            await context.SaveChangesAsync();

//            if (factionArmoryWeaponId == 0)
//                Global.FactionsStoragesItems.Add(factionArmoryWeapon);

//            player.EmitStaffShowMessage($"Arma {(factionArmoryWeaponId == 0 ? "criada" : "editada")}.", true);

//            await player.GravarLog(LogType.Staff, $"Gravar Arma Arsenal | {Functions.Serialize(factionArmoryWeapon)}", null);

//            var html = GetFactionArmoriesWeaponsHTML(factionArmoryId);
//            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
//                target.Emit("StaffFactionsArmoriesWeapons", true, html);
//        }

//        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponRemove))]
//        public static async Task StaffFactionArmoryWeaponRemove(MyPlayer player, string idString)
//        {
//            if (!player.StaffFlags.Contains(StaffFlag.FactionsStorages))
//            {
//                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
//                return;
//            }

//            var id = new Guid(idString);
//            var factionArmoryWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == id);
//            if (factionArmoryWeapon == null)
//                return;

//            await using var context = new DatabaseContext();
//            context.FactionStoragesItems.Remove(factionArmoryWeapon);
//            await context.SaveChangesAsync();
//            Global.FactionsStoragesItems.Remove(factionArmoryWeapon);

//            player.EmitStaffShowMessage($"Arma do Arsenal {factionArmoryWeapon.Id} excluída.");

//            await player.GravarLog(LogType.Staff, $"Remover Arma Arsenal | {Functions.Serialize(factionArmoryWeapon)}", null);

//            var html = GetFactionArmoriesWeaponsHTML(factionArmoryWeapon.FactionStorageId);
//            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsStorages)))
//                target.Emit("StaffFactionsArmoriesWeapons", true, html);
//        }

//        private static string GetFactionsArmoriesHTML()
//        {
//            var html = string.Empty;
//            if (Global.FactionsStorages.Count == 0)
//            {
//                html = "<tr><td class='text-center' colspan='5'>Não há arsenais criados.</td></tr>";
//            }
//            else
//            {
//                foreach (var factionArmory in Global.FactionsStorages.OrderByDescending(x => x.Id))
//                    html += $@"<tr class='pesquisaitem'>
//                        <td>{factionArmory.Id}</td>
//                        <td>{factionArmory.FactionId}</td>
//                        <td>X: {factionArmory.PosX} | Y: {factionArmory.PosY} | Z: {factionArmory.PosZ}</td>
//                        <td>{factionArmory.Dimension}</td>
//                        <td class='text-center'>
//                            <input id='json{factionArmory.Id}' type='hidden' value='{Functions.Serialize(factionArmory)}' />
//                            <button onclick='goto({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
//                            <button onclick='edit({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
//                            <button onclick='editWeapons({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>ARMAS</button>
//                            <button onclick='remove(this, {factionArmory.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
//                        </td>
//                    </tr>";
//            }
//            return html;
//        }

//        private static string GetFactionArmoriesWeaponsHTML(Guid factionArmoryId)
//        {
//            var html = string.Empty;
//            var factionsArmoriesWeapons = Global.FactionsStoragesItems.Where(x => x.FactionStorageId == factionArmoryId);
//            if (!factionsArmoriesWeapons.Any())
//            {
//                html = "<tr><td class='text-center' colspan='8'>Não há armas criadas.</td></tr>";
//            }
//            else
//            {
//                var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == factionArmoryId);
//                var faction = Global.Factions.FirstOrDefault(x => x.Id == factionArmory.FactionId);

//                foreach (var factionArmoryWeapon in factionsArmoriesWeapons)
//                {
//                    var realComponents = new List<string>();
//                    foreach (var component in Functions.Deserialize<List<uint>>(factionArmoryWeapon.ComponentsJSON))
//                    {
//                        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Hash == component && x.Weapon.ToString() == factionArmoryWeapon.Model);
//                        if (comp != null)
//                            realComponents.Add(comp.Name);
//                    }

//                    html += $@"<tr class='pesquisaitem'>
//                        <td>{factionArmoryWeapon.Id}</td>
//                        <td>{factionArmoryWeapon.Model}</td>
//                        <td>{factionArmoryWeapon.Ammo}</td>
//                        <td>{factionArmoryWeapon.Quantity}</td>
//                        <td>{factionArmoryWeapon.TintIndex}</td>
//                        {(!faction.Government && !staff ? $"<td>{Global.Prices.FirstOrDefault(y => y.Type == PriceType.Weapons && y.Name.Equals(factionArmoryWeapon.Model.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0:N0}</td>" : string.Empty)}
//                        <td>{string.Join(", ", realComponents)}</td>
//                        <td class='text-center'>
//                            <input id='json{factionArmoryWeapon.Id}' type='hidden' value='{Functions.Serialize(new
//                    {
//                        Weapon = factionArmoryWeapon.Model.ToString(),
//                        factionArmoryWeapon.Ammo,
//                        factionArmoryWeapon.Quantity,
//                        factionArmoryWeapon.TintIndex,
//                        Components = realComponents,
//                    })}' />
//                            <button onclick='edit({factionArmoryWeapon.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
//                            <button onclick='remove(this, {factionArmoryWeapon.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
//                        </td>
//                    </tr>";
//                }
//            }
//            return html;
//        }
//    }
//}