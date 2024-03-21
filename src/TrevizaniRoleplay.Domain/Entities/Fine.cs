namespace TrevizaniRoleplay.Domain.Entities
{
    public class Fine : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public Guid CharacterId { get; private set; }
        public Guid PoliceOfficerCharacterId { get; private set; }
        public int Value { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public DateTime? PaymentDate { get; private set; }
        public string? Description { get; private set; }

        public Character? Character { get; private set; }
        public Character? PoliceOfficerCharacter { get; private set; }

        public void Create(Guid characterId, Guid policeOfficerCharacterId, string reason, int value)
        {
            CharacterId = characterId;
            Reason = reason;
            PoliceOfficerCharacterId = policeOfficerCharacterId;
            Value = value;
        }

        public void Pay()
        {
            PaymentDate = DateTime.Now;
        }
    }
}