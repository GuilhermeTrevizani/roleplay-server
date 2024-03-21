using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
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

            player.Emit("StaffCrackDens", false, GetCrackDensHTML());
        }

        [ClientEvent(nameof(StaffCrackDenGoto))]
        public static void StaffCrackDenGoto(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            player.LimparIPLs();

            if (crackDen.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Number == crackDen.Dimension)!.Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(crackDen.PosX, crackDen.PosY, crackDen.PosZ), crackDen.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRemove))]
        public static async Task StaffCrackDenRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
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

            var html = GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Boca de Fumo | {Functions.Serialize(crackDen)}", null);
        }

        [AsyncClientEvent(nameof(StaffCrackDenSave))]
        public static async Task StaffCrackDenSave(MyPlayer player, string idString, Vector3 pos, int dimension,
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

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            var crackDen = new CrackDen();
            if (isNew)
            {
                crackDen.Create(pos.X, pos.Y, pos.Z, dimension, onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours);
            }
            else
            {
                crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
                if (crackDen == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                crackDen.Update(pos.X, pos.Y, pos.Z, dimension, onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.CrackDens.AddAsync(crackDen);
            else
                context.CrackDens.Update(crackDen);

            await context.SaveChangesAsync();

            if (isNew)
                Global.CrackDens.Add(crackDen);

            crackDen.CreateIdentifier();

            player.EmitStaffShowMessage($"Boca de fumo {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Boca de Fumo | {Functions.Serialize(crackDen)}", null);

            var html = GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }

        [ClientEvent(nameof(StaffCrackDensItemsShow))]
        public static void StaffCrackDensItemsShow(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            var id = idString.ToGuid();
            player.Emit("StaffCrackDensItems",
                false,
                GetCrackDensItemsHTML(id.Value),
                idString);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemSave))]
        public static async Task StaffCrackDenItemSave(MyPlayer player,
            string crackDenItemIdString,
            string crackDenIdString,
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

            var crackDenId = new Guid(crackDenIdString);
            var crackDenItemId = new Guid(crackDenItemIdString);
            var isNew = string.IsNullOrWhiteSpace(crackDenItemIdString);
            var crackDenItem = new CrackDenItem();
            if (isNew)
            {
                crackDenItem.Create(crackDenId, itemCategory, value);
            }
            else
            {
                crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == crackDenItemId);
                if (crackDenItem == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                crackDenItem.Update(crackDenId, itemCategory, value);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.CrackDensItems.AddAsync(crackDenItem);
            else
                context.CrackDensItems.Update(crackDenItem);

            await context.SaveChangesAsync();

            if (isNew)
                Global.CrackDensItems.Add(crackDenItem);

            player.EmitStaffShowMessage($"Item {(isNew ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Item Boca de Fumo | {Functions.Serialize(crackDenItem)}", null);

            var html = GetCrackDensItemsHTML(crackDenId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemRemove))]
        public static async Task StaffCrackDenItemRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == id);
            if (crackDenItem == null)
                return;

            await using var context = new DatabaseContext();
            context.CrackDensItems.Remove(crackDenItem);
            await context.SaveChangesAsync();
            Global.CrackDensItems.Remove(crackDenItem);

            player.EmitStaffShowMessage($"Item da Boca de Fumo {crackDenItem.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Item Boca de Fumo | {Functions.Serialize(crackDenItem)}", null);

            var html = GetCrackDensItemsHTML(crackDenItem.CrackDenId);
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRevokeCooldown))]
        public static async Task StaffCrackDenRevokeCooldown(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            crackDen.ResetCooldownDate();

            await using var context = new DatabaseContext();
            context.CrackDens.Update(crackDen);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Cooldown da boca de fumo revogado.", true);

            await player.GravarLog(LogType.Staff, $"Revogar Cool Down Boca de Fumo | {id}", null);

            var html = GetCrackDensHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }

        private static string GetCrackDensHTML()
        {
            var html = string.Empty;
            if (Global.CrackDens.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='9'>Não há bocas de fumo criadas.</td></tr>";
            }
            else
            {
                foreach (var crackDen in Global.CrackDens.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{crackDen.Id}</td>
                        <td>X: {crackDen.PosX} | Y: {crackDen.PosY} | Z: {crackDen.PosZ}</td>
                        <td>{crackDen.Dimension}</td>
                        <td>{crackDen.OnlinePoliceOfficers}</td>
                        <td>{crackDen.CooldownQuantityLimit}</td>
                        <td>{crackDen.CooldownHours}</td>
                        <td>{crackDen.CooldownDate}</td>
                        <td>{crackDen.Quantity}</td>
                        <td class='text-center'>
                            <input id='json{crackDen.Id}' type='hidden' value='{Functions.Serialize(crackDen)}' />
                            <button onclick='goto({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editItems({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>ITENS</button>
                            <button onclick='revokeCooldown({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>REVOGAR COOLDOWN</button>
                            <button onclick='remove(this, {crackDen.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
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
                            <input id='json{item.Id}' type='hidden' value='{Functions.Serialize(new
                    {
                        ItemCategory = item.ItemCategory.ToString(),
                        item.Value
                    })}' />
                            <button onclick='edit({item.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {item.Id})'   type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
    }
}