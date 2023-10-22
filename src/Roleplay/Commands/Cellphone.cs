using Discord;
using Discord.WebSocket;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Text.Json;

namespace Roleplay.Commands
{
    public class Cellphone
    {
        [Command("celular", Aliases = new string[] { "cel" })]
        public static void CMD_celular(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            player.Emit("OpenCellphone", player.CellphoneItem.ModoAviao,
                JsonSerializer.Serialize(player.CellphoneItem.Contatos.OrderBy(x => x.Nome).ToList()));
        }

        [Command("sms", "/sms (número ou nome do contato) (mensagem)", GreedyArg = true)]
        public static async Task CMD_sms(MyPlayer player, string numeroNomeContato, string mensagem)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.ModoAviao)
            {
                player.SendMessage(Models.MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (!uint.TryParse(numeroNomeContato, out uint numero) || numero == 0)
                numero = player.CellphoneItem.Contatos.FirstOrDefault(x => x.Nome.ToLower().Contains(numeroNomeContato.ToLower()))?.Numero ?? 0;

            if (numero == player.Cellphone)
            {
                player.SendMessage(Models.MessageType.Error, "Você não pode enviar um SMS para você mesmo.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Cellphone == numero && numero > 0);
            if (target == null || target.CellphoneItem.ModoAviao)
            {
                player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(numero)} está indisponível.", Global.CELLPHONE_SECONDARY_COLOR);
                return;
            }

            player.SendMessage(Models.MessageType.None, $"[CELULAR] SMS para {player.ObterNomeContato(numero)}: {mensagem}", Global.CELLPHONE_SECONDARY_COLOR);
            player.SendMessageToNearbyPlayers("envia uma mensagem de texto.", MessageCategory.Ame, 5);
            player.CellphoneItem.Mensagens.Add(new CellphoneItemMessage
            {
                Numero = target.Cellphone,
                Mensagem = mensagem,
                Tipo = CellphoneMessageType.Text,
            });
            await player.UpdateCellphoneDatabase();

            target.SendMessage(Models.MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(player.Cellphone)}: {mensagem}", Global.CELLPHONE_MAIN_COLOR);
            target.SendMessageToNearbyPlayers("recebe uma mensagem de texto.", MessageCategory.Ame, 5);
            target.CellphoneItem.Mensagens.Add(new CellphoneItemMessage
            {
                Numero = player.Cellphone,
                Mensagem = mensagem,
                Tipo = CellphoneMessageType.Text,
            });
            await target.UpdateCellphoneDatabase();
        }

        [Command("ligar", "/ligar (número ou nome do contato)")]
        public static async Task CMD_ligar(MyPlayer player, string numeroNomeContato) => await Functions.CMDLigar(player, numeroNomeContato);

        [Command("desligar", Aliases = new string[] { "des" })]
        public static async Task CMD_desligar(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneCall.Numero == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está uma ligação.");
                return;
            }

            player.SendMessageToNearbyPlayers("desliga a ligação.", MessageCategory.Ame, 5);
            player.SendMessage(Models.MessageType.None, $"[CELULAR] Você desligou a ligação de {player.ObterNomeContato(player.CellphoneCall.Numero)}.", Global.CELLPHONE_SECONDARY_COLOR);
            await player.EndCellphoneCall();
        }

        [Command("atender")]
        public static void CMD_atender(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneCall.Numero == 0 || player.CellphoneCall.Tipo == CellphoneCallType.Atendida || player.CellphoneCall.Origem)
            {
                player.SendMessage(Models.MessageType.Error, "Seu celular não está tocando.");
                return;
            }

            player.CellphoneCall.Tipo = CellphoneCallType.Atendida;
            player.SendMessageToNearbyPlayers("atende a ligação.", MessageCategory.Ame, 5);
            player.SendMessage(Models.MessageType.None, $"[CELULAR] Você atendeu a ligação de {player.ObterNomeContato(player.CellphoneCall.Numero)}.", Global.CELLPHONE_SECONDARY_COLOR);

            var target = Global.Players.FirstOrDefault(x => x.CellphoneCall.Numero == player.Cellphone);
            if (target != null)
            {
                target.CellphoneCall.Tipo = CellphoneCallType.Atendida;
                target.SendMessage(Models.MessageType.None, $"[CELULAR] Sua ligação para {target.ObterNomeContato(player.Cellphone)} foi atendida.", Global.CELLPHONE_SECONDARY_COLOR);
                target.TimerCelular?.Stop();
                target.TimerCelular = null;
            }
        }

        [Command("an", "/an (mensagem)", GreedyArg = true)]
        public async static Task CMD_an(MyPlayer player, string message)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.ModoAviao)
            {
                player.SendMessage(Models.MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (player.Money < Global.Parameter.AnnouncementValue)
            {
                player.SendMessage(Models.MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.AnnouncementValue));
                return;
            }

            var segundos = 120;
            if ((player.User.VIPValidDate ?? DateTime.MinValue) >= DateTime.Now)
            {
                if (player.User.VIP == UserVIP.Gold)
                    segundos = 30;
                else if (player.User.VIP == UserVIP.Silver)
                    segundos = 60;
            }

            var cooldown = (player.Character.AnnouncementLastUseDate ?? DateTime.MinValue).AddSeconds(segundos);
            if (cooldown > DateTime.Now)
            {
                player.SendMessage(Models.MessageType.Error, $"O uso da central de anúncios estará disponível em {cooldown}.");
                return;
            }

            player.Character.AnnouncementLastUseDate = DateTime.Now;
            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = Global.Parameter.AnnouncementValue
            });

            message = Functions.CheckFinalDot(message);
            foreach (var x in Global.Players.Where(x => x.Character.Id > 0 && !x.User.AnnouncementToggle))
            {
                x.SendMessage(Models.MessageType.None, $"[ANÚNCIO] {message} [CONTATO: {player.Cellphone}]", "#8EBE59");

                if (x.User.Staff != UserStaff.None)
                    x.SendMessage(Models.MessageType.None, $"{player.Character.Name} [{player.SessionId}] ({player.User.Name}) enviou o anúncio.", Global.STAFF_COLOR);
            }

            await player.GravarLog(LogType.Anuncio, message, null);

            if (!string.IsNullOrWhiteSpace(Global.DiscordBotToken))
            {
                var x = new EmbedBuilder
                {
                    Title = $"Anúncio de #{player.Cellphone}",
                    Description = message,
                    Color = new Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
                };
                x.WithFooter($"Enviado em {DateTime.Now}.");

                _ = (Global.DiscordClient.GetChannel(Global.AnnouncementDiscordChannel) as SocketTextChannel).SendMessageAsync(embed: x.Build());
            }
        }
    }
}