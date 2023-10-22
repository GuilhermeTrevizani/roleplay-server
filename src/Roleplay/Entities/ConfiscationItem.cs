using Roleplay.Models;

namespace Roleplay.Entities
{
    public class ConfiscationItem : BaseItem
    {
        public ConfiscationItem() : base() { }

        public ConfiscationItem(ItemCategory category, uint type = 0) : base(category, type) { }

        public ConfiscationItem(ConfiscationItem baseItem) : base(baseItem)
        {
            ConfiscationId = baseItem.ConfiscationId;
        }

        public int ConfiscationId { get; set; }

        public Confiscation Confiscation { get; set; }
    }
}