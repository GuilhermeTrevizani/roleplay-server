namespace TrevizaniRoleplay.Domain.Entities
{
    public class Animation : BaseEntity
    {
        public string Display { get; private set; } = string.Empty;
        public string Dictionary { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public int Flag { get; private set; }
        public int Duration { get; private set; }
        public bool OnlyInVehicle { get; private set; }

        public void Create(string display, string dictionary, string name, int flag, int duration, bool onlyInVehicle)
        {
            Display = display;
            Dictionary = dictionary;
            Name = name;
            Flag = flag;
            Duration = duration;
            OnlyInVehicle = onlyInVehicle;
        }

        public void Update(string display, string dictionary, string name, int flag, int duration, bool onlyInVehicle)
        {
            Display = display;
            Dictionary = dictionary;
            Name = name;
            Flag = flag;
            Duration = duration;
            OnlyInVehicle = onlyInVehicle;
        }
    }
}