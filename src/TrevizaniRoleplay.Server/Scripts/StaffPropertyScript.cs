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
    public class StaffPropertyScript : IScript
    {
        [Command("propriedades")]
        public static void CMD_propriedades(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonInteriors = Functions.Serialize(
                Enum.GetValues(typeof(PropertyInterior))
                .Cast<PropertyInterior>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("StaffProperties", false, Functions.GetPropertiesHTML(), jsonInteriors);
        }

        [Command("int", "/int (tipo)")]
        public static async Task CMD_int(MyPlayer player, byte type)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PropertyInterior), type))
            {
                player.SendMessage(MessageType.Error, "Tipo inválido.");
                return;
            }

            player.IPLs = Functions.GetIPLsByInterior((PropertyInterior)type);
            player.SetarIPLs();
            player.SetPosition(Functions.GetExitPositionByInterior((PropertyInterior)type), 0, false);
            await player.GravarLog(LogType.Staff, $"/int {type}", null);
        }

        [ClientEvent(nameof(StaffPropertyGoto))]
        public static void StaffPropertyGoto(MyPlayer player, int id)
        {
            var property = Global.Properties.FirstOrDefault(x => x.Id == id);
            if (property == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(property.EntrancePosX, property.EntrancePosY, property.EntrancePosZ), property.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffPropertySave))]
        public static async Task StaffPropertySave(MyPlayer player, int id, int interior, int value, int dimension, Vector3 pos, string address)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PropertyInterior), Convert.ToByte(interior)))
            {
                player.EmitStaffShowMessage("Interior inválido.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage("Valor deve ser maior que 0.");
                return;
            }

            var property = new Property();
            if (id > 0)
                property = Global.Properties.FirstOrDefault(x => x.Id == id);
            else
                property.LockNumber = Global.Properties.Select(x => x.LockNumber).DefaultIfEmpty(0u).Max() + 1;

            var propertyInterior = (PropertyInterior)Convert.ToByte(interior);
            var exit = Functions.GetExitPositionByInterior(propertyInterior);

            property.Interior = propertyInterior;
            property.EntrancePosX = pos.X;
            property.EntrancePosY = pos.Y;
            property.EntrancePosZ = pos.Z;
            property.Value = value;
            property.ExitPosX = exit.X;
            property.ExitPosY = exit.Y;
            property.ExitPosZ = exit.Z;
            property.Dimension = dimension;
            property.Address = address;

            await using var context = new DatabaseContext();

            if (property.Id == 0)
                await context.Properties.AddAsync(property);
            else
                context.Properties.Update(property);

            await context.SaveChangesAsync();

            property.CreateIdentifier();

            if (id == 0)
            {
                property.Items = [];
                Global.Properties.Add(property);
            }

            player.EmitStaffShowMessage($"Propriedade {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Propriedade | {Functions.Serialize(property)}", null);

            var html = Functions.GetPropertiesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Properties)))
                target.Emit("StaffProperties", true, html);
        }

        [AsyncClientEvent(nameof(StaffPropertyRemove))]
        public static async Task StaffPropertyRemove(MyPlayer player, int id)
        {
            try
            {
                if (!player.StaffFlags.Contains(StaffFlag.Properties))
                {
                    player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var property = Global.Properties.FirstOrDefault(x => x.Id == id);
                if (property != null)
                {
                    if (property.CharacterId > 0)
                    {
                        player.EmitStaffShowMessage($"Propriedade {id} possui um dono.");
                        return;
                    }

                    await using var context = new DatabaseContext();
                    context.Properties.Remove(property);
                    await context.SaveChangesAsync();

                    if (property.Items.Count != 0)
                    {
                        foreach (var propertyItem in property.Items)
                            context.PropertiesItems.Remove(propertyItem);
                        await context.SaveChangesAsync();
                    }

                    Global.Properties.Remove(property);
                    property.RemoveIdentifier();
                    await player.GravarLog(LogType.Staff, $"Remover Propriedade | {Functions.Serialize(property)}", null);
                }

                player.EmitStaffShowMessage($"Propriedade {id} excluída.");

                var html = Functions.GetPropertiesHTML();
                foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Properties)))
                    target.Emit("StaffProperties", true, html);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }
    }
}