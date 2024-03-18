using AltV.Net;
using AltV.Net.Async;
using Discord.WebSocket;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class CellphoneScript : IScript
    {
        [Command("celular", Aliases = ["cel"])]
        public static void CMD_celular(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            player.Emit("OpenCellphone", player.CellphoneItem.FlightMode,
                Functions.Serialize(player.CellphoneItem.Contacts.OrderBy(x => x.Name).ToList()));
        }

        [Command("sms", "/sms (número ou nome do contato) (mensagem)", GreedyArg = true)]
        public static async Task CMD_sms(MyPlayer player, string numeroNomeContato, string mensagem)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (!uint.TryParse(numeroNomeContato, out uint numero) || numero == 0)
                numero = player.CellphoneItem.Contacts.FirstOrDefault(x => x.Name.Contains(numeroNomeContato, StringComparison.CurrentCultureIgnoreCase))?.Number ?? 0;

            if (numero == player.Cellphone)
            {
                player.SendMessage(MessageType.Error, "Você não pode enviar um SMS para você mesmo.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Cellphone == numero && numero > 0);
            if (target == null || target.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(numero)} está indisponível.", Global.CELLPHONE_SECONDARY_COLOR);
                return;
            }

            player.SendMessage(MessageType.None, $"[CELULAR] SMS para {player.ObterNomeContato(numero)}: {mensagem}", Global.CELLPHONE_SECONDARY_COLOR);
            player.SendMessageToNearbyPlayers("envia uma mensagem de texto.", MessageCategory.Ame, 5);
            player.CellphoneItem.Messages.Add(new CellphoneItemMessage
            {
                Number = target.Cellphone,
                Message = mensagem,
                Type = CellphoneMessageType.Text,
            });
            await player.UpdateCellphoneDatabase();

            target.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(player.Cellphone)}: {mensagem}", Global.CELLPHONE_MAIN_COLOR);
            target.SendMessageToNearbyPlayers("recebe uma mensagem de texto.", MessageCategory.Ame, 5);
            target.CellphoneItem.Messages.Add(new CellphoneItemMessage
            {
                Number = player.Cellphone,
                Message = mensagem,
                Type = CellphoneMessageType.Text,
            });
            await target.UpdateCellphoneDatabase();
        }

        [Command("ligar", "/ligar (número ou nome do contato)")]
        public static async Task CMD_ligar(MyPlayer player, string numeroNomeContato) => await Functions.CMDLigar(player, numeroNomeContato);

        [Command("desligar", Aliases = ["des"])]
        public static async Task CMD_desligar(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneCall.Number == 0)
            {
                player.SendMessage(MessageType.Error, "Você não está uma ligação.");
                return;
            }

            player.SendMessageToNearbyPlayers("desliga a ligação.", MessageCategory.Ame, 5);
            player.SendMessage(MessageType.None, $"[CELULAR] Você desligou a ligação de {player.ObterNomeContato(player.CellphoneCall.Number)}.", Global.CELLPHONE_SECONDARY_COLOR);
            await player.EndCellphoneCall();
        }

        [Command("atender")]
        public static void CMD_atender(MyPlayer player)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneCall.Number == 0 || player.CellphoneCall.Type == CellphoneCallType.Answered || player.CellphoneCall.Origin)
            {
                player.SendMessage(MessageType.Error, "Seu celular não está tocando.");
                return;
            }

            player.CellphoneCall.Type = CellphoneCallType.Answered;
            player.SendMessageToNearbyPlayers("atende a ligação.", MessageCategory.Ame, 5);
            player.SendMessage(MessageType.None, $"[CELULAR] Você atendeu a ligação de {player.ObterNomeContato(player.CellphoneCall.Number)}.", Global.CELLPHONE_SECONDARY_COLOR);

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.CellphoneCall.Number == player.Cellphone);
            if (target != null)
            {
                target.CellphoneCall.Type = CellphoneCallType.Answered;
                target.SendMessage(MessageType.None, $"[CELULAR] Sua ligação para {target.ObterNomeContato(player.Cellphone)} foi atendida.", Global.CELLPHONE_SECONDARY_COLOR);
                target.TimerCelular?.Stop();
                target.TimerCelular = null;
            }
        }

        [Command("an", "/an (mensagem)", GreedyArg = true)]
        public async static Task CMD_an(MyPlayer player, string message)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (player.Money < Global.Parameter.AnnouncementValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.AnnouncementValue));
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
                player.SendMessage(MessageType.Error, $"O uso da central de anúncios estará disponível em {cooldown}.");
                return;
            }

            player.Character.AnnouncementLastUseDate = DateTime.Now;
            await player.RemoveStackedItem(ItemCategory.Money, Global.Parameter.AnnouncementValue);

            message = Functions.CheckFinalDot(message);
            foreach (var x in Global.SpawnedPlayers.Where(x => !x.User.AnnouncementToggle))
            {
                x.SendMessage(MessageType.None, $"[ANÚNCIO] {message} [CONTATO: {player.Cellphone}]", "#8EBE59");

                if (x.User.Staff != UserStaff.None)
                    x.SendMessage(MessageType.None, $"{player.Character.Name} [{player.SessionId}] ({player.User.Name}) enviou o anúncio.", Global.STAFF_COLOR);
            }

            await player.GravarLog(LogType.Advertisement, message, null);

            if (Global.DiscordClient == null
                || Global.DiscordClient.GetChannel(Global.AnnouncementDiscordChannel) is not SocketTextChannel channel)
                return;

            var embedBuilder = new Discord.EmbedBuilder
            {
                Title = $"Anúncio de #{player.Cellphone}",
                Description = message,
                Color = new Discord.Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
            };
            embedBuilder.WithFooter($"Enviado em {DateTime.Now}.");

            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [AsyncClientEvent(nameof(AdicionarContatoCelular))]
        public async Task AdicionarContatoCelular(MyPlayer player, uint numero, string nome)
        {
            var contato = player.CellphoneItem.Contacts.FirstOrDefault(x => x.Number == numero);
            if (contato == null)
                player.CellphoneItem.Contacts.Add(new CellphoneItemContact(numero, nome));
            else
                contato.Name = nome;

            await player.UpdateCellphoneDatabase();
        }

        [AsyncClientEvent(nameof(RemoverContatoCelular))]
        public async Task RemoverContatoCelular(MyPlayer player, uint numero)
        {
            player.CellphoneItem.Contacts.RemoveAll(x => x.Number == numero);
            await player.UpdateCellphoneDatabase();
        }

        [AsyncClientEvent(nameof(LigarContatoCelular))]
        public async Task LigarContatoCelular(MyPlayer player, uint numero) => await Functions.CMDLigar(player, numero.ToString());

        [AsyncClientEvent(nameof(EnviarLocalizacaoContatoCelular))]
        public async Task EnviarLocalizacaoContatoCelular(MyPlayer player, uint numero)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (numero == player.Cellphone)
            {
                player.SendMessage(MessageType.Error, "Você não pode enviar uma localização para você mesmo.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Cellphone == numero && numero > 0);
            if (target == null || target.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(numero)} está indisponível.", Global.CELLPHONE_SECONDARY_COLOR);
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.LocalizacaoCelular,
                SenderCharacterId = player.Character.Id,
                Value = [player.ICPosition.X.ToString(), player.ICPosition.Y.ToString()],
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.LocalizacaoCelular);
            target.Invites.Add(convite);

            player.SendMessage(MessageType.Success, $"Você solicitou o envio de uma localização para {player.ObterNomeContato(target.Cellphone)}.");
            player.CellphoneItem.Messages.Add(new CellphoneItemMessage
            {
                Number = target.Cellphone,
                Type = CellphoneMessageType.Location,
                LocationX = player.ICPosition.X,
                LocationY = player.ICPosition.Y,
            });
            await player.UpdateCellphoneDatabase();

            target.SendMessage(MessageType.Success, $"{target.ObterNomeContato(player.Cellphone)} solicitou enviar uma localização para você. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");
            target.CellphoneItem.Messages.Add(new CellphoneItemMessage
            {
                Number = player.Cellphone,
                Type = CellphoneMessageType.Location,
                LocationX = player.ICPosition.X,
                LocationY = player.ICPosition.Y,
            });
            await target.UpdateCellphoneDatabase();
        }

        [AsyncClientEvent(nameof(ModoAviaoCelular))]
        public async Task ModoAviaoCelular(MyPlayer player, bool modoAviao)
        {
            player.CellphoneItem.FlightMode = modoAviao;
            if (modoAviao)
                await player.EndCellphoneCall();
            await player.UpdateCellphoneDatabase();
        }

        [ClientEvent(nameof(RastrearPropriedade))]
        public void RastrearPropriedade(MyPlayer player, int propriedade)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.None)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.FlightMode)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            var prop = Global.Properties.FirstOrDefault(x => x.Number == propriedade);
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, $"Propriedade {propriedade} não existe.");
                return;
            }

            player.Emit("Server:SetWaypoint", prop.EntrancePosX, prop.EntrancePosY);
            player.SendMessage(MessageType.None, $"[CELULAR] Propriedade {propriedade} foi marcada no GPS.", Global.CELLPHONE_SECONDARY_COLOR);
        }
    }
}