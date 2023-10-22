using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Log
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Date { get; set; } = DateTime.Now;

        public LogType Type { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(OriginCharacter))]
        public int? OriginCharacterId { get; set; }
        public Character OriginCharacter { get; set; }

        public string OriginIp { get; set; } = string.Empty;

        public ulong OriginHardwareIdHash { get; set; }

        public ulong OriginHardwareIdExHash { get; set; }

        [ForeignKey(nameof(TargetCharacter))]
        public int? TargetCharacterId { get; set; }
        public Character TargetCharacter { get; set; }

        public string TargetIp { get; set; } = string.Empty;

        public ulong TargetHardwareIdHash { get; set; }

        public ulong TargetHardwareIdExHash { get; set; }
    }
}