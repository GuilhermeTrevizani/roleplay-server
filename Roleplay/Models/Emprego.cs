using AltV.Net.Data;
using AltV.Net.Enums;

namespace Roleplay.Models
{
    public class Emprego
    {
        public TipoEmprego Tipo { get; set; }
        public Position Posicao { get; set; }
        public Position PosicaoAluguel { get; set; }
        public Rotation RotacaoAluguel { get; set; }
        public VehicleModel Veiculo { get; set; }
        public Rgba CorVeiculo { get; set; }
    }
}