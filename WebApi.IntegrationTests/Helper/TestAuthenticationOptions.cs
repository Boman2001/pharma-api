using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.IntegrationTests.Helper
{
    class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, "6105002a-295f-49b1-ace3-2072c7edbb69"),
                new (ClaimTypes.Name, "Test user"),
                new (ClaimTypes.Email, "test@gmail.com"),
                new (ClaimTypes.Role, "Admin")
            }));

            await next();
        }
    }
}
