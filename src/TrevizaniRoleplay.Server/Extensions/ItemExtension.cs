using AltV.Net;
using AltV.Net.Data;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class ItemExtension
    {
        public static void CreateObject(this Item item)
        {
            var myObject = (MyObject)Alt.CreateObject(
                item.GetObjectName(),
                new Position(item.PosX, item.PosY, item.PosZ),
                new Rotation(item.RotR, item.RotP, item.RotY));

            myObject.Frozen = true;
            myObject.Collision = false;
            myObject.Dimension = item.Dimension;

            UpdateGroundItems(item);
        }

        public static void DeleteObject(this Item item)
        {
            var myObject = Global.Objects.FirstOrDefault(x => x.ItemId == item.Id);
            myObject?.Destroy();

            var audioSpot = Global.AudioSpots.FirstOrDefault(x => x.ItemId == item.Id);
            audioSpot?.RemoveAllClients();

            Global.Items.Remove(item);
            UpdateGroundItems(item);
        }

        private static void UpdateGroundItems(this Item item)
        {
            var position = new Position(item.PosX, item.PosY, item.PosZ);
            foreach (var player in Global.SpawnedPlayers.Where(x => position.Distance(x.Position) <= Global.RP_DISTANCE && x.Dimension == item.Dimension))
                player.ShowInventory(player, update: true);
        }

        public static AudioSpot? GetAudioSpot(this Item item)
            => Global.AudioSpots.FirstOrDefault(x => x.ItemId == item.Id);
    }
}