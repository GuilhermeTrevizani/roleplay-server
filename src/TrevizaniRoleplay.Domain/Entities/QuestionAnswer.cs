namespace TrevizaniRoleplay.Domain.Entities
{
    public class QuestionAnswer : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Guid QuestionId { get; private set; }

        public Question? Question { get; private set; }
    }
}