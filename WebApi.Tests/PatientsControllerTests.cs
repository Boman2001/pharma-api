using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApi.controllers;
using WebApi.Tests.Mocks;
using Xunit;

namespace WebApi.Tests
{
    using Infrastructure.Repositories;
    using Microsoft.AspNetCore.Identity;

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
            
            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            IdentityRepositoryFake = new IdentityRepository(userManager, signInManager, config);

            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersPatient);
            FakeController = new PatientsController(fakeGenericRepo.Object, config, IdentityRepositoryFake);

            setUser(_fakeIdentityUsers[0]);
           
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
            var patient = new Patient
            {
                Name = "name",
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
            var lengthBefore = _fakeUsersPatient.Count;

        
                    var result = await FakeController.Post(patient);
            var objActionResult = (CreatedAtActionResult) result.Result;

            Assert.Equal(lengthBefore + 1, _fakeUsersPatient.Count);
            Assert.Equal(201, objActionResult.StatusCode);
            Assert.Equal(patient, _fakeUsersPatient[lengthBefore]);
        }

        [Trait("Category", "Post Tests")]
        [Fact]
        public async Task Given_Empty_Patient_Returns_Bad_Request()
        {
            var patient = new Patient();
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

        private void setUser (IdentityUser identity)
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
                patient,
                patient02
            });

            var _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email@gmail.com",
                UserName = "email@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };
            var extraIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };

            _fakeIdentityUsers = new List<IdentityUser>
            {
                _fakeIdentityUser, extraIdentityUser
            };
        }
    }
}