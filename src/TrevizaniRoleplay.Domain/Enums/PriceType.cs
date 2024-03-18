using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum PriceType : byte
    {
        [Display(Name = Globalization.JOB_VEHICLE_RENTAL)]
        JobVehicleRental = 1,

        [Display(Name = Globalization.CONVENIENCE_STORE)]
        ConvenienceStore = 2,

        [Display(Name = Globalization.BOATS)]
        Boats = 3,

        [Display(Name = Globalization.HELICOPTERS)]
        Helicopters = 4,

        [Display(Name = Globalization.CARS)]
        Cars = 5,

        [Display(Name = Globalization.AIRPLANES)]
        Airplanes = 6,

        [Display(Name = Globalization.WEAPONS)]
        Weapons = 7,

        [Display(Name = Globalization.JOBS)]
        Jobs = 8,

        [Display(Name = Globalization.MOTORCYCLES_AND_BICYCLES)]
        MotorcyclesAndBicycles = 9,

        [Display(Name = Globalization.DRUGS)]
        Drugs = 10,

        [Display(Name = Globalization.TUNING)]
        Tuning = 11,
    }
}