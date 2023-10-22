using AltV.Net;
using AltV.Net.Elements.Entities;
using Roleplay.Factories;
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
        public IMarker Marker { get; set; }

        [NotMapped, JsonIgnore]
        public MyColShape ColShape { get; set; }

        [NotMapped, JsonIgnore]
        public List<string> AllowedVehicles { get; set; }

        [Obsolete("TODO: Rollback commentary when alt:V implements")]
        public void CreateIdentifier()
        {
            AllowedVehicles = JsonSerializer.Deserialize<List<string>>(AllowedVehiclesJSON);
            RemoveIdentifier();

            var pos = new Vector3(PosX, PosY, PosZ - 0.95f);

            //Marker = Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //Marker.Scale = new Vector3(1, 1, 1.5f);

            ColShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            ColShape.Description = $"[{Name}] {{#FFFFFF}}Use /carregarcaixas ou /cancelarcaixas.";
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