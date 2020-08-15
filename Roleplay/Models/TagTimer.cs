using System.Timers;

namespace Roleplay.Models
{
    public class TagTimer : Timer
    {
        public TagTimer(double interval) : base(interval) { }

        public object Tag { get; set; }
        public int ElapsedCount { get; set; }
    }
}