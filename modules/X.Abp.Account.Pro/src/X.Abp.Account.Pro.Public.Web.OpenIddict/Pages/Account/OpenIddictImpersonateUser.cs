// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(ImpersonateUserModel))]
public class OpenIddictImpersonateUserModel : ImpersonateUserModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictImpersonateUserModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        ITenantStore tenantStore,
        IOptions<AbpAccountOpenIddictOptions> options)
        : base(accountOptions, permissionChecker, currentPrincipalAccessor, tenantStore)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (Request.Query.TryGetValue(OpenIddictConstants.Destinations.AccessToken, out var _))
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
            if (authenticateResult.Succeeded)
            {
                using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
                {
                    if (UserId == CurrentUser.Id)
                    {
                        throw new BusinessException("Volo.Account:YouCanNotImpersonateYourself");
                    }

                    if (!AccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace() && !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationUserPermission))
                    {
                        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser").WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                    }

                    var user = await UserManager.FindByIdAsync(UserId.ToString());
                    if (user == null)
                    {
                        throw new BusinessException("Volo.Account:ThereIsNoUserWithUsernameInTheTenant").WithData("UserId", UserId);
                    }

                    var additionalClaims = new List<Claim>();
                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()!));
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()!));
                            var tenantConfiguration = await HttpContext.RequestServices.GetRequiredService<ITenantStore>().FindAsync(CurrentTenant.Id!.Value);
                            if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                            }
                        }
                    }

                    Claim claim = CurrentUser.FindClaim(AbpClaimTypes.RememberMe);
                    if (claim != null)
                    {
                        additionalClaims.Add(claim);
                    }

                    try
                    {
                        return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, additionalClaims.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser").WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                    }
                }
            }

            throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser").WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
        }

        return await base.OnGetAsync();
    }
}
