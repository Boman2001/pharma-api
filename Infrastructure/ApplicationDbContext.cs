using System;
using Core.Domain;
using Core.DomainServices.QueryExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
        public DbSet<AdditionalExaminationType> AdditionalExaminationTypes { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<PhysicalExaminationType> ExaminationTypes { get; set; }
        public DbSet<IcpcCode> IcpcCodes { get; set; }
        public DbSet<Intolerance> Intolerances { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PhysicalExamination> PhysicalExaminations { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }
        public DbSet<UserJournal> UserJournals { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : base(contextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Activity>().ToTable("Activities");
            builder.Entity<AdditionalExaminationResult>().ToTable("AdditionalExaminationResults");
            builder.Entity<AdditionalExaminationType>().ToTable("AdditionalExaminationTypes");
            builder.Entity<Consultation>().ToTable("Consultations");
            builder.Entity<Episode>().ToTable("Episodes");
            builder.Entity<PhysicalExaminationType>().ToTable("ExaminationTypes");
            builder.Entity<IcpcCode>().ToTable("IcpcCodes");
            builder.Entity<Intolerance>().ToTable("Intolerances");
            builder.Entity<Patient>().ToTable("Patients");
            builder.Entity<PhysicalExamination>().ToTable("PhysicalExaminations");
            builder.Entity<Prescription>().ToTable("Prescriptions");
            builder.Entity<UserInformation>().ToTable("UserInformation");
            builder.Entity<UserJournal>().ToTable("UserJournals");

            builder.Entity<Episode>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.Episodes)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Intolerance>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.Intolerances)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PhysicalExamination>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.PhysicalExaminations)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Prescription>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.Prescriptions)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserJournal>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.UserJournals)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdditionalExaminationResult>()
                .HasOne(a => a.Consultation)
                .WithMany(c => c.AdditionalExaminationResults)
                .OnDelete(DeleteBehavior.Restrict);
            
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IBaseEntitySoftDeletes).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }
        }
    }
}
