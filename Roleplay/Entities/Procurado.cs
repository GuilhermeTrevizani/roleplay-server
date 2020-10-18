using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Procurado
    {
        public int Codigo { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [ForeignKey("PersonagemPolicialBD")]
        public int PersonagemPolicial { get; set; }
        public virtual Personagem PersonagemPolicialBD { get; set; }

        public int Personagem { get; set; } = 0;

        public int Veiculo { get; set; } = 0;

        public string Motivo { get; set; }

        public DateTime? DataExclusao { get; set; } = null;

        public int PersonagemPolicialExclusao { get; set; } = 0;
    }

    public class ProcuradoInfo
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int PersonagemPolicial { get; set; }
        public string NomePersonagemPolicial { get; set; }
        public int Personagem { get; set; } = 0;
        public string NomePersonagem { get; set; }
        public int Veiculo { get; set; } = 0;
        public string ModeloVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Motivo { get; set; }
        public DateTime? DataExclusao { get; set; } = null;
        public int PersonagemPolicialExclusao { get; set; } = 0;
    } 
}