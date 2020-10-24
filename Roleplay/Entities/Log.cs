using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class Log
    {
        public long Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public TipoLog Tipo { get; set; }
        public string Descricao { get; set; }
        public int PersonagemOrigem { get; set; }
        public int PersonagemDestino { get; set; }
        public long SocialClubOrigem { get; set; }
        public long SocialClubDestino { get; set; }
        public string IPOrigem { get; set; }
        public string IPDestino { get; set; }
        public long HardwareIdHashOrigem { get; set; } = 0;
        public long HardwareIdHashDestino { get; set; } = 0;
        public long HardwareIdExHashOrigem { get; set; } = 0;
        public long HardwareIdExHashDestino { get; set; } = 0;
    }

    public class LogInfo
    {
        public long Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public TipoLog Tipo { get; set; }
        public string Descricao { get; set; }
        public int PersonagemOrigem { get; set; }
        public int PersonagemDestino { get; set; }
        public long SocialClubOrigem { get; set; }
        public long SocialClubDestino { get; set; }
        public string IPOrigem { get; set; }
        public string IPDestino { get; set; }
        public long HardwareIdHashOrigem { get; set; } = 0;
        public long HardwareIdHashDestino { get; set; } = 0;
        public long HardwareIdExHashOrigem { get; set; } = 0;
        public long HardwareIdExHashDestino { get; set; } = 0;
        public string NomePersonagemOrigem { get; set; }
        public string NomePersonagemDestino { get; set; }
        public string UsuarioOrigem { get; set; }
        public string UsuarioDestino { get; set; }
    }
}