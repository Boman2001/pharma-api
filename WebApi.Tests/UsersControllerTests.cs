using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Enums;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApi.Controllers;
using WebApi.Mappings;
using WebApi.Models.Users;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;
using Xunit;

namespace WebApi.Tests
{
    [Collection("UsersTest")]
    public class UsersControllerTests
    {
        private List<IdentityUser> _fakeIdentityUsers;
        private IdentityUser _fakeUser01;
        private IdentityUser _fakeUser00;
        private IdentityUser _fakeUser02;
        private List<UserInformation> _fakeUsersInformation;

        public UsersControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersInformation);
            MockUserExtension.ExtendMock(fakeGenericRepo, _fakeUsersInformation);

            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            var fakeIdentityRepository = new Mock<IdentityRepository>(userManager, signInManager, config).Object;

            FakeController = new UsersController(mapper, fakeGenericRepo.Object, fakeIdentityRepository, userManager);

            setUser(_fakeIdentityUsers[0]);
        }

        //Get Tests
        private UsersController FakeController { get; }

        [Trait("Category", "Get User Test")]
        [Fact]
        public void Get_All_Users()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var userDtos = (List<UserDto>) objectResult.Value;

            Assert.Equal(_fakeIdentityUsers.Count, userDtos.Count);
        }

        [Trait("Category", "Get User")]
        [Fact]
        public async Task Given_Correct_Id_Return_User()
        {
            var actionResult = await FakeController.Get(_fakeIdentityUsers[0].Id);
            ActionResult<UserInformation> result = actionResult.Result;
            var objectResult = (OkObjectResult) result.Result;
            var resultDto = (UserDto) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.IsType<UserDto>(resultDto);
        }

        [Trait("Category", "Get User")]
        [Fact]
        public async Task Given_Non_Existing_User_Return_Not_Found()
        {
            var result = await FakeController.Get("invalid");
            ActionResult<UserInformation> actionResult = result.Result;
            var statusCodeResult = (StatusCodeResult) actionResult.Result;

            Assert.Equal(404, statusCodeResult.StatusCode);
        }

        //Post Tests

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Correct_User_Adds_User_And_Returns_200()
        {
            var lengthBefore = _fakeUsersInformation.Count;
            var user = new NewUserDto
            {
                Email = "emailTwdo@gmail.com",
                PhoneNumber = "3145142",
                City = "City",
                Country = "Country",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                HouseNumber = "20",
                HouseNumberAddon = null,
                PostalCode = "Postalcode",
                Name = "TestDaddy",
                Password = "Password",
            };

            var result = await FakeController.Post(user);
            var objectResult = (OkObjectResult) result.Result;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.Equal(lengthBefore + 1, _fakeUsersInformation.Count);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Email_Already_In_Use_Gives_400()
        {
            var user = new NewUserDto
            {
                Email = _fakeUser01.Email, Password = _fakeUser01.PasswordHash
            };

            var result = await FakeController.Post(user);
            var objectResult = (BadRequestObjectResult) result.Result;

            Assert.Equal(400, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.Equal("E-mailadres is al in gebruik.", objectResult.Value.ToString());
        }

        //Put Tests

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_Correct_User_Updates_User()
        {
            var count = _fakeIdentityUsers.Count;
            var postUserDto = new NewUserDto
            {
                Email = "emailTwo@gmail.com", Password = "Password"
            };
            var putUserDto = new UpdateUserDto
            {
                Email = "emaiel@gmail.com", Password = "Password"
            };

            await FakeController.Post(postUserDto);
            await FakeController.Put(_fakeIdentityUsers[count - 1].Id, putUserDto);

            Assert.Equal(putUserDto.Email, _fakeIdentityUsers[count - 1].Email);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_Already_Existing_User_And_Email_Updates_Email()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "emaiel@gmail.com"
            };

            await FakeController.Put(_fakeIdentityUsers[0].Id, putUserDto);

            Assert.Equal(putUserDto.Email, _fakeIdentityUsers[0].Email);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_Non_Existing_Id_Returns_Not_Found()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "email2@gmail.com"
            };

            var result = (NotFoundResult) await FakeController.Put(5.ToString(), putUserDto);

            Assert.IsType<NotFoundResult>(result);
        }

        //Delete Tests

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Correct_User_Deletes_User_Returns_No_Content()
        {
            var result = (NoContentResult) await FakeController.Delete(_fakeIdentityUsers[1].Id);

            Assert.IsType<NoContentResult>(result);
        }

        private void setUser(IdentityUser identity)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, identity.Id),
                new Claim(ClaimTypes.Sid, identity.Id)
            }, "mock"));

            FakeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        private void SeedData()
        {
            _fakeUser00 = new IdentityUser
            {
                UserName = "genericUsername", PasswordHash = "genericUsername", Email = "genericUsername"
            };
            _fakeUser01 = new IdentityUser
            {
                PasswordHash = "genericUsername", Email = "email@gmail.com", UserName = "email@gmail.com"
            };
            _fakeUser02 = new IdentityUser
            {
                PasswordHash = "genericUsername", Email = "email2@gmail.com", UserName = "email2@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser>
            {
                _fakeUser01, _fakeUser02, _fakeUser00
            };
            _fakeUsersInformation = new List<UserInformation>();
            var userInformation00 = new UserInformation
            {
                Name = "name",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe",
                UserId = Guid.Parse(_fakeUser01.Id)
            };
            var userInformation01 = new UserInformation
            {
                UserId = Guid.Parse(_fakeUser00.Id)
            };
            var userInformation02 = new UserInformation
            {
                UserId = Guid.Parse(_fakeUser02.Id)
            };
            _fakeUsersInformation.AddRange(new List<UserInformation>
            {
                userInformation00, userInformation01, userInformation02
            });
        }
    }
}