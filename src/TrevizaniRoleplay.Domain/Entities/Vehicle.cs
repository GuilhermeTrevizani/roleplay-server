using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string Model { get; private set; } = string.Empty;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotR { get; private set; }
        public float RotP { get; private set; }
        public float RotY { get; private set; }
        public byte Color1R { get; private set; } = 255;
        public byte Color1G { get; private set; } = 255;
        public byte Color1B { get; private set; } = 255;
        public byte Color2R { get; private set; } = 255;
        public byte Color2G { get; private set; } = 255;
        public byte Color2B { get; private set; } = 255;
        public Guid? CharacterId { get; private set; }
        public string Plate { get; private set; } = string.Empty;
        public Guid? FactionId { get; private set; }
        public int EngineHealth { get; private set; } = 1000;
        public byte Livery { get; private set; } = 1;
        public int SeizedValue { get; private set; }
        public int Fuel { get; private set; }
        public string StructureDamagesJSON { get; private set; } = "{}";
        public bool Parked { get; private set; }
        public bool Sold { get; private set; }
        public CharacterJob Job { get; private set; } = CharacterJob.None;
        public string DamagesJSON { get; private set; } = "[]";
        public uint BodyHealth { get; private set; } = 1000;
        public uint BodyAdditionalHealth { get; private set; } = 1000;
        public int PetrolTankHealth { get; private set; } = 1000;
        public uint LockNumber { get; private set; }
        public bool FactionGift { get; private set; }
        public byte ProtectionLevel { get; private set; }
        public int DismantledValue { get; private set; }
        public bool XMR { get; private set; }
        public string ModsJSON { get; private set; } = "[]";
        public byte WheelType { get; private set; }
        public byte WheelVariation { get; private set; }
        public byte WheelColor { get; private set; }
        public byte NeonColorR { get; private set; }
        public byte NeonColorG { get; private set; }
        public byte NeonColorB { get; private set; }
        public bool NeonLeft { get; private set; }
        public bool NeonRight { get; private set; }
        public bool NeonFront { get; private set; }
        public bool NeonBack { get; private set; }
        public byte HeadlightColor { get; private set; }
        public float LightsMultiplier { get; private set; } = 1;
        public byte WindowTint { get; private set; }
        public byte TireSmokeColorR { get; private set; }
        public byte TireSmokeColorG { get; private set; }
        public byte TireSmokeColorB { get; private set; }

        public Character? Character { get; private set; }
        public Faction? Faction { get; private set; }
        public ICollection<VehicleItem>? Items { get; private set; }

        public void Create(string model, string plate, byte color1R, byte color1G, byte color1B, byte color2R, byte color2G, byte color2B)
        {
            Model = model;
            Plate = plate;
            Color1R = color1R;
            Color1G = color1G;
            Color1B = color1B;
            Color2R = color2R;
            Color2G = color2G;
            Color2B = color2B;

            //var vehicle = new Vehicle()
            //{
            //    CharacterId = player.Character.Id,
            //    Plate = ,
            //    PosX = concessionaria.VehiclePosition.X,
            //    PosY = concessionaria.VehiclePosition.Y,
            //    PosZ = concessionaria.VehiclePosition.Z,
            //    RotR = concessionaria.VehicleRotation.X,
            //    RotP = concessionaria.VehicleRotation.Y,
            //    RotY = concessionaria.VehicleRotation.Z,
            //};
        }

        public void SetFuel(int fuel)
        {
            Fuel = fuel;
        }

        public void ChangePosition(float posX, float posY, float posZ, float rotR, float rotP, float rotY)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotR = rotR;
            RotP = rotP;
            RotY = rotY;
        }

        public void SetDamages(int engineHealth, uint bodyHealth, uint bodyAdditionalHealth, int petrolTankHealth,
            string structureDamagesJSON, string damagesJSON)
        {
            EngineHealth = engineHealth;
            BodyHealth = bodyHealth;
            BodyAdditionalHealth = bodyAdditionalHealth;
            PetrolTankHealth = petrolTankHealth;
            StructureDamagesJSON = structureDamagesJSON;
            DamagesJSON = damagesJSON;
        }

        public void SetSold()
        {
            Sold = true;
        }

        public void SetSeizedValue(int value)
        {
            SeizedValue = value;
        }

        public void SetOwner(Guid characterId)
        {
            CharacterId = characterId;
            Parked = false;
        }

        public void SetParked()
        {
            Parked = true;
        }

        public void SetLockNumber(uint value)
        {
            LockNumber = value;
        }

        public void SetProtectionLevel(byte value)
        {
            ProtectionLevel = value;
        }

        public void SetXMR()
        {
            XMR = true;
        }
    }
}