using AltV.Net;
using AltV.Net.Async;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffFurnitureScript : IScript
    {
        [Command("amobilias")]
        public static void CMD_amobilias(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFurnitures", false, GetFurnituresHTML());
        }

        [AsyncClientEvent(nameof(StaffFurnitureSave))]
        public static async Task StaffFurnitureSave(MyPlayer player, string idString, string category, string name, string model, int value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!category.Equals("barreiras", StringComparison.CurrentCultureIgnoreCase) && value <= 0)
            {
                player.EmitStaffShowMessage("Valor inválido.");
                return;
            }

            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            var furniture = new Furniture();
            if (isNew)
            {
                furniture.Create(category, name, model, value);
            }
            else
            {
                furniture = Global.Furnitures.FirstOrDefault(x => x.Id == id);
                if (furniture == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                furniture.Update(category, name, model, value);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.Furnitures.AddAsync(furniture);
            else
                context.Furnitures.Update(furniture);

            await context.SaveChangesAsync();

            if (isNew)
                Global.Furnitures.Add(furniture);

            player.EmitStaffShowMessage($"Mobília {(isNew ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Mobília | {Functions.Serialize(furniture)}", null);

            var html = GetFurnituresHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Furnitures)))
                target.Emit("StaffFurnitures", true, html);
        }

        [AsyncClientEvent(nameof(StaffFurnitureRemove))]
        public static async Task StaffFurnitureRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var furniture = Global.Furnitures.FirstOrDefault(x => x.Id == id);
            if (furniture != null)
            {
                await using var context = new DatabaseContext();
                context.Furnitures.Remove(furniture);
                await context.SaveChangesAsync();
                Global.Furnitures.Remove(furniture);
                await player.GravarLog(LogType.Staff, $"Remover Mobília | {Functions.Serialize(furniture)}", null);
            }

            player.EmitStaffShowMessage($"Mobília {id} excluída.");

            var html = GetFurnituresHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Furnitures)))
                target.Emit("StaffFurnitures", true, html);
        }

        private static string GetFurnituresHTML()
        {
            var html = string.Empty;
            if (Global.Furnitures.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='6'>Não há mobílias criadas.</td></tr>";
            }
            else
            {
                foreach (var furniture in Global.Furnitures.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{furniture.Id}</td>
                        <td>{furniture.Category}</td>
                        <td>{furniture.Name}</td>
                        <td>{furniture.Model}</td>
                        <td>${furniture.Value:N0}</td>
                        <td class='text-center'>
                            <input id='json{furniture.Id}' type='hidden' value='{Functions.Serialize(furniture)}' />
                            <button onclick='edit({furniture.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {furniture.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}