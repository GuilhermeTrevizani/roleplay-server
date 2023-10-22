namespace Roleplay.Entities
{
    public class FactionRank
    {
        public int Id { get; set; }

        public int FactionId { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public int Salary { get; set; }
    }
}