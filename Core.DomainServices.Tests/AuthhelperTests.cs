using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Xunit;
using Core.DomainServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Core.DomainServices.Tests
{
    public class AuthhelperTests
    {
        private AuthHelper _authHelper;
        public AuthhelperTests()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            _authHelper = new AuthHelper(config);
        }

        [Trait("Category", "Email Validation")]
        [Fact]
        public async Task Email_Invalled()
        {
            var falseEmaile = "not a valid email";
            Exception result = null;
            try
            {
                _authHelper.IsValidEmail(falseEmaile);
            }
            catch (Exception e)
            {
                result = e;
            }

            Assert.Equal("Incorrect email", result.Message);
        }

        [Trait("Category", "Email Validation")]
        [Fact]
        public async Task Email_valid()
        {
            var validEmail = "maartendonkersloot@gmail.com";
           
            bool result = _authHelper.IsValidEmail(validEmail);
         
            Assert.Equal(true, result);
        }

        [Trait("Category", "Jwt validation test")]
        [Fact]
        public async Task Returns_Jwt()
        {
            IdentityUser identityUser = new IdentityUser();
            identityUser.Email = "maartendonkersloot@gmail.com";
            identityUser.UserName = identityUser.Email;
            identityUser.PasswordHash = "password";

            IList<string> RoleList = new List<string>();
            RoleList.Add("DOCTOR");

            JwtSecurityToken result = _authHelper.GenerateToken(identityUser, RoleList);

            Assert.Equal(5,result.Claims.Count());
        }

    }
}
