using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public float PosX { get; set; } = 128.4853f;
        public float PosY { get; set; } = -1737.086f;
        public float PosZ { get; set; } = 30.11018f;
        public int Vida { get; set; } = 100;
        public int Colete { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public string Sexo { get; set; } = "M";
        public DateTime DataNascimento { get; set; } = DateTime.MinValue;
        public bool Online { get; set; } = true;
        public int TempoConectado { get; set; } = 0;
        public int Faccao { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public int Dinheiro { get; set; } = 500;
        public int Celular { get; set; } = 0;
        public int Banco { get; set; } = 0;
        public string IPL { get; set; } = "[]";
        public int CanalRadio { get; set; } = -1;
        public int CanalRadio2 { get; set; } = 0;
        public int CanalRadio3 { get; set; } = 0;
        public int TempoPrisao { get; set; } = 0;
        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 149.1595f;
        public DateTime? DataMorte { get; set; } = null;
        public string MotivoMorte { get; set; } = string.Empty;
        public TipoEmprego Emprego { get; set; } = TipoEmprego.Nenhum;
        public string InformacoesPersonalizacao { get; set; } = "[]";
        public string RoupasCivil { get; set; } = string.Empty;
        public string Historia { get; set; } = string.Empty;
        public int UsuarioStaffAvaliador { get; set; } = 0;
        public string MotivoRejeicao { get; set; } = string.Empty;
        public TipoStatusNamechange StatusNamechange { get; set; } = TipoStatusNamechange.Liberado;

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
        public List<Propriedade> Propriedades { get => Global.Propriedades.Where(x => x.Personagem == Codigo).ToList(); }

        [NotMapped]
        public string NomeIC { get => Nome; }

        [NotMapped]
        public List<PersonagemContato> Contatos { get; set; }

        [NotMapped]
        public int NumeroLigacao { get; set; } = 0;

        [NotMapped]
        public TipoStatusLigacao StatusLigacao { get; set; } = TipoStatusLigacao.Nenhum;

        [NotMapped]
        public string ExtraLigacao { get; set; } = string.Empty;

        [NotMapped]
        public TagTimer TimerCelular { get; set; }

        [NotMapped]
        public bool IsEmTrabalho { get; set; } = false;

        [NotMapped]
        public List<string> IPLs { get; set; }

        [NotMapped]
        public bool Algemado { get; set; } = false;

        [NotMapped]
        public int AguardandoTipoServico { get; set; } = 0;

        [NotMapped]
        public bool IsEmTrabalhoAdministrativo { get; set; } = false;

        [NotMapped]
        public List<PersonagemRoupa> Roupas { get; set; } = new List<PersonagemRoupa>();

        [NotMapped]
        public List<PersonagemAcessorio> Acessorios { get; set; } = new List<PersonagemAcessorio>();

        [NotMapped]
        public List<Ferimento> Ferimentos { get; set; } = new List<Ferimento>();

        [NotMapped]
        public TagTimer TimerFerido { get; set; } = null;

        [NotMapped]
        public List<PersonagemArma> Armas { get; set; } = new List<PersonagemArma>();

        [NotMapped]
        public string AreaName { get; set; }

        [NotMapped]
        public string ZoneName { get; set; }

        [NotMapped]
        public string StringArmas { get; set; }

        [NotMapped]
        public List<Pergunta> Perguntas { get; set; } = new List<Pergunta>();

        [NotMapped]
        public List<Resposta> Respostas { get; set; } = new List<Resposta>();

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
            foreach (var ipl in IPLs)
                Player.Emit("Server:RequestIpl", ipl);
        }

        public void LimparIPLs()
        {
            foreach (var ipl in IPLs)
                Player.Emit("Server:RemoveIpl", ipl);

            IPLs.Clear();
        }

        public void PlayAnimation(string dic, string name, int flag)
        {
            Player.Emit("Server:PlayAnim", dic, name, flag);
            Player.SetSyncedMetaData("animation", true);
        }

        public void StopAnimation()
        {
            Player.Emit("Server:StopAnim");
            Player.SetSyncedMetaData("animation", false);
        }

        public void SetClothes(int slot, int drawable, int texture, bool setar = true)
        {
            Roupas.RemoveAll(x => x.Slot == slot);
            Roupas.Add(new PersonagemRoupa() { Codigo = Codigo, Slot = slot, Drawable = drawable, Texture = texture });

            if (setar)
                Player.Emit("Server:SetClothes", slot, drawable, texture);
        }

        public void SetAccessories(int slot, int drawable, int texture, bool setar = true)
        {
            Acessorios.RemoveAll(x => x.Slot == slot);
            Acessorios.Add(new PersonagemAcessorio() { Codigo = Codigo, Slot = slot, Drawable = drawable, Texture = texture });

            if (setar)
                Player.Emit("Server:SetAccessories", slot, drawable, texture);
        }

        public class Ferimento
        {
            public DateTime Data { get; set; } = DateTime.Now;
            public ushort Dano { get; set; }
            public uint Arma { get; set; }
            public int CodigoAttacker { get; set; }
            public sbyte BodyPart { get; set; } = -2;
        }

        public class Personalizacao
        {
            public int CabeloCor1 { get; set; } = 0;
            public int CabeloCor2 { get; set; } = 0;
            public int Barba { get; set; } = 255;
            public int BarbaCor { get; set; } = 0;
            public int Maquiagem { get; set; } = 255;
        }
    }
}