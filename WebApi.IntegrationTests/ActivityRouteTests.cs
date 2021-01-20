using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using Assert = Xunit.Assert;


namespace WebApi.IntegrationTests
{
   public class ActivityRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ActivityRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Trait("Category", "api/activities")]
        [Fact, Order(0)]
        public async Task Gets_Activities()
        {
            var defaultPage = await _client.GetAsync("/api/activities");
            var content = defaultPage.Content.ReadAsStringAsync();


            var json = content.Result;
            var jArray = JArray.Parse(json);
            var usersList = jArray.ToObject<List<Activity>>();

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(2, usersList.Count);
        }
    }
}
