using AltV.Net.Data;

namespace Roleplay.Models
{
    public class Concessionaria
    {
        public string Nome { get; set; }
        public TipoPreco Tipo { get; set; }
        public Position PosicaoCompra { get; set; }
        public Position PosicaoSpawn { get; set; }
        public Position RotacaoSpawn { get; set; }
    }
}