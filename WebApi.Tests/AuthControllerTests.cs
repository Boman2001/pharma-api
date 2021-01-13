using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Xunit;
using WebApi.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Mappings;
using WebApi.Models.Authentication;
using WebApi.Tests.Mocks;

namespace WebApi.Tests
{
    public class AuthControllerTests
    {
        private IdentityUser _fakeIdentityUser;
        private List<IdentityUser> _fakeIdentityUsers;
        private List<UserInformation> _userInformations;
        private AuthController Controller { get; }

        public AuthControllerTests()
        {
            SeedData();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<SecurityDbContext>()
                .UseInMemoryDatabase(databaseName: "PharmaPartnersIdentityDb")
                .Options;


            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); 
            });
            var mapper = mockMapper.CreateMapper();

            var fakeSecurityDbContext = new Mock<SecurityDbContext>(options).Object;
            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            var fakeIdentityRepository = new Mock<IdentityRepository>(userManager, signInManager, config, fakeSecurityDbContext).Object;

           
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_userInformations);

            Controller = new AuthController(fakeIdentityRepository, fakeGenericRepo.Object, mapper);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Non_Valid_Email_Response()
        {
            var user = new LoginDto {Email = "email", Password = "password"};

            var result = (BadRequestObjectResult) await Controller.Login(user);
            var objectResult = (ObjectResult) result;
            var stringCast = objectResult.Value.ToString();

            Assert.Equal("{ message = Deze combinatie van e-mailadres en wachtwoord is incorrect. }", stringCast);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_No_Data_Response()
        {
            var user = new LoginDto{Password = "password"};

            var result = (BadRequestObjectResult) await Controller.Login(user);
            var objectResult = (ObjectResult) result;
            var stringCast = objectResult.Value.ToString();

            Assert.Equal("{ message = Deze combinatie van e-mailadres en wachtwoord is incorrect. }", stringCast);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Login_Valid_Response()
        {
            var user = new LoginDto {Email = "email@gmail.com", Password = "password"};
            var actionResult = await Controller.Login(user);
            var okObjectResult = (OkObjectResult) actionResult;

            Assert.Equal(200, okObjectResult.StatusCode);
        }

        internal void SeedData()
        {
            _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email@gmail.com",
                UserName = "email@gmail.com"
            };
            var extraIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser> {_fakeIdentityUser, extraIdentityUser};


            _userInformations = new List<UserInformation>();
            var userInformation = new UserInformation
            {
                Name = "tedst",
                Bsn = "tyest",
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