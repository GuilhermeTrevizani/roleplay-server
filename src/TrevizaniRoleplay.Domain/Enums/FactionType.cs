using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum FactionType : byte
    {
        [Display(Name = Globalization.POLICE)]
        Police = 1,

        [Display(Name = Globalization.FIREFIGHTER)]
        Firefighter = 2,

        [Display(Name = Globalization.CRIMINAL)]
        Criminal = 3,
    }
}