using AltV.Net.Data;

namespace Roleplay.Models
{
    public class Spotlight
    {
        public int Id { get; set; }

        public Position Position { get; set; }

        public Position Direction { get; set; }

        public int Player { get; set; }

        public float Distance { get; set; }

        public float Brightness { get; set; }

        public float Hardness { get; set; }

        public float Radius { get; set; }

        public float Falloff { get; set; }
    }
}