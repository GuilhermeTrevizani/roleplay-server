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
    public class PropertyFurnitureScript : IScript
    {
        [Command("mobilias")]
        public static void CMD_mobilias(MyPlayer player)
        {
            var prop = Global.Properties.FirstOrDefault(x => x.Number == player.Dimension);
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está no interior de uma propriedade.");
                return;
            }

            if (!prop.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, "Você não possui acesso a esta propriedade.");
                return;
            }

            player.Emit("PropertyFurnitures", prop.Id, GetFurnituresHTML(player, prop));
        }

        private static string GetFurnituresHTML(MyPlayer player, Property property)
        {
            var html = string.Empty;
            if (property.Furnitures!.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='7'>Não há mobílias na propriedade.</td></tr>";
            }
            else
            {
                foreach (var furniture in property.Furnitures.OrderBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ))))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{furniture.Id}</td>
                        <td>{furniture.GetModelName()}</td>
                        <td>X: {furniture.PosX} | Y: {furniture.PosY} | Z: {furniture.PosZ}</td>
                        <td>R: {furniture.RotR} | P: {furniture.RotP} | Y: {furniture.RotY}</td>
                        <td>{player.Position.Distance(new Position(furniture.PosX, furniture.PosY, furniture.PosZ))}</td>
                        <td class='text-center'>{(furniture.Interior ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <button onclick='edit(this, {furniture.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {furniture.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        [ClientEvent(nameof(BuyPropertyFurniture))]
        public static void BuyPropertyFurniture(MyPlayer player, string propertyId)
        {
            player.Emit("Server:CloseView");

            var furnitures = Global.Furnitures.Where(x => !x.Category.Equals("barreiras", StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();

            var html = $@"<div class='row'><div class='col-md-3'><select class='form-control' id='sel-category'><option value='Todas' selected>Todas</option>{string.Join("", furnitures.GroupBy(x => x.Category).Select(x => $"<option value='{x.Key}'>{x.Key}</option>").ToList())}</select></div><div class='col-md-9'><input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise as mobílias...' /></div></div><br/>
            <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:hidden;'>
                <table class='table table-bordered table-striped'>
                <thead>
                    <tr class='bg-dark'>
                        <th>Categoria</th>
                        <th>Nome</th>
                        <th>Objeto</th>
                        <th>Valor</th>
                        <th class='text-center'>Opções</th>
                    </tr>
                </thead>
                <tbody>";

            if (furnitures.Count == 0)
            {
                html += "<tr><td class='text-center' colspan='5'>Não há mobílias criadas.</td></tr>";
            }
            else
            {
                foreach (var furniture in furnitures)
                    html += $@"<tr class='pesquisaitem' data-category='{furniture.Category}'>
                        <td>{furniture.Category}</td>
                        <td>{furniture.Name}</td>
                        <td>{furniture.Model}</td>
                        <td>${furniture.Value:N0}</td>
                        <td class='text-center'>
                            <button onclick='buy(this, {furniture.Id})' type='button' class='btn btn-dark btn-sm'>COMPRAR</button>
                        </td>
                    </tr>";
            }

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("BuyPropertyFurnitures", propertyId, html);
        }

        [ClientEvent(nameof(SelectBuyPropertyFurniture))]
        public static void SelectBuyPropertyFurniture(MyPlayer player, string propertyIdString, string furnitureIdString)
        {
            var propertyId = new Guid(propertyIdString);
            var property = Global.Properties.FirstOrDefault(x => x.Id == propertyId);
            if (property == null)
                return;

            var furnitureId = new Guid(furnitureIdString);
            var furniture = Global.Furnitures.FirstOrDefault(x => x.Id == furnitureId);
            if (furniture == null)
                return;

            player.DropPropertyFurniture = new PropertyFurniture();
            player.DropPropertyFurniture.Create(property.Id, furniture.Model, player.Dimension != 0);
            player.Emit("DropObject", player.DropPropertyFurniture.Model, 2);
        }

        [ClientEvent(nameof(EditPropertyFurniture))]
        public static void EditPropertyFurniture(MyPlayer player, string propertyIdString, string propertyFurnitureIdString)
        {
            var propertyId = new Guid(propertyIdString);
            var property = Global.Properties.FirstOrDefault(x => x.Id == propertyId);
            if (property == null)
                return;

            var propertyFurnitureId = new Guid(propertyFurnitureIdString);
            player.DropPropertyFurniture = property.Furnitures!.FirstOrDefault(x => x.Id == propertyFurnitureId);
            if (player.DropPropertyFurniture == null)
                return;

            player.DropPropertyFurniture.DeleteObject();
            player.Emit("DropObject", player.DropPropertyFurniture.Model, 2);
        }

        [AsyncClientEvent(nameof(RemovePropertyFurniture))]
        public static async Task RemovePropertyFurniture(MyPlayer player, string propertyIdString, string propertyFurnitureIdString)
        {
            var propertyId = new Guid(propertyIdString);
            var property = Global.Properties.FirstOrDefault(x => x.Id == propertyId);
            if (property == null)
                return;

            var propertyFurnitureId = new Guid(propertyFurnitureIdString);
            var propertyFurniture = property.Furnitures!.FirstOrDefault(x => x.Id == propertyFurnitureId);
            if (propertyFurniture == null)
                return;

            await using var context = new DatabaseContext();
            context.PropertiesFurnitures.Remove(propertyFurniture);
            await context.SaveChangesAsync();

            property.Furnitures!.Remove(propertyFurniture);
            propertyFurniture.DeleteObject();

            player.SendMessage(MessageType.Success, $"Você removeu a mobília {propertyFurniture.Id}.", notify: true);
            player.Emit("PropertyFurnitures", property.Id, GetFurnituresHTML(player, property));
        }

        [ClientEvent(nameof(CancelDropFurniture))]
        public static void CancelDropFurniture(MyPlayer player)
        {
            if (player.DropPropertyFurniture == null)
                return;

            var property = Global.Properties.FirstOrDefault(x => x.Id == player.DropPropertyFurniture.PropertyId);
            if (property == null)
                return;

            player.SendMessage(MessageType.Success, "Você cancelou o drop da mobília.", notify: true);

            if (player.DropPropertyFurniture.PosX > 0)
            {
                player.DropPropertyFurniture.CreateObject();
                player.Emit("PropertyFurnitures", property.Id, GetFurnituresHTML(player, property));
            }
            else
            {
                BuyPropertyFurniture(player, player.DropPropertyFurniture.PropertyId.ToString());
            }

            player.DropPropertyFurniture = null;
        }

        [AsyncClientEvent(nameof(ConfirmDropFurniture))]
        public static async Task ConfirmDropFurniture(MyPlayer player, Vector3 position, Vector3 rotation)
        {
            if (player.DropPropertyFurniture == null)
                return;

            if (position.X == 0 || position.Y == 0 || position.Z == 0)
            {
                player.SendMessage(MessageType.Error, "Não foi possível recuperar a posição do item.");
                return;
            }

            var property = Global.Properties.FirstOrDefault(x => x.Id == player.DropPropertyFurniture.PropertyId);
            if (property == null)
                return;

            var furniture = Global.Furnitures.FirstOrDefault(x => x.Model == player.DropPropertyFurniture.Model && x.Value > 0);
            if (furniture == null)
                return;

            var newFurniture = false;
            if (player.DropPropertyFurniture.PosX == 0)
            {
                newFurniture = true;

                var maxFurnitures = 100;
                if ((player.User.VIPValidDate ?? DateTime.MinValue) >= DateTime.Now)
                {
                    if (player.User.VIP == UserVIP.Gold)
                        maxFurnitures = 250;
                    else if (player.User.VIP == UserVIP.Silver)
                        maxFurnitures = 200;
                    else if (player.User.VIP == UserVIP.Bronze)
                        maxFurnitures = 150;
                }

                if (property.Furnitures!.Count == maxFurnitures)
                {
                    player.SendMessage(MessageType.Error, $"O limite de {maxFurnitures} mobílias da propriedade foi atingido.", notify: true);
                    return;
                }

                if (player.Money < furniture.Value)
                {
                    player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, furniture.Value), notify: true);
                    return;
                }

                await player.RemoveStackedItem(ItemCategory.Money, furniture.Value);
            }

            player.DropPropertyFurniture.SetPosition(position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z);

            await using var context = new DatabaseContext();

            if (newFurniture)
                await context.PropertiesFurnitures.AddAsync(player.DropPropertyFurniture);
            else
                context.PropertiesFurnitures.Update(player.DropPropertyFurniture);

            await context.SaveChangesAsync();

            if (!property.Furnitures!.Contains(player.DropPropertyFurniture))
                property.Furnitures.Add(player.DropPropertyFurniture);

            player.DropPropertyFurniture.CreateObject();

            await player.GravarLog(LogType.EditPropertyFurniture, Functions.Serialize(player.DropPropertyFurniture), null);

            if (newFurniture)
            {
                player.SendMessage(MessageType.Success, $"Você comprou {furniture.Name} por ${furniture.Value:N0}.", notify: true);
                BuyPropertyFurniture(player, player.DropPropertyFurniture.PropertyId.ToString());
            }
            else
            {
                player.SendMessage(MessageType.Success, $"Você editou a posição de {furniture.Name}.", notify: true);
                player.Emit("PropertyFurnitures", property.Id, GetFurnituresHTML(player, property));
            }

            player.DropPropertyFurniture = null;
        }
    }
}