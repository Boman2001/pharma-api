using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Infrastructure;
using Core.DomainServices;
using Core.Domain;
using WebApi.Controllers;
using WebApi;
using Moq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Core.Domain.Tests
{
   public class UserModelTests
    {

        private IdentityRepository Repository { get; set; }

        private IIdentityRepository Service { get; }

        private LoginController Controller { get; }
        public IConfiguration Configuration { get; }
        private Mock<IConfiguration> _config;

        public UserModelTests()
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfiguration config = new ConfigurationBuilder()
            
               .AddJsonFile("appsettings.json")
               .Build();
            var _FakeUserManager = new FakeUserManager();
            var FakeSignInManager = new FakeSignInManager();

            Service = new IIdentityRespositoryServiceFake(_FakeUserManager, FakeSignInManager, config);
           Controller = new LoginController(_FakeUserManager, Service, config);
        }

        [Fact]
        public async Task StatusCode200WhenRegister()
        {
            User a = new User
            {
                Email = "email@gmail.com",
                UserName = "harry"
            };
            String password = "bijen";
            OkObjectResult result = (OkObjectResult)await Controller.RegisterAsync(a, password);
            Assert.Equal(result.StatusCode, 200);
        }


        [Fact]
        public async Task StatusCode200WhenLogin()
        {
            User a = new User
            {
                Email = "email@gmail.com",
                UserName = "harry"
            };
            String password = "bijen";
            OkObjectResult result = (OkObjectResult)await Controller.LoginAsync(a, password);
            Assert.Equal(result.StatusCode, 200);
        }




    }


}
