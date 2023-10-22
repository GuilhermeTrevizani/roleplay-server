using Roleplay.Models;

namespace Roleplay.Entities
{
    public class FactionDrugHouseItem
    {
        public int Id { get; set; }

        public int FactionDrugHouseId { get; set; }

        public ItemCategory ItemCategory { get; set; }

        public int Quantity { get; set; }
    }
}