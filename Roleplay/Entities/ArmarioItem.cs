namespace Roleplay.Entities
{
    public class ArmarioItem
    {
        public int Codigo { get; set; }
        public long Arma { get; set; }
        public int Municao { get; set; } = 1;
        public int Estoque { get; set; } = 0;
        public byte Pintura { get; set; } = 0;
        public int Rank { get; set; } = 1;
        public string Componentes { get; set; } = "[]";
    }
}