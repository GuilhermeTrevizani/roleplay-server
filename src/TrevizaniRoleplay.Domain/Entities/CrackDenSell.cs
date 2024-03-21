using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class CrackDenSell : BaseEntity
    {
        public Guid CrackDenId { get; private set; }
        public Guid CharacterId { get; private set; }
        public DateTime Date { get; private set; } = DateTime.Now;
        public ItemCategory ItemCategory { get; private set; }
        public int Quantity { get; private set; }
        public int Value { get; private set; }

        public CrackDen? CrackDen { get; private set; }
        public Character? Character { get; private set; }

        public void Create(Guid crackDenId, Guid characterId, ItemCategory itemCategory, int quantity, int value)
        {
            CrackDenId = crackDenId;
            CharacterId = characterId;
            ItemCategory = itemCategory;
            Quantity = quantity;
            Value = value;
        }
    }
}