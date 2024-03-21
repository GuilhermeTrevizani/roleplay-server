using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;
using System.Timers;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server
{
    public class Server : AsyncResource
    {
        public override IEntityFactory<IPlayer> GetPlayerFactory() => new MyPlayerFactory();

        public override IEntityFactory<IVehicle> GetVehicleFactory() => new MyVehicleFactory();

        public override IBaseObjectFactory<IColShape> GetColShapeFactory() => new MyColShapeFactory();

        public override IEntityFactory<IObject> GetObjectFactory() => new MyObjectFactory();

        public override IBaseObjectFactory<IMarker> GetMarkerFactory() => new MyMarkerFactory();

        public override IBaseObjectFactory<IBlip> GetBlipFactory() => new MyBlipFactory();

        System.Timers.Timer MainTimer { get; set; } = new();

        public override async void OnStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture =
                  CultureInfo.GetCultureInfo("pt-BR");

            var config = Alt.GetServerConfig();
            Global.DbConnectionString = config.Get("dbConnectionString").GetString();
            Global.AnnouncementDiscordChannel = config.Get("announcementDiscordChannel").GetULong() ?? 0;
            Global.GovernmentAnnouncementDiscordChannel = config.Get("governmentAnnouncementDiscordChannel").GetULong() ?? 0;
            Global.StaffDiscordChannel = config.Get("staffDiscordChannel").GetULong() ?? 0;
            Global.CompanyAnnouncementDiscordChannel = config.Get("companyAnnouncementDiscordChannel").GetULong() ?? 0;
            Global.RolesStaffMessage = config.Get("rolesStaffMessage").GetList().Select(x => x.GetULong() ?? 0).ToList();
            var discordBotToken = config.Get("discordBotToken").GetString();

            await using var context = new DatabaseContext();
            await context.Database.MigrateAsync();

            Global.Clothes1Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes1male.json"));
            Alt.Log($"Clothes1Male: {Global.Clothes1Male.Count}");

            Global.Clothes1Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes1female.json"));
            Alt.Log($"Clothes1Female: {Global.Clothes1Female.Count}");

            Global.Clothes3Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes3male.json"));
            Alt.Log($"Clothes3Male: {Global.Clothes3Male.Count}");

            Global.Clothes3Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes3female.json"));
            Alt.Log($"Clothes3Female: {Global.Clothes3Female.Count}");

            Global.Clothes4Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes4male.json"));
            Alt.Log($"Clothes4Male: {Global.Clothes4Male.Count}");

            Global.Clothes4Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes4female.json"));
            Alt.Log($"Clothes4Female: {Global.Clothes4Female.Count}");

            Global.Clothes5Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes5male.json"));
            Alt.Log($"Clothes5Male: {Global.Clothes5Male.Count}");

            Global.Clothes5Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes5female.json"));
            Alt.Log($"Clothes5Female: {Global.Clothes5Female.Count}");

            Global.Clothes6Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6male.json"));
            Alt.Log($"Clothes6Male: {Global.Clothes6Male.Count}");

            Global.Clothes6Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6female.json"));
            Alt.Log($"Clothes5Female: {Global.Clothes5Female.Count}");

            Global.Clothes6Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6male.json"));
            Alt.Log($"Clothes6Male: {Global.Clothes6Male.Count}");

            Global.Clothes6Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes6female.json"));
            Alt.Log($"Clothes6Female: {Global.Clothes6Female.Count}");

            Global.Clothes7Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes7male.json"));
            Alt.Log($"Clothes7Male: {Global.Clothes7Male.Count}");

            Global.Clothes7Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes7female.json"));
            Alt.Log($"Clothes7Female: {Global.Clothes7Female.Count}");

            Global.Clothes8Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes8male.json"));
            Alt.Log($"Clothes8Male: {Global.Clothes8Male.Count}");

            Global.Clothes8Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes8female.json"));
            Alt.Log($"Clothes8Female: {Global.Clothes8Female.Count}");

            Global.Clothes9Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes9male.json"));
            Alt.Log($"Clothes9Male: {Global.Clothes9Male.Count}");

            Global.Clothes9Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes9female.json"));
            Alt.Log($"Clothes9Female: {Global.Clothes9Female.Count}");

            Global.Clothes10Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes10male.json"));
            Alt.Log($"Clothes10Male: {Global.Clothes10Male.Count}");

            Global.Clothes10Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes10female.json"));
            Alt.Log($"Clothes10Female: {Global.Clothes10Female.Count}");

            Global.Clothes11Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes11male.json"));
            Alt.Log($"Clothes11Male: {Global.Clothes11Male.Count}");

            Global.Clothes11Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}clothes11female.json"));
            Alt.Log($"Clothes11Female: {Global.Clothes11Female.Count}");

            Global.Accessories0Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories0male.json"));
            Alt.Log($"Accessories0Male: {Global.Accessories0Male.Count}");

            Global.Accessories0Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories0female.json"));
            Alt.Log($"Accessories0Female: {Global.Accessories0Female.Count}");

            Global.Accessories1Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories1male.json"));
            Alt.Log($"Accessories1Male: {Global.Accessories1Male.Count}");

            Global.Accessories1Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories1female.json"));
            Alt.Log($"Accessories1Female: {Global.Accessories1Female.Count}");

            Global.Accessories2Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories2male.json"));
            Alt.Log($"Accessories2Male: {Global.Accessories2Male.Count}");

            Global.Accessories2Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories2female.json"));
            Alt.Log($"Accessories2Female: {Global.Accessories2Female.Count}");

            Global.Accessories6Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories6male.json"));
            Alt.Log($"Accessories6Male: {Global.Accessories6Male.Count}");

            Global.Accessories6Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories6female.json"));
            Alt.Log($"Accessories6Female: {Global.Accessories6Female.Count}");

            Global.Accessories7Male = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories7male.json"));
            Alt.Log($"Accessories7Male: {Global.Accessories7Male.Count}");

            Global.Accessories7Female = Functions.Deserialize<List<ClotheAccessory>>(File.ReadAllText($"{Global.PATH_JSON_CLOTHES}accessories7female.json"));
            Alt.Log($"Accessories7Female: {Global.Accessories7Female.Count}");

            await context.HelpRequests.Where(x => !x.AnswerDate.HasValue).ExecuteUpdateAsync(x => x.SetProperty(y => y.AnswerDate, DateTime.Now));
            Alt.Log("Cleaned Help Requests");

            var parameter = await context.Parameters.FirstOrDefaultAsync();
            if (parameter == null)
            {
                parameter ??= new();
                await context.Parameters.AddAsync(parameter);
            }

            Global.Parameter = parameter;
            Alt.Log("Loaded Parameters");
            Global.IPLs = Functions.Deserialize<List<string>>(Global.Parameter.IPLsJSON);

            Global.Blips = await context.Blips.ToListAsync();
            Global.Blips.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Blips: {Global.Blips.Count}");

            Global.Factions = await context.Factions.ToListAsync();
            Alt.Log($"Factions: {Global.Factions.Count}");

            Global.FactionsRanks = await context.FactionsRanks.ToListAsync();
            Alt.Log($"FactionsRanks: {Global.FactionsRanks.Count}");

            Global.Properties = await context.Properties
                .Include(x => x.Items)
                .Include(x => x.Furnitures!)
                    .ThenInclude(x => x.Property)
                .ToListAsync();
            foreach (var property in Global.Properties)
            {
                property.CreateIdentifier();

                foreach (var furniture in property.Furnitures!)
                    furniture.CreateObject();
            }
            Alt.Log($"Properties: {Global.Properties.Count}");

            Global.Prices = await context.Prices.ToListAsync();
            Alt.Log($"Prices: {Global.Prices.Count}");

            Global.Spots = await context.Spots.ToListAsync();
            Global.Spots.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Spots: {Global.Spots.Count}");

            Global.FactionsStorages = await context.FactionStorages.ToListAsync();
            Global.FactionsStorages.ForEach(x => x.CreateIdentifier());
            Alt.Log($"FactionsStorages: {Global.FactionsStorages.Count}");

            Global.FactionsStoragesItems = await context.FactionStoragesItems.ToListAsync();
            Alt.Log($"FactionsStoragesItems: {Global.FactionsStoragesItems.Count}");

            Global.Questions = await context.Questions.ToListAsync();
            Alt.Log($"Questions: {Global.Questions.Count}");

            Global.QuestionsAnswers = await context.QuestionsAnswers.ToListAsync();
            Alt.Log($"QuestionsAnswers: {Global.QuestionsAnswers.Count}");

            foreach (var dealership in Global.Dealerships)
                Functions.CreateMarkerColShape($"[{dealership.Name}] {{#FFFFFF}}Use /comprar ou /vupgrade.", dealership.Position);
            Alt.Log($"Dealerships: {Global.Dealerships.Count}");

            foreach (var job in Global.Jobs)
            {
                var name = job.CharacterJob.GetDisplay().ToUpper();
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
                .Include(x => x.Character!)
                .Include(x => x.Characters!)
                    .ThenInclude(x => x.Character)
                .ToListAsync();
            Alt.Log($"FactionsUnits: {Global.FactionsUnits.Count}");

            Global.EmergencyCalls = (await context.EmergencyCalls.ToListAsync()).Where(x => (DateTime.Now - x.Date).TotalHours < 24).ToList();
            Alt.Log($"EmergencyCalls: {Global.EmergencyCalls.Count}");

            Global.Items = await context.Items.ToListAsync();
            Global.Items.ForEach(x => x.CreateObject());
            Alt.Log($"Items: {Global.Items.Count}");

            Global.Doors = await context.Doors.ToListAsync();
            Alt.Log($"Doors: {Global.Doors.Count}");

            Global.Infos = await context.Infos.Include(x => x.User).ToListAsync();
            Global.Infos.ForEach(x => x.CreateIdentifier());
            Alt.Log($"Infos: {Global.Infos.Count}");

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

            if (!string.IsNullOrWhiteSpace(discordBotToken))
                DiscordBOT.Main.MainAsync(discordBotToken).GetAwaiter().GetResult();

            Alt.Log("Successfully loaded.");
        }

        public override async void OnStop()
        {
            MainTimer?.Stop();

            foreach (var vehicle in Global.Vehicles)
                await vehicle.Estacionar(null);

            foreach (var player in Global.SpawnedPlayers)
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
                        .Include(x => x.Character)
                            .ThenInclude(x => x!.User)
                        .ToListAsync())
                        .Where(x => (DateTime.Now - x.Character!.LastAccessDate).TotalDays
                            > (x.Character.User!.VIP switch
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

                        prop.RemoveOwner();
                        prop.CreateIdentifier();

                        context.Properties.Update(prop);
                    }

                    Global.Parameter.SetInactivePropertiesDate();
                    context.Parameters.Update(Global.Parameter);
                    await context.SaveChangesAsync();
                }

                var companies = Global.Companies.Where(x => x.CharacterId.HasValue && (x.RentPaymentDate ?? DateTime.MinValue) <= DateTime.Now).ToList();
                if (companies.Count != 0)
                {
                    await using var context = new DatabaseContext();
                    foreach (var company in companies)
                    {
                        var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == company.CharacterId);
                        if (target != null)
                        {
                            if (target.Character.Bank - company.WeekRentValue <= 0)
                            {
                                await company.RemoveOwner();
                                continue;
                            }

                            target.Character.RemoveBank(company.WeekRentValue);
                            await target.Save();
                        }
                        else
                        {
                            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == company.CharacterId);
                            if (character!.Bank - company.WeekRentValue <= 0)
                            {
                                await company.RemoveOwner();
                                continue;
                            }

                            character.RemoveBank(company.WeekRentValue);
                            context.Characters.Update(character);
                            await context.SaveChangesAsync();
                        }

                        company.RenewRent();
                        context.Companies.Update(company);
                        await context.SaveChangesAsync();

                        var financialTransaction = new FinancialTransaction();
                        financialTransaction.Create(FinancialTransactionType.Withdraw,
                            company.CharacterId!.Value,
                            company.WeekRentValue,
                            $"Pagamento do Aluguel da Empresa #{company.Name}");

                        await context.FinancialTransactions.AddAsync(financialTransaction);
                        await context.SaveChangesAsync();
                    }
                }

                var url = "https://api.openweathermap.org/data/2.5/weather?lat=34.0536909&lon=-118.242766&appid=401a061ac0ba4fb46e01ec97d0fb5593&units=metric";

                using var httpClient = new HttpClient();
                Global.WeatherInfo = await httpClient.GetFromJsonAsync<WeatherInfo>(url)!;
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