using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands
{
    public class Cellphone
    {
        [Command("sms", "/sms (número ou nome do contato) (mensagem)", GreedyArg = true)]
        public void CMD_sms(IPlayer player, string numeroNomeContato, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            if (p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora.");
                return;
            }

            int.TryParse(numeroNomeContato, out int numero);
            if (numero == 0)
                numero = p.Contatos.FirstOrDefault(x => x.Nome.ToLower().Contains(numeroNomeContato.ToLower()))?.Celular ?? 0;

            if (numero == p.Celular)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode enviar um SMS para você mesmo.");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == numero && numero > 0);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Número indisponível.");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] SMS para {p.ObterNomeContato(numero)}: {mensagem}", Global.CorCelularSecundaria);
            Functions.SendMessageToNearbyPlayers(player, "envia uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {target.ObterNomeContato(p.Celular)}: {mensagem}", Global.CorCelular);
            Functions.SendMessageToNearbyPlayers(target.Player, "recebe uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
        }

        [Command("ligar", "/ligar (número ou nome do contato)")]
        public void CMD_ligar(IPlayer player, string numeroNomeContato) => Functions.LigarCelular(player, numeroNomeContato);

        [Command("desligar", Alias = "des")]
        public void CMD_desligar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
            if (target == null && p.NumeroLigacao > 0)
                target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao);

            if (target == null && p.NumeroLigacao == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Seu celular não está tocando ou você não está uma ligação.");
                return;
            }

            if (target != null)
            {
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] {target.ObterNomeContato(p.Celular)} desligou a ligação.", Global.CorCelularSecundaria);
                target.LimparLigacao();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você desligou a ligação de {p.ObterNomeContato(target != null ? target.Celular : p.NumeroLigacao)}.", Global.CorCelularSecundaria);
            p.LimparLigacao();
        }

        [Command("atender", Alias = "at")]
        public void CMD_atender(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            if (p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora.");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Seu celular não está tocando.");
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {target.ObterNomeContato(p.Celular)} foi atendida.", Global.CorCelularSecundaria);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você atendeu a ligação de {p.ObterNomeContato(target.Celular)}.", Global.CorCelularSecundaria);

            target.StatusLigacao = TipoStatusLigacao.EmLigacao;
            target.LimparLigacao(true);
        }

        [Command("celular", Alias = "cel")]
        public void CMD_celular(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            if (p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora.");
                return;
            }

            player.Emit("Server:AbrirCelular", p.Celular, JsonConvert.SerializeObject(p.Contatos.OrderBy(x => x.Nome).ToList()));
        }

        [Command("gps", "/gps (propriedade)")]
        public void CMD_gps(IPlayer player, int propriedade)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            if (p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora.");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == propriedade);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {propriedade} não existe.");
                return;
            }

            player.Emit("Server:SetWaypoint", prop.EntradaPosX, prop.EntradaPosY);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Propriedade {propriedade} foi marcada no GPS.", Global.CorCelularSecundaria);
        }

        [Command("localizacao", "/localizacao (número ou nome do contato)", Alias = "loc")]
        public void CMD_localizacao(IPlayer player, string numeroNomeContato) => Functions.EnviarLocalizacaoCelular(player, numeroNomeContato);

        [Command("an", "/an (mensagem)", GreedyArg = true)]
        public void CMD_an(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular.");
                return;
            }

            if (p.TimerFerido != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está ferido.");
                return;
            }

            if (p.Dinheiro < Global.Parametros.ValorAnuncio)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente (${Global.Parametros.ValorAnuncio:N0}).");
                return;
            }

            var segundos = 120;
            if (p.UsuarioBD.VIP == TipoVIP.Ouro)
                segundos = 30;
            else if (p.UsuarioBD.VIP == TipoVIP.Prata)
                segundos = 60;

            var cooldown = (p.DataUltimoUsoAnuncio ?? DateTime.MinValue).AddSeconds(segundos);
            if (cooldown > DateTime.Now)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"O uso da central de anúncios estará disponível em {cooldown}.");
                return;
            }

            p.DataUltimoUsoAnuncio = DateTime.Now;
            p.Dinheiro -= Global.Parametros.ValorAnuncio;
            p.SetDinheiro();

            foreach (var x in Global.PersonagensOnline.Where(x => !x.UsuarioBD.TogAnuncio))
                Functions.EnviarMensagem(x.Player, TipoMensagem.Nenhum, $"[CENTRAL DE ANÚNCIOS] {mensagem} | Contato: {p.Celular}", "#8EBE59");
        }
    }
}