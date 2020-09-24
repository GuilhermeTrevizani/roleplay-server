using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Roleplay
{
    public static class Functions
    {
        public static string Criptografar(string texto)
        {
            var encodedValue = Encoding.UTF8.GetBytes(texto);
            var encryptedPassword = SHA512.Create().ComputeHash(encodedValue);

            var sb = new StringBuilder();
            foreach (var caracter in encryptedPassword)
                sb.Append(caracter.ToString("X2"));

            return sb.ToString();
        }

        public static bool VerificarBanimento(IPlayer player, Banimento ban)
        {
            if (ban == null)
                return true;

            using (var context = new DatabaseContext())
            {
                if (ban.Expiracao != null)
                {
                    if (DateTime.Now > ban.Expiracao)
                    {
                        context.Banimentos.Remove(ban);
                        context.SaveChanges();
                        return true;
                    }
                }

                var staff = context.Usuarios.FirstOrDefault(x => x.Codigo == ban.UsuarioStaff)?.Nome;
                var strBan = ban.Expiracao == null ? " permanentemente." : $". Seu banimento expira em: {ban.Expiracao?.ToString()}";

                player.Emit("Server:BaseHTML", $"Você está banido{strBan}<br/>Data: <strong>{ban.Data}</strong><br/>Motivo: <strong>{ban.Motivo}</strong><br/>Staffer: <strong>{staff}</strong>");
            }

            return false;
        }

        public static Personagem ObterPersonagem(IPlayer player) => Global.PersonagensOnline.FirstOrDefault(x => x?.Player?.HardwareIdHash == player?.HardwareIdHash);

        public static void GravarLog(TipoLog tipo, string descricao, Personagem origem, Personagem destino)
        {
            using var context = new DatabaseContext();
            context.Logs.Add(new Log()
            {
                Tipo = tipo,
                Descricao = descricao,
                PersonagemOrigem = origem.Codigo,
                IPOrigem = origem.IPUltimoAcesso,
                SocialClubOrigem = origem.SocialClubUltimoAcesso,
                PersonagemDestino = destino?.Codigo ?? 0,
                IPDestino = destino?.IPUltimoAcesso,
                SocialClubDestino = destino?.SocialClubUltimoAcesso ?? 0,
                HardwareIdHashOrigem = origem.HardwareIdHashUltimoAcesso,
                HardwareIdHashDestino = destino?.HardwareIdHashUltimoAcesso ?? 0,
                HardwareIdExHashOrigem = origem.HardwareIdExHashUltimoAcesso,
                HardwareIdExHashDestino = destino?.HardwareIdExHashUltimoAcesso ?? 0,
            });
            context.SaveChanges();
        }

        public static int ObterNovoID()
        {
            for (var i = 1; i <= Global.MaxPlayers; i++)
            {
                if (Global.PersonagensOnline.Any(x => x.ID == i))
                    continue;

                return i;
            }

            return 1;
        }

        public static Position ObterPosicaoPorInterior(TipoInterior tipo)
        {
            return tipo switch
            {
                TipoInterior.Motel => new Position(151.2564f, -1007.868f, -98.99999f),
                TipoInterior.CasaBaixa => new Position(265.9522f, -1007.485f, -101.0085f),
                TipoInterior.CasaMedia => new Position(346.4499f, -1012.996f, -99.19622f),
                TipoInterior.IntegrityWay28 => new Position(-31.34092f, -594.9429f, 80.0309f),
                TipoInterior.IntegrityWay30 => new Position(-17.61359f, -589.3938f, 90.11487f),
                TipoInterior.DellPerroHeights4 => new Position(-1452.225f, -540.4642f, 74.04436f),
                TipoInterior.DellPerroHeights7 => new Position(-1451.26f, -523.9634f, 56.92898f),
                TipoInterior.RichardMajestic2 => new Position(-912.6351f, -364.9724f, 114.2748f),
                TipoInterior.TinselTowers42 => new Position(-603.1113f, 58.93406f, 98.20017f),
                TipoInterior.EclipseTowers3 => new Position(-785.1537f, 323.8156f, 211.9973f),
                TipoInterior.WildOatsDrive3655 => new Position(-174.3753f, 497.3086f, 137.6669f),
                TipoInterior.NorthConkerAvenue2044 => new Position(341.9306f, 437.7751f, 149.3901f),
                TipoInterior.NorthConkerAvenue2045 => new Position(373.5803f, 423.7043f, 145.9078f),
                TipoInterior.HillcrestAvenue2862 => new Position(-682.3693f, 592.2678f, 145.393f),
                TipoInterior.HillcrestAvenue2868 => new Position(-758.4348f, 618.8454f, 144.1539f),
                TipoInterior.HillcrestAvenue2874 => new Position(-859.7643f, 690.8358f, 152.8607f),
                TipoInterior.WhispymoundDrive2677 => new Position(117.209f, 559.8086f, 184.3048f),
                TipoInterior.MadWayneThunder2133 => new Position(-1289.775f, 449.3125f, 97.90256f),
                TipoInterior.Modern1Apartment => new Position(-786.8663f, 315.7642f, 217.6385f),
                TipoInterior.Modern2Apartment => new Position(-786.9563f, 315.6229f, 187.9136f),
                TipoInterior.Modern3Apartment => new Position(-774.0126f, 342.0428f, 196.6864f),
                TipoInterior.Mody1Apartment => new Position(-787.0749f, 315.8198f, 217.6386f),
                TipoInterior.Mody2Apartment => new Position(-786.8195f, 315.5634f, 187.9137f),
                TipoInterior.Mody3Apartment => new Position(-774.1382f, 342.0316f, 196.6864f),
                TipoInterior.Vibrant1Apartment => new Position(-786.6245f, 315.6175f, 217.6385f),
                TipoInterior.Vibrant2Apartment => new Position(-786.9584f, 315.7974f, 187.9135f),
                TipoInterior.Vibrant3Apartment => new Position(-774.0223f, 342.1718f, 196.6863f),
                TipoInterior.Sharp1Apartment => new Position(-787.0902f, 315.7039f, 217.6384f),
                TipoInterior.Sharp2Apartment => new Position(-787.0155f, 315.7071f, 187.9135f),
                TipoInterior.Sharp3Apartment => new Position(-773.8976f, 342.1525f, 196.6863f),
                TipoInterior.Monochrome1Apartment => new Position(-786.9887f, 315.7393f, 217.6386f),
                TipoInterior.Monochrome2Apartment => new Position(-786.8809f, 315.6634f, 187.9136f),
                TipoInterior.Monochrome3Apartment => new Position(-774.0675f, 342.0773f, 196.6864f),
                TipoInterior.Seductive1Apartment => new Position(-787.1423f, 315.6943f, 217.6384f),
                TipoInterior.Seductive2Apartment => new Position(-787.0961f, 315.815f, 187.9135f),
                TipoInterior.Seductive3Apartment => new Position(-773.9552f, 341.9892f, 196.6862f),
                TipoInterior.Regal1Apartment => new Position(-787.029f, 315.7113f, 217.6385f),
                TipoInterior.Regal2Apartment => new Position(-787.0574f, 315.6567f, 187.9135f),
                TipoInterior.Regal3Apartment => new Position(-774.0109f, 342.0965f, 196.6863f),
                TipoInterior.Aqua1Apartment => new Position(-786.9469f, 315.5655f, 217.6383f),
                TipoInterior.Aqua2Apartment => new Position(-786.9756f, 315.723f, 187.9134f),
                TipoInterior.Aqua3Apartment => new Position(-774.0349f, 342.0296f, 196.6862f),
                TipoInterior.ArcadiusExecutiveRich => new Position(-141.1987f, -620.913f, 168.8205f),
                TipoInterior.ArcadiusExecutiveCool => new Position(-141.5429f, -620.9524f, 168.8204f),
                TipoInterior.ArcadiusExecutiveContrast => new Position(-141.2896f, -620.9618f, 168.8204f),
                TipoInterior.ArcadiusOldSpiceWarm => new Position(-141.4966f, -620.8292f, 168.8204f),
                TipoInterior.ArcadiusOldSpiceClassical => new Position(-141.3997f, -620.9006f, 168.8204f),
                TipoInterior.ArcadiusOldSpiceVintage => new Position(-141.5361f, -620.9186f, 168.8204f),
                TipoInterior.ArcadiusPowerBrokerIce => new Position(-141.392f, -621.0451f, 168.8204f),
                TipoInterior.ArcadiusPowerBrokeConservative => new Position(-141.1945f, -620.8729f, 168.8204f),
                TipoInterior.ArcadiusPowerBrokePolished => new Position(-141.4924f, -621.0035f, 168.8205f),
                TipoInterior.MazeBankExecutiveRich => new Position(-75.8466f, -826.9893f, 243.3859f),
                TipoInterior.MazeBankExecutiveCool => new Position(-75.49945f, -827.05f, 243.386f),
                TipoInterior.MazeBankExecutiveContrast => new Position(-75.49827f, -827.1889f, 243.386f),
                TipoInterior.MazeBankOldSpiceWarm => new Position(-75.44054f, -827.1487f, 243.3859f),
                TipoInterior.MazeBankOldSpiceClassical => new Position(-75.63942f, -827.1022f, 243.3859f),
                TipoInterior.MazeBankOldSpiceVintage => new Position(-75.47446f, -827.2621f, 243.386f),
                TipoInterior.MazeBankPowerBrokerIce => new Position(-75.56978f, -827.1152f, 243.3859f),
                TipoInterior.MazeBankPowerBrokeConservative => new Position(-75.51953f, -827.0786f, 243.3859f),
                TipoInterior.MazeBankPowerBrokePolished => new Position(-75.41915f, -827.1118f, 243.3858f),
                TipoInterior.LomBankExecutiveRich => new Position(-1579.756f, -565.0661f, 108.523f),
                TipoInterior.LomBankExecutiveCool => new Position(-1579.678f, -565.0034f, 108.5229f),
                TipoInterior.LomBankExecutiveContrast => new Position(-1579.583f, -565.0399f, 108.5229f),
                TipoInterior.LomBankOldSpiceWarm => new Position(-1579.702f, -565.0366f, 108.5229f),
                TipoInterior.LomBankOldSpiceClassical => new Position(-1579.643f, -564.9685f, 108.5229f),
                TipoInterior.LomBankOldSpiceVintage => new Position(-1579.681f, -565.0003f, 108.523f),
                TipoInterior.LomBankPowerBrokerIce => new Position(-1579.677f, -565.0689f, 108.5229f),
                TipoInterior.LomBankPowerBrokeConservative => new Position(-1579.708f, -564.9634f, 108.5229f),
                TipoInterior.LomBankPowerBrokePolished => new Position(-1579.693f, -564.8981f, 108.5229f),
                TipoInterior.MazeBankWestExecutiveRich => new Position(-1392.667f, -480.4736f, 72.04217f),
                TipoInterior.MazeBankWestExecutiveCool => new Position(-1392.542f, -480.4011f, 72.04211f),
                TipoInterior.MazeBankWestExecutiveContrast => new Position(-1392.626f, -480.4856f, 72.04212f),
                TipoInterior.MazeBankWestOldSpiceWarm => new Position(-1392.617f, -480.6363f, 72.04208f),
                TipoInterior.MazeBankWestOldSpiceClassical => new Position(-1392.532f, -480.7649f, 72.04207f),
                TipoInterior.MazeBankWestOldSpiceVintage => new Position(-1392.611f, -480.5562f, 72.04214f),
                TipoInterior.MazeBankWestPowerBrokerIce => new Position(-1392.563f, -480.549f, 72.0421f),
                TipoInterior.MazeBankWestPowerBrokeConservative => new Position(-1392.528f, -480.475f, 72.04206f),
                TipoInterior.MazeBankWestPowerBrokePolished => new Position(-1392.416f, -480.7485f, 72.04207f),
                _ => new Position(),
            };
        }

        public static void EnviarMensagem(IPlayer player, TipoMensagem tipoMensagem, string mensagem, string cor = "#FFFFFF", bool notify = false)
        {
            var gradient = new int[3];
            var icone = string.Empty;
            var type = "info";

            if (tipoMensagem == TipoMensagem.Sucesso)
            {
                cor = Global.CorSucesso;
                gradient = new int[] { 110, 180, 105 };
                icone = "check";
                type = "success";
            }
            else if (tipoMensagem == TipoMensagem.Erro || tipoMensagem == TipoMensagem.Punicao)
            {
                cor = Global.CorErro;
                gradient = new int[] { 255, 106, 77 };
                icone = "alert";
                type = "danger";
            }
            else if (tipoMensagem == TipoMensagem.Titulo)
            {
                cor = "#B0B0B0";
                gradient = new int[] { 176, 176, 176 };
                icone = "info";
            }

            if (notify)
            {
                player.Emit("chat:notify", mensagem, type);
                return;
            }

            var matches = new Regex("{#.*?}").Matches(mensagem).ToList();

            foreach (Match x in matches)
                mensagem = mensagem.Replace(x.Value, $"{(matches.IndexOf(x) != 0 ? "</span>" : string.Empty)}<span style='color:{x.Value.Replace("{", string.Empty).Replace("}", string.Empty)}'>");

            if (matches.Count > 0)
                mensagem += "</span>";

            player.Emit("chat:sendMessage", mensagem, cor, tipoMensagem == TipoMensagem.Nenhum ? null : gradient, icone);
        }

        public static Personagem ObterPersonagemPorIdNome(IPlayer player, string idNome, bool isPodeProprioPlayer = true)
        {
            int.TryParse(idNome, out int id);
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.ID == id);
            if (p != null)
            {
                if (!isPodeProprioPlayer && player == p.Player)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você.");
                    return null;
                }

                return p;
            }

            var ps = Global.PersonagensOnline.Where(x => x.Nome.ToLower().Contains(idNome.Replace("_", " ").ToLower())).ToList();
            if (ps.Count == 1)
            {
                if (!isPodeProprioPlayer && player == ps.FirstOrDefault().Player)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você.");
                    return null;
                }

                return ps.FirstOrDefault();
            }

            if (ps.Count > 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Mais de um jogador foi encontrado com a pesquisa: {idNome}");
                foreach (var pl in ps)
                    EnviarMensagem(player, TipoMensagem.Nenhum, $"[ID: {pl.ID}] {pl.Nome}");
            }
            else
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum jogador foi encontrado com a pesquisa: {idNome}");
            }

            return null;
        }

        public static void SalvarPersonagem(Personagem p, bool online = true)
        {
            if (p.EtapaPersonalizacao != TipoEtapaPersonalizacao.Concluido)
                return;

            p.Player.SetDateTime(DateTime.Now);

            var armas = (p.StringArmas ?? string.Empty).Split(";").Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new PersonagemArma()
                {
                    Arma = long.Parse(x.Split("|")[0]),
                    Municao = int.Parse(x.Split("|")[1]),
                });

            foreach (var x in p.Armas)
                x.Municao = armas.FirstOrDefault(y => y.Arma == x.Arma)?.Municao ?? 0;

            var dif = DateTime.Now - p.DataUltimaVerificacao;
            if (dif.TotalMinutes >= 1)
            {
                p.TempoConectado++;
                p.DataUltimaVerificacao = DateTime.Now;

                if (p.TempoConectado % 60 == 0)
                {
                    var temIncentivoInicial = false;
                    var salario = 0;
                    if (p.Faccao > 0)
                        salario += p.RankBD.Salario;
                    else if (p.Emprego > 0)
                        salario += Global.Parametros.ValorIncentivoGovernamental;

                    if (Convert.ToInt32(p.TempoConectado / 60) <= Global.Parametros.HorasIncentivoInicial)
                    {
                        temIncentivoInicial = true;
                        salario += Global.Parametros.ValorIncentivoInicial;
                    }

                    p.Banco += salario;
                    if (salario > 0)
                    {
                        EnviarMensagem(p.Player, TipoMensagem.Titulo, $"Seu salário de ${salario:N0} foi depositado no banco.");

                        if (p.Faccao > 0)
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Salário Facção: ${p.RankBD.Salario:N0}");

                        if (p.Emprego > 0)
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Incentivo Governamental: ${Global.Parametros.ValorIncentivoGovernamental:N0}");

                        if (temIncentivoInicial)
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"Incentivo Inicial: ${Global.Parametros.ValorIncentivoInicial:N0}");
                    }
                }

                if (p.IsEmTrabalhoAdministrativo)
                    p.UsuarioBD.TempoTrabalhoAdministrativo++;
            }

            if (!online && p.Celular > 0)
            {
                p.LimparLigacao();
                var pLigando = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
                if (pLigando != null)
                {
                    pLigando.LimparLigacao();
                    EnviarMensagem(pLigando.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {pLigando.ObterNomeContato(p.Celular)} caiu.", Constants.CorCelularSecundaria);
                }
            }

            using var context = new DatabaseContext();
            p.Online = online;
            p.Skin = p.Player.Model;
            p.PosX = p.Player.Position.X;
            p.PosY = p.Player.Position.Y;
            p.PosZ = p.Player.Position.Z;
            p.Vida = p.Player.Health;
            p.Colete = p.Player.Armor;
            p.Dimensao = p.Player.Dimension;
            p.IPL = JsonConvert.SerializeObject(p.IPLs);
            p.RotX = p.Player.Rotation.Roll;
            p.RotY = p.Player.Rotation.Pitch;
            p.RotZ = p.Player.Rotation.Yaw;
            p.DataUltimoAcesso = DateTime.Now;
            p.IPUltimoAcesso = ObterIP(p.Player);
            p.InformacoesPersonalizacao = JsonConvert.SerializeObject(p.PersonalizacaoDados);
            context.Personagens.Update(p);

            context.Database.ExecuteSqlRaw($"DELETE FROM PersonagensContatos WHERE Codigo = {p.Codigo}");
            context.Database.ExecuteSqlRaw($"DELETE FROM PersonagensRoupas WHERE Codigo = {p.Codigo}");
            context.Database.ExecuteSqlRaw($"DELETE FROM PersonagensAcessorios WHERE Codigo = {p.Codigo}");
            context.Database.ExecuteSqlRaw($"DELETE FROM PersonagensArmas WHERE Codigo = {p.Codigo}");

            if (p.Contatos.Count > 0)
                context.PersonagensContatos.AddRange(p.Contatos);

            if (p.Roupas.Count > 0)
                context.PersonagensRoupas.AddRange(p.Roupas);

            if (p.Acessorios.Count > 0)
                context.PersonagensAcessorios.AddRange(p.Acessorios);

            if (p.Armas.Count > 0)
                context.PersonagensArmas.AddRange(p.Armas);

            var usuario = context.Usuarios.FirstOrDefault(x => x.Codigo == p.UsuarioBD.Codigo);
            usuario.Staff = p.UsuarioBD.Staff;
            usuario.TempoTrabalhoAdministrativo = p.UsuarioBD.TempoTrabalhoAdministrativo;
            usuario.QuantidadeSOSAceitos = p.UsuarioBD.QuantidadeSOSAceitos;
            usuario.DataUltimoAcesso = p.DataUltimoAcesso;
            usuario.IPUltimoAcesso = p.IPUltimoAcesso;
            usuario.TimeStamp = p.UsuarioBD.TimeStamp;
            context.Usuarios.Update(usuario);

            context.SaveChanges();
        }

        public static void SendMessageToNearbyPlayers(IPlayer player, string message, TipoMensagemJogo type, float range, bool excludePlayer = false)
        {
            if (type == TipoMensagemJogo.Radio)
                excludePlayer = true;

            var p = ObterPersonagem(player);
            var distanceGap = range / 5;

            var targetList = Global.PersonagensOnline.Where(x => x.Player != null && x.Player.Dimension == player.Dimension).Select(x => x.Player).ToList();
            foreach (var target in targetList)
            {
                if (player != target || (player == target && !excludePlayer))
                {
                    var distance = player.Position.Distance(target.Position);

                    if (distance <= range)
                    {
                        var chatMessageColor = "#6E6E6E";

                        if (distance < distanceGap)
                            chatMessageColor = "#E6E6E6";
                        else if (distance < distanceGap * 2)
                            chatMessageColor = "#C8C8C8";
                        else if (distance < distanceGap * 3)
                            chatMessageColor = "#AAAAAA";
                        else if (distance < distanceGap * 4)
                            chatMessageColor = "#8C8C8C";

                        switch (type)
                        {
                            case TipoMensagemJogo.ChatICNormal:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} diz: {message}", chatMessageColor);
                                break;
                            case TipoMensagemJogo.ChatICGrito:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} grita: {message}", chatMessageColor);
                                break;
                            case TipoMensagemJogo.Me:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"* {p.NomeIC} {message}", "#C2A2DA");
                                break;
                            case TipoMensagemJogo.Do:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"* {message} (( {p.NomeIC} ))", "#C2A2DA");
                                break;
                            case TipoMensagemJogo.ChatOOC:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"(( {p.NomeIC} [{p.ID}]: {message} ))", "#BABABA");
                                break;
                            case TipoMensagemJogo.ChatICBaixo:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} diz [baixo]: {message}", chatMessageColor);
                                break;
                            case TipoMensagemJogo.Megafone:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} diz [megafone]: {message}", "#F2FF43");
                                break;
                            case TipoMensagemJogo.Celular:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} [celular]: {message}", chatMessageColor);
                                break;
                            case TipoMensagemJogo.Ame:
                                target.Emit("text:playerAction", player, $"* {p.NomeIC} {message}");
                                break;
                            case TipoMensagemJogo.Ado:
                                target.Emit("text:playerAction", player, $"* {message} (( {p.NomeIC} ))");
                                break;
                            case TipoMensagemJogo.Radio:
                                EnviarMensagem(target, TipoMensagem.Nenhum, $"{p.NomeIC} [rádio]: {message}", chatMessageColor);
                                break;
                        }
                    }
                }
            }
        }

        public static void MostrarStats(IPlayer player, Personagem p)
        {
            var html = $@"<div class='box-header with-border'>
                <h3>{p.NomeIC} [{p.Codigo}] ({DateTime.Now})<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            OOC: <strong>{p.UsuarioBD.Nome}</strong> | SocialClub: <strong>{p.Player.SocialClubId}</strong> | Registro: <strong>{p.DataRegistro}</strong><br/>
            Tempo Conectado (minutos): <strong>{p.TempoConectado}</strong> | Emprego: <strong>{ObterDisplayEnum(p.Emprego)}</strong> | Namechange: <strong>{(p.UsuarioBD.PossuiNamechange ? "SIM" : "NÃO")} {(p.StatusNamechange == TipoStatusNamechange.Bloqueado ? "(BLOQUEADO)" : string.Empty)}</strong><br/>
            Dinheiro: <strong>${p.Dinheiro:N0}</strong> | Banco: <strong>${p.Banco:N0}</strong><br/>
            Skin: <strong>{(PedModel)p.Player.Model}</strong> | Vida: <strong>{p.Player.Health - 100}</strong> | Colete: <strong>{p.Player.Armor}</strong><br/>";

            if (p.UsuarioBD.Staff > 0)
                html += $"Staff: <strong>{ObterDisplayEnum(p.UsuarioBD.Staff)} [{(int)p.UsuarioBD.Staff}]</strong> | Tempo Serviço Administrativo (minutos): <strong>{p.UsuarioBD.TempoTrabalhoAdministrativo}</strong> | SOSs Aceitos: <strong>{p.UsuarioBD.QuantidadeSOSAceitos}</strong><br/>";

            if (p.CanalRadio > -1)
                html += $"Canal Rádio 1: <strong>{p.CanalRadio}</strong> | Canal Rádio 2: <strong>{p.CanalRadio2}</strong> | Canal Rádio 3: <strong>{p.CanalRadio3}</strong><br/>";

            if (p.Faccao > 0)
                html += $"Facção: <strong>{p.FaccaoBD.Nome} [{p.Faccao}]</strong> | Rank: <strong>{p.RankBD.Nome} [{p.Rank}]</strong> | Salário: <strong>${p.RankBD.Salario:N0}</strong><br/>";

            if (p.Propriedades.Count > 0)
            {
                html += $"<h4>Propriedades</h4>";
                foreach (var prop in p.Propriedades)
                    html += $"Código: <strong>{prop.Codigo}</strong> | Valor: <strong>${prop.Valor:N0}</strong><br/>";
            }

            html += "</div>";

            player.Emit("Server:BaseHTML", html);
        }

        public static void EnviarMensagemCelular(Personagem p, Personagem target, string mensagem)
        {
            SendMessageToNearbyPlayers(p.Player, mensagem, TipoMensagemJogo.Celular, p.Player.Dimension > 0 ? 7.5f : 10.0f, false);

            if (target != null)
                EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"[CELULAR] {target.ObterNomeContato(p.Celular)} diz: {mensagem}", "#F0E90D");
        }

        public static void EnviarMensagemTipoFaccao(TipoFaccao tipo, string mensagem, bool isSomenteParaTrabalho, bool isCorFaccao)
        {
            var players = Global.PersonagensOnline.Where(x => x.FaccaoBD?.Tipo == tipo);

            if (isSomenteParaTrabalho)
                players = players.Where(x => x.IsEmTrabalho);

            foreach (var pl in players)
                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem, isCorFaccao ? $"#{pl.FaccaoBD.Cor}" : "#FFFFFF");
        }

        public static string GerarPlacaVeiculo()
        {
            var placa = string.Empty;
            var existePlaca = true;
            while (existePlaca)
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var random = new Random();
                placa = $"{chars[random.Next(25)]}{chars[random.Next(25)]}{random.Next(0, 99999).ToString().PadLeft(5, '0')}{chars[random.Next(25)]}";
                using var context = new DatabaseContext();
                existePlaca = context.Veiculos.Any(x => x.Placa == placa);
            }

            return placa;
        }

        public static List<string> ObterIPLsPorInterior(TipoInterior tipo)
        {
            return tipo switch
            {
                TipoInterior.Modern1Apartment => new List<string> { "apa_v_mp_h_01_a" },
                TipoInterior.Modern2Apartment => new List<string> { "apa_v_mp_h_01_c" },
                TipoInterior.Modern3Apartment => new List<string> { "apa_v_mp_h_01_b" },
                TipoInterior.Mody1Apartment => new List<string> { "apa_v_mp_h_02_a" },
                TipoInterior.Mody2Apartment => new List<string> { "apa_v_mp_h_02_c" },
                TipoInterior.Mody3Apartment => new List<string> { "apa_v_mp_h_02_b" },
                TipoInterior.Vibrant1Apartment => new List<string> { "apa_v_mp_h_03_a" },
                TipoInterior.Vibrant2Apartment => new List<string> { "apa_v_mp_h_03_c" },
                TipoInterior.Vibrant3Apartment => new List<string> { "apa_v_mp_h_03_b" },
                TipoInterior.Sharp1Apartment => new List<string> { "apa_v_mp_h_04_a" },
                TipoInterior.Sharp2Apartment => new List<string> { "apa_v_mp_h_04_c" },
                TipoInterior.Sharp3Apartment => new List<string> { "apa_v_mp_h_04_b" },
                TipoInterior.Monochrome1Apartment => new List<string> { "apa_v_mp_h_05_a" },
                TipoInterior.Monochrome2Apartment => new List<string> { "apa_v_mp_h_05_c" },
                TipoInterior.Monochrome3Apartment => new List<string> { "apa_v_mp_h_05_b" },
                TipoInterior.Seductive1Apartment => new List<string> { "apa_v_mp_h_06_a" },
                TipoInterior.Seductive2Apartment => new List<string> { "apa_v_mp_h_06_c" },
                TipoInterior.Seductive3Apartment => new List<string> { "apa_v_mp_h_06_b" },
                TipoInterior.Regal1Apartment => new List<string> { "apa_v_mp_h_07_a" },
                TipoInterior.Regal2Apartment => new List<string> { "apa_v_mp_h_07_c" },
                TipoInterior.Regal3Apartment => new List<string> { "apa_v_mp_h_07_b" },
                TipoInterior.Aqua1Apartment => new List<string> { "apa_v_mp_h_08_a" },
                TipoInterior.Aqua2Apartment => new List<string> { "apa_v_mp_h_08_c" },
                TipoInterior.Aqua3Apartment => new List<string> { "apa_v_mp_h_08_b" },
                TipoInterior.ArcadiusExecutiveRich => new List<string> { "ex_dt1_02_office_02b" },
                TipoInterior.ArcadiusExecutiveCool => new List<string> { "ex_dt1_02_office_02c" },
                TipoInterior.ArcadiusExecutiveContrast => new List<string> { "ex_dt1_02_office_02a" },
                TipoInterior.ArcadiusOldSpiceWarm => new List<string> { "ex_dt1_02_office_01a" },
                TipoInterior.ArcadiusOldSpiceClassical => new List<string> { "ex_dt1_02_office_01b" },
                TipoInterior.ArcadiusOldSpiceVintage => new List<string> { "ex_dt1_02_office_01c" },
                TipoInterior.ArcadiusPowerBrokerIce => new List<string> { "ex_dt1_02_office_03a" },
                TipoInterior.ArcadiusPowerBrokeConservative => new List<string> { "ex_dt1_02_office_03b" },
                TipoInterior.ArcadiusPowerBrokePolished => new List<string> { "ex_dt1_02_office_03c" },
                TipoInterior.MazeBankExecutiveRich => new List<string> { "ex_dt1_11_office_02b" },
                TipoInterior.MazeBankExecutiveCool => new List<string> { "ex_dt1_11_office_02c" },
                TipoInterior.MazeBankExecutiveContrast => new List<string> { "ex_dt1_11_office_02a" },
                TipoInterior.MazeBankOldSpiceWarm => new List<string> { "ex_dt1_11_office_01a" },
                TipoInterior.MazeBankOldSpiceClassical => new List<string> { "ex_dt1_11_office_01b" },
                TipoInterior.MazeBankOldSpiceVintage => new List<string> { "ex_dt1_11_office_01c" },
                TipoInterior.MazeBankPowerBrokerIce => new List<string> { "ex_dt1_11_office_03a" },
                TipoInterior.MazeBankPowerBrokeConservative => new List<string> { "ex_dt1_11_office_03b" },
                TipoInterior.MazeBankPowerBrokePolished => new List<string> { "ex_dt1_11_office_03c" },
                TipoInterior.LomBankExecutiveRich => new List<string> { "ex_sm_13_office_02b" },
                TipoInterior.LomBankExecutiveCool => new List<string> { "ex_sm_13_office_02c" },
                TipoInterior.LomBankExecutiveContrast => new List<string> { "ex_sm_13_office_02a" },
                TipoInterior.LomBankOldSpiceWarm => new List<string> { "ex_sm_13_office_01a" },
                TipoInterior.LomBankOldSpiceClassical => new List<string> { "ex_sm_13_office_01b" },
                TipoInterior.LomBankOldSpiceVintage => new List<string> { "ex_sm_13_office_01c" },
                TipoInterior.LomBankPowerBrokerIce => new List<string> { "ex_sm_13_office_03a" },
                TipoInterior.LomBankPowerBrokeConservative => new List<string> { "ex_sm_13_office_03b" },
                TipoInterior.LomBankPowerBrokePolished => new List<string> { "ex_sm_13_office_03c" },
                TipoInterior.MazeBankWestExecutiveRich => new List<string> { "ex_sm_15_office_02b" },
                TipoInterior.MazeBankWestExecutiveCool => new List<string> { "ex_sm_15_office_02c" },
                TipoInterior.MazeBankWestExecutiveContrast => new List<string> { "ex_sm_15_office_02a" },
                TipoInterior.MazeBankWestOldSpiceWarm => new List<string> { "ex_sm_15_office_01a" },
                TipoInterior.MazeBankWestOldSpiceClassical => new List<string> { "ex_sm_15_office_01b" },
                TipoInterior.MazeBankWestOldSpiceVintage => new List<string> { "ex_sm_15_office_01c" },
                TipoInterior.MazeBankWestPowerBrokerIce => new List<string> { "ex_sm_15_office_03a" },
                TipoInterior.MazeBankWestPowerBrokeConservative => new List<string> { "ex_sm_15_office_03b" },
                TipoInterior.MazeBankWestPowerBrokePolished => new List<string> { "ex_sm_15_office_03c" },
                _ => new List<string>(),
            };
        }

        public static bool ChecarAnimacoes(IPlayer player, bool pararAnim = false)
        {
            if (player.IsInVehicle)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação em um veículo.");
                return false;
            }

            var p = ObterPersonagem(player);
            if (p.Algemado)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação algemado.");
                return false;
            }

            if (p.TimerFerido != null)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação ferido.");
                return false;
            }

            if (pararAnim)
                p.StopAnimation();

            return true;
        }

        public static void EnviarMensagemChat(Personagem p, string message, TipoMensagemJogo tipoMensagemJogo)
        {
            if (string.IsNullOrWhiteSpace(message) || p == null)
                return;

            if (p.StatusLigacao > 0)
            {
                EnviarMensagemCelular(p, Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao), message);

                if (p.NumeroLigacao == 911)
                {
                    if (p.StatusLigacao == TipoStatusLigacao.EmLigacao)
                    {
                        if (message.ToUpper().Contains("LSPD") || message.ToUpper().Contains("PD"))
                            p.ExtraLigacao = "LSPD";
                        else if (message.ToUpper().Contains("LSFD") || message.ToUpper().Contains("FD"))
                            p.ExtraLigacao = "LSFD";
                        else if (message.ToUpper().Contains("PDFD") || message.ToUpper().Contains("FDPD"))
                            p.ExtraLigacao = "PDFD";

                        if (string.IsNullOrWhiteSpace(p.ExtraLigacao))
                        {
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: Não entendi sua mensagem. Deseja falar com PD, FD ou PDFD?", Constants.CorCelular);
                            return;
                        }

                        p.StatusLigacao = TipoStatusLigacao.AguardandoInformacao;
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: Qual sua emergência?", Constants.CorCelular);
                        return;
                    }

                    if (p.StatusLigacao == TipoStatusLigacao.AguardandoInformacao)
                    {
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: Nossas unidades foram alertadas.", Constants.CorCelular);

                        var tipoFaccao = p.ExtraLigacao == "LSPD" ? TipoFaccao.Policial : TipoFaccao.Medica;

                        var pos = ObterPosicaoPlayerIC(p);
                        void Enviar911()
                        {
                            var ligacao911 = new Ligacao911()
                            {
                                Tipo = tipoFaccao,
                                Celular = p.Celular,
                                PosX = pos.X,
                                PosY = pos.Y,
                                PosZ = pos.Z,
                                Mensagem = message,
                                ID = Global.Ligacoes911.Select(x => x.ID).DefaultIfEmpty(0).Max() + 1,
                            };
                            using var context = new DatabaseContext();
                            context.Ligacoes911.Add(ligacao911);
                            context.SaveChanges();
                            Global.Ligacoes911.Add(ligacao911);

                            EnviarMensagemTipoFaccao(tipoFaccao, $"Central de Emergência | Ligação 911 {{#FFFFFF}}#{ligacao911.ID}", true, true);
                            EnviarMensagemTipoFaccao(tipoFaccao, $"De: {{#FFFFFF}}{p.Celular}", true, true);
                            EnviarMensagemTipoFaccao(tipoFaccao, $"Localização: {{#FFFFFF}}{p.AreaName} - {p.ZoneName}", true, true);
                            EnviarMensagemTipoFaccao(tipoFaccao, $"Mensagem: {{#FFFFFF}}{message}", true, true);
                        }

                        Enviar911();

                        if (p.ExtraLigacao == "PDFD")
                        {
                            tipoFaccao = TipoFaccao.Policial;
                            Enviar911();
                        }

                        p.LimparLigacao();
                    }
                }
                else if (p.NumeroLigacao == 5555555)
                {
                    EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(5555555)} diz: Nossos taxistas em serviço foram alertados e você receberá um SMS de confirmação.", Constants.CorCelular);

                    p.AguardandoTipoServico = (int)TipoEmprego.Taxista;

                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"Downtown Cab Company | Solicitação de Táxi {{#FFFFFF}}#{p.Codigo}", Constants.CorCelular);
                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"De: {{#FFFFFF}}{p.Celular}", Constants.CorCelular);
                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"Localização: {{#FFFFFF}}{p.AreaName} - {p.ZoneName}", Constants.CorCelular);
                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"Destino: {{#FFFFFF}}{message}", Constants.CorCelular);

                    p.LimparLigacao();
                }

                return;
            }

            var targetLigacao = Global.PersonagensOnline.FirstOrDefault(x => x.StatusLigacao > 0 && x.NumeroLigacao == p.Celular);
            if (targetLigacao != null)
            {
                EnviarMensagemCelular(p, targetLigacao, message);
                return;
            }

            SendMessageToNearbyPlayers(p.Player, message, tipoMensagemJogo, p.Player.Dimension > 0 ? 7.5f : 10.0f);
        }

        public static void EnviarMensagemRadio(Personagem p, int slot, string mensagem)
        {
            if (p.CanalRadio == -1)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você não possui um rádio.");
                return;
            }

            if (p.TimerFerido != null)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você está gravamente ferido.");
                return;
            }

            var canal = p.CanalRadio;
            if (slot == 2)
                canal = p.CanalRadio2;
            else if (slot == 3)
                canal = p.CanalRadio3;

            if (canal == 0)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Seu slot {slot} do rádio não possui um canal configurado.");
                return;
            }

            if (((canal >= 911 && canal <= 940) || canal == 999) && !p.IsEmTrabalho)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Você só pode falar no canal {canal} quando estiver em serviço.");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.CanalRadio == canal || x.CanalRadio2 == canal || x.CanalRadio3 == canal))
            {
                if (!pl.IsEmTrabalho && ((canal >= 911 && canal <= 940) || canal == 999))
                    continue;

                var slotPl = 1;
                if (pl.CanalRadio2 == canal)
                    slotPl = 2;
                else if (pl.CanalRadio3 == canal)
                    slotPl = 3;

                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, $"[S:{slotPl} C:{canal}] {p.Nome}: {mensagem}", slotPl == 1 ? "#FFFF9B" : "#9e8d66");
            }

            EnviarMensagemChat(p, mensagem, TipoMensagemJogo.Radio);
        }

        public static string ObterDisplayEnum(Enum valor)
        {
            var fi = valor.GetType().GetField(valor.ToString());
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes?.Length > 0)
                return attributes.FirstOrDefault().Name;
            return valor.ToString();
        }

        public static void EnviarMensagemEmprego(TipoEmprego tipo, string mensagem, string cor = "#FFFFFF")
        {
            foreach (var pl in Global.PersonagensOnline.Where(x => x.Emprego == tipo && x.IsEmTrabalho))
                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem, cor);
        }

        public static string ObterIP(IPlayer player) => player != null ? player.Ip.Replace("::ffff:", string.Empty) : string.Empty;

        public static TextDraw CriarTextDraw(string nome, Position position, float range, float size, int font, Rgba color, int dimension)
        {
            var textDraw = new TextDraw()
            {
                Codigo = Global.TextDraws.Select(x => x.Codigo).DefaultIfEmpty(0).Max() + 1,
                Nome = nome,
                Position = position,
                Range = range,
                Size = size,
                Font = font,
                Color = color,
                Dimension = dimension,
            };
            Global.TextDraws.Add(textDraw);

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                textDraw.CriarIdentificador(x.Player);

            return textDraw;
        }

        public static void RemoverTextDraw(TextDraw textDraw)
        {
            if (textDraw == null)
                return;

            foreach (var x in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                textDraw.DeletarIdentificador(x.Player);
        }

        public static string ObterParteCorpo(sbyte parte)
        {
            return parte switch
            {
                0 => "Pélvis",
                1 => "Quadril Esquerdo",
                2 => "Perna Esquerda",
                3 => "Pé Esquerdo",
                4 => "Quadril Direito",
                5 => "Perna Direita",
                6 => "Pé Direito",
                7 => "Torso Inferior",
                8 => "Torso Superior",
                9 => "Peito",
                10 => "Sob o Pescoço",
                11 => "Ombro Esquerdo",
                12 => "Braço Esquerdo",
                13 => "Cotovelo Esquerdo",
                14 => "Pulso Esquerdo",
                15 => "Ombro Direito",
                16 => "Braço Direito",
                17 => "Cotovelo Direito",
                18 => "Pulso Direito",
                19 => "Pescoço",
                20 => "Cabeça",
                _ => "Desconhecida",
            };
        }

        public static Position ObterPosicaoPlayerIC(Personagem p)
        {
            if (p.Dimensao == 0)
                return p.Player.Position;

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == p.Dimensao);
            return new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ);
        }

        public static bool ValidarEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static async void EnviarEmail(string email, string titulo, string mensagem)
        {
            try
            {
                var msg = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("naoresponda@segundavida.com.br", "Segunda Vida Roleplay"),
                    Subject = titulo,
                    Body = mensagem,
                    BodyEncoding = Encoding.UTF8,
                };
                msg.To.Add(email);

                var clienteSmtp = new SmtpClient("segundavida.com.br")
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("naoresponda@segundavida.com.br", "3sEuV,%GKqg_"),
                    Port = 587,
                };

                await clienteSmtp.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                RecuperarErro(ex);
            }
        }

        public static void RecuperarErro(Exception ex)
        {
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));

                var listaErros = new List<BeautifulException>();

                var path = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(path, "log");
                Directory.CreateDirectory(path);

                path = Path.Combine(path, "erros.json");
                if (File.Exists(path))
                {
                    listaErros = JsonConvert.DeserializeObject<List<BeautifulException>>(File.ReadAllText(path));
                }

                var exception = new BeautifulException()
                {
                    Exception = ex,
                };
                listaErros.Add(exception);

                File.WriteAllText(path, JsonConvert.SerializeObject(listaErros));
            }
            catch (Exception ex1)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex1));
            }
        }

        public static string GerarStringAleatoria(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, tamanho).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}