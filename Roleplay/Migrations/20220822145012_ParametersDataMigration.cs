using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleplay.Migrations
{
    public partial class ParametersDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "AnnouncementValue", "BarberValue", "Blackout", "ClothesValue", "CooldownDismantleHours", "CooldownPropertyRobberyPropertyHours", "CooldownPropertyRobberyRobberHours", "DriverLicenseBuyValue", "DriverLicenseRenewValue", "EndTimeCrackDen", "ExtraPaymentGarbagemanValue", "FirefightersBlockHeal", "FuelValue", "HospitalValue", "IPLsJSON", "InactivePropertiesDate", "InitialTimeCrackDen", "KeyValue", "LockValue", "MaxCharactersOnline", "Paycheck", "PoliceOfficersPropertyRobbery", "PropertyRobberyConnectedTime", "TattooValue", "VehicleParkValue" },
                values: new object[] { 1, 1, 1, false, 1, 0, 0, 0, 1, 1, (byte)0, 1, 0, 1, 1, "[]", null, (byte)0, 1, 1, 0, 1, 0, 0, 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
