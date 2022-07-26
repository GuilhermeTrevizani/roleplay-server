using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Blip
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public ushort Type { get; set; }

        public byte Color { get; set; }

        [NotMapped, JsonIgnore]
        public IBlip BlipAlt { get; set; }

        public void CreateIdentifier()
        {
            RemoveIdentifier();

            BlipAlt = Alt.CreateBlip(4, new Position(PosX, PosY, PosZ));
            BlipAlt.Sprite = Type;
            BlipAlt.Name = Name;
            BlipAlt.Color = Color;
            BlipAlt.ShortRange = true;
            BlipAlt.ScaleXY = new Vector2(0.8f, 0.8f);
            BlipAlt.Display = 2;
        }

        public void RemoveIdentifier()
        {
            BlipAlt?.Remove();
            BlipAlt = null;
        }
    }
}