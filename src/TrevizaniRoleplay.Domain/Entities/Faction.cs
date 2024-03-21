using System.ComponentModel.DataAnnotations.Schema;
using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class Faction : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public FactionType Type { get; private set; }
        public string Color { get; private set; } = string.Empty;
        public int Slots { get; private set; }
        public string ChatColor { get; private set; } = string.Empty;

        [NotMapped]
        public bool BlockedChat { get; private set; }

        [NotMapped]
        public bool Government => Type == FactionType.Police || Type == FactionType.Firefighter;

        public void Create(string name, FactionType factionType, string color, int slots, string chatColor)
        {
            Name = name;
            Type = factionType;
            Color = color;
            Slots = slots;
            ChatColor = chatColor;
        }

        public void Update(string name, FactionType factionType, string color, int slots, string chatColor)
        {
            Name = name;
            Type = factionType;
            Color = color;
            Slots = slots;
            ChatColor = chatColor;
        }

        public List<FactionFlag> GetFlags()
        {
            var flags = Enum.GetValues(typeof(FactionFlag)).Cast<FactionFlag>().ToList();

            if (!Government)
                flags.RemoveAll(x => x == FactionFlag.GovernmentAdvertisement || x == FactionFlag.HQ || x == FactionFlag.RemoveAllBarriers);

            return flags;
        }

        public void ToggleBlockedChat()
        {
            BlockedChat = !BlockedChat;
        }
    }
}