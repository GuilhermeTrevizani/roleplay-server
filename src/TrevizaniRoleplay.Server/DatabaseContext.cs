using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Infra.Data.ModelConfigurations;

namespace TrevizaniRoleplay.Server
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Animation> Animations { get; set; }
        public DbSet<Banishment> Banishments { get; set; }
        public DbSet<Blip> Blips { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterItem> CharactersItems { get; set; }
        public DbSet<CrackDen> CrackDens { get; set; }
        public DbSet<CrackDenItem> CrackDensItems { get; set; }
        public DbSet<CrackDenSell> CrackDensSells { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyCharacter> CompaniesCharacters { get; set; }
        public DbSet<Confiscation> Confiscations { get; set; }
        public DbSet<ConfiscationItem> ConfiscationsItems { get; set; }
        public DbSet<Door> Doors { get; set; }
        public DbSet<EmergencyCall> EmergencyCalls { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<FactionStorage> FactionStorages { get; set; }
        public DbSet<FactionStorageItem> FactionStoragesItems { get; set; }
        public DbSet<FactionRank> FactionsRanks { get; set; }
        public DbSet<FactionUnit> FactionsUnits { get; set; }
        public DbSet<FactionUnitCharacter> FactionsUnitsCharacters { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Info> Infos { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Jail> Jails { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyFurniture> PropertiesFurnitures { get; set; }
        public DbSet<PropertyItem> PropertiesItems { get; set; }
        public DbSet<Punishment> Punishments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionsAnswers { get; set; }
        public DbSet<SeizedVehicle> SeizedVehicles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<TruckerLocation> TruckerLocations { get; set; }
        public DbSet<TruckerLocationDelivery> TruckerLocationsDeliveries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleItem> VehiclesItems { get; set; }
        public DbSet<Wanted> Wanted { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder.UseNpgsql(Global.DbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserModelConfiguration))!);
        }
    }
}