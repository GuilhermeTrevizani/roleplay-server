using Roleplay.Models;

namespace Roleplay.Entities
{
    public class Preco
    {
        public TipoPreco Tipo { get; set; }
        public string Nome { get; set; }
        public int Valor { get; set; }
    }
}