namespace TrevizaniRoleplay.Domain.Entities
{
    public class ConfiscationItem : BaseItem
    {
        public Guid ConfiscationId { get; private set; }

        public Confiscation? Confiscation { get; private set; }
    }
}