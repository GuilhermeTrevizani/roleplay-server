using System;

namespace Roleplay.Models
{
    public class Ferimento
    {
        public DateTime Data { get; set; } = DateTime.Now;
        public ushort Dano { get; set; }
        public uint Arma { get; set; }
        public int CodigoAttacker { get; set; }
        public sbyte BodyPart { get; set; } = -2;
    }
}