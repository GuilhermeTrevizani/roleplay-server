using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Wanted
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(PoliceOfficerCharacter))]
        public int PoliceOfficerCharacterId { get; set; }
        public Character PoliceOfficerCharacter { get; set; }

        [ForeignKey(nameof(WantedCharacter))]
        public int? WantedCharacterId { get; set; }
        public Character WantedCharacter { get; set; }

        [ForeignKey(nameof(WantedVehicle))]
        public int? WantedVehicleId { get; set; }
        public Vehicle WantedVehicle { get; set; }

        public string Reason { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? PoliceOfficerDeletedCharacterId { get; set; }
    } 
}