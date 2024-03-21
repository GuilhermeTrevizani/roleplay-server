using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Property : BaseEntity
    {
        public PropertyInterior Interior { get; private set; }
        public int Value { get; private set; }
        public Guid? CharacterId { get; private set; }
        public float EntrancePosX { get; private set; }
        public float EntrancePosY { get; private set; }
        public float EntrancePosZ { get; private set; }
        public int EntranceDimension { get; private set; }
        public float ExitPosX { get; private set; }
        public float ExitPosY { get; private set; }
        public float ExitPosZ { get; private set; }
        public string Address { get; private set; } = string.Empty;
        public int Number { get; private set; }
        public uint LockNumber { get; private set; }
        public bool Locked { get; private set; }
        public byte ProtectionLevel { get; private set; }
        public int RobberyValue { get; private set; }
        public DateTime? RobberyCooldown { get; private set; }

        public Character? Character { get; private set; }
        public ICollection<PropertyFurniture>? Furnitures { get; set; }
        public ICollection<PropertyItem>? Items { get; set; }

        public void Create(uint lockNumber, PropertyInterior interior, float entrancePosX, float entrancePosY, float entrancePosZ, int entranceDimension,
            int value, float exitPosX, float exitPosY, float exitPosZ, string address, int number)
        {
            LockNumber = lockNumber;
            Interior = interior;
            EntrancePosX = entrancePosX;
            EntrancePosY = entrancePosY;
            EntrancePosZ = entrancePosZ;
            EntranceDimension = entranceDimension;
            Value = value;
            ExitPosX = exitPosX;
            ExitPosY = exitPosY;
            ExitPosZ = exitPosZ;
            Address = address;
            Number = number;
            Items = [];
        }

        public void Update(PropertyInterior interior, float entrancePosX, float entrancePosY, float entrancePosZ, int entranceDimension,
            int value, float exitPosX, float exitPosY, float exitPosZ, string address)
        {
            Interior = interior;
            EntrancePosX = entrancePosX;
            EntrancePosY = entrancePosY;
            EntrancePosZ = entrancePosZ;
            EntranceDimension = entranceDimension;
            Value = value;
            ExitPosX = exitPosX;
            ExitPosY = exitPosY;
            ExitPosZ = exitPosZ;
            Address = address;
        }

        public void RemoveOwner()
        {
            CharacterId = null;
            Locked = false;
            ProtectionLevel = 0;
            RobberyValue = 0;
            RobberyCooldown = null;
            LockNumber = 0;
        }

        public void SetOwner(Guid characterId)
        {
            CharacterId = characterId;
        }

        public void SetLocked(bool locked)
        {
            Locked = locked;
        }

        public void SetProtectionLevel(byte value)
        {
            ProtectionLevel = value;
        }

        public void SetLockNumber(uint value)
        {
            LockNumber = value;
        }

        public void SetRobberyValue(int value)
        {
            RobberyValue = value;
        }

        public void ResetRobberyValue()
        {
            RobberyValue = 0;
        }

        public void SetRobberyCooldown(DateTime cooldown)
        {
            RobberyCooldown = cooldown;
        }
    }
}