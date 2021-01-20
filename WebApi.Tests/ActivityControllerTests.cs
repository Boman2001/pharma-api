using System.Collections.Generic;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Controllers;
using WebApi.Tests.Mocks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace WebApi.Tests
{
    public class ActivityControllerTests
    {
        private List<Activity> _fakeActivities;
        private List<IdentityUser> _fakeIdentityUsers;
        private ActivitiesController FakeController { get; }

        public ActivityControllerTests()
        {
            SeedData();
            var fakeGenericRepo = MockGenericRepository.GetUserInformationMock(_fakeActivities);
            FakeController = new ActivitiesController(fakeGenericRepo.Object);
            SetUser(_fakeIdentityUsers[0]);
        }


        [Trait("Category", "Get Tests")]
        [Fact]
        public void Get_All_Activities_With_200_code()
        {
            var result = FakeController.Get();
            var objectResult = (OkObjectResult) result.Result;
            var activities = (List<Activity>) objectResult.Value;

            Assert.Equal(_fakeActivities.Count, activities.Count);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(activities[0], _fakeActivities[0]);
            Assert.IsType<Activity>(activities[0]);
        }

        private void SetUser(IdentityUser identity)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, identity.Id), new Claim(ClaimTypes.Sid, identity.Id)
            }, "mock"));

            FakeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };
        }

        private void SeedData()
        {
            var activity = new Activity
            {
                Description = "Description",
                Properties = "Properties",
                SubjectId = 10,
                SubjectType = "Type"
            };
            var activity02 = new Activity
            {
                Description = "Description",
                Properties = "Properties",
                SubjectId = 10,
                SubjectType = "Type"
            };
            _fakeActivities = new List<Activity>
            {
                activity, activity02
            };
            var fakeIdentityUser = new IdentityUser
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

            _fakeIdentityUsers = new List<IdentityUser>
            {
                fakeIdentityUser
            };
        }
    }
}
