using System;

namespace Roleplay.Entities
{
    public class Unidade
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public DateTime Inicio { get; set; } = DateTime.Now;
        public string Policiais { get; set; }
        public DateTime? Termino { get; set; } = null;
    }
}