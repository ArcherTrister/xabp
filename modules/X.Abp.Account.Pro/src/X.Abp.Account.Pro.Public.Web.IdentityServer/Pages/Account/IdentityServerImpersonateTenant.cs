// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(ImpersonateTenantModel))]
public class IdentityServerImpersonateTenantModel : ImpersonateTenantModel
{
    protected AbpAccountIdentityServerOptions Options { get; }

    public IdentityServerImpersonateTenantModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<AbpAccountIdentityServerOptions> options)
        : base(accountOptions, permissionChecker, currentPrincipalAccessor, signInManager, userManager, identitySecurityLogManager)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Query.TryGetValue("access_token", out var _))
        {
            return await base.OnGetAsync();
        }

        var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
        if (authenticateResult.Succeeded)
        {
            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                if (CurrentTenant.Id != null)
                {
                    throw new BusinessException("Volo.Account:ImpersonateTenantOnlyAvailableForHost");
                }

                if (AccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() ||
                    !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationTenantPermission))
                {
                    throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant")
                        .WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
                }

                var currentUserId = CurrentUser.Id;
                var currentUserName = CurrentUser.UserName;
                using (CurrentTenant.Change(TenantId))
                {
                    var adminUser = await UserManager.FindByNameAsync(AccountOptions.TenantAdminUserName);
                    if (adminUser != null)
                    {
                        try
                        {
                            await IdentityServerAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, adminUser, new[]
                            {
                                new Claim(AbpClaimTypes.ImpersonatorUserId, currentUserId.ToString()),
                                new Claim(AbpClaimTypes.ImpersonatorUserName, currentUserName)
                            });
                        }
                        catch (Exception e)
                        {
                            Logger.LogException(e);
                            throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant")
                                .WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
                        }

                        return new EmptyResult();
                    }

                    throw new BusinessException("Volo.Account:ThereIsNoUserWithUserName")
                        .WithData("UserName", AccountOptions.TenantAdminUserName);
                }
            }
        }

        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant")
            .WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
    }
}
