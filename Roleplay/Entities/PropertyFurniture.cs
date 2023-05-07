using AltV.Net;
using AltV.Net.Elements.Entities;
using System;
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
        public IObject Object { get; set; }

        [Obsolete("TODO")]
        public void CreateObject()
        {
            //Alt.CreateObject
            //Object = PropStreamer.Create(
            //    Model,
            //    new Vector3(PosX, PosY, PosZ),
            //    new Vector3(RotR, RotP, RotY),
            //    Interior ? PropertyId : 0,
            //    true,
            //    frozen: true,
            //    collision: true);
        }

        public void DeleteObject()
        {
            Object.Destroy();
        }
    }
}