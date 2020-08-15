namespace Roleplay.Entities
{
    public class PersonagemArma
    {
        public int Codigo { get; set; }
        public long Arma { get; set; }
        public int Municao { get; set; } = 0;
        public int Pintura { get; set; } = 0;
        public string Componentes { get; set; } = "[]";
    }
}