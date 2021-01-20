using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Enums;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Mappings;
using WebApi.Models.Episodes;
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
        private List<Consultation> _constulatations;
        private List<IcpcCode> _icpcCodes;
        private EpisodesController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }
        private List<Patient> _patients;

        public EpisodeControllerTests()
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

            var constulatationsMock = MockGenericRepository.GetUserInformationMock(_constulatations);

            var userInformationMock = MockGenericRepository.GetUserInformationMock(_patients);
            var icpcCodesMock = MockGenericRepository.GetUserInformationMock(_icpcCodes);

            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new EpisodesController(
                IdentityRepositoryFake,
                fakeGenericRepo.Object,
                constulatationsMock.Object,
                userInformationMock.Object,
                icpcCodesMock.Object,
                mapper);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

//<<<<<<< feature/examination-types-controller
//        [Trait("Category", "Get Tests")]
//        [Fact]
//        public void Get_All_Episode_With_200_code()
//        {
//            var result = FakeController.Get(null, null, false);
//            var objectResult = (OkObjectResult) result.Result;
//           var activities = (List<EpisodeDto>) objectResult.Value;
//=======
        //[Trait("Category", "Get Tests")]
        //[Fact]
        //public void Get_All_Episode_With_200_code()
        //{
        //    var result = FakeController.Get(null, null);
        //    var objectResult = (OkObjectResult) result.Result;
        //    var activities = (List<EpisodeDto>) objectResult.Value;
//>>>>>>> development

        //    Assert.Equal(_fakeEntities.Count, activities.Count);
        //    Assert.Equal(200, objectResult.StatusCode);
        //    Assert.Equal(activities[0].Description, _fakeEntities[0].Description);
        //    Assert.IsType<EpisodeDto>(activities[0]);
        //}

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_Episode_By_Id_Returns_Episode_With_200_codeAsync()
        {
            var result = await FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (EpisodeDto) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity.Description, _fakeEntities[0].Description);
            Assert.IsType<EpisodeDto>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Episode_Posts_And_Returns_201_Code()
        {
            var entity = new EpisodeDto
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
            var entity = new EpisodeDto
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
            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
            var activity = new Episode
            {
                Id = 1, Description = "description"
            };
            var activity02 = new Episode
            {
                Id = 2, Description = "description"
            };
            _fakeEntities = new List<Episode>
            {
                activity, activity02
            };

            var patient = new Patient
            {
                Name = "jim",
                Bsn = "bsn",
                Email = "jim@jim.com",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "124124",
                City = "hank",
                Street = "lepelaarstraat",
                HouseNumber = "20",
                HouseNumberAddon = "",
                PostalCode = "4273cv",
                Country = "Netherlands"
            };

            var type = new AdditionalExaminationType
            {
                Name = "typename", Unit = "GPS"
            };
            var additional = new AdditionalExaminationResult
            {
                Value = "value", Date = DateTime.Now, AdditionalExaminationType = type
            };
            var ipCode = new IcpcCode
            {
                Name = "Name", Code = "code"
            };
            var episode = new Episode
            {
                Description = "Description", Priority = 10, Patient = patient, IcpcCode = ipCode
            };
            var intolerance = new Intolerance
            {
                Description = "descrption", EndDate = DateTime.Now, StartDate = DateTime.Now, Patient = patient
            };
            var physical = new PhysicalExamination()
            {
                Value = "physical", Date = DateTime.Now, Patient = patient
            };
            var consultation = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(_fakeIdentityUsers[0].Id),
                Doctor = _fakeIdentityUsers[0],
                Patient = patient,
                AdditionalExaminationResults = new List<AdditionalExaminationResult>
                {
                    additional
                },
                Episodes = new List<Episode>
                {
                    episode
                },
                Intolerances = new List<Intolerance>
                {
                    intolerance
                },
                PhysicalExaminations = new List<PhysicalExamination>
                {
                    physical
                }
            };


            _constulatations = new List<Consultation>
            {
                consultation
            };
            _patients = new List<Patient>
            {
                patient
            };
            var code = new IcpcCode()
            {
                Code = "code", Name = "name"
            };

            _icpcCodes = new List<IcpcCode>
            {
                code
            };
        }
    }
}