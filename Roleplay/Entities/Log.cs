using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class Log
    {
        public long Codigo { get; set; }
        public DateTime Data { get; set; }
        public TipoLog Tipo { get; set; }
        public string Descricao { get; set; }
        public int PersonagemOrigem { get; set; }
        public int PersonagemDestino { get; set; }
        public long SocialClubOrigem { get; set; }
        public long SocialClubDestino { get; set; }
        public string IPOrigem { get; set; }
        public string IPDestino { get; set; }
    }
}