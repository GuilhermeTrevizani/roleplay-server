using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Prisao
    {
        public int Codigo { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [ForeignKey("PresoBD")]
        public int Preso { get; set; }
        public virtual Personagem PresoBD { get; set; }

        [ForeignKey("PolicialBD")]
        public int Policial { get; set; }
        public virtual Personagem PolicialBD { get; set; }

        public DateTime Termino { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public string Crimes { get; set; } = string.Empty;
    }
}