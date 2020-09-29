using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class Punicao
    {
        public int Codigo { get; set; }
        public TipoPunicao Tipo { get; set; }
        public int Duracao { get; set; }
        public DateTime Data { get; set; }
        public int Personagem { get; set; }
        public string Motivo { get; set; }
        public int UsuarioStaff { get; set; }
    }

    public class PunicaoAdministrativa
    {
        public int Codigo { get; set; }
        public TipoPunicao Tipo { get; set; }
        public int Duracao { get; set; }
        public DateTime Data { get; set; }
        public int Personagem { get; set; }
        public string Motivo { get; set; }
        public int UsuarioStaff { get; set; }

        public string NomePersonagem { get; set; }
        public string NomeUsuarioStaff { get; set; }
    }
}