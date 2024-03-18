using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class CrackDenExtension
    {
        public static void CreateIdentifier(this CrackDen crackDen)
        {
            RemoveIdentifier(crackDen);

            var pos = new Vector3(crackDen.PosX, crackDen.PosY, crackDen.PosZ - 0.95f);

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //marker.Scale = new Vector3(1, 1, 1.5f);
            //marker.Dimension = crackDen.Dimension;
            //marker.CrackDenId = crackDen.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            colShape.Dimension = crackDen.Dimension;
            colShape.Description = $"[BOCA DE FUMO] {{#FFFFFF}}Use /bocafumo.";
            colShape.CrackDenId = crackDen.Id;
        }

        public static void RemoveIdentifier(this CrackDen crackDen)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.CrackDenId == crackDen.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.CrackDenId == crackDen.Id);
            colShape?.Destroy();
        }
    }
}