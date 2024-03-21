namespace TrevizaniRoleplay.Domain.Entities
{
    public class CharacterItem : BaseItem
    {
        public Guid CharacterId { get; private set; }
        public short Slot { get; private set; }

        public Character? Character { get; private set; }

        public void SetSlot(short slot)
        {
            Slot = slot;
        }

        public void SetCharacterId(Guid characterId)
        {
            CharacterId = characterId;
        }
    }
}