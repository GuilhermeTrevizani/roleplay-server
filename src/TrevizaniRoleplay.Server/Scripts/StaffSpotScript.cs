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
    public class StaffSpotScript : IScript
    {
        [Command("pontos")]
        public static void CMD_pontos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = Functions.Serialize(
                Enum.GetValues(typeof(SpotType))
                .Cast<SpotType>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("StaffSpots", false, GetSpotsHTML(), jsonTypes);
        }

        [ClientEvent(nameof(StaffSpotGoto))]
        public static void StaffSpotGoto(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var spot = Global.Spots.FirstOrDefault(x => x.Id == id);
            if (spot == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(spot.PosX, spot.PosY, spot.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffSpotSave))]
        public static async Task StaffSpotSave(MyPlayer player, string idString, int type, Vector3 pos, Vector3 auxiliarPos)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(SpotType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            var spot = new Spot();
            var id = idString.ToGuid();
            var isNew = string.IsNullOrWhiteSpace(idString);
            if (isNew)
            {
                spot.Create((SpotType)type, pos.X, pos.Y, pos.Z, auxiliarPos.X, auxiliarPos.Y, auxiliarPos.Z);
            }
            else
            {
                spot = Global.Spots.FirstOrDefault(x => x.Id == id);
                if (spot == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                spot.Update((SpotType)type, pos.X, pos.Y, pos.Z, auxiliarPos.X, auxiliarPos.Y, auxiliarPos.Z);
            }

            await using var context = new DatabaseContext();

            if (isNew)
                await context.Spots.AddAsync(spot);
            else
                context.Spots.Update(spot);

            await context.SaveChangesAsync();

            spot.CreateIdentifier();

            if (isNew)
                Global.Spots.Add(spot);

            player.EmitStaffShowMessage($"Ponto {(isNew ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Ponto | {Functions.Serialize(spot)}", null);

            var html = GetSpotsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Spots)))
                target.Emit("StaffSpots", true, html);
        }

        [AsyncClientEvent(nameof(StaffSpotRemove))]
        public static async Task StaffSpotRemove(MyPlayer player, string idString)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var id = idString.ToGuid();
            var spot = Global.Spots.FirstOrDefault(x => x.Id == id);
            if (spot != null)
            {
                await using var context = new DatabaseContext();
                context.Spots.Remove(spot);
                await context.SaveChangesAsync();
                Global.Spots.Remove(spot);
                spot.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Ponto | {Functions.Serialize(spot)}", null);
            }

            player.EmitStaffShowMessage($"Ponto {id} excluído.");

            var html = GetSpotsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Spots)))
                target.Emit("StaffSpots", true, html);
        }

        private static string GetSpotsHTML()
        {
            var html = string.Empty;
            if (Global.Spots.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='5'>Não há pontos criados.</td></tr>";
            }
            else
            {
                foreach (var spot in Global.Spots.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{spot.Id}</td>
                        <td>{spot.Type.GetDisplay()}</td>
                        <td>X: {spot.PosX} | Y: {spot.PosY} | Z: {spot.PosZ}</td>
                        <td>X: {spot.AuxiliarPosX} | Y: {spot.AuxiliarPosY} | Z: {spot.AuxiliarPosZ}</td>
                        <td class='text-center'>
                            <input id='json{spot.Id}' type='hidden' value='{Functions.Serialize(spot)}' />
                            <button onclick='ir({spot.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='editar({spot.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {spot.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }
    }
}