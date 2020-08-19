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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public static void LogarPersonagem(IPlayer player, Personagem p)
        {
            player.Emit("Server:SelecionarPersonagem");
            player.Emit("nametags:Config", true);
            player.Emit("chat:activateTimeStamp", p.TimeStamp);

            foreach (var x in Global.Blips)
                x.CriarIdentificador(player);

            foreach (var x in Global.TextDraws)
                x.CriarIdentificador(player);

            player.SetSyncedMetaData("nametag", p.Nome);
            player.Dimension = (int)p.Dimensao;
            p.IPLs = JsonConvert.DeserializeObject<List<string>>(p.IPL);
            p.SetarIPLs();
            player.SetDateTime(DateTime.Now);
            player.Health = (ushort)(p.Vida + 100);
            player.Armor = (ushort)p.Colete;
            player.Model = (uint)p.Skin;
            p.SetDinheiro();
            player.SetWeather(Global.Weather);

            using (var context = new DatabaseContext())
            {
                p.Contatos = context.PersonagensContatos.Where(x => x.Codigo == p.Codigo).ToList();

                var roupas = context.PersonagensRoupas.Where(x => x.Codigo == p.Codigo).ToList();
                foreach (var x in roupas)
                    p.SetClothes(x.Slot, x.Drawable, x.Texture);

                var acessorios = context.PersonagensAcessorios.Where(x => x.Codigo == p.Codigo).ToList();
                foreach (var x in acessorios)
                    p.SetAccessories(x.Slot, x.Drawable, x.Texture);

                p.Armas = context.PersonagensArmas.Where(x => x.Codigo == p.Codigo).ToList();
                foreach (var x in p.Armas)
                {
                    player.GiveWeapon((WeaponModel)x.Arma, x.Municao, false);
                    player.SetWeaponTintIndex((WeaponModel)x.Arma, (byte)x.Pintura);
                    foreach (var c in JsonConvert.DeserializeObject<List<uint>>(x.Componentes))
                        player.AddWeaponComponent((WeaponModel)x.Arma, c);
                }

                if (Global.PersonagensOnline.Count > Global.Parametros.RecordeOnline)
                {
                    Global.Parametros.RecordeOnline = Global.PersonagensOnline.Count;
                    context.Parametros.Update(Global.Parametros);
                    context.SaveChanges();
                }
            }

            player.Spawn(new Position(p.PosX, p.PosY, p.PosZ));
            player.Rotation = new Position(p.RotX, p.RotY, p.RotZ);

            GravarLog(TipoLog.Entrada, string.Empty, p, null);
        }

        public static Personagem ObterPersonagem(IPlayer player)
        {
            return Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == (long)(player?.SocialClubId ?? 0));
        }

        public static void GravarLog(TipoLog tipo, string descricao, Personagem origem, Personagem destino)
        {
            using var context = new DatabaseContext();
            context.Logs.Add(new Log()
            {
                Data = DateTime.Now,
                Tipo = tipo,
                Descricao = descricao,
                PersonagemOrigem = origem.Codigo,
                IPOrigem = ObterIP(origem.Player),
                SocialClubOrigem = (long)origem.Player.SocialClubId,
                PersonagemDestino = destino?.Codigo ?? 0,
                IPDestino = ObterIP(destino?.Player),
                SocialClubDestino = (long)(destino?.Player?.SocialClubId ?? 0),
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
                cor = "#6EB469";
                gradient = new int[] { 110, 180, 105 };
                icone = "check";
                type = "success";
            }
            else if (tipoMensagem == TipoMensagem.Erro || tipoMensagem == TipoMensagem.Punicao)
            {
                cor = "#FF6A4D";
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
                player.Emit("chat:notify", mensagem, type);
            else
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
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você!");
                    return null;
                }

                return p;
            }

            var ps = Global.PersonagensOnline.Where(x => x.Nome.ToLower().Contains(idNome.Replace("_", " ").ToLower())).ToList();
            if (ps.Count == 1)
            {
                if (!isPodeProprioPlayer && player == ps.FirstOrDefault().Player)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você!");
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
            p.Player.SetDateTime(DateTime.Now);
            p.Player.GetSyncedMetaData("armas", out string weapons);

            var armas = (weapons ?? string.Empty).Split(";").Where(x => !string.IsNullOrWhiteSpace(x))
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

                if (p.TempoPrisao > 0)
                {
                    p.TempoPrisao--;
                    if (p.TempoPrisao == 0)
                    {
                        p.Player.Position = new Position(432.8367f, -981.7594f, 30.71048f);
                        p.Player.Rotation = new Rotation(0, 0, 86.37479f);
                        EnviarMensagem(p.Player, TipoMensagem.Sucesso, $"Seu tempo de prisão acabou e você foi libertado!");
                    }
                }

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
                        EnviarMensagem(p.Player, TipoMensagem.Titulo, $"Seu salário de ${salario:N0} foi depositado no banco!");

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
                    EnviarMensagem(pLigando.Player, TipoMensagem.Nenhum, $"[CELULAR] Sua ligação para {pLigando.ObterNomeContato(p.Celular)} caiu!", Constants.CorCelularSecundaria);
                }
            }

            using var context = new DatabaseContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == p.Codigo);
            personagem.Online = online;
            personagem.Skin = p.Player.Model;
            personagem.PosX = p.Player.Position.X;
            personagem.PosY = p.Player.Position.Y;
            personagem.PosZ = p.Player.Position.Z;
            personagem.Vida = p.Player.Health;
            personagem.Colete = p.Player.Armor;
            personagem.Dimensao = p.Player.Dimension;
            personagem.TempoConectado = p.TempoConectado;
            personagem.Faccao = p.Faccao;
            personagem.Rank = p.Rank;
            personagem.Dinheiro = p.Dinheiro;
            personagem.Celular = p.Celular;
            personagem.Banco = p.Banco;
            personagem.IPL = JsonConvert.SerializeObject(p.IPLs);
            personagem.CanalRadio = p.CanalRadio;
            personagem.CanalRadio2 = p.CanalRadio2;
            personagem.CanalRadio3 = p.CanalRadio3;
            personagem.TempoPrisao = p.TempoPrisao;
            personagem.RotX = p.Player.Rotation.Roll;
            personagem.RotY = p.Player.Rotation.Pitch;
            personagem.RotZ = p.Player.Rotation.Yaw;
            personagem.DataMorte = p.DataMorte;
            personagem.MotivoMorte = p.MotivoMorte;
            personagem.Emprego = p.Emprego;
            personagem.DataUltimoAcesso = DateTime.Now;
            personagem.IPUltimoAcesso = ObterIP(p.Player);
            personagem.TimeStamp = p.TimeStamp;
            context.Personagens.Update(personagem);

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
            usuario.DataUltimoAcesso = personagem.DataUltimoAcesso;
            usuario.IPUltimoAcesso = personagem.IPUltimoAcesso;
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
                        var chatMessageColor = GetChatMessageColor(distance, distanceGap);
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

        private static string GetChatMessageColor(float distance, float distanceGap)
        {
            if (distance < distanceGap)
                return "#E6E6E6";
            else if (distance < distanceGap * 2)
                return "#C8C8C8";
            else if (distance < distanceGap * 3)
                return "#AAAAAA";
            else if (distance < distanceGap * 4)
                return "#8C8C8C";

            return "#6E6E6E";
        }

        public static void MostrarStats(IPlayer player, Personagem p)
        {
            var html = $@"<div class='box-header with-border'>
                <h3>{p.NomeIC} [{p.Codigo}] ({DateTime.Now})<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            OOC: <strong>{p.UsuarioBD.Nome}</strong> | SocialClub: <strong>{p.Player.SocialClubId}</strong> | Registro: <strong>{p.DataRegistro}</strong><br/>
            Tempo Conectado (minutos): <strong>{p.TempoConectado}</strong> | Celular: <strong>{p.Celular}</strong> | Emprego: <strong>{ObterDisplayEnum(p.Emprego)}</strong><br/>
            Sexo: <strong>{p.Sexo}</strong> | Nascimento: <strong>{p.DataNascimento.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - p.DataNascimento).TotalDays / 365):N0} anos)</strong> | Dinheiro: <strong>${p.Dinheiro:N0}</strong> | Banco: <strong>${p.Banco:N0}</strong><br/>
            Skin: <strong>{(PedModel)p.Player.Model}</strong> | Vida: <strong>{p.Player.Health - 100}</strong> | Colete: <strong>{p.Player.Armor}</strong> | Tempo de Prisão: <strong>{p.TempoPrisao}</strong><br/>";

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
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação em um veículo!");
                return false;
            }

            var p = ObterPersonagem(player);
            if (p.Algemado)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação algemado!");
                return false;
            }

            if (p.TimerFerido != null)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação ferido!");
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
                        if (message.ToUpper().Contains("LSPD"))
                            p.ExtraLigacao = "LSPD";
                        else if (message.ToUpper().Contains("LSFD"))
                            p.ExtraLigacao = "LSFD";

                        if (string.IsNullOrWhiteSpace(p.ExtraLigacao))
                        {
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: Não entendi sua mensagem. Deseja falar com LSPD ou LSFD?", Constants.CorCelular);
                            return;
                        }

                        p.StatusLigacao = TipoStatusLigacao.AguardandoInformacao;
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: {p.ExtraLigacao}, qual sua emergência?", Constants.CorCelular);
                        return;
                    }

                    if (p.StatusLigacao == TipoStatusLigacao.AguardandoInformacao)
                    {
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(911)} diz: Nossas unidades foram alertadas!", Constants.CorCelular);

                        var tipoFaccao = p.ExtraLigacao == "LSPD" ? TipoFaccao.Policial : TipoFaccao.Medica;
                        var pos = ObterPosicaoPlayerIC(p);

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

                        EnviarMensagemTipoFaccao(tipoFaccao, $"Central de Emergência | Ligação 911 #{ligacao911.ID}", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"De: {p.Celular}", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"Mensagem: {message}", true, true);

                        p.LimparLigacao();
                    }
                }
                else if (p.NumeroLigacao == 5555555)
                {
                    EnviarMensagem(p.Player, TipoMensagem.Nenhum, $"[CELULAR] {p.ObterNomeContato(5555555)} diz: Nossos taxistas em serviço foram alertados e você receberá um SMS de confirmação!", Constants.CorCelular);

                    p.AguardandoTipoServico = (int)TipoEmprego.Taxista;

                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"Downtown Cab Company | Solicitação de Táxi #{p.Codigo}", Constants.CorCelular);
                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"De: {p.Celular}", Constants.CorCelular);
                    EnviarMensagemEmprego(TipoEmprego.Taxista, $"Destino: {message}", Constants.CorCelular);

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
            if (p == null)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.CanalRadio == -1)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você não possui um rádio!");
                return;
            }

            if (p.TempoPrisao > 0)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você está preso!");
                return;
            }

            if (p.TimerFerido != null)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você está gravamente ferido!");
                return;
            }

            var canal = p.CanalRadio;
            if (slot == 2)
                canal = p.CanalRadio2;
            else if (slot == 3)
                canal = p.CanalRadio3;

            if (canal == 0)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Seu slot {slot} do rádio não possui um canal configurado!");
                return;
            }

            if ((canal == 911 || canal == 912 || canal == 999) && !p.IsEmTrabalho)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Você só pode falar no canal {canal} quando estiver em serviço!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.CanalRadio == canal || x.CanalRadio2 == canal || x.CanalRadio3 == canal))
            {
                if (!pl.IsEmTrabalho && (canal == 911 || canal == 912 || canal == 999))
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

        public static void CarregarConcessionarias()
        {
            Global.Concessionarias = new List<Concessionaria>()
            {
                new Concessionaria()
                {
                    Nome = "Concessionária de Carros e Motos",
                    Tipo = TipoPreco.CarrosMotos,
                    PosicaoCompra = new Position(-38.63479f, -1109.706f, 26.43781f),
                    PosicaoSpawn = new Position(-60.224174f, -1106.1494f, 25.909912f),
                    RotacaoSpawn = new Position(-0.015625f, 0, 1.203125f),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Barcos",
                    Tipo = TipoPreco.Barcos,
                    PosicaoCompra = new Position(-787.1262f, -1354.725f, 5.150271f),
                    PosicaoSpawn = new Position(-805.2659f, -1418.4264f, 0.33190918f),
                    RotacaoSpawn = new Position(-0.015625f, 0, 0.859375f),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Helicópteros",
                    Tipo = TipoPreco.Helicopteros,
                    PosicaoCompra = new Position(-753.5287f, -1512.43f, 5.020952f),
                    PosicaoSpawn = new Position(- 745.4902f, -1468.695f, 5.099712f),
                    RotacaoSpawn = new Position(0, 0, 328.6675f),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária Industrial",
                    Tipo = TipoPreco.Industrial,
                    PosicaoCompra = new Position(473.9496f, -1951.891f, 24.6132f),
                    PosicaoSpawn = new Position(468.1417f, -1957.425f, 24.72257f),
                    RotacaoSpawn = new Position(0, 0, 208.0628f),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Aviões",
                    Tipo = TipoPreco.Avioes,
                    PosicaoCompra = new Position(1725.616f, 3291.571f, 41.19078f),
                    PosicaoSpawn = new Position(1712.708f, 3252.634f, 41.67871f),
                    RotacaoSpawn = new Position(0, 0, 122.1655f),
                },
            };

            foreach (var c in Global.Concessionarias)
            {
                CriarTextDraw(c.Nome, c.PosicaoCompra, 5, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);
                CriarTextDraw("Use /comprar", new Position(c.PosicaoCompra.X, c.PosicaoCompra.Y, c.PosicaoCompra.Z - 0.15f), 5, 0.4f, 4, new Rgba(255, 255, 255, 255), 0);
            }
        }

        public static string ObterDisplayEnum(Enum valor)
        {
            var fi = valor.GetType().GetField(valor.ToString());
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes?.Length > 0)
                return attributes.FirstOrDefault().Name;
            return valor.ToString();
        }

        public static void CarregarEmpregos()
        {
            Global.Empregos = new List<Emprego>()
            {
                new Emprego()
                {
                    Tipo = TipoEmprego.Taxista,
                    Posicao = new Position(895.0308f, -179.1359f, 74.70036f),
                },
            };

            foreach (var c in Global.Empregos)
            {
                var nome = ObterDisplayEnum(c.Tipo);
                CriarTextDraw($"Emprego de {nome}", c.Posicao, 5, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);
                CriarTextDraw($"Use /emprego para se tornar um {nome.ToLower()}", new Position(c.Posicao.X, c.Posicao.Y, c.Posicao.Z - 0.15f), 5, 0.4f, 4, new Rgba(255, 255, 255, 255), 0);
            }
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
                16 => "Braço Direto",
                17 => "Cotovelo Direto",
                18 => "Pulso Direito",
                19 => "Pescoço",
                20 => "Cabeça",
                _ => "Desconhecida",
            };
        }

        public static void CarregarWeaponComponents()
        {
            Global.WeaponComponents = new List<WeaponComponent>()
            {
                new WeaponComponent(WeaponModel.BrassKnuckles, "BaseModel", 0xF3462F33),
                new WeaponComponent(WeaponModel.BrassKnuckles, "ThePimp", 0xC613F685),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheBallas", 0xEED9FD63),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheHustler", 0x50910C31),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheRock", 0x9761D9DC),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheHater", 0x7DECFE30),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheLover", 0x3F4E8AA6),
                new WeaponComponent(WeaponModel.BrassKnuckles, "ThePlayer", 0x8B808BB),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheKing", 0xE28BABEF),
                new WeaponComponent(WeaponModel.BrassKnuckles, "TheValor", 0x7AF3F785),
                new WeaponComponent(WeaponModel.Switchblade, "DefaultHandle", 0x9137A500),
                new WeaponComponent(WeaponModel.Switchblade, "VIPVariant", 0x5B3E7DB6),
                new WeaponComponent(WeaponModel.Switchblade, "BodyguardVariant", 0xE7939662),
                new WeaponComponent(WeaponModel.Pistol, "DefaultClip", 0xFED0FD71),
                new WeaponComponent(WeaponModel.Pistol, "ExtendedClip", 0xED265A1C),
                new WeaponComponent(WeaponModel.Pistol, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.Pistol, "Suppressor", 0x65EA7EBB),
                new WeaponComponent(WeaponModel.Pistol, "YusufAmirLuxuryFinish", 0xD7391086),
                new WeaponComponent(WeaponModel.CombatPistol, "DefaultClip", 0x721B079),
                new WeaponComponent(WeaponModel.CombatPistol, "ExtendedClip", 0xD67B4F2D),
                new WeaponComponent(WeaponModel.CombatPistol, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.CombatPistol, "Suppressor", 0xC304849A),
                new WeaponComponent(WeaponModel.CombatPistol, "YusufAmirLuxuryFinish", 0xC6654D72),
                new WeaponComponent(WeaponModel.APPistol, "DefaultClip", 0x31C4B22A),
                new WeaponComponent(WeaponModel.APPistol, "ExtendedClip", 0x249A17D5),
                new WeaponComponent(WeaponModel.APPistol, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.APPistol, "Suppressor", 0xC304849A),
                new WeaponComponent(WeaponModel.APPistol, "GildedGunMetalFinish", 0x9B76C72C),
                new WeaponComponent(WeaponModel.Pistol50, "DefaultClip", 0x2297BE19),
                new WeaponComponent(WeaponModel.Pistol50, "ExtendedClip", 0xD9D3AC92),
                new WeaponComponent(WeaponModel.Pistol50, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.Pistol50, "Suppressor", 0xA73D4664),
                new WeaponComponent(WeaponModel.Pistol50, "PlatinumPearlDeluxeFinish", 0x77B8AB2F),
                new WeaponComponent(WeaponModel.HeavyRevolver, "VIPVariant", 0x16EE3040),
                new WeaponComponent(WeaponModel.HeavyRevolver, "BodyguardVariant", 0x9493B80D),
                new WeaponComponent(WeaponModel.HeavyRevolver, "DefaultClip", 0xE9867CE3),
                new WeaponComponent(WeaponModel.SNSPistol, "DefaultClip", 0xF8802ED9),
                new WeaponComponent(WeaponModel.SNSPistol, "ExtendedClip", 0x7B0033B3),
                new WeaponComponent(WeaponModel.SNSPistol, "EtchedWoodGripFinish", 0x8033ECAF),
                new WeaponComponent(WeaponModel.HeavyPistol, "DefaultClip", 0xD4A969A),
                new WeaponComponent(WeaponModel.HeavyPistol, "ExtendedClip", 0x64F9C62B),
                new WeaponComponent(WeaponModel.HeavyPistol, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.HeavyPistol, "Suppressor", 0xC304849A),
                new WeaponComponent(WeaponModel.HeavyPistol, "EtchedWoodGripFinish", 0x7A6A7B7B),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "DefaultRounds", 0xBA23D8BE),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "TracerRounds", 0xC6D8E476),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "IncendiaryRounds", 0xEFBF25),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "HollowPointRounds", 0x10F42E8F),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "FullMetalJacketRounds", 0xDC8BA3F),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "HolographicSight", 0x420FD713),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "SmallScope", 0x49B2945),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Flashlight", 0x359B7AAE),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Compensator", 0x27077CCB),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "DigitalCamo", 0xC03FED9F),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "BrushstrokeCamo", 0xB5DE24),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "WoodlandCamo", 0xA7FF1B8),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Skull", 0xF2E24289),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "SessantaNove", 0x11317F27),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Perseus", 0x17C30C42),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Leopard", 0x257927AE),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Zebra", 0x37304B1C),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Geometric", 0x48DAEE71),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Boom", 0x20ED9B5B),
                new WeaponComponent(WeaponModel.HeavyRevolverMkII, "Patriotic", 0xD951E867),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "DefaultClip", 0x1466CE6),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "ExtendedClip", 0xCE8C0772),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "TracerRounds", 0x902DA26E),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "IncendiaryRounds", 0xE6AD5F79),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "HollowPointRounds", 0x8D107402),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "FullMetalJacketRounds", 0xC111EB26),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Flashlight", 0x4A4965F3),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "MountedScope", 0x47DE9258),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Suppressor", 0x65EA7EBB),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Compensator", 0xAA8283BF),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "DigitalCamo", 0xF7BEEDD),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "BrushstrokeCamo", 0x8A612EF6),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "WoodlandCamo", 0x76FA8829),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Skull", 0xA93C6CAC),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "SessantaNove", 0x9C905354),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Perseus", 0x4DFA3621),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Leopard", 0x42E91FFF),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Zebra", 0x54A8437D),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Geometric", 0x68C2746),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Boom", 0x2366E467),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Boom2", 0x441882E6),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "DigitalCamo", 0xE7EE68EA),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "BrushstrokeCamo", 0x29366D21),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "WoodlandCamo", 0x3ADE514B),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "SkullSlide", 0xE64513E9),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "SessantaNoveSlide", 0xCD7AEB9A),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "PerseusSlide", 0xFA7B27A6),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "LeopardSlide", 0xE285CA9A),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "ZebraSlide", 0x2B904B19),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "GeometricSlide", 0x22C24F9C),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "BoomSlide", 0x8D0D5ECD),
                new WeaponComponent(WeaponModel.SNSPistolMkII, "Patriotic", 0x1F07150A),
            };
        }

        public static Position ObterPosicaoPlayerIC(Personagem p)
        {
            if (p.Dimensao == 0)
                return p.Player.Position;

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == p.Dimensao);
            return new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ);
        }
    }
}