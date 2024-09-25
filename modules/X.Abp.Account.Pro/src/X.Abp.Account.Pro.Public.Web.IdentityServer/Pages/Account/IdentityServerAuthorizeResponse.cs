// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

public static class IdentityServerAuthorizeResponse
{
    public static async Task GenerateAuthorizeResponseAsync(HttpContext context, IdentityUser user, params Claim[] additionalClaims)
    {
        var parameters = context.Request.Query.AsNameValueCollection();

        var idp = context.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdentityProvider) ??
                  new Claim(JwtClaimTypes.IdentityProvider, "local");

        var authTime = context.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.AuthenticationTime) ??
               new Claim(JwtClaimTypes.AuthenticationTime,
                   new DateTimeOffset(TimeProvider.System.GetUtcNow().UtcDateTime).ToUnixTimeSeconds().ToString(),
                   ClaimValueTypes.Integer64);

        var userPrincipal = await context.RequestServices.GetRequiredService<SignInManager<IdentityUser>>().CreateUserPrincipalAsync(user);
        if (userPrincipal.Identity is ClaimsIdentity claimsIdentity)
        {
            claimsIdentity.AddIfNotContains(idp);
            claimsIdentity.AddIfNotContains(authTime);
            foreach (var additionalClaim in additionalClaims)
            {
                claimsIdentity.AddIfNotContains(additionalClaim);
            }
        }

        var result = await context.RequestServices.GetRequiredService<IAuthorizeRequestValidator>().ValidateAsync(parameters, userPrincipal);
        if (!result.IsError)
        {
            var response = await context.RequestServices.GetRequiredService<IAuthorizeResponseGenerator>().CreateResponseAsync(result.ValidatedRequest);

            var authorizeResultType = typeof(IdentityServer4.Endpoints.Results.LoginPageResult).Assembly.GetType("IdentityServer4.Endpoints.Results.AuthorizeResult");
            if (authorizeResultType == null)
            {
                throw new AbpException("Unable get type of IdentityServer4.Endpoints.Results.AuthorizeResult!");
            }

            var endpointResult = Activator.CreateInstance(authorizeResultType, args: new object[] { response }).As<IEndpointResult>();
            if (endpointResult == null)
            {
                throw new AbpException($"Cannot instantiate type {authorizeResultType.FullName}");
            }

            await endpointResult.ExecuteAsync(context);
            return;
        }

        throw new AbpException(result.Error + Environment.NewLine + result.ErrorDescription);
    }
}
