using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class Radio 
    {
        [Command("canal", "/canal (slot [1-3]) (canal)")]
        public void CMD_canal(IPlayer player, int slot, int canal)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.CanalRadio == -1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um rádio.");
                return;
            }

            if (p.TimerFerido != null)
            {
                Functions.EnviarMensagem(p.Player, TipoMensagem.Erro, "Você está gravamente ferido.");
                return;
            }

            if (slot < 1 || slot > 3)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Slot deve ser entre 1 e 3.");
                return;
            }

            if (canal >= 911 && canal <= 920 && p.FaccaoBD?.Tipo != TipoFaccao.Policial)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 911 até 920 é reservado para facções policiais.");
                return;
            }

            if (canal >= 921 && canal <= 930 && p.FaccaoBD?.Tipo != TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 921 até 930 é reservado para facções médicas.");
                return;
            }

            if (canal >= 931 && canal <= 940 && p.FaccaoBD?.Tipo != TipoFaccao.Governo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 931 até 940 é reservado para o governo.");
                return;
            }

            if (canal == 999 && p.FaccaoBD?.Tipo != TipoFaccao.Policial && p.FaccaoBD?.Tipo != TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 999 é reservado para facções governamentais.");
                return;
            }

            if (slot == 1)
                p.CanalRadio = canal;
            else if (slot == 2)
                p.CanalRadio2 = canal;
            else if (slot == 3)
                p.CanalRadio3 = canal;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou seu canal de rádio do slot {slot} para {canal}.");
        }

        [Command("r", "/r (mensagem)", GreedyArg = true)]
        public void CMD_r(IPlayer player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 1, mensagem);

        [Command("r2", "/r2 (mensagem)", GreedyArg = true)]
        public void CMD_r2(IPlayer player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 2, mensagem);

        [Command("r3", "/r3 (mensagem)", GreedyArg = true)]
        public void CMD_r3(IPlayer player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 3, mensagem);
    }
}