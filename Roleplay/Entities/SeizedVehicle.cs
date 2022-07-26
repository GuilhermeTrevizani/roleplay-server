using System;

namespace Roleplay.Entities
{
    public class SeizedVehicle
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int VehicleId { get; set; }

        public int PoliceOfficerCharacterId { get; set; }

        public int Value { get; set; }

        public string Reason { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int FactionId { get; set; }

        public string Description { get; set; }
    }
}