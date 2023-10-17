using AltV.Net;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class Test : IScript
    {
        [Command("x")]
        public static async Task CMD_x(MyPlayer player)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var property = Global.Properties.FirstOrDefault();
            property.StartAlarm();

            //await Global.DiscordClient.SetGameAsync("olá marilene");

            //Alt.CreateColShapeRectangle()

            player.SendMessage(MessageType.Success, "/x");

            /*
                //player.PlayAnimation("cellphone@", "cellphone_call_in", (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody)); //pegando celular
                //await Task.Delay(2000); // 2 seg
                //player.PlayAnimation("cellphone@", "cellphone_call_listen_base", (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody)); //em ligação
            */
        }

        [Command("y")]
        public static async Task CMD_y(MyPlayer player)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var property = Global.Properties.FirstOrDefault();
            property.StopAlarm();
            player.SendMessage(MessageType.Success, "/y");
        }

        [Command("x1", "/x1 (dlc) (component) (drawable) (texture)")]
        public static void CMD_x1(MyPlayer player, string dlc, byte component, int drawable, byte texture)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var msg = $"/x1 {component} {drawable} {texture} 0 {Alt.Hash(dlc)}";
            var success = player.SetDlcClothes(component, Convert.ToUInt16(drawable), texture, 0, Alt.Hash(dlc));
            player.SendMessage(success ? MessageType.Success : MessageType.Error, msg);
            Alt.Log(msg);
        }

        [Command("x2", "/x2 (dlc) (component) (drawable) (texture)")]
        public static void CMD_x2(MyPlayer player, string dlc, byte component, int drawable, byte texture)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var msg = $"/x2 {component} {drawable} {texture} 0 {Alt.Hash(dlc)}";
            var success = player.SetDlcProps(component, Convert.ToUInt16(drawable), texture, Alt.Hash(dlc));
            player.SendMessage(success ? MessageType.Success : MessageType.Error, msg);
            Alt.Log(msg);
        }

        [Command("x3", "/x3 (collection) (overlay)")]
        public static void CMD_x3(MyPlayer player, string collection, string overlay)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var msg = $"/x3 {collection} {overlay}";
            player.AddDecoration(Alt.Hash(collection), Alt.Hash(overlay));
            player.SendMessage(MessageType.None, msg);
            Alt.Log(msg);
        }

        [Command("l", "/l (livery)")]
        public static void CMD_l(MyPlayer player, byte livery)
        {
            if (!Alt.IsDebug)
            {
                player.SendMessage(MessageType.Error, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo.");
                return;
            }

            player.Vehicle.Livery = livery;
        }
    }
}