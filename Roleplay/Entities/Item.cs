using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Item : BaseItem
    {
        public Item() : base() { }

        public Item(ItemCategory category, uint type = 0) : base(category, type) { }

        public Item(Item baseItem) : base(baseItem)
        {
            Dimension = baseItem.Dimension;
            PosX = baseItem.PosX;
            PosY = baseItem.PosY;
            PosZ = baseItem.PosZ;
            RotR = baseItem.RotR;
            RotP = baseItem.RotP;
            RotY = baseItem.RotY;
        }

        public int Dimension { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public float RotR { get; set; }

        public float RotP { get; set; }

        public float RotY { get; set; }

        [NotMapped, JsonIgnore]
        public MyObject Object { get; set; }

        [NotMapped, JsonIgnore]
        public AudioSpot AudioSpot { get; set; }

        public void CreateObject()
        {
            Object = (MyObject) Alt.CreateNetworkObject(
                ObjectName,
                new Position(PosX, PosY, PosZ),
                new Rotation(RotR, RotP, RotY));

            Object.Frozen = true;
            Object.Collision = false;
            Object.Dimension = Dimension;

            UpdateGroundItems();
        }

        public void DeleteObject()
        {
            Global.Items.Remove(this);
            Object.Destroy();
            AudioSpot?.RemoveAllClients();
            UpdateGroundItems();
        }

        private void UpdateGroundItems()
        {
            var position = new Position(PosX, PosY, PosZ);
            foreach (var player in Global.Players.Where(x => position.Distance(x.Position) <= Global.RP_DISTANCE && x.Dimension == Dimension))
                player.ShowInventory(player, update: true);
        }
    }
}