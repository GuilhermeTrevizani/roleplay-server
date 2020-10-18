using System;

namespace Roleplay.Entities
{
    public class Apreensao
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Veiculo { get; set; }
        public int PersonagemPolicial { get; set; }
        public int Valor { get; set; }
        public string Motivo { get; set; }
        public DateTime? DataPagamento { get; set; } = null;
        public int Faccao { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}