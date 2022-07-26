using Roleplay.Models;

namespace Roleplay.Entities
{
    public class PropertyItem : BaseItem
    {
        public PropertyItem() : base() { }

        public PropertyItem(ItemCategory category, uint type = 0) : base(category, type) { }

        public PropertyItem(PropertyItem baseItem) : base(baseItem)
        {
            PropertyId = baseItem.PropertyId;
            Slot = baseItem.Slot;
        }

        public int PropertyId { get; set; }

        public byte Slot { get; set; }
    }
}