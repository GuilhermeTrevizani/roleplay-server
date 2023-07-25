using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleplay.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Display = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dictionary = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Vehicle = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Blips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Type = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Color = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blips", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharactersItems",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<short>(type: "smallint", nullable: false),
                    Category = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Type = table.Column<uint>(type: "int unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Extra = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharactersItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    WeekRentValue = table.Column<int>(type: "int", nullable: false),
                    RentPaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BlipType = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    BlipColor = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CrackDens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false),
                    OnlinePoliceOfficers = table.Column<int>(type: "int", nullable: false),
                    CooldownQuantityLimit = table.Column<int>(type: "int", nullable: false),
                    CooldownHours = table.Column<int>(type: "int", nullable: false),
                    CooldownDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDens", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CrackDensItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CrackDenId = table.Column<int>(type: "int", nullable: false),
                    ItemCategory = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDensItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Doors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hash = table.Column<long>(type: "bigint", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: true),
                    Locked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doors", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmergencyCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Number = table.Column<uint>(type: "int unsigned", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCalls", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    ChatColor = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsArmories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsArmories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsArmoriesWeapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionArmoryId = table.Column<int>(type: "int", nullable: false),
                    Weapon = table.Column<uint>(type: "int unsigned", nullable: false),
                    Ammo = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TintIndex = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ComponentsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsArmoriesWeapons", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsDrugsHouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsDrugsHouses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsDrugsHousesItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionDrugHouseId = table.Column<int>(type: "int", nullable: false),
                    ItemCategory = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsDrugsHousesItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Salary = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsRanks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Furnitures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Furnitures", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HelpRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AnswerDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    StaffUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpRequests", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Dimension = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    RotR = table.Column<float>(type: "float", nullable: false),
                    RotP = table.Column<float>(type: "float", nullable: false),
                    RotY = table.Column<float>(type: "float", nullable: false),
                    Category = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Type = table.Column<uint>(type: "int unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Extra = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MaxCharactersOnline = table.Column<int>(type: "int", nullable: false),
                    VehicleParkValue = table.Column<int>(type: "int", nullable: false),
                    HospitalValue = table.Column<int>(type: "int", nullable: false),
                    BarberValue = table.Column<int>(type: "int", nullable: false),
                    ClothesValue = table.Column<int>(type: "int", nullable: false),
                    DriverLicenseBuyValue = table.Column<int>(type: "int", nullable: false),
                    DriverLicenseRenewValue = table.Column<int>(type: "int", nullable: false),
                    FuelValue = table.Column<int>(type: "int", nullable: false),
                    Paycheck = table.Column<int>(type: "int", nullable: false),
                    AnnouncementValue = table.Column<int>(type: "int", nullable: false),
                    ExtraPaymentGarbagemanValue = table.Column<int>(type: "int", nullable: false),
                    Blackout = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InactivePropertiesDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    KeyValue = table.Column<int>(type: "int", nullable: false),
                    LockValue = table.Column<int>(type: "int", nullable: false),
                    IPLsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TattooValue = table.Column<int>(type: "int", nullable: false),
                    CooldownDismantleHours = table.Column<int>(type: "int", nullable: false),
                    PropertyRobberyConnectedTime = table.Column<int>(type: "int", nullable: false),
                    CooldownPropertyRobberyRobberHours = table.Column<int>(type: "int", nullable: false),
                    CooldownPropertyRobberyPropertyHours = table.Column<int>(type: "int", nullable: false),
                    PoliceOfficersPropertyRobbery = table.Column<int>(type: "int", nullable: false),
                    InitialTimeCrackDen = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    EndTimeCrackDen = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    FirefightersBlockHeal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PropertiesItems",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Category = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Type = table.Column<uint>(type: "int unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Extra = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CorrectQuestionAnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionsAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsAnswers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SeizedVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    PoliceOfficerCharacterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeizedVehicles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    InitialDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinalDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    AuxiliarPosX = table.Column<float>(type: "float", nullable: false),
                    AuxiliarPosY = table.Column<float>(type: "float", nullable: false),
                    AuxiliarPosZ = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TruckerLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    DeliveryValue = table.Column<int>(type: "int", nullable: false),
                    LoadWaitTime = table.Column<int>(type: "int", nullable: false),
                    UnloadWaitTime = table.Column<int>(type: "int", nullable: false),
                    AllowedVehiclesJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckerLocations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TruckerLocationsDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TruckerLocationId = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckerLocationsDeliveries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegisterIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegisterDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RegisterHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RegisterHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    LastAccessIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastAccessDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastAccessHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    LastAccessHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Staff = table.Column<int>(type: "int", nullable: false),
                    NameChanges = table.Column<int>(type: "int", nullable: false),
                    HelpRequestsAnswersQuantity = table.Column<int>(type: "int", nullable: false),
                    StaffDutyTime = table.Column<int>(type: "int", nullable: false),
                    TimeStampToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EmailConfirmationToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VIP = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    VIPValidDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ForumNameChanges = table.Column<int>(type: "int", nullable: false),
                    PMToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StaffChatToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FactionChatToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PlateChanges = table.Column<int>(type: "int", nullable: false),
                    AnnouncementToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Discord = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    VehicleTagToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ChatFontType = table.Column<int>(type: "int", nullable: false),
                    ChatLines = table.Column<int>(type: "int", nullable: false),
                    ChatFontSize = table.Column<int>(type: "int", nullable: false),
                    DiscordConfirmationToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetPasswordToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StaffFlagsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionToggle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CharacterApplicationsQuantity = table.Column<int>(type: "int", nullable: false),
                    CooldownDismantle = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PropertyRobberyCooldown = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehiclesItems",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Category = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Type = table.Column<uint>(type: "int unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Extra = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RegisterIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegisterHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RegisterHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    LastAccessDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastAccessIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastAccessHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    LastAccessHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Model = table.Column<uint>(type: "int unsigned", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Health = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Armor = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false),
                    BirthdayDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ConnectedTime = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: true),
                    FactionRankId = table.Column<int>(type: "int", nullable: true),
                    Bank = table.Column<int>(type: "int", nullable: false),
                    IPLsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeathDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeathReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Job = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    PersonalizationJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    History = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EvaluatingStaffUserId = table.Column<int>(type: "int", nullable: true),
                    EvaluatorStaffUserId = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameChangeStatus = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    PersonalizationStep = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    JailFinalDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DriverLicenseValidDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PoliceOfficerBlockedDriverLicenseCharacterId = table.Column<int>(type: "int", nullable: true),
                    Badge = table.Column<int>(type: "int", nullable: false),
                    AnnouncementLastUseDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Savings = table.Column<int>(type: "int", nullable: false),
                    ExtraPayment = table.Column<int>(type: "int", nullable: false),
                    WoundsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Wound = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Sex = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mask = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    FactionFlagsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DrugItemCategory = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    DrugEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ThresoldDeath = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ThresoldDeathEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CKAvaliation = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_PoliceOfficerBlockedDriverLicenseChara~",
                        column: x => x.PoliceOfficerBlockedDriverLicenseCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_FactionsRanks_FactionRankId",
                        column: x => x.FactionRankId,
                        principalTable: "FactionsRanks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Users_EvaluatingStaffUserId",
                        column: x => x.EvaluatingStaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Users_EvaluatorStaffUserId",
                        column: x => x.EvaluatorStaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Infos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Banishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StaffUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banishments_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Banishments_Users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Banishments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompaniesCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    FlagsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompaniesCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompaniesCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompaniesCharacters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Confiscations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    PoliceOfficerCharacterId = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confiscations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Confiscations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Confiscations_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Confiscations_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CrackDensSells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CrackDenId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ItemCategory = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDensSells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrackDensSells_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    InitialDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinalDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Plate = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsUnits_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    PoliceOfficerCharacterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fines_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fines_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    PoliceOfficerCharacterId = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jails_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jails_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jails_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginCharacterId = table.Column<int>(type: "int", nullable: true),
                    OriginIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    OriginHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TargetCharacterId = table.Column<int>(type: "int", nullable: true),
                    TargetIp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetHardwareIdHash = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TargetHardwareIdExHash = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Characters_OriginCharacterId",
                        column: x => x.OriginCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Logs_Characters_TargetCharacterId",
                        column: x => x.TargetCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Interior = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    EntrancePosX = table.Column<float>(type: "float", nullable: false),
                    EntrancePosY = table.Column<float>(type: "float", nullable: false),
                    EntrancePosZ = table.Column<float>(type: "float", nullable: false),
                    ExitPosX = table.Column<float>(type: "float", nullable: false),
                    ExitPosY = table.Column<float>(type: "float", nullable: false),
                    ExitPosZ = table.Column<float>(type: "float", nullable: false),
                    Dimension = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LockNumber = table.Column<uint>(type: "int unsigned", nullable: false),
                    Locked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ProtectionLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    RobberyValue = table.Column<int>(type: "int", nullable: false),
                    RobberyCooldown = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Punishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StaffUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punishments_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Punishments_Users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    RotR = table.Column<float>(type: "float", nullable: false),
                    RotP = table.Column<float>(type: "float", nullable: false),
                    RotY = table.Column<float>(type: "float", nullable: false),
                    Color1R = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color1G = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color1B = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color2R = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color2G = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Color2B = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    Plate = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionId = table.Column<int>(type: "int", nullable: true),
                    EngineHealth = table.Column<int>(type: "int", nullable: false),
                    Livery = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SeizedValue = table.Column<int>(type: "int", nullable: false),
                    Fuel = table.Column<int>(type: "int", nullable: false),
                    StructureDamagesJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Parked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sold = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Job = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DamagesJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BodyHealth = table.Column<uint>(type: "int unsigned", nullable: false),
                    BodyAdditionalHealth = table.Column<uint>(type: "int unsigned", nullable: false),
                    PetrolTankHealth = table.Column<int>(type: "int", nullable: false),
                    LockNumber = table.Column<uint>(type: "int unsigned", nullable: false),
                    FactionGift = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ProtectionLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DismantledValue = table.Column<int>(type: "int", nullable: false),
                    XMR = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModsJSON = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WheelType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    WheelVariation = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    WheelColor = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NeonColorR = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NeonColorG = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NeonColorB = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NeonLeft = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NeonRight = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NeonFront = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NeonBack = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HeadlightColor = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    LightsMultiplier = table.Column<float>(type: "float", nullable: false),
                    WindowTint = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TireSmokeColorR = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TireSmokeColorG = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TireSmokeColorB = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfiscationsItems",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ConfiscationId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Type = table.Column<uint>(type: "int unsigned", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Extra = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiscationsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiscationsItems_Confiscations_ConfiscationId",
                        column: x => x.ConfiscationId,
                        principalTable: "Confiscations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FactionsUnitsCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FactionUnitId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsUnitsCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsUnitsCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactionsUnitsCharacters_FactionsUnits_FactionUnitId",
                        column: x => x.FactionUnitId,
                        principalTable: "FactionsUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PropertiesFurnitures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosX = table.Column<float>(type: "float", nullable: false),
                    PosY = table.Column<float>(type: "float", nullable: false),
                    PosZ = table.Column<float>(type: "float", nullable: false),
                    RotR = table.Column<float>(type: "float", nullable: false),
                    RotP = table.Column<float>(type: "float", nullable: false),
                    RotY = table.Column<float>(type: "float", nullable: false),
                    Interior = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesFurnitures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesFurnitures_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Wanted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PoliceOfficerCharacterId = table.Column<int>(type: "int", nullable: false),
                    WantedCharacterId = table.Column<int>(type: "int", nullable: true),
                    WantedVehicleId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PoliceOfficerDeletedCharacterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wanted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wanted_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wanted_Characters_WantedCharacterId",
                        column: x => x.WantedCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Wanted_Vehicles_WantedVehicleId",
                        column: x => x.WantedVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Banishments_CharacterId",
                table: "Banishments",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Banishments_StaffUserId",
                table: "Banishments",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Banishments_UserId",
                table: "Banishments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EvaluatingStaffUserId",
                table: "Characters",
                column: "EvaluatingStaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EvaluatorStaffUserId",
                table: "Characters",
                column: "EvaluatorStaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_FactionRankId",
                table: "Characters",
                column: "FactionRankId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PoliceOfficerBlockedDriverLicenseCharacterId",
                table: "Characters",
                column: "PoliceOfficerBlockedDriverLicenseCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesCharacters_CharacterId",
                table: "CompaniesCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesCharacters_CompanyId",
                table: "CompaniesCharacters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Confiscations_CharacterId",
                table: "Confiscations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Confiscations_FactionId",
                table: "Confiscations",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Confiscations_PoliceOfficerCharacterId",
                table: "Confiscations",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiscationsItems_ConfiscationId",
                table: "ConfiscationsItems",
                column: "ConfiscationId");

            migrationBuilder.CreateIndex(
                name: "IX_CrackDensSells_CharacterId",
                table: "CrackDensSells",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnits_CharacterId",
                table: "FactionsUnits",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnitsCharacters_CharacterId",
                table: "FactionsUnitsCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnitsCharacters_FactionUnitId",
                table: "FactionsUnitsCharacters",
                column: "FactionUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_CharacterId",
                table: "Fines",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_PoliceOfficerCharacterId",
                table: "Fines",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Infos_UserId",
                table: "Infos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jails_CharacterId",
                table: "Jails",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Jails_FactionId",
                table: "Jails",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Jails_PoliceOfficerCharacterId",
                table: "Jails",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OriginCharacterId",
                table: "Logs",
                column: "OriginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_TargetCharacterId",
                table: "Logs",
                column: "TargetCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CharacterId",
                table: "Properties",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesFurnitures_PropertyId",
                table: "PropertiesFurnitures",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_CharacterId",
                table: "Punishments",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_StaffUserId",
                table: "Punishments",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CharacterId",
                table: "Vehicles",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_FactionId",
                table: "Vehicles",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_PoliceOfficerCharacterId",
                table: "Wanted",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_WantedCharacterId",
                table: "Wanted",
                column: "WantedCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_WantedVehicleId",
                table: "Wanted",
                column: "WantedVehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animations");

            migrationBuilder.DropTable(
                name: "Banishments");

            migrationBuilder.DropTable(
                name: "Blips");

            migrationBuilder.DropTable(
                name: "CharactersItems");

            migrationBuilder.DropTable(
                name: "CompaniesCharacters");

            migrationBuilder.DropTable(
                name: "ConfiscationsItems");

            migrationBuilder.DropTable(
                name: "CrackDens");

            migrationBuilder.DropTable(
                name: "CrackDensItems");

            migrationBuilder.DropTable(
                name: "CrackDensSells");

            migrationBuilder.DropTable(
                name: "Doors");

            migrationBuilder.DropTable(
                name: "EmergencyCalls");

            migrationBuilder.DropTable(
                name: "FactionsArmories");

            migrationBuilder.DropTable(
                name: "FactionsArmoriesWeapons");

            migrationBuilder.DropTable(
                name: "FactionsDrugsHouses");

            migrationBuilder.DropTable(
                name: "FactionsDrugsHousesItems");

            migrationBuilder.DropTable(
                name: "FactionsUnitsCharacters");

            migrationBuilder.DropTable(
                name: "FinancialTransactions");

            migrationBuilder.DropTable(
                name: "Fines");

            migrationBuilder.DropTable(
                name: "Furnitures");

            migrationBuilder.DropTable(
                name: "HelpRequests");

            migrationBuilder.DropTable(
                name: "Infos");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Jails");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "PropertiesFurnitures");

            migrationBuilder.DropTable(
                name: "PropertiesItems");

            migrationBuilder.DropTable(
                name: "Punishments");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionsAnswers");

            migrationBuilder.DropTable(
                name: "SeizedVehicles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Spots");

            migrationBuilder.DropTable(
                name: "TruckerLocations");

            migrationBuilder.DropTable(
                name: "TruckerLocationsDeliveries");

            migrationBuilder.DropTable(
                name: "VehiclesItems");

            migrationBuilder.DropTable(
                name: "Wanted");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Confiscations");

            migrationBuilder.DropTable(
                name: "FactionsUnits");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropTable(
                name: "FactionsRanks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
