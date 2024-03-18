using System.ComponentModel.DataAnnotations.Schema;
using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Price : BaseEntity
    {
        public PriceType Type { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public float Value { get; private set; }

        [NotMapped]
        public bool IsVehicle => Type == PriceType.Cars || Type == PriceType.MotorcyclesAndBicycles
            || Type == PriceType.Airplanes || Type == PriceType.Helicopters || Type == PriceType.Boats;

        public void Create(PriceType type, string name, float value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}