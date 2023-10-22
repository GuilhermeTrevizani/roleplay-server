using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Discord.WebSocket;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Reflection;

namespace Roleplay
{
    public sealed class Global
    {
        public const string SUCCESS_COLOR = "#6EB469";

        public const string MAIN_COLOR = "#AE6AB2";

        public const string ERROR_COLOR = "#FF6A4D";

        public const string SERVER_NAME = "Segunda Vida Roleplay";

        public const string SERVER_INITIALS = "SGV:RP";

        public const float RP_DISTANCE = 3.5f;

        public const string STAFF_COLOR = "#AE6AB2";

        public const string CELLPHONE_MAIN_COLOR = "#F0E90D";

        public const string CELLPHONE_SECONDARY_COLOR = "#F2FF43";

        public const string MENSAGEM_SEM_AUTORIZACAO = "Você não possui autorização para usar esse comando.";

        public const string MENSAGEM_PK = "Você morreu e perdeu a memória.";

        public const string MENSAGEM_ERRO_DISCORD = "Não foi possível recuperar informações do banco de dados. Reporte o bug no fórum.";

        public const string MENSAGEM_DISCORD_NAO_VINCULADO = "Seu Discord não foi vinculado em nenhum usuário.";

        public const float PESO_MAXIMO_INVENTARIO = 30;

        public const string MENSAGEM_GRAVEMENTE_FERIDO = "Você não pode executar esse comando pois está gravemente ferido.";

        public const int QUANTIDADE_SLOTS_INVENTARIO = 30;

        public const string PATH_JSON_CLOTHES = "resources/client/resources/json/";

        public const int EMERGENCY_NUMBER = 911;

        public const int TAXI_NUMBER = 5555555;

        public const int MECHANIC_NUMBER = 7777777;

        public const string VEHICLE_DRIVER_ERROR_MESSAGE = "Você não é o motorista de um veículo.";

        public const string VEHICLE_ACCESS_ERROR_MESSAGE = "Você não possui acesso ao veículo.";

        public const string VEHICLE_OWNER_ERROR_MESSAGE = "Você não é o proprietário do veículo.";

        public const string INSUFFICIENT_MONEY_ERROR_MESSAGE = "Você não possui dinheiro suficiente (${0:N0}).";

        public const string MENSAGEM_ERRO_FORA_VEICULO = "Você não está em um veículo.";

        public const string BLOCKED_CELLPHONE_ERROR_MESSAGE = "Você não pode usar o celular agora.";

        public const string UNEQUIPPED_CELPPHONE_ERROR_MESSAGE = "Você não possui um celular equipado.";

        public const string MENSAGEM_ERRO_SALDO_INSUFICIENTE = "Você não possui saldo suficiente na sua conta bancária (${0:N0}).";

        public const string ROBBED_PROPERTY_ERROR_MESSAGE = "A propriedade foi roubada. Use /pliberar.";

        public const string URL_AUDIO_PROPERTY_ALARM = "https://trevizani.com.br/propertyalarm.mp3";

        public const string URL_AUDIO_VEHICLE_ALARM = "https://trevizani.com.br/vehiclealarm.mp3";

        public static Rgba MainRgba { get; } = new Rgba(174, 106, 178, 75);

        public static Parameter Parameter { get; set; }

        public static List<Blip> Blips { get; set; }

        public static List<Faction> Factions { get; set; }

        public static List<FactionRank> FactionsRanks { get; set; }

        public static List<Property> Properties { get; set; }

        public static List<Price> Prices { get; set; }

        public static List<Spot> Spots { get; set; }

        public static List<FactionArmory> FactionsArmories { get; set; }

        public static List<FactionArmoryWeapon> FactionsArmoriesWeapons { get; set; }

        public static List<Question> Questions { get; set; }

        public static List<QuestionAnswer> QuestionsAnswers { get; set; }

