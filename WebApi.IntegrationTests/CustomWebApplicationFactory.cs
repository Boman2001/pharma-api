using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core.Domain;
using Core.DomainServices.Helpers;
using Microsoft.AspNetCore.Identity;

namespace WebApi.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<ApplicationDbContext>));

                var descriptorSecurityDb = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<SecurityDbContext>));

                services.Remove(descriptor);
                services.Remove(descriptorSecurityDb);

                var provider = services
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDatadbForTesting");
                    options.UseInternalServiceProvider(provider);
                });

                services.AddDbContext<SecurityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(provider);
                });
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var securedb = scopedServices.GetRequiredService<SecurityDbContext>();
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();


                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                db.Database.EnsureCreated();
                securedb.Database.EnsureCreated();

                try
                {
                    InitializeDbForTests(securedb, db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        "database with test messages. Error: {Message}", ex.Message);
                }
            });
        }

        public static void InitializeDbForTests(SecurityDbContext db, ApplicationDbContext dbdata)
        {
            db.Users.AddRange(GetSeedingIdentityUsers());
            db.SaveChanges();
            dbdata.UserInformation.AddRange(GetSeedingDoctors());
            dbdata.SaveChanges();
        }

        public static void ReinitializeDbForTests(SecurityDbContext db, ApplicationDbContext dbdata)
        {
            db.Users.RemoveRange(db.Users);
            dbdata.UserInformation.RemoveRange(dbdata.UserInformation);
            InitializeDbForTests(db, dbdata);
        }

        public static List<IdentityUser> GetSeedingIdentityUsers()
        {
            return new List<IdentityUser>()
            {
                new IdentityUser {Id = new Guid("###1").ToString(), UserName = "maartena@gmail.coma", PasswordHash = AuthHelper.HashPassword("LEANDER223"), Email = "MAARTENA@GMAIL.COMA"},
                new IdentityUser {Id = new Guid("###2").ToString(), UserName = "maartenv@gmail.coma", PasswordHash = "m", Email = "MAARTENV@GMAIL.CMA"},
                new IdentityUser {Id = new Guid("###3").ToString(), UserName = "d@LEander.com", PasswordHash = "m", Email = "d@LEander.com", NormalizedEmail = "D@LEANDER.COM"}
            };
        }

        public static List<UserInformation> GetSeedingDoctors()
        {
            return new List<UserInformation>
            {
                new UserInformation {Id = 1, UserId = new Guid("###1")},
                new UserInformation {Id = 2, UserId = new Guid("###2")},
                new UserInformation {Id = 3, UserId = new Guid("###3")},
                new UserInformation {UserId = new Guid("###3")},
                new UserInformation {UserId = new Guid("###3")},
                new UserInformation {UserId = new Guid("###3")},
            };
        }
    }
}
