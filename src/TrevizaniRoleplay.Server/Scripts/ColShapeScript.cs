using AltV.Net;
using AltV.Net.Elements.Entities;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class ColShapeScript : IScript
    {
        [ScriptEvent(ScriptEventType.ColShape)]
        public static async void OnColShape(MyColShape colShape, IEntity targetEntity, bool state)
        {
            if (!state)
                return;

            if (targetEntity is not MyPlayer player)
                return;

            if (!string.IsNullOrWhiteSpace(colShape.Description))
            {
                if (colShape.InfoId.HasValue)
                {
                    var info = Global.Infos.FirstOrDefault(x => x.Id == colShape.InfoId);
                    if (info != null)
                    {
                        if (DateTime.Now > info.ExpirationDate)
                        {
                            await using var context = new DatabaseContext();
                            info.RemoveIdentifier();
                            Global.Infos.Remove(info);
                            context.Infos.Remove(info);
                            await context.SaveChangesAsync();
                            return;
                        }
                    }
                }

                player.SendMessage(MessageType.None, colShape.Description, Global.MAIN_COLOR);
                return;
            }

            if (!player.IsInVehicle)
                return;

            if (player.Vehicle.Driver != player)
                return;

            if (colShape.PoliceOfficerCharacterId.HasValue && colShape.MaxSpeed.HasValue)
            {
                if (player.Vehicle is not MyVehicle veh)
                    return;

                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == colShape.PoliceOfficerCharacterId);
                target?.SendMessage(MessageType.None, $"[RADAR] {{{(veh.Speed > colShape.MaxSpeed ? Global.ERROR_COLOR : Global.SUCCESS_COLOR)}}}{veh.Speed} {{#FFFFFF}}km/h.");
            }
        }
    }
}