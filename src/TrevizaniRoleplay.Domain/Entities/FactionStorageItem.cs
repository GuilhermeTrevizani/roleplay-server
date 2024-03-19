namespace TrevizaniRoleplay.Domain.Entities
{
    public class FactionStorageItem : BaseItem
    {
        public Guid FactionStorageId { get; private set; }

        public FactionStorage? FactionStorage { get; private set; }
    }
}