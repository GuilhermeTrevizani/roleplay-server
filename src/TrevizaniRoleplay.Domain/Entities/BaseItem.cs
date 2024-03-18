using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public abstract class BaseItem : BaseEntity
    {
        public ItemCategory Category { get; private set; }
        public uint Type { get; private set; }
        public int Quantity { get; private set; } = 1;
        public string? Extra { get; private set; }

        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
        }

        public void SetExtra(string? extra)
        {
            Extra = extra;
        }

        public void Create(ItemCategory category, uint type, int quantity, string? extra)
        {
            Category = category;
            Type = type;
            Quantity = quantity;
            Extra = extra;
        }
    }
}