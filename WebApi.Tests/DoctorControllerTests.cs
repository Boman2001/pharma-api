﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Controllers;
using Xunit;
using Core.Domain.DataTransferObject;
using Core.Domain.Models;
using Microsoft.Extensions.Configuration;
using WebApi.Mappings;
using WebApi.Tests.Mocks;
using WebApi.Tests.Mocks.Extends;

namespace WebApi.Tests
{
    [Collection("DoctorTest")]
    public class DoctorControllerTests
    {
        private UserInformation _userInformation;
        private IdentityUser _fakeIdentityUser;
        private List<IdentityUser> _fakeIdentityUsers;
        private List<UserInformation> _fakeUsersInformation;
        private IdentityUser _fakeUser;

        private UserinformationController FakeController { get; }

        public DoctorControllerTests()
        {
            SeedData();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
             
            var options = new DbContextOptionsBuilder<SecurityDbContext>()
                .UseInMemoryDatabase(databaseName: "PharmaPartnersIdentityDb")
                .Options;

            var fakeSecurityDbContext = new Mock<SecurityDbContext>(options).Object;
           
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeUsersInformation);
            MockDoctorExtension.ExtendMock(fakeGenericRepo, _fakeUsersInformation);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); //your automapperprofile 
            });
            var mapper = mockMapper.CreateMapper();

            var userManager = MockUserManager.GetMockUserManager(_fakeIdentityUsers).Object;
            var signInManager = MockSigninManager.GetSignInManager<IdentityUser>(userManager).Object;
            var fakeIdentityRepository = new Mock<IdentityRepository>(userManager, signInManager, config, fakeSecurityDbContext).Object;
            FakeController = new UserinformationController(mapper, fakeGenericRepo.Object, fakeIdentityRepository);
        }


        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Get_allAsync()
        {
            var objectResult = await FakeController.GetDoctorsAsync();
            var ok = (OkObjectResult) objectResult.Result;
            var objectResultValue = (List<UserInformationDto>) ok.Value;
            Assert.Equal(3, objectResultValue.Count);
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Get_By_Id_204()
        {
            var actionResult = await FakeController.GetDoctor(123);
            ActionResult<UserInformation> result = actionResult.Result;
            var statusCodeResult = (StatusCodeResult) result.Result;

            Assert.Equal(204, statusCodeResult.StatusCode);
        }


        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Get_By_Id_MaxValue()
        {
            var doctor = await FakeController.GetDoctor(int.MaxValue);
            ActionResult<UserInformation> result = doctor.Result;
            var statusCodeResult = (StatusCodeResult) result.Result;

            Assert.Equal(204, statusCodeResult.StatusCode);
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Get_By_Id_MinValue()
        {
            var doctor = await FakeController.GetDoctor(int.MinValue);
            ActionResult<UserInformation> result = doctor.Result;
            var statusCodeResult = (StatusCodeResult) result.Result;

            Assert.Equal(204, statusCodeResult.StatusCode);
        }

        //[Trait("Category", "Doctor")]
        //[Fact]
        //public async Task Post_Doctor()
        //{
        //    var lengthBefore = _fakeUsersInformation.Count;
        //    var user = new UserDto
        //    {
        //        Email = "emailTwdo@gmail.com",
        //        Password = "test"
        //    };

        //    var post = await FakeController.Post(user);
        //    var okObjectResult = (OkObjectResult) post.Result;

        //    Assert.Equal(200, okObjectResult.StatusCode);
        //    Assert.NotNull(okObjectResult.Value);
        //    Assert.Equal(lengthBefore + 1, _fakeUsersInformation.Count);
        //}

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Post_Doctor_Empty()
        {
            var user = new UserDto();

            var post = await FakeController.Post(user);
            var badRequestObjectResult = (BadRequestObjectResult) post.Result;

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult.Value);
            Assert.Equal("{ message = Incorrect password }", badRequestObjectResult.Value.ToString());
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Post_doctor_existing()
        {
            var user = new UserDto
            {
                Id = 1,
                Email = _fakeIdentityUser.Email,
                Password = _fakeIdentityUser.PasswordHash,
                User = _fakeIdentityUser
            };


            var post = await FakeController.Post(user);
            var badRequestObjectResult = (BadRequestObjectResult) post.Result;

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult.Value);
            Assert.Equal("{ message = Email already in use }", badRequestObjectResult.Value.ToString());
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Put_Doctor()
        {
            var count = _fakeIdentityUsers.Count;
            var postUserDto = new UserDto
            {
                Id = 7,
                Email = "emailTwo@gmail.com",
                Password = "test",
                User = _fakeUser
            };
            var putUserDto = new UserDto
            {
                Id = 7,
                Email = "emaiel@gmail.com",
                Password = "test",
                User = _fakeUser
            };
            await FakeController.Post(postUserDto);

            var doctor = (OkResult) await FakeController.PutDoctor(7, putUserDto);
     
            Assert.Equal(200, doctor.StatusCode);
            Assert.Equal(count + 1, _fakeIdentityUsers.Count);
            Assert.Equal(putUserDto.Email, _fakeIdentityUsers[2].Email);
            Assert.Equal(putUserDto.User.UserName, _fakeUsersInformation[0].User.UserName);
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public void Put_Doctor_non_existent_id()
        {
            var putUserDto = new UserDto
            {
                Id = 1,
                Email = "emaiel@gmail.com",
                Password = "password",
                User = _fakeUser
            };

            Assert.ThrowsAsync<Exception>(() => FakeController.PutDoctor(112312312, putUserDto));
        }


        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Put_Doctore_Already_existing()
        {
            var putUserDto = new UserDto
            {
                Id = 7,
                Email = "emaiel@gmail.com",
                Password = "test",
                User = _fakeUser
            };

            var badRequestObjectResult = (OkResult) await FakeController.PutDoctor(1, putUserDto);

            Assert.Equal(200, badRequestObjectResult.StatusCode);
            Assert.Equal(putUserDto.Email, _fakeUsersInformation[2].User.Email);
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Put_Doctor_Email_Already_In_Usee()
        {
            var putUserDto = new UserDto
            {
                Id = 1,
                Email = "email2@gmail.com",
                Password = "password",
            };

            var badRequestObjectResult = (BadRequestObjectResult) await FakeController.PutDoctor(2, putUserDto);

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.Equal("{ message = Email already in use }", badRequestObjectResult.Value.ToString());
        }

        [Trait("Category", "Doctor")]
        [Fact]
        public async Task Put_doctor_not_existing()
        {
            var putUserDto = new UserDto
            {
                Id = 1,
                Email = "email2@gmail.com",
                Password = "password",
            };

            var badRequestObjectResult = (BadRequestObjectResult) await FakeController.PutDoctor(5, putUserDto);

            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.Equal("{ message = This userId doesn't correspond to an existing user }", badRequestObjectResult.Value.ToString());
        }

        //[Trait("Category", "Doctor")]
        //[Fact]
        //public async Task Delete_Doctor()
        //{
        //    var lengthBefore = _fakeUsersInformation.Count;
        //    var userLengthBefore = _fakeIdentityUsers.Count;

        //    var deleteDoctor = await FakeController.DeleteDoctor(1);
        //    var okResult = (OkResult) deleteDoctor;

        //    Assert.Equal(200, okResult.StatusCode);
        //    Assert.Equal(lengthBefore - 1, _fakeUsersInformation.Count);
        //    Assert.Equal(userLengthBefore - 1, _fakeIdentityUsers.Count);
        //}

        internal void SeedData()
        {
            _fakeUser = new IdentityUser
            {
                UserName = "genericUsername",
                PasswordHash = "genericUsername",
                Email = "genericUsername"
            };
            _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "genericUsername",
                Email = "email@gmail.com",
                UserName = "email@gmail.com"
            };
            var akeIdentityUser = new IdentityUser
            {
                PasswordHash = "genericUsername",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com"
            };
            _fakeIdentityUsers = new List<IdentityUser> {_fakeIdentityUser, akeIdentityUser};
            _fakeUsersInformation = new List<UserInformation>();
            _userInformation = new UserInformation
            {
                User = _fakeUser,
                CreatedAt = new DateTime(2000, 10, 10).Date
            };
            var userInformationSecond = new UserInformation
            {
                Id = 2,
                UserId = Guid.Parse(_fakeIdentityUser.Id),
                User = _fakeUser
            };
            var userInformationThird = new UserInformation
            {
                Id = 1,
                UserId = Guid.Parse(akeIdentityUser.Id),
                User = akeIdentityUser
            };
            _fakeUsersInformation.AddRange(new List<UserInformation>
            {
                _userInformation,
                userInformationSecond,
                userInformationThird,
            });
        }
    }
}
