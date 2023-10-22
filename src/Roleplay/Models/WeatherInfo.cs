using AltV.Net.Enums;

namespace Roleplay.Models
{
    public class WeatherInfo
    {
        public List<WeatherInfoWeather> Weather { get; set; }

        public WeatherInfoMain Main { get; set; }

        public WeatherType WeatherType { get; set; } = WeatherType.Clear;
    }

    public class WeatherInfoWeather
    {
        public string Main { get; set; }
    }

    public class WeatherInfoMain
    {
        public float Temp { get; set; }
    }
}