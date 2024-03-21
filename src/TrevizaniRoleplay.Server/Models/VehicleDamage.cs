namespace TrevizaniRoleplay.Server.Models
{
    public class VehicleDamage
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public uint BodyHealthDamage { get; set; }
        public uint AdditionalBodyHealthDamage { get; set; }
        public uint EngineHealthDamage { get; set; }
        public uint PetrolTankDamage { get; set; }
        public uint WeaponHash { get; set; }
        public string? Attacker { get; set; }
        public float Distance { get; set; }
    }
}