using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations.ApplicationDb
{
    public partial class AddLatLongToPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserInformation");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longditude",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Longditude",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserInformation",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
