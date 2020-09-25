using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay
{
    public class Testes
    {
        [Command("fix")]
        public void CMD_fix(IPlayer player)
        {
            if (!player.IsInVehicle)
                return;

            player.Vehicle.BodyHealth = 1000;
            player.Vehicle.EngineHealth = 1000;
            player.Vehicle.PetrolTankHealth = 1000;
        }

        [Command("v")]
        public void CMD_v(IPlayer player)
        {
            if (!player.IsInVehicle)
                return;

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Engine: {player.Vehicle.EngineOn}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"EngineHealth: {player.Vehicle.EngineHealth}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"BodyHealth: {player.Vehicle.BodyHealth}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"BodyAdditionalHealth: {player.Vehicle.BodyAdditionalHealth}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"PetrolTankHealth: {player.Vehicle.PetrolTankHealth}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"DamageData: {player.Vehicle.DamageData}");
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

        [Command("wc")]
        public void CMD_wc(IPlayer player, string arma, string componente)
        {
            var wep = Enum.GetValues(typeof(WeaponModel)).Cast<WeaponModel>().FirstOrDefault(x => x.ToString().ToLower() == arma.ToLower());
            if (wep == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe.");
                return;
            }

            var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.ToLower() == componente.ToLower() && x.Weapon == wep);
            if (comp == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Componente {componente} não existe para a arma {wep}.");
                return;
            }

            player.AddWeaponComponent(wep, comp.Hash);
        }

        [Command("anim")]
        public void CMD_anim(IPlayer player, string dic, string name)
        {
            var p = Functions.ObterPersonagem(player);
            p.PlayAnimation(dic, name, (int)AnimationFlags.Loop);
        }
    }
}