using System;
using System.Collections.Generic;
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
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApi.IntegrationTests
{
    public class AdditionalExaminationResultsRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AdditionalExaminationResultsRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }


        [Fact, Order(0)]
        public async Task Get_AdditionalExaminationResult_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationResults");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<AdditionalExaminationResult>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }

        [Fact, Order(1)]
        public async Task Get_AdditionalExaminationResult_By_Id_With_Token()
        {
            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationResults/" + 1);
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<AdditionalExaminationResult>();


            Environment.SetEnvironmentVariable("exmanation", json);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<AdditionalExaminationResult>(user);
        }

        [Fact, Order(2)]
        public async Task Get_Non_Existing_AdditionalExaminationResult()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationResults/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);


            var c = new AdditionalExaminationResultDto
            {
                ConsultationId = 1,
                PatientId = 1,
                Value = "namname",
                AdditionalExaminationTypeId = 1,
                Date = DateTime.Now
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = _client.PostAsync("/api/AdditionalExaminationResults/", content).Result;

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Invalid_Post_Data_Gives_Error()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var c = new Consultation
            {
                Id = 1, Date = DateTime.Now
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/AdditionalExaminationResults/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("exmanation");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<Consultation>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/AdditionalExaminationResults/" + userDto.Id, content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(6)]
        public async Task Given_AdditionalExaminationResult_Id_Deletes_prescriptions()
        {
            var userr = Environment.GetEnvironmentVariable("exmanation");
            var du = JObject.Parse(userr);
            var udser = du.ToObject<AdditionalExaminationResult>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/AdditionalExaminationResults/" + udser.Id);

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}