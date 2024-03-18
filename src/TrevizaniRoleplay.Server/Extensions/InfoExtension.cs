using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class InfoExtension
    {
        public static void CreateIdentifier(this Info info)
        {
            RemoveIdentifier(info);

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, new Vector3(info.PosX, info.PosY, info.PosZ), new Rgba(227, 170, 36, 75));
            //marker.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            //marker.Dimension = info.Dimension;
            //marker.InfoId = info.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(new Vector3(info.PosX, info.PosY, info.PosZ), 1, 1.5f);
            colShape.Description = $"[INFO] {{#FFFFFF}}{info.Message}";
            colShape.Dimension = info.Dimension;
            colShape.InfoId = info.Id;
        }

        public static void RemoveIdentifier(this Info info)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.InfoId == info.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.InfoId == info.Id);
            colShape?.Destroy();
        }
    }
}