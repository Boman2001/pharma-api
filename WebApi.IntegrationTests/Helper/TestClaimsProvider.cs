using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApi.IntegrationTests.Helper
{
    public class TestClaimsProvider
    {
        private IList<Claim> Claims { get; }

        public TestClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public TestClaimsProvider()
        {
            Claims = new List<Claim>();
        }

        public static TestClaimsProvider WithAdminClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            provider.Claims.Add(new Claim(ClaimTypes.Name, "Admin user"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, "Doctor"));
            return provider;
        }

        public static TestClaimsProvider WithUserClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            provider.Claims.Add(new Claim(ClaimTypes.Name, "User"));

            return provider;
        }
    }
}
