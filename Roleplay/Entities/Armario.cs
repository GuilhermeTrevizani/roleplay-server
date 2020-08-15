using AltV.Net.Data;
using Roleplay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        
        [NotMapped]
        public TextDraw TextLabel2 { get; set; }

        [NotMapped]
        public List<ArmarioItem> Itens { get => Global.ArmariosItens.Where(x => x.Codigo == Codigo).OrderBy(x => x.Rank).ThenBy(x => x.Arma).ToList(); }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = Functions.CriarTextDraw("Armário", new Position(PosX, PosY, PosZ), 5, 0.4f, 4, new Rgba(254, 189, 12, 255), (int)Dimensao);
            TextLabel2 = Functions.CriarTextDraw("Use /armario", new Position(PosX, PosY, PosZ - 0.15f), 5, 0.4f, 4, new Rgba(255, 255, 255, 255), (int)Dimensao);
        }

        public void DeletarIdentificador()
        {
            Functions.RemoverTextDraw(TextLabel);
            Functions.RemoverTextDraw(TextLabel2);
        }
    }
}