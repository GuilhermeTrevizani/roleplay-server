using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Log : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public LogType Type { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Guid? OriginCharacterId { get; private set; }
        public string? OriginIp { get; private set; }
        public ulong OriginHardwareIdHash { get; private set; }
        public ulong OriginHardwareIdExHash { get; private set; }
        public Guid? TargetCharacterId { get; private set; }
        public string? TargetIp { get; private set; }
        public ulong TargetHardwareIdHash { get; private set; }
        public ulong TargetHardwareIdExHash { get; private set; }

        public Character? OriginCharacter { get; private set; }
        public Character? TargetCharacter { get; private set; }

        public void Create(LogType type, string description)
        {
            Type = type;
            Description = description;
        }

        public void Create(LogType type, string description,
            Guid? originCharacterId, string? originIp, ulong originHardwareIdHash, ulong originHardwareIdExHash,
            Guid? targetCharacterId, string? targetIp, ulong targetHardwareIdHash, ulong targetHardwareIdExHash)
        {
            Type = type;
            Description = description;
            OriginCharacterId = originCharacterId;
            OriginIp = originIp;
            OriginHardwareIdHash = originHardwareIdHash;
            OriginHardwareIdExHash = originHardwareIdExHash;
            TargetCharacterId = targetCharacterId;
            TargetIp = targetIp;
            TargetHardwareIdHash = targetHardwareIdHash;
            TargetHardwareIdExHash = targetHardwareIdExHash;
        }
    }
}