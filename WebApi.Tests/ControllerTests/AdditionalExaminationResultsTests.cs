using System;
using System.Collections.Generic;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Controllers;
using WebApi.Tests.Mocks;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Mappings;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks.Extends;
using Xunit;


namespace WebApi.Tests.ControllerTests
{
    public class AdditionalExaminationResultsTests
    {
        private List<AdditionalExaminationResult> _fakeEntities;
        private List<IdentityUser> _fakeIdentityUsers;
        private AdditionalExaminationResultsController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }

        public AdditionalExaminationResultsTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;

            IdentityRepositoryFake = new IdentityRepository(userManager, signInManager, config);
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeEntities);

            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new AdditionalExaminationResultsController(fakeGenericRepo.Object, IdentityRepositoryFake);
            
            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_AdditionExaminationResults_With_200_code()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var activities = (List<AdditionalExaminationResult>) objectResult.Value;

            Assert.Equal(_fakeEntities.Count, activities.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(activities[0], _fakeEntities[0]);
            Assert.IsType<AdditionalExaminationResult>(activities[0]);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_AdditionExaminationResults_By_Id_Returns_AdditionExaminationResult_With_200_codeAsync()
        {
            var result = await FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (AdditionalExaminationResult) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity, _fakeEntities[0]);
            Assert.IsType<AdditionalExaminationResult>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_Posts_And_Returns_201_Code()
        {
            var entity = new AdditionalExaminationResult
            {
                Value = "value",
                Date = DateTime.Now
            };

            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Post(entity);
            var objActionResult = (CreatedAtActionResult) result.Result;
            var createdPatient = _fakeEntities[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeEntities.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(createdPatient.Date, entity.Date);
            Assert.Equal(createdPatient.Value, entity.Value);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_To_Update_returns_200()
        {
            var entity = new AdditionalExaminationResult
            {
                Value = "valueupdated",
                Date = DateTime.Now
            };
            var result = await FakeController.Put(_fakeEntities[0].Id, entity);

            var objectResult = (OkObjectResult) result;
            Assert.NotNull(_fakeEntities[0].UpdatedAt);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Id_To_Delete_Deletes_AdditionalExaminationResult()
        {
            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Delete(_fakeEntities[0].Id);
            var objContentResult = (NoContentResult) result;

            Assert.Equal(204, objContentResult.StatusCode);
            Assert.Equal(lengthBefore - 1, _fakeEntities.Count);
        }

        private void SeedData()
        {
            var activity = new AdditionalExaminationResult
            {
                Id = 1,
                Value = "value",
                Date = DateTime.Now
            };
            var activity02 = new AdditionalExaminationResult
            {
                Id = 2,
                Value = "value",
                Date = DateTime.Now
            };
            _fakeEntities = new List<AdditionalExaminationResult>
            {
                activity, activity02
            };

            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
        }
    }
}
