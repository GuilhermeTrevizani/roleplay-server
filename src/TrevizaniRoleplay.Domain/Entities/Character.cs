using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Character : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime RegisterDate { get; private set; } = DateTime.Now;
        public string RegisterIp { get; private set; } = string.Empty;
        public ulong RegisterHardwareIdHash { get; private set; }
        public ulong RegisterHardwareIdExHash { get; private set; }
        public DateTime LastAccessDate { get; private set; } = DateTime.Now;
        public string? LastAccessIp { get; private set; }
        public ulong LastAccessHardwareIdHash { get; private set; }
        public ulong LastAccessHardwareIdExHash { get; private set; }
        public uint Model { get; private set; }
        public float PosX { get; private set; } = 265.66153f;
        public float PosY { get; private set; } = -1205.723f;
        public float PosZ { get; private set; } = 29.279907f;
        public ushort Health { get; private set; } = 100;
        public ushort Armor { get; private set; }
        public int Dimension { get; private set; }
        public DateTime BirthdayDate { get; private set; }
        public int ConnectedTime { get; private set; }
        public Guid? FactionId { get; private set; }
        public Guid? FactionRankId { get; private set; }
        public int Bank { get; private set; } = 3500;
        public string IPLsJSON { get; private set; } = "[]";
        public DateTime? DeathDate { get; private set; }
        public string DeathReason { get; private set; } = string.Empty;
        public CharacterJob Job { get; private set; } = CharacterJob.None;
        public string? PersonalizationJSON { get; private set; }
        public string History { get; private set; } = string.Empty;
        public Guid? EvaluatingStaffUserId { get; private set; }
        public Guid? EvaluatorStaffUserId { get; private set; }
        public string RejectionReason { get; private set; } = string.Empty;
        public CharacterNameChangeStatus NameChangeStatus { get; private set; } = CharacterNameChangeStatus.Allowed;
        public CharacterPersonalizationStep PersonalizationStep { get; private set; } = CharacterPersonalizationStep.Character;
        public DateTime? DeletedDate { get; private set; }
        public DateTime? JailFinalDate { get; private set; }
        public DateTime? DriverLicenseValidDate { get; private set; }
        public Guid? PoliceOfficerBlockedDriverLicenseCharacterId { get; private set; }
        public int Badge { get; private set; }
        public DateTime? AnnouncementLastUseDate { get; private set; }
        public int Savings { get; private set; }
        public int ExtraPayment { get; private set; }
        public string WoundsJSON { get; private set; } = "[]";
        public CharacterWound Wound { get; private set; } = CharacterWound.None;
        public CharacterSex Sex { get; private set; }
        public string? Image { get; private set; }
        public ulong Mask { get; private set; }
        public string FactionFlagsJSON { get; private set; } = "[]";
        public ItemCategory? DrugItemCategory { get; private set; }
        public DateTime? DrugEndDate { get; private set; }
        public byte ThresoldDeath { get; private set; }
        public DateTime? ThresoldDeathEndDate { get; private set; }
        public bool CKAvaliation { get; private set; }

        public User? User { get; private set; }
        public Faction? Faction { get; private set; }
        public FactionRank? FactionRank { get; private set; }
        public User? EvaluatingStaffUser { get; private set; }
        public User? EvaluatorStaffUser { get; private set; }
        public Character? PoliceOfficerBlockedDriverLicenseCharacter { get; private set; }

        public void Create(string name)
        {
            Name = name;
        }

        public void Delete()
        {
            DeletedDate = DateTime.Now;
        }

        public void SetJob(CharacterJob job)
        {
            Job = job;
        }

        public void QuitJob()
        {
            Job = CharacterJob.None;
            ExtraPayment = 0;
        }

        public void AddExtraPayment(int extraPayment)
        {
            ExtraPayment += extraPayment;
        }

        public void SetEvaluatingStaffUser(Guid userId)
        {
            EvaluatingStaffUserId = userId;
        }

        public void AcceptAplication(Guid userId)
        {
            EvaluatorStaffUserId = userId;
            EvaluatingStaffUserId = null;
        }

        public void RejectApplication(Guid userId, string reason)
        {
            EvaluatorStaffUserId = userId;
            EvaluatingStaffUserId = null;
            RejectionReason = reason;
        }

        public void SetJailFinalDate(DateTime date)
        {
            JailFinalDate = date;
        }
    }
}