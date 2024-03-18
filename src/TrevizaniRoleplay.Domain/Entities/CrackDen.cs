namespace TrevizaniRoleplay.Domain.Entities
{
    public class CrackDen : BaseEntity
    {
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public int Dimension { get; private set; }
        public int OnlinePoliceOfficers { get; private set; }
        public int CooldownQuantityLimit { get; private set; }
        public int CooldownHours { get; private set; }
        public DateTime CooldownDate { get; private set; } = DateTime.Now;
        public int Quantity { get; private set; }
    }
}