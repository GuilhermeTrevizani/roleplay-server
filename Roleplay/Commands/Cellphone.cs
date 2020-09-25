using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using Roleplay.Models;
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

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] SMS para {p.ObterNomeContato(numero)}: {mensagem}", Constants.CorCelularSecundaria);
            Functions.SendMessageToNearbyPlayers(player, "envia uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] SMS de {target.ObterNomeContato(p.Celular)}: {mensagem}", Constants.CorCelular);
            Functions.SendMessageToNearbyPlayers(target.Player, "recebe uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
        }

        [Command("ligar", "/ligar (número ou nome do contato)")]
        public void CMD_ligar(IPlayer player, string numeroNomeContato)
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

            if (p.NumeroLigacao > 0 || Global.PersonagensOnline.Any(x => x.NumeroLigacao == p.Celular))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está em uma ligação.");
                return;
            }

            int.TryParse(numeroNomeContato, out int numero);
            if (numero == 0)
                numero = p.Contatos.FirstOrDefault(x => x.Nome.ToLower().Contains(numeroNomeContato.ToLower()))?.Celular ?? 0;

            if (numero == p.Celular)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode ligar para você mesmo.");
                return;
            }

            if (numero == 911)
            {
                p.NumeroLigacao = numero;
                p.StatusLigacao = TipoStatusLigacao.EmLigacao;
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.", Constants.CorCelularSecundaria);
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(numero)} diz: Central de emergência, deseja falar com PD, FD ou PDFD?", Constants.CorCelular);
                return;
            }

            if (numero == 5555555)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.", Constants.CorCelularSecundaria);
                if (Global.PersonagensOnline.Count(x => x.Emprego == TipoEmprego.Taxista && x.EmTrabalho) == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(numero)} diz: Desculpe, não temos nenhum taxista em serviço no momento.", Constants.CorCelular);
                    return;
                }

                p.NumeroLigacao = numero;
                p.StatusLigacao = TipoStatusLigacao.EmLigacao;
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(numero)} diz: Downtown Cab Company, para onde deseja ir?", Constants.CorCelular);
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == numero && numero > 0);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Número indisponível.");
                return;
            }

            if (target.NumeroLigacao > 0 || Global.PersonagensOnline.Any(x => x.NumeroLigacao == target.Celular))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(numero)} está ocupado.", Constants.CorCelularSecundaria);
                return;
            }

            p.NumeroLigacao = numero;
            p.StatusLigacao = TipoStatusLigacao.Nenhum;
            p.TimerCelular = new TagTimer(8000)
            {
                Tag = p.Codigo,
            };
            p.TimerCelular.Elapsed += TimerCelular_Elapsed;
            p.TimerCelular.Start();
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.", Constants.CorCelularSecundaria);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(p.Celular)}. (/at ou /des)", Constants.CorCelularSecundaria);
            Functions.SendMessageToNearbyPlayers(target.Player, $"O celular de {target.NomeIC} está tocando.", TipoMensagemJogo.Do, 5, true);
        }

        private void TimerCelular_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = (TagTimer)sender;
            timer.ElapsedCount++;
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == (int)timer.Tag);
            if (p == null)
            {
                timer?.Stop();
                return;
            }

            if (timer.ElapsedCount == 5)
            {
                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {p.ObterNomeContato(p.NumeroLigacao)} caiu após tocar 5 vezes.", Constants.CorCelularSecundaria);
                p.LimparLigacao();
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao);
            if (target == null)
            {
                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {p.ObterNomeContato(p.NumeroLigacao)} caiu.", Constants.CorCelularSecundaria);
                p.LimparLigacao();
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(p.Celular)}. (/at ou /des)", Constants.CorCelularSecundaria);
            Functions.SendMessageToNearbyPlayers(target.Player, $"O celular de {target.NomeIC} está tocando.", TipoMensagemJogo.Do, 5, true);
        }

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

            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Seu celular não está tocando ou você não está uma ligação.");
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] {target.ObterNomeContato(p.Celular)} desligou a ligação.", Constants.CorCelularSecundaria);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você desligou a ligação de {p.ObterNomeContato(target.Celular)}.", Constants.CorCelularSecundaria);

            p.LimparLigacao();
            target.LimparLigacao();
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

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {target.ObterNomeContato(p.Celular)} foi atendida.", Constants.CorCelularSecundaria);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Você atendeu a ligação de {p.ObterNomeContato(target.Celular)}.", Constants.CorCelularSecundaria);

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
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[CELULAR] Propriedade {propriedade} foi marcada no GPS.", Constants.CorCelularSecundaria);
        }

        [Command("localizacao", "/localizacao (número ou nome do contato)", Alias = "loc")]
        public void CMD_localizacao(IPlayer player, string numeroNomeContato)
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
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode enviar uma localização para você mesmo.");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == numero && numero > 0);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Número indisponível.");
                return;
            }

            var convite = new Convite()
            {
                Tipo = TipoConvite.LocalizacaoCelular,
                Personagem = p.Codigo,
                Valor = new string[] { p.PosicaoIC.X.ToString(), p.PosicaoIC.Y.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == TipoConvite.LocalizacaoCelular);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você solicitou o envio de uma localização para {target.Nome}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Solicitou enviar uma localização para você. (/ac {(int)convite.Tipo} para aceitar ou /rc {(int)convite.Tipo} para recusar)");
        }
    }
}