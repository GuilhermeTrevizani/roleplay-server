using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class FinancialTransaction
    {
        public ulong Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public FinancialTransactionType Type { get; set; }

        public int CharacterId { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }
    }
}