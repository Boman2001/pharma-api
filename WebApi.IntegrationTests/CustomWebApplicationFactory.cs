using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core.Domain;
using Core.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using WebApi.IntegrationTests.Helper;

namespace WebApi.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class

    {
        private List<IdentityUser> users;
        private List<UserInformation> userInformations;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            users = new List<IdentityUser>();
            userInformations = new List<UserInformation>();
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
            admin.PasswordHash = passwordHasher.HashPassword(admin,"password");
            users.Add(admin);
            users.Add(user1);
            users.Add(user2);
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

            userInformations.Add(userinformation);
            userInformations.Add(userinformation1);
            userInformations.Add(userinformation2);


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
            db.Users.AddRange(users);
            db.SaveChanges();
            dbdata.UserInformation.AddRange(userInformations);
            dbdata.SaveChanges();
        }

        private async void setRole(UserManager<IdentityUser> userManager, IdentityUser admin)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
