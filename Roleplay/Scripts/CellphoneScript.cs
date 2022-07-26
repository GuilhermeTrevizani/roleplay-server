using AltV.Net;
using AltV.Net.Async;
using Roleplay.Factories;
using Roleplay.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class CellphoneScript : IScript
    {
        [AsyncClientEvent(nameof(AdicionarContatoCelular))]
        public async Task AdicionarContatoCelular(MyPlayer player, uint numero, string nome)
        {
            var contato = player.CellphoneItem.Contatos.FirstOrDefault(x => x.Numero == numero);
            if (contato == null)
                player.CellphoneItem.Contatos.Add(new CellphoneItemContact(numero, nome));
            else
                contato.Nome = nome;

            await player.UpdateCellphoneDatabase();
        }

        [AsyncClientEvent(nameof(RemoverContatoCelular))]
        public async Task RemoverContatoCelular(MyPlayer player, uint numero)
        {
            player.CellphoneItem.Contatos.RemoveAll(x => x.Numero == numero);
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

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.ModoAviao)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (numero == player.Cellphone)
            {
                player.SendMessage(MessageType.Error, "Você não pode enviar uma localização para você mesmo.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Cellphone == numero && numero > 0);
            if (target == null || target.CellphoneItem.ModoAviao)
            {
                player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(numero)} está indisponível.", Global.CELLPHONE_SECONDARY_COLOR);
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.LocalizacaoCelular,
                SenderCharacterId = player.Character.Id,
                Value = new string[] { player.ICPosition.X.ToString(), player.ICPosition.Y.ToString() },
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.LocalizacaoCelular);
            target.Invites.Add(convite);

            player.SendMessage(MessageType.Success, $"Você solicitou o envio de uma localização para {player.ObterNomeContato(target.Cellphone)}.");
            player.CellphoneItem.Mensagens.Add(new CellphoneItemMessage
            {
                Numero = target.Cellphone,
                Mensagem = $"{player.ICPosition.X}|{player.ICPosition.Y}",
                Tipo = CellphoneMessageType.Location,
            });
            await player.UpdateCellphoneDatabase();

            target.SendMessage(MessageType.Success, $"{target.ObterNomeContato(player.Cellphone)} solicitou enviar uma localização para você. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");
            target.CellphoneItem.Mensagens.Add(new CellphoneItemMessage
            {
                Numero = player.Cellphone,
                Mensagem = $"{player.ICPosition.X}|{player.ICPosition.Y}",
                Tipo = CellphoneMessageType.Location,
            });
            await target.UpdateCellphoneDatabase();
        }

        [AsyncClientEvent(nameof(ModoAviaoCelular))]
        public async Task ModoAviaoCelular(MyPlayer player, bool modoAviao)
        {
            player.CellphoneItem.ModoAviao = modoAviao;
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

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.ModoAviao)
            {
                player.SendMessage(MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            var prop = Global.Properties.FirstOrDefault(x => x.Id == propriedade);
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