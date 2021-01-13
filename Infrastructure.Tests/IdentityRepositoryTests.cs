using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApi.Tests.Mocks;
using Xunit;
using Assert = Xunit.Assert;

namespace Infrastructure.Tests
{
    public class IdentityRepositoryTests
    {
        private IdentityUser _fakeIdentityUser;
        private List<IdentityUser> _fakeIdentityUsers;

        private IdentityUser _fakeUser;
        private IdentityRepository Controller { get; }

        public IdentityRepositoryTests()
        {
            SeedData();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<SecurityDbContext>()
                .UseInMemoryDatabase(databaseName: "PharmaPartnersIdentityDb")
                .Options;

            var fakeSecurityDbContext = new Mock<SecurityDbContext>(options).Object;
            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            Controller = new IdentityRepository(userManager, signInManager, config, fakeSecurityDbContext);
        }

        [Trait("Category", "Identity Register")]
        [Fact]
        public async Task Register()
        {
           var identityUser = new IdentityUser
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "email1d@gmail.com"
            };
            var count = _fakeIdentityUsers.Count;

            var register = await Controller.Register(identityUser, identityUser.PasswordHash);

            Assert.IsType<JwtSecurityToken>(register);
            Assert.Equal(count + 1, _fakeIdentityUsers.Count);
            Assert.Equal(_fakeIdentityUsers[count].Email, identityUser.Email);
        }

        [Trait("Category", "Identity Register")]
        [Fact]
        public async Task Register_Empty()
        {
            var emptyUser = new IdentityUser();

            await Assert.ThrowsAsync<ArgumentNullException>(() => Controller.Register(emptyUser, _fakeUser.PasswordHash));
        }

        [Trait("Category", "Identity Register")]
        [Fact]
        public async Task Register_User_Already_Exists()
        {
            await Assert.ThrowsAsync<Exception>(() => Controller.Register(_fakeIdentityUsers[0], _fakeUser.PasswordHash));
        }

        [Trait("Category", "Identity Login")]
        [Fact]
        public async Task Login()
        {
            var login = await Controller.Login(_fakeIdentityUsers[0], _fakeIdentityUsers[0].PasswordHash);

            Assert.IsType<JwtSecurityToken>(login);
        }

        [Trait("Category", "Identity Login")]
        [Fact]
        public async Task Login_Empty()
        {
            var emptyUser = new IdentityUser();

            await Assert.ThrowsAsync<Exception>(() => Controller.Login(emptyUser, _fakeUser.PasswordHash));
        }

        [Trait("Category", "Error Handling")]
        [Fact]
        public void Error_Handling()
        {
            var errors = new List<IdentityError>()
            {
                new IdentityError {Code = "1", Description = "First error"},
                new IdentityError {Code = "2", Description = "Second validation error"}
            };
            var identityResult = IdentityResult.Failed(errors.ToArray());
            Assert.Throws<AggregateException>(() => IdentityRepository.ErrorHandling(identityResult));
        }

        [Trait("Category", "Claims")]
        [Fact]
        public async Task Get_Current_User_From_Principal()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, _fakeIdentityUsers[0].Email)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            var result = await Controller.GetCurrentUser(principal);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }

        [Trait("Category", "Email")]
        [Fact]
        public async Task Get_Current_User_From_Email()
        {
            var result = await Controller.GetUserByEmail(_fakeIdentityUsers[0].Email);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }


        [Trait("Category", "Id")]
        [Fact]
        public async Task Get_Current_User_From_Id()
        {
            var result = await Controller.GetUserById(_fakeIdentityUsers[0].Id);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }


        [Trait("Category", "Email")]
        [Fact]
        public async Task Get_Current_User_From_NonExisting_Email()
        {
            var result = await Controller.GetUserByEmail("NonEsxiting@newmail.COm");

            Assert.Null(result);
        }

        [Trait("Category", "Identity Update")]
        [Fact]
        public async Task Update()
        {
            var identityUser = new IdentityUser()
            {
                Id = _fakeIdentityUsers[0].Id,
                UserName = _fakeIdentityUsers[0].UserName,
                PasswordHash = _fakeIdentityUsers[0].PasswordHash,
                Email = _fakeIdentityUsers[0].Email
            };

            identityUser.PhoneNumber = "nummer";
            var result = await Controller.Update(identityUser);

            Assert.True(result.Succeeded);
            Assert.Equal(_fakeIdentityUsers[0].PhoneNumber, identityUser.PhoneNumber);
        }

        [Trait("Category", "Identity Update")]
        [Fact]
        public void Update_Non_Existing_Guid()
        {
            var identityUser = new IdentityUser()
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "newmail@newmail.com"
            };

            Assert.ThrowsAsync<Exception>(() => Controller.Update(identityUser));
        }

        [Trait("Category", "Identity Update")]
        [Fact]
        public void Update_Email_Already_Exists()
        {
            var identityUser = new IdentityUser()
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "email2@gmail.com"
            };

            Assert.ThrowsAsync<Exception>(() => Controller.Update(identityUser));
        }

        [Trait("Category", "Identity Update")]
        [Fact]
        public void Delete_user()
        {
            var result = Controller.Delete(_fakeIdentityUsers[2]);

            Assert.True(result.Result.Succeeded);
        }


        internal void SeedData()
        {
            _fakeUser = new IdentityUser
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "email1@gmail.com"
            };
            _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "GenericUsername",
                Email = "email@gmail.com",
                UserName = "email@gmail.com"
            };
            var identityUser = new IdentityUser
            {
                Id = "!#!@#!@!#@#@2",
                PasswordHash = "GenericUsername",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser> {_fakeIdentityUser, identityUser, _fakeUser};
        }
    }
}
