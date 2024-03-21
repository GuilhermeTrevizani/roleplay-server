using AltV.Net;
using AltV.Net.Async;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffParameterScript : IScript
    {
        [Command("parametros")]
        public static void CMD_parametros(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.HeadAdministrator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffParameters", Functions.Serialize(Global.Parameter));
        }

        [AsyncClientEvent(nameof(StaffParametersSave))]
        public static async Task StaffParametersSave(MyPlayer player, string jsonParameters)
        {
            if (player.User.Staff < UserStaff.HeadAdministrator)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var parametrosAntigo = Functions.Serialize(Global.Parameter);

            var parameter = Functions.Deserialize<Parameter>(jsonParameters);

            if (parameter.InitialTimeCrackDen < 0 || parameter.InitialTimeCrackDen > 23)
            {
                player.EmitStaffShowMessage("Hora Inicial para Uso da Boca de Fumo não foi preenchida corretamente.");
                return;
            }

            if (parameter.EndTimeCrackDen < 0 || parameter.EndTimeCrackDen > 23)
            {
                player.EmitStaffShowMessage("Hora Final para Uso da Boca de Fumo não foi preenchida corretamente.");
                return;
            }

            Global.Parameter.Update(parameter.VehicleParkValue, parameter.HospitalValue, parameter.BarberValue,
                parameter.ClothesValue, parameter.DriverLicenseBuyValue, parameter.Paycheck, parameter.DriverLicenseRenewValue,
                parameter.AnnouncementValue, parameter.ExtraPaymentGarbagemanValue, parameter.Blackout, parameter.KeyValue,
                parameter.LockValue, parameter.IPLsJSON ?? "[]", parameter.TattooValue, parameter.CooldownDismantleHours,
                parameter.PropertyRobberyConnectedTime, parameter.CooldownPropertyRobberyRobberHours, parameter.CooldownPropertyRobberyPropertyHours,
                parameter.PoliceOfficersPropertyRobbery, parameter.InitialTimeCrackDen, parameter.EndTimeCrackDen, parameter.FirefightersBlockHeal);

            Global.IPLs.ForEach(ipl => Alt.EmitAllClients("Server:RemoveIpl", ipl));
            Global.IPLs = Functions.Deserialize<List<string>>(Global.Parameter.IPLsJSON);
            Global.IPLs.ForEach(ipl => Alt.EmitAllClients("Server:RequestIpl", ipl));

            Alt.EmitAllClients("Server:setArtificialLightsState", Global.Parameter.Blackout);

            await using var context = new DatabaseContext();
            context.Parameters.Update(Global.Parameter);
            await context.SaveChangesAsync();

            await player.GravarLog(LogType.Staff, $"Parâmetros | {parametrosAntigo} | {Functions.Serialize(Global.Parameter)}", null);

            player.EmitStaffShowMessage("Parâmetros do servidor alterados.");
        }
    }
}