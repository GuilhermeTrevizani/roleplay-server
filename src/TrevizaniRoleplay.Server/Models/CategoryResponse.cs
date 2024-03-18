namespace TrevizaniRoleplay.Server.Models
{
    public class CategoryResponse
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extra { get; set; } = string.Empty;
        public bool HasType { get; set; }
        public bool IsStack { get; set; }
    }
}