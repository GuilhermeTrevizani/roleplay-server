using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Faccao
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public TipoFaccao Tipo { get; set; }
        public string Cor { get; set; } = string.Empty;
        public int RankGestor { get; set; } = 0;
        public int RankLider { get; set; } = 0;
        public int Slots { get; set; } = 0;

        [NotMapped]
        public bool ChatBloqueado { get; set; } = false;
    }
}