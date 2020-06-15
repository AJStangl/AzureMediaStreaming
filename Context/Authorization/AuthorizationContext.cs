using AzureMediaStreaming.Context.Authorization.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AzureMediaStreaming.Context.Authorization
{
    public class AuthorizationContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public AuthorizationContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }
}