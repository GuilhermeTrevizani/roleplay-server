using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffCrackDenScript : IScript
    {
        [Command("bocasfumo")]
        public static void CMD_bocasfumo(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffCrackDens", false, Functions.GetCrackDensHTML());
        }

        [ClientEvent(nameof(StaffCrackDenGoto))]
        public static void StaffCrackDenGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            player.LimparIPLs();

            if (crackDen.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Id == crackDen.Dimension).Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(crackDen.PosX, crackDen.PosY, crackDen.PosZ), crackDen.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRemove))]
        public static async Task StaffCrackDenRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            await using var context = new DatabaseContext();
            context.CrackDens.Remove(crackDen);
            context.CrackDensItems.RemoveRange(Global.CrackDensItems.Where(x => x.CrackDenId == id));
            await context.SaveChangesAsync();
            await context.CrackDensSells.Where(x => x.CrackDenId == id).ExecuteDeleteAsync();
            Global.CrackDens.Remove(crackDen);
            Global.CrackDensItems.RemoveAll(x => x.CrackDenId == crackDen.Id);
            crackDen.RemoveIdentifier();

            player.EmitStaffShowMessage($"Boca de fumo {crackDen.Id} excluída.");

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Boca de Fumo | {Functions.Serialize(crackDen)}", null);
        }

        [AsyncClientEvent(nameof(StaffCrackDenSave))]
        public static async Task StaffCrackDenSave(MyPlayer player, int id, Vector3 pos, int dimension,
            int onlinePoliceOfficers, int cooldownQuantityLimit, int cooldownHours)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (onlinePoliceOfficers < 0)
            {
                player.EmitStaffShowMessage($"Policiais Online deve ser maior ou igual a 0.");
                return;
            }

            if (cooldownQuantityLimit < 0)
            {
                player.EmitStaffShowMessage($"Quantidade Limite Cooldown deve ser maior ou igual a 0.");
                return;
            }

            if (cooldownHours < 0)
            {
                player.EmitStaffShowMessage($"Horas Cooldown deve ser maior ou igual a 0.");
                return;
            }

            var crackDen = new CrackDen();
            if (id > 0)
                crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);

            crackDen.PosX = pos.X;
            crackDen.PosY = pos.Y;
            crackDen.PosZ = pos.Z;
            crackDen.Dimension = dimension;
            crackDen.OnlinePoliceOfficers = onlinePoliceOfficers;
            crackDen.CooldownQuantityLimit = cooldownQuantityLimit;
            crackDen.CooldownHours = cooldownHours;

            await using var context = new DatabaseContext();

            if (crackDen.Id == 0)
                await context.CrackDens.AddAsync(crackDen);
            else
                context.CrackDens.Update(crackDen);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.CrackDens.Add(crackDen);

            crackDen.CreateIdentifier();

            player.EmitStaffShowMessage($"Boca de fumo {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Boca de Fumo | {Functions.Serialize(crackDen)}", null);

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }

        [ClientEvent(nameof(StaffCrackDensItemsShow))]
        public static void StaffCrackDensItemsShow(MyPlayer player, int crackDenId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffCrackDensItems",
                false,
                Functions.GetCrackDensItemsHTML(crackDenId, true),
                crackDenId);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemSave))]
        public static async Task StaffCrackDenItemSave(MyPlayer player,
            int crackDenItemId,
            int crackDenId,
            string strItemCategory,
            int value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (!Functions.CheckIfIsDrug(itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage($"Valor deve ser maior que 0.");
                return;
            }

            var crackDenItem = new CrackDenItem();
            if (crackDenItemId > 0)
                crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == crackDenItemId);

            crackDenItem.CrackDenId = crackDenId;
            crackDenItem.ItemCategory = itemCategory;
            crackDenItem.Value = value;

            await using var context = new DatabaseContext();

            if (crackDenItem.Id == 0)
                await context.CrackDensItems.AddAsync(crackDenItem);
            else
                context.CrackDensItems.Update(crackDenItem);

            await context.SaveChangesAsync();

            if (crackDenItemId == 0)
                Global.CrackDensItems.Add(crackDenItem);

            player.EmitStaffShowMessage($"Item {(crackDenItemId == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Item Boca de Fumo | {Functions.Serialize(crackDenItem)}", null);

            var html = Functions.GetCrackDensItemsHTML(crackDenId, true);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemRemove))]
        public static async Task StaffCrackDenItemRemove(MyPlayer player, int crackDenItemId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == crackDenItemId);
            if (crackDenItem == null)
                return;

            await using var context = new DatabaseContext();
            context.CrackDensItems.Remove(crackDenItem);
            await context.SaveChangesAsync();
            Global.CrackDensItems.Remove(crackDenItem);

            player.EmitStaffShowMessage($"Item da Boca de Fumo {crackDenItem.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Item Boca de Fumo | {Functions.Serialize(crackDenItem)}", null);

            var html = Functions.GetCrackDensItemsHTML(crackDenItem.CrackDenId, true);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRevokeCooldown))]
        public static async Task StaffCrackDenRevokeCooldown(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            crackDen.CooldownDate = DateTime.Now;

            await using var context = new DatabaseContext();
            context.CrackDens.Update(crackDen);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Cooldown da boca de fumo revogado.", true);

            await player.GravarLog(LogType.Staff, $"Revogar Cool Down Boca de Fumo | {id}", null);

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }
    }
}