using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay
{
    public class Testes
    {
        [Command("i", "/i (ipl)")]
        public void CMD_i(IPlayer player, string ipl)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            player.Emit("Server:RequestIpl", ipl);
        }

        [Command("ri", "/ri (ipl)")]
        public void CMD_ri(IPlayer player, string ipl)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            player.Emit("Server:RemoveIpl", ipl);
        }

        [Command("ca", "/ca (slot) (drawable) (texture)")]
        public void CMD_ca(IPlayer player, int slot, int drawable, int texture)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.SetAccessories(slot, drawable, texture);
        }

        [Command("cc", "/cc (slot) (drawable) (texture)")]
        public void CMD_cc(IPlayer player, int slot, int drawable, int texture)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.SetClothes(slot, drawable, texture);
        }

        [Command("anim", "/cc (dic) (name) (flag)")]
        public void CMD_anim(IPlayer player, string dic, string name, int flag)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            p.PlayAnimation(dic, name, flag);
        }
    }
}