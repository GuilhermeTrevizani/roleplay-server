using AltV.Net.Data;
using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Propriedade
    {
        public int Codigo { get; set; }
        public TipoInterior Interior { get; set; }
        public int Valor { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public float EntradaPosX { get; set; } = 0;
        public float EntradaPosY { get; set; } = 0;
        public float EntradaPosZ { get; set; } = 0;
        public float SaidaPosX { get; set; } = 0;
        public float SaidaPosY { get; set; } = 0;
        public float SaidaPosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public string Endereco { get; set; } = string.Empty;

        [NotMapped]
        public TextDraw TextLabel { get; set; }

        [NotMapped]
        public bool Aberta { get; set; } = false;

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = Functions.CriarTextDraw($"Propriedade Nº {Codigo}{(Personagem == 0 ? $"\n~w~Use /comprar para comprar por ${Valor:N0}" : string.Empty)}", new Position(EntradaPosX, EntradaPosY, EntradaPosZ), 10, 0.4f, 4, new Rgba(254, 189, 12, 255), (int)Dimensao);
        }

        public void DeletarIdentificador()
        {
            Functions.RemoverTextDraw(TextLabel);
        }
    }
}