using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Tests.Helpers
{
    public class IdentityHelper
    {
        public static List<IdentityUser> GetIdentityUsers()
        {
            var _fakeIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email@gmail.com",
                UserName = "email@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };
            var extraIdentityUser = new IdentityUser
            {
                PasswordHash = "password",
                Email = "email2@gmail.com",
                UserName = "email@gmail.com",
                PhoneNumber = "+31623183611",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "M@GMAIL.COM",
                NormalizedEmail = "M@GMAIL.COM",
                EmailConfirmed = true,
            };

            return new List<IdentityUser>
            {
                _fakeIdentityUser, extraIdentityUser
            };
        }

        public static void SetUser(IdentityUser identity, ControllerBase FakeController)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, identity.Id), new Claim(ClaimTypes.Sid, identity.Id)
            }, "mock"));

            FakeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };
        }
    }
}
