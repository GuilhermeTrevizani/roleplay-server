using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum UserStaff : byte
    {
        [Display(Name = Globalization.NONE)]
        None = 1,

        [Display(Name = Globalization.MODERATOR)]
        Moderator = 2,

        [Display(Name = Globalization.GAME_ADMINISTRATOR)]
        GameAdministrator = 3,

        [Display(Name = Globalization.LEAD_ADMINISTRATOR)]
        LeadAdministrator = 100,

        [Display(Name = Globalization.HEAD_ADMINISTRATOR)]
        HeadAdministrator = 200,

        [Display(Name = Globalization.MANAGER)]
        Manager = 254,

        [Display(Name = Globalization.FOUNDER)]
        Founder = 255,
    }
}