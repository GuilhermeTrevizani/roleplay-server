using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Server.Models
{
    public class ClotheAccessoryItem
    {
        public string? DLC { get; set; }
        public byte Texture { get; set; }
        public CharacterSex Sex { get; set; }
    }
}