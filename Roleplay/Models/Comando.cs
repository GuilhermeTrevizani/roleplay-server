namespace Roleplay.Models
{
    public class Comando
    {
        public Comando(string categoria, string nome, string descricao = "")
        {
            Categoria = categoria;
            Nome = nome;
            Descricao = descricao;
        }

        public string Categoria { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}