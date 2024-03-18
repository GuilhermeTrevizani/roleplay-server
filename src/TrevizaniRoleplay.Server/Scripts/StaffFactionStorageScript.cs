using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
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
        [Command("arsenais")]
        public static void CMD_arsenais(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFactionsArmories", false, Functions.GetFactionsArmoriesHTML());
        }

        [ClientEvent(nameof(StaffFactionArmoryGoto))]
        public static void StaffFactionArmoryGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
            if (factionArmory == null)
                return;

            player.LimparIPLs();

            if (factionArmory.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Id == factionArmory.Dimension).Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(factionArmory.PosX, factionArmory.PosY, factionArmory.PosZ), factionArmory.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryRemove))]
        public static async Task StaffFactionArmoryRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);
            if (factionArmory == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionStorages.Remove(factionArmory);
            context.FactionStoragesItems.RemoveRange(Global.FactionsStoragesItems.Where(x => x.FactionStorageId == id));
            await context.SaveChangesAsync();
            Global.FactionsStorages.Remove(factionArmory);
            Global.FactionsStoragesItems.RemoveAll(x => x.FactionStorageId == factionArmory.Id);
            factionArmory.RemoveIdentifier();

            player.EmitStaffShowMessage($"Arsenal {factionArmory.Id} excluído.");

            var html = Functions.GetFactionsArmoriesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmories", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Arsenal | {Functions.Serialize(factionArmory)}", null);
        }

        [AsyncClientEvent(nameof(StaffFactionArmorySave))]
        public static async Task StaffFactionArmorySave(MyPlayer player, int id, int factionId, Vector3 pos, int dimension)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (factionId != 0 && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            var factionArmory = new FactionStorage();
            if (id > 0)
                factionArmory = Global.FactionsStorages.FirstOrDefault(x => x.Id == id);

            factionArmory.FactionId = factionId;
            factionArmory.PosX = pos.X;
            factionArmory.PosY = pos.Y;
            factionArmory.PosZ = pos.Z;
            factionArmory.Dimension = dimension;

            await using var context = new DatabaseContext();

            if (factionArmory.Id == 0)
                await context.FactionStorages.AddAsync(factionArmory);
            else
                context.FactionStorages.Update(factionArmory);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.FactionsStorages.Add(factionArmory);

            factionArmory.CreateIdentifier();

            player.EmitStaffShowMessage($"Arsenal {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Arsenal | {Functions.Serialize(factionArmory)}", null);

            var html = Functions.GetFactionsArmoriesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmories", true, html);
        }

        [ClientEvent(nameof(StaffFactionsArmorysWeaponsShow))]
        public static void StaffFactionsArmorysWeaponsShow(MyPlayer player, int factionArmoryId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffFactionsArmoriesWeapons",
                false,
                Functions.GetFactionArmoriesWeaponsHTML(factionArmoryId, true),
                factionArmoryId);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponSave))]
        public static async Task StaffFactionArmoryWeaponSave(MyPlayer player, int factionArmoryWeaponId, int factionArmoryId, string weapon,
            int ammo, int quantity, int tintIndex, string componentsJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (quantity < 0)
            {
                player.EmitStaffShowMessage($"Estoque deve ser maior ou igual a 0.");
                return;
            }

            if (!Enum.TryParse(weapon, true, out WeaponModel wep) || wep == 0)
            {
                player.EmitStaffShowMessage($"Arma {weapon} não existe.");
                return;
            }

            if (ammo <= 0)
            {
                player.EmitStaffShowMessage($"Munição deve ser maior que 0.");
                return;
            }

            if (tintIndex < 0 || tintIndex > 7)
            {
                player.EmitStaffShowMessage($"Pintura deve ser entre 0 e 7.");
                return;
            }

            var realComponents = new List<uint>();
            var components = Functions.Deserialize<List<string>>(componentsJSON);
            foreach (var component in components)
            {
                var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.Equals(component, StringComparison.CurrentCultureIgnoreCase) && x.Weapon == wep);
                if (comp == null)
                {
                    player.EmitStaffShowMessage($"Componente {component} não existe para a arma {wep}.");
                    return;
                }

                if (realComponents.Contains(comp.Hash))
                {
                    player.EmitStaffShowMessage($"Componente {component} foi inserido na lista mais de uma vez.");
                    return;
                }

                realComponents.Add(comp.Hash);
            }

            var factionArmoryWeapon = new FactionStorageItem();
            if (factionArmoryWeaponId > 0)
                factionArmoryWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == factionArmoryWeaponId);

            factionArmoryWeapon.FactionArmoryId = factionArmoryId;
            factionArmoryWeapon.Model = wep;
            factionArmoryWeapon.Ammo = ammo;
            factionArmoryWeapon.Quantity = quantity;
            factionArmoryWeapon.TintIndex = Convert.ToByte(tintIndex);
            factionArmoryWeapon.ComponentsJSON = Functions.Serialize(realComponents);

            await using var context = new DatabaseContext();

            if (factionArmoryWeapon.Id == 0)
                await context.FactionStoragesItems.AddAsync(factionArmoryWeapon);
            else
                context.FactionStoragesItems.Update(factionArmoryWeapon);

            await context.SaveChangesAsync();

            if (factionArmoryWeaponId == 0)
                Global.FactionsStoragesItems.Add(factionArmoryWeapon);

            player.EmitStaffShowMessage($"Arma {(factionArmoryWeaponId == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Arma Arsenal | {Functions.Serialize(factionArmoryWeapon)}", null);

            var html = Functions.GetFactionArmoriesWeaponsHTML(factionArmoryId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmoriesWeapons", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponRemove))]
        public static async Task StaffFactionArmoryWeaponRemove(MyPlayer player, int factionArmoryWeaponId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmoryWeapon = Global.FactionsStoragesItems.FirstOrDefault(x => x.Id == factionArmoryWeaponId);
            if (factionArmoryWeapon == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionStoragesItems.Remove(factionArmoryWeapon);
            await context.SaveChangesAsync();
            Global.FactionsStoragesItems.Remove(factionArmoryWeapon);

            player.EmitStaffShowMessage($"Arma do Arsenal {factionArmoryWeapon.Id} excluída.");

            await player.GravarLog(LogType.Staff, $"Remover Arma Arsenal | {Functions.Serialize(factionArmoryWeapon)}", null);

            var html = Functions.GetFactionArmoriesWeaponsHTML(factionArmoryWeapon.FactionArmoryId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmoriesWeapons", true, html);
        }
    }
}