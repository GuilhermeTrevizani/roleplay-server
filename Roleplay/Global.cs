using AltV.Net.Enums;
using Roleplay.Entities;
using Roleplay.Models;
using System.Collections.Generic;

namespace Roleplay
{
    public static class Global
    {
        public static int MaxPlayers { get; set; }
        public static string ConnectionString { get; set; }
        public static Parametro Parametros { get; set; }
        public static List<Personagem> PersonagensOnline { get; set; }
        public static List<Blip> Blips { get; set; }
        public static List<Faccao> Faccoes { get; set; }
        public static List<Rank> Ranks { get; set; }
        public static List<Propriedade> Propriedades { get; set; }
        public static List<Preco> Precos { get; set; }
        public static List<Veiculo> Veiculos { get; set; }
        public static List<Ponto> Pontos { get; set; }
        public static List<Armario> Armarios { get; set; }
        public static List<ArmarioItem> ArmariosItens { get; set; }
        public static List<Concessionaria> Concessionarias { get; set; }
        public static List<Emprego> Empregos { get; set; }
        public static List<SOS> SOSs { get; set; }
        public static List<TextDraw> TextDraws { get; set; }
        public static WeatherType Weather { get; set; }
        public static List<WeaponComponent> WeaponComponents { get; set; }
        public static List<Ligacao911> Ligacoes911 { get; set; }
    }
}