using Roleplay.Models;

namespace Roleplay.Entities
{
    public class VehicleItem : BaseItem
    {
        public VehicleItem() : base() { }

        public VehicleItem(ItemCategory category, uint type = 0) : base(category, type) { }

        public VehicleItem(VehicleItem baseItem) : base(baseItem)
        {
            VehicleId = baseItem.VehicleId;
            Slot = baseItem.Slot;
        }

        public int VehicleId { get; set; }

        public byte Slot { get; set; }
    }
}