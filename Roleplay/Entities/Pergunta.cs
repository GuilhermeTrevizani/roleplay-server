using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Pergunta
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public int RespostaCorreta { get; set; }

        [NotMapped]
        public List<Resposta> Respostas { get; set; } = new List<Resposta>();

        [NotMapped]
        public int RespostaSelecionada { get; set; }
    }
}