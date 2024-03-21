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

        public void Create(string name, DateTime birthdayDate, string history, CharacterSex sex,
            Guid userId, string ip, uint model, ulong hardwareIdHash, ulong hardwareIdExHash, ushort health, Guid? evaluatorStaffUserId)
        {
            Name = name;
            BirthdayDate = birthdayDate;
            History = history;
            Sex = sex;
            UserId = userId;
            RegisterIp = LastAccessIp = ip;
            Model = model;
            RegisterHardwareIdHash = LastAccessHardwareIdHash = hardwareIdHash;
            RegisterHardwareIdExHash = LastAccessHardwareIdExHash = hardwareIdExHash;
            Health = health;
            EvaluatorStaffUserId = evaluatorStaffUserId;
        }

        public void Update(uint model, float posX, float posY, float posZ, ushort health, ushort armor, int dimension,
            string iplsJSON, string personalizationJSON, string woundsJSON, string factionFlagsJSON)
        {
            Model = model;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Health = health;
            Armor = armor;
            Dimension = dimension;
            IPLsJSON = iplsJSON;
            PersonalizationJSON = personalizationJSON;
            WoundsJSON = woundsJSON;
            FactionFlagsJSON = factionFlagsJSON;
            SetLastAccessDate();
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
            ResetExtraPayment();
        }

        public void ResetExtraPayment()
        {
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

        public void SetWound(CharacterWound wound)
        {
            Wound = wound;
        }

        public void SetDeath(DateTime date, string reason)
        {
            DeathDate = date;
            DeathReason = reason;
        }

        public void SetFaction(Guid factionId, Guid factionRankId, bool criminal)
        {
            FactionId = factionId;
            FactionRankId = factionRankId;
            if (!criminal)
                Job = CharacterJob.None;
        }

        public void UpdateFaction(Guid factionRankId, string factionFlagsJSON, int badge)
        {
            FactionRankId = factionRankId;
            FactionFlagsJSON = factionFlagsJSON;
            Badge = badge;
        }

        public void UseDrug(ItemCategory itemCategory, int quantity)
        {
            var minutesDuration = 0;
            var thresoldDeath = 0;

            switch (itemCategory)
            {
                case ItemCategory.Weed:
                    minutesDuration = 10;
                    break;
                case ItemCategory.Cocaine:
                    minutesDuration = 3;
                    thresoldDeath = 10;
                    break;
                case ItemCategory.Crack:
                    minutesDuration = 1;
                    thresoldDeath = 25;
                    break;
                case ItemCategory.Heroin:
                    minutesDuration = 10;
                    thresoldDeath = 25;
                    break;
                case ItemCategory.MDMA:
                    minutesDuration = 10;
                    thresoldDeath = 5;
                    break;
                case ItemCategory.Xanax:
                    minutesDuration = 5;
                    thresoldDeath = 1;
                    break;
                case ItemCategory.Oxycontin:
                    minutesDuration = 3;
                    thresoldDeath = 10;
                    break;
                case ItemCategory.Metanfetamina:
                    minutesDuration = 2;
                    thresoldDeath = 50;
                    break;
            }

            minutesDuration *= quantity;
            thresoldDeath *= quantity;

            var newThresoldDeath = ThresoldDeath + thresoldDeath;
            ThresoldDeath = Convert.ToByte(newThresoldDeath > 100 ? 100 : newThresoldDeath);
            DrugItemCategory = itemCategory;
            DrugEndDate = (ThresoldDeath == 100 ? DateTime.Now : DrugEndDate ?? DateTime.Now).AddMinutes(minutesDuration);
            ThresoldDeathEndDate = null;
        }

        public void ClearDrug()
        {
            DrugItemCategory = null;
            DrugEndDate = null;
            ThresoldDeath = 0;
            ThresoldDeathEndDate = null;
        }

        public void SetThresoldDeathEndDate()
        {
            DrugItemCategory = null;
            DrugEndDate = null;
            ThresoldDeathEndDate = DateTime.Now.AddHours(1);
        }

        public void AddBank(int value)
        {
            Bank += value;
        }

        public void RemoveBank(int value)
        {
            Bank -= value;
        }

        public void SetAnnouncementLastUseDate()
        {
            AnnouncementLastUseDate = DateTime.Now;
        }

        public void SetMask(ulong mask)
        {
            Mask = mask;
        }

        public void ResetFaction()
        {
            FactionId = FactionRankId = null;
            Badge = 0;
            FactionFlagsJSON = "[]";
            Armor = 0;
        }

        public void SetLastAccessDate()
        {
            LastAccessDate = DateTime.Now;
        }

        public void SetSavings(int savings)
        {
            Savings = savings;
        }

        public void AddConnectedTime()
        {
            ConnectedTime++;
        }

        public void SetBankAndSavings(int bank, int savings)
        {
            Bank = bank;
            Savings = savings;
        }

        public void SetNameChangeStatus(CharacterNameChangeStatus status)
        {
            NameChangeStatus = status;
        }

        public void UpdateLastAccess(string ip, ulong hardwareIdHash, ulong hardwareIdExHash)
        {
            LastAccessIp = ip;
            LastAccessHardwareIdHash = hardwareIdHash;
            LastAccessHardwareIdExHash = hardwareIdExHash;
            JailFinalDate = null;
            Mask = 0;
        }

        public void SetPoliceOfficerBlockedDriverLicenseCharacterId(Guid id)
        {
            PoliceOfficerBlockedDriverLicenseCharacterId = id;
        }
    }
}