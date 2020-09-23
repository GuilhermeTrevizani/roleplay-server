using System;

namespace Roleplay.Entities
{
    public class Prisao
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Preso { get; set; } = 0;
        public int Policial { get; set; } = 0;
        public DateTime Termino { get; set; } = DateTime.MinValue;
    }
}