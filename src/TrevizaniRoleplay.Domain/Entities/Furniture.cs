namespace TrevizaniRoleplay.Domain.Entities
{
    public class Furniture : BaseEntity
    {
        public string Category { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Model { get; private set; } = string.Empty;
        public int Value { get; private set; }

        public void Create(string category, string name, string model, int value)
        {
            Category = category;
            Name = name;
            Model = model;
            Value = value;
        }

        public void Update(string category, string name, string model, int value)
        {
            Category = category;
            Name = name;
            Model = model;
            Value = value;
        }
    }
}