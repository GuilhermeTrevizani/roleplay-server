using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class Session
    {
        public ulong Id { get; set; }

        public int CharacterId { get; set; }

        public SessionType Type { get; set; }

        public DateTime InitialDate { get; set; } = DateTime.Now;

        public DateTime? FinalDate { get; set; }
    }
}