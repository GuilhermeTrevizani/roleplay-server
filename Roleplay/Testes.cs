using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay
{
    public class Testes
    {
        [Command("w", "/w (arma)")]
        public void CMD_w(IPlayer player, string arma)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            player.GiveWeapon(wep, 2000, true);
        }

        [Command("int", "/int (código)")]
        public void CMD_int(IPlayer player, int código)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), código))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            var pos = Functions.ObterPosicaoPorInterior((TipoInterior)código);

            p.IPLs = Functions.ObterIPLsPorInterior((TipoInterior)código);
            p.SetarIPLs();
            player.Dimension = 0;
            p.SetPosition(pos, false);
        }

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

        [Command("l", "/l (livery)")]
        public void CMD_l(IPlayer player, int livery)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
                return;

            player.Vehicle.Livery = (byte)livery;
        }

        [Command("rl", "/rl (livery)")]
        public void CMD_rl(IPlayer player, int livery)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
                return;

            player.Vehicle.RoofLivery = (byte)livery;
        }

        [Command("e", "/e (extra)")]
        public void CMD_e(IPlayer player, int extra)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
                return;

            player.Vehicle.ToggleExtra((byte)extra, !player.Vehicle.IsExtraOn((byte)extra));
        }
    }
}