using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations.ApplicationDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Activities",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    Properties = table.Column<string>("nvarchar(max)", nullable: true),
                    SubjectId = table.Column<int>("int", nullable: false),
                    CauserId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Activities", x => x.Id); });

            migrationBuilder.CreateTable(
                "AdditionalExaminationTypes",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Unit = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AdditionalExaminationTypes", x => x.Id); });

            migrationBuilder.CreateTable(
                "ExaminationTypes",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Unit = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_ExaminationTypes", x => x.Id); });

            migrationBuilder.CreateTable(
                "IcpcCodes",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Code = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_IcpcCodes", x => x.Id); });

            migrationBuilder.CreateTable(
                "Patients",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Bsn = table.Column<string>("nvarchar(max)", nullable: true),
                    Email = table.Column<string>("nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>("datetime2", nullable: false),
                    Gender = table.Column<int>("int", nullable: false),
                    PhoneNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    City = table.Column<string>("nvarchar(max)", nullable: true),
                    Street = table.Column<string>("nvarchar(max)", nullable: true),
                    HouseNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    HouseNumberAddon = table.Column<string>("nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>("nvarchar(max)", nullable: true),
                    Country = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Patients", x => x.Id); });

            migrationBuilder.CreateTable(
                "UserInformation",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Bsn = table.Column<string>("nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>("datetime2", nullable: false),
                    Gender = table.Column<int>("int", nullable: false),
                    PhoneNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    City = table.Column<string>("nvarchar(max)", nullable: true),
                    Street = table.Column<string>("nvarchar(max)", nullable: true),
                    HouseNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    HouseNumberAddon = table.Column<string>("nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>("nvarchar(max)", nullable: true),
                    Country = table.Column<string>("nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_UserInformation", x => x.Id); });

            migrationBuilder.CreateTable(
                "Consultations",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>("datetime2", nullable: false),
                    DoctorId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.Id);
                    table.ForeignKey(
                        "FK_Consultations_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AdditionalExaminationResults",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>("nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>("datetime2", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    AdditionalExaminationTypeId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalExaminationResults", x => x.Id);
                    table.ForeignKey(
                        "FK_AdditionalExaminationResults_AdditionalExaminationTypes_AdditionalExaminationTypeId",
                        x => x.AdditionalExaminationTypeId,
                        "AdditionalExaminationTypes",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AdditionalExaminationResults_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_AdditionalExaminationResults_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Episodes",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    Priority = table.Column<int>("int", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    IcpcCodeId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        "FK_Episodes_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Episodes_IcpcCodes_IcpcCodeId",
                        x => x.IcpcCodeId,
                        "IcpcCodes",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Episodes_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Intolerances",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>("datetime2", nullable: false),
                    EndDate = table.Column<DateTime>("datetime2", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intolerances", x => x.Id);
                    table.ForeignKey(
                        "FK_Intolerances_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Intolerances_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PhysicalExaminations",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>("nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>("datetime2", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    ExaminationTypeId = table.Column<int>("int", nullable: false),
                    PhysicalExaminationTypeId = table.Column<int>("int", nullable: true),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExaminations", x => x.Id);
                    table.ForeignKey(
                        "FK_PhysicalExaminations_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PhysicalExaminations_ExaminationTypes_PhysicalExaminationTypeId",
                        x => x.PhysicalExaminationTypeId,
                        "ExaminationTypes",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PhysicalExaminations_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Prescriptions",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>("datetime2", nullable: false),
                    EndDate = table.Column<DateTime>("datetime2", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        "FK_Prescriptions_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Prescriptions_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserJournals",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    Property = table.Column<int>("int", nullable: false),
                    ConsultationId = table.Column<int>("int", nullable: false),
                    PatientId = table.Column<int>("int", nullable: false),
                    CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>("datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>("datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJournals", x => x.Id);
                    table.ForeignKey(
                        "FK_UserJournals_Consultations_ConsultationId",
                        x => x.ConsultationId,
                        "Consultations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_UserJournals_Patients_PatientId",
                        x => x.PatientId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_AdditionalExaminationResults_AdditionalExaminationTypeId",
                "AdditionalExaminationResults",
                "AdditionalExaminationTypeId");

            migrationBuilder.CreateIndex(
                "IX_AdditionalExaminationResults_ConsultationId",
                "AdditionalExaminationResults",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_AdditionalExaminationResults_PatientId",
                "AdditionalExaminationResults",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_Consultations_PatientId",
                "Consultations",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_Episodes_ConsultationId",
                "Episodes",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_Episodes_IcpcCodeId",
                "Episodes",
                "IcpcCodeId");

            migrationBuilder.CreateIndex(
                "IX_Episodes_PatientId",
                "Episodes",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_Intolerances_ConsultationId",
                "Intolerances",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_Intolerances_PatientId",
                "Intolerances",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_PhysicalExaminations_ConsultationId",
                "PhysicalExaminations",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_PhysicalExaminations_PatientId",
                "PhysicalExaminations",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_PhysicalExaminations_PhysicalExaminationTypeId",
                "PhysicalExaminations",
                "PhysicalExaminationTypeId");

            migrationBuilder.CreateIndex(
                "IX_Prescriptions_ConsultationId",
                "Prescriptions",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_Prescriptions_PatientId",
                "Prescriptions",
                "PatientId");

            migrationBuilder.CreateIndex(
                "IX_UserJournals_ConsultationId",
                "UserJournals",
                "ConsultationId");

            migrationBuilder.CreateIndex(
                "IX_UserJournals_PatientId",
                "UserJournals",
                "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Activities");

            migrationBuilder.DropTable(
                "AdditionalExaminationResults");

            migrationBuilder.DropTable(
                "Episodes");

            migrationBuilder.DropTable(
                "Intolerances");

            migrationBuilder.DropTable(
                "PhysicalExaminations");

            migrationBuilder.DropTable(
                "Prescriptions");

            migrationBuilder.DropTable(
                "UserInformation");

            migrationBuilder.DropTable(
                "UserJournals");

            migrationBuilder.DropTable(
                "AdditionalExaminationTypes");

            migrationBuilder.DropTable(
                "IcpcCodes");

            migrationBuilder.DropTable(
                "ExaminationTypes");

            migrationBuilder.DropTable(
                "Consultations");

            migrationBuilder.DropTable(
                "Patients");
        }
    }
}