using Roleplay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Ligacao911
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public TipoFaccao Tipo { get; set; }
        public int Celular { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public string Mensagem { get; set; }

        [NotMapped]
        public int ID { get; set; }
    }
}