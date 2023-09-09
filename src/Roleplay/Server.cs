using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Timers;

namespace Roleplay
{
    public class Server : AsyncResource
    {
        public override IEntityFactory<IPlayer> GetPlayerFactory() => new MyPlayerFactory();

        public override IEntityFactory<IVehicle> GetVehicleFactory() => new MyVehicleFactory();

        public override IBaseObjectFactory<IColShape> GetColShapeFactory() => new MyColShapeFactory();

        public override IEntityFactory<IObject> GetObjectFactory() => new MyObjectFactory();

        System.Timers.Timer MainTimer { get; set; }

        public override async void OnStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture =
                  CultureInfo.GetCultureInfo("pt-BR");

            var config = Alt.GetServerConfig();
            Global.DbConnectionString = config.Get("dbConnectionString").GetString();
            Global.DiscordBotToken = config.Get("discordBotToken").GetString();
            Global.AnnouncementDiscordChannel = config.Get("announcementDiscordChannel").GetULong() ?? 0;
            Global.GovernmentAnnouncementDiscordChannel = config.Get("governmentAnnouncementDiscordChannel").GetULong() ?? 0;
            Global.StaffDiscordChannel = config.Get("staffDiscordChannel").GetULong() ?? 0;
            Global.CompanyAnnouncementDiscordChannel = config.Get("companyAnnouncementDiscordChannel").GetULong() ?? 0;
            Global.RolesStaffMessage = config.Get("rolesStaffMessage").GetList().Select(x => x.GetULong() ?? 0).ToList();

            await using var context = new DatabaseContext();
            await context.Database.MigrateAsync();

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Global.Clothes1Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes1male.json"),
                jsonOptions);
            Alt.Log($"Clothes1Male: {Global.Clothes1Male.Count}");

