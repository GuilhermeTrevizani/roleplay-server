namespace TrevizaniRoleplay.Domain.Entities
{
    public class Confiscation : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public Guid CharacterId { get; private set; }
        public Guid PoliceOfficerCharacterId { get; private set; }
        public Guid FactionId { get; private set; }
        public string? Description { get; private set; }

        public Character? Character { get; private set; }
        public Character? PoliceOfficerCharacter { get; private set; }
        public Faction? Faction { get; private set; }
        public ICollection<ConfiscationItem>? Items { get; private set; }

        public void Create(Guid characterId, Guid policeOfficerCharacterid, Guid factionId, ICollection<ConfiscationItem> items)
        {
            CharacterId = characterId;
            PoliceOfficerCharacterId = policeOfficerCharacterid;
            FactionId = factionId;
            Items = items;
        }
    }
}