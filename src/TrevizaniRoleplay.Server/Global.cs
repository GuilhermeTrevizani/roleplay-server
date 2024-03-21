using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Discord.WebSocket;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server
{
    public sealed class Global
    {
        // O que for globalização, levar para o Globalization
        // O que for constantes de configurações, levar pro Constants
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

        public static Parameter Parameter { get; set; } = default!;

        public static List<Blip> Blips { get; set; } = [];

        public static List<Faction> Factions { get; set; } = [];

        public static List<FactionRank> FactionsRanks { get; set; } = [];

        public static List<Property> Properties { get; set; } = [];

        public static List<Price> Prices { get; set; } = [];

        public static List<Spot> Spots { get; set; } = [];

        public static List<FactionStorage> FactionsStorages { get; set; } = [];

        public static List<FactionStorageItem> FactionsStoragesItems { get; set; } = [];

        public static List<Question> Questions { get; set; } = [];

        public static List<QuestionAnswer> QuestionsAnswers { get; set; } = [];

        public static ReadOnlyCollection<Dealership> Dealerships { get; } = new List<Dealership>
        {
            new()
            {
                Name = "CONCESSIONÁRIA DE BARCOS",
                PriceType = PriceType.Boats,
                Position = new Position(-787.1262f, -1354.725f, 5.150271f),
                VehiclePosition = new Position(-805.2659f, -1418.4264f, 0.33190918f),
                VehicleRotation = new Position(-0.015625f, 0, 0.859375f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE HELICÓPTEROS",
                PriceType = PriceType.Helicopters,
                Position = new Position(-753.5287f, -1512.43f, 5.020952f),
                VehiclePosition = new Position(-745.4902f, -1468.695f, 5.099712f),
                VehicleRotation = new Position(0, 0, 328.6675f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE AVIÕES",
                PriceType = PriceType.Airplanes,
                Position = new Position(-941.34064f, -2954.8748f, 13.9296875f),
                VehiclePosition = new Position(-979.5824f, -2996.3472f, 13.457886f),
                VehicleRotation = new Position(0f, 0f, 1.046875f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE MOTOCICLETAS E BICICLETAS",
                PriceType = PriceType.MotorcyclesAndBicycles,
                Position = new Position(268.73407f, -1155.455f, 29.279907f),
                VehiclePosition = new Position(255.13846f, -1155.9165f, 28.774414f),
                VehicleRotation = new Position(0f, -0.015625f, 1.578125f),
            },
            new()
            {
                Name = "CONCESSIONÁRIA DE CARROS",
                PriceType = PriceType.Cars,
                Position = new Position(-56.096703f, -1099.3978f, 26.415405f),
                VehiclePosition = new Position(-57.81099f, -1107.6263f, 25.96045f),
                VehicleRotation = new Position(0f, 0f, 1.25f),
            },
        }.AsReadOnly();

        public static ReadOnlyCollection<Job> Jobs { get; } = new List<Job>
        {
            new()
            {
                CharacterJob = CharacterJob.TaxiDriver,
                Position = new Position(895.0308f, -179.1359f, 74.70036f),
                VehicleModel = VehicleModel.Taxi,
                VehicleRentPosition = new Position(907.33185f, -175.21318f, 74.11719f),
                VehicleRentRotation = new Rotation(0f, 0f, -2.1273777f),
                VehicleColor = new Rgba(252, 186, 3, 255),
            },
            new()
            {
                CharacterJob = CharacterJob.Mechanic,
                Position = new Position(-27.178022f, -1673.1956f, 29.482056f),
                VehicleModel = VehicleModel.TowTruck2,
                VehicleRentPosition = new Position(-27.96923f, -1680.8967f, 29.431519f),
                VehicleRentRotation = new Rotation(0f, 0f, 2.0779037f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
            new()
            {
                CharacterJob = CharacterJob.GarbageCollector,
                Position = new Position(-355.18683f, -1513.411f, 27.712769f),
                VehicleModel = VehicleModel.Trash,
                VehicleRentPosition = new Position(-350.62418f, -1520.6901f, 27.712769f),
                VehicleRentRotation = new Rotation(0f, 0f, -1.6326387f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
            new()
            {
                CharacterJob = CharacterJob.Trucker,
                Position = new Position(895.1341f, -896.2813f, 27.780273f),
                VehicleModel = VehicleModel.Burrito,
                VehicleRentPosition = new Position(886.87915f, -889.3714f, 26.533325f),
                VehicleRentRotation = new Rotation(0f, 0f, 1.5831648f),
                VehicleColor = new Rgba(0, 0, 0, 255),
            },
        }.AsReadOnly();

        public static List<HelpRequest> HelpRequests { get; set; } = [];

        public static List<WeaponComponent> WeaponComponents { get; } =
        [
            new(WeaponModel.BrassKnuckles, "BaseModel", 0xF3462F33),
            new(WeaponModel.BrassKnuckles, "ThePimp", 0xC613F685),
            new(WeaponModel.BrassKnuckles, "TheBallas", 0xEED9FD63),
            new(WeaponModel.BrassKnuckles, "TheHustler", 0x50910C31),
            new(WeaponModel.BrassKnuckles, "TheRock", 0x9761D9DC),
            new(WeaponModel.BrassKnuckles, "TheHater", 0x7DECFE30),
            new(WeaponModel.BrassKnuckles, "TheLover", 0x3F4E8AA6),
            new(WeaponModel.BrassKnuckles, "ThePlayer", 0x8B808BB),
            new(WeaponModel.BrassKnuckles, "TheKing", 0xE28BABEF),
            new(WeaponModel.BrassKnuckles, "TheValor", 0x7AF3F785),
            new(WeaponModel.Switchblade, "DefaultHandle", 0x9137A500),
            new(WeaponModel.Switchblade, "VIPVariant", 0x5B3E7DB6),
            new(WeaponModel.Switchblade, "BodyguardVariant", 0xE7939662),
            new(WeaponModel.Pistol, "DefaultClip", 0xFED0FD71),
            new(WeaponModel.Pistol, "ExtendedClip", 0xED265A1C),
            new(WeaponModel.Pistol, "Flashlight", 0x359B7AAE),
            new(WeaponModel.Pistol, "Suppressor", 0x65EA7EBB),
            new(WeaponModel.Pistol, "YusufAmirLuxuryFinish", 0xD7391086),
            new(WeaponModel.CombatPistol, "DefaultClip", 0x721B079),
            new(WeaponModel.CombatPistol, "ExtendedClip", 0xD67B4F2D),
            new(WeaponModel.CombatPistol, "Flashlight", 0x359B7AAE),
            new(WeaponModel.CombatPistol, "Suppressor", 0xC304849A),
            new(WeaponModel.CombatPistol, "YusufAmirLuxuryFinish", 0xC6654D72),
            new(WeaponModel.APPistol, "DefaultClip", 0x31C4B22A),
            new(WeaponModel.APPistol, "ExtendedClip", 0x249A17D5),
            new(WeaponModel.APPistol, "Flashlight", 0x359B7AAE),
            new(WeaponModel.APPistol, "Suppressor", 0xC304849A),
            new(WeaponModel.APPistol, "GildedGunMetalFinish", 0x9B76C72C),
            new(WeaponModel.Pistol50, "DefaultClip", 0x2297BE19),
            new(WeaponModel.Pistol50, "ExtendedClip", 0xD9D3AC92),
            new(WeaponModel.Pistol50, "Flashlight", 0x359B7AAE),
            new(WeaponModel.Pistol50, "Suppressor", 0xA73D4664),
            new(WeaponModel.Pistol50, "PlatinumPearlDeluxeFinish", 0x77B8AB2F),
            new(WeaponModel.HeavyRevolver, "VIPVariant", 0x16EE3040),
            new(WeaponModel.HeavyRevolver, "BodyguardVariant", 0x9493B80D),
            new(WeaponModel.HeavyRevolver, "DefaultClip", 0xE9867CE3),
            new(WeaponModel.SNSPistol, "DefaultClip", 0xF8802ED9),
            new(WeaponModel.SNSPistol, "ExtendedClip", 0x7B0033B3),
            new(WeaponModel.SNSPistol, "EtchedWoodGripFinish", 0x8033ECAF),
            new(WeaponModel.HeavyPistol, "DefaultClip", 0xD4A969A),
            new(WeaponModel.HeavyPistol, "ExtendedClip", 0x64F9C62B),
            new(WeaponModel.HeavyPistol, "Flashlight", 0x359B7AAE),
            new(WeaponModel.HeavyPistol, "Suppressor", 0xC304849A),
            new(WeaponModel.HeavyPistol, "EtchedWoodGripFinish", 0x7A6A7B7B),
            new(WeaponModel.HeavyRevolverMkII, "DefaultRounds", 0xBA23D8BE),
            new(WeaponModel.HeavyRevolverMkII, "TracerRounds", 0xC6D8E476),
            new(WeaponModel.HeavyRevolverMkII, "IncendiaryRounds", 0xEFBF25),
            new(WeaponModel.HeavyRevolverMkII, "HollowPointRounds", 0x10F42E8F),
            new(WeaponModel.HeavyRevolverMkII, "FullMetalJacketRounds", 0xDC8BA3F),
            new(WeaponModel.HeavyRevolverMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.HeavyRevolverMkII, "SmallScope", 0x49B2945),
            new(WeaponModel.HeavyRevolverMkII, "Flashlight", 0x359B7AAE),
            new(WeaponModel.HeavyRevolverMkII, "Compensator", 0x27077CCB),
            new(WeaponModel.HeavyRevolverMkII, "DigitalCamo", 0xC03FED9F),
            new(WeaponModel.HeavyRevolverMkII, "BrushstrokeCamo", 0xB5DE24),
            new(WeaponModel.HeavyRevolverMkII, "WoodlandCamo", 0xA7FF1B8),
            new(WeaponModel.HeavyRevolverMkII, "Skull", 0xF2E24289),
            new(WeaponModel.HeavyRevolverMkII, "SessantaNove", 0x11317F27),
            new(WeaponModel.HeavyRevolverMkII, "Perseus", 0x17C30C42),
            new(WeaponModel.HeavyRevolverMkII, "Leopard", 0x257927AE),
            new(WeaponModel.HeavyRevolverMkII, "Zebra", 0x37304B1C),
            new(WeaponModel.HeavyRevolverMkII, "Geometric", 0x48DAEE71),
            new(WeaponModel.HeavyRevolverMkII, "Boom", 0x20ED9B5B),
            new(WeaponModel.HeavyRevolverMkII, "Patriotic", 0xD951E867),
            new(WeaponModel.SNSPistolMkII, "DefaultClip", 0x1466CE6),
            new(WeaponModel.SNSPistolMkII, "ExtendedClip", 0xCE8C0772),
            new(WeaponModel.SNSPistolMkII, "TracerRounds", 0x902DA26E),
            new(WeaponModel.SNSPistolMkII, "IncendiaryRounds", 0xE6AD5F79),
            new(WeaponModel.SNSPistolMkII, "HollowPointRounds", 0x8D107402),
            new(WeaponModel.SNSPistolMkII, "FullMetalJacketRounds", 0xC111EB26),
            new(WeaponModel.SNSPistolMkII, "Flashlight", 0x4A4965F3),
            new(WeaponModel.SNSPistolMkII, "MountedScope", 0x47DE9258),
            new(WeaponModel.SNSPistolMkII, "Suppressor", 0x65EA7EBB),
            new(WeaponModel.SNSPistolMkII, "Compensator", 0xAA8283BF),
            new(WeaponModel.SNSPistolMkII, "DigitalCamo", 0xF7BEEDD),
            new(WeaponModel.SNSPistolMkII, "BrushstrokeCamo", 0x8A612EF6),
            new(WeaponModel.SNSPistolMkII, "WoodlandCamo", 0x76FA8829),
            new(WeaponModel.SNSPistolMkII, "Skull", 0xA93C6CAC),
            new(WeaponModel.SNSPistolMkII, "SessantaNove", 0x9C905354),
            new(WeaponModel.SNSPistolMkII, "Perseus", 0x4DFA3621),
            new(WeaponModel.SNSPistolMkII, "Leopard", 0x42E91FFF),
            new(WeaponModel.SNSPistolMkII, "Zebra", 0x54A8437D),
            new(WeaponModel.SNSPistolMkII, "Geometric", 0x68C2746),
            new(WeaponModel.SNSPistolMkII, "Boom", 0x2366E467),
            new(WeaponModel.SNSPistolMkII, "Boom2", 0x441882E6),
            new(WeaponModel.SNSPistolMkII, "DigitalCamo", 0xE7EE68EA),
            new(WeaponModel.SNSPistolMkII, "BrushstrokeCamo", 0x29366D21),
            new(WeaponModel.SNSPistolMkII, "WoodlandCamo", 0x3ADE514B),
            new(WeaponModel.SNSPistolMkII, "SkullSlide", 0xE64513E9),
            new(WeaponModel.SNSPistolMkII, "SessantaNoveSlide", 0xCD7AEB9A),
            new(WeaponModel.SNSPistolMkII, "PerseusSlide", 0xFA7B27A6),
            new(WeaponModel.SNSPistolMkII, "LeopardSlide", 0xE285CA9A),
            new(WeaponModel.SNSPistolMkII, "ZebraSlide", 0x2B904B19),
            new(WeaponModel.SNSPistolMkII, "GeometricSlide", 0x22C24F9C),
            new(WeaponModel.SNSPistolMkII, "BoomSlide", 0x8D0D5ECD),
            new(WeaponModel.SNSPistolMkII, "Patriotic", 0x1F07150A),
            new(WeaponModel.PistolMkII, "DefaultClip", 0x94F42D62),
            new(WeaponModel.PistolMkII, "ExtendedClip", 0x5ED6C128),
            new(WeaponModel.PistolMkII, "TracerRounds", 0x25CAAEAF),
            new(WeaponModel.PistolMkII, "IncendiaryRounds", 0x2BBD7A3A),
            new(WeaponModel.PistolMkII, "HollowPointRounds", 0x85FEA109),
            new(WeaponModel.PistolMkII, "FullMetalJacketRounds", 0x4F37DF2A),
            new(WeaponModel.PistolMkII, "MountedScope", 0x8ED4BB70),
            new(WeaponModel.PistolMkII, "Flashlight	", 0x43FD595B),
            new(WeaponModel.PistolMkII, "Suppressor", 0x65EA7EBB),
            new(WeaponModel.PistolMkII, "Compensator", 0x21E34793),
            new(WeaponModel.PistolMkII, "DigitalCamo", 0x5C6C749C),
            new(WeaponModel.PistolMkII, "BrushstrokeCamo", 0x15F7A390),
            new(WeaponModel.PistolMkII, "WoodlandCamo", 0x968E24DB),
            new(WeaponModel.PistolMkII, "Skull", 0x17BFA99),
            new(WeaponModel.PistolMkII, "SessantaNove", 0xF2685C72),
            new(WeaponModel.PistolMkII, "Perseus", 0xDD2231E6),
            new(WeaponModel.PistolMkII, "Leopard", 0xBB43EE76),
            new(WeaponModel.PistolMkII, "Zebra", 0x4D901310),
            new(WeaponModel.PistolMkII, "Geometric", 0x5F31B653),
            new(WeaponModel.PistolMkII, "Boom", 0x697E19A0),
            new(WeaponModel.PistolMkII, "Patriotic", 0x930CB951),
            new(WeaponModel.PistolMkII, "DigitalCamoSlide", 0xB4FC92B0),
            new(WeaponModel.PistolMkII, "BrushstrokeCamoSlide", 0x1A1F1260),
            new(WeaponModel.PistolMkII, "WoodlandCamoSlime", 0xE4E00B70),
            new(WeaponModel.PistolMkII, "SkullSlide", 0x2C298B2B),
            new(WeaponModel.PistolMkII, "SessantaNoveSlide", 0xDFB79725),
            new(WeaponModel.PistolMkII, "PerseusSlide", 0x6BD7228C),
            new(WeaponModel.PistolMkII, "LeopardSlide", 0x9DDBCF8C),
            new(WeaponModel.PistolMkII, "ZebraSlide", 0xB319A52C),
            new(WeaponModel.PistolMkII, "GeometricSlide", 0xC6836E12),
            new(WeaponModel.PistolMkII, "BoomSlide", 0x43B1B173),
            new(WeaponModel.PistolMkII, "PatrioticSlide", 0x4ABDA3FA),
            new(WeaponModel.VintagePistol, "DefaultClip", 0x45A3B6BB),
            new(WeaponModel.VintagePistol, "ExtendedClip", 0x33BA12E8),
            new(WeaponModel.VintagePistol, "Suppressor", 0xC304849A),
            new(WeaponModel.UpnAtomizer, "FestiveTint", 0xD7DBF707),
            new(WeaponModel.MicroSMG, "DefaultClip", 0xCB48AEF0),
            new(WeaponModel.MicroSMG, "ExtendedClip", 0x10E6BA2B),
            new(WeaponModel.MicroSMG, "Flashlight", 0x359B7AAE),
            new(WeaponModel.MicroSMG, "Scope", 0x9D2FBF29),
            new(WeaponModel.MicroSMG, "Suppressor", 0xA73D4664),
            new(WeaponModel.MicroSMG, "YusufAmirLuxuryFinish", 0x487AAE09),
            new(WeaponModel.SMG, "DefaultClip", 0x26574997),
            new(WeaponModel.SMG, "ExtendedClip", 0x350966FB),
            new(WeaponModel.SMG, "DrumMagazine", 0x79C77076),
            new(WeaponModel.SMG, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.SMG, "Scope", 0x3CC6BA57),
            new(WeaponModel.SMG, "Suppressor", 0xC304849A),
            new(WeaponModel.SMG, "YusufAmirLuxuryFinish", 0x27872C90),
            new(WeaponModel.AssaultSMG, "DefaultClip", 0x8D1307B0),
            new(WeaponModel.AssaultSMG, "ExtendedClip", 0xBB46E417),
            new(WeaponModel.AssaultSMG, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.AssaultSMG, "Scope", 0x9D2FBF29),
            new(WeaponModel.AssaultSMG, "Suppressor", 0xA73D4664),
            new(WeaponModel.AssaultSMG, "YusufAmirLuxuryFinish", 0x278C78AF),
            new(WeaponModel.MiniSMG, "DefaultClip", 0x84C8B2D3),
            new(WeaponModel.MiniSMG, "ExtendedClip", 0x937ED0B7),
            new(WeaponModel.SMGMkII, "DefaultClip", 0x4C24806E),
            new(WeaponModel.SMGMkII, "ExtendedClip", 0xB9835B2E),
            new(WeaponModel.SMGMkII, "TracerRounds", 0x7FEA36EC),
            new(WeaponModel.SMGMkII, "IncendiaryRounds", 0xD99222E5),
            new(WeaponModel.SMGMkII, "HollowPointRounds", 0x3A1BD6FA),
            new(WeaponModel.SMGMkII, "FullMetalJacketRounds", 0xB5A715F),
            new(WeaponModel.SMGMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.SMGMkII, "HolographicSight", 0x9FDB5652),
            new(WeaponModel.SMGMkII, "SmallScope", 0xE502AB6B),
            new(WeaponModel.SMGMkII, "MediumScope", 0x3DECC7DA),
            new(WeaponModel.SMGMkII, "Suppressor", 0xC304849A),
            new(WeaponModel.SMGMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.SMGMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.SMGMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.SMGMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.SMGMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.SMGMkII, "SlantedMuzzle Brake", 0x347EF8AC),
            new(WeaponModel.SMGMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.SMGMkII, "DefaultBarrel", 0xD9103EE1),
            new(WeaponModel.SMGMkII, "HeavyBarrel", 0xA564D78B),
            new(WeaponModel.SMGMkII, "DigitalCamo", 0xC4979067),
            new(WeaponModel.SMGMkII, "BrushstrokeCamo", 0x3815A945),
            new(WeaponModel.SMGMkII, "WoodlandCamo", 0x4B4B4FB0),
            new(WeaponModel.SMGMkII, "Skull", 0xEC729200),
            new(WeaponModel.SMGMkII, "SessantaNove", 0x48F64B22),
            new(WeaponModel.SMGMkII, "Perseus", 0x35992468),
            new(WeaponModel.SMGMkII, "Leopard", 0x24B782A5),
            new(WeaponModel.SMGMkII, "Zebra", 0xA2E67F01),
            new(WeaponModel.SMGMkII, "Geometric", 0x2218FD68),
            new(WeaponModel.SMGMkII, "Boom", 0x45C5C3C5),
            new(WeaponModel.SMGMkII, "Patriotic", 0x399D558F),
            new(WeaponModel.MachinePistol, "DefaultClip", 0x476E85FF),
            new(WeaponModel.MachinePistol, "ExtendedClip", 0xB92C6979),
            new(WeaponModel.MachinePistol, "DrumMagazine", 0xA9E9CAF4),
            new(WeaponModel.MachinePistol, "Suppressor", 0xC304849A),
            new(WeaponModel.CombatPDW, "DefaultClip", 0x4317F19E),
            new(WeaponModel.CombatPDW, "ExtendedClip", 0x334A5203),
            new(WeaponModel.CombatPDW, "DrumMagazine", 0x6EB8C8DB),
            new(WeaponModel.CombatPDW, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.CombatPDW, "Grip", 0xC164F53),
            new(WeaponModel.CombatPDW, "Scope", 0xAA2C45B4),
            new(WeaponModel.PumpShotgun, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.PumpShotgun, "Suppressor", 0xE608B35E),
            new(WeaponModel.PumpShotgun, "YusufAmirLuxuryFinish", 0xA2D79DDB),
            new(WeaponModel.SawedOffShotgun, "GildedGunMetalFinish", 0x85A64DF9),
            new(WeaponModel.AssaultShotgun, "DefaultClip", 0x94E81BC7),
            new(WeaponModel.AssaultShotgun, "ExtendedClip", 0x86BD7F72),
            new(WeaponModel.AssaultShotgun, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.AssaultShotgun, "Suppressor", 0x837445AA),
            new(WeaponModel.AssaultShotgun, "Grip", 0xC164F53),
            new(WeaponModel.BullpupShotgun, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.BullpupShotgun, "Suppressor", 0xA73D4664),
            new(WeaponModel.BullpupShotgun, "Grip", 0xC164F53),
            new(WeaponModel.PumpShotgunMkII, "DefaultShells", 0xCD940141),
            new(WeaponModel.PumpShotgunMkII, "DragonsBreathShells", 0x9F8A1BF5),
            new(WeaponModel.PumpShotgunMkII, "SteelBuckshotShells", 0x4E65B425),
            new(WeaponModel.PumpShotgunMkII, "FlechetteShells", 0xE9582927),
            new(WeaponModel.PumpShotgunMkII, "ExplosiveSlugs", 0x3BE4465D),
            new(WeaponModel.PumpShotgunMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.PumpShotgunMkII, "SmallScope", 0x49B2945),
            new(WeaponModel.PumpShotgunMkII, "MediumScope", 0x3F3C8181),
            new(WeaponModel.PumpShotgunMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.PumpShotgunMkII, "Suppressor", 0xAC42DF71),
            new(WeaponModel.PumpShotgunMkII, "SquaredMuzzleBrake", 0x5F7DCE4D),
            new(WeaponModel.PumpShotgunMkII, "DigitalCamo", 0xE3BD9E44),
            new(WeaponModel.PumpShotgunMkII, "BrushstrokeCamo", 0x17148F9B),
            new(WeaponModel.PumpShotgunMkII, "WoodlandCamo", 0x24D22B16),
            new(WeaponModel.PumpShotgunMkII, "Skull", 0xF2BEC6F0),
            new(WeaponModel.PumpShotgunMkII, "SessantaNove", 0x85627D),
            new(WeaponModel.PumpShotgunMkII, "Perseus", 0xDC2919C5),
            new(WeaponModel.PumpShotgunMkII, "Leopard", 0xE184247B),
            new(WeaponModel.PumpShotgunMkII, "Zebra", 0xD8EF9356),
            new(WeaponModel.PumpShotgunMkII, "Geometric", 0xEF29BFCA),
            new(WeaponModel.PumpShotgunMkII, "Boom", 0x67AEB165),
            new(WeaponModel.PumpShotgunMkII, "Patriotic", 0x46411A1D),
            new(WeaponModel.HeavyShotgun, "DefaultClip", 0x324F2D5F),
            new(WeaponModel.HeavyShotgun, "ExtendedClip", 0x971CF6FD),
            new(WeaponModel.HeavyShotgun, "DrumMagazine", 0x88C7DA53),
            new(WeaponModel.HeavyShotgun, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.HeavyShotgun, "Suppressor", 0xA73D4664),
            new(WeaponModel.HeavyShotgun, "Grip", 0xC164F53),
            new(WeaponModel.AssaultRifle, "DefaultClip", 0xBE5EEA16),
            new(WeaponModel.AssaultRifle, "ExtendedClip", 0xB1214F9B),
            new(WeaponModel.AssaultRifle, "DrumMagazine", 0xDBF0A53D),
            new(WeaponModel.AssaultRifle, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.AssaultRifle, "Scope", 0x9D2FBF29),
            new(WeaponModel.AssaultRifle, "Suppressor", 0xA73D4664),
            new(WeaponModel.AssaultRifle, "Grip", 0xC164F53),
            new(WeaponModel.AssaultRifle, "YusufAmirLuxuryFinish", 0x4EAD7533),
            new(WeaponModel.CarbineRifle, "DefaultClip", 0x9FBE33EC),
            new(WeaponModel.CarbineRifle, "ExtendedClip", 0x91109691),
            new(WeaponModel.CarbineRifle, "BoxMagazine", 0xBA62E935),
            new(WeaponModel.CarbineRifle, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.CarbineRifle, "Scope", 0xA0D89C42),
            new(WeaponModel.CarbineRifle, "Suppressor", 0x837445AA),
            new(WeaponModel.CarbineRifle, "Grip", 0xC164F53),
            new(WeaponModel.CarbineRifle, "YusufAmirLuxuryFinish", 0xD89B9658),
            new(WeaponModel.AdvancedRifle, "DefaultClip", 0xFA8FA10F),
            new(WeaponModel.AdvancedRifle, "ExtendedClip", 0x8EC1C979),
            new(WeaponModel.AdvancedRifle, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.AdvancedRifle, "Scope", 0xAA2C45B4),
            new(WeaponModel.AdvancedRifle, "Suppressor", 0x837445AA),
            new(WeaponModel.AdvancedRifle, "GildedGunMetalFinish", 0x377CD377),
            new(WeaponModel.SpecialCarbine, "DefaultClip", 0xC6C7E581),
            new(WeaponModel.SpecialCarbine, "ExtendedClip", 0x7C8BD10E),
            new(WeaponModel.SpecialCarbine, "DrumMagazine", 0x6B59AEAA),
            new(WeaponModel.SpecialCarbine, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.SpecialCarbine, "Scope", 0xA0D89C42),
            new(WeaponModel.SpecialCarbine, "Suppressor", 0xA73D4664),
            new(WeaponModel.SpecialCarbine, "Grip", 0xC164F53),
            new(WeaponModel.SpecialCarbine, "EtchedGunMetalFinish", 0x730154F2),
            new(WeaponModel.BullpupRifle, "DefaultClip", 0xC5A12F80),
            new(WeaponModel.BullpupRifle, "ExtendedClip", 0xB3688B0F),
            new(WeaponModel.BullpupRifle, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.BullpupRifle, "Scope", 0xAA2C45B4),
            new(WeaponModel.BullpupRifle, "Suppressor", 0x837445AA),
            new(WeaponModel.BullpupRifle, "Grip", 0xC164F53),
            new(WeaponModel.BullpupRifle, "GildedGunMetalFinish", 0xA857BC78),
            new(WeaponModel.BullpupRifleMkII, "DefaultClip", 0x18929DA),
            new(WeaponModel.BullpupRifleMkII, "ExtendedClip", 0xEFB00628),
            new(WeaponModel.BullpupRifleMkII, "TracerRounds", 0x822060A9),
            new(WeaponModel.BullpupRifleMkII, "IncendiaryRounds", 0xA99CF95A),
            new(WeaponModel.BullpupRifleMkII, "ArmorPiercingRounds", 0xFAA7F5ED),
            new(WeaponModel.BullpupRifleMkII, "FullMetalJacketRounds", 0x43621710),
            new(WeaponModel.BullpupRifleMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.BullpupRifleMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.BullpupRifleMkII, "SmallScope", 0xC7ADD105),
            new(WeaponModel.BullpupRifleMkII, "MediumScope", 0x3F3C8181),
            new(WeaponModel.BullpupRifleMkII, "DefaultBarrel", 0x659AC11B),
            new(WeaponModel.BullpupRifleMkII, "HeavyBarrel", 0x3BF26DC7),
            new(WeaponModel.BullpupRifleMkII, "Suppressor", 0x837445AA),
            new(WeaponModel.BullpupRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.BullpupRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.BullpupRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.BullpupRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.BullpupRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.BullpupRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.BullpupRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.BullpupRifleMkII, "Grip", 0x9D65907A),
            new(WeaponModel.BullpupRifleMkII, "DigitalCamo", 0xAE4055B7),
            new(WeaponModel.BullpupRifleMkII, "BrushstrokeCamo", 0xB905ED6B),
            new(WeaponModel.BullpupRifleMkII, "WoodlandCamo", 0xA6C448E8),
            new(WeaponModel.BullpupRifleMkII, "Skull", 0x9486246C),
            new(WeaponModel.BullpupRifleMkII, "SessantaNove", 0x8A390FD2),
            new(WeaponModel.BullpupRifleMkII, "Perseus", 0x2337FC5),
            new(WeaponModel.BullpupRifleMkII, "Leopard", 0xEFFFDB5E),
            new(WeaponModel.BullpupRifleMkII, "Zebra", 0xDDBDB6DA),
            new(WeaponModel.BullpupRifleMkII, "Geometric", 0xCB631225),
            new(WeaponModel.BullpupRifleMkII, "Boom", 0xA87D541E),
            new(WeaponModel.BullpupRifleMkII, "Patriotic", 0xC5E9AE52),
            new(WeaponModel.SpecialCarbineMkII, "DefaultClip", 0x16C69281),
            new(WeaponModel.SpecialCarbineMkII, "ExtendedClip", 0xDE1FA12C),
            new(WeaponModel.SpecialCarbineMkII, "TracerRounds", 0x8765C68A),
            new(WeaponModel.SpecialCarbineMkII, "IncendiaryRounds", 0xDE011286),
            new(WeaponModel.SpecialCarbineMkII, "ArmorPiercingRounds", 0x51351635),
            new(WeaponModel.SpecialCarbineMkII, "FullMetalJacketRounds", 0x503DEA90),
            new(WeaponModel.SpecialCarbineMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.SpecialCarbineMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.SpecialCarbineMkII, "SmallScope", 0x49B2945),
            new(WeaponModel.SpecialCarbineMkII, "LargeScope", 0xC66B6542),
            new(WeaponModel.SpecialCarbineMkII, "Suppressor", 0xA73D4664),
            new(WeaponModel.SpecialCarbineMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.SpecialCarbineMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.SpecialCarbineMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.SpecialCarbineMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.SpecialCarbineMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.SpecialCarbineMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.SpecialCarbineMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.SpecialCarbineMkII, "Grip", 0x9D65907A),
            new(WeaponModel.SpecialCarbineMkII, "DefaultBarrel", 0xE73653A9),
            new(WeaponModel.SpecialCarbineMkII, "HeavyBarrel", 0xF97F783B),
            new(WeaponModel.SpecialCarbineMkII, "DigitalCamo", 0xF97F783B),
            new(WeaponModel.SpecialCarbineMkII, "BrushstrokeCamo", 0x431B238B),
            new(WeaponModel.SpecialCarbineMkII, "WoodlandCamo", 0x34CF86F4),
            new(WeaponModel.SpecialCarbineMkII, "Skull", 0xB4C306DD),
            new(WeaponModel.SpecialCarbineMkII, "SessantaNove", 0xEE677A25),
            new(WeaponModel.SpecialCarbineMkII, "Perseus", 0xDF90DC78),
            new(WeaponModel.SpecialCarbineMkII, "Leopard", 0xA4C31EE),
            new(WeaponModel.SpecialCarbineMkII, "Zebra", 0x89CFB0F7),
            new(WeaponModel.SpecialCarbineMkII, "Geometric", 0x7B82145C),
            new(WeaponModel.SpecialCarbineMkII, "Boom", 0x899CAF75),
            new(WeaponModel.SpecialCarbineMkII, "Patriotic", 0x5218C819),
            new(WeaponModel.AssaultRifleMkII, "DefaultClip", 0x8610343F),
            new(WeaponModel.AssaultRifleMkII, "ExtendedClip", 0xD12ACA6F),
            new(WeaponModel.AssaultRifleMkII, "TracerRounds", 0xEF2C78C1),
            new(WeaponModel.AssaultRifleMkII, "IncendiaryRounds", 0xFB70D853),
            new(WeaponModel.AssaultRifleMkII, "ArmorPiercingRounds", 0xA7DD1E58),
            new(WeaponModel.AssaultRifleMkII, "FullMetalJacketRounds", 0x63E0A098),
            new(WeaponModel.AssaultRifleMkII, "Grip", 0x9D65907A),
            new(WeaponModel.AssaultRifleMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.AssaultRifleMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.AssaultRifleMkII, "SmallScope", 0x49B2945),
            new(WeaponModel.AssaultRifleMkII, "LargeScope", 0xC66B6542),
            new(WeaponModel.AssaultRifleMkII, "Suppressor", 0xA73D4664),
            new(WeaponModel.AssaultRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.AssaultRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.AssaultRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.AssaultRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.AssaultRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.AssaultRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.AssaultRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.AssaultRifleMkII, "DefaultBarrel", 0x43A49D26),
            new(WeaponModel.AssaultRifleMkII, "HeavyBarrel", 0x5646C26A),
            new(WeaponModel.AssaultRifleMkII, "DigitalCamo", 0x911B24AF),
            new(WeaponModel.AssaultRifleMkII, "BrushstrokeCamo", 0x37E5444B),
            new(WeaponModel.AssaultRifleMkII, "WoodlandCamo", 0x538B7B97),
            new(WeaponModel.AssaultRifleMkII, "Skull", 0x25789F72),
            new(WeaponModel.AssaultRifleMkII, "SessantaNove", 0xC5495F2D),
            new(WeaponModel.AssaultRifleMkII, "Perseus", 0xCF8B73B1),
            new(WeaponModel.AssaultRifleMkII, "Leopard", 0xA9BB2811),
            new(WeaponModel.AssaultRifleMkII, "Zebra", 0xFC674D54),
            new(WeaponModel.AssaultRifleMkII, "Geometric", 0x7C7FCD9B),
            new(WeaponModel.AssaultRifleMkII, "Boom", 0xA5C38392),
            new(WeaponModel.AssaultRifleMkII, "Patriotic", 0xB9B15DB0),
            new(WeaponModel.CarbineRifleMkII, "DefaultClip", 0x4C7A391E),
            new(WeaponModel.CarbineRifleMkII, "ExtendedClip", 0x5DD5DBD5),
            new(WeaponModel.CarbineRifleMkII, "TracerRounds", 0x1757F566),
            new(WeaponModel.CarbineRifleMkII, "IncendiaryRounds", 0x3D25C2A7),
            new(WeaponModel.CarbineRifleMkII, "ArmorPiercingRounds", 0x255D5D57),
            new(WeaponModel.CarbineRifleMkII, "FullMetalJacketRounds", 0x44032F11),
            new(WeaponModel.CarbineRifleMkII, "Grip", 0x9D65907A),
            new(WeaponModel.CarbineRifleMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.CarbineRifleMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.CarbineRifleMkII, "SmallScope", 0x49B2945),
            new(WeaponModel.CarbineRifleMkII, "LargeScope", 0xC66B6542),
            new(WeaponModel.CarbineRifleMkII, "Suppressor", 0x837445AA),
            new(WeaponModel.CarbineRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.CarbineRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.CarbineRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.CarbineRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.CarbineRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.CarbineRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.CarbineRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.CarbineRifleMkII, "DefaultBarrel", 0x833637FF),
            new(WeaponModel.CarbineRifleMkII, "HeavyBarrel", 0x8B3C480B),
            new(WeaponModel.CarbineRifleMkII, "DigitalCamo", 0x4BDD6F16),
            new(WeaponModel.CarbineRifleMkII, "BrushstrokeCamo", 0x406A7908),
            new(WeaponModel.CarbineRifleMkII, "WoodlandCamo", 0x2F3856A4),
            new(WeaponModel.CarbineRifleMkII, "Skull", 0xE50C424D),
            new(WeaponModel.CarbineRifleMkII, "SessantaNove", 0xD37D1F2F),
            new(WeaponModel.CarbineRifleMkII, "Perseus", 0x86268483),
            new(WeaponModel.CarbineRifleMkII, "Leopard", 0xF420E076),
            new(WeaponModel.CarbineRifleMkII, "Zebra", 0xAAE14DF8),
            new(WeaponModel.CarbineRifleMkII, "Geometric", 0x9893A95D),
            new(WeaponModel.CarbineRifleMkII, "Boom", 0x6B13CD3E),
            new(WeaponModel.CarbineRifleMkII, "Patriotic", 0xDA55CD3F),
            new(WeaponModel.CompactRifle, "DefaultClip", 0x513F0A63),
            new(WeaponModel.CompactRifle, "ExtendedClip", 0x59FF9BF8),
            new(WeaponModel.CompactRifle, "DrumMagazine", 0xC607740E),
            new(WeaponModel.MG, "DefaultClip", 0xF434EF84),
            new(WeaponModel.MG, "ExtendedClip", 0x82158B47),
            new(WeaponModel.MG, "Scope", 0x3C00AFED),
            new(WeaponModel.MG, "YusufAmirLuxuryFinish", 0xD6DABABE),
            new(WeaponModel.CombatMG, "DefaultClip", 0xE1FFB34A),
            new(WeaponModel.CombatMG, "ExtendedClip", 0xD6C59CD6),
            new(WeaponModel.CombatMG, "Scope", 0xA0D89C42),
            new(WeaponModel.CombatMG, "Grip", 0xC164F53),
            new(WeaponModel.CombatMG, "EtchedGunMetalFinish", 0x92FECCDD),
            new(WeaponModel.CombatMGMkII, "DefaultClip", 0x492B257C),
            new(WeaponModel.CombatMGMkII, "ExtendedClip", 0x17DF42E9),
            new(WeaponModel.CombatMGMkII, "TracerRounds", 0xF6649745),
            new(WeaponModel.CombatMGMkII, "IncendiaryRounds", 0xC326BDBA),
            new(WeaponModel.CombatMGMkII, "ArmorPiercingRounds", 0x29882423),
            new(WeaponModel.CombatMGMkII, "FullMetalJacketRounds", 0x57EF1CC8),
            new(WeaponModel.CombatMGMkII, "Grip", 0x9D65907A),
            new(WeaponModel.CombatMGMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.CombatMGMkII, "MediumScope", 0x3F3C8181),
            new(WeaponModel.CombatMGMkII, "LargeScope", 0xC66B6542),
            new(WeaponModel.CombatMGMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.CombatMGMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.CombatMGMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.CombatMGMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.CombatMGMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.CombatMGMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.CombatMGMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.CombatMGMkII, "DefaultBarrel", 0xC34EF234),
            new(WeaponModel.CombatMGMkII, "HeavyBarrel", 0xB5E2575B),
            new(WeaponModel.CombatMGMkII, "DigitalCamo", 0x4A768CB5),
            new(WeaponModel.CombatMGMkII, "BrushstrokeCamo", 0xCCE06BBD),
            new(WeaponModel.CombatMGMkII, "WoodlandCamo", 0xBE94CF26),
            new(WeaponModel.CombatMGMkII, "Skull", 0x7609BE11),
            new(WeaponModel.CombatMGMkII, "SessantaNove", 0x48AF6351),
            new(WeaponModel.CombatMGMkII, "Perseus", 0x9186750A),
            new(WeaponModel.CombatMGMkII, "Leopard", 0x84555AA8),
            new(WeaponModel.CombatMGMkII, "Zebra", 0x1B4C088B),
            new(WeaponModel.CombatMGMkII, "Geometric", 0xE046DFC),
            new(WeaponModel.CombatMGMkII, "Boom", 0x28B536E),
            new(WeaponModel.CombatMGMkII, "Patriotic", 0xD703C94D),
            new(WeaponModel.GusenbergSweeper, "DefaultClip", 0x1CE5A6A5),
            new(WeaponModel.GusenbergSweeper, "ExtendedClip", 0xEAC8C270),
            new(WeaponModel.SniperRifle, "DefaultClip", 0x9BC64089),
            new(WeaponModel.SniperRifle, "Suppressor", 0xA73D4664),
            new(WeaponModel.SniperRifle, "Scope", 0xD2443DDC),
            new(WeaponModel.SniperRifle, "AdvancedScope", 0xBC54DA77),
            new(WeaponModel.SniperRifle, "EtchedWoodGripFinish", 0x4032B5E7),
            new(WeaponModel.HeavySniper, "DefaultClip", 0x476F52F4),
            new(WeaponModel.HeavySniper, "Scope", 0xD2443DDC),
            new(WeaponModel.HeavySniper, "AdvancedScope", 0xBC54DA77),
            new(WeaponModel.MarksmanRifleMkII, "DefaultClip", 0x94E12DCE),
            new(WeaponModel.MarksmanRifleMkII, "ExtendedClip", 0xE6CFD1AA),
            new(WeaponModel.MarksmanRifleMkII, "TracerRounds", 0xD77A22D2),
            new(WeaponModel.MarksmanRifleMkII, "IncendiaryRounds", 0x6DD7A86E),
            new(WeaponModel.MarksmanRifleMkII, "ArmorPiercingRounds", 0xF46FD079),
            new(WeaponModel.MarksmanRifleMkII, "FullMetalJacketRounds", 0xE14A9ED3),
            new(WeaponModel.MarksmanRifleMkII, "HolographicSight", 0x420FD713),
            new(WeaponModel.MarksmanRifleMkII, "LargeScope", 0xC66B6542),
            new(WeaponModel.MarksmanRifleMkII, "ZoomScope", 0x5B1C713C),
            new(WeaponModel.MarksmanRifleMkII, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.MarksmanRifleMkII, "Suppressor", 0x837445AA),
            new(WeaponModel.MarksmanRifleMkII, "FlatMuzzleBrake", 0xB99402D4),
            new(WeaponModel.MarksmanRifleMkII, "TacticalMuzzleBrake", 0xC867A07B),
            new(WeaponModel.MarksmanRifleMkII, "FatEndMuzzleBrake", 0xDE11CBCF),
            new(WeaponModel.MarksmanRifleMkII, "PrecisionMuzzleBrake", 0xEC9068CC),
            new(WeaponModel.MarksmanRifleMkII, "HeavyDutyMuzzleBrake", 0x2E7957A),
            new(WeaponModel.MarksmanRifleMkII, "SlantedMuzzleBrake", 0x347EF8AC),
            new(WeaponModel.MarksmanRifleMkII, "SplitEndMuzzleBrake", 0x4DB62ABE),
            new(WeaponModel.MarksmanRifleMkII, "DefaultBarrel", 0x381B5D89),
            new(WeaponModel.MarksmanRifleMkII, "HeavyBarrel", 0x68373DDC),
            new(WeaponModel.MarksmanRifleMkII, "Grip", 0x9D65907A),
            new(WeaponModel.MarksmanRifleMkII, "DigitalCamo", 0x9094FBA0),
            new(WeaponModel.MarksmanRifleMkII, "BrushstrokeCamo", 0x7320F4B2),
            new(WeaponModel.MarksmanRifleMkII, "WoodlandCamo", 0x60CF500F),
            new(WeaponModel.MarksmanRifleMkII, "Skull", 0xFE668B3F),
            new(WeaponModel.MarksmanRifleMkII, "SessantaNove", 0xF3757559),
            new(WeaponModel.MarksmanRifleMkII, "Perseus", 0x193B40E8),
            new(WeaponModel.MarksmanRifleMkII, "Leopard", 0x107D2F6C),
            new(WeaponModel.MarksmanRifleMkII, "Zebra", 0xC4E91841),
            new(WeaponModel.MarksmanRifleMkII, "Geometric", 0x9BB1C5D3),
            new(WeaponModel.MarksmanRifleMkII, "Boom", 0x3B61040B),
            new(WeaponModel.MarksmanRifleMkII, "Patriotic", 0xB7A316DA),
            new(WeaponModel.HeavySniperMkII, "DefaultClip", 0xFA1E1A28),
            new(WeaponModel.HeavySniperMkII, "ExtendedClip", 0x2CD8FF9D),
            new(WeaponModel.HeavySniperMkII, "IncendiaryRounds", 0xEC0F617),
            new(WeaponModel.HeavySniperMkII, "ArmorPiercingRounds", 0xF835D6D4),
            new(WeaponModel.HeavySniperMkII, "FullMetalJacketRounds", 0x3BE948F6),
            new(WeaponModel.HeavySniperMkII, "ExplosiveRounds", 0x89EBDAA7),
            new(WeaponModel.HeavySniperMkII, "ZoomScope", 0x82C10383),
            new(WeaponModel.HeavySniperMkII, "AdvancedScope", 0xBC54DA77),
            new(WeaponModel.HeavySniperMkII, "NightVisionScope", 0xB68010B0),
            new(WeaponModel.HeavySniperMkII, "ThermalScope", 0x2E43DA41),
            new(WeaponModel.HeavySniperMkII, "Suppressor", 0xAC42DF71),
            new(WeaponModel.HeavySniperMkII, "SquaredMuzzleBrake", 0x5F7DCE4D),
            new(WeaponModel.HeavySniperMkII, "BellEndMuzzleBrake", 0x6927E1A1),
            new(WeaponModel.HeavySniperMkII, "DefaultBarrel", 0x909630B7),
            new(WeaponModel.HeavySniperMkII, "HeavyBarrel", 0x108AB09E),
            new(WeaponModel.HeavySniperMkII, "DigitalCamo", 0xF8337D02),
            new(WeaponModel.HeavySniperMkII, "BrushstrokeCamo", 0xC5BEDD65),
            new(WeaponModel.HeavySniperMkII, "WoodlandCamo", 0xE9712475),
            new(WeaponModel.HeavySniperMkII, "Skull", 0x13AA78E7),
            new(WeaponModel.HeavySniperMkII, "SessantaNove", 0x26591E50),
            new(WeaponModel.HeavySniperMkII, "Perseus", 0x302731EC),
            new(WeaponModel.HeavySniperMkII, "Leopard", 0xAC722A78),
            new(WeaponModel.HeavySniperMkII, "Zebra", 0xBEA4CEDD),
            new(WeaponModel.HeavySniperMkII, "Geometric", 0xCD776C82),
            new(WeaponModel.HeavySniperMkII, "Boom", 0xABC5ACC7),
            new(WeaponModel.HeavySniperMkII, "Patriotic", 0x6C32D2EB),
            new(WeaponModel.MarksmanRifle, "DefaultClip", 0xD83B4141),
            new(WeaponModel.MarksmanRifle, "ExtendedClip", 0xCCFD2AC5),
            new(WeaponModel.MarksmanRifle, "Scope", 0x1C221B1A),
            new(WeaponModel.MarksmanRifle, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.MarksmanRifle, "Suppressor", 0x837445AA),
            new(WeaponModel.MarksmanRifle, "Grip", 0xC164F53),
            new(WeaponModel.MarksmanRifle, "YusufAmirLuxuryFinish", 0x161E9241),
            new(WeaponModel.GrenadeLauncher, "DefaultClip", 0x11AE5C97),
            new(WeaponModel.GrenadeLauncher, "Flashlight", 0x7BC4CDDC),
            new(WeaponModel.GrenadeLauncher, "Grip", 0xC164F53),
            new(WeaponModel.GrenadeLauncher, "Scope", 0xAA2C45B4),
        ];

        public static List<EmergencyCall> EmergencyCalls { get; set; } = [];

        public static DiscordSocketClient? DiscordClient { get; set; }

        public static List<Spotlight> Spotlights { get; set; } = [];

        public static IEnumerable<MyPlayer> AllPlayers { get => Alt.GetAllPlayers().Cast<MyPlayer>(); }

        public static IEnumerable<MyPlayer> SpawnedPlayers { get => Alt.GetAllPlayers().Cast<MyPlayer>().Where(x => x.Character?.PersonalizationStep == CharacterPersonalizationStep.Ready); }

        public static IEnumerable<MyVehicle> Vehicles { get => Alt.GetAllVehicles().Cast<MyVehicle>(); }

        public static List<FactionUnit> FactionsUnits { get; set; } = [];

        public static List<Item> Items { get; set; } = [];

        public static List<ClotheAccessory> Clothes1Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes1Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes3Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes3Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes4Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes4Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes5Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes5Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes6Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes6Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes7Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes7Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes8Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes8Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes9Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes9Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes10Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes10Female { get; set; } = [];

        public static List<ClotheAccessory> Clothes11Male { get; set; } = [];

        public static List<ClotheAccessory> Clothes11Female { get; set; } = [];

        public static List<ClotheAccessory> Accessories0Male { get; set; } = [];

        public static List<ClotheAccessory> Accessories0Female { get; set; } = [];

        public static List<ClotheAccessory> Accessories1Male { get; set; } = [];

        public static List<ClotheAccessory> Accessories1Female { get; set; } = [];

        public static List<ClotheAccessory> Accessories2Male { get; set; } = [];

        public static List<ClotheAccessory> Accessories2Female { get; set; } = [];

        public static List<ClotheAccessory> Accessories6Male { get; set; } = [];

        public static List<ClotheAccessory> Accessories6Female { get; set; } = [];

        public static List<ClotheAccessory> Accessories7Male { get; set; } = [];

        public static List<ClotheAccessory> Accessories7Female { get; set; } = [];

        public static WeatherInfo WeatherInfo { get; set; }

        public static List<Door> Doors { get; set; } = [];

        public static List<string> IPLs { get; set; } = [];

        public static List<Info> Infos { get; set; } = [];

        public static List<CrackDen> CrackDens { get; set; } = [];

        public static List<CrackDenItem> CrackDensItems { get; set; } = [];

        public static List<TruckerLocation> TruckerLocations { get; set; } = [];

        public static List<TruckerLocationDelivery> TruckerLocationsDeliveries { get; set; } = [];

        public static List<AudioSpot> AudioSpots { get; set; } = [];

        public static List<Furniture> Furnitures { get; set; } = [];

        public static List<Animation> Animations { get; set; } = [];

        public static List<Company> Companies { get; set; } = [];

        public static List<Tuple<string, string>> Scenarios { get; set; } =
        [
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
        ];

        public static List<MethodInfo> Commands { get; set; } = [];

        public static string DbConnectionString { get; set; } = "Server=localhost;Database=bdsegundavidaroleplay;Uid=root;Password=159357";

        public static ulong AnnouncementDiscordChannel { get; set; }

        public static ulong GovernmentAnnouncementDiscordChannel { get; set; }

        public static ulong StaffDiscordChannel { get; set; }

        public static List<ulong> RolesStaffMessage { get; set; } = [];

        public static ulong CompanyAnnouncementDiscordChannel { get; set; }

        public static IEnumerable<MyObject> Objects => Alt.GetAllNetworkObjects().Cast<MyObject>();

        public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true,
        };

        public const string RECORD_NOT_FOUND = "Registro não encontrado.";

        public static IEnumerable<MyColShape> ColShapes => Alt.GetAllColShapes().Cast<MyColShape>();

        public static IEnumerable<MyMarker> Markers => Alt.GetAllMarkers().Cast<MyMarker>();

        public static IEnumerable<MyBlip> MyBlips => Alt.GetAllMarkers().Cast<MyBlip>();

        public const int DEFAULT_SAVINGS = 50_000;

        public const int MAX_SAVINGS = 1_000_000;
    }
}