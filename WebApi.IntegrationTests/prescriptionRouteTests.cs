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
using WebApi.Models.Prescriptions;
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApi.IntegrationTests
{
    public class PrescriptionRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public PrescriptionRouteTests(CustomWebApplicationFactory<Startup> factory)
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

            var defaultPage = await _client.GetAsync("/api/prescriptions");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<Prescription>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }

        [Fact, Order(1)]
        public async Task Get_Prescriptions_By_Id_With_Token()
        {
            var defaultPage = await _client.GetAsync("/api/prescriptions/" + 1);
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<Prescription>();


            Environment.SetEnvironmentVariable("prescriptions", json);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<Prescription>(user);
        }

        [Fact, Order(2)]
        public async Task Get_Non_Existing_Prescriptions()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/prescriptions/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var resultPatient = _client.GetAsync("/api/patients/").Result;
            var readAsStringAsyncPatient = resultPatient.Content.ReadAsStringAsync();
            var jsonPatient = readAsStringAsyncPatient.Result;

            var jArray = JArray.Parse(jsonPatient);
            var patientList = jArray.ToObject<List<Patient>>();


            var constulMessage = _client.GetAsync("/api/consultations/").Result;
            var consuAsStringAsync = constulMessage.Content.ReadAsStringAsync();
            var jsonResult = consuAsStringAsync.Result;

            var jArrayr = JArray.Parse(jsonResult);
            var consulList = jArrayr.ToObject<List<Consultation>>();

            var c = new Prescription
            {
                Id = 1,
                Description = "descriptie",
                Patient = patientList[0],
                PatientId = patientList[0].Id,
                ConsultationId = consulList[0].Id,
                Consultation = consulList[0]
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = _client.PostAsync("/api/prescriptions/", content).Result;

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
                Id = 1,
                Date = DateTime.Now,
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = _client.PostAsync("/api/prescriptions/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Update_prescriptions_And_test_If_User_Changed()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("prescriptions");
            var jObject = JObject.Parse(environmentVariable);
            var dto = jObject.ToObject<Prescription>();

            var update = new UpdatePrescriptionDto
            {
                Description = "commentscomments",
                PatientId = dto.Patient.Id,
                ConsultationId = dto.Consultation.Id,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now
            };

            var serialize = JsonConvert.SerializeObject(update);
            var content = new StringContent(serialize, Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/prescriptions/" + dto.Id, content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<Prescription>();


            var defaultPager = await _client.GetAsync("/api/prescriptions/" + dto.Id);
            var asStringAsync = defaultPager.Content.ReadAsStringAsync();
            var result = asStringAsync.Result;
            var parsedJObject = JObject.Parse(result);
            var userDto = parsedJObject.ToObject<Prescription>();

            Assert.Equal(HttpStatusCode.OK, defaultPager.StatusCode);
            Assert.NotNull(environmentVariable);
            Assert.IsType<Prescription>(userDto);
            Assert.Equal(update.Description, userDto.Description);
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("prescriptions");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<Consultation>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/prescriptions/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(6)]
        public async Task Given_Prescription_Id_Deletes_prescriptions()
        {
            var userr = Environment.GetEnvironmentVariable("prescriptions");
            var du = JObject.Parse(userr);
            var udser = du.ToObject<Prescription>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/consultations/" + udser.Id.ToString());

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}
