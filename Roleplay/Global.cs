using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Roleplay.Entities;
using Roleplay.Models;
using System.Collections.Generic;

namespace Roleplay
{
    public static class Global
    {
        public static string CorSucesso { get; set; } = "#6EB469";
        public static string CorAmarelo { get; set; } = "#FEBD0C";
        public static string CorErro { get; set; } = "#FF6A4D";
        public static string NomeServidor { get; set; } = "Segunda Vida Roleplay";
        public static int MaxPlayers { get; set; }
        public static string ConnectionString { get; set; }
        public static Parametro Parametros { get; set; }
        public static List<Personagem> PersonagensOnline { get; set; } = new List<Personagem>();
        public static List<Entities.Blip> Blips { get; set; }
        public static List<Faccao> Faccoes { get; set; }
        public static List<Rank> Ranks { get; set; }
        public static List<Propriedade> Propriedades { get; set; }
        public static List<Preco> Precos { get; set; }
        public static List<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public static List<Ponto> Pontos { get; set; }
        public static List<Armario> Armarios { get; set; }
        public static List<ArmarioItem> ArmariosItens { get; set; }
        public static List<Pergunta> Perguntas { get; set; } = new List<Pergunta>();
        public static List<Resposta> Respostas { get; set; } = new List<Resposta>();
        public static List<Concessionaria> Concessionarias { get; set; } = new List<Concessionaria>()
        {
            new Concessionaria()
            {
                Nome = "Concessionária de Carros e Motos",
                Tipo = TipoPreco.CarrosMotos,
                PosicaoCompra = new Position(-56.637363f, -1099.0286f, 26.415405f),
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
        public static List<Emprego> Empregos { get; set; } = new List<Emprego>()
        {
            new Emprego()
            {
                Tipo = TipoEmprego.Taxista,
                Posicao = new Position(895.0308f, -179.1359f, 74.70036f),
            },
        };
        public static List<SOS> SOSs { get; set; } = new List<SOS>();
        public static List<TextDraw> TextDraws { get; set; } = new List<TextDraw>();
        public static WeatherType Weather { get; set; } = WeatherType.Clear;
        public static List<WeaponComponent> WeaponComponents { get; set; } = new List<WeaponComponent>()
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
        public static List<Ligacao911> Ligacoes911 { get; set; } = new List<Ligacao911>();
        public static List<IVoiceChannel> TACVoice { get; set; }
    }
}