using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class CrackDenItem : BaseEntity
    {
        public Guid CrackDenId { get; private set; }
        public ItemCategory ItemCategory { get; private set; }
        public int Value { get; private set; }

        public CrackDen? CrackDen { get; set; }

        public void Create(Guid crackDenId, ItemCategory itemCategory, int value)
        {
            CrackDenId = crackDenId;
            ItemCategory = itemCategory;
            Value = value;
        }

        public void Update(Guid crackDenId, ItemCategory itemCategory, int value)
        {
            CrackDenId = crackDenId;
            ItemCategory = itemCategory;
            Value = value;
        }
    }
}