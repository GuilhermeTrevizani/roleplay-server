using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared.Enums;
using Roleplay.Factories;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Info
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public int Dimension { get; set; }

        public DateTime ExpirationDate { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public string Message { get; set; }

        [JsonIgnore, NotMapped]
        public IMarker Marker { get; set; }

        [JsonIgnore, NotMapped]
        public MyColShape ColShape { get; set; }

        public void CreateIdentifier()
        {
            RemoveIdentifier();

            Marker = Alt.CreateMarker(MarkerType.MarkerHalo, new Vector3(PosX, PosY, PosZ), new Rgba(227, 170, 36, 75));
            Marker.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            Marker.Dimension = Dimension;

            ColShape = (MyColShape)Alt.CreateColShapeCylinder(Marker.Position, 1, 1.5f);
            ColShape.Description = $"[INFO] {{#FFFFFF}}{Message}";
            ColShape.Dimension = Dimension;
        }

        public void RemoveIdentifier()
        {
            Marker?.Destroy();
            Marker = null;

            ColShape?.Destroy();
            ColShape = null;
        }
    }
}