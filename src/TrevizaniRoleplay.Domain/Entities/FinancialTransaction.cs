﻿using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class FinancialTransaction : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public FinancialTransactionType Type { get; private set; }
        public Guid CharacterId { get; private set; }
        public int Value { get; private set; }
        public string Description { get; private set; } = string.Empty;

        public Character? Character { get; private set; }

        public void Create(FinancialTransactionType type, Guid characterId, int value, string description)
        {
            Type = type;
            CharacterId = characterId;
            Value = value;
            Description = description;
        }
    }
}