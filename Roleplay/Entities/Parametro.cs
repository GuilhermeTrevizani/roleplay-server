namespace Roleplay.Entities
{
    public class Parametro
    {
        public int Codigo { get; set; }
        public int RecordeOnline { get; set; } = 0;
        public int ValorVagaVeiculo { get; set; } = 0;
        public int ValorCustosHospitalares { get; set; } = 0;
        public int ValorBarbearia { get; set; } = 0;
        public int ValorRoupas { get; set; } = 0;
        public int ValorLicencaMotorista { get; set; } = 0;
        public int ValorComponentes { get; set; } = 0;
        public int ValorCombustivel { get; set; } = 0;
        public int Paycheck { get; set; } = 1;
        public int ValorLicencaMotoristaRenovacao { get; set; } = 0;
        public int ValorAnuncio { get; set; } = 0;
    }
}