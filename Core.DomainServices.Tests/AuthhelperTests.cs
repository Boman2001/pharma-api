using System.Collections.Generic;
using System.Linq;
using Core.DomainServices.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Core.DomainServices.Tests
{
    public class AuthhelperTests
    {
        [Trait("Category", "Jwt validation test")]
        [Fact]
        public void Returns_Jwt()
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
    }
}