using Roleplay.Models;

namespace Roleplay.Entities
{
    public class Session
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int CharacterId { get; set; }

        public SessionType Type { get; set; }

        public DateTime InitialDate { get; set; } = DateTime.Now;

        public DateTime? FinalDate { get; set; }
    }
}