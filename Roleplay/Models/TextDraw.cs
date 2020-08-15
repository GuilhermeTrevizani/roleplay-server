using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace Roleplay.Models
{
    public class TextDraw
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public Position Position { get; set; }
        public float Range { get; set; }
        public float Size { get; set; }
        public int Font { get; set; }
        public Rgba Color { get; set; }
        public int Dimension { get; set; }

        public void CriarIdentificador(IPlayer player)
        {
            DeletarIdentificador(player);

            player.Emit("textDraw:create", Codigo, Nome, Position, Range, Size, Font, Color, Dimension);
        }

        public void DeletarIdentificador(IPlayer player) => player.Emit("textDraw:remove", Codigo);
    }
}