using Roleplay.Models;

namespace Roleplay.Entities
{
    public class FinancialTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Date { get; set; } = DateTime.Now;

        public FinancialTransactionType Type { get; set; }

        public int CharacterId { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }
    }
}