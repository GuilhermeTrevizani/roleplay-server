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

            Global.Parameter.VehicleParkValue = parameter.VehicleParkValue;
            Global.Parameter.HospitalValue = parameter.HospitalValue;
            Global.Parameter.BarberValue = parameter.BarberValue;
            Global.Parameter.ClothesValue = parameter.ClothesValue;
            Global.Parameter.DriverLicenseBuyValue = parameter.DriverLicenseBuyValue;
            Global.Parameter.Paycheck = parameter.Paycheck;
            Global.Parameter.DriverLicenseRenewValue = parameter.DriverLicenseRenewValue;
            Global.Parameter.AnnouncementValue = parameter.AnnouncementValue;
            Global.Parameter.ExtraPaymentGarbagemanValue = parameter.ExtraPaymentGarbagemanValue;
            Global.Parameter.Blackout = parameter.Blackout;
            Global.Parameter.KeyValue = parameter.KeyValue;
            Global.Parameter.LockValue = parameter.LockValue;
            Global.Parameter.IPLsJSON = parameter.IPLsJSON ?? "[]";
            Global.Parameter.TattooValue = parameter.TattooValue;
            Global.Parameter.CooldownDismantleHours = parameter.CooldownDismantleHours;
            Global.Parameter.PropertyRobberyConnectedTime = parameter.PropertyRobberyConnectedTime;
            Global.Parameter.CooldownPropertyRobberyRobberHours = parameter.CooldownPropertyRobberyRobberHours;
            Global.Parameter.CooldownPropertyRobberyPropertyHours = parameter.CooldownPropertyRobberyPropertyHours;
            Global.Parameter.PoliceOfficersPropertyRobbery = parameter.PoliceOfficersPropertyRobbery;
            Global.Parameter.InitialTimeCrackDen = parameter.InitialTimeCrackDen;
            Global.Parameter.EndTimeCrackDen = parameter.EndTimeCrackDen;
            Global.Parameter.FirefightersBlockHeal = parameter.FirefightersBlockHeal;

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