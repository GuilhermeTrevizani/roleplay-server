using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string DiscordId { get; set; }

        public string DiscordUsername { get; set; }

        public string DiscordDiscriminator { get; set; }

        [NotMapped]
        public string Name => $"{DiscordUsername}#{DiscordDiscriminator}";

        public string RegisterIp { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;

        public ulong RegisterHardwareIdHash { get; set; }

        public ulong RegisterHardwareIdExHash { get; set; }

        public string LastAccessIp { get; set; }

        public DateTime LastAccessDate { get; set; } = DateTime.Now;

        public ulong LastAccessHardwareIdHash { get; set; }

        public ulong LastAccessHardwareIdExHash { get; set; }

        public UserStaff Staff { get; set; } = UserStaff.None;

        public int NameChanges { get; set; }

        public int HelpRequestsAnswersQuantity { get; set; }

        public int StaffDutyTime { get; set; }

        public bool TimeStampToggle { get; set; } = true;

        public UserVIP VIP { get; set; } = UserVIP.None;

        public DateTime? VIPValidDate { get; set; }

        public int ForumNameChanges { get; set; }

        public bool PMToggle { get; set; }

        public bool StaffChatToggle { get; set; }

        public bool FactionChatToggle { get; set; }

        public int PlateChanges { get; set; }

        public bool AnnouncementToggle { get; set; }

        public bool VehicleTagToggle { get; set; }

        public int ChatFontType { get; set; }

        public int ChatLines { get; set; } = 10;

        public int ChatFontSize { get; set; } = 14;

        public string StaffFlagsJSON { get; set; } = "[]";

        public bool FactionToggle { get; set; }

        public int CharacterApplicationsQuantity { get; set; }

        [JsonIgnore]
        public ICollection<Character> Characters { get; set; }

        [JsonIgnore]
        public ICollection<Character> EvaluatingCharacters { get; set; }

        [JsonIgnore]
        public ICollection<Character> EvaluatorCharacters { get; set; }

        public DateTime? CooldownDismantle { get; set; }

        public DateTime? PropertyRobberyCooldown { get; set; }

        public bool AnsweredQuestions { get; set; }
    }
}