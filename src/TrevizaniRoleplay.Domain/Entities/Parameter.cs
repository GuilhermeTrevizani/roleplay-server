namespace TrevizaniRoleplay.Domain.Entities
{
    public class Parameter : BaseEntity
    {
        public int MaxCharactersOnline { get; private set; }
        public int VehicleParkValue { get; private set; } = 1;
        public int HospitalValue { get; private set; } = 1;
        public int BarberValue { get; private set; } = 1;
        public int ClothesValue { get; private set; } = 1;
        public int DriverLicenseBuyValue { get; private set; } = 1;
        public int DriverLicenseRenewValue { get; private set; } = 1;
        public int FuelValue { get; private set; } = 1;
        public int Paycheck { get; private set; } = 1;
        public int AnnouncementValue { get; private set; } = 1;
        public int ExtraPaymentGarbagemanValue { get; private set; } = 1;
        public bool Blackout { get; private set; }
        public DateTime? InactivePropertiesDate { get; private set; }
        public int KeyValue { get; private set; } = 1;
        public int LockValue { get; private set; } = 1;
        public string IPLsJSON { get; private set; } = "[]";
        public int TattooValue { get; private set; } = 1;
        public int CooldownDismantleHours { get; private set; }
        public int PropertyRobberyConnectedTime { get; private set; }
        public int CooldownPropertyRobberyRobberHours { get; private set; }
        public int CooldownPropertyRobberyPropertyHours { get; private set; }
        public int PoliceOfficersPropertyRobbery { get; private set; }
        public byte InitialTimeCrackDen { get; private set; }
        public byte EndTimeCrackDen { get; private set; }
        public int FirefightersBlockHeal { get; private set; }

        public void SetInactivePropertiesDate()
        {
            InactivePropertiesDate = DateTime.Now.AddDays(1);
        }

        public void SetMaxCharactersOnline(int value)
        {
            MaxCharactersOnline = value;
        }
    }
}