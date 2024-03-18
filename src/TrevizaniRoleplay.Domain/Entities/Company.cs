﻿namespace TrevizaniRoleplay.Domain.Entities
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
    }
}