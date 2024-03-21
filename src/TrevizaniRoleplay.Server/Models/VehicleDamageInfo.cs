using AltV.Net.Enums;

namespace TrevizaniRoleplay.Server.Models
{
    public class VehicleDamageInfo
    {
        public List<bool> WindowsDamaged { get; set; } = [];
        public List<bool> LightsDamaged { get; set; } = [];
        public List<bool> SpecialLightsDamaged { get; set; } = [];
        public List<Wheel> Wheels { get; set; } = [];
        public List<Part> Parts { get; set; } = [];
        public List<Bumper> Bumpers { get; set; } = [];
        public List<ArmoredWindow> ArmoredWindows { get; set; } = [];

        public class Wheel(byte id, float health, bool detached, bool burst, bool hasTire)
        {
            public byte Id { get; set; } = id;
            public float Health { get; set; } = health;
            public bool Detached { get; set; } = detached;
            public bool Burst { get; set; } = burst;
            public bool HasTire { get; set; } = hasTire;
        }

        public class ArmoredWindow(byte id, float health, byte shootCount)
        {
            public byte Id { get; set; } = id;
            public float Health { get; set; } = health;
            public byte ShootCount { get; set; } = shootCount;
        }

        public class Part(VehiclePart vehiclePart, VehiclePartDamage vehiclePartDamage, byte bulletHoles)
        {
            public VehiclePart VehiclePart { get; set; } = vehiclePart;
            public VehiclePartDamage VehiclePartDamage { get; set; } = vehiclePartDamage;
            public byte BulletHoles { get; set; } = bulletHoles;
        }

        public class Bumper(VehicleBumper vehicleBumper, VehicleBumperDamage vehicleBumperDamage)
        {
            public VehicleBumper VehicleBumper { get; set; } = vehicleBumper;
            public VehicleBumperDamage VehicleBumperDamage { get; set; } = vehicleBumperDamage;
        }
    }
}