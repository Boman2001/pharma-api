using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations.ApplicationDb
{
    public partial class AddLatLongToPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PhoneNumber",
                "UserInformation");

            migrationBuilder.AddColumn<double>(
                "Latitude",
                "Patients",
                "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                "Longditude",
                "Patients",
                "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Latitude",
                "Patients");

            migrationBuilder.DropColumn(
                "Longditude",
                "Patients");

            migrationBuilder.AddColumn<string>(
                "PhoneNumber",
                "UserInformation",
                "nvarchar(max)",
                nullable: true);
        }
    }
}