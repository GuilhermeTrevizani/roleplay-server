using AltV.Net.Data;
using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Armario
    {
        public int Codigo { get; set; }
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public int Faccao { get; set; } = 0;

        [NotMapped]
        public TextDraw TextLabel { get; set; }

        public void CriarIdentificador()
        {
            DeletarIdentificador();
            TextLabel = Functions.CriarTextDraw("Armário\n~w~Use /armario", new Position(PosX, PosY, PosZ), 5, 0.4f, 4, new Rgba(254, 189, 12, 255), (int)Dimensao);
        }

        public void DeletarIdentificador() => Functions.RemoverTextDraw(TextLabel);
    }
}