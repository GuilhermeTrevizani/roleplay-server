using AltV.Net.Elements.Entities;
using Roleplay.Models;

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

        [Command("i")]
        public void CMD_i(IPlayer player, string ipl) => player.Emit("Server:RequestIpl", ipl);

        [Command("ri")]
        public void CMD_ri(IPlayer player, string ipl) => player.Emit("Server:RemoveIpl", ipl);

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

        [Command("teste")]
        public void CMD_teste(IPlayer player)
        {
            player.Emit("character:Edit", "");
        }
    }
}