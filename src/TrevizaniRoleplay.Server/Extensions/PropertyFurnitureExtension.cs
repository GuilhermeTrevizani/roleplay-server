using AltV.Net;
using AltV.Net.Data;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class PropertyFurnitureExtension
    {
        public static string GetModelName(this PropertyFurniture propertyFurniture)
            => Global.Furnitures.FirstOrDefault(x => x.Model.ToLower() == propertyFurniture.Model.ToLower())?.Name ?? string.Empty;

        public static void CreateObject(this PropertyFurniture propertyFurniture)
        {
            var myObject = (MyObject)Alt.CreateObject(
                propertyFurniture.Model,
                new Position(propertyFurniture.PosX, propertyFurniture.PosY, propertyFurniture.PosZ),
                new Rotation(propertyFurniture.RotR, propertyFurniture.RotP, propertyFurniture.RotY));

            myObject.Dimension = propertyFurniture.Interior ? propertyFurniture.Property!.Number : 0;
            myObject.Frozen = true;
            myObject.Collision = true;
            myObject.Collision = true;
            myObject.PropertyFurnitureId = propertyFurniture.Id;
        }

        public static void DeleteObject(this PropertyFurniture propertyFurniture)
        {
            var myObject = Global.Objects.FirstOrDefault(x => x.PropertyFurnitureId == propertyFurniture.Id);
            myObject?.Destroy();
        }
    }
}