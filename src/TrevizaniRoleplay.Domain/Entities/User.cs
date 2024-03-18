using System.ComponentModel.DataAnnotations.Schema;
using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class User : BaseEntity
    {
        public string DiscordId { get; private set; } = string.Empty;
        public string DiscordUsername { get; private set; } = string.Empty;
        public string DiscordDisplayName { get; private set; } = string.Empty;
        public string RegisterIp { get; private set; } = string.Empty;
        public DateTime RegisterDate { get; private set; } = DateTime.Now;
        public ulong RegisterHardwareIdHash { get; private set; }
        public ulong RegisterHardwareIdExHash { get; private set; }
        public string LastAccessIp { get; private set; } = string.Empty;
        public DateTime LastAccessDate { get; private set; } = DateTime.Now;
        public ulong LastAccessHardwareIdHash { get; private set; }
        public ulong LastAccessHardwareIdExHash { get; private set; }
        public UserStaff Staff { get; private set; } = UserStaff.None;
        public int NameChanges { get; private set; }
        public int HelpRequestsAnswersQuantity { get; private set; }
        public int StaffDutyTime { get; private set; }
        public bool TimeStampToggle { get; private set; } = true;
        public UserVIP VIP { get; private set; } = UserVIP.None;
        public DateTime? VIPValidDate { get; private set; }
        public int ForumNameChanges { get; private set; }
        public bool PMToggle { get; private set; }
        public bool StaffChatToggle { get; private set; }
        public bool FactionChatToggle { get; private set; }
        public int PlateChanges { get; private set; }
        public bool AnnouncementToggle { get; private set; }
        public bool VehicleTagToggle { get; private set; }
        public int ChatFontType { get; private set; }
        public int ChatLines { get; private set; } = 10;
        public int ChatFontSize { get; private set; } = 14;
        public string StaffFlagsJSON { get; private set; } = "[]";
        public bool FactionToggle { get; private set; }
        public int CharacterApplicationsQuantity { get; private set; }
        public DateTime? CooldownDismantle { get; private set; }
        public DateTime? PropertyRobberyCooldown { get; private set; }
        public bool AnsweredQuestions { get; private set; }

        public ICollection<Character>? Characters { get; private set; }

        [NotMapped]
        public string Name => string.IsNullOrWhiteSpace(DiscordDisplayName) ? DiscordUsername : DiscordDisplayName;
    }
}