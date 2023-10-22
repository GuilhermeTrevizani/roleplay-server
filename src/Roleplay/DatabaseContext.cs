using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;

namespace Roleplay
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
        public DbSet<FactionArmory> FactionsArmories { get; set; }
        public DbSet<FactionArmoryWeapon> FactionsArmoriesWeapons { get; set; }
        public DbSet<FactionDrugHouse> FactionsDrugsHouses { get; set; }
        public DbSet<FactionDrugHouseItem> FactionsDrugsHousesItems { get; set; }
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
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animation>().HasKey(x => x.Id);

            modelBuilder.Entity<Banishment>().HasKey(x => x.Id);

            modelBuilder.Entity<Blip>().HasKey(x => x.Id);

            modelBuilder.Entity<Character>().HasKey(x => x.Id);
            modelBuilder.Entity<Character>().HasOne(x => x.User).WithMany(x => x.Characters).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Character>().HasOne(x => x.EvaluatingStaffUser).WithMany(x => x.EvaluatingCharacters).HasForeignKey(x => x.EvaluatingStaffUserId);
            modelBuilder.Entity<Character>().HasOne(x => x.EvaluatorStaffUser).WithMany(x => x.EvaluatorCharacters).HasForeignKey(x => x.EvaluatorStaffUserId);

            modelBuilder.Entity<CharacterItem>().HasKey(x => x.Id);

            modelBuilder.Entity<CrackDen>().HasKey(x => x.Id);

            modelBuilder.Entity<CrackDenItem>().HasKey(x => x.Id);

            modelBuilder.Entity<CrackDenSell>().HasKey(x => x.Id);

            modelBuilder.Entity<Company>().HasKey(x => x.Id);

            modelBuilder.Entity<CompanyCharacter>().HasKey(x => x.Id);
            modelBuilder.Entity<CompanyCharacter>().HasOne(x => x.Company).WithMany(x => x.Characters).HasForeignKey(x => x.CompanyId);

            modelBuilder.Entity<Confiscation>().HasKey(x => x.Id);

            modelBuilder.Entity<ConfiscationItem>().HasKey(x => x.Id);
            modelBuilder.Entity<ConfiscationItem>().HasOne(x => x.Confiscation).WithMany(x => x.Items).HasForeignKey(x => x.ConfiscationId);
            modelBuilder.Entity<Door>().HasKey(x => x.Id);

            modelBuilder.Entity<EmergencyCall>().HasKey(x => x.Id);

            modelBuilder.Entity<Faction>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionArmory>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionArmoryWeapon>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionDrugHouse>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionDrugHouseItem>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionRank>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionUnit>().HasKey(x => x.Id);

            modelBuilder.Entity<FactionUnitCharacter>().HasKey(x => x.Id);
            modelBuilder.Entity<FactionUnitCharacter>().HasOne(x => x.FactionUnit).WithMany(x => x.Characters).HasForeignKey(x => x.FactionUnitId);

            modelBuilder.Entity<FinancialTransaction>().HasKey(x => x.Id);

            modelBuilder.Entity<Fine>().HasKey(x => x.Id);

            modelBuilder.Entity<Furniture>().HasKey(x => x.Id);

            modelBuilder.Entity<Info>().HasKey(x => x.Id);

            modelBuilder.Entity<Item>().HasKey(x => x.Id);

            modelBuilder.Entity<Jail>().HasKey(x => x.Id);

            modelBuilder.Entity<Log>().HasKey(x => x.Id);

            modelBuilder.Entity<Parameter>().HasKey(x => x.Id);
            modelBuilder.Entity<Parameter>().HasData(new Parameter());

            modelBuilder.Entity<Price>().HasKey(x => x.Id);

            modelBuilder.Entity<Property>().HasKey(x => x.Id);

            modelBuilder.Entity<PropertyFurniture>().HasKey(x => x.Id);
            modelBuilder.Entity<PropertyFurniture>().HasOne(x => x.Property).WithMany(x => x.Furnitures).HasForeignKey(x => x.PropertyId);

            modelBuilder.Entity<PropertyItem>().HasKey(x => x.Id);

            modelBuilder.Entity<Punishment>().HasKey(x => x.Id);

            modelBuilder.Entity<Question>().HasKey(x => x.Id);

            modelBuilder.Entity<QuestionAnswer>().HasKey(x => x.Id);

            modelBuilder.Entity<SeizedVehicle>().HasKey(x => x.Id);

            modelBuilder.Entity<Session>().HasKey(x => x.Id);

            modelBuilder.Entity<Spot>().HasKey(x => x.Id);

            modelBuilder.Entity<HelpRequest>().HasKey(x => x.Id);

            modelBuilder.Entity<TruckerLocation>().HasKey(x => x.Id);

            modelBuilder.Entity<TruckerLocationDelivery>().HasKey(x => x.Id);

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            modelBuilder.Entity<Vehicle>().HasKey(x => x.Id);

            modelBuilder.Entity<VehicleItem>().HasKey(x => x.Id);

            modelBuilder.Entity<Wanted>().HasKey(x => x.Id);
        }
    }
}