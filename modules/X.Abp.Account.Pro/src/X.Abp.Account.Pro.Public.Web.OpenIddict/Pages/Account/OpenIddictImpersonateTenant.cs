// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(ImpersonateTenantModel))]
public class OpenIddictImpersonateTenantModel : ImpersonateTenantModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictImpersonateTenantModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptions<AbpAccountOpenIddictOptions> options)
        : base(accountOptions, permissionChecker, currentPrincipalAccessor)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Query.ContainsKey(OpenIddictConstants.Destinations.AccessToken))
        {
            // ISSUE: reference to a compiler-generated method
            return await base.OnGetAsync();
        }

        if (TenantUserName.IsNullOrWhiteSpace())
        {
            TenantUserName = AccountOptions.TenantAdminUserName;
        }

        if (ReturnUrl != null && !Url.IsLocalUrl(ReturnUrl) && !ReturnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/"), StringComparison.InvariantCultureIgnoreCase) && !await AppUrlProvider.IsRedirectAllowedUrlAsync(ReturnUrl))
        {
            ReturnUrl = null;
        }

        try
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant").WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
            }

            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                if (CurrentTenant.Id.HasValue)
                {
                    throw new BusinessException("Volo.Account:ImpersonateTenantOnlyAvailableForHost");
                }

                if (!AccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() && !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationTenantPermission))
                {
                    throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant").WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
                }

                using (CurrentTenant.Change(TenantId))
                {
                    IdentityUser user = await UserManager.FindByNameAsync(TenantUserName) ?? throw new BusinessException("Volo.Account:ThereIsNoUserWithUserName").WithData("UserName", TenantUserName);
                    try
                    {
                        return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()), new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant").WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
                    }
                }
            }
        }
        catch (BusinessException ex)
        {
            Alerts.Danger(ExceptionToErrorInfoConverter.Convert(ex).Message);
            return Page();
        }
    }
}
