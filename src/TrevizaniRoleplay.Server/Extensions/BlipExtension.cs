using AltV.Net;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class BlipExtension
    {
        public static void CreateIdentifier(this Blip blip)
        {
            RemoveIdentifier(blip);

            var myBlip = (MyBlip)Alt.CreateBlip(true, 4, new Position(blip.PosX, blip.PosY, blip.PosZ), Array.Empty<MyPlayer>());
            myBlip.Sprite = blip.Type;
            myBlip.Name = blip.Name;
            myBlip.Color = blip.Color;
            myBlip.ShortRange = true;
            myBlip.ScaleXY = new Vector2(0.8f, 0.8f);
            myBlip.Display = 2;
            myBlip.BlipId = blip.Id;
        }

        public static void RemoveIdentifier(this Blip blip)
        {
            var myBlip = Global.MyBlips.FirstOrDefault(x => x.BlipId == blip.Id);
            myBlip?.Destroy();
        }
    }
}