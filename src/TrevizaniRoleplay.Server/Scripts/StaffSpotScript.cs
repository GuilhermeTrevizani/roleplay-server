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

            player.Emit("StaffSpots", false, Functions.GetSpotsHTML(), jsonTypes);
        }

        [ClientEvent(nameof(StaffSpotGoto))]
        public static void StaffSpotGoto(MyPlayer player, int id)
        {
            var spot = Global.Spots.FirstOrDefault(x => x.Id == id);
            if (spot == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(spot.PosX, spot.PosY, spot.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffSpotSave))]
        public static async Task StaffSpotSave(MyPlayer player, int id, int type, Vector3 pos, Vector3 auxiliarPos)
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
            if (id > 0)
                spot = Global.Spots.FirstOrDefault(x => x.Id == id);

            spot.Type = (SpotType)type;
            spot.PosX = pos.X;
            spot.PosY = pos.Y;
            spot.PosZ = pos.Z;
            spot.AuxiliarPosX = auxiliarPos.X;
            spot.AuxiliarPosY = auxiliarPos.Y;
            spot.AuxiliarPosZ = auxiliarPos.Z;

            await using var context = new DatabaseContext();

            if (spot.Id == 0)
                await context.Spots.AddAsync(spot);
            else
                context.Spots.Update(spot);

            await context.SaveChangesAsync();

            spot.CreateIdentifier();

            if (id == 0)
                Global.Spots.Add(spot);

            player.EmitStaffShowMessage($"Ponto {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Ponto | {Functions.Serialize(spot)}", null);

            var html = Functions.GetSpotsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Spots)))
                target.Emit("StaffSpots", true, html);
        }

        [AsyncClientEvent(nameof(StaffSpotRemove))]
        public static async Task StaffSpotRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

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

            var html = Functions.GetSpotsHTML();
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
                            <input id='json{spot.Id}' type='hidden' value='{Serialize(spot)}' />
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