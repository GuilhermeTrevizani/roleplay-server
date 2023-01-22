using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using Roleplay.Streamer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Roleplay
{
    public static class Functions
    {
        public static string Encrypt(string text)
        {
            var encodedValue = Encoding.UTF8.GetBytes(text);
            var encryptedPassword = SHA512.Create().ComputeHash(encodedValue);

            var sb = new StringBuilder();
            foreach (var caracter in encryptedPassword)
                sb.Append(caracter.ToString("X2"));

            return sb.ToString();
        }

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
            var players = Global.Players.Where(x => x.Faction?.Type == factionType);

            if (onlyOnDuty)
                players = players.Where(x => x.OnDuty);

            foreach (var player in players)
                player.SendMessage(Models.MessageType.None, message, factionColor ? $"#{player.Faction.Color}" : "#FFFFFF");
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
                    plate = $"1{random.Next(0, 999999).ToString().PadLeft(6, '0')}";
                else
                    plate = $"{random.Next(2, 9)}{chars[random.Next(25)]}{chars[random.Next(25)]}{chars[random.Next(25)]}{random.Next(0, 999).ToString().PadLeft(3, '0')}";
            } while (await context.Vehicles.AnyAsync(x => x.Plate == plate));

            return plate;
        }

        public static List<string> GetIPLsByInterior(PropertyInterior propertyInterior)
        {
            return propertyInterior switch
            {
                PropertyInterior.Modern1Apartment => new List<string> { "apa_v_mp_h_01_a" },
                PropertyInterior.Modern2Apartment => new List<string> { "apa_v_mp_h_01_c" },
                PropertyInterior.Modern3Apartment => new List<string> { "apa_v_mp_h_01_b" },
                PropertyInterior.Mody1Apartment => new List<string> { "apa_v_mp_h_02_a" },
                PropertyInterior.Mody2Apartment => new List<string> { "apa_v_mp_h_02_c" },
                PropertyInterior.Mody3Apartment => new List<string> { "apa_v_mp_h_02_b" },
                PropertyInterior.Vibrant1Apartment => new List<string> { "apa_v_mp_h_03_a" },
                PropertyInterior.Vibrant2Apartment => new List<string> { "apa_v_mp_h_03_c" },
                PropertyInterior.Vibrant3Apartment => new List<string> { "apa_v_mp_h_03_b" },
                PropertyInterior.Sharp1Apartment => new List<string> { "apa_v_mp_h_04_a" },
                PropertyInterior.Sharp2Apartment => new List<string> { "apa_v_mp_h_04_c" },
                PropertyInterior.Sharp3Apartment => new List<string> { "apa_v_mp_h_04_b" },
                PropertyInterior.Monochrome1Apartment => new List<string> { "apa_v_mp_h_05_a" },
                PropertyInterior.Monochrome2Apartment => new List<string> { "apa_v_mp_h_05_c" },
                PropertyInterior.Monochrome3Apartment => new List<string> { "apa_v_mp_h_05_b" },
                PropertyInterior.Seductive1Apartment => new List<string> { "apa_v_mp_h_06_a" },
                PropertyInterior.Seductive2Apartment => new List<string> { "apa_v_mp_h_06_c" },
                PropertyInterior.Seductive3Apartment => new List<string> { "apa_v_mp_h_06_b" },
                PropertyInterior.Regal1Apartment => new List<string> { "apa_v_mp_h_07_a" },
                PropertyInterior.Regal2Apartment => new List<string> { "apa_v_mp_h_07_c" },
                PropertyInterior.Regal3Apartment => new List<string> { "apa_v_mp_h_07_b" },
                PropertyInterior.Aqua1Apartment => new List<string> { "apa_v_mp_h_08_a" },
                PropertyInterior.Aqua2Apartment => new List<string> { "apa_v_mp_h_08_c" },
                PropertyInterior.Aqua3Apartment => new List<string> { "apa_v_mp_h_08_b" },
                PropertyInterior.ArcadiusExecutiveRich => new List<string> { "ex_dt1_02_office_02b" },
                PropertyInterior.ArcadiusExecutiveCool => new List<string> { "ex_dt1_02_office_02c" },
                PropertyInterior.ArcadiusExecutiveContrast => new List<string> { "ex_dt1_02_office_02a" },
                PropertyInterior.ArcadiusOldSpiceWarm => new List<string> { "ex_dt1_02_office_01a" },
                PropertyInterior.ArcadiusOldSpiceClassical => new List<string> { "ex_dt1_02_office_01b" },
                PropertyInterior.ArcadiusOldSpiceVintage => new List<string> { "ex_dt1_02_office_01c" },
                PropertyInterior.ArcadiusPowerBrokerIce => new List<string> { "ex_dt1_02_office_03a" },
                PropertyInterior.ArcadiusPowerBrokeConservative => new List<string> { "ex_dt1_02_office_03b" },
                PropertyInterior.ArcadiusPowerBrokePolished => new List<string> { "ex_dt1_02_office_03c" },
                PropertyInterior.MazeBankExecutiveRich => new List<string> { "ex_dt1_11_office_02b" },
                PropertyInterior.MazeBankExecutiveCool => new List<string> { "ex_dt1_11_office_02c" },
                PropertyInterior.MazeBankExecutiveContrast => new List<string> { "ex_dt1_11_office_02a" },
                PropertyInterior.MazeBankOldSpiceWarm => new List<string> { "ex_dt1_11_office_01a" },
                PropertyInterior.MazeBankOldSpiceClassical => new List<string> { "ex_dt1_11_office_01b" },
                PropertyInterior.MazeBankOldSpiceVintage => new List<string> { "ex_dt1_11_office_01c" },
                PropertyInterior.MazeBankPowerBrokerIce => new List<string> { "ex_dt1_11_office_03a" },
                PropertyInterior.MazeBankPowerBrokeConservative => new List<string> { "ex_dt1_11_office_03b" },
                PropertyInterior.MazeBankPowerBrokePolished => new List<string> { "ex_dt1_11_office_03c" },
                PropertyInterior.LomBankExecutiveRich => new List<string> { "ex_sm_13_office_02b" },
                PropertyInterior.LomBankExecutiveCool => new List<string> { "ex_sm_13_office_02c" },
                PropertyInterior.LomBankExecutiveContrast => new List<string> { "ex_sm_13_office_02a" },
                PropertyInterior.LomBankOldSpiceWarm => new List<string> { "ex_sm_13_office_01a" },
                PropertyInterior.LomBankOldSpiceClassical => new List<string> { "ex_sm_13_office_01b" },
                PropertyInterior.LomBankOldSpiceVintage => new List<string> { "ex_sm_13_office_01c" },
                PropertyInterior.LomBankPowerBrokerIce => new List<string> { "ex_sm_13_office_03a" },
                PropertyInterior.LomBankPowerBrokeConservative => new List<string> { "ex_sm_13_office_03b" },
                PropertyInterior.LomBankPowerBrokePolished => new List<string> { "ex_sm_13_office_03c" },
                PropertyInterior.MazeBankWestExecutiveRich => new List<string> { "ex_sm_15_office_02b" },
                PropertyInterior.MazeBankWestExecutiveCool => new List<string> { "ex_sm_15_office_02c" },
                PropertyInterior.MazeBankWestExecutiveContrast => new List<string> { "ex_sm_15_office_02a" },
                PropertyInterior.MazeBankWestOldSpiceWarm => new List<string> { "ex_sm_15_office_01a" },
                PropertyInterior.MazeBankWestOldSpiceClassical => new List<string> { "ex_sm_15_office_01b" },
                PropertyInterior.MazeBankWestOldSpiceVintage => new List<string> { "ex_sm_15_office_01c" },
                PropertyInterior.MazeBankWestPowerBrokerIce => new List<string> { "ex_sm_15_office_03a" },
                PropertyInterior.MazeBankWestPowerBrokeConservative => new List<string> { "ex_sm_15_office_03b" },
                PropertyInterior.MazeBankWestPowerBrokePolished => new List<string> { "ex_sm_15_office_03c" },
                PropertyInterior.Clubhouse1 => new List<string> { "bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo" },
                PropertyInterior.Clubhouse2 => new List<string> { "bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo" },
                PropertyInterior.MethLab => new List<string> { "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo" },
                PropertyInterior.WeedFarm => new List<string> { "bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo" },
                PropertyInterior.CocaineLockup => new List<string> { "bkr_biker_interior_placement_interior_4_biker_dlc_int_ware03_milo" },
                PropertyInterior.CounterfeitCashFactory => new List<string> { "bkr_biker_interior_placement_interior_5_biker_dlc_int_ware04_milo" },
                PropertyInterior.DocumentForgeryOffice => new List<string> { "bkr_biker_interior_placement_interior_6_biker_dlc_int_ware05_milo" },
                PropertyInterior.WarehouseSmall => new List<string> { "ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo" },
                PropertyInterior.WarehouseMedium => new List<string> { "ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo" },
                PropertyInterior.WarehouseLarge => new List<string> { "ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo" },
                _ => new List<string>(),
            };
        }

        public static string GetEnumDisplay(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes?.Length > 0)
                return attributes.FirstOrDefault().Name;
            return value.ToString();
        }

        public static void SendJobMessage(CharacterJob characterJob, string message, string color = "#FFFFFF")
        {
            foreach (var player in Global.Players.Where(x => x.Character.Job == characterJob && x.OnDuty))
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

        public static bool CheckEmail(string email)
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

        public static async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Global.EmailHost))
                    return;

                var msg = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress(Global.EmailAddress, Global.EmailName),
                    Subject = subject,
                    Body = body,
                    BodyEncoding = Encoding.UTF8,
                };
                msg.To.Add(email);

                var clienteSmtp = new SmtpClient(Global.EmailHost)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Global.EmailAddress, Global.EmailPassword),
                    Port = Global.EmailPort,
                };

                await clienteSmtp.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                GetException(ex);
            }
        }

        public static void GetException(Exception ex) => Alt.LogError($"{ex.InnerException?.Message ?? ex.Message} - {ex.Source} - {ex.StackTrace}");

        public static string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

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
            var players = Global.Players.Where(x => x.Character.PersonalizationStep == CharacterPersonalizationStep.Ready).ToList();
            if (onlyStaff)
                players = players.Where(x => x.User.Staff != UserStaff.None).ToList();

            foreach (var player in players)
                player.SendMessage(Models.MessageType.None, $"[{Global.SERVER_INITIALS}] {message}", Global.STAFF_COLOR);

            if (discord && !string.IsNullOrWhiteSpace(Global.DiscordBotToken))
            {
                var mentions = string.Empty;
                foreach (var role in Global.RolesStaffMessage)
                    mentions += $@" {MentionUtils.MentionRole(role)}";

                var embedBuilder = new EmbedBuilder
                {
                    Title = Global.SERVER_NAME,
                    Description = message,
                    Color = new Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
                };
                embedBuilder.WithFooter($"Enviada em {DateTime.Now}.");

                var channel = Global.DiscordClient.GetChannel(Global.StaffDiscordChannel) as SocketTextChannel;
                await channel.SendMessageAsync(embed: embedBuilder.Build());

                if (!string.IsNullOrWhiteSpace(mentions))
                    await channel.SendMessageAsync(mentions);
            }
        }

        public static Tuple<string, UserVIP> CheckVIPVehicle(string model)
        {
            var display = string.Empty;
            var vip = UserVIP.None;
            model = model.ToLower();

            if (model == VehicleModel.Speeder.ToString().ToLower()
                || model == VehicleModel.TriBike3.ToString().ToLower()
                || model == VehicleModel.Supervolito2.ToString().ToLower()
                || model == VehicleModel.Bf400.ToString().ToLower()
                || model == VehicleModel.Dominator3.ToString().ToLower()
                || model == VehicleModel.Dubsta3.ToString().ToLower()
                || model == VehicleModel.Luxor2.ToString().ToLower()
                || model == VehicleModel.Contender.ToString().ToLower()
                || model == VehicleModel.Patriot2.ToString().ToLower()
                || model == VehicleModel.Deveste.ToString().ToLower()
                || model == VehicleModel.Elegy.ToString().ToLower()
                || model == VehicleModel.Neon.ToString().ToLower()
                || model == VehicleModel.Issi7.ToString().ToLower()
                || model == VehicleModel.Pfister811.ToString().ToLower()
                || model == VehicleModel.Banshee2.ToString().ToLower()
                || model == VehicleModel.Shinobi.ToString().ToLower()
                || model == VehicleModel.Reever.ToString().ToLower()
                || model == VehicleModel.Comet7.ToString().ToLower()
                || model == VehicleModel.Deity.ToString().ToLower()
                || model == VehicleModel.Granger2.ToString().ToLower()
                || model == VehicleModel.Zeno.ToString().ToLower()
                || model == VehicleModel.Blazer2.ToString().ToLower()
            )
            {
                display = $"<span class='label' style='background-color:#f1c40f'>VIP OURO</span>";
                vip = UserVIP.Gold;
            }
            else if (model == VehicleModel.Tropic2.ToString().ToLower()
                || model == VehicleModel.Issi2.ToString().ToLower()
                || model == VehicleModel.Windsor2.ToString().ToLower()
                || model == VehicleModel.TriBike2.ToString().ToLower()
                || model == VehicleModel.Akuma.ToString().ToLower()
                || model == VehicleModel.CarbonRs.ToString().ToLower()
                || model == VehicleModel.Yosemite2.ToString().ToLower()
                || model == VehicleModel.Brawler.ToString().ToLower()
                || model == VehicleModel.Everon.ToString().ToLower()
                || model == VehicleModel.Nimbus.ToString().ToLower()
                || model == VehicleModel.Comet5.ToString().ToLower()
                || model == VehicleModel.Ninef2.ToString().ToLower()
                || model == VehicleModel.Entity2.ToString().ToLower()
                || model == VehicleModel.Prototipo.ToString().ToLower()
                || model == VehicleModel.Emerus.ToString().ToLower()
                || model == VehicleModel.Reever.ToString().ToLower()
                || model == VehicleModel.Iwagen.ToString().ToLower()
                || model == VehicleModel.Astron.ToString().ToLower()
                || model == VehicleModel.Jubilee.ToString().ToLower()
                || model == VehicleModel.Ignus.ToString().ToLower()
                || model == VehicleModel.Patriot3.ToString().ToLower()
            )
            {
                display = $"<span class='label' style='background-color:#607d8b'>VIP PRATA</span>";
                vip = UserVIP.Silver;
            }
            else if (model == VehicleModel.Seashark.ToString().ToLower()
                || model == VehicleModel.Seashark3.ToString().ToLower()
                || model == VehicleModel.TriBike.ToString().ToLower()
                || model == VehicleModel.Havok.ToString().ToLower()
                || model == VehicleModel.Double.ToString().ToLower()
                || model == VehicleModel.Hakuchou2.ToString().ToLower()
                || model == VehicleModel.Vindicator.ToString().ToLower()
                || model == VehicleModel.Baller2.ToString().ToLower()
                || model == VehicleModel.Locust.ToString().ToLower()
                || model == VehicleModel.Komoda.ToString().ToLower()
                || model == VehicleModel.Turismo2.ToString().ToLower()
                || model == VehicleModel.Krieger.ToString().ToLower()
                || model == VehicleModel.Nero2.ToString().ToLower()
                || model == VehicleModel.Tyrant.ToString().ToLower()
                || model == VehicleModel.Cinquemila.ToString().ToLower()
                || model == VehicleModel.Buffalo4.ToString().ToLower()
                || model == VehicleModel.Baller7.ToString().ToLower()
                || model == VehicleModel.Youga4.ToString().ToLower()
                || model == VehicleModel.Mule5.ToString().ToLower()
                || model == VehicleModel.Sanchez2.ToString().ToLower()
                || model == VehicleModel.Blazer3.ToString().ToLower()
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

            MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleSkinny,
                position,
                new Vector3(1, 1, 1.5f),
                color: Global.MainRgba);

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

        public static async Task WriteLog(LogType logType, string description)
        {
            await using var context = new DatabaseContext();
            await context.Logs.AddAsync(new Log
            {
                Type = logType,
                Description = description,
            });
            await context.SaveChangesAsync();
        }

        public static void SendRadioMessage(int channel, string message)
        {
            foreach (var player in Global.Players.Where(x => x.RadioCommunicatorItem.Canal1 == channel
                            || x.RadioCommunicatorItem.Canal2 == channel
                            || x.RadioCommunicatorItem.Canal3 == channel
                            || x.RadioCommunicatorItem.Canal4 == channel
                            || x.RadioCommunicatorItem.Canal5 == channel))
            {
                if (!player.OnDuty && ((channel >= 911 && channel <= 950) || channel == 999))
                    continue;

                var slotPlayer = 1;
                if (player.RadioCommunicatorItem.Canal2 == channel)
                    slotPlayer = 2;
                else if (player.RadioCommunicatorItem.Canal3 == channel)
                    slotPlayer = 3;
                else if (player.RadioCommunicatorItem.Canal4 == channel)
                    slotPlayer = 4;
                else if (player.RadioCommunicatorItem.Canal5 == channel)
                    slotPlayer = 5;

                player.SendMessage(Models.MessageType.None, $"[S:{slotPlayer} C:{channel}] {message}", slotPlayer == 1 ? "#FFFF9B" : "#9e8d66");
            }
        }

        #region Items
        public static string GetItemExtra(BaseItem item)
        {
            if (item.Category == ItemCategory.Weapon)
            {
                var extra = JsonSerializer.Deserialize<WeaponItem>(item.Extra);
                var components = string.Join(", ", extra.Components.Select(x => Global.WeaponComponents.FirstOrDefault(y => y.Hash == x)?.Name ?? string.Empty));
                if (!string.IsNullOrWhiteSpace(components))
                    components = $"<br/>Componentes: <strong>{components}</strong>";

                return $"Munição: <strong>{extra.Ammo}</strong><br/>Pintura: <strong>{extra.TintIndex}</strong>{components}";
            }

            if (item.IsCloth)
            {
                var extra = JsonSerializer.Deserialize<ClotheAccessoryItem>(item.Extra);
                return $"Modelo: <strong>{item.Type}{(!string.IsNullOrWhiteSpace(extra.DLC) ? $" ({extra.DLC})" : string.Empty)}</strong><br/>Textura: <strong>{extra.Texture}</strong>";
            }

            if (item.Category == ItemCategory.PropertyKey
                || item.Category == ItemCategory.VehicleKey)
                return $"Fechadura: <strong>{item.Type}</strong>";

            if (item.Category == ItemCategory.WalkieTalkie)
            {
                var extra = JsonSerializer.Deserialize<RadioCommunicatorItem>(item.Extra);
                return $"Canal 1: <strong>{extra.Canal1}</strong><br/>Canal 2: <strong>{extra.Canal2}</strong><br/>Canal 3: <strong>{extra.Canal3}</strong><br/>Canal 4: <strong>{extra.Canal4}</strong><br/>Canal 5: <strong>{extra.Canal5}</strong>";
            }

            if (item.Category == ItemCategory.Cellphone)
                return $"Número: <strong>{item.Type}</strong>";

            if (item.Category == ItemCategory.Boombox)
                return $"Alcance: <strong>{item.Type} metros</strong>";

            return string.Empty;
        }

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
                    var extra = JsonSerializer.Deserialize<ClotheAccessoryItem>(characterItem.Extra);
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
                .Where(x => x.Id == player.Dimension
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
                prox.Locked = !prox.Locked;
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

        public static void CMDMotor(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(Models.MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(Models.MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.Vehicle.Fuel == 0)
            {
                player.SendMessage(Models.MessageType.Error, "Veículo não possui combustível.");
                return;
            }

            if (veh.Info.Type == VehicleModelType.BMX)
            {
                player.SendMessage(Models.MessageType.Error, "Veículo não possui motor.");
                return;
            }

            player.SendMessageToNearbyPlayers($"{(player.Vehicle.EngineOn ? "des" : string.Empty)}liga o motor do veículo.", MessageCategory.Ame, 5);
            player.Vehicle.EngineOn = !player.Vehicle.EngineOn;
        }

        public static void CMDTuning(MyPlayer player, MyPlayer target, bool staff)
        {
            if (player.Vehicle is not MyVehicle veh || veh == null || veh.Driver != player)
            {
                player.SendMessage(Models.MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            var vehiclePrice = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.Vehicle.Model.ToLower());

            if (staff)
            {
                if (!veh.Vehicle.FactionId.HasValue)
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

            var realMods = JsonSerializer.Deserialize<List<VehicleMod>>(veh.Vehicle.ModsJSON);

            var vehicleTuning = new VehicleTuning();
            if (player.VehicleTuning?.VehicleId == veh.Vehicle.Id)
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
                                ((Global.Prices.FirstOrDefault(y => y.Type == PriceType.Tuning && y.Name.ToLower() == x.ToString().ToLower())?.Value ?? 0)
                                / 100))),
                            Current = realMods.FirstOrDefault(y => y.Type == (byte)x)?.Id ?? 0,
                            Selected = realMods.FirstOrDefault(y => y.Type == (byte)x)?.Id ?? 0,
                        }).Where(x => x.ModsCount > 0 && (x.UnitaryValue > 0 || staff)).ToList(),
                    RepairValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "repair")?.Value ?? 0) / 100))),
                    WheelType = veh.Vehicle.WheelType,
                    WheelVariation = veh.Vehicle.WheelVariation,
                    WheelColor = veh.Vehicle.WheelColor,
                    WheelValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "wheel")?.Value ?? 0) / 100))),
                    Color1 = $"#{veh.Vehicle.Color1R:X2}{veh.Vehicle.Color1G:X2}{veh.Vehicle.Color1B:X2}",
                    Color2 = $"#{veh.Vehicle.Color2R:X2}{veh.Vehicle.Color2G:X2}{veh.Vehicle.Color2B:X2}",
                    ColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "color")?.Value ?? 0) / 100))),
                    NeonColor = $"#{veh.Vehicle.NeonColorR:X2}{veh.Vehicle.NeonColorG:X2}{veh.Vehicle.NeonColorB:X2}",
                    NeonLeft = Convert.ToByte(veh.Vehicle.NeonLeft),
                    NeonRight = Convert.ToByte(veh.Vehicle.NeonRight),
                    NeonFront = Convert.ToByte(veh.Vehicle.NeonFront),
                    NeonBack = Convert.ToByte(veh.Vehicle.NeonBack),
                    NeonValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "neon")?.Value ?? 0) / 100))),
                    HeadlightColor = veh.Vehicle.HeadlightColor,
                    LightsMultiplier = veh.Vehicle.LightsMultiplier,
                    XenonColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "xenoncolor")?.Value ?? 0) / 100))),
                    WindowTint = veh.Vehicle.WindowTint,
                    WindowTintValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "insulfilm")?.Value ?? 0) / 100))),
                    TireSmokeColor = $"#{veh.Vehicle.TireSmokeColorR:X2}{veh.Vehicle.TireSmokeColorG:X2}{veh.Vehicle.TireSmokeColorB:X2}",
                    TireSmokeColorValue = Convert.ToInt32(Math.Abs((vehiclePrice?.Value ?? 0) * ((Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "tiresmoke")?.Value ?? 0) / 100))),
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

            player.Emit("VehicleTuning",
                JsonSerializer.Serialize(vehicleTuning, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                }));
        }

        public static async Task CMDVenderVeiculoConcessionaria(MyPlayer player, int id, bool confirm)
        {
            if (Global.Vehicles.Any(x => x.Vehicle.Id == id))
            {
                player.SendMessage(Models.MessageType.Error, $"Veículo {id} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(Models.MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (veh.FactionGift)
            {
                player.SendMessage(Models.MessageType.Error, "Você não pode vender este veículo pois é um benefício da facção.");
                return;
            }

            var price = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.Model.ToLower());
            if (price == null)
            {
                player.SendMessage(Models.MessageType.Error, "Preço do veículo não foi encontrado.");
                return;
            }

            var dealership = Global.Dealerships.FirstOrDefault(x => x.PriceType == price.Type);
            if (dealership == null || player.Position.Distance(dealership.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(Models.MessageType.Error, $"Você não está na concessionária que vende este veículo.");
                return;
            }

            var value = Convert.ToInt32(price.Value / 2);

            if (confirm)
            {
                var res = await player.GiveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = value
                });

                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.SendMessage(Models.MessageType.Error, res);
                    return;
                }

                veh.Sold = true;
                context.Vehicles.Update(veh);
                await context.SaveChangesAsync();

                player.SendMessage(Models.MessageType.Success, $"Você vendeu seu veículo {veh.Model.ToUpper()} ({veh.Plate.ToUpper()}) [{veh.Id}] para a concessionária por ${value:N0}.");
                await player.GravarLog(LogType.Venda, $"/vvenderconce {veh.Id} {value}", null);
            }
            else
            {
                player.TargetConfirmation = new int[] { id };
                player.ShowConfirm("Confirmar Venda", $"Confirma vender o veículo {veh.Model.ToUpper()} para a concessionária por ${value:N0}?", "VenderVeiculo");
            }
        }

        public static async Task CMDVenderPropriedadeGoverno(MyPlayer player, bool confirm)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(Models.MessageType.Error, "Você não está próximo de uma propriedade sua.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            var value = Convert.ToInt32(prop.Value / 2);

            if (confirm)
            {
                var res = await player.GiveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = value
                });

                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.SendMessage(Models.MessageType.Error, res);
                    return;
                }

                prop.CharacterId = null;
                prop.Locked = false;
                prop.ProtectionLevel = 0;
                prop.CreateIdentifier();

                await using var context = new DatabaseContext();
                context.Properties.Update(prop);
                await context.SaveChangesAsync();

                player.SendMessage(Models.MessageType.Success, $"Você vendeu sua propriedade {prop.Id} para o governo por ${value:N0}.");
                await player.GravarLog(LogType.Venda, $"/pvendergoverno {prop.Id} {value}", null);
            }
            else
            {
                player.ShowConfirm("Confirmar Venda", $"Confirma vender a propriedade {prop.Id} para o governo por ${value:N0}?", "VenderPropriedade");
            }
        }
        #endregion Commands

        #region Staff
        public static string GetDoorsHTML()
        {
            var html = string.Empty;
            if (!Global.Doors.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há portas criadas.</td></tr>";
            }
            else
            {
                foreach (var door in Global.Doors.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{door.Id}</td>
                        <td>{door.Name}</td>
                        <td>{door.Hash}</td>
                        <td>X: {door.PosX} | Y: {door.PosY} | Z: {door.PosZ}</td>
                        <td>{door.FactionId}</td>
                        <td>{door.CompanyId}</td>
                        <td class='text-center'>{(door.Locked ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{door.Id}' type='hidden' value='{JsonSerializer.Serialize(door)}' />
                            <button onclick='goto({door.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({door.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {door.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static async Task<string> GetBansJSON()
        {
            await using var context = new DatabaseContext();
            var banishments = (await context.Banishments
                .Include(x => x.Character)
                .Include(x => x.User)
                .Include(x => x.StaffUser)
                .ToListAsync())
                .OrderByDescending(x => x.ExpirationDate)
                .Select(x => new
                {
                    x.Id,
                    Date = x.Date.ToString(),
                    ExpirationDate = x.ExpirationDate.HasValue ? x.ExpirationDate?.ToString() : "Permanente",
                    x.Reason,
                    Character = $"{x.Character.Name} [{x.CharacterId}]",
                    User = x.UserId.HasValue ? $"{x.User.Name} [{x.UserId}]" : string.Empty,
                    UserStaff = $"{x.StaffUser.Name} [{x.StaffUserId}]",
                });

            return JsonSerializer.Serialize(banishments);
        }

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
            return JsonSerializer.Serialize(sos);
        }

        public static string GetPricesHTML()
        {
            var html = string.Empty;
            if (!Global.Prices.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há preços criados.</td></tr>";
            }
            else
            {
                foreach (var price in Global.Prices.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{price.Id}</td>
                        <td>{GetEnumDisplay(price.Type)}</td>
                        <td>{price.Name}</td>
                        <td>{(price.Type == PriceType.Tuning ? $"{price.Value:N}%" : $"${price.Value:N0}")}</td>
                        <td class='text-center'>
                            <input id='json{price.Id}' type='hidden' value='{JsonSerializer.Serialize(price)}' />
                            <button onclick='editar({price.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {price.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetFactionsHTML()
        {
            var html = string.Empty;
            if (!Global.Factions.Any())
            {
                html = "<tr><td class='text-center' colspan='7'>Não há facções criadas.</td></tr>";
            }
            else
            {
                foreach (var faction in Global.Factions.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{faction.Id}</td>
                        <td>{faction.Name}</td>
                        <td>{GetEnumDisplay(faction.Type)}</td>
                        <td><span style='color:#{faction.Color}'>#{faction.Color}</span></td>
                        <td><span style='color:#{faction.ChatColor}'>#{faction.ChatColor}</span></td>
                        <td>{faction.Slots}</td>
                        <td class='text-center'>
                            <input id='json{faction.Id}' type='hidden' value='{JsonSerializer.Serialize(faction)}' />
                            <button onclick='editar({faction.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='ranks({faction.Id})' type='button' class='btn btn-dark btn-sm'>RANKS</button>
                            <button onclick='members({faction.Id})' type='button' class='btn btn-dark btn-sm'>MEMBROS</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetFactionRanksHTML(int factionId)
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
                            <input id='jsonRank{rank.Id}' type='hidden' value='{JsonSerializer.Serialize(rank)}' />
                            <button onclick='editRank({rank.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='removeRank(this, {rank.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static async Task<string> GetFactionMembersHTML(int factionId)
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
                    OnlineCharacter = Global.Players.FirstOrDefault(y => y.Character.Id == x.Id),
                })
                .OrderByDescending(x => x.OnlineCharacter?.Character?.Id > 0)
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
                            <input id='jsonMember{member.Character.Id}' type='hidden' value='{JsonSerializer.Serialize(new { member.Character.Name, member.Character.Badge, member.Character.FactionRankId, member.Character.FactionFlagsJSON })}' />
                            <button onclick='editMember({member.Character.Id})' type='button' class='btn btn-dark btn-sm editMember'>EDITAR</button>
                            <button onclick='removeMember({member.Character.Id}, `{member.Character.Name}`)' type='button' class='btn btn-danger btn-sm removeMember'>EXPULSAR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static string GetFactionsArmoriesHTML()
        {
            var html = string.Empty;
            if (!Global.FactionsArmories.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há arsenais criados.</td></tr>";
            }
            else
            {
                foreach (var factionArmory in Global.FactionsArmories.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{factionArmory.Id}</td>
                        <td>{factionArmory.FactionId}</td>
                        <td>X: {factionArmory.PosX} | Y: {factionArmory.PosY} | Z: {factionArmory.PosZ}</td>
                        <td>{factionArmory.Dimension}</td>
                        <td class='text-center'>
                            <input id='json{factionArmory.Id}' type='hidden' value='{JsonSerializer.Serialize(factionArmory)}' />
                            <button onclick='goto({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editWeapons({factionArmory.Id})' type='button' class='btn btn-dark btn-sm'>ARMAS</button>
                            <button onclick='remove(this, {factionArmory.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetFactionArmoriesWeaponsHTML(int factionArmoryId, bool staff)
        {
            var html = string.Empty;
            var factionsArmoriesWeapons = Global.FactionsArmoriesWeapons.Where(x => x.FactionArmoryId == factionArmoryId);
            if (!factionsArmoriesWeapons.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há armas criadas.</td></tr>";
            }
            else
            {
                var factionArmory = Global.FactionsArmories.FirstOrDefault(x => x.Id == factionArmoryId);
                var faction = Global.Factions.FirstOrDefault(x => x.Id == factionArmory.FactionId);

                foreach (var factionArmoryWeapon in factionsArmoriesWeapons)
                {
                    var realComponents = new List<string>();
                    foreach (var component in JsonSerializer.Deserialize<List<uint>>(factionArmoryWeapon.ComponentsJSON))
                    {
                        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Hash == component && x.Weapon == factionArmoryWeapon.Weapon);
                        if (comp != null)
                            realComponents.Add(comp.Name);
                    }

                    var htmlOptions = string.Empty;
                    if (staff)
                        htmlOptions = $@"<input id='json{factionArmoryWeapon.Id}' type='hidden' value='{JsonSerializer.Serialize(new
                        {
                            Weapon = factionArmoryWeapon.Weapon.ToString(),
                            factionArmoryWeapon.Ammo,
                            factionArmoryWeapon.Quantity,
                            factionArmoryWeapon.TintIndex,
                            Components = realComponents,
                        })}' />
                            <button onclick='edit({factionArmoryWeapon.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {factionArmoryWeapon.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>";
                    else
                        htmlOptions = $@"<button onclick='equip({factionArmoryWeapon.Id})' type='button' class='btn btn-dark btn-sm'>EQUIPAR</button>";

                    html += $@"<tr class='pesquisaitem'>
                        <td>{factionArmoryWeapon.Id}</td>
                        <td>{factionArmoryWeapon.Weapon}</td>
                        <td>{factionArmoryWeapon.Ammo}</td>
                        <td>{factionArmoryWeapon.Quantity}</td>
                        <td>{factionArmoryWeapon.TintIndex}</td>
                        {(!faction.Government && !staff ? $"<td>{Global.Prices.FirstOrDefault(y => y.Type == PriceType.Armas && y.Name.ToLower() == factionArmoryWeapon.Weapon.ToString().ToLower())?.Value ?? 0:N0}</td>" : string.Empty)}
                        <td>{string.Join(", ", realComponents)}</td>
                        <td class='text-center'>
                            {htmlOptions}
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static string GetPropertiesHTML()
        {
            var html = string.Empty;
            if (!Global.Properties.Any())
            {
                html = "<tr><td class='text-center' colspan='7'>Não há propriedades criadas.</td></tr>";
            }
            else
            {
                foreach (var property in Global.Properties.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{property.Id}</td>
                        <td>{GetEnumDisplay(property.Interior)} [{(byte)property.Interior}]</td>
                        <td>{property.Address}</td>
                        <td>{property.Value:N0}</td>
                        <td>{property.Dimension}</td>
                        <td>X: {property.EntrancePosX} | Y: {property.EntrancePosY} | Z: {property.EntrancePosZ}</td>
                        <td class='text-center'>
                            <input id='json{property.Id}' type='hidden' value='{JsonSerializer.Serialize(property)}' />
                            <button onclick='ir({property.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='editar({property.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {property.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetSpotsHTML()
        {
            var html = string.Empty;
            if (!Global.Spots.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há pontos criados.</td></tr>";
            }
            else
            {
                foreach (var spot in Global.Spots.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{spot.Id}</td>
                        <td>{GetEnumDisplay(spot.Type)}</td>
                        <td>X: {spot.PosX} | Y: {spot.PosY} | Z: {spot.PosZ}</td>
                        <td>X: {spot.AuxiliarPosX} | Y: {spot.AuxiliarPosY} | Z: {spot.AuxiliarPosZ}</td>
                        <td class='text-center'>
                            <input id='json{spot.Id}' type='hidden' value='{JsonSerializer.Serialize(spot)}' />
                            <button onclick='ir({spot.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='editar({spot.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {spot.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetBlipsHTML()
        {
            var html = string.Empty;
            if (!Global.Blips.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há blips criados.</td></tr>";
            }
            else
            {
                foreach (var blip in Global.Blips.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{blip.Id}</td>
                        <td>{blip.Name}</td>
                        <td>{blip.Type}</td>
                        <td>{blip.Color}</td>
                        <td>X: {blip.PosX} | Y: {blip.PosY} | Z: {blip.PosZ}</td>
                        <td class='text-center'>
                            <input id='json{blip.Id}' type='hidden' value='{JsonSerializer.Serialize(blip)}' />
                            <button onclick='ir({blip.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='editar({blip.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='excluir(this, {blip.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static async Task<string> GetVehiclesHTML()
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var vehicles = await context.Vehicles
                .Where(x => !x.Sold && (
                    x.Job != CharacterJob.None
                    || x.FactionId.HasValue
                    || (x.CharacterId.HasValue && x.FactionGift)))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            if (!vehicles.Any())
            {
                html = "<tr><td class='text-center' colspan='6'>Não há veículos criados para facções, aluguel de empregos ou benefícios de personagens de facções.</td></tr>";
            }
            else
            {
                foreach (var vehicle in vehicles)
                    html += $@"<tr class='pesquisaitem'>
                        <td>{vehicle.Id}</td>
                        <td>{vehicle.Model}</td>
                        <td>{GetEnumDisplay(vehicle.Job)}</td>
                        <td>{vehicle.FactionId}</td>
                        <td>{vehicle.CharacterId}</td>
                        <td class='text-center'>
                            <input id='json{vehicle.Id}' type='hidden' value='{JsonSerializer.Serialize(vehicle)}' />
                            <button onclick='edit({vehicle.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {vehicle.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetInfosHTML(int? userId)
        {
            var html = string.Empty;
            var infos = Global.Infos;
            if (userId.HasValue)
                infos = infos.Where(x => x.UserId == userId).ToList();

            if (!infos.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há informações criadas.</td></tr>";
            }
            else
            {
                foreach (var info in infos.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{info.Id}</td>
                        <td>{info.Date}</td>
                        <td>{info.ExpirationDate}</td>
                        <td>{info.User.Name} [{info.UserId}]</td>
                        <td>X: {info.PosX} | Y: {info.PosY} | Z: {info.PosZ}</td>
                        <td>{info.Dimension}</td>
                        <td>{info.Message}</td>
                        <td class='text-center'>
                            {(!userId.HasValue ? $"<button onclick='goto({info.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>" : string.Empty)}
                            <button onclick='remove(this, {info.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetFactionsDrugsHousesHTML()
        {
            var html = string.Empty;
            if (!Global.FactionsDrugsHouses.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há drug houses criadas.</td></tr>";
            }
            else
            {
                foreach (var factionDrugHouse in Global.FactionsDrugsHouses.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{factionDrugHouse.Id}</td>
                        <td>{factionDrugHouse.FactionId}</td>
                        <td>X: {factionDrugHouse.PosX} | Y: {factionDrugHouse.PosY} | Z: {factionDrugHouse.PosZ}</td>
                        <td>{factionDrugHouse.Dimension}</td>
                        <td class='text-center'>
                            <input id='json{factionDrugHouse.Id}' type='hidden' value='{JsonSerializer.Serialize(factionDrugHouse)}' />
                            <button onclick='goto({factionDrugHouse.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({factionDrugHouse.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editItems({factionDrugHouse.Id})' type='button' class='btn btn-dark btn-sm'>ITENS</button>
                            <button onclick='remove(this, {factionDrugHouse.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetFactionsDrugsHousesItemsHTML(int factionDrugHouseId, bool staff)
        {
            var html = string.Empty;
            var items = Global.FactionsDrugsHousesItems.Where(x => x.FactionDrugHouseId == factionDrugHouseId);
            if (!items.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há itens criados.</td></tr>";
            }
            else
            {
                foreach (var item in items)
                {
                    var htmlOptions = string.Empty;
                    if (staff)
                        htmlOptions = $@"<input id='json{item.Id}' type='hidden' value='{JsonSerializer.Serialize(new
                        {
                            ItemCategory = item.ItemCategory.ToString(),
                            item.Quantity
                        })}' />
                            <button onclick='edit({item.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {item.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>";
                    else
                        htmlOptions = $@"<button onclick='buy({item.Id})' type='button' class='btn btn-dark btn-sm'>COMPRAR</button>";

                    html += $@"<tr class='pesquisaitem'>
                        <td>{item.Id}</td>
                        <td>{GetEnumDisplay(item.ItemCategory)}</td>
                        <td>{item.Quantity}</td>
                        {(!staff ? $"<td>{Global.Prices.FirstOrDefault(y => y.Type == PriceType.Drogas && y.Name.ToLower() == item.ItemCategory.ToString().ToLower())?.Value ?? 0:N0}</td>" : string.Empty)}
                        <td class='text-center'>
                            {htmlOptions}
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static string GetCrackDensHTML()
        {
            var html = string.Empty;
            if (!Global.CrackDens.Any())
            {
                html = "<tr><td class='text-center' colspan='9'>Não há bocas de fumo criadas.</td></tr>";
            }
            else
            {
                foreach (var crackDen in Global.CrackDens.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{crackDen.Id}</td>
                        <td>X: {crackDen.PosX} | Y: {crackDen.PosY} | Z: {crackDen.PosZ}</td>
                        <td>{crackDen.Dimension}</td>
                        <td>{crackDen.OnlinePoliceOfficers}</td>
                        <td>{crackDen.CooldownQuantityLimit}</td>
                        <td>{crackDen.CooldownHours}</td>
                        <td>{crackDen.CooldownDate}</td>
                        <td>{crackDen.Quantity}</td>
                        <td class='text-center'>
                            <input id='json{crackDen.Id}' type='hidden' value='{JsonSerializer.Serialize(crackDen)}' />
                            <button onclick='goto({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editItems({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>ITENS</button>
                            <button onclick='revokeCooldown({crackDen.Id})' type='button' class='btn btn-dark btn-sm'>REVOGAR COOLDOWN</button>
                            <button onclick='remove(this, {crackDen.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetCrackDensItemsHTML(int crackDenId, bool staff)
        {
            var html = string.Empty;
            var items = Global.CrackDensItems.Where(x => x.CrackDenId == crackDenId);
            if (!items.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há itens criados.</td></tr>";
            }
            else
            {
                foreach (var item in items)
                {
                    var htmlOptions = string.Empty;
                    if (staff)
                        htmlOptions = $@"<input id='json{item.Id}' type='hidden' value='{JsonSerializer.Serialize(new
                        {
                            ItemCategory = item.ItemCategory.ToString(),
                            item.Value
                        })}' />
                            <button onclick='edit({item.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {item.Id})'   type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>";
                    else
                        htmlOptions = $@"<button onclick='sell({item.Id})' type='button' class='btn btn-dark btn-sm'>VENDER</button>";

                    html += $@"<tr class='pesquisaitem'>
                        <td>{item.Id}</td>
                        <td>{GetEnumDisplay(item.ItemCategory)}</td>
                        <td>${item.Value:N0}</td>
                        <td class='text-center'>
                            {htmlOptions}
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static string GetTruckerLocationsHTML()
        {
            var html = string.Empty;
            if (!Global.TruckerLocations.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há localizações de caminhoneiros criadas.</td></tr>";
            }
            else
            {
                foreach (var truckerLocation in Global.TruckerLocations.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{truckerLocation.Id}</td>
                        <td>{truckerLocation.Name}</td>
                        <td>X: {truckerLocation.PosX} | Y: {truckerLocation.PosY} | Z: {truckerLocation.PosZ}</td>
                        <td>${truckerLocation.DeliveryValue:N0}</td>
                        <td>{truckerLocation.LoadWaitTime}</td>
                        <td>{truckerLocation.UnloadWaitTime}</td>
                        <td>{string.Join(", ", truckerLocation.AllowedVehicles)}</td>
                        <td class='text-center'>
                            <input id='json{truckerLocation.Id}' type='hidden' value='{JsonSerializer.Serialize(truckerLocation)}' />
                            <button onclick='goto({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='editDeliveries({truckerLocation.Id})' type='button' class='btn btn-dark btn-sm'>ENTREGAS</button>
                            <button onclick='remove(this, {truckerLocation.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetTruckerLocationsDeliverysHTML(int truckerLocationId)
        {
            var html = string.Empty;
            var truckerLocationDeliveries = Global.TruckerLocationsDeliveries.Where(x => x.TruckerLocationId == truckerLocationId);
            if (!truckerLocationDeliveries.Any())
            {
                html = "<tr><td class='text-center' colspan='3'>Não há entregas criadas.</td></tr>";
            }
            else
            {
                foreach (var truckerLocationDelivery in truckerLocationDeliveries)
                {
                    html += $@"<tr class='pesquisaitem'>
                        <td>{truckerLocationDelivery.Id}</td>
                        <td>X: {truckerLocationDelivery.PosX} | Y: {truckerLocationDelivery.PosY} | Z: {truckerLocationDelivery.PosZ}</td>
                        <td class='text-center'>
                            <input id='json{truckerLocationDelivery.Id}' type='hidden' value='{JsonSerializer.Serialize(truckerLocationDelivery)}' />
                            <button onclick='goto({truckerLocationDelivery.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({truckerLocationDelivery.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {truckerLocationDelivery.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static string GetFurnituresHTML()
        {
            var html = string.Empty;
            if (!Global.Furnitures.Any())
            {
                html = "<tr><td class='text-center' colspan='6'>Não há mobílias criadas.</td></tr>";
            }
            else
            {
                foreach (var furniture in Global.Furnitures.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{furniture.Id}</td>
                        <td>{furniture.Category}</td>
                        <td>{furniture.Name}</td>
                        <td>{furniture.Model}</td>
                        <td>${furniture.Value:N0}</td>
                        <td class='text-center'>
                            <input id='json{furniture.Id}' type='hidden' value='{JsonSerializer.Serialize(furniture)}' />
                            <button onclick='edit({furniture.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {furniture.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetAnimationsHTML()
        {
            var html = string.Empty;
            if (!Global.Animations.Any())
            {
                html = "<tr><td class='text-center' colspan='8'>Não há animações criadas.</td></tr>";
            }
            else
            {
                foreach (var animation in Global.Animations.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{animation.Id}</td>
                        <td>{animation.Display}</td>
                        <td>{animation.Dictionary}</td>
                        <td>{animation.Name}</td>
                        <td>{animation.Flag}</td>
                        <td>{animation.Duration}</td>
                        <td class='text-center'>{(animation.Vehicle ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{animation.Id}' type='hidden' value='{JsonSerializer.Serialize(animation)}' />
                            <button onclick='edit({animation.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            <button onclick='remove(this, {animation.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetCompaniesHTML()
        {
            var html = string.Empty;
            if (!Global.Companies.Any())
            {
                html = "<tr><td class='text-center' colspan='11'>Não há empresas criadas.</td></tr>";
            }
            else
            {
                foreach (var company in Global.Companies.OrderByDescending(x => x.Id))
                    html += $@"<tr class='pesquisaitem'>
                        <td>{company.Id}</td>
                        <td>{company.Name}</td>
                        <td>X: {company.PosX} | Y: {company.PosY} | Z: {company.PosZ}</td>
                        <td>${company.WeekRentValue:N0}</td>
                        <td>{company.RentPaymentDate}</td>
                        <td>{company.CharacterId}</td>
                        <td><span style='color:#{company.Color}'>#{company.Color}</span></td>
                        <td>{company.BlipType}</td>
                        <td>{company.BlipColor}</td>
                        <td class='text-center'>{(company.Open ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{company.Id}' type='hidden' value='{JsonSerializer.Serialize(company)}' />
                            <button onclick='goto({company.Id})' type='button' class='btn btn-dark btn-sm'>IR</button>
                            <button onclick='edit({company.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>
                            {(company.CharacterId.HasValue ? $"<button onclick='removeOwner(this, {company.Id})' type='button' class='btn btn-dark btn-sm'>REMOVER DONO</button>" : string.Empty)}
                            <button onclick='remove(this, {company.Id})' type='button' class='btn btn-danger btn-sm'>EXCLUIR</button>
                        </td>
                    </tr>";
            }
            return html;
        }

        public static string GetCompaniesByCharacterHTML(MyPlayer player)
        {
            var html = string.Empty;
            if (!player.Companies.Any())
            {
                html = "<tr><td class='text-center' colspan='11'>Não há empresas criadas.</td></tr>";
            }
            else
            {
                foreach (var company in player.Companies.OrderByDescending(x => x.Id))
                {
                    var companyCharacter = company.Characters.FirstOrDefault(x => x.CharacterId == player.Character.Id);

                    var htmlOptions = string.Empty;

                    if (company.CharacterId == player.Character.Id || (companyCharacter?.Flags?.Contains(CompanyFlag.Open) ?? false))
                    {
                        htmlOptions += $@"<button onclick='openClose({company.Id})' type='button' class='btn btn-dark btn-sm'>ABRIR/FECHAR</button>";

                        if (company.Open)
                            htmlOptions += $@" <button onclick='announce({company.Id})' type='button' class='btn btn-dark btn-sm'>ANUNCIAR</button>";
                    }

                    html += $@"<tr class='pesquisaitem'>
                        <td>{company.Id}</td>
                        <td>{company.Name}</td>
                        <td>${company.WeekRentValue:N0}</td>
                        <td>{company.RentPaymentDate}</td>
                        <td><span style='color:#{company.Color}'>#{company.Color}</span></td>
                        <td>{company.BlipType}</td>
                        <td>{company.BlipColor}</td>
                        <td class='text-center'>{(company.Open ? "SIM" : "NÃO")}</td>
                        <td class='text-center'>
                            <input id='json{company.Id}' type='hidden' value='{JsonSerializer.Serialize(company)}' />
                            {(company.CharacterId == player.Character.Id ? $"<button onclick='edit({company.Id})' type='button' class='btn btn-dark btn-sm'>EDITAR</button>" : string.Empty)}
                            <button onclick='employees({company.Id})' type='button' class='btn btn-dark btn-sm'>FUNCIONÁRIOS</button>
                            {htmlOptions}
                        </td>
                    </tr>";
                }
            }
            return html;
        }

        public static async Task<string> GetCompanyCharactersHTML(int companyId)
        {
            await using var context = new DatabaseContext();
            var characters = (await context.CompaniesCharacters
                .Where(x => x.CompanyId == companyId)
                .Include(x => x.Character)
                .ThenInclude(x => x.User)
                .ToListAsync())
                .Select(x => new
                {
                    CompanyCharacter = x,
                    OnlineCharacter = Global.Players.FirstOrDefault(y => y.Character.Id == x.CharacterId),
                })
                .OrderByDescending(x => x.OnlineCharacter?.Character?.Id > 0)
                .ThenBy(x => x.CompanyCharacter.Character.Name);

            var html = string.Empty;
            if (!characters.Any())
            {
                html = "<tr><td class='text-center' colspan='5'>Não há funcionários na empresa.</td></tr>";
            }
            else
            {
                foreach (var character in characters)
                {
                    var online = character.OnlineCharacter != null ?
                        $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>ONLINE</span>"
                        :
                        $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>OFFLINE</span>";

                    html += $@"<tr class='pesquisaitem'>
                        <td>{character.CompanyCharacter.Character.Name} [{character.CompanyCharacter.Character.Id}]</td>
                        <td>{character.CompanyCharacter.Character.User.Name} [{character.CompanyCharacter.Character.UserId}]</td>
                        <td>{character.CompanyCharacter.Character.LastAccessDate}</td>
                        <td class='text-center'>{online}</td>
                        <td class='text-center tdOptions'>
                            <input id='jsonMember{character.CompanyCharacter.Character.Id}' type='hidden' value='{JsonSerializer.Serialize(new { character.CompanyCharacter.Character.Name, character.CompanyCharacter.FlagsJSON })}' />
                            <button onclick='edit({character.CompanyCharacter.Character.Id})' type='button' class='btn btn-dark btn-sm editMember'>EDITAR</button>
                            <button onclick='remove(this, {character.CompanyCharacter.Character.Id})' type='button' class='btn btn-danger btn-sm removeMember'>EXPULSAR</button>
                        </td>
                    </tr>";
                }
            }
            return html;
        }
        #endregion Staff

        #region MDC
        public async static Task<Tuple<string, string>> GetWantedsHTML()
        {
            var htmlAPB = string.Empty;
            var htmlBOLO = string.Empty;
            await using var context = new DatabaseContext();
            var wanted = await context.Wanted
                .Where(x => !x.DeletedDate.HasValue)
                .Include(x => x.PoliceOfficerCharacter)
                .Include(x => x.WantedCharacter)
                .Include(x => x.WantedVehicle)
                .ToListAsync();

            var apbs = wanted.Where(x => x.WantedCharacterId.HasValue);
            var bolos = wanted.Where(x => x.WantedVehicleId.HasValue);

            if (!apbs.Any())
            {
                htmlAPB = $@"<div class='alert alert-danger'>Nenhum APB encontrado.</div>";
            }
            else
            {
                htmlAPB = $@"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Código</th>
                                    <th>Data</th>
                                    <th>Policial</th>
                                    <th>Procurado</th>
                                    <th>Motivo</th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in apbs)
                    htmlAPB += $@"<tr>
                            <td>{x.Id}</td>
                            <td>{x.Date}</td>
                            <td>{x.PoliceOfficerCharacter.Name}</td>
                            <td>{x.WantedCharacter.Name}</td>
                            <td>{x.Reason}</td>
                            </tr>";

                htmlAPB += @"</tbody>
                        </table>
                        </div>";
            }

            if (!bolos.Any())
            {
                htmlBOLO = $@"<div class='alert alert-danger'>Nenhum BOLO encontrado.</div>";
            }
            else
            {
                htmlBOLO = $@"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Código</th>
                                    <th>Data</th>
                                    <th>Policial</th>
                                    <th>Veículo</th>
                                    <th>Motivo</th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in bolos)
                    htmlBOLO += $@"<tr>
                            <td>{x.Id}</td>
                            <td>{x.Date}</td>
                            <td>{x.PoliceOfficerCharacter.Name}</td>
                            <td>{x.WantedVehicle.Model} {x.WantedVehicle.Plate}</td>
                            <td>{x.Reason}</td>
                            </tr>";

                htmlBOLO += @"</tbody>
                        </table>
                        </div>";
            }

            return new Tuple<string, string>(htmlAPB, htmlBOLO);
        }

        public static string GetFactionsUnitsHTML(MyPlayer player)
        {
            var html = string.Empty;
            var factionUnits = Global.FactionsUnits.Where(x => x.FactionId == player.Character.FactionId);
            if (!factionUnits.Any())
            {
                html = $@"<div class='alert alert-danger'>Nenhuma unidade em serviço.</div>";
            }
            else
            {
                html = @"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Nome</th>
                                    <th>Numeração</th>
                                    <th>Placa</th>
                                    <th>Início</th>
                                    <th>Ocupantes</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in factionUnits)
                {
                    var characters = new List<Character>
                    {
                        x.Character
                    };
                    characters.AddRange(x.Characters.Select(y => y.Character));

                    html += $@"<tr>
                            <td>{x.Name}</td>
                            <td>{x.Description}</td>
                            <td>{x.Plate}</td>
                            <td>{x.InitialDate}</td>
                            <td>{string.Join(", ", characters.Select(y => y.Name))}</td>
                            <td class='text-center'>{(x.CharacterId == player.Character.Id ? $"<button id='btn-encerrarunidade{x.Id}' class='btn btn-dark btn-xs' onclick='encerrarUnidade({x.Id});'>Encerrar</button>" : string.Empty)}</td>
                            </tr>";
                }

                html += @"</tbody>
                        </table>
                        </div>";
            }
            return html;
        }

        public static async Task<string> GetSearchPersonHTML(string search)
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var character = await context.Characters.Where(x => x.Name.ToLower() == search.ToLower())
                .Include(x => x.PoliceOfficerBlockedDriverLicenseCharacter)
                .FirstOrDefaultAsync();
            if (character == null)
            {
                html = $@"<div class='alert alert-danger'>Nenhuma pessoa foi encontrada com o nome <strong>{search}</strong>.</div>";
            }
            else
            {
                var properties = Global.Properties
                    .Where(x => x.CharacterId == character.Id)
                    .ToList();

                var vehicles = await context.Vehicles
                    .Where(x => x.CharacterId == character.Id && !x.Sold)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var fines = await context.Fines
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var jails = await context.Jails
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var confiscations = await context.Confiscations
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .Include(x => x.Items)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var job = GetEnumDisplay(character.Job);
                if (character.FactionId > 0)
                {
                    var faction = Global.Factions.FirstOrDefault(x => x.Id == character.FactionId);
                    if (faction?.Type != FactionType.Criminal)
                        job = $"{Global.FactionsRanks.FirstOrDefault(x => x.Id == character.FactionRankId)?.Name ?? string.Empty} - {faction?.Name ?? string.Empty}";
                }

                var revokeDriverLicense = $@"<button onclick='revogarLicenca({character.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Revogar Licença de Motorista</button>";
                var driverLicense = $"<span class='label' style='background-color:{Global.SUCCESS_COLOR};'>VÁLIDA</span>";
                if (character.PoliceOfficerBlockedDriverLicenseCharacterId.HasValue)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>REVOGADA POR {character.PoliceOfficerBlockedDriverLicenseCharacter?.Name?.ToUpper() ?? string.Empty}</span>";
                    revokeDriverLicense = string.Empty;
                }
                else if (!character.DriverLicenseValidDate.HasValue)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>NÃO POSSUI</span>";
                    revokeDriverLicense = string.Empty;
                }
                else if (character.DriverLicenseValidDate?.Date < DateTime.Now.Date)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>VENCIDA</span>";
                    revokeDriverLicense = string.Empty;
                }

                var strBolo = string.Empty;
                var boloButton = $@"<button onclick='adicionarBOLO(1, {character.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Adicionar APB</button>";

                var bolo = await context.Wanted.Where(x => x.WantedCharacterId == character.Id && !x.DeletedDate.HasValue).Include(x => x.PoliceOfficerCharacter).FirstOrDefaultAsync();
                if (bolo != null)
                {
                    boloButton = $@"<button onclick='removerBOLO({bolo.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Remover APB</button>";
                    strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>APB</span> {bolo.Reason}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> em <strong>{bolo.Date}</strong>.</p>
                                </div>
                            </div>
                        </div>";
                }

                html = $@"<div class='row'>
                                <div class='col-md-6'>
                                    <h3>{character.Name}</h3>
                                </div>
                                <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                                    <button onclick='multar({character.Id}, ""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Multar</button>
                                    {boloButton}
                                    {revokeDriverLicense}
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-md-12'>
                                    <p>Data de Nascimento: <strong>{character.BirthdayDate.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - character.BirthdayDate).TotalDays / 365):N0} anos)</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Sexo: <strong>{GetEnumDisplay(character.Sex)}</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Emprego: <strong>{job}</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Licença de Motorista: {driverLicense}</p>
                                </div>
                            </div>
                            {strBolo}";

                if (!string.IsNullOrWhiteSpace(character.Image))
                    html += $"<img src='data:image/png;base64, {character.Image}' alt='headshot' />";

                if (properties.Count > 0)
                {
                    html += $@"<h4>Propriedades</h4>";
                    foreach (var x in properties)
                        html += $@"<p>Nº {x.Id} - {x.Address}</p>";
                }

                if (vehicles.Count > 0)
                {
                    html += $@"<h4>Veículos</h4>";
                    foreach (var x in vehicles)
                        html += $@"<p>Modelo: <strong>{x.Model.ToUpper()}</strong> Placa: <strong>{x.Plate}</strong></p>";
                }

                if (fines.Count > 0)
                {
                    html += $@"<h4>Multas</h4>";
                    foreach (var x in fines)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;
                        html += $@"<p>Data: <strong>{x.Date}</strong> Valor: <strong>${x.Value:N0}</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> Motivo: <strong>{x.Reason}</strong> Status: <strong>{(x.PaymentDate.HasValue ? $"<span class='label' style='background-color:{Global.SUCCESS_COLOR};'>PAGA</span>" : $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>PENDENTE</span>")}</strong> {details}</p>";
                    }
                }

                if (jails.Count > 0)
                {
                    html += $@"<h4>Prisões</h4>";
                    foreach (var x in jails)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;
                        html += $@"<p>Data: <strong>{x.Date}</strong> Tempo: <strong>{Convert.ToInt32((x.EndDate - x.Date).TotalMinutes)} minutos</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong></p> {details}</p>";
                    }
                }

                if (confiscations.Count > 0)
                {
                    html += $@"<h4>Confiscos</h4>";
                    foreach (var x in confiscations)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;

                        html += $@"<div class=""well"">Data: <strong>{x.Date}</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> Itens: <br/>";

                        foreach (var item in x.Items.Select(z => new ConfiscationItem(z)).ToList())
                            html += $"<p>Nome: <strong>{item.Name}</strong> | Quantidade: <strong>{item.Quantity:N0}</strong>{(!string.IsNullOrWhiteSpace(item.Extra) ? $" | Extra: <strong>{GetItemExtra(item).Replace("<br/>", ", ")}</strong>" : string.Empty)}<p/>";

                        html += $"<br/>{details}</div>";
                    }
                }
            }

            return html;
        }

        public static async Task<string> GetSearchVehicleHTML(string search)
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var vehicle = await context.Vehicles.Where(x => x.Plate.ToLower() == search.ToLower())
                .Include(x => x.Character)
                .Include(x => x.Faction)
                .FirstOrDefaultAsync();
            if (vehicle == null)
            {
                html = $@"<div class='alert alert-danger'>Nenhum veículo foi encontrado com a pesquisa <strong>{search}</strong>.</div>";
            }
            else
            {
                var strBolo = string.Empty;
                var boloButton = string.Empty;
                var owner = $"Emprego de {GetEnumDisplay(vehicle.Job)}";
                if (vehicle.CharacterId.HasValue)
                {
                    boloButton = $@"<button onclick='adicionarBOLO(2, {vehicle.Id},""{search}"");' type='button' class='btn btn-xs btn-dark'>Adicionar BOLO</button>";
                    if (vehicle.Sold)
                        owner = "CONCESSIONÁRIA";
                    else
                        owner = vehicle.Character.Name;

                    var bolo = await context.Wanted.Where(x => x.WantedVehicleId == vehicle.Id && !x.DeletedDate.HasValue)
                        .Include(x => x.PoliceOfficerCharacter)
                        .FirstOrDefaultAsync();
                    if (bolo != null)
                    {
                        boloButton = $@"<button onclick='removerBOLO({bolo.Id},""{search}"");' type='button' class='btn btn-xs btn-dark'>Remover BOLO</button>";
                        strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>BOLO</span> {bolo.Reason}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> em <strong>{bolo.Date}</strong>.</p>
                                </div>
                            </div>
                            </div>";
                    }
                }
                else if (vehicle.FactionId.HasValue)
                {
                    owner = vehicle.Faction.Name;
                }

                html += $@"<div class='row'>
                    <div class='col-md-6'>
                        <h3>{vehicle.Plate}</h3>
                    </div>
                    <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                        {boloButton}
                        {(vehicle.FactionId.HasValue || vehicle.Job != CharacterJob.None ? $"<button onclick='rastrearVeiculo({vehicle.Id});' type='button' class='btn btn-xs btn-dark'>Rastrear</button>" : string.Empty)}
                    </div>
                    </div>
                    <div class='row'>
                        <div class='col-md-12'>
                            <p>Modelo: <strong>{vehicle.Model.ToUpper()}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Proprietário: <strong>{owner}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Apreendido: <strong>{(vehicle.SeizedValue > 0 ? $"SIM (${vehicle.SeizedValue:N0})" : "NÃO")}</strong></p>
                        </div>
                    </div>
                    {strBolo}";
            }

            return html;
        }
        #endregion MDC

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

        public static async Task CMDLigar(MyPlayer player, string numberNameContact)
        {
            if (player.Cellphone == 0)
            {
                player.SendMessage(Models.MessageType.Error, Global.UNEQUIPPED_CELPPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(Models.MessageType.Error, Global.BLOCKED_CELLPHONE_ERROR_MESSAGE);
                return;
            }

            if (player.CellphoneItem.ModoAviao)
            {
                player.SendMessage(Models.MessageType.Error, "Seu celular está em modo avião.");
                return;
            }

            if (player.CellphoneCall.Numero > 0)
            {
                player.SendMessage(Models.MessageType.Error, "Você está em uma ligação.");
                return;
            }

            if (!uint.TryParse(numberNameContact, out uint cellphone) || cellphone == 0)
                cellphone = player.CellphoneItem.Contatos
                    .FirstOrDefault(x => x.Nome.ToLower().Contains(numberNameContact.ToLower()))?.Numero ?? 0;

            if (cellphone == player.Cellphone)
            {
                player.SendMessage(Models.MessageType.Error, "Você não pode ligar para você mesmo.");
                return;
            }

            player.EmergencyCallType = null;

            if (cellphone == Global.EMERGENCY_NUMBER)
            {
                player.CellphoneCall = new CellphoneItemCall
                {
                    Numero = cellphone,
                    Tipo = CellphoneCallType.Atendida,
                };
                player.SendMessage(Models.MessageType.None, $"[CELULAR] Você está ligando para {player.ObterNomeContato(cellphone)}.", Global.CELLPHONE_SECONDARY_COLOR);
                player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} diz: Central de emergência, deseja falar com polícia, bombeiros ou ambos?", Global.CELLPHONE_MAIN_COLOR);
                return;
            }

            if (cellphone == Global.TAXI_NUMBER)
            {
                player.CellphoneCall = new CellphoneItemCall
                {
                    Numero = cellphone,
                    Tipo = CellphoneCallType.Atendida,
                };

                player.SendMessage(Models.MessageType.None, $"[CELULAR] Você está ligando para {player.ObterNomeContato(cellphone)}.", Global.CELLPHONE_SECONDARY_COLOR);
                if (!Global.Players.Any(x => x.Character.Job == CharacterJob.TaxiDriver && x.OnDuty))
                {
                    await player.EndCellphoneCall();
                    player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} diz: Desculpe, não temos nenhum taxista em serviço no momento.", Global.CELLPHONE_MAIN_COLOR);
                    return;
                }

                player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} diz: Downtown Cab Company, para onde deseja ir?", Global.CELLPHONE_MAIN_COLOR);
                return;
            }

            if (cellphone == Global.MECHANIC_NUMBER)
            {
                player.CellphoneCall = new CellphoneItemCall
                {
                    Numero = cellphone,
                    Tipo = CellphoneCallType.Atendida,
                };

                player.SendMessage(Models.MessageType.None, $"[CELULAR] Você está ligando para {player.ObterNomeContato(cellphone)}.", Global.CELLPHONE_SECONDARY_COLOR);
                if (!Global.Players.Any(x => x.Character.Job == CharacterJob.Mechanic && x.OnDuty))
                {
                    await player.EndCellphoneCall();
                    player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} diz: Desculpe, não temos nenhum mecânico em serviço no momento.", Global.CELLPHONE_MAIN_COLOR);
                    return;
                }

                player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} diz: Central de Mecânicos, como podemos ajudar?", Global.CELLPHONE_MAIN_COLOR);
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Cellphone == cellphone && cellphone > 0);
            if (target == null || target.CellphoneItem.ModoAviao || target.CellphoneCall.Numero > 0)
            {
                player.SendMessage(Models.MessageType.None, $"[CELULAR] {player.ObterNomeContato(cellphone)} está indisponível.", Global.CELLPHONE_SECONDARY_COLOR);
                return;
            }

            player.CellphoneCall = new CellphoneItemCall
            {
                Numero = cellphone,
            };

            target.CellphoneCall = new CellphoneItemCall
            {
                Numero = player.Cellphone,
                DataInicio = player.CellphoneCall.DataInicio,
                Origem = false,
            };

            player.TimerCelularElapsedCount = 0;
            player.TimerCelular = new Timer(8000);
            player.TimerCelular.Elapsed += player.TimerCelular_Elapsed;
            player.TimerCelular.Start();
            player.SendMessage(Models.MessageType.None, $"[CELULAR] Você está ligando para {player.ObterNomeContato(cellphone)}.", Global.CELLPHONE_SECONDARY_COLOR);
            target.SendMessage(Models.MessageType.None, $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(player.Cellphone)}. (/atender ou /des)", Global.CELLPHONE_SECONDARY_COLOR);
            target.SendMessageToNearbyPlayers($"O celular de {target.ICName} está tocando.", MessageCategory.Do, 5, true);
        }

        public static List<CellphoneItemContact> GetDefaultsContacts()
        {
            return new List<CellphoneItemContact>
            {
                new CellphoneItemContact(Global.EMERGENCY_NUMBER, "Central de Emergência"),
                new CellphoneItemContact(Global.MECHANIC_NUMBER, "Central de Mecânicos"),
                new CellphoneItemContact(Global.TAXI_NUMBER, "Downtown Cab Company"),
            };
        }
        #endregion Cellphone
    }
}