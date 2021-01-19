using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            Controller = new IdentityRepository(userManager, signInManager, config);
        }

        //Login Tests

        [Trait("Category", "Login Tests")]
        [Fact]
        public async Task Given_Correct_Login_Details_Returns_Jwt()
        {
            var result = await Controller.Login(_fakeIdentityUsers[0], _fakeIdentityUsers[0].PasswordHash);

            Assert.IsType<JwtSecurityToken>(result);
        }

        [Trait("Category", "Login Tests")]
        [Fact]
        public async Task Given_In_Correct_Email_Returns_Error()
        {
           var user = new IdentityUser
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "wrong@gmail.com"
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await Controller.Login(user, user.PasswordHash));
        }

        //Update Tests

        [Trait("Category", "Update Tests")]
        [Fact]
        public void Given_Non_Existing_User_Throws_Exception()
        {
            var identityUser = new IdentityUser
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "newmail@newmail.com"
            };

            Assert.ThrowsAsync<Exception>(() => Controller.Update(identityUser));
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public void Given_Non_Existing_User_To_Update_Returns_Exception()
        {
            var identityUser = new IdentityUser
            {
                UserName = "GenericUsername",
                PasswordHash = "GenericUsername",
                Email = "email2@gmail.com"
            };

            Assert.ThrowsAsync<Exception>(() => Controller.Update(identityUser));
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_User_And_New_Details_Update_Returns_Succeeded_And_Updates()
        {
            var identityUser = new IdentityUser
            {
                Id = _fakeIdentityUsers[0].Id,
                UserName = _fakeIdentityUsers[0].UserName,
                PasswordHash = _fakeIdentityUsers[0].PasswordHash,
                Email = _fakeIdentityUsers[0].Email,
                PhoneNumber = "phoneNumber"
            };

            var result = await Controller.Update(identityUser);

            Assert.True(result.Succeeded);
            Assert.Equal(_fakeIdentityUsers[0].PhoneNumber, identityUser.PhoneNumber);
        }

        //Delete Tests

        [Trait("Category", "Delete Tests")]
        [Fact]
        public void Given_User_To_Delete_Returns_Succeeded()
        {
            var count = _fakeIdentityUsers.Count;

            var result = Controller.Delete(_fakeIdentityUsers[2]);

            Assert.True(result.Result.Succeeded);
            Assert.Equal(count - 1, _fakeIdentityUsers.Count);
        }

        [Trait("Category", "Delete Tests")]
        [Fact]
        public void Given_Non_Existing_User_To_Delete_Dont_Delete_Anything()
        {
            var identityUser = new IdentityUser
            {
                UserName = "Username",
                PasswordHash = "Password",
                Email = "Maarten@gmail.com"
            };
            var count = _fakeIdentityUsers.Count;

            var result = Controller.Delete(identityUser);

            Assert.True(result.Result.Succeeded);
            Assert.Equal(count, _fakeIdentityUsers.Count);
        }

        //GetUser Tests

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Correct_Claims_Principle_Returns_User()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, _fakeIdentityUsers[0].Email)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            var result = await Controller.GetCurrentUser(principal);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Incorrect_Claims_Principle_Returns_Null()
        {
            var claims = Array.Empty<Claim>();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            var result = await Controller.GetCurrentUser(principal);

            Assert.Null(result);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Id_Returns_Correct_User()
        {
            var result = await Controller.GetUserById(_fakeIdentityUsers[0].Id);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Incorrect_Id_Returns_Null()
        {
            var result = await Controller.GetUserById("Id");

            Assert.Null(result);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Correct_Email_Returns_User()
        {
            var result = await Controller.GetUserByEmail(_fakeIdentityUsers[0].Email);

            Assert.Equal(_fakeIdentityUsers[0], result);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Non_Existing_Email_Details_Returns_Null()
        {
            var result = await Controller.GetUserByEmail("NonEsxiting@newmail.COm");

            Assert.Null(result);
        }

        private void SeedData()
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