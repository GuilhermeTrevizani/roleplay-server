namespace TrevizaniRoleplay.Domain.Entities
{
    public class Blip : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public ushort Type { get; private set; }
        public byte Color { get; private set; }
    }
}