using AltV.Net.Enums;

namespace TrevizaniRoleplay.Server.Models
{
    public class WeaponComponent(WeaponModel weapon, string name, uint hash)
    {
        public WeaponModel Weapon { get; set; } = weapon;
        public string Name { get; set; } = name;
        public uint Hash { get; set; } = hash;
    }
}