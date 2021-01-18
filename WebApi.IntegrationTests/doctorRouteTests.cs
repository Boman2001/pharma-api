using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models.Authentication;
using WebApi.Models.Users;
using Xunit;
using Xunit.Extensions.Ordering;
using Assert = Xunit.Assert;

namespace WebApi.IntegrationTests
{
    public class DoctorRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private JToken _token;
        private CustomWebApplicationFactory<Startup> _factory;

        public DoctorRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            
        }

        [Trait("Category", "api/doctors")]
        [Fact, Order(0)]
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

            _token = jObject["token"].Value<string>();
            Environment.SetEnvironmentVariable("Token", _token.ToString());

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(_token.ToString()) as JwtSecurityToken;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsType<JwtSecurityToken>(token);
            Assert.InRange(token.Claims.Count(), 5, 10);
            Assert.InRange(token.Payload.Count, 5, 10);
        }

        [Fact, Order(0)]
        public async Task Get_Doctors_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/users");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<UserDto>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }


        [Fact, Order(1)]
        public async Task Get_Doctor_By_Id_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var httpResponseMessage = await _client.GetAsync("/api/users");
            var readAsStringAsync = httpResponseMessage.Content.ReadAsStringAsync();

            var result = readAsStringAsync.Result;
            var jArray = JArray.Parse(result);
            var users = jArray.ToObject<List<UserDto>>();


            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/users/" + users[0].Id.ToString());
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<UserDto>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<UserDto>(user);
        }

        [Fact, Order(2)]
        public async Task Test3_Get_Non_Existing_Doctor()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/users/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var newUserDto = new NewUserDto
            {
                Email = "new@gmail.com",
                Password = "password",
                City = "hank",
                Country = "netherlands",
                Dob = DateTime.Now,
                Gender = Gender.Female,
                HouseNumber = "20",
                HouseNumberAddon = "as",
                Street = "lepelaarstraat",
                Name = "maarten",
                PostalCode = "3273cv",
                PhoneNumber = "+316236132"
            };

            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/users/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<UserDto>();
            var userSerializeObject = JsonConvert.SerializeObject(user);

            Environment.SetEnvironmentVariable("User", userSerializeObject);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(newUserDto.Name, user.Name);
        }

        [Fact, Order(3)]
        public void Given_Invalid_Post_Data_Gives_Error()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var newUserDto = new NewUserDto
            {
                Email = "new@gmail.com",
                Password = "password",
                City = "hank",
            };

            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/users/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(3)]
        public void Given_Already_Existing_Email_Returns_Error()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var newUserDto = new NewUserDto
            {
                Email = "m1@gmail.com",
                Password = "password",
                City = "hank",
                Country = "netherlands",
                Dob = DateTime.Now,
                Gender = Gender.Female,
                HouseNumber = "20",
                HouseNumberAddon = "as",
                Street = "lepelaarstraat",
                Name = "maarten",
                PostalCode = "3273cv",
                PhoneNumber = "+316236132"
            };

            var serialize = JsonConvert.SerializeObject(newUserDto);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/users/", content).Result;
            var resultContent = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(resultContent.Result.Length, 10, int.MaxValue);
            Assert.Equal("E-mailadres is al in gebruik.", resultContent.Result);
        }


        [Fact, Order(4)]
        public async Task Update_User_And_test_If_User_Changed()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            var environmentVariable = Environment.GetEnvironmentVariable("User");
            var jObject = JObject.Parse(environmentVariable);
            var dto = jObject.ToObject<UserDto>();

            var serialize = JsonConvert.SerializeObject(dto);
            var content = new StringContent(serialize, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.PutAsync("/api/users/" + dto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<UserDto>();

            var defaultPager = await _client.GetAsync("/api/users/" + user.Id.ToString());
            var asStringAsync = defaultPager.Content.ReadAsStringAsync();
            var result = asStringAsync.Result;
            var parsedJObject = JObject.Parse(result);
            var userDto = parsedJObject.ToObject<UserDto>();

            Assert.Equal(HttpStatusCode.OK, defaultPager.StatusCode);
            Assert.NotNull(environmentVariable);
            Assert.IsType<UserDto>(userDto);
            Assert.Equal(userDto.Name, user.Name);
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.Equal(dto.Name, user.Name);
        }

        [Fact, Order(4)]
        public async Task Given_Update_Data_Update_Already_Existing_Email()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            var environmentVariable = Environment.GetEnvironmentVariable("User");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<UserDto>();
            userDto.Email = "m2@gmail.com";
            var serialize = JsonConvert.SerializeObject(userDto);
            var content = new StringContent(serialize, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.PutAsync("/api/users/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            var defaultPager = await _client.GetAsync("/api/users/" + userDto.Id.ToString());
            var asStringAsync = defaultPager.Content.ReadAsStringAsync();
            var result = asStringAsync.Result;
            var resultObject = JObject.Parse(result);
            var dto = resultObject.ToObject<UserDto>();

            Assert.Equal(HttpStatusCode.OK, defaultPager.StatusCode);
            Assert.NotNull(environmentVariable);
            Assert.IsType<UserDto>(dto);
            Assert.NotEqual(dto.Email, userDto.Email);
            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            var environmentVariable = Environment.GetEnvironmentVariable("User");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<UserDto>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.PutAsync("/api/users/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Given_Nonexistent_Update_Returns_Error()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var content = new StringContent($"email={"d@LEander.com"}&name={"new@LEander.com"}&password={"new"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PutAsync("/api/users/5454", content);

            Assert.Equal(HttpStatusCode.UnsupportedMediaType, defaultPage.StatusCode);
        }


        [Fact, Order(6)]
        public async Task Given_User_Id_Deletes_User()
        {
            var userr = Environment.GetEnvironmentVariable("User");
            var du = JObject.Parse(userr);
            var udser = du.ToObject<UserDto>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/users/" + udser.Id.ToString());

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }

        [Fact]
        public async Task UnAuth_Get_Doctor()
        {
            var defaultPage = await _client.GetAsync("/api/users/1");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact]
        public async Task UnAuth_Get_Non_Existing_Doctor()
        {
            var defaultPage = await _client.GetAsync("/api/users/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact]
        public async Task UnAuth_Update()
        {
            var content = new StringContent($"email={"d@LEander.com"}&name={"new@LEander.com"}&password={"new"}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var defaultPage = await _client.PutAsync("/api/users/3", content);

            Assert.Equal(HttpStatusCode.UnsupportedMediaType, defaultPage.StatusCode);
        }

        [Fact]
        public async Task Unauth_delete_doctor()
        {
            var defaultPage = await _client.DeleteAsync("/api/users/1");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }
    }
}
