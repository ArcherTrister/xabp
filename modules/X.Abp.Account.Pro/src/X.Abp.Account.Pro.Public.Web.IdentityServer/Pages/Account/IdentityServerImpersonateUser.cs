// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
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
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(ImpersonateUserModel))]
public class IdentityServerImpersonateUserModel : ImpersonateUserModel
{
    protected AbpAccountIdentityServerOptions Options { get; }

    public IdentityServerImpersonateUserModel(
        IOptions<AbpAccountIdentityServerOptions> options,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        ITenantStore tenantStore,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<AbpAccountOptions> accountOptions)
        : base(accountOptions, permissionChecker, currentPrincipalAccessor, tenantStore, signInManager, userManager, identitySecurityLogManager)
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
                if (UserId == CurrentUser.Id)
                {
                    throw new BusinessException("Volo.Account:YouCanNotImpersonateYourself");
                }

                if (AccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace() ||
                    !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationUserPermission))
                {
                    throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
                        .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                }

                var user = await UserManager.FindByIdAsync(UserId.ToString());
                if (user != null)
                {
                    var additionalClaims = new List<Claim>();
                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()));
                            var tenantConfiguration = await TenantStore.FindAsync(CurrentTenant.Id.Value);
                            if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                            }
                        }
                    }

                    try
                    {
                        await IdentityServerAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, additionalClaims.ToArray());
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
                            .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                    }

                    return new EmptyResult();
                }

                throw new BusinessException("Volo.Account:ThereIsNoUserWithUsernameInTheTenant")
                    .WithData("UserId", UserId);
            }
        }

        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
            .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
    }
}
