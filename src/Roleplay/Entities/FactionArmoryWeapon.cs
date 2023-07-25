using AltV.Net.Enums;

namespace Roleplay.Entities
{
    public class FactionArmoryWeapon
    {
        public int Id { get; set; }

        public int FactionArmoryId { get; set; }

        public WeaponModel Weapon { get; set; }

        public int Ammo { get; set; } = 1;

        public int Quantity { get; set; }

        public byte TintIndex { get; set; }

        public string ComponentsJSON { get; set; } = "[]";
    }
}