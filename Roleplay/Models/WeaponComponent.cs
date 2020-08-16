using AltV.Net.Enums;

namespace Roleplay.Models
{
    public class WeaponComponent
    {
        public WeaponComponent(WeaponModel weapon, string name, uint hash)
        {
            Weapon = weapon;
            Name = name;
            Hash = hash;
        }

        public WeaponModel Weapon { get; set; }
        public string Name { get; set; }
        public uint Hash { get; set; }
    }
}