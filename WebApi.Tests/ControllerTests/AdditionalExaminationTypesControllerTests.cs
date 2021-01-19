using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Mappings;
using WebApi.Models.AdditionalExaminationTypes;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;
using Xunit;

namespace WebApi.Tests.ControllerTests
{
    public class AdditionalExaminationTypesControllerTests
    {
        private List<AdditionalExaminationType> _fakeEntities;
        private List<IdentityUser> _fakeIdentityUsers;
        private AdditionalExaminationTypesController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }

        public AdditionalExaminationTypesControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;

            IdentityRepositoryFake = new IdentityRepository(userManager, signInManager, config);
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeEntities);

            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new AdditionalExaminationTypesController(IdentityRepositoryFake, fakeGenericRepo.Object, mapper);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_AdditionalExaminationType_With_200_code()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var activities = (List<AdditionalExaminationTypeDto>) objectResult.Value;

            Assert.Equal(_fakeEntities.Count, activities.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(activities[0].Name, _fakeEntities[0].Name);
            Assert.IsType<AdditionalExaminationTypeDto>(activities[0]);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_AdditionalExaminationType_By_Id_Returns_AdditionExaminationResult_With_200_codeAsync()
        {
            var result = await FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (AdditionalExaminationTypeDto) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity.Name, _fakeEntities[0].Name);
            Assert.IsType<AdditionalExaminationTypeDto>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationType_Posts_And_Returns_201_Code()
        {
            var entity = new AdditionalExaminationTypeDto
            {
                Name = "Naamm",
                Unit = "Unitt"
            };

            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Post(entity);
            var objActionResult = (CreatedAtActionResult) result.Result;
            var createdPatient = _fakeEntities[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeEntities.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(createdPatient.Name, entity.Name);
            Assert.Equal(createdPatient.Unit, entity.Unit);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationType_To_Update_returns_200()
        {
            var entity = new AdditionalExaminationTypeDto
            {
                Name = "Naamm",
                Unit = "Unitt"
            };

            var result = await FakeController.Put(_fakeEntities[0].Id, entity);

            var objectResult = (OkObjectResult) result;
            Assert.NotNull(_fakeEntities[0].UpdatedAt);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Id_To_Delete_Deletes_AdditionalExaminationType()
        {
            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Delete(_fakeEntities[0].Id);
            var objContentResult = (NoContentResult) result;

            Assert.Equal(204, objContentResult.StatusCode);
            Assert.Equal(lengthBefore - 1, _fakeEntities.Count);
        }

        private void SeedData()
        {
            var entity = new AdditionalExaminationType
            {
                Id = 1,
                Name = "Naam",
                Unit = "Unit"
            };
            var aentity02 = new AdditionalExaminationType
            {
                Id = 2,
                Name = "Naam",
                Unit = "Unit"
            };
            _fakeEntities = new List<AdditionalExaminationType>
            {
                entity, aentity02
            };

            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
        }
    }
}
