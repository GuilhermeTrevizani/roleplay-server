using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Roleplay.Entities
{
    public class Personagem
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Usuario { get; set; } = 0;
        public long SocialClubRegistro { get; set; } = 0;
        public long SocialClubUltimoAcesso { get; set; } = 0;
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public string IPRegistro { get; set; } = string.Empty;
        public long HardwareIdHashRegistro { get; set; } = 0;
        public long HardwareIdExHashRegistro { get; set; } = 0;
        public DateTime DataUltimoAcesso { get; set; } = DateTime.Now;
        public string IPUltimoAcesso { get; set; } = string.Empty;
        public long HardwareIdHashUltimoAcesso { get; set; } = 0;
        public long HardwareIdExHashUltimoAcesso { get; set; } = 0;
        public long Skin { get; set; } = (long)PedModel.FreemodeMale01;
        public float PosX { get; set; } = 265.66153f;
        public float PosY { get; set; } = -1205.723f;
        public float PosZ { get; set; } = 29.279907f;
        public int Vida { get; set; } = 100;
        public int Colete { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public DateTime DataNascimento { get; set; } = DateTime.MinValue;
        public bool Online { get; set; } = true;
        public int TempoConectado { get; set; } = 0;
        public int Faccao { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public int Dinheiro { get; set; } = 0;
        public int Celular { get; set; } = 0;
        public int Banco { get; set; } = 3500;
        public string IPL { get; set; } = "[]";
        public int CanalRadio { get; set; } = -1;
        public int CanalRadio2 { get; set; } = 0;
        public int CanalRadio3 { get; set; } = 0;
        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 3.0673819f;
        public DateTime? DataMorte { get; set; } = null;
        public string MotivoMorte { get; set; } = string.Empty;
        public TipoEmprego Emprego { get; set; } = TipoEmprego.Nenhum;
        public string InformacoesPersonalizacao { get; set; } = "[]";
        public string Historia { get; set; } = string.Empty;
        public int UsuarioStaffAvaliador { get; set; } = 0;
        public string MotivoRejeicao { get; set; } = string.Empty;
        public TipoStatusNamechange StatusNamechange { get; set; } = TipoStatusNamechange.Liberado;
        public TipoEtapaPersonalizacao EtapaPersonalizacao { get; set; } = TipoEtapaPersonalizacao.Caracteristicas;
        public DateTime? DataExclusao { get; set; } = null;
        public DateTime? DataTerminoPrisao { get; set; } = null;
        public DateTime? DataValidadeLicencaMotorista { get; set; } = null;
        public int PolicialRevogacaoLicencaMotorista { get; set; } = 0;
        public int Distintivo { get; set; } = 0;
        public string InformacoesRoupas { get; set; } = "[]";
        public string InformacoesAcessorios { get; set; } = "[]";
        public string InformacoesArmas { get; set; } = "[]";
        public string InformacoesContatos { get; set; } = "[]";
        public int PecasVeiculares { get; set; } = 0;
        public DateTime? DataUltimoUsoAnuncio { get; set; } = null;
        public int Mascara { get; set; } = 0;
        public int Roupa { get; set; } = 1;
        public int Poupanca { get; set; } = 0;
        public int PagamentoExtra { get; set; } = 0;
        public string InformacoesFerimentos { get; set; } = "[]";
        public int TipoFerido { get; set; } = 0;

        [NotMapped]
        public Personalizacao PersonalizacaoDados { get; set; } = new Personalizacao();

        [NotMapped]
        public int ID { get; set; }

        [NotMapped]
        public Usuario UsuarioBD { get; set; }

        [NotMapped]
        public IPlayer Player { get; set; }

        [NotMapped]
        public DateTime DataUltimaVerificacao { get; set; }

        [NotMapped]
        public Faccao FaccaoBD { get => Global.Faccoes.FirstOrDefault(x => x.Codigo == Faccao); }

        [NotMapped]
        public Rank RankBD { get => Global.Ranks.FirstOrDefault(x => x.Faccao == Faccao && x.Codigo == Rank); }

        [NotMapped]
        public List<Convite> Convites { get; set; } = new List<Convite>();

        [NotMapped]
        public string NomeIC { get => UsandoMascara ? $"Mascarado {Mascara}" : Nome; }

        [NotMapped]
        public List<Contato> Contatos { get; set; } = new List<Contato>();

        [NotMapped]
        public int NumeroLigacao { get; set; } = 0;

        [NotMapped]
        public TipoStatusLigacao StatusLigacao { get; set; } = TipoStatusLigacao.Nenhum;

        [NotMapped]
        public string ExtraLigacao { get; set; } = string.Empty;

        [NotMapped]
        public TagTimer TimerCelular { get; set; }

        [NotMapped]
        public bool EmTrabalho { get; set; } = false;

        [NotMapped]
        public List<string> IPLs { get; set; }

        [NotMapped]
        public bool Algemado { get; set; } = false;

        [NotMapped]
        public int AguardandoTipoServico { get; set; } = 0;

        [NotMapped]
        public bool EmTrabalhoAdministrativo { get; set; } = false;

        [NotMapped]
        public List<Vestimenta> Roupas { get; set; } = new List<Vestimenta>();

        [NotMapped]
        public List<Vestimenta> Acessorios { get; set; } = new List<Vestimenta>();

        [NotMapped]
        public List<Ferimento> Ferimentos { get; set; } = new List<Ferimento>();

        [NotMapped]
        public TagTimer TimerFerido { get; set; } = null;

        [NotMapped]
        public List<Arma> Armas { get; set; } = new List<Arma>();

        [NotMapped]
        public string AreaName { get; set; } = string.Empty;

        [NotMapped]
        public Position PosicaoIC
        {
            get
            {
                if (Player.Dimension == 0)
                    return Player.Position;

                var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == Player.Dimension);
                if (prop == null)
                    return Player.Position;

                return new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ);
            }
        }

        [NotMapped]
        public Position? PosicaoSpec { get; set; } = null;

        [NotMapped]
        public int DimensaoSpec { get; set; } = 0;

        [NotMapped]
        public int IDSpec { get; set; } = 0;

        [NotMapped]
        public List<string> IPLsSpec { get; set; } = new List<string>();

        [NotMapped]
        public string CorStaff => UsuarioBD.Staff switch
        {
            TipoStaff.Moderator => "#804000",
            TipoStaff.GameAdministrator => "#40BFFF",
            TipoStaff.LeadAdministrator => "#00AA00",
            TipoStaff.Manager => "#CC4545",
            _ => string.Empty,
        };

        [NotMapped]
        public bool DL { get; set; } = false;

        [NotMapped]
        public bool Ferido => Ferimentos.Count > 0 || TimerFerido != null || Player.Health != Player.MaxHealth;

        [NotMapped]
        public List<Propriedade> Propriedades { get => Global.Propriedades.Where(x => x.Personagem == Codigo).ToList(); }

        [NotMapped]
        public bool UsandoMascara { get; set; } = false;

        [NotMapped]
        public int SlotsRoupas
        {
            get
            {
                var slots = Roupas.Where(x => x.Drawable != 0).GroupBy(x => x.ID).Count();
                if ((UsuarioBD.DataExpiracaoVIP ?? DateTime.MinValue) < DateTime.Now)
                    return slots > 2 ? slots : 2;

                return UsuarioBD.VIP switch
                {
                    TipoVIP.Bronze => 6,
                    TipoVIP.Prata => 8,
                    TipoVIP.Ouro => 10,
                    _ => slots > 2 ? slots : 2,
                };
            }
        }

        [NotMapped]
        public List<Ponto> PontosColeta { get; set; } = new List<Ponto>();

        [NotMapped]
        public Ponto PontoColetando { get; set; } = new Ponto();

        [NotMapped]
        public int ItensColetados { get; set; } = 0;

        public void SetDinheiro()
        {
            if (Player != null)
                Player.SetSyncedMetaData("dinheiro", $"${Dinheiro:N0}");
        }

        public string ObterNomeContato(int numero)
        {
            if (numero == 911)
                return "Central de Emergência";

            if (numero == 5555555)
                return "Downtown Cab Company";

            if (numero == 7777777)
                return "Central de Mecânicos";

            var contato = Contatos.FirstOrDefault(x => x.Celular == numero);
            return contato == null ? $"#{numero}" : $"{contato.Nome} #{numero}";
        }

        public void LimparLigacao(bool isApenasPararTimer = false)
        {
            TimerCelular?.Stop();
            TimerCelular = null;

            if (!isApenasPararTimer)
            {
                NumeroLigacao = 0;
                StatusLigacao = TipoStatusLigacao.Nenhum;
                ExtraLigacao = string.Empty;
            }
        }

        public void SetarIPLs()
        {
            if (IPLs == null)
                IPLs = new List<string>();

            foreach (var ipl in IPLs)
                Player.Emit("Server:RequestIpl", ipl);
        }

        public void LimparIPLs()
        {
            if (IPLs == null)
            {
                IPLs = new List<string>();
            }
            else
            {
                foreach (var ipl in IPLs)
                    Player.Emit("Server:RemoveIpl", ipl);

                IPLs = new List<string>();
            }
        }

        public void PlayAnimation(string dic, string name, int flag) => Player.Emit("Server:PlayAnim", dic, name, flag);

        public void StopAnimation() => Player.Emit("Server:StopAnim");

        public void SetPosition(Position position, bool spawn)
        {
            if (spawn)
                Player.Spawn(position);
            else
                Player.Position = position;

            foreach (var x in Global.PersonagensOnline.Where(x => x.IDSpec == ID))
            {
                x.LimparIPLs();
                x.IPLs = IPLs;
                x.SetarIPLs();
                x.Player.Dimension = Player.Dimension;
                x.SetPosition(new Position(position.X, position.Y, position.Z + 5), true);
                x.Player.Emit("SpectatePlayer", Player);
            }
        }

        public void Unspectate()
        {
            LimparIPLs();
            IPLs = IPLsSpec;
            SetarIPLs();
            Player.Dimension = DimensaoSpec;
            SetPosition(PosicaoSpec.Value, true);
            PosicaoSpec = null;
            DimensaoSpec = IDSpec = 0;
            IPLsSpec = new List<string>();
            Player.Emit("UnspectatePlayer");
            SetNametag();
        }

        public void Curar()
        {
            TipoFerido = 0;
            Player.SetSyncedMetaData("ferido", 0);
            if (TimerFerido != null)
            {
                SetPosition(Player.Position, true);
                StopAnimation();
            }
            Ferimentos = new List<Ferimento>();
            Player.Emit("Server:ToggleFerido", 0);
            Player.Health = Player.MaxHealth;
            TimerFerido?.Stop();
            TimerFerido = null;
        }

        public void DarArma(WeaponModel weapon, int municao, byte pintura = 0, string components = "[]", bool selectWeapon = true)
        {
            Player.GiveWeapon(weapon, municao, selectWeapon);
            Player.SetWeaponTintIndex(weapon, pintura);

            var armaComponentes = JsonConvert.DeserializeObject<List<uint>>(components);
            foreach (var x in components)
                Player.AddWeaponComponent(weapon, x);

            Armas.RemoveAll(x => x.Codigo == (long)weapon);
            Armas.Add(new Arma()
            {
                Codigo = (long)weapon,
                Municao = municao,
                Pintura = pintura,
                Componentes = components,
            });
        }

        public void Spawnar()
        {
            Player.Dimension = (int)Dimensao;

            Functions.EnviarMensagem(Player, TipoMensagem.Nenhum, $"Olá {{{Global.CorPrincipal}}}{UsuarioBD.Nome}{{#FFFFFF}}, que bom te ver por aqui! Seu último login foi em {{{Global.CorPrincipal}}}{DataUltimoAcesso}{{#FFFFFF}}.");

            if (UsuarioBD.DataExpiracaoVIP.HasValue)
                Functions.EnviarMensagem(Player, TipoMensagem.Nenhum, $"Seu {{{Global.CorPrincipal}}}VIP {UsuarioBD.VIP}{{#FFFFFF}} {(UsuarioBD.DataExpiracaoVIP.Value < DateTime.Now ? "expirou" : "expira")} em {{{Global.CorPrincipal}}}{UsuarioBD.DataExpiracaoVIP.Value}{{#FFFFFF}}.");
            else if ((UsuarioBD.DataExpiracaoVIP ?? DateTime.MinValue) < DateTime.Now)
                UsuarioBD.TogPM = false;

            SetNametag();
            Player.Emit("nametags:Config", true);
            Player.Emit("chat:activateTimeStamp", UsuarioBD.TimeStamp);
            Player.SetSyncedMetaData("ferido", 0);
            SetPosition(new Position(PosX, PosY, PosZ), true);
            Player.Rotation = new Position(RotX, RotY, RotZ);
            Player.SetSyncedMetaData("id", ID);
            DataUltimoAcesso = DateTime.Now;
            Player.Emit("Server:setArtificialLightsState", Global.Parametros.Blackout);
            Player.Health = (ushort)Vida;
            Player.Armor = (ushort)Colete;

            if (TipoFerido > 0)
                SetarFerido(false);

            AltAsync.Do(async () =>
            {
                var context = new DatabaseContext();
                var multas = await context.Multas.AsQueryable().CountAsync(x => x.PersonagemMultado == Codigo && !x.DataPagamento.HasValue);
                if (multas > 0)
                {
                    var strMultas = multas > 1 ? "s" : string.Empty;
                    Functions.EnviarMensagem(Player, TipoMensagem.Erro, $"Você possui {multas} multa{strMultas} pendente{strMultas}.");
                }
            });
        }

        public void SetarFerido(bool temDelay)
        {
            if (temDelay || TipoFerido == 1)
                Functions.EnviarMensagem(Player, TipoMensagem.Erro, "Você foi gravemente ferido. Os médicos deverão chegar em até 5 minutos ou você perderá a consciência.");
            else if (TipoFerido == 2)
                Functions.EnviarMensagem(Player, TipoMensagem.Erro, "Você perdeu a consciência.");

            Algemado = false;
            TimerFerido?.Stop();
            TimerFerido = new TagTimer(300000)
            {
                Tag = Codigo,
            };
            TimerFerido.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                var timer = (TagTimer)sender;
                timer.ElapsedCount++;

                var p = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == (int)timer.Tag);
                if (p == null)
                {
                    timer?.Stop();
                    return;
                }

                Functions.EnviarMensagem(p.Player, TipoMensagem.Erro, "Digite /aceitarhospital para que você receba os cuidados dos médicos e seja levado ao hospital.");
                Functions.EnviarMensagem(p.Player, TipoMensagem.Erro, "Digite /aceitarck para aplicar CK no seu personagem. ESSA OPERAÇÃO É IRREVERSÍVEL.");
                timer?.Stop();
            };
            TimerFerido.Start();

            AltAsync.Do(async () =>
            {
                if (temDelay)
                    await Task.Delay(5000);

                if (Player.IsDead || !temDelay)
                {
                    if (temDelay)
                    {
                        await Player.SpawnAsync(Player.Position);
                        TipoFerido = 1;
                    }

                    StopAnimation();
                    await Player.EmitAsync("Server:ToggleFerido", TipoFerido);
                    Player.SetSyncedMetaData("ferido", TipoFerido);
                }
            });
        }

        public void SetNametag() => Player.SetSyncedMetaData("nametag", !PosicaoSpec.HasValue ? (EmTrabalhoAdministrativo ? $"~q~{UsuarioBD?.Nome}" : NomeIC) : string.Empty);

        public class Ferimento
        {
            public DateTime Data { get; set; } = DateTime.Now;
            public ushort Dano { get; set; }
            public uint Arma { get; set; }
            public string Attacker { get; set; }
            public sbyte BodyPart { get; set; } = -2;
        }

        public class Vestimenta
        {
            public Vestimenta()
            {
            }

            public Vestimenta(int id, int slot)
            {
                ID = id;
                Slot = slot;
            }

            public Vestimenta(int id, int slot, int drawable)
            {
                ID = id;
                Slot = slot;
                Drawable = drawable;
            }

            public int ID { get; set; }
            public int Slot { get; set; }
            public int Drawable { get; set; } = 0;
            public int Texture { get; set; } = 0;
        }

        public class Arma
        {
            public long Codigo { get; set; }
            public int Municao { get; set; } = 0;
            public byte Pintura { get; set; } = 0;
            public string Componentes { get; set; } = "[]";
        }

        public class Contato
        {
            public int Celular { get; set; }
            public string Nome { get; set; }
        }

        public class Personalizacao
        {
            /// <summary>
            /// 0 - Mulher
            /// 1 - Homem
            /// </summary>
            public int sex { get; set; } = 1;
            public int faceFather { get; set; } = 0;
            public int faceMother { get; set; } = 0;
            public int skinFather { get; set; } = 0;
            public int skinMother { get; set; } = 0;
            public double faceMix { get; set; } = 0.5;
            public double skinMix { get; set; } = 0.5;
            public List<double> structure { get; set; } = new List<double>();
            public int hair { get; set; } = 4;
            public int hairColor1 { get; set; } = 1;
            public int hairColor2 { get; set; } = 5;
            public HairOverlay hairOverlay { get; set; } = new HairOverlay("multiplayer_overlays", "NG_M_Hair_004");
            public int facialHair { get; set; } = 0;
            public int facialHairColor1 { get; set; } = 0;
            public double facialHairOpacity { get; set; } = 0;
            public int eyebrows { get; set; } = 0;
            public double eyebrowsOpacity { get; set; } = 0;
            public int eyebrowsColor1 { get; set; } = 0;
            public int eyes { get; set; } = 0;
            public List<OpacityOverlay> opacityOverlays { get; set; } = new List<OpacityOverlay>();
            public List<ColorOverlay> colorOverlays { get; set; } = new List<ColorOverlay>();

            public class HairOverlay
            {
                public HairOverlay(string _collection, string _overlay)
                {
                    collection = _collection;
                    overlay = _overlay;
                }

                public string collection { get; set; }
                public string overlay { get; set; }
            }

            public class OpacityOverlay
            {
                public OpacityOverlay(int _id)
                {
                    id = _id;
                }

                public int id { get; set; } = 0;
                public double opacity { get; set; } = 0;
                public double value { get; set; } = 0;
            }

            public class ColorOverlay
            {
                public ColorOverlay(int _id)
                {
                    id = _id;
                }

                public int id { get; set; } = 0;
                public double opacity { get; set; } = 0;
                public int color1 { get; set; } = 0;
                public int color2 { get; set; } = 0;
                public double value { get; set; } = 0;
            }
        }
    }
}