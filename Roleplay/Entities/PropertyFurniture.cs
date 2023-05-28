using AltV.Net;
using AltV.Net.Data;
using Roleplay.Factories;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class PropertyFurniture
    {
        public int Id { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public int PropertyId { get; set; }

        [JsonIgnore]
        public Property Property { get; set; }

        public string Model { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public float RotR { get; set; }

        public float RotP { get; set; }

        public float RotY { get; set; }

        public bool Interior { get; set; }

        [NotMapped, JsonIgnore]
        public string ModelName { get => Global.Furnitures.FirstOrDefault(x => x.Model.ToLower() == Model.ToLower())?.Name ?? string.Empty; }

        [NotMapped, JsonIgnore]
        public MyObject Object { get; set; }

        public void CreateObject()
        {
            Object = (MyObject)Alt.CreateNetworkObject(
                Model,
                new Position(PosX, PosY, PosZ),
                new Rotation(RotR, RotP, RotY));

            Object.Dimension = Interior ? PropertyId : 0;
            Object.Frozen = true;
            Object.Collision = true;
        }

        public void DeleteObject()
        {
            Object.Destroy();
        }
    }
}