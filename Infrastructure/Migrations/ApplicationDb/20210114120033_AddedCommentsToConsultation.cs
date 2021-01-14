using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations.ApplicationDb
{
    public partial class AddedCommentsToConsultation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Consultations");
        }
    }
}
