namespace TrevizaniRoleplay.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime RegisterDate { get; } = DateTime.Now;
    }
}