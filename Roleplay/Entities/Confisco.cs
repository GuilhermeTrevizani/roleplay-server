using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Confisco
    {
        public int Codigo { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [ForeignKey("PersonagemBD")]
        public int Personagem { get; set; }
        public virtual Personagem PersonagemBD { get; set; }

        [ForeignKey("PersonagemPolicialBD")]
        public int PersonagemPolicial { get; set; }
        public virtual Personagem PersonagemPolicialBD { get; set; }

        public string Armas { get; set; }

        public string Descricao { get; set; } = string.Empty;
    }
}