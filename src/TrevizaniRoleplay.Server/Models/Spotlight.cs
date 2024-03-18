﻿using AltV.Net.Data;

namespace TrevizaniRoleplay.Server.Models
{
    public class Spotlight
    {
        public uint Id { get; set; }
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