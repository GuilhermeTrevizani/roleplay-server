using System;

namespace Roleplay.Entities
{
    public class Parameter
    {
        public int Id { get; set; }

        public int MaxCharactersOnline { get; set; }

        public int VehicleParkValue { get; set; }

        public int HospitalValue { get; set; }

        public int BarberValue { get; set; }

        public int ClothesValue { get; set; }

        public int DriverLicenseBuyValue { get; set; }

        public int DriverLicenseRenewValue { get; set; }

        public int FuelValue { get; set; }

        public int Paycheck { get; set; } = 1;

        public int AnnouncementValue { get; set; }

        public int ExtraPaymentGarbagemanValue { get; set; }

        public bool Blackout { get; set; }

        public DateTime? InactivePropertiesDate { get; set; }

        public int KeyValue { get; set; }

        public int LockValue { get; set; }

        public string IPLsJSON { get; set; }

        public int TattooValue { get; set; }

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