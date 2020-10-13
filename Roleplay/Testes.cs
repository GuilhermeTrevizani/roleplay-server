using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay
{
    public class Testes
    {
        [Command("x")]
        public void CMD_x(IPlayer player)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            //Alt.CreateBlip(player, BlipType.Object, player.Position);
            //Alt.CreateCheckpoint(CheckpointType.Cyclinder, player.Position, 1, 1, new Rgba(255, 255, 255, 255));
        }

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
        public void CMD_int(IPlayer player, int codigo)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            var pos = Functions.ObterPosicaoPorInterior((TipoInterior)codigo);

            p.IPLs = Functions.ObterIPLsPorInterior((TipoInterior)codigo);
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

        [Command("anim", "/anim (dic) (name) (flag)")]
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
        public void CMD_l(IPlayer player, byte livery)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
                return;

            player.Vehicle.Livery = livery;
        }

        [Command("e", "/e (extra)")]
        public void CMD_e(IPlayer player, byte extra)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            if (!player.IsInVehicle)
                return;

            player.Vehicle.ToggleExtra(extra, player.Vehicle.IsExtraOn(extra));
        }

        [Command("dinheiro", "/dinheiro (dinheiro)")]
        public void CMD_dinheiro(IPlayer player, int dinheiro)
        {
            if (!Global.Development)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "O servidor não está em modo desenvolvimento.");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            p.Dinheiro = dinheiro;
            p.SetDinheiro();
        }
    }
}