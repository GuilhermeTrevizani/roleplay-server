using ExcelDataReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Testador
{
    class Program
    {
        class Base
        {
            public List<Opa> ClothData { get; set; }
        }

        class Opa
        {
            public int DrawableType { get; set; }
            public int Position { get; set; }
            public int TargetGender { get; set; }

            public List<Filho> Textures { get; set; }
        }

        class Filho
        {
            public int Position { get; set; }
        }

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        };


        static async Task Main(string[] args)
        {
            try
            {
                await UpdateServer();
            }
            catch (Exception ex)
            {
                var a = ex;
            }

            return;
            var con = new MySqlConnection("Server=localhost;Database=bdbackup;Uid=root;Password=159357");
            con.Open();
            var cmd = new MySqlCommand("SELECT Email from Usuarios where Codigo >= 272", con);
            var dr = cmd.ExecuteReader();
            var clienteSmtp = new SmtpClient("smtp.titan.email")
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("naoresponda@segundavida.com.br", "aDGi^O&deHoG"),
                Port = 587,
            };
            while (dr.Read())
            {
                var msg = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("naoresponda@segundavida.com.br", "Segunda Vida Roleplay"),
                    Subject = "Segunda Vida Roleplay está online!",
                    Body = "Nós sentimos sua falta... Segunda Vida Roleplay está online! Entre no nosso Discord e saiba mais: https://discord.gg/segundavidaroleplay",
                    BodyEncoding = Encoding.UTF8,
                };
                msg.To.Add(dr[0].ToString());
                clienteSmtp.Send(msg);
            }
            dr?.Close();
            dr?.Dispose();
            cmd?.Dispose();
            con?.Close();
            con?.Dispose();

            using (var stream = File.Open(@"C:\test.xlsx", FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    var category = "Cozinha";
                    var sql = string.Empty;

                    do
                    {
                        while (reader.Read())
                        {
                            var prop = reader.GetString(0);
                            if (!string.IsNullOrWhiteSpace(prop))
                            {
                                var name = reader.GetString(1);
                                var value = reader.GetDouble(2);

                                sql += $@"('{category}', '{name}', '{prop}', {value}), ";
                            }
                        }
                    } while (reader.NextResult());

                    sql = sql;

                    // 2. Use the AsDataSet extension method
                    //var result = reader.AsDataSet();

                    // The result of each spreadsheet is in result.Tables
                }
            }

            //var args2 = args;

            //var a1 = (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody); // 49
            //var a2 = (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl); // 33
            //var a3 = (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody); // 48
            //var a4 = (int)AnimationFlags.Loop; // 1
            //var a5 = (int)AnimationFlags.StopOnLastFrame; // 2

            //var files = Directory.GetFiles(@"C:\Projetos\SegundaVida\ALTVSERVER\resources\roleplayclient\inventory\img\1");
            //foreach (var file in files)
            //{
            //    File.Move(file, file.Replace(".webp", ".png"));
            //}

            var json = JsonSerializer.Deserialize<Base>(File.ReadAllText(@"C:\Projetos\SegundaVida\ALTVSERVER\sgv.durty-cloth.json"));
            var sb = new StringBuilder();
            var lastPosition = -1;
            var lastDrawableType = 0;
            // Gender: 0 Male | 1 Female
            foreach (var x in json.ClothData.Where(x => x.TargetGender == 1 && x.DrawableType != 2).OrderBy(x => x.DrawableType).ThenBy(x => x.Position))
            {
                if (lastPosition + 1 != x.Position)
                {
                    if (x.DrawableType == lastDrawableType)
                    {
                    }
                }

                lastPosition = x.Position;
                lastDrawableType = x.DrawableType;

                sb.AppendLine($"{{ \"component\": {x.DrawableType}, \"drawable\": {x.Position}, \"dlc\": \"mp_f_sgv\", \"tipoFaccao\": 0, \"maxTexture\": {x.Textures.Max(y => y.Position)} }},");
            }

            File.WriteAllText(@"C:\HomemDoKevorkians.txt", sb.ToString());

            //var text = File.ReadAllText(@"C:\EUP_MULHER_V3.txt");
            //text = text.Replace("drawable", "\"drawable\"");
            //text = text.Replace("dlc", "\"dlc\"");
            //text = text.Replace("tipoFaccao", "\"tipoFaccao\"");
            //text = text.Replace("maxTexture", "\"maxTexture\"");
            //text = text.Replace("'mp_m_eup'", "\"mp_m_eup\"");
            //text = text.Replace("'mp_f_eup'", "\"mp_f_eup\"");
            /*text = text.Replace(Environment.NewLine, string.Empty);
            text = text.Replace("    ", string.Empty);
            text = text.Replace(" ", string.Empty);*/

            //var a = JsonSerializer.Deserialize<List<ClotheAccessory>>(text);

            /*var aaaaa = Criptografar("123");
            var txt = "const accessories7Female = {";
            for (var i = 0; i <= 14; i++)
                txt += $"{Environment.NewLine}{i}: {{ drawable: {i}, tipoFaccao: 0 }},";
            txt += $"{Environment.NewLine}}};";*/
        }

        public class ClotheAccessory
        {
            public string DLC { get; set; }
            public int Drawable { get; set; }
            public int? TipoFaccao { get; set; }
            public byte? MaxTexture { get; set; }
        }

        /*private void MigrarPrecos()
        {
            var str = $@"dagger|2500
hatchet|500
battleaxe|500
knife|3000
machete|1500
stone_hatchet|1000
switchblade|1000
appistol|12000
ceramicpistol|2000
combatpistol|4500
doubleaction|3500
heavypistol|5000
marksmanpistol|1800
pistol|2000
pistol50|4000
snspistol|2300
SNSPistolMkII|2800
vintagepistol|2500
DoubleActionRevolver|36800
HeavyRevolver|3400
HeavyRevolverMkII|4800
PistolMkII|2950
navyrevolver|1300
assaultsmg|13000
combatpdw|10000
machinepistol|8000
microsmg|17000
smg|10500
smgmkii|13000
assaultshotgun|35000
bullpupshotgun|13000
DoubleBarrelShotgun|7000
heavyshotgun|33000
musket|2000
pumpshotgun|7000
pumpshotgunmkii|10000
SawedOffShotgun|7000
SweeperShotgun|33000
advancedrifle|18000
assaultrifle|13000
assaultriflemkii|15000
bullpuprifle|18000
bullpupriflemkii|21000
carbinerifle|13500
carbineriflemkii|17000
compactrifle|35000
specialcarbine|18000
specialcarbinemkii|20000
combatmg|13000
combatmgmkii|15000
GusenbergSweeper|9000
mg|10500
heavysniper|60000
heavysnipermkii|75000
marksmanrifle|50000
marksmanriflemkii|55000
sniperrifle|45000
granade|40000
molotov|5000
pipebombs|85000
ProximityMines|100000
stickybomb|80000";

            var ret = "INSERT INTO Precos VALUES ";
            var arr = str.Split(Environment.NewLine.ToCharArray());
            foreach (var x in arr)
            {
                if (string.IsNullOrWhiteSpace(x))
                    continue;

                var s = x.Split('|');
                ret += $"(7,'{s[0]}',{s[1]}),";
            }
            var a = ret;
        }
        */
       
        static async Task DownloadFile(string uri, string fileName)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            using var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            await response.Content.CopyToAsync(fs);
        }

        static async Task UpdateServer()
        {
            // pegar dos json e com base nele tem os nomes dos arquivos com a pasta base, assim vamos saber onde atualizar

            // https://cdn.alt-mp.com/server/dev/x64_win32/update.json
            // https://cdn.alt-mp.com/data/dev/update.json
            // https://cdn.alt-mp.com/coreclr-module/dev/x64_win32/update.json
            // https://cdn.alt-mp.com/js-module/dev/x64_win32/update.json

            var branch = "dev"; // release | rc | dev
            var path = @"C:\Projects\GuilhermeTrevizani\roleplay-server\ALTVSERVER";

            var files = new Dictionary<string, string>
            {
                { $"https://cdn.alt-mp.com/server/{branch}/x64_win32/altv-server.exe", Path.Combine(path, @"altv-server.exe") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/vehmodels.bin", Path.Combine(path, @"data\vehmodels.bin") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/vehmods.bin", Path.Combine(path, @"data\vehmods.bin") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/clothes.bin", Path.Combine(path, @"data\clothes.bin") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/pedmodels.bin", Path.Combine(path, @"data\pedmodels.bin") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/rpfdata.bin", Path.Combine(path, @"data\rpfdata.bin") },
                { $"https://cdn.alt-mp.com/data/{branch}/data/weaponmodels.bin", Path.Combine(path, @"data\weaponmodels.bin") },
                { $"https://cdn.alt-mp.com/coreclr-module/{branch}/x64_win32/AltV.Net.Host.dll", Path.Combine(path, @"AltV.Net.Host.dll") },
                { $"https://cdn.alt-mp.com/coreclr-module/{branch}/x64_win32/modules/csharp-module.dll", Path.Combine(path, @"modules\csharp-module.dll") },
                { $"https://cdn.alt-mp.com/js-module/{branch}/x64_win32/modules/js-module/libnode.dll", Path.Combine(path, @"modules\js-module\libnode.dll") },
                { $"https://cdn.alt-mp.com/js-module/{branch}/x64_win32/modules/js-module/js-module.dll", Path.Combine(path, @"modules\js-module\js-module.dll") },
            };

            var tasks = new List<Task>();
            foreach (var file in files)
                tasks.Add(DownloadFile(file.Key, file.Value));

            await Task.WhenAll(tasks);
        }
    }
}