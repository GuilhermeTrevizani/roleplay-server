using AltV.Net.Data;
using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Server.Models
{
    public class Dealership
    {
        public string Name { get; set; } = string.Empty;
        public PriceType PriceType { get; set; }
        public Position Position { get; set; }
        public Position VehiclePosition { get; set; }
        public Position VehicleRotation { get; set; }
    }
}