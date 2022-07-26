using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Price
    {
        public int Id { get; set; }

        public PriceType Type { get; set; }

        public string Name { get; set; }

        public float Value { get; set; }

        [NotMapped, JsonIgnore]
        public bool Vehicle => Type == PriceType.Carros  || Type == PriceType.MotocicletasBicicletas
            || Type == PriceType.Avioes || Type == PriceType.Helicopteros || Type == PriceType.Barcos;
    }
}