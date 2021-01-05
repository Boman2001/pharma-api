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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel;

namespace WebApi.Tests
{
   public class LoginControllerTests
    {
        private IIdentityRepository Service { get; }
        private LoginController Controller { get; }
        public IConfiguration Configuration { get; }
        
        public LoginControllerTests()
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfiguration config = new ConfigurationBuilder()
            
               .AddJsonFile("appsettings.json")
               .Build();
            var _FakeUserManager = new FakeUserManager();
            var FakeSignInManager = new FakeSignInManager();

            var userStore = new Mock<IUserStore<User>>();


            //Setup DbContext and DbSet mock
            var dbContextMock = new Mock<IdentityAppdbContext>();
            var dbSetMock = new Mock<DbSet<User>>();
            dbSetMock.Setup(s => s.FindAsync(It.IsAny<Guid>())).Returns(ValueTask.FromResult(new User()));
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            ////Execute method of SUT (ProductsRepository)
            //Repository = new IdentityRepository(_FakeUserManager, FakeSignInManager, config);
           
           Service = new IIdentityRespositoryServiceFake(_FakeUserManager, FakeSignInManager, config);
           Controller = new LoginController(_FakeUserManager, Service, config);
        }



        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Valid_Response()
        {
            User a = new User
            {
                Email = "email@gmail.com"
            };
            String password = "bijen";
            OkObjectResult result = (OkObjectResult)await Controller.LoginAsync(a, password);
            Assert.Equal(result.StatusCode, 200);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Non_Valid_Email_Response()
        {
            User a = new User
            {
                Email = "email"
            };
            String password = "bijen";
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.LoginAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Email isnt valid or set }", b);
            Assert.Equal(400, aa.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_No_Data_Response()
        {
            User a = new User
            {
            };
            String password = "bijen";
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.LoginAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Email isnt valid or set }", b);
            Assert.Equal(400, aa.StatusCode);
        }

       

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Only_Username_Email_Response()
        {
            User a = new User
            {
                Email = "email@gmail.com"
            };
            String password = null;
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.LoginAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Password not given }", b);
            Assert.Equal(400, aa.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Valid_Response()
        {
            User a = new User
            {
                Email = "email@gmail.com"
            };
            String password = "bijen";
            OkObjectResult result = (OkObjectResult)await Controller.RegisterAsync(a, password);
            Assert.Equal(result.StatusCode, 200);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Non_Valid_Email_Response()
        {
            User a = new User
            {
                Email = "email"
            };
            String password = "bijen";
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.RegisterAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Email isnt valid or set }", b);
            Assert.Equal(400, aa.StatusCode);
        }


        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_No_Data_Response()
        {
            User a = new User
            {
            };
            String password = "bijen";
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.RegisterAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Email not given }", b);
            Assert.Equal(400, aa.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Only_Username_Email_Response()
        {
            User a = new User
            {
                Email = "email@gmail.com"
            };
            String password = null;
            BadRequestObjectResult result = (BadRequestObjectResult)await Controller.RegisterAsync(a, password);
            ObjectResult aa = (ObjectResult)result;
            var ad = aa.Value;
            string b = ad.ToString();
            Assert.Equal("{ message = Password not given }", b);
            Assert.Equal(400, aa.StatusCode);
        }

    }
}
