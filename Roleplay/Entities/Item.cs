using AltV.Net.Elements.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
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
        public IObject Object { get; set; }

        [NotMapped, JsonIgnore]
        public AudioSpot AudioSpot { get; set; }

        [Obsolete("TODO")]
        public void CreateObject(MyPlayer player)
        {
            //Object = PropStreamer.Create(
            //    ObjectName,
            //    new Vector3(PosX, PosY, PosZ),
            //    new Vector3(RotR, RotP, RotY),
            //    Dimension,
            //    true,
            //    frozen: true,
            //    collision: false,
            //    streamRange: 25);

            if (player != null)
                UpdateGroundItems(player);
        }

        public void DeleteObject(MyPlayer player)
        {
            Global.Items.Remove(this);
            Object.Destroy();
            AudioSpot?.RemoveAllClients();
            UpdateGroundItems(player);
        }

        private static void UpdateGroundItems(MyPlayer player)
        {
            foreach (var x in Global.Players.Where(x => player.Position.Distance(x.Position) <= Global.RP_DISTANCE && x.Dimension == player.Dimension))
                x.ShowInventory(x, update: true);
        }
    }
}