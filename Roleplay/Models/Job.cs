using AltV.Net.Data;
using AltV.Net.Enums;

namespace Roleplay.Models
{
    public class Job
    {
        public CharacterJob CharacterJob { get; set; }

        public Position Position { get; set; }

        public Position VehicleRentPosition { get; set; }

        public Rotation VehicleRentRotation { get; set; }

        public VehicleModel VehicleModel { get; set; }

        public Rgba VehicleColor { get; set; }
    }
}