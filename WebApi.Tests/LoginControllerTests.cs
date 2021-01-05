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
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            Service = new IdentityRepositoryServiceFake(config);
            Controller = new LoginController(Service);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Valid_Response()
        {
            User User = new User { Email = "email@gmail.com" };
            String Password = "bijen";

            OkObjectResult ObjectResult = (OkObjectResult)await Controller.LoginAsync(User, Password);

            Assert.Equal( 200, ObjectResult.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Non_Valid_Email_Response()
        {
            User User = new User { Email = "email" };
            String Password = "bijen";

            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.LoginAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;
            string StringCast = ObjectResult.Value.ToString();

            Assert.Equal("{ message = Incorrect email }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_No_Data_Response()
        {
            User User = new User{};
            String Password = "bijen";

            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.LoginAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;
            string StringCast = ObjectResult.Value.ToString();

            Assert.Equal("{ message = Incorrect email }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }

       

        [Trait("Category", "Login")]
        [Fact]
        public async Task Login_Only_Username_Email_Response()
        {
            User User = new User { Email = "email@gmail.com" };
            String Password = null;

            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.LoginAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;
            string StringCast = ObjectResult.Value.ToString();

            Assert.Equal("{ message = Password not given }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Valid_Response()
        {
            User User = new User { Email = "email@gmail.com" };
            String Password = "bijen";

            OkObjectResult Result = (OkObjectResult)await Controller.RegisterAsync(User, Password);

            Assert.Equal(200, Result.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Non_Valid_Email_Response()
        {
            User User = new User { Email = "email" };
            String Password = "bijen";

            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.RegisterAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;

            
            string StringCast = ObjectResult.Value.ToString();
            Assert.Equal("{ message = Incorrect email }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }


        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_No_Data_Response()
        {
            User User = new User {};
            String Password = "bijen";

            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.RegisterAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;

            string StringCast = ObjectResult.Value.ToString();
            Assert.Equal("{ message = Incorrect email }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }

        [Trait("Category", "Register")]
        [Fact]
        public async Task Register_Only_Username_Email_Response()
        {
            User User = new User
            {
                Email = "email@gmail.com"
            };
            String Password = null;
            BadRequestObjectResult Result = (BadRequestObjectResult)await Controller.RegisterAsync(User, Password);
            ObjectResult ObjectResult = (ObjectResult)Result;

            string StringCast = ObjectResult.Value.ToString();
            Assert.Equal("{ message = Password not given }", StringCast);
            Assert.Equal(400, ObjectResult.StatusCode);
        }

    }
}
