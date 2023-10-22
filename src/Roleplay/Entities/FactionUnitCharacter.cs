using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class FactionUnitCharacter
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FactionUnit))]
        public int FactionUnitId { get; set; }

        [JsonIgnore]
        public FactionUnit FactionUnit { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }
    }
}