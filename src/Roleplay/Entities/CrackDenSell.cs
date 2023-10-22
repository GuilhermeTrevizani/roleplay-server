using Roleplay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class CrackDenSell
    {
        public int Id { get; set; }

        public int CrackDenId { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public ItemCategory ItemCategory { get; set; }

        public int Quantity { get; set; }

        public int Value { get; set; }
    }
}