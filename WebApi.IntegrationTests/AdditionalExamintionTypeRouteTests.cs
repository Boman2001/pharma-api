using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models.AdditionalExaminationTypes;
using WebApi.Models.Prescriptions;
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApi.IntegrationTests
{
    public class AdditionalExamintionTypeRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private JToken _token;
        private CustomWebApplicationFactory<Startup> _factory;
        public AdditionalExamintionTypeRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact, Order(0)]
        public async Task Get_Prescriptions_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationTypes");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<AdditionalExaminationType>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }


        [Fact, Order(1)]
        public async Task Get_Prescriptions_By_Id_With_Token()
        {
            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationTypes/" + 1);
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<AdditionalExaminationType>();


            Environment.SetEnvironmentVariable("exmanation", json);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<AdditionalExaminationType>(user);
        }

        [Fact, Order(2)]
        public async Task Get_Non_Existing_Prescriptions()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/AdditionalExaminationTypes/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);


            AdditionalExaminationType c = new AdditionalExaminationType
            {
                Name = "namname",
                Unit = "unitunit"
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = _client.PostAsync("/api/AdditionalExaminationTypes/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<AdditionalExaminationType>();
            var userSerializeObject = JsonConvert.SerializeObject(user);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Invalid_Post_Data_Gives_Error()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            Consultation c = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/AdditionalExaminationTypes/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Update_prescriptions_And_test_If_User_Changed()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("exmanation");
            var jObject = JObject.Parse(environmentVariable);
            var dto = jObject.ToObject<AdditionalExaminationTypeDto>();

            AdditionalExaminationTypeDto update = new AdditionalExaminationTypeDto
            {
                Id = dto.Id,
                Name = dto.Name,
                Unit = dto.Unit
            };

            var serialize = JsonConvert.SerializeObject(update);
            var content = new StringContent(serialize, Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/AdditionalExaminationTypes/" + dto.Id, content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<AdditionalExaminationType>();


            var defaultPager = await _client.GetAsync("/api/AdditionalExaminationTypes/" + dto.Id);
            var asStringAsync = defaultPager.Content.ReadAsStringAsync();
            var result = asStringAsync.Result;
            var parsedJObject = JObject.Parse(result);
            var userDto = parsedJObject.ToObject<AdditionalExaminationType>();

            Assert.Equal(HttpStatusCode.OK, defaultPager.StatusCode);
            Assert.NotNull(environmentVariable);
            Assert.IsType<AdditionalExaminationType>(userDto);
            Assert.Equal(update.Name, update.Name);
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("exmanation");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<Consultation>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/AdditionalExaminationTypes/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(6)]
        public async Task Given_Prescription_Id_Deletes_prescriptions()
        {
            var userr = Environment.GetEnvironmentVariable("exmanation");
            var du = JObject.Parse(userr);
            var udser = du.ToObject<AdditionalExaminationType>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/AdditionalExaminationTypes/" + udser.Id.ToString());

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}
