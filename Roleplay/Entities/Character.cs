using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Character
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;

        public string RegisterIp { get; set; }

        public ulong RegisterHardwareIdHash { get; set; }

        public ulong RegisterHardwareIdExHash { get; set; }

        public DateTime LastAccessDate { get; set; } = DateTime.Now;

        public string LastAccessIp { get; set; }

        public ulong LastAccessHardwareIdHash { get; set; }

        public ulong LastAccessHardwareIdExHash { get; set; }

        public PedModel Model { get; set; }

        public float PosX { get; set; } = 265.66153f;

        public float PosY { get; set; } = -1205.723f;

        public float PosZ { get; set; } = 29.279907f;

        public ushort Health { get; set; } = 100;

        public ushort Armor { get; set; }

        public int Dimension { get; set; }

        public DateTime BirthdayDate { get; set; }

        public int ConnectedTime { get; set; }

        public int? FactionId { get; set; }

        public int? FactionRankId { get; set; }

        [JsonIgnore]
        public FactionRank FactionRank { get; set; }

        public int Bank { get; set; } = 3500;

        public string IPLsJSON { get; set; } = "[]";

        public DateTime? DeathDate { get; set; }

        public string DeathReason { get; set; } = string.Empty;

        public CharacterJob Job { get; set; } = CharacterJob.None;

        public string PersonalizationJSON { get; set; }

        public string History { get; set; }

        public int? EvaluatingStaffUserId { get; set; }

        [JsonIgnore]
        public User EvaluatingStaffUser { get; set; }

        public int? EvaluatorStaffUserId { get; set; }

        [JsonIgnore]
        public User EvaluatorStaffUser { get; set; }

        public string RejectionReason { get; set; } = string.Empty;

        public CharacterNameChangeStatus NameChangeStatus { get; set; } = CharacterNameChangeStatus.Liberado;

        public CharacterPersonalizationStep PersonalizationStep { get; set; } = CharacterPersonalizationStep.Character;

        public DateTime? DeletedDate { get; set; }

        public DateTime? JailFinalDate { get; set; }

        public DateTime? DriverLicenseValidDate { get; set; }

        public int? PoliceOfficerBlockedDriverLicenseCharacterId { get; set; }

        public Character PoliceOfficerBlockedDriverLicenseCharacter { get; set; }

        public int Badge { get; set; }

        public DateTime? AnnouncementLastUseDate { get; set; }

        public int Savings { get; set; }

        public int ExtraPayment { get; set; }

        public string WoundsJSON { get; set; } = "[]";

        public CharacterWound Wound { get; set; }

        public CharacterSex Sex { get; set; }

        public string Image { get; set; }

        public ulong Mask { get; set; }

        public string FactionFlagsJSON { get; set; } = "[]";

        public ItemCategory? DrugItemCategory { get; set; }

        public DateTime? DrugEndDate { get; set; }

        public byte ThresoldDeath { get; set; }

        public DateTime? ThresoldDeathEndDate { get; set; }

        public bool CKAvaliation { get; set; }
    }
}