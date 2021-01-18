using System.Collections.Generic;
using System.Linq;
using Core.DomainServices.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Core.DomainServices.Tests
{
    public class AuthHelperTests
    {
        [Trait("Category", "Jwt Tests")]
        [Fact]
        public void Given_User_Returns_Jwt_With_roles()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var authHelper = new AuthHelper(config);

            var identityUser = new IdentityUser
            {
                Email = "maartendonkersloot@gmail.com",
                UserName = "maartendonkersloot@gmail.com",
                PasswordHash = "password"
            };

            var roleList = new List<string>
            {
                "Doctor"
            };

            var result = authHelper.GenerateToken(identityUser, roleList);

            Assert.Equal(6, result.Claims.Count());
        }

        [Trait("Category", "Jwt Tests")]
        [Fact]
        public void Given_User_Returns_Jwt_Without_Roles()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var authHelper = new AuthHelper(config);

            var identityUser = new IdentityUser
            {
                Email = "maartendonkersloot@gmail.com",
                UserName = "maartendonkersloot@gmail.com",
                PasswordHash = "password"
            };

            var result = authHelper.GenerateToken(identityUser, null);

            Assert.Equal(5, result.Claims.Count());
        }
    }
}