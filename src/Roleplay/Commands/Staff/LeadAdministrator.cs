using AltV.Net;
using AltV.Net.Enums;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Commands.Staff
{
    public class LeadAdministrator
    {
        [Command("tempo", "/tempo (tempo)")]
        public static async Task CMD_tempo(MyPlayer player, uint tempo)
        {
            if (player.User.Staff < UserStaff.LeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(WeatherType), tempo))
            {
                player.SendMessage(MessageType.Error, "Tempo inválido.");
                return;
            }

            Global.WeatherInfo.WeatherType = (WeatherType)tempo;
            Alt.EmitAllClients("SyncWeather", Global.WeatherInfo.WeatherType.ToString().ToUpper());

            await Functions.SendStaffMessage($"{player.User.Name} alterou o tempo para {Global.WeatherInfo.WeatherType}.", true);
            await player.GravarLog(LogType.Staff, $"/tempo {Global.WeatherInfo.WeatherType}", null);
        }

        [Command("limparchatgeral")]
        public static async Task CMD_limparchatgeral(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.LeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            foreach (var x in Global.Players.Where(x => x.Character.Id > 0))
            {
                x.LimparChat();
                x.SendMessage(MessageType.Success, $"{player.User.Name} limpou o chat de todos.", notify: true);
            }

            await player.GravarLog(LogType.Staff, "/limparchatgeral", null);
        }

        [Command("areparar", "/areparar (veículo)")]
        public static async Task CMD_areparar(MyPlayer player, int veiculo)
        {
            if (player.User.Staff < UserStaff.LeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == veiculo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Veículo {veiculo} não está spawnado.");
                return;
            }

            veh = await veh.Reparar();
            await Functions.SendStaffMessage($"{player.User.Name} reparou o veículo {veiculo}.", true);
            await player.GravarLog(LogType.Staff, $"/areparar {veiculo}", null);
        }

        [Command("jetpack")]
        public static async Task CMD_jetpack(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.LeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não pode executar esse comando em um veículo.");
                return;
            }

            var veh = Alt.CreateVehicle(VehicleModel.Thruster, player.Position, player.Rotation);
            veh.Dimension = player.Dimension;
            player.SetIntoVehicle(veh, 1);
            veh.ManualEngineControl = true;
            veh.EngineOn = true;
            await player.GravarLog(LogType.Staff, "/jetpack", null);
        }
    }
}