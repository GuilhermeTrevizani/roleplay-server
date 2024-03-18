namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionUnitCharacter : BaseEntity
    {
        public Guid FactionUnitId { get; private set; }
        public Guid CharacterId { get; private set; }

        public FactionUnit? FactionUnit { get; private set; }
        public Character? Character { get; private set; }
    }
}