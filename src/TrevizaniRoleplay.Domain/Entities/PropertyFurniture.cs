namespace TrevizaniRoleplay.Domain.Entities
{
    public class PropertyFurniture : BaseEntity
    {
        public Guid PropertyId { get; private set; }
        public string Model { get; private set; } = string.Empty;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotR { get; private set; }
        public float RotP { get; private set; }
        public float RotY { get; private set; }
        public bool Interior { get; private set; }

        public Property? Property { get; set; }
    }
}