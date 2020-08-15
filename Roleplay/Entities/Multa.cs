using System;

namespace Roleplay.Entities
{
    public class Multa
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int PersonagemMultado { get; set; } = 0;
        public int PersonagemPolicial { get; set; } = 0;
        public int Valor { get; set; } = 0;
        public string Motivo { get; set; } = string.Empty;
        public DateTime? DataPagamento { get; set; } = null;
    }
}