using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WebApi.Models.Authentication;

namespace WebApi.IntegrationTests
{
    public class AuthRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        private JToken _token;

        public AuthRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_Valid_Login_Details_Returns_Ok_And_Valid_Token()
        {
            var newUserDto = new LoginDto { Email = "m@gmail.com", Password = "password" };

            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;

            var readAsStringAsync = result.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var jObject = JObject.Parse(json);
            var email = jObject["email"].Value<string>();


            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(email, newUserDto.Email);
            
        }


        [Trait("Category", "Routes")]
        [Fact]
        public async Task Requesting_To_Non_Existing_Route_Gives_Bad_Request()
        {
            var defaultPage = await _client.GetAsync("/ntoFound");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_No_Email_Login_Returns_Bad_Request()
        {
            var newUserDto = new LoginDto {Password = "password"};


            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_No_Password_Login_Returns_Bad_Request()
        {
            var newUserDto = new LoginDto {Email = "m@gmail.com"};


            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }


        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_No_Data_Login_Returns_Bad_Request()
        {
            var newUserDto = new LoginDto();


            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_InCorrect_Email_Returns_Bad_Request()
        {
            var newUserDto = new LoginDto {Email = "incorrect@gmail.com", Password = "password"};
            
            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public void Given_InCorrect_Password_Returns_Bad_Request()
        {
            var newUserDto = new LoginDto {Email = "incorrect@gmail.com", Password = "passwordIncorrect"};

            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/auth/login", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }
    }
}
