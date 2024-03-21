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

        public void Create(string discordId, string discordUsername, string discordDisplayName, string ip, ulong hardwareIdHash, ulong hardwareIdExHash,
            UserStaff staff, string staffFlagsJson)
        {
            DiscordId = discordId;
            DiscordUsername = discordUsername;
            DiscordDisplayName = discordDisplayName;
            RegisterIp = LastAccessIp = ip;
            RegisterHardwareIdHash = LastAccessHardwareIdHash = hardwareIdHash;
            RegisterHardwareIdExHash = LastAccessHardwareIdExHash = hardwareIdExHash;
            Staff = staff;
            StaffFlagsJSON = staffFlagsJson;
        }

        public void UpdateLastAccess(string ip, ulong hardwareIdHash, ulong hardwareIdExHash, string discordUsername, string discordDisplayName)
        {
            LastAccessDate = DateTime.Now;
            LastAccessIp = ip;
            LastAccessHardwareIdHash = hardwareIdHash;
            LastAccessHardwareIdExHash = hardwareIdExHash;
            DiscordUsername = discordUsername;
            DiscordDisplayName = discordDisplayName;
        }

        public void SetVIP(UserVIP vip, int months)
        {
            VIP = vip;
            VIPValidDate = (VIPValidDate > DateTime.Now && VIP == vip ? VIPValidDate.Value : DateTime.Now).AddMonths(months);
            NameChanges += vip switch
            {
                UserVIP.Gold => 4,
                UserVIP.Silver => 3,
                _ => 2,
            };

            ForumNameChanges += vip switch
            {
                UserVIP.Gold => 2,
                _ => 1,
            };

            PlateChanges += vip switch
            {
                UserVIP.Gold => 2,
                UserVIP.Silver => 1,
                _ => 0,
            };
        }

        public void AddCharacterApplicationsQuantity()
        {
            CharacterApplicationsQuantity++;
        }

        public void AddHelpRequestsAnswersQuantity()
        {
            HelpRequestsAnswersQuantity++;
        }

        public void SetVehicleTagToggle(bool value)
        {
            VehicleTagToggle = value;
        }

        public void SetLastAccessDate()
        {
            LastAccessDate = DateTime.Now;
        }

        public void SetPMToggle(bool value)
        {
            PMToggle = value;
        }

        public void AddStaffDutyTime()
        {
            StaffDutyTime++;
        }

        public void SetAnsweredQuestions()
        {
            AnsweredQuestions = true;
        }

        public void RemoveNameChange()
        {
            NameChanges--;
        }

        public void UpdateSettings(bool timeStampToggle, bool vehicleTagToggle, bool announcementToggle, bool pmToggle,
            bool factionChatToggle, bool staffChatToggle, int chatFontType, int chatLines, int chatFontSize, bool factionToggle)
        {
            TimeStampToggle = timeStampToggle;
            VehicleTagToggle = vehicleTagToggle;
            AnnouncementToggle = announcementToggle;
            PMToggle = pmToggle;
            FactionChatToggle = factionChatToggle;
            StaffChatToggle = staffChatToggle;
            ChatFontType = chatFontType;
            ChatLines = chatLines;
            ChatFontSize = chatFontSize;
            FactionToggle = factionToggle;
        }

        public void RemoveForumNameChange()
        {
            ForumNameChanges--;
        }

        public void SetStaff(UserStaff staff, string staffFlagsJson)
        {
            Staff = staff;
            StaffFlagsJSON = staffFlagsJson;
        }

        public void RemovePlateChanges()
        {
            PlateChanges--;
        }

        public void SetCooldownDismantle(DateTime date)
        {
            CooldownDismantle = date;
        }

        public void SetPropertyRobberyCooldown(DateTime date)
        {
            PropertyRobberyCooldown = date;
        }
    }
}