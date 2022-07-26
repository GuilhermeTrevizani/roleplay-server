using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Confiscation
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }

        [ForeignKey(nameof(PoliceOfficerCharacter))]
        public int PoliceOfficerCharacterId { get; set; }

        [JsonIgnore]
        public Character PoliceOfficerCharacter { get; set; }

        [ForeignKey(nameof(Faction))]
        public int FactionId { get; set; }

        [JsonIgnore]
        public Faction Faction { get; set; }

        public string Description { get; set; }

        public ICollection<ConfiscationItem> Items { get; set; }
    }
}