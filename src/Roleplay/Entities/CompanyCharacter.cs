using Roleplay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class CompanyCharacter
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }

        [JsonIgnore]
        public Company Company { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        public Character Character { get; set; }

        public string FlagsJSON { get; set; } = "[]";

        [JsonIgnore, NotMapped]
        public List<CompanyFlag> Flags { get => JsonSerializer.Deserialize<List<CompanyFlag>>(FlagsJSON); }
    }
}