using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApi.Controllers;
using WebApi.Models.Authentication;
using WebApi.Tests.Mocks;
using Xunit;

namespace WebApi.Tests
{
    public class AuthControllerTests
    {
        private IdentityUser _fakeIdentityUser;
        private List<IdentityUser> _fakeIdentityUsers;
        private List<UserInformation> _userInformations;

        public AuthControllerTests()
        {
            SeedData();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            var fakeIdentityRepository = new Mock<IdentityRepository>(userManager, signInManager, config).Object;
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_userInformations);

            Controller = new AuthController(fakeIdentityRepository, fakeGenericRepo.Object);
        }

        private AuthController Controller { get; }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Given_Non_Acceptable_Login_Details_Returns_Error_Message()
        {
            var user = new LoginDto
            {
                Email = "email", Password = "password"
            };

            var result = (BadRequestObjectResult) await Controller.Login(user);
            var objectResult = (ObjectResult) result;
            var stringCast = objectResult.Value.ToString();

            Assert.Equal("{ message = Deze combinatie van e-mailadres en wachtwoord is incorrect. }", stringCast);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Given_No_Email_Returns_Error_Message()
        {
            var user = new LoginDto
            {
                Password = "password"
            };

            var result = (BadRequestObjectResult) await Controller.Login(user);
            var objectResult = (ObjectResult) result;
            var stringCast = objectResult.Value.ToString();

            Assert.Equal("{ message = Deze combinatie van e-mailadres en wachtwoord is incorrect. }", stringCast);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Given_No_Password_Returns_Error_Message()
        {
            var user = new LoginDto
            {
                Email = "Email@gmail.com"
            };

            var result = (BadRequestObjectResult) await Controller.Login(user);
            var objectResult = (ObjectResult) result;
            var stringCast = objectResult.Value.ToString();

            Assert.Equal("{ message = Deze combinatie van e-mailadres en wachtwoord is incorrect. }", stringCast);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Given_Correct_Login_Details_Returns_200_Code()
        {
            var user = new LoginDto
            {
                Email = "email@gmail.com", Password = "password"
            };
            var result = await Controller.Login(user);
            var okObjectResult = (OkObjectResult)result;

            Assert.Equal(200, okObjectResult.StatusCode);
        }

        private void SeedData()
        {
            _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "password", Email = "email@gmail.com", UserName = "email@gmail.com"
            };
            var extraIdentityUser = new IdentityUser
            {
                PasswordHash = "password", Email = "email2@gmail.com", UserName = "email@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser>
            {
                _fakeIdentityUser, extraIdentityUser
            };

            _userInformations = new List<UserInformation>();
            var userInformation = new UserInformation
            {
                Name = "name",
                Bsn = "bsn",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe",
                UserId = Guid.Parse(_fakeIdentityUser.Id)
            };
            _userInformations.AddRange(new List<UserInformation>
            {
                userInformation
            });
        }
    }
}