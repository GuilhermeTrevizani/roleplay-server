using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class Usuario
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public long SocialClubRegistro { get; set; } = 0;
        public long SocialClubUltimoAcesso { get; set; } = 0;
        public string IPRegistro { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public long HardwareIdHashRegistro { get; set; } = 0;
        public long HardwareIdExHashRegistro { get; set; } = 0;
        public string IPUltimoAcesso { get; set; } = string.Empty;
        public DateTime DataUltimoAcesso { get; set; } = DateTime.Now;
        public long HardwareIdHashUltimoAcesso { get; set; } = 0;
        public long HardwareIdExHashUltimoAcesso { get; set; } = 0;
        public TipoStaff Staff { get; set; } = 0;
        public bool PossuiNamechange { get; set; } = false;
        public int QuantidadeSOSAceitos { get; set; } = 0;
        public int TempoTrabalhoAdministrativo { get; set; } = 0;
        public bool TimeStamp { get; set; } = true;
    }
}