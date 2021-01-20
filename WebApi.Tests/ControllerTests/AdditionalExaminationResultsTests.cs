using System;
using System.Collections.Generic;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Controllers;
using WebApi.Tests.Mocks;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Mappings;
using WebApi.Models.AdditionalExaminationResults;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks.Extends;
using Xunit;


namespace WebApi.Tests.ControllerTests
{
    public class AdditionalExaminationResultsTests
    {
        private List<AdditionalExaminationResult> _fakeEntities;
        private List<IdentityUser> _fakeIdentityUsers;
        private List<Patient> _fakeUsersPatient;
        private List<Consultation> _consultations;
        private List<AdditionalExaminationType> _types;
        private AdditionalExaminationResultsController FakeController { get; }
        
        private IdentityRepository IdentityRepositoryFake { get; }

        public AdditionalExaminationResultsTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mock = mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;

            IdentityRepositoryFake = new IdentityRepository(userManager, signInManager, config);
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeEntities);
            var constulationRepo = MockGenericRepository.GetUserInformationMock(_consultations);
            var patientRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersPatient);
            var typeRepo = MockGenericRepository.GetUserInformationMock(_types);
            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);

            FakeController = new AdditionalExaminationResultsController(IdentityRepositoryFake, fakeGenericRepo.Object, 
                constulationRepo.Object,
                patientRepo.Object,
                typeRepo.Object,
                mock);
            
            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_AdditionExaminationResults_With_200_code()
        {
            var result = FakeController.Get(null);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (List<AdditionalExaminationResultDto>) objectResult.Value;

            Assert.Equal(_fakeEntities.Count, entity.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity[0].Value, _fakeEntities[0].Value);
            Assert.IsType<AdditionalExaminationResultDto>(entity[0]);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_AdditionExaminationResults_By_Id_Returns_AdditionExaminationResult_With_200_codeAsync()
        {
            var result = FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (AdditionalExaminationResultDto) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(entity.Value, _fakeEntities[0].Value);
            Assert.IsType<AdditionalExaminationResultDto>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_Posts_And_Returns_201_Code()
        {
            var entity = new AdditionalExaminationResultDto
            {
                Value = "value",
                Date = DateTime.Now,
                ConsultationId = _consultations[0].Id
            };

            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Post(entity);
            var objActionResult = (CreatedAtActionResult) result.Result;
            var createdEntity = _fakeEntities[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeEntities.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(createdEntity.Date, entity.Date);
            Assert.Equal(createdEntity.Value, entity.Value);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_To_Update_returns_200()
        {
            var entity = new AdditionalExaminationResultDto
            {
                Value = "valueupdated",
                Date = DateTime.Now,
                ConsultationId = _consultations[0].Id
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
            _fakeUsersPatient = new List<Patient>();

            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
            var patient = new Patient
            {
                Name = "Name",
                Bsn = "Bsn",
                Email = "test",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "1321",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe"
            };
            var patient02 = new Patient
            {
                Name = "Name",
                Bsn = "Bsn",
                Email = "test",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "1321",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe"
            };
            _fakeUsersPatient.AddRange(new List<Patient>
            {
                patient, patient02
            });

            var activity = new AdditionalExaminationResult
            {
                Id = 1,
                Value = "value",
                Date = DateTime.Now
            };


            var type = new AdditionalExaminationType
            {
                Name = "typename",
                Unit = "GPS"
            };
            var additional = new AdditionalExaminationResult
            {
                Value = "value",
                Date = DateTime.Now,
                AdditionalExaminationType = type
            };
            var ipCode = new IcpcCode
            {
                Name = "Name",
                Code = "code"
            };
            var episode = new Episode
            {
                Description = "Description",
                Priority = 10,
                Patient = patient02,
                IcpcCode = ipCode
            };
            var intolerance = new Intolerance
            {
                Description = "descrption",
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Patient = patient02
            };
            var physical = new PhysicalExamination()
            {
                Value = "physical",
                Date = DateTime.Now,
                Patient = patient02
            };
            var consultation = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(_fakeIdentityUsers[0].Id),
                Doctor = _fakeIdentityUsers[0],
                Patient = patient02,
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


            var activity02 = new AdditionalExaminationResult
            {
                Id = 2,
                Value = "value",
                Date = DateTime.Now,
                Patient = patient02,
                PatientId = patient02.Id,
                Consultation = consultation,
                ConsultationId = consultation.Id,
                AdditionalExaminationType = type,
                AdditionalExaminationTypeId = type.Id
            };
            _fakeEntities = new List<AdditionalExaminationResult>
            {
                activity, activity02
            };
            _consultations = new List<Consultation>();
            _consultations.Add(consultation);
            _types = new List<AdditionalExaminationType>();
            _types.Add(type);
        }
    }
}
