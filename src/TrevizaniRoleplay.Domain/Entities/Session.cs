using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Session : BaseEntity
    {
        public Guid CharacterId { get; private set; }
        public SessionType Type { get; private set; }
        public DateTime InitialDate { get; private set; } = DateTime.Now;
        public DateTime? FinalDate { get; private set; }

        public Character? Character { get; private set; }

        public void Create(Guid characterId, SessionType type)
        {
            CharacterId = characterId;
            Type = type;
        }

        public void End()
        {
            FinalDate = DateTime.Now;
        }
    }
}