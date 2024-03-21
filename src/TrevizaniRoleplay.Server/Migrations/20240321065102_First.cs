using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrevizaniRoleplay.Server.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Display = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Dictionary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    OnlyInVehicle = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    WeekRentValue = table.Column<int>(type: "integer", nullable: false),
                    RentPaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Color = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    BlipType = table.Column<int>(type: "integer", nullable: false),
                    BlipColor = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrackDens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    OnlinePoliceOfficers = table.Column<int>(type: "integer", nullable: false),
                    CooldownQuantityLimit = table.Column<int>(type: "integer", nullable: false),
                    CooldownHours = table.Column<int>(type: "integer", nullable: false),
                    CooldownDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyCalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Color = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Slots = table.Column<int>(type: "integer", nullable: false),
                    ChatColor = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Furnitures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Furnitures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    RotR = table.Column<float>(type: "real", nullable: false),
                    RotP = table.Column<float>(type: "real", nullable: false),
                    RotY = table.Column<float>(type: "real", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxCharactersOnline = table.Column<int>(type: "integer", nullable: false),
                    VehicleParkValue = table.Column<int>(type: "integer", nullable: false),
                    HospitalValue = table.Column<int>(type: "integer", nullable: false),
                    BarberValue = table.Column<int>(type: "integer", nullable: false),
                    ClothesValue = table.Column<int>(type: "integer", nullable: false),
                    DriverLicenseBuyValue = table.Column<int>(type: "integer", nullable: false),
                    DriverLicenseRenewValue = table.Column<int>(type: "integer", nullable: false),
                    FuelValue = table.Column<int>(type: "integer", nullable: false),
                    Paycheck = table.Column<int>(type: "integer", nullable: false),
                    AnnouncementValue = table.Column<int>(type: "integer", nullable: false),
                    ExtraPaymentGarbagemanValue = table.Column<int>(type: "integer", nullable: false),
                    Blackout = table.Column<bool>(type: "boolean", nullable: false),
                    InactivePropertiesDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    KeyValue = table.Column<int>(type: "integer", nullable: false),
                    LockValue = table.Column<int>(type: "integer", nullable: false),
                    IPLsJSON = table.Column<string>(type: "text", nullable: false),
                    TattooValue = table.Column<int>(type: "integer", nullable: false),
                    CooldownDismantleHours = table.Column<int>(type: "integer", nullable: false),
                    PropertyRobberyConnectedTime = table.Column<int>(type: "integer", nullable: false),
                    CooldownPropertyRobberyRobberHours = table.Column<int>(type: "integer", nullable: false),
                    CooldownPropertyRobberyPropertyHours = table.Column<int>(type: "integer", nullable: false),
                    PoliceOfficersPropertyRobbery = table.Column<int>(type: "integer", nullable: false),
                    InitialTimeCrackDen = table.Column<byte>(type: "smallint", nullable: false),
                    EndTimeCrackDen = table.Column<byte>(type: "smallint", nullable: false),
                    FirefightersBlockHeal = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    AuxiliarPosX = table.Column<float>(type: "real", nullable: false),
                    AuxiliarPosY = table.Column<float>(type: "real", nullable: false),
                    AuxiliarPosZ = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TruckerLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    DeliveryValue = table.Column<int>(type: "integer", nullable: false),
                    LoadWaitTime = table.Column<int>(type: "integer", nullable: false),
                    UnloadWaitTime = table.Column<int>(type: "integer", nullable: false),
                    AllowedVehiclesJSON = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckerLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscordId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DiscordUsername = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DiscordDisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegisterIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegisterHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RegisterHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastAccessIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastAccessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastAccessHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastAccessHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Staff = table.Column<byte>(type: "smallint", nullable: false),
                    NameChanges = table.Column<int>(type: "integer", nullable: false),
                    HelpRequestsAnswersQuantity = table.Column<int>(type: "integer", nullable: false),
                    StaffDutyTime = table.Column<int>(type: "integer", nullable: false),
                    TimeStampToggle = table.Column<bool>(type: "boolean", nullable: false),
                    VIP = table.Column<byte>(type: "smallint", nullable: false),
                    VIPValidDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ForumNameChanges = table.Column<int>(type: "integer", nullable: false),
                    PMToggle = table.Column<bool>(type: "boolean", nullable: false),
                    StaffChatToggle = table.Column<bool>(type: "boolean", nullable: false),
                    FactionChatToggle = table.Column<bool>(type: "boolean", nullable: false),
                    PlateChanges = table.Column<int>(type: "integer", nullable: false),
                    AnnouncementToggle = table.Column<bool>(type: "boolean", nullable: false),
                    VehicleTagToggle = table.Column<bool>(type: "boolean", nullable: false),
                    ChatFontType = table.Column<int>(type: "integer", nullable: false),
                    ChatLines = table.Column<int>(type: "integer", nullable: false),
                    ChatFontSize = table.Column<int>(type: "integer", nullable: false),
                    StaffFlagsJSON = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FactionToggle = table.Column<bool>(type: "boolean", nullable: false),
                    CharacterApplicationsQuantity = table.Column<int>(type: "integer", nullable: false),
                    CooldownDismantle = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyRobberyCooldown = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AnsweredQuestions = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrackDensItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CrackDenId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemCategory = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDensItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrackDensItems_CrackDens_CrackDenId",
                        column: x => x.CrackDenId,
                        principalTable: "CrackDens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Hash = table.Column<long>(type: "bigint", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Locked = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Doors_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactionsRanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Salary = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsRanks_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactionsStorages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsStorages_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TruckerLocationsDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TruckerLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckerLocationsDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TruckerLocationsDeliveries_TruckerLocations_TruckerLocation~",
                        column: x => x.TruckerLocationId,
                        principalTable: "TruckerLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HelpRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StaffUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpRequests_Users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HelpRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Infos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegisterIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegisterHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RegisterHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastAccessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastAccessIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastAccessHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastAccessHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Model = table.Column<long>(type: "bigint", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    Armor = table.Column<int>(type: "integer", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    BirthdayDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConnectedTime = table.Column<int>(type: "integer", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FactionRankId = table.Column<Guid>(type: "uuid", nullable: true),
                    Bank = table.Column<int>(type: "integer", nullable: false),
                    IPLsJSON = table.Column<string>(type: "text", nullable: false),
                    DeathDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeathReason = table.Column<string>(type: "text", nullable: false),
                    Job = table.Column<byte>(type: "smallint", nullable: false),
                    PersonalizationJSON = table.Column<string>(type: "text", nullable: true),
                    History = table.Column<string>(type: "text", nullable: false),
                    EvaluatingStaffUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    EvaluatorStaffUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: false),
                    NameChangeStatus = table.Column<byte>(type: "smallint", nullable: false),
                    PersonalizationStep = table.Column<byte>(type: "smallint", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    JailFinalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DriverLicenseValidDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PoliceOfficerBlockedDriverLicenseCharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Badge = table.Column<int>(type: "integer", nullable: false),
                    AnnouncementLastUseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Savings = table.Column<int>(type: "integer", nullable: false),
                    ExtraPayment = table.Column<int>(type: "integer", nullable: false),
                    WoundsJSON = table.Column<string>(type: "text", nullable: false),
                    Wound = table.Column<byte>(type: "smallint", nullable: false),
                    Sex = table.Column<byte>(type: "smallint", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Mask = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FactionFlagsJSON = table.Column<string>(type: "text", nullable: false),
                    DrugItemCategory = table.Column<byte>(type: "smallint", nullable: true),
                    DrugEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ThresoldDeath = table.Column<byte>(type: "smallint", nullable: false),
                    ThresoldDeathEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CKAvaliation = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_PoliceOfficerBlockedDriverLicenseChar~",
                        column: x => x.PoliceOfficerBlockedDriverLicenseCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_FactionsRanks_FactionRankId",
                        column: x => x.FactionRankId,
                        principalTable: "FactionsRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Users_EvaluatingStaffUserId",
                        column: x => x.EvaluatingStaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Users_EvaluatorStaffUserId",
                        column: x => x.EvaluatorStaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactionsStoragesItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionStorageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsStoragesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsStoragesItems_FactionsStorages_FactionStorageId",
                        column: x => x.FactionStorageId,
                        principalTable: "FactionsStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Banishments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StaffUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banishments_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banishments_Users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banishments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharactersItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<short>(type: "smallint", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharactersItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharactersItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompaniesCharacters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlagsJSON = table.Column<string>(type: "text", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Confiscations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoliceOfficerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confiscations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Confiscations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Confiscations_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Confiscations_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrackDensSells",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CrackDenId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ItemCategory = table.Column<byte>(type: "smallint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrackDensSells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrackDensSells_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrackDensSells_CrackDens_CrackDenId",
                        column: x => x.CrackDenId,
                        principalTable: "CrackDens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactionsUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Description = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Plate = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsUnits_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactionsUnits_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialTransactions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoliceOfficerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fines_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fines_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoliceOfficerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jails_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jails_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jails_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OriginCharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OriginHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    OriginHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TargetCharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TargetHardwareIdHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TargetHardwareIdExHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Characters_OriginCharacterId",
                        column: x => x.OriginCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_Characters_TargetCharacterId",
                        column: x => x.TargetCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Interior = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntrancePosX = table.Column<float>(type: "real", nullable: false),
                    EntrancePosY = table.Column<float>(type: "real", nullable: false),
                    EntrancePosZ = table.Column<float>(type: "real", nullable: false),
                    EntranceDimension = table.Column<int>(type: "integer", nullable: false),
                    ExitPosX = table.Column<float>(type: "real", nullable: false),
                    ExitPosY = table.Column<float>(type: "real", nullable: false),
                    ExitPosZ = table.Column<float>(type: "real", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    LockNumber = table.Column<long>(type: "bigint", nullable: false),
                    Locked = table.Column<bool>(type: "boolean", nullable: false),
                    ProtectionLevel = table.Column<byte>(type: "smallint", nullable: false),
                    RobberyValue = table.Column<int>(type: "integer", nullable: false),
                    RobberyCooldown = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Punishments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StaffUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punishments_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Punishments_Users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    InitialDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    RotR = table.Column<float>(type: "real", nullable: false),
                    RotP = table.Column<float>(type: "real", nullable: false),
                    RotY = table.Column<float>(type: "real", nullable: false),
                    Color1R = table.Column<byte>(type: "smallint", nullable: false),
                    Color1G = table.Column<byte>(type: "smallint", nullable: false),
                    Color1B = table.Column<byte>(type: "smallint", nullable: false),
                    Color2R = table.Column<byte>(type: "smallint", nullable: false),
                    Color2G = table.Column<byte>(type: "smallint", nullable: false),
                    Color2B = table.Column<byte>(type: "smallint", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Plate = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EngineHealth = table.Column<int>(type: "integer", nullable: false),
                    Livery = table.Column<byte>(type: "smallint", nullable: false),
                    SeizedValue = table.Column<int>(type: "integer", nullable: false),
                    Fuel = table.Column<int>(type: "integer", nullable: false),
                    StructureDamagesJSON = table.Column<string>(type: "text", nullable: false),
                    Parked = table.Column<bool>(type: "boolean", nullable: false),
                    Sold = table.Column<bool>(type: "boolean", nullable: false),
                    Job = table.Column<byte>(type: "smallint", nullable: false),
                    DamagesJSON = table.Column<string>(type: "text", nullable: false),
                    BodyHealth = table.Column<long>(type: "bigint", nullable: false),
                    BodyAdditionalHealth = table.Column<long>(type: "bigint", nullable: false),
                    PetrolTankHealth = table.Column<int>(type: "integer", nullable: false),
                    LockNumber = table.Column<long>(type: "bigint", nullable: false),
                    StaffGift = table.Column<bool>(type: "boolean", nullable: false),
                    ProtectionLevel = table.Column<byte>(type: "smallint", nullable: false),
                    DismantledValue = table.Column<int>(type: "integer", nullable: false),
                    XMR = table.Column<bool>(type: "boolean", nullable: false),
                    ModsJSON = table.Column<string>(type: "text", nullable: false),
                    WheelType = table.Column<byte>(type: "smallint", nullable: false),
                    WheelVariation = table.Column<byte>(type: "smallint", nullable: false),
                    WheelColor = table.Column<byte>(type: "smallint", nullable: false),
                    NeonColorR = table.Column<byte>(type: "smallint", nullable: false),
                    NeonColorG = table.Column<byte>(type: "smallint", nullable: false),
                    NeonColorB = table.Column<byte>(type: "smallint", nullable: false),
                    NeonLeft = table.Column<bool>(type: "boolean", nullable: false),
                    NeonRight = table.Column<bool>(type: "boolean", nullable: false),
                    NeonFront = table.Column<bool>(type: "boolean", nullable: false),
                    NeonBack = table.Column<bool>(type: "boolean", nullable: false),
                    HeadlightColor = table.Column<byte>(type: "smallint", nullable: false),
                    LightsMultiplier = table.Column<float>(type: "real", nullable: false),
                    WindowTint = table.Column<byte>(type: "smallint", nullable: false),
                    TireSmokeColorR = table.Column<byte>(type: "smallint", nullable: false),
                    TireSmokeColorG = table.Column<byte>(type: "smallint", nullable: false),
                    TireSmokeColorB = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiscationsItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfiscationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiscationsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiscationsItems_Confiscations_ConfiscationId",
                        column: x => x.ConfiscationId,
                        principalTable: "Confiscations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactionsUnitsCharacters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FactionUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionsUnitsCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionsUnitsCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactionsUnitsCharacters_FactionsUnits_FactionUnitId",
                        column: x => x.FactionUnitId,
                        principalTable: "FactionsUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesFurnitures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    PosZ = table.Column<float>(type: "real", nullable: false),
                    RotR = table.Column<float>(type: "real", nullable: false),
                    RotP = table.Column<float>(type: "real", nullable: false),
                    RotY = table.Column<float>(type: "real", nullable: false),
                    Interior = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesFurnitures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesFurnitures_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<byte>(type: "smallint", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesItems_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeizedVehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoliceOfficerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeizedVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeizedVehicles_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeizedVehicles_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeizedVehicles_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<byte>(type: "smallint", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclesItems_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wanted",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PoliceOfficerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WantedCharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    WantedVehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PoliceOfficerDeletedCharacterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wanted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wanted_Characters_PoliceOfficerCharacterId",
                        column: x => x.PoliceOfficerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wanted_Characters_PoliceOfficerDeletedCharacterId",
                        column: x => x.PoliceOfficerDeletedCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wanted_Characters_WantedCharacterId",
                        column: x => x.WantedCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wanted_Vehicles_WantedVehicleId",
                        column: x => x.WantedVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CorrectQuestionAnswerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Characters_FactionId",
                table: "Characters",
                column: "FactionId");

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
                name: "IX_CharactersItems_CharacterId",
                table: "CharactersItems",
                column: "CharacterId");

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
                name: "IX_CrackDensItems_CrackDenId",
                table: "CrackDensItems",
                column: "CrackDenId");

            migrationBuilder.CreateIndex(
                name: "IX_CrackDensSells_CharacterId",
                table: "CrackDensSells",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CrackDensSells_CrackDenId",
                table: "CrackDensSells",
                column: "CrackDenId");

            migrationBuilder.CreateIndex(
                name: "IX_Doors_CompanyId",
                table: "Doors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Doors_FactionId",
                table: "Doors",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsRanks_FactionId",
                table: "FactionsRanks",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsStorages_FactionId",
                table: "FactionsStorages",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsStoragesItems_FactionStorageId",
                table: "FactionsStoragesItems",
                column: "FactionStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnits_CharacterId",
                table: "FactionsUnits",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnits_FactionId",
                table: "FactionsUnits",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnitsCharacters_CharacterId",
                table: "FactionsUnitsCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FactionsUnitsCharacters_FactionUnitId",
                table: "FactionsUnitsCharacters",
                column: "FactionUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_CharacterId",
                table: "FinancialTransactions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_CharacterId",
                table: "Fines",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_PoliceOfficerCharacterId",
                table: "Fines",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_StaffUserId",
                table: "HelpRequests",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_UserId",
                table: "HelpRequests",
                column: "UserId");

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
                name: "IX_PropertiesItems_PropertyId",
                table: "PropertiesItems",
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
                name: "IX_Questions_CorrectQuestionAnswerId",
                table: "Questions",
                column: "CorrectQuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswers_QuestionId",
                table: "QuestionsAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SeizedVehicles_FactionId",
                table: "SeizedVehicles",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SeizedVehicles_PoliceOfficerCharacterId",
                table: "SeizedVehicles",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SeizedVehicles_VehicleId",
                table: "SeizedVehicles",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CharacterId",
                table: "Sessions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckerLocationsDeliveries_TruckerLocationId",
                table: "TruckerLocationsDeliveries",
                column: "TruckerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CharacterId",
                table: "Vehicles",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_FactionId",
                table: "Vehicles",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesItems_VehicleId",
                table: "VehiclesItems",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_PoliceOfficerCharacterId",
                table: "Wanted",
                column: "PoliceOfficerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_PoliceOfficerDeletedCharacterId",
                table: "Wanted",
                column: "PoliceOfficerDeletedCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_WantedCharacterId",
                table: "Wanted",
                column: "WantedCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Wanted_WantedVehicleId",
                table: "Wanted",
                column: "WantedVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_QuestionsAnswers_CorrectQuestionAnswerId",
                table: "Questions",
                column: "CorrectQuestionAnswerId",
                principalTable: "QuestionsAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_QuestionsAnswers_CorrectQuestionAnswerId",
                table: "Questions");

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
                name: "CrackDensItems");

            migrationBuilder.DropTable(
                name: "CrackDensSells");

            migrationBuilder.DropTable(
                name: "Doors");

            migrationBuilder.DropTable(
                name: "EmergencyCalls");

            migrationBuilder.DropTable(
                name: "FactionsStoragesItems");

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
                name: "SeizedVehicles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Spots");

            migrationBuilder.DropTable(
                name: "TruckerLocationsDeliveries");

            migrationBuilder.DropTable(
                name: "VehiclesItems");

            migrationBuilder.DropTable(
                name: "Wanted");

            migrationBuilder.DropTable(
                name: "Confiscations");

            migrationBuilder.DropTable(
                name: "CrackDens");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "FactionsStorages");

            migrationBuilder.DropTable(
                name: "FactionsUnits");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "TruckerLocations");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "FactionsRanks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropTable(
                name: "QuestionsAnswers");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
