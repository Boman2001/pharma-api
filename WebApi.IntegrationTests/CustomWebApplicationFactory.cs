using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Enums;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core.Domain.Models;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using WebApi.IntegrationTests.Helper;

namespace WebApi.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class

    {
        private List<IdentityUser> _users;
        private List<UserInformation> _userInformations;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SeedUsers();
            builder.ConfigureTestServices(services =>
            {
                services.AddMvc(options =>
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                    options.Filters.Add(new FakeUserFilter());
                })
                    .AddApplicationPart(typeof(Startup).Assembly);
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            });


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
                var sp
                    = services.BuildServiceProvider();
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


        public void InitializeDbForTests(SecurityDbContext db, ApplicationDbContext dbdata)
        {
            db.Users.AddRange(_users);
            db.SaveChanges();
            dbdata.UserInformation.AddRange(_userInformations);
            dbdata.Activities.AddRange(GetActivities());
            dbdata.Consultations.AddRange(getConsultations());
            dbdata.SaveChanges();
        }

        private List<Consultation> getConsultations()
        {
            Patient p = new Patient
            {
                Name = "jim",
                Bsn = "bsn",
                Email = "jim@jim.com",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "124124",
                City = "hank",
                Street = "lepelaarstraat",
                HouseNumber = "20",
                HouseNumberAddon = "",
                PostalCode = "4273cv",
                Country = "Netherlands"
            };
          
            AdditionalExaminationType type = new AdditionalExaminationType
            {
                Name = "typename",
                Unit = "GPS"
            };
            AdditionalExaminationResult additional = new AdditionalExaminationResult
            {
                Value = "value",
                Date = DateTime.Now,
                AdditionalExaminationType = type
            };
            IcpcCode ipCode = new IcpcCode
            {
                Name = "Name",
                Code = "code"
            };
            Episode ep = new Episode
            {
                Description = "Description",
                Priority = 10,
                Patient = p,
                IcpcCode = ipCode
            };
            Intolerance intolerances = new Intolerance
            {
                Description = "descrption",
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Patient = p
            };
            PhysicalExamination physical = new PhysicalExamination()
            {
                Value = "physical",
                Date = DateTime.Now,
                Patient = p,
            };
            Consultation c = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(_users[0].Id),
                Doctor = _users[0],
                Patient = p,
                AdditionalExaminationResults = new List<AdditionalExaminationResult>()
                {
                    additional
                },
                Episodes = new List<Episode>()
                {
                    ep
                },
                Intolerances = new List<Intolerance>()
                {
                    intolerances
                },
                PhysicalExaminations = new List<PhysicalExamination>()
                {
                    physical
                }
            };
            return new List<Consultation>(){c};
        }

        private List<Activity> GetActivities()
        {
            var activities = new List<Activity>();
            var activity = new Activity
            {
                Description = "Description",
                Properties = "Properties",
                SubjectId = 10,
                SubjectType = "Type"
            };
            var activity02 = new Activity
            {
                Description = "Description",
                Properties = "Properties",
                SubjectId = 10,
                SubjectType = "Type"
            };
            activities.Add(activity);
            activities.Add(activity02);
            return activities;
        }

        private void SeedUsers()
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            _users = new List<IdentityUser>();
            _userInformations = new List<UserInformation>();
            var admin = new IdentityUser
            {
                Id = "6105002a-295f-49b1-ace3-2072c7edbb69",
                UserName = "m@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                Email = "m@gmail.com",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };

            var user1 = new IdentityUser
            {
                Id = "6105002a-295f-49b1-ace3-2072c7edbb61",
                UserName = "m1@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M1@GMAIL.COM",
                Email = "m1@gmail.com",
                NormalizedEmail = "M1@GMAIL.COM",
                EmailConfirmed = true,
            };

            var user2 = new IdentityUser
            {
                Id = "6105002a-295f-49b1-ace3-2072c7edbb62",
                UserName = "m2@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M2@GMAIL.COM",
                Email = "m2@gmail.com",
                NormalizedEmail = "M2@GMAIL.COM",
                EmailConfirmed = true,
            };
            user1.PasswordHash = passwordHasher.HashPassword(user1, "password");
            user2.PasswordHash = passwordHasher.HashPassword(user2, "password");
            admin.PasswordHash = passwordHasher.HashPassword(admin, "password");
            _users.Add(admin);
            _users.Add(user1);
            _users.Add(user2);
            var userinformation = new UserInformation
            {
                UserId = Guid.Parse(admin.Id),
                Name = "maartena@gmail.coma",
                City = "Hank",
                Country = "Netherlands"
            };

            var userinformation1 = new UserInformation
            {
                UserId = Guid.Parse(user1.Id),
                Name = "m1@gmail.com",
                City = "Hank",
                Country = "Netherlands"
            };
            var userinformation2 = new UserInformation
            {
                UserId = Guid.Parse(user2.Id),
                Name = "m2@gmail.com",
                City = "Hank",
                Country = "Netherlands"
            };

            _userInformations.Add(userinformation);
            _userInformations.Add(userinformation1);
            _userInformations.Add(userinformation2);
        }
    }
}