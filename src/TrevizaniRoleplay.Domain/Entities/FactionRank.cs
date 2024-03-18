namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionRank : BaseEntity
    {
        public Guid FactionId { get; private set; }
        public int Position { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Salary { get; private set; }

        public Faction? Faction { get; private set; }
    }
}