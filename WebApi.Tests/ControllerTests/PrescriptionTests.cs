using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Enums;
using Core.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Controllers;
using WebApi.Mappings;
using WebApi.Models.Prescriptions;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;
using Xunit;

namespace WebApi.Tests.ControllerTests
{
    public class PrescriptionTests
    {
        private List<Prescription> _fakeEntities;
        private List<Consultation> _constulatations;
        private List<Patient> _patients;
        private List<IdentityUser> _fakeIdentityUsers;
        private PrescriptionsController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }
        private List<UserInformation> _fakeUsersInformation;

        public PrescriptionTests()
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

            var fakeGenericRepoUserInformationMock = MockGenericRepository.GetUserInformationMock(_fakeUsersInformation);
            MockUserExtension.ExtendMock(fakeGenericRepoUserInformationMock, _fakeUsersInformation);

            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new PrescriptionsController(fakeGenericRepo.Object, IdentityRepositoryFake, fakeGenericRepoUserInformationMock.Object, mapper);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_All_AdditionExaminationResults_With_200_codeAsync()
        {
            var result = await FakeController.Get();
            var objectResult = (OkObjectResult)result.Result;

            Assert.Equal(200, objectResult.StatusCode);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Get_AdditionExaminationResults_By_Id_Returns_AdditionExaminationResult_With_200_codeAsync()
        {
            var result = await FakeController.Get(_fakeEntities[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var entity = (PrescriptionDto) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.IsType<PrescriptionDto>(entity);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_Posts_And_Returns_201_Code()
        {
            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Post(_fakeEntities[0]);
            var objActionResult = (CreatedAtActionResult)result.Result;
            var createdPatient = _fakeEntities[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeEntities.Count);
            Assert.Equal(201, objActionResult.StatusCode);
        }

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_AdditionalExaminationResult_To_Update_returns_200()
        {
            var entity = _fakeEntities[0];
            entity.Description = " dedede";
            var result = await FakeController.Put(_fakeEntities[0].Id, entity);

            var objectResult = (OkObjectResult)result;
            Assert.NotNull(_fakeEntities[0].UpdatedAt);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Id_To_Delete_Deletes_AdditionalExaminationResult()
        {
            var lengthBefore = _fakeEntities.Count;

            var result = await FakeController.Delete(_fakeEntities[0].Id);
            var objContentResult = (NoContentResult)result;

            Assert.Equal(204, objContentResult.StatusCode);
            Assert.Equal(lengthBefore - 1, _fakeEntities.Count);
        }


        private void SeedData()
        {
            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
            var userInformation = new UserInformation
            {
                Name = "name",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe",
                UserId = Guid.Parse(_fakeIdentityUsers[0].Id)
            };
            _fakeUsersInformation = new List<UserInformation>();
            _fakeUsersInformation.AddRange(new List<UserInformation>
            {
                userInformation
            });

            Patient p = new Patient
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

            AdditionalExaminationType type = new AdditionalExaminationType
            {
                Name = "typename",
                Unit = "GPS"
            };
            AdditionalExaminationResult additional = new AdditionalExaminationResult
            {
                Value = "value",
                Date = DateTime.Now,
                AdditionalExaminationType = type
            };
            IcpcCode ipCode = new IcpcCode
            {
                Name = "Name",
                Code = "code"
            };
            Episode ep = new Episode
            {
                Description = "Description",
                Priority = 10,
                Patient = p,
                IcpcCode = ipCode
            };
            Intolerance intolerances = new Intolerance
            {
                Description = "descrption",
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Patient = p
            };
            PhysicalExamination physical = new PhysicalExamination()
            {
                Value = "physical",
                Date = DateTime.Now,
                Patient = p,
            };
            Consultation c = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(_fakeIdentityUsers[0].Id),
                Doctor = _fakeIdentityUsers[0],
                Patient = p,
                AdditionalExaminationResults = new List<AdditionalExaminationResult>()
                {
                    additional
                },
                Episodes = new List<Episode>()
                {
                    ep
                },
                Intolerances = new List<Intolerance>()
                {
                    intolerances
                },
                PhysicalExaminations = new List<PhysicalExamination>()
                {
                    physical
                }
            };

            var activity = new Prescription
            {
                Id = 1,
                Description = "description",
                StartDate = DateTime.Now,
                EndDate = DateTime.MaxValue,
                Patient = p,
                Consultation = c
            };
            var activity02 = new Prescription
            {
                Id = 2,
                Description = "description",
                StartDate = DateTime.Now,
                EndDate = DateTime.MaxValue,
                Patient = p,
                Consultation = c
            };
            _fakeEntities = new List<Prescription>
            {
                activity, activity02
            };

            _constulatations = new List<Consultation>
            {
                c
            };
            _patients = new List<Patient>
            {
                p
            };
        }
    }
}
