using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class TruckerLocationExtension
    {
        public static List<string> GetAllowedVehicles(this TruckerLocation truckerLocation)
            => Functions.Deserialize<List<string>>(truckerLocation.AllowedVehiclesJSON);

        public static void CreateIdentifier(this TruckerLocation truckerLocation)
        {
            RemoveIdentifier(truckerLocation);

            var pos = new Vector3(truckerLocation.PosX, truckerLocation.PosY, truckerLocation.PosZ - 0.95f);

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //marker.Scale = new Vector3(1, 1, 1.5f);
            //marker.TruckerLocationId = truckerLocation.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            colShape.Description = $"[{truckerLocation.Name}] {{#FFFFFF}}Use /carregarcaixas ou /cancelarcaixas.";
            colShape.TruckerLocationId = truckerLocation.Id;
        }

        public static void RemoveIdentifier(this TruckerLocation truckerLocation)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.TruckerLocationId == truckerLocation.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.TruckerLocationId == truckerLocation.Id);
            colShape?.Destroy();
        }
    }
}