using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models.AdditionalExaminationResults;
using WebApi.Models.Episodes;
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApi.IntegrationTests
{
    public class EpisodeRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public EpisodeRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact, Order(0)]
        public async Task Get_Episodes_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/Episodes");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<Episode>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }


        [Fact, Order(1)]
        public async Task Get_Episodes_By_Id_With_Token()
        {
            var defaultPage = await _client.GetAsync("/api/Episodes/" + 1);
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<Episode>();


            Environment.SetEnvironmentVariable("Episode", json);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<Episode>(user);
        }

        [Fact, Order(2)]
        public async Task Get_Non_Existing_Episode()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/Episodes/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);


            var c = new EpisodeDto()
            {
                Description = "NewDesc",
                ConsultationId = 1,
                PatientId = 1,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Priority = 10,
                IcpcCodeId = 1
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = _client.PostAsync("/api/Episodes/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("Episode");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<EpisodeDto>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/Episodes/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(6)]
        public async Task Given_AdditionalExaminationResult_Id_Deletes_episodes()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("Episode");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<EpisodeDto>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/Episodes/" + userDto.Id.ToString());

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}
