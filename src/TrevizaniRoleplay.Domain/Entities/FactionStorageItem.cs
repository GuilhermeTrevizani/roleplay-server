namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionStorageItem : BaseItem
    {
        public Guid FactionStorageId { get; private set; }

        // Tudo isso virou o BaseItem
        //public string Model { get; private set; } = string.Empty;
        //public int Ammo { get; private set; } = 1;
        //public int Quantity { get; private set; }
        //public byte TintIndex { get; private set; }
        //public string ComponentsJSON { get; private set; } = "[]";
        //public ItemCategory ItemCategory { get; private set; }
        //public int Quantity { get; private set; }

        public FactionStorage? FactionStorage { get; private set; }
    }
}