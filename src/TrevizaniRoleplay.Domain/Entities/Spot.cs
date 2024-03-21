using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Spot : BaseEntity
    {
        public SpotType Type { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float AuxiliarPosX { get; private set; }
        public float AuxiliarPosY { get; private set; }
        public float AuxiliarPosZ { get; private set; }

        public void Create(SpotType type,
            float posX, float posY, float posZ,
            float auxiliarPosX, float auxiliarPosY, float auxiliarPosZ)
        {
            Type = type;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            AuxiliarPosX = auxiliarPosX;
            AuxiliarPosY = auxiliarPosY;
            AuxiliarPosZ = auxiliarPosZ;
        }

        public void Update(SpotType type,
            float posX, float posY, float posZ,
            float auxiliarPosX, float auxiliarPosY, float auxiliarPosZ)
        {
            Type = type;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            AuxiliarPosX = auxiliarPosX;
            AuxiliarPosY = auxiliarPosY;
            AuxiliarPosZ = auxiliarPosZ;
        }
    }
}