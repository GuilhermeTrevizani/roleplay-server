using AltV.Net.Data;

namespace TrevizaniRoleplay.Server.Models
{
    public class Wound
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public ushort Damage { get; set; }
        public uint Weapon { get; set; }
        public string? Attacker { get; set; }
        public BodyPart BodyPart { get; set; }
        public float Distance { get; set; }
        public Position ShotOffset { get; set; }
    }
}