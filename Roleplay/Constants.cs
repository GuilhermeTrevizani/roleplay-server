using AltV.Net.Data;

namespace Roleplay
{
    public class Constants
    {
        public static string NomeServidor { get; } = "GTA: Downtown";
        public static Position PosicaoPrisao { get; } = new Position(461.7921f, -989.0697f, 24.91488f);
        public static string CorCelular { get; } = "#F0E90D";
        public static string CorCelularSecundaria { get; } = "#F2FF43";
        public static string CorRadio { get; } = "#FFFF9B";

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        };
    }
}