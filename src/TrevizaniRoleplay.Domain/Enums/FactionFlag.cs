using System.ComponentModel.DataAnnotations;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Domain.Enums
{
    public enum FactionFlag : byte
    {
        [Display(Name = Globalization.INVITE_MEMBER)]
        InviteMember = 1,

        [Display(Name = Globalization.EDIT_MEMBER)]
        EditMember = 2,

        [Display(Name = Globalization.REMOVE_MEMBER)]
        RemoveMember = 3,

        [Display(Name = Globalization.BLOCK_CHAT)]
        BlockChat = 4,

        [Display(Name = Globalization.GOVERNMENT_ADVERTISEMENT)]
        GovernmentAdvertisement = 5,

        [Display(Name = Globalization.HQ)]
        HQ = 6,

        [Display(Name = Globalization.STORAGE)]
        Storage = 7,

        [Display(Name = Globalization.REMOVE_ALL_BARRIERS)]
        RemoveAllBarriers = 8,
    }
}