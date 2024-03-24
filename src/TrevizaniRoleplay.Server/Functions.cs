using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server
{
    public static class Functions
    {
        public static Position GetExitPositionByInterior(PropertyInterior propertyInterior)
        {
            return propertyInterior switch
            {
                PropertyInterior.Motel => new Position(151.2564f, -1007.868f, -98.99999f),
                PropertyInterior.CasaBaixa => new Position(265.9522f, -1007.485f, -101.0085f),
                PropertyInterior.CasaMedia => new Position(346.4499f, -1012.996f, -99.19622f),
                PropertyInterior.IntegrityWay28 => new Position(-31.34092f, -594.9429f, 80.0309f),
                PropertyInterior.IntegrityWay30 => new Position(-17.61359f, -589.3938f, 90.11487f),
                PropertyInterior.DellPerroHeights4 => new Position(-1452.225f, -540.4642f, 74.04436f),
                PropertyInterior.DellPerroHeights7 => new Position(-1451.26f, -523.9634f, 56.92898f),
                PropertyInterior.RichardMajestic2 => new Position(-912.6351f, -364.9724f, 114.2748f),
                PropertyInterior.TinselTowers42 => new Position(-603.1113f, 58.93406f, 98.20017f),
                PropertyInterior.EclipseTowers3 => new Position(-785.1537f, 323.8156f, 211.9973f),
                PropertyInterior.WildOatsDrive3655 => new Position(-174.3753f, 497.3086f, 137.6669f),
                PropertyInterior.NorthConkerAvenue2044 => new Position(341.9306f, 437.7751f, 149.3901f),
                PropertyInterior.NorthConkerAvenue2045 => new Position(373.5803f, 423.7043f, 145.9078f),
                PropertyInterior.HillcrestAvenue2862 => new Position(-682.3693f, 592.2678f, 145.393f),
                PropertyInterior.HillcrestAvenue2868 => new Position(-758.4348f, 618.8454f, 144.1539f),
                PropertyInterior.HillcrestAvenue2874 => new Position(-859.7643f, 690.8358f, 152.8607f),
                PropertyInterior.WhispymoundDrive2677 => new Position(117.209f, 559.8086f, 184.3048f),
                PropertyInterior.MadWayneThunder2133 => new Position(-1289.775f, 449.3125f, 97.90256f),
                PropertyInterior.Modern1Apartment => new Position(-786.8663f, 315.7642f, 217.6385f),
                PropertyInterior.Modern2Apartment => new Position(-786.9563f, 315.6229f, 187.9136f),
                PropertyInterior.Modern3Apartment => new Position(-774.0126f, 342.0428f, 196.6864f),
                PropertyInterior.Mody1Apartment => new Position(-787.0749f, 315.8198f, 217.6386f),
                PropertyInterior.Mody2Apartment => new Position(-786.8195f, 315.5634f, 187.9137f),
                PropertyInterior.Mody3Apartment => new Position(-774.1382f, 342.0316f, 196.6864f),
                PropertyInterior.Vibrant1Apartment => new Position(-786.6245f, 315.6175f, 217.6385f),
                PropertyInterior.Vibrant2Apartment => new Position(-786.9584f, 315.7974f, 187.9135f),
                PropertyInterior.Vibrant3Apartment => new Position(-774.0223f, 342.1718f, 196.6863f),
                PropertyInterior.Sharp1Apartment => new Position(-787.0902f, 315.7039f, 217.6384f),
                PropertyInterior.Sharp2Apartment => new Position(-787.0155f, 315.7071f, 187.9135f),
                PropertyInterior.Sharp3Apartment => new Position(-773.8976f, 342.1525f, 196.6863f),
                PropertyInterior.Monochrome1Apartment => new Position(-786.9887f, 315.7393f, 217.6386f),
                PropertyInterior.Monochrome2Apartment => new Position(-786.8809f, 315.6634f, 187.9136f),
                PropertyInterior.Monochrome3Apartment => new Position(-774.0675f, 342.0773f, 196.6864f),
                PropertyInterior.Seductive1Apartment => new Position(-787.1423f, 315.6943f, 217.6384f),
                PropertyInterior.Seductive2Apartment => new Position(-787.0961f, 315.815f, 187.9135f),
                PropertyInterior.Seductive3Apartment => new Position(-773.9552f, 341.9892f, 196.6862f),
                PropertyInterior.Regal1Apartment => new Position(-787.029f, 315.7113f, 217.6385f),
                PropertyInterior.Regal2Apartment => new Position(-787.0574f, 315.6567f, 187.9135f),
                PropertyInterior.Regal3Apartment => new Position(-774.0109f, 342.0965f, 196.6863f),
                PropertyInterior.Aqua1Apartment => new Position(-786.9469f, 315.5655f, 217.6383f),
                PropertyInterior.Aqua2Apartment => new Position(-786.9756f, 315.723f, 187.9134f),
                PropertyInterior.Aqua3Apartment => new Position(-774.0349f, 342.0296f, 196.6862f),
                PropertyInterior.ArcadiusExecutiveRich => new Position(-141.1987f, -620.913f, 168.8205f),
                PropertyInterior.ArcadiusExecutiveCool => new Position(-141.5429f, -620.9524f, 168.8204f),
                PropertyInterior.ArcadiusExecutiveContrast => new Position(-141.2896f, -620.9618f, 168.8204f),
                PropertyInterior.ArcadiusOldSpiceWarm => new Position(-141.4966f, -620.8292f, 168.8204f),
                PropertyInterior.ArcadiusOldSpiceClassical => new Position(-141.3997f, -620.9006f, 168.8204f),
                PropertyInterior.ArcadiusOldSpiceVintage => new Position(-141.5361f, -620.9186f, 168.8204f),
                PropertyInterior.ArcadiusPowerBrokerIce => new Position(-141.392f, -621.0451f, 168.8204f),
                PropertyInterior.ArcadiusPowerBrokeConservative => new Position(-141.1945f, -620.8729f, 168.8204f),
                PropertyInterior.ArcadiusPowerBrokePolished => new Position(-141.4924f, -621.0035f, 168.8205f),
                PropertyInterior.MazeBankExecutiveRich => new Position(-75.8466f, -826.9893f, 243.3859f),
                PropertyInterior.MazeBankExecutiveCool => new Position(-75.49945f, -827.05f, 243.386f),
                PropertyInterior.MazeBankExecutiveContrast => new Position(-75.49827f, -827.1889f, 243.386f),
                PropertyInterior.MazeBankOldSpiceWarm => new Position(-75.44054f, -827.1487f, 243.3859f),
                PropertyInterior.MazeBankOldSpiceClassical => new Position(-75.63942f, -827.1022f, 243.3859f),
                PropertyInterior.MazeBankOldSpiceVintage => new Position(-75.47446f, -827.2621f, 243.386f),
                PropertyInterior.MazeBankPowerBrokerIce => new Position(-75.56978f, -827.1152f, 243.3859f),
                PropertyInterior.MazeBankPowerBrokeConservative => new Position(-75.51953f, -827.0786f, 243.3859f),
                PropertyInterior.MazeBankPowerBrokePolished => new Position(-75.41915f, -827.1118f, 243.3858f),
                PropertyInterior.LomBankExecutiveRich => new Position(-1579.756f, -565.0661f, 108.523f),
                PropertyInterior.LomBankExecutiveCool => new Position(-1579.678f, -565.0034f, 108.5229f),
                PropertyInterior.LomBankExecutiveContrast => new Position(-1579.583f, -565.0399f, 108.5229f),
                PropertyInterior.LomBankOldSpiceWarm => new Position(-1579.702f, -565.0366f, 108.5229f),
                PropertyInterior.LomBankOldSpiceClassical => new Position(-1579.643f, -564.9685f, 108.5229f),
                PropertyInterior.LomBankOldSpiceVintage => new Position(-1579.681f, -565.0003f, 108.523f),
                PropertyInterior.LomBankPowerBrokerIce => new Position(-1579.677f, -565.0689f, 108.5229f),
                PropertyInterior.LomBankPowerBrokeConservative => new Position(-1579.708f, -564.9634f, 108.5229f),
                PropertyInterior.LomBankPowerBrokePolished => new Position(-1579.693f, -564.8981f, 108.5229f),
                PropertyInterior.MazeBankWestExecutiveRich => new Position(-1392.667f, -480.4736f, 72.04217f),
                PropertyInterior.MazeBankWestExecutiveCool => new Position(-1392.542f, -480.4011f, 72.04211f),
                PropertyInterior.MazeBankWestExecutiveContrast => new Position(-1392.626f, -480.4856f, 72.04212f),
                PropertyInterior.MazeBankWestOldSpiceWarm => new Position(-1392.617f, -480.6363f, 72.04208f),
                PropertyInterior.MazeBankWestOldSpiceClassical => new Position(-1392.532f, -480.7649f, 72.04207f),
                PropertyInterior.MazeBankWestOldSpiceVintage => new Position(-1392.611f, -480.5562f, 72.04214f),
                PropertyInterior.MazeBankWestPowerBrokerIce => new Position(-1392.563f, -480.549f, 72.0421f),
                PropertyInterior.MazeBankWestPowerBrokeConservative => new Position(-1392.528f, -480.475f, 72.04206f),
                PropertyInterior.MazeBankWestPowerBrokePolished => new Position(-1392.416f, -480.7485f, 72.04207f),
                PropertyInterior.Clubhouse1 => new Position(1110.145f, -3166.932f, -37.529663f),
                PropertyInterior.Clubhouse2 => new Position(997.2791f, -3164.4395f, -38.911377f),
                PropertyInterior.MethLab => new Position(996.8967f, -3200.6902f, -36.400757f),
                PropertyInterior.WeedFarm => new Position(1066.2594f, -3183.521f, -39.164062f),
                PropertyInterior.CocaineLockup => new Position(1088.6901f, -3187.5562f, -38.995605f),
                PropertyInterior.CounterfeitCashFactory => new Position(1138.1143f, -3199.1472f, -39.669556f),
                PropertyInterior.DocumentForgeryOffice => new Position(1173.7451f, -3196.6682f, -39.01245f),
                PropertyInterior.WarehouseSmall => new Position(1087.4374f, -3099.323f, -39.01245f),
                PropertyInterior.WarehouseMedium => new Position(1048.0352f, -3097.1077f, -39.01245f),
                PropertyInterior.WarehouseLarge => new Position(1027.7803f, -3101.6309f, -39.01245f),
                PropertyInterior.Nightclub => new Position(246.73846f, -1589.4594f, -187.0044f),
                _ => new Position(),
            };
        }

        public static void SendFactionTypeMessage(FactionType factionType, string message, bool onlyOnDuty, bool factionColor)
        {
            var players = Global.SpawnedPlayers.Where(x => x.Faction?.Type == factionType);

            if (onlyOnDuty)
                players = players.Where(x => x.OnDuty);

            foreach (var player in players)
                player.SendMessage(Models.MessageType.None, message, factionColor ? $"#{player.Faction!.Color}" : "#FFFFFF");
        }

        public static async Task<string> GenerateVehiclePlate(bool government)
        {
            var plate = string.Empty;
            await using var context = new DatabaseContext();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            do
            {
                if (government)
                    plate = $"1{random.Next(0, 9999999).ToString().PadLeft(7, '0')}";
                else
                    plate = $"{random.Next(2, 9)}{chars[random.Next(25)]}{chars[random.Next(25)]}{chars[random.Next(25)]}{random.Next(0, 9999).ToString().PadLeft(4, '0')}";
            } while (await context.Vehicles.AnyAsync(x => x.Plate == plate));

            return plate;
        }

        public static List<string> GetIPLsByInterior(PropertyInterior propertyInterior)
        {
            return propertyInterior switch
            {
                PropertyInterior.Modern1Apartment => ["apa_v_mp_h_01_a"],
                PropertyInterior.Modern2Apartment => ["apa_v_mp_h_01_c"],
                PropertyInterior.Modern3Apartment => ["apa_v_mp_h_01_b"],
                PropertyInterior.Mody1Apartment => ["apa_v_mp_h_02_a"],
                PropertyInterior.Mody2Apartment => ["apa_v_mp_h_02_c"],
                PropertyInterior.Mody3Apartment => ["apa_v_mp_h_02_b"],
                PropertyInterior.Vibrant1Apartment => ["apa_v_mp_h_03_a"],
                PropertyInterior.Vibrant2Apartment => ["apa_v_mp_h_03_c"],
                PropertyInterior.Vibrant3Apartment => ["apa_v_mp_h_03_b"],
                PropertyInterior.Sharp1Apartment => ["apa_v_mp_h_04_a"],
                PropertyInterior.Sharp2Apartment => ["apa_v_mp_h_04_c"],
                PropertyInterior.Sharp3Apartment => ["apa_v_mp_h_04_b"],
                PropertyInterior.Monochrome1Apartment => ["apa_v_mp_h_05_a"],
                PropertyInterior.Monochrome2Apartment => ["apa_v_mp_h_05_c"],
                PropertyInterior.Monochrome3Apartment => ["apa_v_mp_h_05_b"],
                PropertyInterior.Seductive1Apartment => ["apa_v_mp_h_06_a"],
                PropertyInterior.Seductive2Apartment => ["apa_v_mp_h_06_c"],
                PropertyInterior.Seductive3Apartment => ["apa_v_mp_h_06_b"],
                PropertyInterior.Regal1Apartment => ["apa_v_mp_h_07_a"],
                PropertyInterior.Regal2Apartment => ["apa_v_mp_h_07_c"],
                PropertyInterior.Regal3Apartment => ["apa_v_mp_h_07_b"],
                PropertyInterior.Aqua1Apartment => ["apa_v_mp_h_08_a"],
                PropertyInterior.Aqua2Apartment => ["apa_v_mp_h_08_c"],
                PropertyInterior.Aqua3Apartment => ["apa_v_mp_h_08_b"],
                PropertyInterior.ArcadiusExecutiveRich => ["ex_dt1_02_office_02b"],
                PropertyInterior.ArcadiusExecutiveCool => ["ex_dt1_02_office_02c"],
                PropertyInterior.ArcadiusExecutiveContrast => ["ex_dt1_02_office_02a"],
                PropertyInterior.ArcadiusOldSpiceWarm => ["ex_dt1_02_office_01a"],
                PropertyInterior.ArcadiusOldSpiceClassical => ["ex_dt1_02_office_01b"],
                PropertyInterior.ArcadiusOldSpiceVintage => ["ex_dt1_02_office_01c"],
                PropertyInterior.ArcadiusPowerBrokerIce => ["ex_dt1_02_office_03a"],
                PropertyInterior.ArcadiusPowerBrokeConservative => ["ex_dt1_02_office_03b"],
                PropertyInterior.ArcadiusPowerBrokePolished => ["ex_dt1_02_office_03c"],
                PropertyInterior.MazeBankExecutiveRich => ["ex_dt1_11_office_02b"],
                PropertyInterior.MazeBankExecutiveCool => ["ex_dt1_11_office_02c"],
                PropertyInterior.MazeBankExecutiveContrast => ["ex_dt1_11_office_02a"],
                PropertyInterior.MazeBankOldSpiceWarm => ["ex_dt1_11_office_01a"],
                PropertyInterior.MazeBankOldSpiceClassical => ["ex_dt1_11_office_01b"],
                PropertyInterior.MazeBankOldSpiceVintage => ["ex_dt1_11_office_01c"],
                PropertyInterior.MazeBankPowerBrokerIce => ["ex_dt1_11_office_03a"],
                PropertyInterior.MazeBankPowerBrokeConservative => ["ex_dt1_11_office_03b"],
                PropertyInterior.MazeBankPowerBrokePolished => ["ex_dt1_11_office_03c"],
                PropertyInterior.LomBankExecutiveRich => ["ex_sm_13_office_02b"],
                PropertyInterior.LomBankExecutiveCool => ["ex_sm_13_office_02c"],
                PropertyInterior.LomBankExecutiveContrast => ["ex_sm_13_office_02a"],
                PropertyInterior.LomBankOldSpiceWarm => ["ex_sm_13_office_01a"],
                PropertyInterior.LomBankOldSpiceClassical => ["ex_sm_13_office_01b"],
                PropertyInterior.LomBankOldSpiceVintage => ["ex_sm_13_office_01c"],
                PropertyInterior.LomBankPowerBrokerIce => ["ex_sm_13_office_03a"],
                PropertyInterior.LomBankPowerBrokeConservative => ["ex_sm_13_office_03b"],
                PropertyInterior.LomBankPowerBrokePolished => ["ex_sm_13_office_03c"],
                PropertyInterior.MazeBankWestExecutiveRich => ["ex_sm_15_office_02b"],
                PropertyInterior.MazeBankWestExecutiveCool => ["ex_sm_15_office_02c"],
                PropertyInterior.MazeBankWestExecutiveContrast => ["ex_sm_15_office_02a"],
                PropertyInterior.MazeBankWestOldSpiceWarm => ["ex_sm_15_office_01a"],
                PropertyInterior.MazeBankWestOldSpiceClassical => ["ex_sm_15_office_01b"],
                PropertyInterior.MazeBankWestOldSpiceVintage => ["ex_sm_15_office_01c"],
                PropertyInterior.MazeBankWestPowerBrokerIce => ["ex_sm_15_office_03a"],
                PropertyInterior.MazeBankWestPowerBrokeConservative => ["ex_sm_15_office_03b"],
                PropertyInterior.MazeBankWestPowerBrokePolished => ["ex_sm_15_office_03c"],
                PropertyInterior.Clubhouse1 => ["bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo"],
                PropertyInterior.Clubhouse2 => ["bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo"],
                PropertyInterior.MethLab => ["bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo"],
                PropertyInterior.WeedFarm => ["bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo"],
                PropertyInterior.CocaineLockup => ["bkr_biker_interior_placement_interior_4_biker_dlc_int_ware03_milo"],
                PropertyInterior.CounterfeitCashFactory => ["bkr_biker_interior_placement_interior_5_biker_dlc_int_ware04_milo"],
                PropertyInterior.DocumentForgeryOffice => ["bkr_biker_interior_placement_interior_6_biker_dlc_int_ware05_milo"],
                PropertyInterior.WarehouseSmall => ["ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo"],
                PropertyInterior.WarehouseMedium => ["ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo"],
                PropertyInterior.WarehouseLarge => ["ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo"],
                _ => [],
            };
        }

        public static void SendJobMessage(CharacterJob characterJob, string message, string color = "#FFFFFF")
        {
            foreach (var player in Global.SpawnedPlayers.Where(x => x.Character.Job == characterJob && x.OnDuty))
                player.SendMessage(Models.MessageType.None, message, color);
        }

        public static string GetBodyPartName(BodyPart bodyPart)
        {
            return bodyPart switch
            {
                BodyPart.Pelvis => "Pélvis",
                BodyPart.LeftHip => "Quadril Esquerdo",
                BodyPart.LeftLeg => "Perna Esquerda",
                BodyPart.LeftFoot => "Pé Esquerdo",
                BodyPart.RightHip => "Quadril Direito",
                BodyPart.RightLeg => "Perna Direita",
                BodyPart.RightFoot => "Pé Direito",
                BodyPart.LowerTorso => "Torso Inferior",
                BodyPart.UpperTorso => "Torso Superior",
                BodyPart.Chest => "Peito",
                BodyPart.UnderNeck => "Sob o Pescoço",
                BodyPart.LeftShoulder => "Ombro Esquerdo",
                BodyPart.LeftUpperArm => "Braço Esquerdo",
                BodyPart.LeftElbow => "Cotovelo Esquerdo",
                BodyPart.LeftWrist => "Pulso Esquerdo",
                BodyPart.RightShoulder => "Ombro Direito",
                BodyPart.RightUpperArm => "Braço Direito",
                BodyPart.RightElbow => "Cotovelo Direito",
                BodyPart.RightWrist => "Pulso Direito",
                BodyPart.Neck => "Pescoço",
                BodyPart.Head => "Cabeça",
                _ => "Desconhecida",
            };
        }

        public static void GetException(Exception ex) => Alt.LogError($"{ex.InnerException?.Message ?? ex.Message} - {ex.Source} - {ex.StackTrace}");

        public static string GetBaseHTML(string title, string body)
        {
            return $@"<div class='panel panel-bordered panel-primary' style='width:60%;text-align:left;display:inline-block;margin-top:3%;'>
            <div class='panel-heading'>
                <div class='panel-control'>
                    <button class='btn btn-default' onclick='closeView();'>
                        Fechar
                    </button>
                </div>
                <h3 class='panel-title'>{title}</h3>
            </div>
            <div class='panel-body'>
            {body}
            </div>
            </div>";
        }

        public static async Task SendStaffMessage(string message, bool onlyStaff, bool discord = false)
        {
            var players = Global.SpawnedPlayers;
            if (onlyStaff)
                players = players.Where(x => x.User.Staff != UserStaff.None).ToList();

            foreach (var player in players)
                player.SendMessage(Models.MessageType.None, $"[{Global.SERVER_INITIALS}] {message}", Global.STAFF_COLOR);

            if (!discord
                || Global.DiscordClient == null
                || Global.DiscordClient.GetChannel(Global.StaffDiscordChannel) is not SocketTextChannel channel)
                return;

            var mentions = string.Empty;
            foreach (var role in Global.RolesStaffMessage)
                mentions += $@" {MentionUtils.MentionRole(role)}";

            var embedBuilder = new EmbedBuilder
            {
                Title = Global.SERVER_NAME,
                Description = message,
                Color = new Discord.Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
            };
            embedBuilder.WithFooter($"Enviada em {DateTime.Now}.");

            await channel.SendMessageAsync(embed: embedBuilder.Build());

            if (!string.IsNullOrWhiteSpace(mentions))
                await channel.SendMessageAsync(mentions);
        }

        public static Tuple<string, UserVIP> CheckVIPVehicle(string model)
        {
            var display = string.Empty;
            var vip = UserVIP.None;
            model = model.ToLower();

            if (model.Equals(VehicleModel.Speeder.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.TriBike3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Supervolito2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Bf400.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Dominator3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Dubsta3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Luxor2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Contender.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Patriot2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Deveste.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Elegy.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Neon.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Issi7.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Pfister811.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Banshee2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Shinobi.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Reever.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Comet7.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Deity.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Granger2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Zeno.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Blazer2.ToString(), StringComparison.CurrentCultureIgnoreCase)
            )
            {
                display = $"<span class='label' style='background-color:#f1c40f'>VIP OURO</span>";
                vip = UserVIP.Gold;
            }
            else if (model.Equals(VehicleModel.Tropic2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Issi2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Windsor2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.TriBike2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Akuma.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.CarbonRs.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Yosemite2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Brawler.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Everon.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Nimbus.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Comet5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Ninef2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Entity2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Prototipo.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Emerus.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Reever.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Iwagen.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Astron.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Jubilee.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Ignus.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Patriot3.ToString(), StringComparison.CurrentCultureIgnoreCase)
            )
            {
                display = $"<span class='label' style='background-color:#607d8b'>VIP PRATA</span>";
                vip = UserVIP.Silver;
            }
            else if (model.Equals(VehicleModel.Seashark.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Seashark3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.TriBike.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Havok.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Double.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Hakuchou2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Vindicator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Baller2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Locust.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Komoda.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Turismo2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Krieger.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Nero2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Tyrant.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Cinquemila.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Buffalo4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Baller7.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Youga4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Mule5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Sanchez2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || model.Equals(VehicleModel.Blazer3.ToString(), StringComparison.CurrentCultureIgnoreCase)
            )
            {
                display = $"<span class='label' style='background-color:#a84300'>VIP BRONZE</span>";
                vip = UserVIP.Bronze;
            }

            return new Tuple<string, UserVIP>(display, vip);
        }

        public static void CreateMarkerColShape(string description, Position position)
        {
            position.Z -= 0.95f;

            // TODO: Rollback commentary when alt:V implements
            //var marker = Alt.CreateMarker(MarkerType.MarkerHalo, position, Global.MainRgba);
            //marker.Scale = new Vector3(1, 1, 1.5f);

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(position, 1, 1.5f);
            colShape.Description = description;
        }

        public static string CheckFinalDot(string message)
        {
            message = message.Trim();
            var caracter = message.LastOrDefault();
            if (caracter != '.' && caracter != '!' && caracter != '?')
                message += ".";

            return message;
        }

        public static async Task WriteLog(LogType type, string description)
        {
            await using var context = new DatabaseContext();
            var log = new Log();
            log.Create(type, description);
            await context.Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public static void SendRadioMessage(int channel, string message)
        {
            foreach (var player in Global.SpawnedPlayers.Where(x => x.RadioCommunicatorItem.Channel1 == channel
                            || x.RadioCommunicatorItem.Channel2 == channel
                            || x.RadioCommunicatorItem.Channel3 == channel
                            || x.RadioCommunicatorItem.Channel4 == channel
                            || x.RadioCommunicatorItem.Channel5 == channel))
            {
                if (!player.OnDuty && ((channel >= 911 && channel <= 950) || channel == 999))
                    continue;

                var slotPlayer = 1;
                if (player.RadioCommunicatorItem.Channel2 == channel)
                    slotPlayer = 2;
                else if (player.RadioCommunicatorItem.Channel3 == channel)
                    slotPlayer = 3;
                else if (player.RadioCommunicatorItem.Channel4 == channel)
                    slotPlayer = 4;
                else if (player.RadioCommunicatorItem.Channel5 == channel)
                    slotPlayer = 5;

                player.SendMessage(Models.MessageType.None, $"[S:{slotPlayer} C:{channel}] {message}", slotPlayer == 1 ? "#FFFF9B" : "#9e8d66");
            }
        }

        public static async Task SendDiscordMessage(string discordId, string text)
        {
            if (Global.DiscordClient == null)
                return;

            var user = Global.DiscordClient.GetUser(Convert.ToUInt64(discordId));
            if (user == null)
                return;

            await user.SendMessageAsync(text);
        }

        #region Items
        public static int GetItemMaxQuantity(ItemCategory itemCategory)
        {
            if (itemCategory == ItemCategory.Cloth1 || itemCategory == ItemCategory.Cloth3
                    || itemCategory == ItemCategory.Cloth4 || itemCategory == ItemCategory.Cloth5
                    || itemCategory == ItemCategory.Cloth6 || itemCategory == ItemCategory.Cloth7
                    || itemCategory == ItemCategory.Cloth8 || itemCategory == ItemCategory.Cloth9
                    || itemCategory == ItemCategory.Cloth10 || itemCategory == ItemCategory.Cloth11)
                return 2;

            return int.MaxValue;
        }

        public static bool CanDropItem(CharacterSex characterSex, Faction faction, CharacterItem characterItem)
        {
            if (faction?.Government ?? false)
            {
                if (characterItem.Category == ItemCategory.Weapon || characterItem.Category == ItemCategory.WalkieTalkie)
                    return false;

                bool FilterClothes(List<ClotheAccessory> clothes)
                {
                    var extra = Deserialize<ClotheAccessoryItem>(characterItem.Extra);
                    return clothes.Any(x => x.Drawable == characterItem.Type && x.DLC == extra?.DLC && x.TipoFaccao > -2 && x.TipoFaccao < 1);
                }

                if (characterItem.Category == ItemCategory.Cloth1)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes1Male : Global.Clothes1Female);

                if (characterItem.Category == ItemCategory.Cloth3)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes3Male : Global.Clothes3Female);

                if (characterItem.Category == ItemCategory.Cloth4)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes4Male : Global.Clothes4Female);

                if (characterItem.Category == ItemCategory.Cloth5)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes5Male : Global.Clothes5Female);

                if (characterItem.Category == ItemCategory.Cloth6)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes6Male : Global.Clothes6Female);

                if (characterItem.Category == ItemCategory.Cloth7)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes7Male : Global.Clothes7Female);

                if (characterItem.Category == ItemCategory.Cloth8)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes8Male : Global.Clothes8Female);

                if (characterItem.Category == ItemCategory.Cloth9)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes9Male : Global.Clothes9Female);

                if (characterItem.Category == ItemCategory.Cloth10)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes10Male : Global.Clothes10Female);

                if (characterItem.Category == ItemCategory.Cloth11)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Clothes11Male : Global.Clothes11Female);

                if (characterItem.Category == ItemCategory.Accessory0)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Accessories0Male : Global.Accessories0Female);

                if (characterItem.Category == ItemCategory.Accessory1)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Accessories1Male : Global.Accessories1Female);

                if (characterItem.Category == ItemCategory.Accessory2)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Accessories2Male : Global.Accessories2Female);

                if (characterItem.Category == ItemCategory.Accessory6)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Accessories6Male : Global.Accessories6Female);

                if (characterItem.Category == ItemCategory.Accessory7)
                    return FilterClothes(characterSex == CharacterSex.Man ? Global.Accessories7Male : Global.Accessories7Female);
            }

            return true;
        }

        public static bool CheckIfIsDrug(ItemCategory itemCategory)
        {
            return itemCategory == ItemCategory.Weed || itemCategory == ItemCategory.Cocaine
                || itemCategory == ItemCategory.Crack || itemCategory == ItemCategory.Heroin
                || itemCategory == ItemCategory.MDMA || itemCategory == ItemCategory.Xanax
                || itemCategory == ItemCategory.Oxycontin || itemCategory == ItemCategory.Metanfetamina;
        }
        #endregion Items

        #region Commands
        public static async Task CMDTrancar(MyPlayer player)
        {
            var prox = Global.Properties
                .Where(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));

            prox ??= Global.Properties
                .Where(x => x.Number == player.Dimension
                    && player.Position.Distance(new Position(x.ExitPosX, x.ExitPosY, x.ExitPosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.ExitPosX, x.ExitPosY, x.ExitPosZ)));

            if (prox != null)
            {
                if (!prox.CanAccess(player))
                {
                    player.SendMessage(Models.MessageType.Error, "Você não possui acesso a esta propriedade.");
                    return;
                }

                prox.StopAlarm();
                prox.SetLocked(!prox.Locked);
                player.SendMessageToNearbyPlayers($"{(!prox.Locked ? "des" : string.Empty)}tranca a porta.", MessageCategory.Ame, 5);

                await using var context = new DatabaseContext();
                context.Properties.Update(prox);
                await context.SaveChangesAsync();

                return;
            }

            var veh = Global.Vehicles
                .Where(x => x.Dimension == player.Dimension
                    && player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 5)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh != null)
            {
                if (!veh.CanAccess(player))
                {
                    player.SendMessage(Models.MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                    return;
                }

                if (veh.LockState == VehicleLockState.Locked)
                {
                    if (!player.CheckAnimations(true))
                        return;
                }

                veh.StopAlarm();
                veh.LockState = veh.LockState == VehicleLockState.Locked ? VehicleLockState.Unlocked : VehicleLockState.Locked;
                player.SendMessageToNearbyPlayers($"{(veh.LockState == VehicleLockState.Unlocked ? "des" : string.Empty)}tranca o veículo.", MessageCategory.Ame, 5);
                return;
            }

            player.SendMessage(Models.MessageType.Error, "Você não tem acesso a nenhuma propriedade ou veículo próximos.");
        }

        public static void CMDTuning(MyPlayer player, MyPlayer? target, bool staff)
        {
            if (player.Vehicle is not MyVehicle veh || veh == null || veh.Driver != player)
            {
                player.SendMessage(Models.MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            var vehiclePrice = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.VehicleDB.Model, StringComparison.CurrentCultureIgnoreCase));

            if (staff)
            {
                if (!veh.VehicleDB.FactionId.HasValue)
                {
                    player.SendMessage(Models.MessageType.Error, "O veículo não pertence a uma facção.");
                    return;
                }
            }
            else
            {
                var spot = Global.Spots.FirstOrDefault(x => x.Type == SpotType.MechanicWorkshop
                    && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
                if (spot == null)
                {
                    player.SendMessage(Models.MessageType.Error, "Você não está próximo de nenhum ponto de oficina mecânica.");
                    return;
                }

                if (vehiclePrice == null)
                {
                    player.SendMessage(Models.MessageType.Error, "Preço do veículo não foi encontrado.");
                    return;
                }
            }

            static string GetVehicleModTypeName(VehicleModType vehicleModType)
            {
                return vehicleModType switch
                {
                    VehicleModType.FrontBumper => "Para-choque Dianteiro",
                    VehicleModType.RearBumper => "Para-choque Traseiro",
                    VehicleModType.SideSkirt => "Saia Lateral",
                    VehicleModType.Exhaust => "Descarga",
                    VehicleModType.Frame => "Quadro",
                    VehicleModType.Grille => "Grade",
                    VehicleModType.Hood => "Capô",
                    VehicleModType.Fender => "Paralama",
                    VehicleModType.RightFender => "Paralama Direito",
                    VehicleModType.Roof => "Teto",
                    VehicleModType.Engine => "Motor",
                    VehicleModType.Brakes => "Freio",
                    VehicleModType.Transmission => "Transmissão",
                    VehicleModType.Horns => "Buzina",
                    VehicleModType.Suspension => "Suspensão",
                    VehicleModType.Armor => "Blindagem",
                    VehicleModType.FrontWheels => "Rodas Dianteiras",
                    VehicleModType.BackWheels => "Rodas Traseiras",
                    VehicleModType.PlateHolders => "Suporte de Placa",
                    VehicleModType.TrimDesign => "Acabamento",
                    VehicleModType.Ornaments => "Enfeites",
                    VehicleModType.DialDesign => "Discagem",
                    VehicleModType.SteeringWheel => "Volante",
                    VehicleModType.ShiftLever => "Câmbio",
                    VehicleModType.Plaques => "Placa",
                    VehicleModType.Hydraulics => "Hidráulica",
                    VehicleModType.Plate => "Placa",
                    VehicleModType.WindowTint => "Cor do Vidro",
                    VehicleModType.DashboardColor => "Cor do Painel",
                    VehicleModType.TrimColor => "Cor da Guarnição",
                    _ => vehicleModType.ToString(),
                };
            }

            var realMods = Deserialize<List<VehicleMod>>(veh.VehicleDB.ModsJSON);

            var vehicleTuning = new VehicleTuning();
            if (player.VehicleTuning?.VehicleId == veh.VehicleDB.Id)
            {
                vehicleTuning = player.VehicleTuning;
            }
            else
            {
                vehicleTuning = new VehicleTuning
                {
                    TargetId = target?.Character?.Id,
                    Staff = staff,
                    CurrentMods = Enum.GetValues(typeof(VehicleModType)).Cast<VehicleModType>()
                        .Select(x => new VehicleTuning.Mod
                        {
                            Type = Convert.ToByte(x),
                            Name = GetVehicleModTypeName(x),
                            ModsCount = player.Vehicle.GetModsCountExt(x),
                            UnitaryValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) *
                                ((Global.Prices.FirstOrDefault(y => y.Type == PriceType.Tuning && y.Name.Equals(x.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0)
                                / 100))),
                            Current = realMods.FirstOrDefault(y => y.Type == (byte)x)?.Id ?? 0,
                            Selected = realMods.FirstOrDefault(y => y.Type == (byte)x)?.Id ?? 0,
                        }).Where(x => x.ModsCount > 0 && (x.UnitaryValue > 0 || staff)).ToList(),
                    RepairValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("repair", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    WheelType = veh.VehicleDB.WheelType,
                    WheelVariation = veh.VehicleDB.WheelVariation,
                    WheelColor = veh.VehicleDB.WheelColor,
                    WheelValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("wheel", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    Color1 = $"#{veh.VehicleDB.Color1R:X2}{veh.VehicleDB.Color1G:X2}{veh.VehicleDB.Color1B:X2}",
                    Color2 = $"#{veh.VehicleDB.Color2R:X2}{veh.VehicleDB.Color2G:X2}{veh.VehicleDB.Color2B:X2}",
                    ColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("color", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    NeonColor = $"#{veh.VehicleDB.NeonColorR:X2}{veh.VehicleDB.NeonColorG:X2}{veh.VehicleDB.NeonColorB:X2}",
                    NeonLeft = Convert.ToByte(veh.VehicleDB.NeonLeft),
                    NeonRight = Convert.ToByte(veh.VehicleDB.NeonRight),
                    NeonFront = Convert.ToByte(veh.VehicleDB.NeonFront),
                    NeonBack = Convert.ToByte(veh.VehicleDB.NeonBack),
                    NeonValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("neon", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    HeadlightColor = veh.VehicleDB.HeadlightColor,
                    LightsMultiplier = veh.VehicleDB.LightsMultiplier,
                    XenonColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("xenoncolor", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    WindowTint = veh.VehicleDB.WindowTint,
                    WindowTintValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("insulfilm", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                    TireSmokeColor = $"#{veh.VehicleDB.TireSmokeColorR:X2}{veh.VehicleDB.TireSmokeColorG:X2}{veh.VehicleDB.TireSmokeColorB:X2}",
                    TireSmokeColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("tiresmoke", StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) / 100))),
                };

                vehicleTuning.CurrentWheelType = vehicleTuning.WheelType;
                vehicleTuning.CurrentWheelVariation = vehicleTuning.WheelVariation;
                vehicleTuning.CurrentColor1 = vehicleTuning.Color1;
                vehicleTuning.CurrentColor2 = vehicleTuning.Color2;
                vehicleTuning.CurrentNeonColor = vehicleTuning.NeonColor;
                vehicleTuning.CurrentNeonLeft = vehicleTuning.NeonLeft;
                vehicleTuning.CurrentNeonRight = vehicleTuning.NeonRight;
                vehicleTuning.CurrentNeonFront = vehicleTuning.NeonFront;
                vehicleTuning.CurrentNeonBack = vehicleTuning.NeonBack;
                vehicleTuning.CurrentHeadlightColor = vehicleTuning.HeadlightColor;
                vehicleTuning.CurrentLightsMultiplier = vehicleTuning.LightsMultiplier;
                vehicleTuning.CurrentWindowTint = vehicleTuning.WindowTint;
                vehicleTuning.CurrentTireSmokeColor = vehicleTuning.TireSmokeColor;
            }

            player.Emit("VehicleTuning", Serialize(vehicleTuning));
        }
        #endregion Commands

        #region Staff
        public static string GetSOSJSON()
        {
            var sos = Global.HelpRequests.OrderBy(x => x.Date)
                .Select(x => new
                {
                    x.CharacterSessionId,
                    Date = x.Date.ToString(),
                    x.CharacterName,
                    x.UserName,
                    x.Message,
                });
            return Serialize(sos);
        }

        public static string GetFactionRanksHTML(Guid factionId)
        {
            var html = string.Empty;
            var ranks = Global.FactionsRanks.Where(x => x.FactionId == factionId);
            if (!ranks.Any())
            {
                html = "<tr><td class='text-center' colspan='4'>Não há ranks criados.</td></tr>";
            }
            else
            {
                foreach (var rank in ranks.OrderBy(x => x.Position))
                    html += $@"<tr data-id='{rank.Id}' class='searchRank'>
                        <td>{rank.Id}</td>
                        <td>{rank.Name}</td>
                        <td>{rank.Salary:N0}</td>
                        <td class='leader text-center'>
                            <input id='jsonRank{rank.Id}' type='hidden' value='{Serialize(rank)}' />
                            <button onclick='editRank(`{rank.Id}`)' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='removeRank(this, `{rank.Id}`)' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static async Task<string> GetFactionMembersHTML(Guid factionId)
        {
            await using var context = new DatabaseContext();
            var members = (await context.Characters
                .Where(x => x.FactionId == factionId && !x.DeathDate.HasValue && !x.DeletedDate.HasValue)
                .Include(x => x.User)
                .Include(x => x.FactionRank)
                .ToListAsync())
                .Select(x => new
                {
                    Character = x,
                    OnlineCharacter = Global.SpawnedPlayers.FirstOrDefault(y => y.Character.Id == x.Id),
                })
                .OrderByDescending(x => x.OnlineCharacter != null)
                .ThenByDescending(x => x.OnlineCharacter?.OnDuty ?? false)
                .ThenByDescending(x => x.Character.FactionRank.Position)
                .ThenBy(x => x.Character.Name);

            var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);

            var html = string.Empty;
            if (!members.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há membros na facção.</td></tr>";
            }
            else
            {
                foreach (var member in members)
                {
                    var online = member.OnlineCharacter != null ?
                        $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>ONLINE</span>"
                        :
                        $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>OFFLINE</span>";

                    var status = member.OnlineCharacter?.OnDuty ?? false ?
                        $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>EM SERVIÇO</span>"
                        :
                        $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>FORA DE SERVIÇO</span>";

                    html += $@"<tr class='searchMember'>
                        <td>{member.Character.FactionRank.Name}</td>
                        <td>{member.Character.Name} [{member.Character.Id}]</td>
                        <td>{member.Character.User.Name} [{member.Character.UserId}]</td>
                        <td>{member.Character.LastAccessDate}</td>
                        {(faction.Government ? $"<td>{member.Character.Badge}</td>" : string.Empty)}
                        <td class='text-center'>{online}</td>
                        {(faction.Government ? $"<td class='text-center'>{status}</td>" : string.Empty)}
                        <td class='text-center tdOptions'>
                            <input id='jsonMember{member.Character.Id}' type='hidden' value='{Serialize(new { member.Character.Name, member.Character.Badge, member.Character.FactionRankId, member.Character.FactionFlagsJSON })}' />
                            <button onclick='editMember(`{member.Character.Id}`)' type='button' class='btn btn-dark btn-sm editMember'>EDITAR</button>
                            <button onclick='removeMember(`{member.Character.Id}`, `{member.Character.Name}`)' type='button' class='btn btn-danger btn-sm removeMember'>EXPULSAR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
        #endregion Staff

        #region Cellphone
        public async static Task<uint> GetNewCellphoneNumber()
        {
            var cellphone = 0u;
            await using var context = new DatabaseContext();
            do
            {
                cellphone = Convert.ToUInt32(new Random().Next(1111111, 9999999));
                if (cellphone == Global.MECHANIC_NUMBER
                    || cellphone == Global.TAXI_NUMBER
                    || await context.CharactersItems.AnyAsync(x => x.Category == ItemCategory.Cellphone && x.Type == cellphone)
                    || await context.Items.AnyAsync(x => x.Category == ItemCategory.Cellphone && x.Type == cellphone)
                    || await context.VehiclesItems.AnyAsync(x => x.Category == ItemCategory.Cellphone && x.Type == cellphone)
                    || await context.PropertiesItems.AnyAsync(x => x.Category == ItemCategory.Cellphone && x.Type == cellphone))
                    cellphone = 0;
            } while (cellphone == 0);
            return cellphone;
        }

        public static List<CellphoneItemContact> GetDefaultsContacts()
        {
            return
            [
                new(Global.EMERGENCY_NUMBER, "Central de Emergência"),
                new(Global.MECHANIC_NUMBER, "Central de Mecânicos"),
                new(Global.TAXI_NUMBER, "Downtown Cab Company"),
            ];
        }
        #endregion Cellphone

        public static string Serialize(object data) => JsonSerializer.Serialize(data, Global.JsonSerializerOptions);

        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Global.JsonSerializerOptions);
    }
}