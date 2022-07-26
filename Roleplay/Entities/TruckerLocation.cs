using AltV.Net;
using Roleplay.Factories;
using Roleplay.Streamer;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class TruckerLocation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public int DeliveryValue { get; set; }

        public int LoadWaitTime { get; set; }

        public int UnloadWaitTime { get; set; }

        public string AllowedVehiclesJSON { get; set; }

        [NotMapped, JsonIgnore]
        public Marker Marker { get; set; }

        [NotMapped, JsonIgnore]
        public MyColShape ColShape { get; set; }

        [NotMapped, JsonIgnore]
        public List<string> AllowedVehicles { get; set; }

        public void CreateIdentifier()
        {
            AllowedVehicles = JsonSerializer.Deserialize<List<string>>(AllowedVehiclesJSON);
            RemoveIdentifier();

            var pos = new Vector3(PosX, PosY, PosZ - 0.95f);

            Marker = MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleSkinny,
                pos,
                new Vector3(1, 1, 1.5f),
                color: Global.MainRgba);

            ColShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            ColShape.Description = $"[{Name}] {{#FFFFFF}}Use /carregarcaixas ou /cancelarcaixas.";
        }

        public void RemoveIdentifier()
        {
            Marker?.Destroy();
            Marker = null;

            ColShape?.Remove();
            ColShape = null;
        }
    }
}