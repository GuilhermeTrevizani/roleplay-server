using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay
{
    public class Testes
    {
        [Command("neon")]
        public void CMD_neon(IPlayer player)
        {
            if (!player.IsInVehicle)
                return;

            player.Vehicle.NeonColor = new AltV.Net.Data.Rgba(85, 45, 58, 255);
            player.Vehicle.SetNeonActive(true, true, true, true);
        }

        [Command("i")]
        public void CMD_i(IPlayer player, string ipl) => player.Emit("Server:RequestIpl", ipl);

        [Command("ri")]
        public void CMD_ri(IPlayer player, string ipl) => player.Emit("Server:RemoveIpl", ipl);

        [Command("ca", "/ca (slot) (drawable) (texture)")]
        public void CMD_ca(IPlayer player, int slot, int drawable, int texture)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.SetAccessories(slot, drawable, texture);
        }

        [Command("cc", "/cc (slot) (drawable) (texture)")]
        public void CMD_cc(IPlayer player, int slot, int drawable, int texture)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.SetClothes(slot, drawable, texture);
        }

        [Command("anim")]
        public void CMD_anim(IPlayer player, string dic, string name)
        {
            var p = Functions.ObterPersonagem(player);
            p.PlayAnimation(dic, name, (int)AnimationFlags.Loop);
        }
    }
}