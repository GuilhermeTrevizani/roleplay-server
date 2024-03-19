using System.ComponentModel.DataAnnotations;

namespace TrevizaniRoleplay.Server.Models
{
    public enum StaffFlag : byte
    {
        [Display(Name = "Desbanimento")]
        Unban = 1,

        [Display(Name = "Portas")]
        Doors = 2,

        [Display(Name = "Preços")]
        Prices = 3,

        [Display(Name = "Facções")]
        Factions = 4,

        [Display(Name = "Armazenamentos de Facções")]
        FactionsStorages = 5,

        [Display(Name = "Propriedades")]
        Properties = 6,

        [Display(Name = "Pontos")]
        Spots = 7,

        [Display(Name = "Blips")]
        Blips = 8,

        [Display(Name = "Veículos")]
        Vehicles = 9,

        [Display(Name = "CK")]
        CK = 10,

        [Display(Name = "Dar Item")]
        GiveItem = 11,

        [Display(Name = "Bocas de Fumo")]
        CrackDens = 12,

        [Display(Name = "Localizações de Caminhoneiro")]
        TruckerLocations = 13,

        [Display(Name = "Mobílias")]
        Furnitures = 14,

        [Display(Name = "Animações")]
        Animations = 15,

        [Display(Name = "Empresas")]
        Companies = 16,
    }
}