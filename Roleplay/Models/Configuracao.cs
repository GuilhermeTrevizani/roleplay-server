namespace Roleplay.Models
{
    public class Configuracao
    {
        public string DBHost { get; set; }
        public string DBName { get; set; }
        public string DBUser { get; set; }
        public string DBPassword { get; set; }
        public int MaxPlayers { get; set; }
    }
}