using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;
using Xunit;

namespace WebApi.Tests.ControllerTests
{
    public class EpisodeControllerTests
    {
        private List<Episode> _fakeEntities;
        private List<IdentityUser> _fakeIdentityUsers;
        private EpisodesController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }

        public EpisodeControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;

            IdentityRepositoryFake = new IdentityRepository(userManager, signInManager, config);
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeEntities);

            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new EpisodesController(fakeGenericRepo.Object, IdentityRepositoryFake);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_Episode_With_200_code()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var activities = (List<Episode>) objectResult.Value;

            Assert.Equal(_fakeEntities.Count, activities.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(activities[0], _fakeEntities[0]);
            Assert.IsType<Episode>(activities[0]);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_Episode_By_Id_Returns_Episode_With_200_codeAsync()
        {
            var result = await FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (Episode) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity, _fakeEntities[0]);
            Assert.IsType<Episode>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Episode_Posts_And_Returns_201_Code()
        {
            var entity = new Episode
            {
                Description = "NewDesc"
            };

            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Post(entity);
            var objActionResult = (CreatedAtActionResult) result.Result;
            var createdPatient = _fakeEntities[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeEntities.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(createdPatient.Description, entity.Description);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_Episode_To_Update_returns_200()
        {
            var entity = new Episode
            {
                Description = "UpdatedDesc"
            };
            var result = await FakeController.Put(_fakeEntities[0].Id, entity);

            var objectResult = (OkObjectResult) result;
            Assert.NotNull(_fakeEntities[0].UpdatedAt);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Id_To_Delete_Deletes_Episode()
        {
            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Delete(_fakeEntities[0].Id);
            var objContentResult = (NoContentResult) result;

            Assert.Equal(204, objContentResult.StatusCode);
            Assert.Equal(lengthBefore - 1, _fakeEntities.Count);
        }

        private void SeedData()
        {
            var activity = new Episode
            {
                Id = 1,
                Description = "description"
            };
            var activity02 = new Episode
            {
                Id = 2,
                Description = "description"
            };
            _fakeEntities = new List<Episode>
            {
                activity, activity02
            };

            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
        }
    }
}
