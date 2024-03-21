namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionUnit : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Guid FactionId { get; private set; }
        public Guid CharacterId { get; private set; }
        public DateTime InitialDate { get; private set; } = DateTime.Now;
        public DateTime? FinalDate { get; private set; }
        public string Plate { get; private set; } = string.Empty;

        public Faction? Faction { get; private set; }
        public Character? Character { get; private set; }
        public ICollection<FactionUnitCharacter>? Characters { get; private set; }

        public void Create(string name, string description, Guid factionId, Guid characterId, string plate,
            ICollection<FactionUnitCharacter> characters)
        {
            Name = name;
            Description = description;
            FactionId = factionId;
            CharacterId = characterId;
            Plate = plate;
            Characters = characters;
        }

        public void End()
        {
            FinalDate = DateTime.Now;
        }
    }
}