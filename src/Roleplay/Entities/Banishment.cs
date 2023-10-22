using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Banishment
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public DateTime? ExpirationDate { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }

        /// <summary>
        /// If null means user is banned only in character
        /// </summary>
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public string Reason { get; set; }

        [ForeignKey(nameof(StaffUser))]
        public int StaffUserId { get; set; }

        [JsonIgnore]
        public User StaffUser { get; set; }
    }
}