using System;

namespace Roleplay.Entities
{
    public class Parameter
    {
        public int Id { get; set; } = 1;

        public int MaxCharactersOnline { get; set; }

        public int VehicleParkValue { get; set; } = 1;

        public int HospitalValue { get; set; } = 1;

        public int BarberValue { get; set; } = 1;

        public int ClothesValue { get; set; } = 1;

        public int DriverLicenseBuyValue { get; set; } = 1;

        public int DriverLicenseRenewValue { get; set; } = 1;

        public int FuelValue { get; set; } = 1;

        public int Paycheck { get; set; } = 1;

        public int AnnouncementValue { get; set; } = 1;

        public int ExtraPaymentGarbagemanValue { get; set; } = 1;

        public bool Blackout { get; set; }

        public DateTime? InactivePropertiesDate { get; set; }

        public int KeyValue { get; set; } = 1;

        public int LockValue { get; set; } = 1;

        public string IPLsJSON { get; set; } = "[]";

        public int TattooValue { get; set; } = 1;

        public int CooldownDismantleHours { get; set; }

        public int PropertyRobberyConnectedTime { get; set; }

        public int CooldownPropertyRobberyRobberHours { get; set; }

        public int CooldownPropertyRobberyPropertyHours { get; set; }

        public int PoliceOfficersPropertyRobbery { get; set; }

        public byte InitialTimeCrackDen { get; set; }

        public byte EndTimeCrackDen { get; set; }

        public int FirefightersBlockHeal { get; set; }
    }
}