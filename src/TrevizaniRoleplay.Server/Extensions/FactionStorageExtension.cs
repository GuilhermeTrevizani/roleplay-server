using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class FactionStorageExtension
    {
        public static void CreateIdentifier(this FactionStorage factionStorage)
        {
            RemoveIdentifier(factionStorage);

            var pos = new Vector3(factionStorage.PosX, factionStorage.PosY, factionStorage.PosZ - 0.95f);

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //marker.Scale = new Vector3(1, 1, 1.5f);
            //marker.Dimension = factionStorage.Dimension;
            //marker.FactionStorageId = factionStorage.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            colShape.Dimension = factionStorage.Dimension;
            colShape.Description = $"[ARMAZENAMENTO] {{#FFFFFF}}Use /farmazenamento.";
            colShape.FactionStorageId = factionStorage.Id;
        }

        public static void RemoveIdentifier(this FactionStorage factionStorage)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.FactionStorageId == factionStorage.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.FactionStorageId == factionStorage.Id);
            colShape?.Destroy();
        }
    }
}