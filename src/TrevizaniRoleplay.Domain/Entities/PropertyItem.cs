namespace TrevizaniRoleplay.Domain.Entities
{
    public class PropertyItem : BaseItem
    {
        public Guid PropertyId { get; private set; }
        public byte Slot { get; private set; }

        public Property? Property { get; private set; }

        public void SetPropertyId(Guid propertyId)
        {
            PropertyId = propertyId;
        }

        public void SetSlot(byte slot)
        {
            Slot = slot;
        }
    }
}