using AltV.Net.Data;

namespace Roleplay.Models
{
    public class Dealership
    {
        public string Name { get; set; }

        public PriceType PriceType { get; set; }

        public Position Position { get; set; }

        public Position VehiclePosition { get; set; }

        public Position VehicleRotation { get; set; }
    }
}