namespace Roleplay.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CorrectQuestionAnswerId { get; set; }
    }
}