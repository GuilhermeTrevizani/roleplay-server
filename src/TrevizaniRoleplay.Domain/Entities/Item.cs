namespace TrevizaniRoleplay.Domain.Entities
{
    public class Item : BaseItem
    {
        public int Dimension { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotR { get; private set; }
        public float RotP { get; private set; }
        public float RotY { get; private set; }
    }
}