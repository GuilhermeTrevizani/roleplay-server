using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Fine
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }
        public Character Character { get; set; }

        [ForeignKey(nameof(PoliceOfficerCharacter))]
        public int PoliceOfficerCharacterId { get; set; }
        public Character PoliceOfficerCharacter { get; set; }

        public int Value { get; set; }

        public string Reason { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string Description { get; set; }
    }
}