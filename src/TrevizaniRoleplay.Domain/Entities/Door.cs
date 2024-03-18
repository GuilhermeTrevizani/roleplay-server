namespace TrevizaniRoleplay.Domain.Entities
{
    public class Door : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public long Hash { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public Guid? FactionId { get; private set; }
        public bool Locked { get; private set; } = true;
        public Guid? CompanyId { get; private set; }

        public Faction? Faction { get; private set; }
        public Company? Company { get; private set; }
    }
}