using System;
using System.Collections.Generic;
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
using WebApi.Models.Consultations;
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApi.IntegrationTests
{
    public class ConsultationRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private JToken _token;
        private CustomWebApplicationFactory<Startup> _factory;

        public ConsultationRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact, Order(0)]
        public async Task Get_Consultations_With_Token()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var defaultPage = await _client.GetAsync("/api/consultations");
            var content = defaultPage.Content.ReadAsStringAsync();

            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<Consultation>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.InRange(usersList.Count, 1, 100);
        }

        [Fact, Order(1)]
        public async Task Get_Consultations_By_Id_With_Token()
        {
            var defaultPage = await _client.GetAsync("/api/consultations/" + 1);
            var asStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = asStringAsync.Result;
            var jToken = JObject.Parse(json);
            var user = jToken.ToObject<Consultation>();


            Environment.SetEnvironmentVariable("consultation", json);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
            Assert.IsType<Consultation>(user);
        }

        [Fact, Order(2)]
        public async Task Get_Non_Existing_Consultation()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.GetAsync("/api/consultations/1323232");

            Assert.Equal(HttpStatusCode.NotFound, defaultPage.StatusCode);
        }

        [Fact, Order(3)]
        public void Given_Valid_Post_Data_Posts()
        {
            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);

            var admin = new IdentityUser
            {
                Id = "6105002a-295f-49b1-ace3-2072c7edbb69",
                UserName = "m@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                Email = "m@gmail.com",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };
            Patient p = new Patient
            {
                Name = "jim",
                Bsn = "bsn",
                Email = "jim@jim.com",
                Dob = DateTime.Now,
                Gender = Gender.Other,
                PhoneNumber = "124124",
                City = "hank",
                Street = "lepelaarstraat",
                HouseNumber = "20",
                HouseNumberAddon = "",
                PostalCode = "4273cv",
                Country = "Netherlands"
            };


            var resultPatient = _client.GetAsync("/api/patients/").Result;
            var readAsStringAsyncPatient = resultPatient.Content.ReadAsStringAsync();
            var jsonPatient = readAsStringAsyncPatient.Result;

            var jArray = JArray.Parse(jsonPatient);
            var patientList = jArray.ToObject<List<Patient>>();

            Consultation c = new Consultation
            {
                Id = 1,
                Date = DateTime.Now,
                Comments = "comments",
                DoctorId = Guid.Parse(admin.Id),
                Doctor = admin,
                Patient = patientList[0],
                PatientId = patientList[0].Id
            };

            var serialize = JsonConvert.SerializeObject(c);

            var content = new StringContent(serialize, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = _client.PostAsync("/api/consultations/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<Consultation>();
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
            var result = _client.PostAsync("/api/consultations/", content).Result;
            var readAsStringAsync = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(4)]
        public async Task Update_Consultation_And_test_If_User_Changed()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("consultation");
            var jObject = JObject.Parse(environmentVariable);
            var dto = jObject.ToObject<Consultation>();
            dto.Comments = "NEW COMMENT";

            UpdateConsultationDto update = new UpdateConsultationDto
            {
                Comments = "commentscomments",
                DoctorId = Guid.Parse(dto.Doctor.Id),
                PatientId = dto.Patient.Id,
                Date = DateTime.Now,
                
            };

            var serialize = JsonConvert.SerializeObject(update);
            var content = new StringContent(serialize, Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/consultations/" + dto.Id, content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();
            var json = readAsStringAsync.Result;
            var u = JObject.Parse(json);
            var user = u.ToObject<Consultation>();


            var defaultPager = await _client.GetAsync("/api/consultations/" + dto.Id);
            var asStringAsync = defaultPager.Content.ReadAsStringAsync();
            var result = asStringAsync.Result;
            var parsedJObject = JObject.Parse(result);
            var userDto = parsedJObject.ToObject<Consultation>();

            Assert.Equal(HttpStatusCode.OK, defaultPager.StatusCode);
            Assert.NotNull(environmentVariable);
            Assert.IsType<Consultation>(userDto);
            Assert.Equal(update.Comments, user.Comments);
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.NotNull(user);
        }

        [Fact, Order(4)]
        public async Task Given_Invalid_Data_Update_Return_Bad_Request()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("consultation");
            var jObject = JObject.Parse(environmentVariable);
            var userDto = jObject.ToObject<Consultation>();

            var content = new StringContent("test", Encoding.UTF8, "application/json");

            var defaultPage = await _client.PutAsync("/api/consultations/" + userDto.Id.ToString(), content);
            var readAsStringAsync = defaultPage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, defaultPage.StatusCode);
            Assert.InRange(readAsStringAsync.Result.Length, 10, int.MaxValue);
        }

        [Fact, Order(6)]
        public async Task Given_Consultation_Id_Deletes_Consultation()
        {
            var userr = Environment.GetEnvironmentVariable("consultation");
            var du = JObject.Parse(userr);
            var udser = du.ToObject<Consultation>();

            var tokenEnvironmentVariable = Environment.GetEnvironmentVariable("Token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenEnvironmentVariable);
            var defaultPage = await _client.DeleteAsync("/api/consultations/" + udser.Id.ToString());

            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}
