using AltV.Net;
using AltV.Net.Async;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniTextRoleplay.Server.Scripts
{
    public class StaffAnimationScript : IScript
    {
        [Command("animacoes")]
        public static void CMD_animacoes(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffAnimations", false, GetAnimationsHTML());
        }

        private static string GetAnimationsHTML()
        {
            var html = string.Empty;
            if (Global.Animations.Count == 0)
            {
                html = "<tr><td class='text-center' colspan='7'>Não há animações criadas.</td></tr>";
            }
            else
            {
                foreach (var animation in Global.Animations.OrderByDescending(x => x.Display))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{animation.Display}</td>
                        <td>{animation.Dictionary}</td>
                        <td>{animation.Name}</td>
                        <td>{animation.Flag}</td>
                        <td>{animation.Duration}</td>
                        <td class='text-center'>{(animation.OnlyInVehicle ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{animation.Id}' type='hidden' value='{Functions.Serialize(animation)}' />
                            <button onclick='edit(`{animation.Id}`)' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, `{animation.Id}`)' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        [AsyncClientEvent(nameof(StaffAnimationSave))]
        public static async Task StaffAnimationSave(MyPlayer player, string id, string display, string dictionary, string name,
            int flag, int duration, bool onlyInVehicle)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var animation = new Animation();
            var newAnimation = false;
            if (string.IsNullOrWhiteSpace(id))
            {
                newAnimation = true;
                animation.Create(display, dictionary, name, flag, duration, onlyInVehicle);
            }
            else
            {
                animation = Global.Animations.FirstOrDefault(x => x.Id == new Guid(id));
                if (animation == null)
                {
                    player.EmitStaffShowMessage(Global.RECORD_NOT_FOUND);
                    return;
                }

                animation.Update(display, dictionary, name, flag, duration, onlyInVehicle);
            }

            await using var context = new DatabaseContext();

            if (newAnimation)
                await context.Animations.AddAsync(animation);
            else
                context.Animations.Update(animation);

            await context.SaveChangesAsync();

            if (newAnimation)
                Global.Animations.Add(animation);

            player.EmitStaffShowMessage($"Animação {(newAnimation ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Animação | {Functions.Serialize(animation)}", null);

            var html = GetAnimationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Animations)))
                target.Emit("StaffAnimations", true, html);
        }

        [AsyncClientEvent(nameof(StaffAnimationRemove))]
        public static async Task StaffAnimationRemove(MyPlayer player, string id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var animation = Global.Animations.FirstOrDefault(x => x.Id == new Guid(id));
            if (animation != null)
            {
                await using var context = new DatabaseContext();
                context.Animations.Remove(animation);
                await context.SaveChangesAsync();
                Global.Animations.Remove(animation);
                await player.GravarLog(LogType.Staff, $"Remover Animação | {Functions.Serialize(animation)}", null);
            }

            player.EmitStaffShowMessage($"Animação {id} excluída.");

            var html = GetAnimationsHTML();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.StaffFlags.Contains(StaffFlag.Animations)))
                target.Emit("StaffAnimations", true, html);
        }
    }
}