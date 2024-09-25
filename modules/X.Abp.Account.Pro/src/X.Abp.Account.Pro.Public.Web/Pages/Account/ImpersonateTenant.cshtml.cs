// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

[Authorize]
[IgnoreAntiforgeryToken]
public class ImpersonateTenantModel : AccountPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid TenantId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string TenantUserName { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    protected AbpAccountOptions AccountOptions { get; }

    protected IPermissionChecker PermissionChecker { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    public ImpersonateTenantModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        AccountOptions = accountOptions.Value;
        PermissionChecker = permissionChecker;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (ReturnUrl != null && !Url.IsLocalUrl(ReturnUrl) && !ReturnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/"), StringComparison.InvariantCultureIgnoreCase) && !await AppUrlProvider.IsRedirectAllowedUrlAsync(ReturnUrl))
            {
                ReturnUrl = null;
            }

            if (CurrentUser.FindImpersonatorUserId().HasValue)
            {
                throw new BusinessException("Volo.Account:NestedImpersonationIsNotAllowed");
            }

            if (CurrentTenant.Id.HasValue)
            {
                throw new BusinessException("Volo.Account:ImpersonateTenantOnlyAvailableForHost");
            }

            if (AccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() ||
                !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationTenantPermission))
            {
                throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant")
                    .WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
            }

            using (CurrentTenant.Change(TenantId))
            {
                if (TenantUserName.IsNullOrWhiteSpace())
                {
                    TenantUserName = AccountOptions.TenantAdminUserName;
                }

                IdentityUser adminUser = await UserManager.FindByNameAsync(TenantUserName);
                if (adminUser == null)
                {
                    throw new BusinessException("Volo.Account:ThereIsNoUserWithUserName").WithData("UserName", AccountOptions.TenantAdminUserName);
                }

                bool isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
                await SignInManager.SignOutAsync();
                List<Claim> additionalClaims = new List<Claim>()
          {
            new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()),
            new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName)
          };
                Claim claim = CurrentUser.FindClaim(AbpClaimTypes.RememberMe);
                if (claim != null)
                {
                    additionalClaims.Add(claim);
                }

                AuthenticationProperties authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = isPersistent
                };

                await SignInManager.SignInWithClaimsAsync(adminUser, authenticationProperties, additionalClaims);
                ClaimsPrincipal principal = await SignInManager.CreateUserPrincipalAsync(adminUser);
                principal.Identities.First().AddClaims(additionalClaims);
                using (CurrentPrincipalAccessor.Change(principal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "ImpersonateUser"
                    });
                }

                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(adminUser.Id, adminUser.TenantId);

                return Redirect("~/");
            }
        }
        catch (BusinessException ex)
        {
            Alerts.Danger(ExceptionToErrorInfoConverter.Convert(ex).Message);
            return Page();
        }
    }
}
