using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffCompanyScript : IScript
    {
        [Command("empresas")]
        public static void CMD_empresas(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffCompanies", false, Functions.GetCompaniesHTML());
        }

        [ClientEvent(nameof(StaffCompanyGoto))]
        public static void StaffCompanyGoto(MyPlayer player, int id)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(company.PosX, company.PosY, company.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffCompanySave))]
        public static async Task StaffCompanySave(MyPlayer player, int id, string name, Vector3 pos, int weekRentValue)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (weekRentValue < 0)
            {
                player.EmitStaffShowMessage("Aluguel Semanal deve ser igual ou maior que 0.");
                return;
            }

            var company = new Company();
            if (id > 0)
                company = Global.Companies.FirstOrDefault(x => x.Id == id);

            company.Name = name;
            company.PosX = pos.X;
            company.PosY = pos.Y;
            company.PosZ = pos.Z;
            company.WeekRentValue = weekRentValue;

            await using var context = new DatabaseContext();

            if (company.Id == 0)
                await context.Companies.AddAsync(company);
            else
                context.Companies.Update(company);

            await context.SaveChangesAsync();

            company.CreateIdentifier();

            if (id == 0)
            {
                company.Characters = [];
                Global.Companies.Add(company);
            }

            player.EmitStaffShowMessage($"Empresa {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Empresa | {Functions.Serialize(company)}", null);

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }

        [AsyncClientEvent(nameof(StaffCompanyRemove))]
        public static async Task StaffCompanyRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company != null)
            {
                await using var context = new DatabaseContext();
                context.Companies.Remove(company);
                await context.SaveChangesAsync();
                Global.Companies.Remove(company);
                company.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Empresa | {Functions.Serialize(company)}", null);
            }

            player.EmitStaffShowMessage($"Empresa {id} excluída.");

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }

        [AsyncClientEvent(nameof(StaffCompanyRemoveOwner))]
        public static async Task StaffCompanyRemoveOwner(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company != null)
            {
                await company.RemoveOwner();
                await player.GravarLog(LogType.Staff, $"Remover Dono Empresa {id}", null);
            }

            player.EmitStaffShowMessage($"Dono da empresa {id} removido.");

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }
    }
}