using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum SpotType : byte
    {
        [Display(Name = Globalization.BANK)]
        Bank = 1,

        [Display(Name = Globalization.CONVENIENCE_STORE)]
        ConvenienceStore = 2,

        [Display(Name = Globalization.CLTOHES_STORE)]
        ClothesStore = 3,

        [Display(Name = Globalization.FACTION_VEHICLE_SPAWN)]
        FactionVehicleSpawn = 4,

        [Display(Name = Globalization.VEHICLE_SEIZURE)]
        VehicleSeizure = 5,

        [Display(Name = Globalization.VEHICLE_RELEASE)]
        VehicleRelease = 6,

        [Display(Name = Globalization.BARBER_SHOP)]
        BarberShop = 7,

        [Display(Name = Globalization.UNIFORM)]
        Uniform = 8,

        [Display(Name = Globalization.MDC)]
        MDC = 9,

        [Display(Name = Globalization.DMV)]
        DMV = 10,

        [Display(Name = Globalization.ENTRANCE)]
        Entrance = 11,

        [Display(Name = Globalization.HEAL_ME)]
        HealMe = 12,

        [Display(Name = Globalization.PRISON)]
        Prison = 13,

        [Display(Name = Globalization.GARBAGE_COLLECTOR)]
        GarbageCollector = 14,

        [Display(Name = Globalization.LSPD_SERVICE)]
        LSPDService = 15,

        [Display(Name = Globalization.CONFISCATION)]
        Confiscation = 16,

        [Display(Name = Globalization.TATTOO_SHOP)]
        TattooShop = 17,

        [Display(Name = Globalization.MECHANIC_WORKSHOP)]
        MechanicWorkshop = 18,
    }
}