            Global.Clothes1Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes1female.json"),
                jsonOptions);
            Alt.Log($"Clothes1Female: {Global.Clothes1Female.Count}");

            Global.Clothes3Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes3male.json"),
                jsonOptions);
            Alt.Log($"Clothes3Male: {Global.Clothes3Male.Count}");

            Global.Clothes3Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes3female.json"),
                jsonOptions);
            Alt.Log($"Clothes3Female: {Global.Clothes3Female.Count}");

            Global.Clothes4Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes4male.json"),
                jsonOptions);
            Alt.Log($"Clothes4Male: {Global.Clothes4Male.Count}");

            Global.Clothes4Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes4female.json"),
                jsonOptions);
            Alt.Log($"Clothes4Female: {Global.Clothes4Female.Count}");

            Global.Clothes5Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes5male.json"),
                jsonOptions);
            Alt.Log($"Clothes5Male: {Global.Clothes5Male.Count}");

            Global.Clothes5Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes5female.json"),
                jsonOptions);
            Alt.Log($"Clothes5Female: {Global.Clothes5Female.Count}");

            Global.Clothes6Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6male.json"),
                jsonOptions);
            Alt.Log($"Clothes6Male: {Global.Clothes6Male.Count}");

            Global.Clothes6Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6female.json"),
                jsonOptions);
            Alt.Log($"Clothes5Female: {Global.Clothes5Female.Count}");

            Global.Clothes6Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6male.json"),
                jsonOptions);
            Alt.Log($"Clothes6Male: {Global.Clothes6Male.Count}");

            Global.Clothes6Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6female.json"),
                jsonOptions);
            Alt.Log($"Clothes6Female: {Global.Clothes6Female.Count}");

            Global.Clothes7Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes7male.json"),
                jsonOptions);
            Alt.Log($"Clothes7Male: {Global.Clothes7Male.Count}");

            Global.Clothes7Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes7female.json"),
                jsonOptions);
            Alt.Log($"Clothes7Female: {Global.Clothes7Female.Count}");

            Global.Clothes8Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes8male.json"),
                jsonOptions);
            Alt.Log($"Clothes8Male: {Global.Clothes8Male.Count}");

            Global.Clothes8Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes8female.json"),
                jsonOptions);
            Alt.Log($"Clothes8Female: {Global.Clothes8Female.Count}");

            Global.Clothes9Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes9male.json"),
                jsonOptions);
            Alt.Log($"Clothes9Male: {Global.Clothes9Male.Count}");

            Global.Clothes9Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes9female.json"),
                jsonOptions);
            Alt.Log($"Clothes9Female: {Global.Clothes9Female.Count}");

            Global.Clothes10Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes10male.json"),
                jsonOptions);
            Alt.Log($"Clothes10Male: {Global.Clothes10Male.Count}");

            Global.Clothes10Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes10female.json"),
                jsonOptions);
            Alt.Log($"Clothes10Female: {Global.Clothes10Female.Count}");

            Global.Clothes11Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes11male.json"),
                jsonOptions);
            Alt.Log($"Clothes11Male: {Global.Clothes11Male.Count}");

            Global.Clothes11Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes11female.json"),
                jsonOptions);
            Alt.Log($"Clothes11Female: {Global.Clothes11Female.Count}");

            Global.Accessories0Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories0male.json"),
                jsonOptions);
            Alt.Log($"Accessories0Male: {Global.Accessories0Male.Count}");

            Global.Accessories0Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories0female.json"),
                jsonOptions);
            Alt.Log($"Accessories0Female: {Global.Accessories0Female.Count}");

            Global.Accessories1Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories1male.json"),
                jsonOptions);
            Alt.Log($"Accessories1Male: {Global.Accessories1Male.Count}");

            Global.Accessories1Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories1female.json"),
                jsonOptions);
            Alt.Log($"Accessories1Female: {Global.Accessories1Female.Count}");

            Global.Accessories2Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories2male.json"),
                jsonOptions);
            Alt.Log($"Accessories2Male: {Global.Accessories2Male.Count}");

            Global.Accessories2Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories2female.json"),
                jsonOptions);
            Alt.Log($"Accessories2Female: {Global.Accessories2Female.Count}");

            Global.Accessories6Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories6male.json"),
                jsonOptions);
            Alt.Log($"Accessories6Male: {Global.Accessories6Male.Count}");

            Global.Accessories6Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories6female.json"),
                jsonOptions);
            Alt.Log($"Accessories6Female: {Global.Accessories6Female.Count}");

            Global.Accessories7Male = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories7male.json"),
                jsonOptions);
            Alt.Log($"Accessories7Male: {Global.Accessories7Male.Count}");

            Global.Accessories7Female = JsonSerializer.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories7female.json"),
                jsonOptions);
            Alt.Log($"Accessories7Female: {Global.Accessories7Female.Count}");

            await context.HelpRequests.Where(x => !x.AnswerDate.HasValue).ExecuteUpdateAsync(x => x.SetProperty(y => y.AnswerDate, DateTime.Now));
            Alt.Log("Cleaned Help Requests");

            Global.Parameter = await context.Parameters.FirstOrDefaultAsync();
            Alt.Log("Loaded Parameters");
            Global.IPLs = JsonSerializer.Deserialize<List<string>>(Global.Parameter.IPLsJSON);

            Global.Blips = await context.Blips.ToListAsync();
            Global.Blips.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Blips: {Global.Blips.Count}");

            Global.Factions = await context.Factions.ToListAsync();
            Alt.Log($"Factions: {Global.Factions.Count}");

            Global.FactionsRanks = await context.FactionsRanks.ToListAsync();
            Alt.Log($"FactionsRanks: {Global.FactionsRanks.Count}");

            var propertiesItems = (await context.PropertiesItems.ToListAsync()).Select(x => new PropertyItem(x)).ToList();
            Global.Properties = await context.Properties.Include(x => x.Furnitures).ToListAsync();
            foreach (var property in Global.Properties)
            {
                property.Items = propertiesItems.Where(y => y.PropertyId == property.Id).ToList();
                property.CreateIdentifier();

                foreach (var furniture in property.Furnitures)
                    furniture.CreateObject();
            }
            Alt.Log($"Properties: {Global.Properties.Count}");
            Alt.Log($"PropertiesItems: {propertiesItems.Count}");

            Global.Prices = await context.Prices.ToListAsync();
            Alt.Log($"Prices: {Global.Prices.Count}");

            Global.Spots = await context.Spots.ToListAsync();
            Global.Spots.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Spots: {Global.Spots.Count}");

            Global.FactionsArmories = await context.FactionsArmories.ToListAsync();
            Global.FactionsArmories.ForEach(x => x.CreateIdentifier());
            Alt.Log($"FactionsArmories: {Global.FactionsArmories.Count}");

            Global.FactionsArmoriesWeapons = await context.FactionsArmoriesWeapons.ToListAsync();
            Alt.Log($"FactionsArmoriesWeapons: {Global.FactionsArmoriesWeapons.Count}");

            Global.Questions = await context.Questions.ToListAsync();
            Alt.Log($"Questions: {Global.Questions.Count}");

            Global.QuestionsAnswers = await context.QuestionsAnswers.ToListAsync();
            Alt.Log($"QuestionsAnswers: {Global.QuestionsAnswers.Count}");

            Global.Dealerships.ForEach(x => Functions.CreateMarkerColShape($"[{x.Name}] {{#FFFFFF}}Use /comprar ou /vupgrade.", x.Position));
            Alt.Log($"Dealerships: {Global.Dealerships.Count}");

            foreach (var job in Global.Jobs)
            {
                var name = Functions.GetEnumDisplay(job.CharacterJob).ToUpper();
                Functions.CreateMarkerColShape($"[EMPREGO DE {name}] {{#FFFFFF}}Use /emprego para se tornar um {name.ToLower()}.", job.Position);

                var spot = $"[ALUGUEL DE VEÍCULOS {name}] {{#FFFFFF}}Use /valugar";
                if (job.CharacterJob == CharacterJob.Mechanic)
                    spot += ", /reparar";

                spot += " ou /vestacionar.";

                Functions.CreateMarkerColShape(spot, job.VehicleRentPosition);
            }
            Alt.Log($"Jobs: {Global.Jobs.Count}");

            Global.FactionsUnits = await context.FactionsUnits
                .Where(x => !x.FinalDate.HasValue)
                .Include(x => x.Character)
                .Include(x => x.Characters).ThenInclude(x => x.Character)
                .ToListAsync();
            Alt.Log($"FactionsUnits: {Global.FactionsUnits.Count}");

            Global.FactionsUnitsCharacters.AddRange(Global.FactionsUnits.Select(x => x.Characters).SelectMany(x => x));
            Alt.Log($"FactionsUnitsCharacters: {Global.FactionsUnitsCharacters.Count}");

            Global.EmergencyCalls = (await context.EmergencyCalls.ToListAsync()).Where(x => (DateTime.Now - x.Date).TotalHours < 24).ToList();
            Alt.Log($"EmergencyCalls: {Global.EmergencyCalls.Count}");

            Global.Items = (await context.Items.ToListAsync()).Select(x => new Item(x)).ToList();
            Global.Items.ForEach(x => x.CreateObject());
            Alt.Log($"Items: {Global.Items.Count}");

            Global.Doors = await context.Doors.ToListAsync();
            Alt.Log($"Doors: {Global.Doors.Count}");

            Global.Infos = await context.Infos.Include(x => x.User).ToListAsync();
            Global.Infos.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Infos: {Global.Infos.Count}");

            Global.FactionsDrugsHouses = await context.FactionsDrugsHouses.ToListAsync();
            Global.FactionsDrugsHouses.ForEach(x => x.CreateIdentifier());
            Alt.Log($"FactionsDrugsHouses: {Global.FactionsDrugsHouses.Count}");

            Global.FactionsDrugsHousesItems = await context.FactionsDrugsHousesItems.ToListAsync();
            Alt.Log($"FactionsDrugsHousesItems: {Global.FactionsDrugsHousesItems.Count}");

            Global.CrackDens = await context.CrackDens.ToListAsync();
            Global.CrackDens.ForEach(x => x.CreateIdentifier());
            Alt.Log($"CrackDens: {Global.CrackDens.Count}");

            Global.CrackDensItems = await context.CrackDensItems.ToListAsync();
            Alt.Log($"CrackDensItems: {Global.CrackDensItems.Count}");

            Global.TruckerLocations = await context.TruckerLocations.ToListAsync();
            Global.TruckerLocations.ForEach(x => x.CreateIdentifier());
            Alt.Log($"TruckerLocations: {Global.TruckerLocations.Count}");

            Global.TruckerLocationsDeliveries = await context.TruckerLocationsDeliveries.ToListAsync();
            Alt.Log($"TruckerLocationsDeliveries: {Global.TruckerLocationsDeliveries.Count}");

            Global.Furnitures = await context.Furnitures.ToListAsync();
            Alt.Log($"Furnitures: {Global.Furnitures.Count}");

            Global.Animations = await context.Animations.ToListAsync();
            Alt.Log($"Animations: {Global.Animations.Count}");

            Global.Companies = await context.Companies.Include(x => x.Characters).ToListAsync();
            Global.Companies.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Companies: {Global.Companies.Count}");

            Alt.Log($"Scenarios: {Global.Scenarios.Count}");

            Global.Commands = Assembly.GetExecutingAssembly().GetTypes()
                    .SelectMany(x => x.GetMethods())
                    .Where(x => x.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToList();
            Alt.Log($"Commands: {Global.Commands.Count}");

            MainTimer = new System.Timers.Timer(600000);
            MainTimer.Elapsed += MainTimer_Elapsed;
            MainTimer.Start();
            MainTimer_Elapsed(null, null);

            if (!string.IsNullOrWhiteSpace(Global.DiscordBotToken))
                DiscordBOT.Main.MainAsync().GetAwaiter().GetResult();

            Alt.Log("Successfully loaded.");
        }

        public override async void OnStop()
        {
            MainTimer?.Stop();

            foreach (var vehicle in Global.Vehicles)
                await vehicle.Estacionar(null);

            foreach (var player in Global.Players)
                await player.Disconnect("OnStop", true);
        }

        private async void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if ((Global.Parameter.InactivePropertiesDate ?? DateTime.MinValue) < DateTime.Now)
                {
                    await using var context = new DatabaseContext();
                    var properties = (await context.Properties
                        .Where(x => x.CharacterId.HasValue)
                        .Include(x => x.Character).ThenInclude(x => x.User)
                        .ToListAsync())
                        .Where(x => (DateTime.Now - x.Character.LastAccessDate).TotalDays
                            > (x.Character.User.VIP switch
                            {
                                UserVIP.Gold => 21,
                                UserVIP.Silver => 14,
                                _ => 7,
                            })
                        );
                    foreach (var x in properties)
                    {
                        var prop = Global.Properties.FirstOrDefault(y => y.Id == x.Id);
                        if (prop == null)
                            continue;

                        prop.CharacterId = null;
                        prop.CreateIdentifier();

                        context.Properties.Update(prop);
                    }

                    Global.Parameter.InactivePropertiesDate = DateTime.Now.AddDays(1);
                    context.Parameters.Update(Global.Parameter);
                    await context.SaveChangesAsync();
                }

                var companies = Global.Companies.Where(x => x.CharacterId.HasValue && (x.RentPaymentDate ?? DateTime.MinValue) <= DateTime.Now).ToList();
                if (companies.Any())
                {
                    await using var context = new DatabaseContext();
                    foreach (var company in companies)
                    {
                        var target = Global.Players.FirstOrDefault(x => x.Character.Id == company.CharacterId);
                        if (target != null)
                        {
                            if (target.Character.Bank - company.WeekRentValue <= 0)
                            {
                                await company.RemoveOwner();
                                continue;
                            }

                            target.Character.Bank -= company.WeekRentValue;
                            await target.Save();
                        }
                        else
                        {
                            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == company.CharacterId);
                            if (character.Bank - company.WeekRentValue <= 0)
                            {
                                await company.RemoveOwner();
                                continue;
                            }

                            character.Bank -= company.WeekRentValue;
                            context.Characters.Update(character);
                            await context.SaveChangesAsync();
                        }

                        company.RentPaymentDate = DateTime.Now.AddDays(7);
                        context.Companies.Update(company);
                        await context.SaveChangesAsync();

                        await context.FinancialTransactions.AddAsync(new FinancialTransaction
                        {
                            Type = FinancialTransactionType.Withdraw,
                            CharacterId = company.CharacterId ?? 0,
                            Value = company.WeekRentValue,
                            Description = $"Pagamento do Aluguel da Empresa #{company.Id}",
                        });
                        await context.SaveChangesAsync();
                    }
                }

                var url = "https://api.openweathermap.org/data/2.5/weather?lat=34.0536909&lon=-118.242766&appid=401a061ac0ba4fb46e01ec97d0fb5593&units=metric";

                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = httpClient.Send(request);
                using var reader = new StreamReader(response.Content.ReadAsStream());
                var json = reader.ReadToEnd();

                Global.WeatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                Global.WeatherInfo.WeatherType = Global.WeatherInfo.Weather.FirstOrDefault()?.Main switch
                {
                    "Drizzle" => WeatherType.Clearing,
                    "Clouds" => WeatherType.Clouds,
                    "Rain" => WeatherType.Rain,
                    "Thunderstorm" or "Thunder" => WeatherType.Thunder,
                    "Foggy" or "Fog" or "Mist" or "Smoke" => WeatherType.Foggy,
                    "Smog" => WeatherType.Smog,
                    "Overcast" => WeatherType.Overcast,
                    "Snowing" or "Snow" => WeatherType.Snow,
                    "Blizzard" => WeatherType.Blizzard,
                    _ => WeatherType.Clear,
                };

                Alt.SetSyncedMetaData("Temperature", $"{Global.WeatherInfo.Main.Temp:N0}ºC");
                Alt.SetSyncedMetaData("WeatherType", Global.WeatherInfo.WeatherType.ToString().ToUpper());
                Alt.EmitAllClients("SyncWeather", Global.WeatherInfo.WeatherType.ToString().ToUpper());
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }
    }
}