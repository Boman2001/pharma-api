using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json.Linq;

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

        [Trait("Category", "Routes")]
        [Fact]
        public async Task Step0_Not_Found_Test()
        {
            var defaultPage = await _client.GetAsync("/ntoFound");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public async Task Step1_Register()
        {
            var content = new StringContent($"email={"de@LEander.com"}&password={"eLEANDER223"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/doctors", content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            _token = JObject.Parse(json)["token"];
            var jToken = JObject.Parse(json)["user"];
            var user = jToken.ToObject<IdentityUser>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(_token);
            Assert.Equal("de@LEander.com", user.Email);
        }

        [Trait("Category", "api/auth.login")]
        [Fact]
        public async Task Step2_Login()
        {
            var content = new StringContent($"email={"de@LEander.com"}&password={"eLEANDER223"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/auth/login", content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            _token = JObject.Parse(json)["token"];
            var jToken = JObject.Parse(json)["user"];
            var user = jToken.ToObject<IdentityUser>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(_token);
            Assert.Equal("de@LEander.com", user.Email);
        }

        [Trait("Category", "api/auth.login")]
        [Fact]
        public async Task Step3_Login_non_existent_user()
        {
            var content = new StringContent($"email={"d@LEanader.com"}&password={"LEANDEaR223"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/auth/login", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["message"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
            Assert.Equal("User doesnt exist", message);
        }

        [Trait("Category", "api/auth.login")]
        [Fact]
        public async Task Step4_Login_wrong_password()
        {
            var content = new StringContent($"email={"d@LEander.com"}&password={"LEANDEaR223dsadsda"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/auth/login", content);
            var contenTask = defaultPage.Content.ReadAsStringAsync();

            var json = contenTask.Result;
            var message = JObject.Parse(json)["message"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
        }

        [Trait("Category", "api/auth.login")]
        [Fact]
        public async Task Step5_Login_wrong_email()
        {
            var content = new StringContent($"email={"d@LEanderr.com"}&password={"LEANDEaR223"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/auth/login", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["message"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
            Assert.Equal("User doesnt exist", message);
        }

        [Trait("Category", "api/auth.login")]
        [Fact]
        public async Task Step6_Login_no_data()
        {
            var content = new StringContent($"",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/auth/login", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["message"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
            Assert.Equal("Incorrect password", message);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public async Task Step7_Register_already_existing_email()
        {
            var content = new StringContent($"email={"de@LEander.com"}&password={"LEANDER223"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/doctors", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["message"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
        }
        
        [Trait("Category", "api/doctors")]
        [Fact]
        public async Task Register_no_password()
        {
            var content = new StringContent($"email={"d@LEander.com"}&password={""}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/doctors", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["errors"]["Password"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public async Task Register_no_email()
        {
            var content = new StringContent($"email={""}&password={"a"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/doctors", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["errors"]["Email"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
        }

        [Trait("Category", "api/doctors")]
        [Fact]
        public async Task Register_no_data()
        {
            var content = new StringContent($"email={""}&password={""}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PostAsync("/api/doctors", content);
            var contesnt = defaultPage.Content.ReadAsStringAsync();

            var json = contesnt.Result;
            var message = JObject.Parse(json)["errors"]["Email"];
            var passwordMessage = JObject.Parse(json)["errors"]["Password"];
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.NotNull(message);
            Assert.NotNull(passwordMessage);
        }
    }
}
