﻿namespace TrevizaniRoleplay.Domain.Entities
{
    public class Banishment : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public DateTime? ExpirationDate { get; private set; }
        public Guid CharacterId { get; private set; }
        /// <summary>
        /// If null means only Character is banned
        /// </summary>
        public Guid? UserId { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public Guid StaffUserId { get; private set; }

        public Character? Character { get; private set; }
        public User? User { get; private set; }
        public User? StaffUser { get; private set; }

        public void Create(DateTime? expirationDate, Guid characterId, Guid userId, string reason, Guid staffUserId)
        {
            ExpirationDate = expirationDate;
            CharacterId = characterId;
            UserId = userId;
            Reason = reason;
            StaffUserId = staffUserId;
        }

        public void ClearUserId()
        {
            UserId = null;
        }
    }
}