// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(BackToImpersonatorModel))]
public class OpenIddictBackToImpersonatorModel : BackToImpersonatorModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictBackToImpersonatorModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptions<AbpAccountOpenIddictOptions> options)
        : base(currentPrincipalAccessor)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Query.TryGetValue(OpenIddictConstants.Destinations.AccessToken, out var _))
        {
            return await base.OnGetAsync();
        }

        var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
        if (authenticateResult.Succeeded)
        {
            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                var impersonatorTenantId = CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId();
                var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();
                if ((impersonatorTenantId.HasValue || impersonatorUserId.HasValue) && impersonatorUserId.HasValue)
                {
                    using (CurrentTenant.Change(impersonatorTenantId, null))
                    {
                        var user = await UserManager.GetByIdAsync(impersonatorUserId.Value);
                        try
                        {
                            return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            return Unauthorized();
                        }
                    }
                }

                return Unauthorized();
            }
        }

        return Unauthorized();
    }
}
