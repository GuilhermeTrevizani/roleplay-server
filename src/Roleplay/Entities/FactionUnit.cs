using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class FactionUnit
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int FactionId { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }

        public DateTime InitialDate { get; set; } = DateTime.Now;

        public DateTime? FinalDate { get; set; }

        public string Plate { get; set; }

        [JsonIgnore]
        public ICollection<FactionUnitCharacter> Characters { get; set; }
    }
}