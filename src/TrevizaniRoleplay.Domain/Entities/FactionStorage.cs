namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionStorage : BaseEntity
    {
        public Guid FactionId { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public int Dimension { get; private set; }

        public Faction? Faction { get; private set; }
    }
}