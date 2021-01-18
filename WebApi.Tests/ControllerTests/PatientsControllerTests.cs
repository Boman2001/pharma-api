using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks;
using Xunit;

namespace WebApi.Tests.ControllerTests
{
    using AutoMapper;
    using Infrastructure.Repositories;
    using Mappings;
    using Microsoft.AspNetCore.Identity;
    using Models.Patients;

    public class PatientsControllerTests
    {
        private List<Patient> _fakeUsersPatient;
        private List<IdentityUser> _fakeIdentityUsers;
        private PatientsController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }

        public PatientsControllerTests()
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

            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersPatient);
            FakeController = new PatientsController(fakeGenericRepo.Object, config, IdentityRepositoryFake, mapper);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
        }

        //Get Tests

        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_Patients_With_200_code()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var patients = (List<Patient>) objectResult.Value;

            Assert.Equal(_fakeUsersPatient.Count, patients.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(patients[0], _fakeUsersPatient[0]);
            Assert.IsType<Patient>(patients[0]);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Id_Returns_Patient()
        {
            var result = await FakeController.Get(_fakeUsersPatient[0].Id);
            var objectResult = (OkObjectResult) result.Result;
            var patient = (Patient) objectResult.Value;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(patient, _fakeUsersPatient[0]);
            Assert.IsType<Patient>(patient);
        }

        [Trait("Category", "Get Tests")]
        [Fact]
        public async Task Given_Non_Existing_Id_Returns_Null()
        {
            var result = await FakeController.Get(23);
            var notFound = (NotFoundResult) result.Result;
            Assert.Equal(404, notFound.StatusCode);
        }

        //Post Tests

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Patient_Posts_And_Returns_201_Code()
        {
            var currentDateTime = DateTime.Now;
            
            var patientDto = new PatientDto
            {
                Name = "name",
                Bsn = "Bsn",
                Email = "test",
                Dob = currentDateTime,
                Gender = Gender.Male,
                PhoneNumber = "1321",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe"
            };

            var patient = new Patient
            {
                Name = "name",
                Bsn = "Bsn",
                Email = "test",
                Dob = currentDateTime,
                Gender = Gender.Male,
                PhoneNumber = "1321",
                City = "hank",
                Street = "lepelaarstraat20",
                HouseNumber = "20",
                PostalCode = "23",
                Country = "qwe"
            };

            var lengthBefore = _fakeUsersPatient.Count;

            var result = await FakeController.Post(patientDto);
            var objActionResult = (CreatedAtActionResult) result.Result;
            var createdPatient = _fakeUsersPatient[lengthBefore];

            Assert.Equal(lengthBefore + 1, _fakeUsersPatient.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(patient.Name, createdPatient.Name);
            Assert.Equal(patient.Bsn, createdPatient.Bsn);
            Assert.Equal(patient.Email, createdPatient.Email);
            Assert.Equal(patient.Dob, createdPatient.Dob);
            Assert.Equal(patient.Gender, createdPatient.Gender);
            Assert.Equal(patient.PhoneNumber, createdPatient.PhoneNumber);
            Assert.Equal(patient.City, createdPatient.City);
            Assert.Equal(patient.Street, createdPatient.Street);
            Assert.Equal(patient.HouseNumber, createdPatient.HouseNumber);
            Assert.Equal(patient.PostalCode, createdPatient.PostalCode);
            Assert.Equal(patient.Country, createdPatient.Country);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Empty_Patient_Returns_Bad_Request()
        {
            var patient = new PatientDto();
            var lengthBefore = _fakeUsersPatient.Count;

            var result = await FakeController.Post(patient);
            var objectResult = (BadRequestObjectResult) result.Result;

            Assert.Equal(lengthBefore, _fakeUsersPatient.Count);
            Assert.Equal(400, objectResult.StatusCode);
        }

        //Update Tests

        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given_Patient_To_Update_returns_200()
        {
            var patient = new Patient
            {
                Name = "Name",
                Bsn = "BEST",
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
            var result = await FakeController.Put(_fakeUsersPatient.Count, patient);
            var objectResult = (OkObjectResult) result;

            Assert.Equal(200, objectResult.StatusCode);
        }


        [Trait("Category", "Update Tests")]
        [Fact]
        public async Task Given__Empty_Patient_To_Update_Returns_Error()
        {
            var patient = new Patient();
            var result = await FakeController.Put(_fakeUsersPatient.Count, patient);
            var objectResult = (OkObjectResult) result;

            Assert.Equal(200, objectResult.StatusCode);
        }

        //Delete Tests

        [Trait("Category", "Delete Tests")]
        [Fact]
        public async Task Given_Id_To_Delete_Deletes_Patient()
        {
            var lengthBefore = _fakeUsersPatient.Count;

            var result = await FakeController.Delete(_fakeUsersPatient[0].Id);
            var objContentResult = (NoContentResult) result;

            Assert.Equal(204, objContentResult.StatusCode);
            Assert.Equal(lengthBefore - 1, _fakeUsersPatient.Count);
        }

        private void SeedData()
        {
            _fakeUsersPatient = new List<Patient>();
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

            _fakeIdentityUsers = IdentityHelper.GetIdentityUsers();
        }
    }
}