using Roleplay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Punishment
    {
        public int Id { get; set; }

        public PunishmentType Type { get; set; }

        public int Duration { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }

        public string Reason { get; set; }

        [ForeignKey(nameof(StaffUser))]
        public int StaffUserId { get; set; }

        [JsonIgnore]
        public User StaffUser { get; set; }
    }
}