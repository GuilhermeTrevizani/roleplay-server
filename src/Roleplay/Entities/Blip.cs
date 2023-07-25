using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Factories;
using System;
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

            BlipAlt = Alt.CreateBlip(true, 4, new Position(PosX, PosY, PosZ), Array.Empty<MyPlayer>());
            BlipAlt.Sprite = Type;
            BlipAlt.Name = Name;
            BlipAlt.Color = Color;
            BlipAlt.ShortRange = true;
            BlipAlt.ScaleXY = new Vector2(0.8f, 0.8f);
            BlipAlt.Display = 2;
        }

        public void RemoveIdentifier()
        {
            BlipAlt?.Destroy();
            BlipAlt = null;
        }
    }
}