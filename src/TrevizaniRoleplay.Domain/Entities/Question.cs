namespace TrevizaniRoleplay.Domain.Entities
{
    public class Question : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Guid CorrectQuestionAnswerId { get; private set; }

        public QuestionAnswer? CorrectQuestionAnswer { get; private set; }
    }
}