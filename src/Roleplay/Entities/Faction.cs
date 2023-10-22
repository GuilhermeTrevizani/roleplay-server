using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Faction
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public FactionType Type { get; set; }

        public string Color { get; set; }

        public int Slots { get; set; }

        public string ChatColor { get; set; }

        [NotMapped, JsonIgnore]
        public bool BlockedChat { get; set; }

        [NotMapped, JsonIgnore]
        public bool Government { get => Type == FactionType.Police || Type == FactionType.Firefighter || Type == FactionType.Coroner; }

        public List<FactionFlag> GetFlags()
        {
            var flags = Enum.GetValues(typeof(FactionFlag)).Cast<FactionFlag>().ToList();

            if (!Government)
                flags.RemoveAll(x => x == FactionFlag.GovernmentAnnouncement || x == FactionFlag.HQ || x == FactionFlag.RemoveAllBarriers);

            if (Type != FactionType.Criminal)
                flags.RemoveAll(x => x == FactionFlag.DrugHouse);

            return flags;
        }
    }
}