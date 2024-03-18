namespace TrevizaniRoleplay.Domain.Entities
{
    public class CompanyCharacter : BaseEntity
    {
        public Guid CompanyId { get; private set; }
        public Guid CharacterId { get; private set; }
        public string FlagsJSON { get; private set; } = "[]";

        public Character? Character { get; private set; }
        public Company? Company { get; private set; }
    }
}