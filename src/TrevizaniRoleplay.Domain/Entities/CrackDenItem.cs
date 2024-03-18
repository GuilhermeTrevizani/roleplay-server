using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class CrackDenItem : BaseEntity
    {
        public Guid CrackDenId { get; private set; }
        public ItemCategory ItemCategory { get; private set; }
        public int Value { get; private set; }

        public CrackDen? CrackDen { get; set; }
    }
}