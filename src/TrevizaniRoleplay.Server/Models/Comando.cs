namespace TrevizaniRoleplay.Server.Models
{
    public class Comando(string categoria, string nome, string descricao)
    {
        public string Categoria { get; set; } = categoria;
        public string Nome { get; set; } = nome;
        public string Descricao { get; set; } = descricao;
    }
}