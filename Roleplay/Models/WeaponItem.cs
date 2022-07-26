using System.Collections.Generic;

namespace Roleplay.Models
{
    public class WeaponItem
    {
        public int Ammo { get; set; } = 1;
        public byte TintIndex { get; set; }
        public List<uint> Components { get; set; } = new();
    }
}