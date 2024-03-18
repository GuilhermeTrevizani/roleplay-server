namespace TrevizaniRoleplay.Domain.Entities
{
    public class TruckerLocationDelivery : BaseEntity
    {
        public Guid TruckerLocationId { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }

        public TruckerLocation? TruckerLocation { get; private set; }
    }
}