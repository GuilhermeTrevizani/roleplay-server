using Roleplay.Models;

namespace Roleplay.Entities
{
    public class CrackDenItem
    {
        public int Id { get; set; }

        public int CrackDenId { get; set; }

        public ItemCategory ItemCategory { get; set; }

        public int Value { get; set; }
    }
}