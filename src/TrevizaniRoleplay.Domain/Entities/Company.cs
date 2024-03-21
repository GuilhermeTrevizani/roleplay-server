namespace TrevizaniRoleplay.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public int WeekRentValue { get; private set; }
        public DateTime? RentPaymentDate { get; private set; }
        public Guid? CharacterId { get; private set; }
        public string Color { get; private set; } = "000000";
        public ushort BlipType { get; private set; }
        public byte BlipColor { get; private set; }

        public ICollection<CompanyCharacter>? Characters { get; private set; }

        public void Create(string name, float posX, float posY, float posZ, int weekRentValue)
        {
            Name = name;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            WeekRentValue = weekRentValue;
            Characters = [];
        }

        public void Update(string name, float posX, float posY, float posZ, int weekRentValue)
        {
            Name = name;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            WeekRentValue = weekRentValue;
        }

        public void Rent(Guid characterId)
        {
            CharacterId = characterId;
            RenewRent();
        }

        public void Update(string color, ushort blipType, byte blipColor)
        {
            Color = color;
            BlipType = Convert.ToUInt16(blipType);
            BlipColor = Convert.ToByte(blipColor);
        }

        public void RenewRent()
        {
            RentPaymentDate = DateTime.Now.AddDays(7);
        }

        public void ResetOwner()
        {
            CharacterId = null;
            RentPaymentDate = null;
        }
    }
}