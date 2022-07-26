using System.Collections.Generic;

namespace Roleplay.Models
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public string DiscordBotToken { get; set; }

        public ulong AnnouncementDiscordChannel { get; set; }

        public ulong GovernmentAnnouncementDiscordChannel { get; set; }

        public ulong StaffDiscordChannel { get; set; }

        public string EmailHost { get; set; }

        public string EmailAddress { get; set; }

        public string EmailName { get; set; }

        public string EmailPassword { get; set; }

        public int EmailPort { get; set; }

        public string MySQLVersion { get; set; }

        public List<ulong> RolesStaffMessage { get; set; }

        public ulong CompanyAnnouncementDiscordChannel { get; set; }
    }
}