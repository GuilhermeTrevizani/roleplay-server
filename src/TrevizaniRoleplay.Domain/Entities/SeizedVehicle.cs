namespace TrevizaniRoleplay.Domain.Entities
{
    public class SeizedVehicle : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public Guid VehicleId { get; private set; }
        public Guid PoliceOfficerCharacterId { get; private set; }
        public int Value { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public DateTime? PaymentDate { get; private set; }
        public Guid FactionId { get; private set; }
        public string? Description { get; private set; }

        public Vehicle? Vehicle { get; private set; }
        public Character? PoliceOfficerCharacter { get; private set; }
        public Faction? Faction { get; private set; }

        public void Create(Guid vehicleId, Guid policeOfficerCharacterId, int value, string reason, Guid factionId)
        {
            VehicleId = vehicleId;
            PoliceOfficerCharacterId = policeOfficerCharacterId;
            Value = value;
            Reason = reason;
            FactionId = factionId;
        }
    }
}