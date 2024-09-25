// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

using OpenIddict.Abstractions;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace X.Abp.OpenIddict;

public class OpenIddictClaimsPrincipalContributor : IAbpClaimsPrincipalContributor, ITransientDependency
{
  public virtual Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
  {
    var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
    if (identity != null)
    {
      var options = context.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;
      var usernameClaim = identity.FindFirst(options.ClaimsIdentity.UserNameClaimType);
      if (usernameClaim != null)
      {
        identity.AddIfNotContains(new Claim(OpenIddictConstants.Claims.PreferredUsername, usernameClaim.Value));
        identity.AddIfNotContains(new Claim(JwtRegisteredClaimNames.UniqueName, usernameClaim.Value));
      }

      var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
      if (httpContext != null)
      {
        var clientId = httpContext.GetOpenIddictServerRequest()?.ClientId;
        if (clientId != null)
        {
          identity.AddClaim(OpenIddictConstants.Claims.ClientId, clientId);
        }
      }
    }

    return Task.CompletedTask;
  }
}
