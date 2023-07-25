using Roleplay.Models;

namespace Roleplay.Entities
{
    public class CharacterItem : BaseItem
    {
        public CharacterItem() : base() { }

        public CharacterItem(ItemCategory category, uint type = 0) : base(category, type) { }

        public CharacterItem(CharacterItem baseItem) : base(baseItem)
        {
            CharacterId = baseItem.CharacterId;
            Slot = baseItem.Slot;
        }

        public int CharacterId { get; set; }

        public short Slot { get; set; }
    }
}