using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.controllers;
using WebApi.Tests.Mocks;
using Xunit;

namespace WebApi.Tests
{
    public class PatientsControllerTests
    {
        private IdentityUser _fakeIdentityUser;

        private List<Patient> _fakeUsersPatient;

        private PatientsController FakeController { get; }

        public PatientsControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersPatient);
            FakeController = new PatientsController(fakeGenericRepo.Object, config);
        }


        [Trait("Category", "Patient")]
        [Fact]
        public void Get_All()
        {
            var objectResult = FakeController.Get();
            var ok = (OkObjectResult) objectResult.Result;
            var objectResultValue = (List<Patient>) ok.Value;

            Assert.Equal(_fakeUsersPatient.Count, objectResultValue.Count);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(objectResultValue[0], _fakeUsersPatient[0]);
            Assert.IsType<Patient>(objectResultValue[0]);
        }


        [Trait("Category", "Patient")]
        [Fact]
        public async Task Get_By_Id_Async()
        {
            var objectResult = await FakeController.Get(_fakeUsersPatient[0].Id);
            var ok = (OkObjectResult) objectResult.Result;
            var objectResultValue = (Patient) ok.Value;

            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(objectResultValue, _fakeUsersPatient[0]);
            Assert.IsType<Patient>(objectResultValue);
        }


        [Trait("Category", "Patient")]
        [Fact]
        public async Task Post_Patient()
        {
            var patient = new Patient
            {
                Name = "tedst",
                Bsn = "tyest",
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
            var lengthBefore = _fakeUsersPatient.Count;

            var objectResult = await FakeController.Post(patient);
            var created = (CreatedAtActionResult) objectResult.Result;

            Assert.Equal(lengthBefore + 1, _fakeUsersPatient.Count);
            Assert.Equal(201, created.StatusCode);
            Assert.Equal(patient, _fakeUsersPatient[lengthBefore]);
        }

        [Trait("Category", "Patient")]
        [Fact]
        public async Task Put_Patient()
        {
            var patient = new Patient
            {
                Name = "tedst",
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
            var objectResult = await FakeController.Put(_fakeUsersPatient.Count, patient);
            var ok = (OkObjectResult) objectResult;

            Assert.Equal(200, ok.StatusCode);
        }

        [Trait("Category", "Patient")]
        [Fact]
        public async Task Delete_Patient()
        {
            var objectResult = await FakeController.Delete(_fakeUsersPatient[0].Id);
            var ok = (NoContentResult) objectResult;

            Assert.Equal(204, ok.StatusCode);
            Assert.Empty(_fakeUsersPatient);
        }

        public void SeedData()
        {
            _fakeUsersPatient = new List<Patient>();
            var patient = new Patient
            {
                Name = "tedst",
                Bsn = "tyest",
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
                patient
            });
        }
    }
}