        public static List<Dealership> Dealerships { get; } = new()
        {
            new()
            {
                Name = "CONCESSIONÁRIA DE BARCOS",
                PriceType = PriceType.Barcos,
                Position = new Position(-787.1262f, -1354.725f, 5.150271f),
                VehiclePosition = new Position(-805.2659f, -1418.4264f, 0.33190918f),
                VehicleRotation = new Position(-0.015625f, 0, 0.859375f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE HELICÓPTEROS",
                PriceType = PriceType.Helicopteros,
                Position = new Position(-753.5287f, -1512.43f, 5.020952f),
                VehiclePosition = new Position(-745.4902f, -1468.695f, 5.099712f),
                VehicleRotation = new Position(0, 0, 328.6675f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE AVIÕES",
                PriceType = PriceType.Avioes,
                Position = new Position(-941.34064f, -2954.8748f, 13.9296875f),
                VehiclePosition = new Position(-979.5824f, -2996.3472f, 13.457886f),
                VehicleRotation = new Position(0f, 0f, 1.046875f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE MOTOCICLETAS E BICICLETAS",
                PriceType = PriceType.MotocicletasBicicletas,
                Position = new Position(268.73407f, -1155.455f, 29.279907f),
                VehiclePosition = new Position(255.13846f, -1155.9165f, 28.774414f),
                VehicleRotation = new Position(0f, -0.015625f, 1.578125f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE CARROS",
                PriceType = PriceType.Carros,
                Position = new Position(-56.096703f, -1099.3978f, 26.415405f),
                VehiclePosition = new Position(-57.81099f, -1107.6263f, 25.96045f),
                VehicleRotation = new Position(0f, 0f, 1.25f),
            },
        };

        public static List<Job> Jobs { get; } = new()
        {
            new Job
            {
                CharacterJob = CharacterJob.TaxiDriver,
                Position = new Position(895.0308f, -179.1359f, 74.70036f),
                VehicleModel = VehicleModel.Taxi,
                VehicleRentPosition = new Position(907.33185f, -175.21318f, 74.11719f),
                VehicleRentRotation = new Rotation(0f, 0f, -2.1273777f),
                VehicleColor = new Rgba(252, 186, 3, 255),
            },
            new Job
            {
                CharacterJob = CharacterJob.Mechanic,
                Position = new Position(-27.178022f, -1673.1956f, 29.482056f),
                VehicleModel = VehicleModel.TowTruck2,
                VehicleRentPosition = new Position(-27.96923f, -1680.8967f, 29.431519f),
                VehicleRentRotation = new Rotation(0f, 0f, 2.0779037f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
            new Job
            {
                CharacterJob = CharacterJob.Garbageman,
                Position = new Position(-355.18683f, -1513.411f, 27.712769f),
                VehicleModel = VehicleModel.Trash,
                VehicleRentPosition = new Position(-350.62418f, -1520.6901f, 27.712769f),
                VehicleRentRotation = new Rotation(0f, 0f, -1.6326387f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
            new Job
            {
                CharacterJob = CharacterJob.Trucker,
                Position = new Position(895.1341f, -896.2813f, 27.780273f),
                VehicleModel = VehicleModel.Burrito,
                VehicleRentPosition = new Position(886.87915f, -889.3714f, 26.533325f),
                VehicleRentRotation = new Rotation(0f, 0f, 1.5831648f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
        };

        public static List<HelpRequest> HelpRequests { get; set; } = new();

        public static List<WeaponComponent> WeaponComponents { get; } = new()
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
            new WeaponComponent(WeaponModel.PistolMkII, "DefaultClip", 0x94F42D62),
            new WeaponComponent(WeaponModel.PistolMkII, "ExtendedClip", 0x5ED6C128),
            new WeaponComponent(WeaponModel.PistolMkII, "TracerRounds", 0x25CAAEAF),
            new WeaponComponent(WeaponModel.PistolMkII, "IncendiaryRounds", 0x2BBD7A3A),
            new WeaponComponent(WeaponModel.PistolMkII, "HollowPointRounds", 0x85FEA109),
            new WeaponComponent(WeaponModel.PistolMkII, "FullMetalJacketRounds", 0x4F37DF2A),
            new WeaponComponent(WeaponModel.PistolMkII, "MountedScope", 0x8ED4BB70),
            new WeaponComponent(WeaponModel.PistolMkII, "Flashlight	", 0x43FD595B),
            new WeaponComponent(WeaponModel.PistolMkII, "Suppressor", 0x65EA7EBB),
            new WeaponComponent(WeaponModel.PistolMkII, "Compensator", 0x21E34793),
            new WeaponComponent(WeaponModel.PistolMkII, "DigitalCamo", 0x5C6C749C),
            new WeaponComponent(WeaponModel.PistolMkII, "BrushstrokeCamo", 0x15F7A390),
            new WeaponComponent(WeaponModel.PistolMkII, "WoodlandCamo", 0x968E24DB),
            new WeaponComponent(WeaponModel.PistolMkII, "Skull", 0x17BFA99),
            new WeaponComponent(WeaponModel.PistolMkII, "SessantaNove", 0xF2685C72),
            new WeaponComponent(WeaponModel.PistolMkII, "Perseus", 0xDD2231E6),
            new WeaponComponent(WeaponModel.PistolMkII, "Leopard", 0xBB43EE76),
            new WeaponComponent(WeaponModel.PistolMkII, "Zebra", 0x4D901310),
            new WeaponComponent(WeaponModel.PistolMkII, "Geometric", 0x5F31B653),
            new WeaponComponent(WeaponModel.PistolMkII, "Boom", 0x697E19A0),
            new WeaponComponent(WeaponModel.PistolMkII, "Patriotic", 0x930CB951),
            new WeaponComponent(WeaponModel.PistolMkII, "DigitalCamoSlide", 0xB4FC92B0),
            new WeaponComponent(WeaponModel.PistolMkII, "BrushstrokeCamoSlide", 0x1A1F1260),
            new WeaponComponent(WeaponModel.PistolMkII, "WoodlandCamoSlime", 0xE4E00B70),
            new WeaponComponent(WeaponModel.PistolMkII, "SkullSlide", 0x2C298B2B),
            new WeaponComponent(WeaponModel.PistolMkII, "SessantaNoveSlide", 0xDFB79725),
            new WeaponComponent(WeaponModel.PistolMkII, "PerseusSlide", 0x6BD7228C),
            new WeaponComponent(WeaponModel.PistolMkII, "LeopardSlide", 0x9DDBCF8C),
            new WeaponComponent(WeaponModel.PistolMkII, "ZebraSlide", 0xB319A52C),
            new WeaponComponent(WeaponModel.PistolMkII, "GeometricSlide", 0xC6836E12),
            new WeaponComponent(WeaponModel.PistolMkII, "BoomSlide", 0x43B1B173),
            new WeaponComponent(WeaponModel.PistolMkII, "PatrioticSlide", 0x4ABDA3FA),
            new WeaponComponent(WeaponModel.VintagePistol, "DefaultClip", 0x45A3B6BB),
            new WeaponComponent(WeaponModel.VintagePistol, "ExtendedClip", 0x33BA12E8),
            new WeaponComponent(WeaponModel.VintagePistol, "Suppressor", 0xC304849A),
            new WeaponComponent(WeaponModel.UpnAtomizer, "FestiveTint", 0xD7DBF707),
            new WeaponComponent(WeaponModel.MicroSMG, "DefaultClip", 0xCB48AEF0),
            new WeaponComponent(WeaponModel.MicroSMG, "ExtendedClip", 0x10E6BA2B),
            new WeaponComponent(WeaponModel.MicroSMG, "Flashlight", 0x359B7AAE),
            new WeaponComponent(WeaponModel.MicroSMG, "Scope", 0x9D2FBF29),
            new WeaponComponent(WeaponModel.MicroSMG, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.MicroSMG, "YusufAmirLuxuryFinish", 0x487AAE09),
            new WeaponComponent(WeaponModel.SMG, "DefaultClip", 0x26574997),
            new WeaponComponent(WeaponModel.SMG, "ExtendedClip", 0x350966FB),
            new WeaponComponent(WeaponModel.SMG, "DrumMagazine", 0x79C77076),
            new WeaponComponent(WeaponModel.SMG, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.SMG, "Scope", 0x3CC6BA57),
            new WeaponComponent(WeaponModel.SMG, "Suppressor", 0xC304849A),
            new WeaponComponent(WeaponModel.SMG, "YusufAmirLuxuryFinish", 0x27872C90),
            new WeaponComponent(WeaponModel.AssaultSMG, "DefaultClip", 0x8D1307B0),
            new WeaponComponent(WeaponModel.AssaultSMG, "ExtendedClip", 0xBB46E417),
            new WeaponComponent(WeaponModel.AssaultSMG, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.AssaultSMG, "Scope", 0x9D2FBF29),
            new WeaponComponent(WeaponModel.AssaultSMG, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.AssaultSMG, "YusufAmirLuxuryFinish", 0x278C78AF),
            new WeaponComponent(WeaponModel.MiniSMG, "DefaultClip", 0x84C8B2D3),
            new WeaponComponent(WeaponModel.MiniSMG, "ExtendedClip", 0x937ED0B7),
            new WeaponComponent(WeaponModel.SMGMkII, "DefaultClip", 0x4C24806E),
            new WeaponComponent(WeaponModel.SMGMkII, "ExtendedClip", 0xB9835B2E),
            new WeaponComponent(WeaponModel.SMGMkII, "TracerRounds", 0x7FEA36EC),
            new WeaponComponent(WeaponModel.SMGMkII, "IncendiaryRounds", 0xD99222E5),
            new WeaponComponent(WeaponModel.SMGMkII, "HollowPointRounds", 0x3A1BD6FA),
            new WeaponComponent(WeaponModel.SMGMkII, "FullMetalJacketRounds", 0xB5A715F),
            new WeaponComponent(WeaponModel.SMGMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.SMGMkII, "HolographicSight", 0x9FDB5652),
            new WeaponComponent(WeaponModel.SMGMkII, "SmallScope", 0xE502AB6B),
            new WeaponComponent(WeaponModel.SMGMkII, "MediumScope", 0x3DECC7DA),
            new WeaponComponent(WeaponModel.SMGMkII, "Suppressor", 0xC304849A),
            new WeaponComponent(WeaponModel.SMGMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.SMGMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.SMGMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.SMGMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.SMGMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.SMGMkII, "SlantedMuzzle Brake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.SMGMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.SMGMkII, "DefaultBarrel", 0xD9103EE1),
            new WeaponComponent(WeaponModel.SMGMkII, "HeavyBarrel", 0xA564D78B),
            new WeaponComponent(WeaponModel.SMGMkII, "DigitalCamo", 0xC4979067),
            new WeaponComponent(WeaponModel.SMGMkII, "BrushstrokeCamo", 0x3815A945),
            new WeaponComponent(WeaponModel.SMGMkII, "WoodlandCamo", 0x4B4B4FB0),
            new WeaponComponent(WeaponModel.SMGMkII, "Skull", 0xEC729200),
            new WeaponComponent(WeaponModel.SMGMkII, "SessantaNove", 0x48F64B22),
            new WeaponComponent(WeaponModel.SMGMkII, "Perseus", 0x35992468),
            new WeaponComponent(WeaponModel.SMGMkII, "Leopard", 0x24B782A5),
            new WeaponComponent(WeaponModel.SMGMkII, "Zebra", 0xA2E67F01),
            new WeaponComponent(WeaponModel.SMGMkII, "Geometric", 0x2218FD68),
            new WeaponComponent(WeaponModel.SMGMkII, "Boom", 0x45C5C3C5),
            new WeaponComponent(WeaponModel.SMGMkII, "Patriotic", 0x399D558F),
            new WeaponComponent(WeaponModel.MachinePistol, "DefaultClip", 0x476E85FF),
            new WeaponComponent(WeaponModel.MachinePistol, "ExtendedClip", 0xB92C6979),
            new WeaponComponent(WeaponModel.MachinePistol, "DrumMagazine", 0xA9E9CAF4),
            new WeaponComponent(WeaponModel.MachinePistol, "Suppressor", 0xC304849A),
            new WeaponComponent(WeaponModel.CombatPDW, "DefaultClip", 0x4317F19E),
            new WeaponComponent(WeaponModel.CombatPDW, "ExtendedClip", 0x334A5203),
            new WeaponComponent(WeaponModel.CombatPDW, "DrumMagazine", 0x6EB8C8DB),
            new WeaponComponent(WeaponModel.CombatPDW, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.CombatPDW, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.CombatPDW, "Scope", 0xAA2C45B4),
            new WeaponComponent(WeaponModel.PumpShotgun, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.PumpShotgun, "Suppressor", 0xE608B35E),
            new WeaponComponent(WeaponModel.PumpShotgun, "YusufAmirLuxuryFinish", 0xA2D79DDB),
            new WeaponComponent(WeaponModel.SawedOffShotgun, "GildedGunMetalFinish", 0x85A64DF9),
            new WeaponComponent(WeaponModel.AssaultShotgun, "DefaultClip", 0x94E81BC7),
            new WeaponComponent(WeaponModel.AssaultShotgun, "ExtendedClip", 0x86BD7F72),
            new WeaponComponent(WeaponModel.AssaultShotgun, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.AssaultShotgun, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.AssaultShotgun, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.BullpupShotgun, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.BullpupShotgun, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.BullpupShotgun, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "DefaultShells", 0xCD940141),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "DragonsBreathShells", 0x9F8A1BF5),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "SteelBuckshotShells", 0x4E65B425),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "FlechetteShells", 0xE9582927),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "ExplosiveSlugs", 0x3BE4465D),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "SmallScope", 0x49B2945),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "MediumScope", 0x3F3C8181),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Suppressor", 0xAC42DF71),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "SquaredMuzzleBrake", 0x5F7DCE4D),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "DigitalCamo", 0xE3BD9E44),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "BrushstrokeCamo", 0x17148F9B),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "WoodlandCamo", 0x24D22B16),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Skull", 0xF2BEC6F0),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "SessantaNove", 0x85627D),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Perseus", 0xDC2919C5),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Leopard", 0xE184247B),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Zebra", 0xD8EF9356),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Geometric", 0xEF29BFCA),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Boom", 0x67AEB165),
            new WeaponComponent(WeaponModel.PumpShotgunMkII, "Patriotic", 0x46411A1D),
            new WeaponComponent(WeaponModel.HeavyShotgun, "DefaultClip", 0x324F2D5F),
            new WeaponComponent(WeaponModel.HeavyShotgun, "ExtendedClip", 0x971CF6FD),
            new WeaponComponent(WeaponModel.HeavyShotgun, "DrumMagazine", 0x88C7DA53),
            new WeaponComponent(WeaponModel.HeavyShotgun, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.HeavyShotgun, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.HeavyShotgun, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.AssaultRifle, "DefaultClip", 0xBE5EEA16),
            new WeaponComponent(WeaponModel.AssaultRifle, "ExtendedClip", 0xB1214F9B),
            new WeaponComponent(WeaponModel.AssaultRifle, "DrumMagazine", 0xDBF0A53D),
            new WeaponComponent(WeaponModel.AssaultRifle, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.AssaultRifle, "Scope", 0x9D2FBF29),
            new WeaponComponent(WeaponModel.AssaultRifle, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.AssaultRifle, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.AssaultRifle, "YusufAmirLuxuryFinish", 0x4EAD7533),
            new WeaponComponent(WeaponModel.CarbineRifle, "DefaultClip", 0x9FBE33EC),
            new WeaponComponent(WeaponModel.CarbineRifle, "ExtendedClip", 0x91109691),
            new WeaponComponent(WeaponModel.CarbineRifle, "BoxMagazine", 0xBA62E935),
            new WeaponComponent(WeaponModel.CarbineRifle, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.CarbineRifle, "Scope", 0xA0D89C42),
            new WeaponComponent(WeaponModel.CarbineRifle, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.CarbineRifle, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.CarbineRifle, "YusufAmirLuxuryFinish", 0xD89B9658),
            new WeaponComponent(WeaponModel.AdvancedRifle, "DefaultClip", 0xFA8FA10F),
            new WeaponComponent(WeaponModel.AdvancedRifle, "ExtendedClip", 0x8EC1C979),
            new WeaponComponent(WeaponModel.AdvancedRifle, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.AdvancedRifle, "Scope", 0xAA2C45B4),
            new WeaponComponent(WeaponModel.AdvancedRifle, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.AdvancedRifle, "GildedGunMetalFinish", 0x377CD377),
            new WeaponComponent(WeaponModel.SpecialCarbine, "DefaultClip", 0xC6C7E581),
            new WeaponComponent(WeaponModel.SpecialCarbine, "ExtendedClip", 0x7C8BD10E),
            new WeaponComponent(WeaponModel.SpecialCarbine, "DrumMagazine", 0x6B59AEAA),
            new WeaponComponent(WeaponModel.SpecialCarbine, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.SpecialCarbine, "Scope", 0xA0D89C42),
            new WeaponComponent(WeaponModel.SpecialCarbine, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.SpecialCarbine, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.SpecialCarbine, "EtchedGunMetalFinish", 0x730154F2),
            new WeaponComponent(WeaponModel.BullpupRifle, "DefaultClip", 0xC5A12F80),
            new WeaponComponent(WeaponModel.BullpupRifle, "ExtendedClip", 0xB3688B0F),
            new WeaponComponent(WeaponModel.BullpupRifle, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.BullpupRifle, "Scope", 0xAA2C45B4),
            new WeaponComponent(WeaponModel.BullpupRifle, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.BullpupRifle, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.BullpupRifle, "GildedGunMetalFinish", 0xA857BC78),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "DefaultClip", 0x18929DA),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "ExtendedClip", 0xEFB00628),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "TracerRounds", 0x822060A9),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "IncendiaryRounds", 0xA99CF95A),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "ArmorPiercingRounds", 0xFAA7F5ED),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "FullMetalJacketRounds", 0x43621710),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "SmallScope", 0xC7ADD105),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "MediumScope", 0x3F3C8181),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "DefaultBarrel", 0x659AC11B),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "HeavyBarrel", 0x3BF26DC7),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "DigitalCamo", 0xAE4055B7),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "BrushstrokeCamo", 0xB905ED6B),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "WoodlandCamo", 0xA6C448E8),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Skull", 0x9486246C),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "SessantaNove", 0x8A390FD2),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Perseus", 0x2337FC5),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Leopard", 0xEFFFDB5E),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Zebra", 0xDDBDB6DA),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Geometric", 0xCB631225),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Boom", 0xA87D541E),
            new WeaponComponent(WeaponModel.BullpupRifleMkII, "Patriotic", 0xC5E9AE52),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "DefaultClip", 0x16C69281),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "ExtendedClip", 0xDE1FA12C),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "TracerRounds", 0x8765C68A),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "IncendiaryRounds", 0xDE011286),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "ArmorPiercingRounds", 0x51351635),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "FullMetalJacketRounds", 0x503DEA90),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "SmallScope", 0x49B2945),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "LargeScope", 0xC66B6542),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "DefaultBarrel", 0xE73653A9),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "HeavyBarrel", 0xF97F783B),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "DigitalCamo", 0xF97F783B),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "BrushstrokeCamo", 0x431B238B),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "WoodlandCamo", 0x34CF86F4),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Skull", 0xB4C306DD),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "SessantaNove", 0xEE677A25),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Perseus", 0xDF90DC78),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Leopard", 0xA4C31EE),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Zebra", 0x89CFB0F7),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Geometric", 0x7B82145C),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Boom", 0x899CAF75),
            new WeaponComponent(WeaponModel.SpecialCarbineMkII, "Patriotic", 0x5218C819),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "DefaultClip", 0x8610343F),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "ExtendedClip", 0xD12ACA6F),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "TracerRounds", 0xEF2C78C1),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "IncendiaryRounds", 0xFB70D853),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "ArmorPiercingRounds", 0xA7DD1E58),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "FullMetalJacketRounds", 0x63E0A098),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "SmallScope", 0x49B2945),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "LargeScope", 0xC66B6542),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "DefaultBarrel", 0x43A49D26),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "HeavyBarrel", 0x5646C26A),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "DigitalCamo", 0x911B24AF),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "BrushstrokeCamo", 0x37E5444B),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "WoodlandCamo", 0x538B7B97),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Skull", 0x25789F72),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "SessantaNove", 0xC5495F2D),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Perseus", 0xCF8B73B1),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Leopard", 0xA9BB2811),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Zebra", 0xFC674D54),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Geometric", 0x7C7FCD9B),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Boom", 0xA5C38392),
            new WeaponComponent(WeaponModel.AssaultRifleMkII, "Patriotic", 0xB9B15DB0),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "DefaultClip", 0x4C7A391E),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "ExtendedClip", 0x5DD5DBD5),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "TracerRounds", 0x1757F566),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "IncendiaryRounds", 0x3D25C2A7),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "ArmorPiercingRounds", 0x255D5D57),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "FullMetalJacketRounds", 0x44032F11),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "SmallScope", 0x49B2945),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "LargeScope", 0xC66B6542),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "DefaultBarrel", 0x833637FF),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "HeavyBarrel", 0x8B3C480B),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "DigitalCamo", 0x4BDD6F16),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "BrushstrokeCamo", 0x406A7908),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "WoodlandCamo", 0x2F3856A4),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Skull", 0xE50C424D),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "SessantaNove", 0xD37D1F2F),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Perseus", 0x86268483),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Leopard", 0xF420E076),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Zebra", 0xAAE14DF8),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Geometric", 0x9893A95D),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Boom", 0x6B13CD3E),
            new WeaponComponent(WeaponModel.CarbineRifleMkII, "Patriotic", 0xDA55CD3F),
            new WeaponComponent(WeaponModel.CompactRifle, "DefaultClip", 0x513F0A63),
            new WeaponComponent(WeaponModel.CompactRifle, "ExtendedClip", 0x59FF9BF8),
            new WeaponComponent(WeaponModel.CompactRifle, "DrumMagazine", 0xC607740E),
            new WeaponComponent(WeaponModel.MG, "DefaultClip", 0xF434EF84),
            new WeaponComponent(WeaponModel.MG, "ExtendedClip", 0x82158B47),
            new WeaponComponent(WeaponModel.MG, "Scope", 0x3C00AFED),
            new WeaponComponent(WeaponModel.MG, "YusufAmirLuxuryFinish", 0xD6DABABE),
            new WeaponComponent(WeaponModel.CombatMG, "DefaultClip", 0xE1FFB34A),
            new WeaponComponent(WeaponModel.CombatMG, "ExtendedClip", 0xD6C59CD6),
            new WeaponComponent(WeaponModel.CombatMG, "Scope", 0xA0D89C42),
            new WeaponComponent(WeaponModel.CombatMG, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.CombatMG, "EtchedGunMetalFinish", 0x92FECCDD),
            new WeaponComponent(WeaponModel.CombatMGMkII, "DefaultClip", 0x492B257C),
            new WeaponComponent(WeaponModel.CombatMGMkII, "ExtendedClip", 0x17DF42E9),
            new WeaponComponent(WeaponModel.CombatMGMkII, "TracerRounds", 0xF6649745),
            new WeaponComponent(WeaponModel.CombatMGMkII, "IncendiaryRounds", 0xC326BDBA),
            new WeaponComponent(WeaponModel.CombatMGMkII, "ArmorPiercingRounds", 0x29882423),
            new WeaponComponent(WeaponModel.CombatMGMkII, "FullMetalJacketRounds", 0x57EF1CC8),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.CombatMGMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.CombatMGMkII, "MediumScope", 0x3F3C8181),
            new WeaponComponent(WeaponModel.CombatMGMkII, "LargeScope", 0xC66B6542),
            new WeaponComponent(WeaponModel.CombatMGMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.CombatMGMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.CombatMGMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.CombatMGMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.CombatMGMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.CombatMGMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.CombatMGMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.CombatMGMkII, "DefaultBarrel", 0xC34EF234),
            new WeaponComponent(WeaponModel.CombatMGMkII, "HeavyBarrel", 0xB5E2575B),
            new WeaponComponent(WeaponModel.CombatMGMkII, "DigitalCamo", 0x4A768CB5),
            new WeaponComponent(WeaponModel.CombatMGMkII, "BrushstrokeCamo", 0xCCE06BBD),
            new WeaponComponent(WeaponModel.CombatMGMkII, "WoodlandCamo", 0xBE94CF26),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Skull", 0x7609BE11),
            new WeaponComponent(WeaponModel.CombatMGMkII, "SessantaNove", 0x48AF6351),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Perseus", 0x9186750A),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Leopard", 0x84555AA8),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Zebra", 0x1B4C088B),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Geometric", 0xE046DFC),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Boom", 0x28B536E),
            new WeaponComponent(WeaponModel.CombatMGMkII, "Patriotic", 0xD703C94D),
            new WeaponComponent(WeaponModel.GusenbergSweeper, "DefaultClip", 0x1CE5A6A5),
            new WeaponComponent(WeaponModel.GusenbergSweeper, "ExtendedClip", 0xEAC8C270),
            new WeaponComponent(WeaponModel.SniperRifle, "DefaultClip", 0x9BC64089),
            new WeaponComponent(WeaponModel.SniperRifle, "Suppressor", 0xA73D4664),
            new WeaponComponent(WeaponModel.SniperRifle, "Scope", 0xD2443DDC),
            new WeaponComponent(WeaponModel.SniperRifle, "AdvancedScope", 0xBC54DA77),
            new WeaponComponent(WeaponModel.SniperRifle, "EtchedWoodGripFinish", 0x4032B5E7),
            new WeaponComponent(WeaponModel.HeavySniper, "DefaultClip", 0x476F52F4),
            new WeaponComponent(WeaponModel.HeavySniper, "Scope", 0xD2443DDC),
            new WeaponComponent(WeaponModel.HeavySniper, "AdvancedScope", 0xBC54DA77),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "DefaultClip", 0x94E12DCE),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "ExtendedClip", 0xE6CFD1AA),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "TracerRounds", 0xD77A22D2),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "IncendiaryRounds", 0x6DD7A86E),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "ArmorPiercingRounds", 0xF46FD079),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "FullMetalJacketRounds", 0xE14A9ED3),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "HolographicSight", 0x420FD713),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "LargeScope", 0xC66B6542),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "ZoomScope", 0x5B1C713C),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "DefaultBarrel", 0x381B5D89),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "HeavyBarrel", 0x68373DDC),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Grip", 0x9D65907A),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "DigitalCamo", 0x9094FBA0),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "BrushstrokeCamo", 0x7320F4B2),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "WoodlandCamo", 0x60CF500F),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Skull", 0xFE668B3F),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "SessantaNove", 0xF3757559),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Perseus", 0x193B40E8),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Leopard", 0x107D2F6C),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Zebra", 0xC4E91841),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Geometric", 0x9BB1C5D3),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Boom", 0x3B61040B),
            new WeaponComponent(WeaponModel.MarksmanRifleMkII, "Patriotic", 0xB7A316DA),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "DefaultClip", 0xFA1E1A28),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "ExtendedClip", 0x2CD8FF9D),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "IncendiaryRounds", 0xEC0F617),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "ArmorPiercingRounds", 0xF835D6D4),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "FullMetalJacketRounds", 0x3BE948F6),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "ExplosiveRounds", 0x89EBDAA7),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "ZoomScope", 0x82C10383),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "AdvancedScope", 0xBC54DA77),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "NightVisionScope", 0xB68010B0),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "ThermalScope", 0x2E43DA41),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Suppressor", 0xAC42DF71),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "SquaredMuzzleBrake", 0x5F7DCE4D),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "BellEndMuzzleBrake", 0x6927E1A1),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "DefaultBarrel", 0x909630B7),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "HeavyBarrel", 0x108AB09E),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "DigitalCamo", 0xF8337D02),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "BrushstrokeCamo", 0xC5BEDD65),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "WoodlandCamo", 0xE9712475),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Skull", 0x13AA78E7),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "SessantaNove", 0x26591E50),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Perseus", 0x302731EC),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Leopard", 0xAC722A78),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Zebra", 0xBEA4CEDD),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Geometric", 0xCD776C82),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Boom", 0xABC5ACC7),
            new WeaponComponent(WeaponModel.HeavySniperMkII, "Patriotic", 0x6C32D2EB),
            new WeaponComponent(WeaponModel.MarksmanRifle, "DefaultClip", 0xD83B4141),
            new WeaponComponent(WeaponModel.MarksmanRifle, "ExtendedClip", 0xCCFD2AC5),
            new WeaponComponent(WeaponModel.MarksmanRifle, "Scope", 0x1C221B1A),
            new WeaponComponent(WeaponModel.MarksmanRifle, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.MarksmanRifle, "Suppressor", 0x837445AA),
            new WeaponComponent(WeaponModel.MarksmanRifle, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.MarksmanRifle, "YusufAmirLuxuryFinish", 0x161E9241),
            new WeaponComponent(WeaponModel.GrenadeLauncher, "DefaultClip", 0x11AE5C97),
            new WeaponComponent(WeaponModel.GrenadeLauncher, "Flashlight", 0x7BC4CDDC),
            new WeaponComponent(WeaponModel.GrenadeLauncher, "Grip", 0xC164F53),
            new WeaponComponent(WeaponModel.GrenadeLauncher, "Scope", 0xAA2C45B4),
        };

        public static List<EmergencyCall> EmergencyCalls { get; set; }

        public static DiscordSocketClient DiscordClient { get; set; }

        public static List<Spotlight> Spotlights { get; set; } = new();

        public static IEnumerable<MyPlayer> Players { get => Alt.GetAllPlayers().Cast<MyPlayer>(); }

        public static IEnumerable<MyVehicle> Vehicles { get => Alt.GetAllVehicles().Cast<MyVehicle>(); }

        public static List<FactionUnit> FactionsUnits { get; set; }

        public static List<FactionUnitCharacter> FactionsUnitsCharacters { get; set; } = new();

        public static List<Item> Items { get; set; }

        public static List<ClotheAccessory> Clothes1Male { get; set; }

        public static List<ClotheAccessory> Clothes1Female { get; set; }

        public static List<ClotheAccessory> Clothes3Male { get; set; }

        public static List<ClotheAccessory> Clothes3Female { get; set; }

        public static List<ClotheAccessory> Clothes4Male { get; set; }

        public static List<ClotheAccessory> Clothes4Female { get; set; }

        public static List<ClotheAccessory> Clothes5Male { get; set; }

        public static List<ClotheAccessory> Clothes5Female { get; set; }

        public static List<ClotheAccessory> Clothes6Male { get; set; }

        public static List<ClotheAccessory> Clothes6Female { get; set; }

        public static List<ClotheAccessory> Clothes7Male { get; set; }

        public static List<ClotheAccessory> Clothes7Female { get; set; }

        public static List<ClotheAccessory> Clothes8Male { get; set; }

        public static List<ClotheAccessory> Clothes8Female { get; set; }

        public static List<ClotheAccessory> Clothes9Male { get; set; }

        public static List<ClotheAccessory> Clothes9Female { get; set; }

        public static List<ClotheAccessory> Clothes10Male { get; set; }

        public static List<ClotheAccessory> Clothes10Female { get; set; }

        public static List<ClotheAccessory> Clothes11Male { get; set; }

        public static List<ClotheAccessory> Clothes11Female { get; set; }

        public static List<ClotheAccessory> Accessories0Male { get; set; }

        public static List<ClotheAccessory> Accessories0Female { get; set; }

        public static List<ClotheAccessory> Accessories1Male { get; set; }

        public static List<ClotheAccessory> Accessories1Female { get; set; }

        public static List<ClotheAccessory> Accessories2Male { get; set; }

        public static List<ClotheAccessory> Accessories2Female { get; set; }

        public static List<ClotheAccessory> Accessories6Male { get; set; }

        public static List<ClotheAccessory> Accessories6Female { get; set; }

        public static List<ClotheAccessory> Accessories7Male { get; set; }

        public static List<ClotheAccessory> Accessories7Female { get; set; }

        public static WeatherInfo WeatherInfo { get; set; }

        public static List<Door> Doors { get; set; }

        public static List<string> IPLs { get; set; }

        public static List<Info> Infos { get; set; }

        public static List<FactionDrugHouse> FactionsDrugsHouses { get; set; }

        public static List<FactionDrugHouseItem> FactionsDrugsHousesItems { get; set; }

        public static List<CrackDen> CrackDens { get; set; }

        public static List<CrackDenItem> CrackDensItems { get; set; }

        public static List<TruckerLocation> TruckerLocations { get; set; }

        public static List<TruckerLocationDelivery> TruckerLocationsDeliveries { get; set; }

        public static List<AudioSpot> AudioSpots { get; set; } = new();

        public static List<Furniture> Furnitures { get; set; } = new();

        public static List<Animation> Animations { get; set; } = new();

        public static List<Company> Companies { get; set; } = new();

        public static List<Tuple<string, string>> Scenarios { get; set; } = new()
        {
            new("WORLD_HUMAN_AA_COFFEE", "Spawna um copo de café e segura com as duas mãos a frente do peito"),
            new("WORLD_HUMAN_AA_SMOKE", "Spawna um cigarro aceso na mão direita e dá tragos"),
            new("WORLD_HUMAN_BINOCULARS", "Spawna um binóculo e observa através dele algumas vezes"),
            new("WORLD_HUMAN_BUM_FREEWAY", "Spawna uma papelão de mendigo e segura com as duas mãos a frente do peito "),
            new("WORLD_HUMAN_BUM_SLUMPED", "Deita no chão e faz alguns movimentos"),
            new("WORLD_HUMAN_BUM_STANDING", "Faz uma pose com os ombros arqueados e faz alguns movimentos"),
            new("WORLD_HUMAN_BUM_WASH", "Abaixa-se e faz alguns movimentos jogando água no próprio corpo"),
            new("WORLD_HUMAN_VALET", "Mãos para trás com alguns movimentos"),
            new("WORLD_HUMAN_CAR_PARK_ATTENDANT", "Utiliza um sinalizador para orientar o trânsito"),
            new("WORLD_HUMAN_CHEERING", "Aplaude de baixo para cima"),
            new("WORLD_HUMAN_CLIPBOARD", "Spawna uma prancheta na mão esquerda e fazer alguns movimentos"),
            new("WORLD_HUMAN_CLIPBOARD_FACILITY", "Spawna uma prancheta na mão esquerda e fazer alguns movimentos"),
            new("WORLD_HUMAN_CONST_DRILL", "Spawna uma britadeira e faz algumas animações"),
            new("WORLD_HUMAN_COP_IDLES", "Coloca as mãos na cintura segurando o cinto e faz alguns movimentos"),
            new("WORLD_HUMAN_DRINKING", "Spawna uma garrafa ensacada ou não (aleatório) na mão direita, bebe alguns goles e faz algumas animações"),
            new("WORLD_HUMAN_DRINKING_FACILITY", "Spawna uma garrafa ensacada ou não (aleatório) na mão direita, bebe alguns goles e faz algumas animações"),
            new("WORLD_HUMAN_DRINKING_CASINO_TERRACE", "Spawna uma garrafa ensacada ou não (aleatório) na mão direita, bebe alguns goles e faz algumas animações"),
            new("WORLD_HUMAN_DRUG_DEALER", "Spawna um baseado na mão esquerda e dá tragos"),
            new("WORLD_HUMAN_DRUG_DEALER_HARD", "Faz alguns movimentos simulando tragos"),
            new("WORLD_HUMAN_MOBILE_FILM_SHOCKING", "Retira um celular do bolso traseiro da calça e o usa para filmar algo a frente"),
            new("WORLD_HUMAN_GARDENER_LEAF_BLOWER", "Spawna um soprador na mão direita, balança ele algumas vezes a sua frente"),
            new("WORLD_HUMAN_GARDENER_PLANT", "Pega uma pequena pá no chão e cava alguns buracos"),
            new("WORLD_HUMAN_GUARD_PATROL", "Olha várias vezes de um lado para o outro desconfiado e faz alguns movimentos"),
            new("WORLD_HUMAN_GUARD_STAND", "Mãos na frente do corpo na altura da cintura e faz alguns movimentos"),
            new("WORLD_HUMAN_GUARD_STAND_CASINO", "Mãos na frente do corpo na altura da cintura e faz alguns movimentos"),
            new("WORLD_HUMAN_GUARD_STAND_CLUBHOUSE", "Mãos na frente do corpo na altura da cintura e faz alguns movimentos"),
            new("WORLD_HUMAN_GUARD_STAND_FACILITY", "Mãos na frente do corpo na altura da cintura e faz alguns movimentos"),
            new("WORLD_HUMAN_HAMMERING", "Spawna um martelo na mão direita e dá marteladas"),
            new("WORLD_HUMAN_HANG_OUT_STREET", "Conversa e dá risadas gesticulando com as mãos e com a cabeça"),
            new("WORLD_HUMAN_HANG_OUT_STREET_CLUBHOUSE", "Conversa e dá risadas gesticulando com as mãos e com a cabeça"),
            new("WORLD_HUMAN_HIKER_STANDING", "Faz movimentos como se estivesse segurando as alças de uma mochila"),
            new("WORLD_HUMAN_HUMAN_STATUE", "Faz uma pose para foto"),
            new("WORLD_HUMAN_JANITOR", "Spawna uma vassoura na mão direita e fica apoiado nela "),
            new("WORLD_HUMAN_JOG", "Faz alguns trotes no lugar, se preparando para correr"),
            new("WORLD_HUMAN_JOG_STANDING", "Faz alguns trotes no lugar, se preparando para correr e faz alguns alongamentos"),
            new("WORLD_HUMAN_LEANING", "Apoia-se de costas em uma parede e faz alguns movimentos com os braços e mãos"),
            new("WORLD_HUMAN_LEANING_CASINO_TERRACE", "Apoia-se de costas em uma parede e faz alguns movimentos com os braços e mãos"),
            new("WORLD_HUMAN_MAID_CLEAN", "Spawna um pano amarelo na mão direita e esfrega/limpa algo"),
            new("WORLD_HUMAN_MUSCLE_FLEX", "Flexiona os dois braços e exibe os músculos"),
            new("WORLD_HUMAN_MUSCLE_FREE_WEIGHTS", "Spawna uma barra W com duas anilhas de cada lado e faz musculação"),
            new("WORLD_HUMAN_MUSICIAN", "Spawna um bongô embaixo do braço esquerdo e toca o instrumento"),
            new("WORLD_HUMAN_PAPARAZZI", "Spawna uma câmera profissional e tira algumas fotos"),
            new("WORLD_HUMAN_PARTYING", "Spawna uma garrafa na mão esquerda, toma alguns goles enquanto dança e conversa"),
            new("WORLD_HUMAN_PICNIC", "Senta-se no chão com as pernas esticadas como se estivesse tomando sol na praia"),
            new("WORLD_HUMAN_PROSTITUTE_HIGH_CLASS", "Spawna um cigarro na mão direita, traga algumas vezes e faz movimentos sugestivos com mãos na cintura rebolando"),
            new("WORLD_HUMAN_PROSTITUTE_LOW_CLASS", "Faz movimentos sugestivos com mãos na cintura rebolando"),
            new("WORLD_HUMAN_PUSH_UPS", "Faz flexões de braço no chão, com pequenos intervalos"),
            new("WORLD_HUMAN_SEAT_LEDGE", "Se senta em uma mureta/local alto"),
            new("WORLD_HUMAN_SEAT_LEDGE_EATING", "Se senta em uma mureta/local alto e spawna uma rosquinha em mãos e dá algumas mastigadas"),
            new("WORLD_HUMAN_SEAT_WALL", "Se senta em uma mureta/local alto"),
            new("WORLD_HUMAN_SEAT_WALL_EATING", "Se senta em uma mureta/local alto e spawna uma rosquinha em mãos e dá algumas mastigadas"),
            new("WORLD_HUMAN_SECURITY_SHINE_TORCH", "Retira uma lanterna do bolso"),
            new("WORLD_HUMAN_SIT_UPS", "Deita-se no chão e faz abdominais"),
            new("WORLD_HUMAN_SMOKING", "Retira um cigarro do bolso traseiro da calça, acende e dá alguns tragos"),
            new("WORLD_HUMAN_SMOKING_CLUBHOUSE", "Retira um cigarro do bolso traseiro da calça, acende e dá alguns tragos"),
            new("WORLD_HUMAN_SMOKING_POT", "Spawna um baseado na mão esquerda e fuma"),
            new("WORLD_HUMAN_SMOKING_POT_CLUBHOUSE", "Spawna um baseado na mão esquerda e fuma"),
            new("WORLD_HUMAN_STAND_FIRE", "Aquece as mãos em uma fogueira"),
            new("WORLD_HUMAN_STAND_FISHING", "Spawna uma vara de pescar e a segura com ambas as mãos"),
            new("WORLD_HUMAN_STAND_IMPATIENT", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("WORLD_HUMAN_STAND_IMPATIENT_CLUBHOUSE", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("WORLD_HUMAN_STAND_IMPATIENT_FACILITY", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT_FACILITY", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("PROP_HUMAN_STAND_IMPATIENT", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("CODE_HUMAN_CROSS_ROAD_WAIT", "Balança os braços, olha no relógio e procura por algo/alguém ao longe"),
            new("WORLD_HUMAN_STAND_MOBILE", "Retira um celular do bolso e mexe nele algumas vezes"),
            new("WORLD_HUMAN_STAND_MOBILE_CLUBHOUSE", "Retira um celular do bolso e mexe nele algumas vezes"),
            new("WORLD_HUMAN_STAND_MOBILE_FACILITY", "Retira um celular do bolso e mexe nele algumas vezes"),
            new("WORLD_HUMAN_STAND_MOBILE_UPRIGHT", "Retira um celular do bolso e mexe nele algumas vezes"),
            new("WORLD_HUMAN_STAND_MOBILE_UPRIGHT_CLUBHOUSE", "Retira um celular do bolso e mexe nele algumas vezes"),
            new("WORLD_HUMAN_STRIP_WATCH_STAND", "Conversa animada com dancinhas e aplausos"),
            new("WORLD_HUMAN_STUPOR", "Deitado no chão com as pernas esticadas, tosse e faz menção a cochilar mas acaba acordando"),
            new("WORLD_HUMAN_STUPOR_CLUBHOUSE", "Sentado no chão olhando para baixo e ao redor"),
            new("WORLD_HUMAN_SUNBATHE", "Deita de bruços na posição de tomar sol"),
            new("WORLD_HUMAN_SUNBATHE_BACK", "Deita de barriga pra cima na posição de tomar sol"),
            new("WORLD_HUMAN_TENNIS_PLAYER", "Spawna uma raqueta rosa e faz alguns movimentos"),
            new("WORLD_HUMAN_TOURIST_MAP", "Spawna uma mapa turístico de Los santos"),
            new("WORLD_HUMAN_TOURIST_MOBILE", "Spawna um celular na mão esquerda e faz alguns movimentos como se estivesse tirando uma selfie"),
            new("WORLD_HUMAN_VEHICLE_MECHANIC", "Deita no chão com os braços para cima como se estivesse mexendo na parte inferior de um carro"),
            new("WORLD_HUMAN_WELDING", "Spawna uma maçarico e segura ele com as duas mãos liga ele em um objeto a frente"),
            new("WORLD_HUMAN_WINDOW_SHOP_BROWSE", "Observa algo com atenção e cruza o braço algumas vezes"),
            new("WORLD_HUMAN_YOGA", "Pose de yoga"),
            new("PROP_HUMAN_ATM", "Usa uma ATM"),
            new("PROP_HUMAN_BBQ", "Spawna uma espátula de cozinha na mão direita e a usa para mexer na churrasqueira"),
            new("PROP_HUMAN_BUM_BIN", "Abaixa-se como se estivesse tentando pegar algo dentro de uma caixa"),
            new("PROP_HUMAN_BUM_SHOPPING_CART", "Apoia-se em um balcão e faz alguns movimentos"),
            new("PROP_HUMAN_MUSCLE_CHIN_UPS", "Faz exercícios em uma barra paralela"),
            new("PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY", "Faz exercícios em uma barra paralela"),
            new("PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON", "Faz exercícios em uma barra paralela"),
            new("PROP_HUMAN_PARKING_METER", "Separa droga pequenas com as mãos"),
            new("PROP_HUMAN_SEAT_BAR", "Sentado em uma banqueta com as mãos apoiadas em um balcão"),
            new("CODE_HUMAN_MEDIC_KNEEL", "Procura algo no chão investigando"),
            new("CODE_HUMAN_MEDIC_TEND_TO_DEAD", "Procura algo no chão investigando"),
            new("CODE_HUMAN_MEDIC_TIME_OF_DEATH", "Spawna um bloco de notas e escreve algo nele"),
            new("CODE_HUMAN_POLICE_CROWD_CONTROL", "Orienta algumas pessoas próximas"),
            new("CODE_HUMAN_POLICE_INVESTIGATE", "Coloca as mãos na cintura e investiga o local e fala no rádio algumas vezes"),
        };

        public static List<MethodInfo> Commands { get; set; }

        public static string DbConnectionString { get; set; } = "Server=localhost;Database=bdsegundavidaroleplay;Uid=root;Password=159357";

        public static string DiscordBotToken { get; set; }

        public static ulong AnnouncementDiscordChannel { get; set; }

        public static ulong GovernmentAnnouncementDiscordChannel { get; set; }

        public static ulong StaffDiscordChannel { get; set; }

        public static List<ulong> RolesStaffMessage { get; set; }

        public static ulong CompanyAnnouncementDiscordChannel { get; set; }

        public static IEnumerable<MyObject> Objects => Alt.GetAllNetworkObjects().Cast<MyObject>();
    }
}