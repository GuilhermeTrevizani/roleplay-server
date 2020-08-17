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

        [Command("x")]
        public void CMD_x(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Sexo == "M")
            {
                p.SetClothes(3, 0, 0);
                p.SetClothes(8, 58, 0);
                p.SetClothes(6, 25, 0);
                p.SetClothes(4, 35, 0);
                p.SetClothes(11, 55, 0);
            }
            else
            {
                p.SetClothes(3, 0, 0);
                p.SetClothes(8, 35, 0);
                p.SetClothes(6, 25, 0);
                p.SetClothes(4, 34, 0);
                p.SetClothes(11, 48, 0);
            }
        }

        [Command("teste")]
        public void CMD_teste(IPlayer player)
        {
            player.Emit("character:Edit", "");
        }

        /* [Command("skin", "/skin)")]
        public void CMD_skin(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.LojaRoupas && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Constants.DistanciaRP))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma loja de roupas!");
                return;
            }

            // Abrir loja de roupas

            if (p.Dinheiro < Global.Parametros.ValorSkin)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            p.Dinheiro -= Global.Parametros.ValorSkin;
            p.SetDinheiro();
            player.SetSkin(pedHash);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a skin {pedHash} por ${Global.Parametros.ValorSkin:N0}.");
        }*/
    }
}