using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum ItemCategory : byte
    {
        [Display(Name = Globalization.WEAPON)]
        Weapon = 1,

        [Display(Name = Globalization.PROPERTY_KEY)]
        PropertyKey = 2,

        [Display(Name = Globalization.MASK)]
        Cloth1 = 3,

        [Display(Name = Globalization.TORSO)]
        Cloth3 = 4,

        [Display(Name = Globalization.PANTS)]
        Cloth4 = 5,

        [Display(Name = Globalization.BACKPACK)]
        Cloth5 = 6,

        [Display(Name = Globalization.SHOES)]
        Cloth6 = 7,

        [Display(Name = Globalization.EXTRA)]
        Cloth7 = 8,

        [Display(Name = Globalization.SHIRT)]
        Cloth8 = 9,

        [Display(Name = Globalization.VEST)]
        Cloth9 = 10,

        [Display(Name = Globalization.DECAL)]
        Cloth10 = 11,

        [Display(Name = Globalization.JACKET)]
        Cloth11 = 12,

        [Display(Name = Globalization.HAT)]
        Accessory0 = 13,

        [Display(Name = Globalization.GLASSES)]
        Accessory1 = 14,

        [Display(Name = Globalization.EAR)]
        Accessory2 = 15,

        [Display(Name = Globalization.WATCH)]
        Accessory6 = 16,

        [Display(Name = Globalization.BRACELET)]
        Accessory7 = 17,

        [Display(Name = Globalization.MONEY)]
        Money = 18,

        [Display(Name = Globalization.MICROPHONE)]
        Microphone = 19,

        [Display(Name = Globalization.VEHICLE_KEY)]
        VehicleKey = 20,

        [Display(Name = Globalization.WALKIE_TALKIE)]
        WalkieTalkie = 21,

        [Display(Name = Globalization.CELLPHONE)]
        Cellphone = 22,

        [Display(Name = Globalization.WEED)]
        Weed = 23,

        [Display(Name = Globalization.COCAINE)]
        Cocaine = 24,

        [Display(Name = Globalization.CRACK)]
        Crack = 25,

        [Display(Name = Globalization.HEROIN)]
        Heroin = 26,

        [Display(Name = Globalization.MDMA)]
        MDMA = 27,

        [Display(Name = Globalization.XANAX)]
        Xanax = 28,

        [Display(Name = Globalization.OXYCONTIN)]
        Oxycontin = 29,

        [Display(Name = Globalization.METANFETAMINA)]
        Metanfetamina = 30,

        [Display(Name = Globalization.BOOMBOX)]
        Boombox = 31,
    }
}