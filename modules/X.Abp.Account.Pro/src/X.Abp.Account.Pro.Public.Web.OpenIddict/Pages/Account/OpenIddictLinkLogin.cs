// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(LinkLoginModel))]
public class OpenIddictLinkLoginModel : LinkLoginModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictLinkLoginModel(
      ICurrentPrincipalAccessor currentPrincipalAccessor,
      IOptions<AbpAccountOptions> accountOptions,
      ITenantStore tenantStore,
      IOptions<AbpAccountOpenIddictOptions> options)
      : base(currentPrincipalAccessor, accountOptions, tenantStore)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnPostAsync()
    {
        if (Request.Query[OpenIddictConstants.Destinations.AccessToken].ToString().IsNullOrEmpty())
        {
            return await base.OnGetAsync();
        }

        AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(Options.LinkLoginAuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return await base.OnGetAsync();
        }

        using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
        {
            using (CurrentTenant.Change(authenticateResult.Principal.FindTenantId()))
            {
                return await base.OnGetAsync();
            }
        }
    }
}
