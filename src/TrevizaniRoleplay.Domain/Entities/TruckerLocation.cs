namespace TrevizaniRoleplay.Domain.Entities
{
    public class TruckerLocation : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public int DeliveryValue { get; private set; }
        public int LoadWaitTime { get; private set; }
        public int UnloadWaitTime { get; private set; }
        public string AllowedVehiclesJSON { get; private set; } = "[]";
    }
}