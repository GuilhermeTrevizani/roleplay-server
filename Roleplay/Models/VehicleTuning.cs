using System.Collections.Generic;

namespace Roleplay.Models
{
    public class VehicleTuning
    {
        public int? VehicleId { get; set; }

        public int? TargetId { get; set; }

        public bool Staff { get; set; }

        public List<Mod> Mods { get; set; } = new List<Mod>();

        public List<Mod> CurrentMods { get; set; }

        public byte Repair { get; set; }

        public int RepairValue { get; set; }

        public byte WheelType { get; set; }

        public byte CurrentWheelType { get; set; }

        public byte WheelVariation { get; set; }

        public byte CurrentWheelVariation { get; set; }

        public byte WheelColor { get; set; }

        public byte CurrentWheelColor { get; set; }

        public int WheelValue { get; set; }

        public string Color1 { get; set; }

        public string CurrentColor1 { get; set; }

        public string Color2 { get; set; }

        public string CurrentColor2 { get; set; }

        public int ColorValue { get; set; }

        public string NeonColor { get; set; }

        public string CurrentNeonColor { get; set; }

        public byte NeonLeft { get; set; }

        public byte CurrentNeonLeft { get; set; }

        public byte NeonRight { get; set; }

        public byte CurrentNeonRight { get; set; }

        public byte NeonFront { get; set; }

        public byte CurrentNeonFront { get; set; }

        public byte NeonBack { get; set; }

        public byte CurrentNeonBack { get; set; }

        public int NeonValue { get; set; }

        public byte HeadlightColor { get; set; }

        public float LightsMultiplier { get; set; }

        public int XenonColorValue { get; set; }

        public byte CurrentHeadlightColor { get; set; }

        public float CurrentLightsMultiplier { get; set; }

        public byte WindowTint { get; set; }

        public byte CurrentWindowTint { get; set; }

        public int WindowTintValue { get; set; }

        public string TireSmokeColor { get; set; }

        public string CurrentTireSmokeColor { get; set; }

        public int TireSmokeColorValue { get; set; }

        public class Mod
        {
            public byte Type { get; set; }

            public string Name { get; set; }

            public int UnitaryValue { get; set; }

            public int Value { get; set; }

            public byte Current { get; set; }

            public byte Selected { get; set; }

            public int ModsCount { get; set; }
        }
    }
}