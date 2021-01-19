using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using WebApi.Models.Prescriptions;
using WebApi.Tests.Helpers;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;
using Xunit;

namespace WebApi.Tests.ControllerTests
{
    public class ConstulationsControllerTests
    {
        private List<Prescription> _fakeEntities;
        private List<Consultation> _constulatations;
        private List<Patient> _patients;
        private List<IdentityUser> _fakeIdentityUsers;
        private PrescriptionsController FakeController { get; }
        private IdentityRepository IdentityRepositoryFake { get; }
        private List<UserInformation> _fakeUsersInformation;

        public ConstulationsControllerTests()
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

            var userInformationMock = MockGenericRepository.GetUserInformationMock(_patients);
            var constulatationsMock = MockGenericRepository.GetUserInformationMock(_constulatations);
            MockGenericExtension.ExtendMock(fakeGenericRepo, _fakeEntities);
            FakeController = new PrescriptionsController(IdentityRepositoryFake, fakeGenericRepo.Object, fakeGenericRepoUserInformationMock.Object, userInformationMock.Object,
                constulatationsMock.Object,
                mapper);

            IdentityHelper.SetUser(_fakeIdentityUsers[0], FakeController);
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

            var p = new Patient
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
            var ep = new Episode
            {
                Description = "Description",
                Priority = 10,
                Patient = p,
                IcpcCode = ipCode
            };
            var intolerances = new Intolerance
            {
                Description = "descrption",
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Patient = p
            };
            var physical = new PhysicalExamination()
            {
                Value = "physical",
                Date = DateTime.Now,
                Patient = p
            };
            var c = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(_fakeIdentityUsers[0].Id),
                Doctor = _fakeIdentityUsers[0],
                Patient = p,
                AdditionalExaminationResults = new List<AdditionalExaminationResult>
                {
                    additional
                },
                Episodes = new List<Episode>
                {
                    ep
                },
                Intolerances = new List<Intolerance>
                {
                    intolerances
                },
                PhysicalExaminations = new List<PhysicalExamination>
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
