using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
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

            player.Emit("StaffCompanies", false, GetCompaniesHTML());
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

            var html = GetCompaniesHTML();
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

            var html = GetCompaniesHTML();
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

            var html = GetCompaniesHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }

        public static string GetCompaniesHTML()
        {
            var html = string.Empty;
            if (Global.Companies.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='11'>Não há empresas criadas.</td></tr>";
            }
            else
            {
                foreach (var company in Global.Companies.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{company.Id}</td>
                        <td>{company.Name}</td>
                        <td>X: {company.PosX} | Y: {company.PosY} | Z: {company.PosZ}</td>
                        <td>${company.WeekRentValue:N0}</td>
                        <td>{company.RentPaymentDate}</td>
                        <td>{company.CharacterId}</td>
                        <td><span style='color:#{company.Color}'>#{company.Color}</span></td>
                        <td>{company.BlipType}</td>
                        <td>{company.BlipColor}</td>
                        <td class='text-center'>{(company.GetIsOpen() ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{company.Id}' type='hidden' value='{Functions.Serialize(company)}' />
                            <button onclick='goto({company.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({company.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            {(company.CharacterId.HasValue ? $"<button onclick='removeOwner(this, {company.Id})' type='button' class='btn btn-dark btn-sm'>REMOVER DONO</button>" : string.Empty)}
                            <button onclick='remove(this, {company.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}