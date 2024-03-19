namespace TrevizaniRoleplay.Domain.Entities
{
    public class Jail : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public Guid CharacterId { get; private set; }
        public Guid PoliceOfficerCharacterId { get; private set; }
        public Guid FactionId { get; private set; }
        public DateTime EndDate { get; private set; }
        public string? Description { get; private set; }

        public Character? Character { get; private set; }
        public Character? PoliceOfficerCharacter { get; private set; }
        public Faction? Faction { get; private set; }

        public void Create(Guid characterId, Guid policeOfficerCharacterId, Guid factionId, int minutes)
        {
            CharacterId = characterId;
            PoliceOfficerCharacterId = policeOfficerCharacterId;
            FactionId = factionId;
            EndDate = DateTime.Now.AddMinutes(minutes);
        }
    }
}