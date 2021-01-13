using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Controllers;
using Xunit;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.Extensions.Configuration;
using WebApi.Mappings;
using WebApi.Models.Users;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;

namespace WebApi.Tests
{
    [Collection("UsersTest")]
    public class UsersControllerTests
    {
        private UserInformation _userInformation;
        private IdentityUser _fakeIdentityUser;
        private List<IdentityUser> _fakeIdentityUsers;
        private List<UserInformation> _fakeUsersInformation;
        private IdentityUser _fakeUser;

        private UsersController FakeController { get; }

        public UsersControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<SecurityDbContext>()
                .UseInMemoryDatabase(databaseName: "PharmaPartnersIdentityDb")
                .Options;

            var fakeSecurityDbContext = new Mock<SecurityDbContext>(options).Object;

            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersInformation);
            MockUserExtension.ExtendMock(fakeGenericRepo, _fakeUsersInformation);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); //your automapperprofile 
             });
            var mapper = mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            var fakeIdentityRepository = new Mock<IdentityRepository>(userManager, signInManager, config, fakeSecurityDbContext).Object;
            var fakeUserInformationRepository = new Mock<IRepository<UserInformation>>().Object;
            FakeController = new UsersController(mapper, fakeGenericRepo.Object, fakeIdentityRepository, userManager);
        }


        [Trait("Category", "User")]
        [Fact]
        public void Get_All_Users()
        {
            var objectResult = FakeController.Get();
            var ok = (OkObjectResult)objectResult.Result;
            var objectResultValue = (List<UserDto>)ok.Value;
            Assert.Equal(_fakeIdentityUsers.Count, objectResultValue.Count);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Get_By_Id_404()
        {
            var actionResult = await FakeController.Get("invalid");
            ActionResult<UserInformation> result = actionResult.Result;
            var statusCodeResult = (StatusCodeResult)result.Result;

            Assert.Equal(404, statusCodeResult.StatusCode);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Post_User()
        {
            var lengthBefore = _fakeUsersInformation.Count;
            var user = new NewUserDto
            {
                Email = "emailTwdo@gmail.com",
            };

            var post = await FakeController.Post(user);
            var okObjectResult = (OkObjectResult)post.Result;

            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(lengthBefore + 1, _fakeUsersInformation.Count);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Post_User_Empty()
        {
            var user = new NewUserDto();

            var post = await FakeController.Post(user);
            var badRequestObjectResult = (BadRequestObjectResult)post.Result;

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult.Value);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Post_Doctor_Existing()
        {
            var user = new NewUserDto
            {
                Email = _fakeIdentityUser.Email,
                Password = _fakeIdentityUser.PasswordHash,
            };

            var post = await FakeController.Post(user);
            var badRequestObjectResult = (BadRequestObjectResult)post.Result;

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult.Value);
            Assert.Equal("{ message = E-mailadres is al in gebruik. }", badRequestObjectResult.Value.ToString());
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Put_User()
        {
            var count = _fakeIdentityUsers.Count;
            var postUserDto = new NewUserDto
            {
                Email = "emailTwo@gmail.com",
                Password = "test"
            };
            var putUserDto = new UpdateUserDto
            {
                Email = "emaiel@gmail.com",
                Password = "test"
            };
           var a = await FakeController.Post(postUserDto);

            var doctor = await FakeController.Put(_fakeIdentityUsers[count].Id, putUserDto);

            Assert.Equal(count + 1, _fakeIdentityUsers.Count);
            Assert.Equal(putUserDto.Email, _fakeIdentityUsers[count].Email);
        }

        [Trait("Category", "User")]
        [Fact]
        public void Put_User_non_existent_id()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "emaiel@gmail.com"
            };

            Assert.ThrowsAsync<Exception>(() => FakeController.Put(112312312.ToString(), putUserDto));
        }


        [Trait("Category", "User")]
        [Fact]
        public async Task Put_Usere_Already_existing()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "emaiel@gmail.com"
            };

            var badRequestObjectResult = await FakeController.Put( _fakeIdentityUsers[0].Id, putUserDto);

            Assert.Equal(putUserDto.Email, _fakeIdentityUsers[0].Email);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Put_User_Email_Already_In_Usee()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "email2@gmail.com",
            };

            var badRequestObjectResult = (BadRequestObjectResult)await FakeController.Put(_fakeIdentityUsers[0].Id, putUserDto);

            Assert.IsType<NotFoundResult>(badRequestObjectResult);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task Put_doctor_not_existing()
        {
            var putUserDto = new UpdateUserDto
            {
                Email = "email2@gmail.com",
            };

            var badRequestObjectResult = (NotFoundResult)await FakeController.Put(5.ToString(), putUserDto);

            Assert.IsType<NotFoundResult>(badRequestObjectResult);
        }

        [Trait("Category", "User")]
        [Fact]
        public async Task zDelete_User()
        {
            var lengthBefore = _fakeUsersInformation.Count;
            var userLengthBefore = _fakeIdentityUsers.Count;


            var badRequestObjectResult = (NoContentResult)await FakeController.Delete(_fakeIdentityUsers[1].Id);

            Assert.IsType<NoContentResult>(badRequestObjectResult);
        }

        internal void SeedData()
        {
            _fakeUser = new IdentityUser
            {
                UserName = "genericUsername",
                PasswordHash = "genericUsername",
                Email = "genericUsername"
            };
            _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "genericUsername",
                Email = "email@gmail.com",
                UserName = "email@gmail.com"
            };
            var akeIdentityUser = new IdentityUser
            {
                PasswordHash = "genericUsername",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser> { _fakeIdentityUser, akeIdentityUser, _fakeUser };
            _fakeUsersInformation = new List<UserInformation>();
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
            var userInformationSecond = new UserInformation
            {
                UserId = Guid.Parse(_fakeUser.Id)
            };
            var userInformationThird = new UserInformation
            {
                UserId = Guid.Parse(akeIdentityUser.Id)
            };
            _fakeUsersInformation.AddRange(new List<UserInformation>
             {
                 userInformation,
                 userInformationSecond,
                 userInformationThird,
             });
        }
    }
}
