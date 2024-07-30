// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;

using X.Abp.Account.Localization;

using static OpenIddict.Server.OpenIddictServerEvents;

using IdentityUser = Volo.Abp.Identity.IdentityUser;
using OpenIddictExtensions = OpenIddict.Abstractions.OpenIddictExtensions;

namespace X.Abp.Account.Public.Web.ExtensionGrants;

public class ImpersonationExtensionGrant : TokenExtensionGrantBase
{
    public const string ExtensionGrantName = "Impersonation";

    public override string Name => ExtensionGrantName;

    protected IPermissionChecker PermissionChecker { get; set; }

    protected ICurrentTenant CurrentTenant { get; set; }

    protected ICurrentUser CurrentUser { get; set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }

    protected IdentityUserManager UserManager { get; set; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; set; }

    protected ILogger<ImpersonationExtensionGrant> Logger { get; set; }

    protected AbpAccountOptions AbpAccountOptions { get; set; }

    protected IUserClaimsPrincipalFactory<IdentityUser> UserClaimsPrincipalFactory { get; set; }

    protected IStringLocalizer<AccountResource> Localizer { get; set; }

    [UnitOfWork]
    public override async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        PermissionChecker = context.HttpContext.RequestServices.GetRequiredService<IPermissionChecker>();
        CurrentTenant = context.HttpContext.RequestServices.GetRequiredService<ICurrentTenant>();
        CurrentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        CurrentPrincipalAccessor = context.HttpContext.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
        UserManager = context.HttpContext.RequestServices.GetRequiredService<IdentityUserManager>();
        IdentitySecurityLogManager = context.HttpContext.RequestServices.GetRequiredService<IdentitySecurityLogManager>();
        Logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ImpersonationExtensionGrant>>();
        AbpAccountOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AbpAccountOptions>>().Value;
        UserClaimsPrincipalFactory = context.HttpContext.RequestServices.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();
        Localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AccountResource>>();

        var transaction = context.HttpContext.GetOpenIddictServerTransaction();
        Check.NotNull(transaction, nameof(transaction));
        Check.NotNull(transaction.Request, nameof(transaction.Request));

        transaction.EndpointType = OpenIddictServerEndpointType.Introspection;
        transaction.Request = new OpenIddictRequest
        {
            ClientId = context.Request.ClientId,
            ClientSecret = context.Request.ClientSecret,
            Token = context.Request.AccessToken
        };

        var notification = new ProcessAuthenticationContext(transaction);
        var dispatcher = context.HttpContext.RequestServices.GetRequiredService<IOpenIddictServerDispatcher>();
        await dispatcher.DispatchAsync(notification);

        if (notification.IsRejected)
        {
            Logger.LogError("Process authentication rejected");
            return new ForbidResult(new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme },
                new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = notification.Error ?? OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = notification.ErrorDescription,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorUri] = notification.ErrorUri
                }));
        }

        var principal = notification.GenericTokenPrincipal;
        if (principal != null)
        {
            var request = OpenIddictServerAspNetCoreHelpers.GetOpenIddictServerRequest(context.HttpContext);
            using (CurrentPrincipalAccessor.Change(principal.Claims))
            {
                var tenantId = await GetRawValueOrNullAsync(request, "TenantId");
                var userId = await GetRawValueOrNullAsync(request, "UserId");
                var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();
                if (userId.HasValue || tenantId.HasValue || !impersonatorUserId.HasValue)
                {
                    if (impersonatorUserId.HasValue)
                    {
                        return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:NestedImpersonationIsNotAllowed"]
                        }));
                    }

                    if (CurrentTenant.IsAvailable)
                    {
                        if (userId.HasValue)
                        {
                            return await ImpersonateUserAsync(context, principal, CurrentTenant.Id, userId.Value);
                        }
                    }
                    else
                    {
                        if (!userId.HasValue && tenantId.HasValue)
                        {
                            return await ImpersonateTenantAsync(context, principal, tenantId.Value);
                        }

                        if (userId.HasValue && !tenantId.HasValue)
                        {
                            return await ImpersonateUserAsync(context, principal, null, userId.Value);
                        }
                    }

                    return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidTenantIdOrUserId"]
                    }));
                }

                return await BackToImpersonatorAsync(context, principal, CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId(), impersonatorUserId.Value);
            }
        }

        Logger.LogError("Process authentication principal is null");
        return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidAccessToken"]
        }));
    }

    protected virtual async Task<IActionResult> ImpersonateTenantAsync(ExtensionGrantContext context, ClaimsPrincipal principal, Guid tenantId)
    {
        if (AbpAccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() ||
            await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationTenantPermission))
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByNameAsync(AbpAccountOptions.TenantAdminUserName);
                if (user == null)
                {
                    Logger.LogError(Localizer["Volo.Account:ThereIsNoUserWithUserName"].Value.Replace("{UserName}", AbpAccountOptions.TenantAdminUserName, StringComparison.Ordinal));
                    return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:ThereIsNoUserWithUserName"].Value.Replace("{UserName}", AbpAccountOptions.TenantAdminUserName, StringComparison.Ordinal)
                    }));
                }

                var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                List<Claim> claims = new()
                {
                    new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()!),
                    new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName)
                };
                claimsPrincipal.Identities.First().AddClaims(claims);
                using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "ImpersonateUser"
                    });
                }

                claimsPrincipal.SetScopes(principal.GetScopes());
                claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));
                await SetClaimsDestinationsAsync(context, claimsPrincipal);
                return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
            }
        }

        Logger.LogError(Localizer["Volo.Account:RequirePermissionToImpersonateTenant"].Value.Replace("{PermissionName}", AbpAccountOptions.ImpersonationTenantPermission, StringComparison.Ordinal));
        return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string> { [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest, [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid request." }));
    }

    protected virtual async Task<IActionResult> ImpersonateUserAsync(ExtensionGrantContext context, ClaimsPrincipal principal, Guid? tenantId, Guid userId)
    {
        if (userId == CurrentUser.Id)
        {
            Logger.LogError(Localizer["Volo.Account:YouCanNotImpersonateYourself"]);
            return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:YouCanNotImpersonateYourself"]
            }));
        }

        if (AbpAccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace() ||
            await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationUserPermission))
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    Logger.LogError(Localizer["Volo.Account:ThereIsNoUserWithId"].Value.Replace("{UserId}", userId.ToString(), StringComparison.Ordinal));
                    return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:ThereIsNoUserWithId"].Value.Replace("{UserId}", userId.ToString(), StringComparison.Ordinal)
                    }));
                }

                var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                List<Claim> additionalClaims = new();
                if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                {
                    additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()!));
                    additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                    if (CurrentTenant.IsAvailable)
                    {
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()!));
                        var tenantConfiguration = await context.HttpContext.RequestServices.GetRequiredService<ITenantStore>().FindAsync(CurrentTenant.Id.Value);
                        if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                        }
                    }
                }

                claimsPrincipal.Identities.First().AddClaims(additionalClaims);
                using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "ImpersonateUser"
                    });
                }

                claimsPrincipal.SetScopes(OpenIddictExtensions.GetScopes(principal));
                claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));
                await SetClaimsDestinationsAsync(context, claimsPrincipal);
                return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
            }
        }

        Logger.LogError(Localizer["Volo.Account:RequirePermissionToImpersonateUser"].Value.Replace("{PermissionName}", AbpAccountOptions.ImpersonationUserPermission, StringComparison.Ordinal));
        return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:RequirePermissionToImpersonateUser"].Value.Replace("{PermissionName}", AbpAccountOptions.ImpersonationUserPermission, StringComparison.Ordinal)
        }));
    }

    protected virtual async Task<IActionResult> BackToImpersonatorAsync(ExtensionGrantContext context, ClaimsPrincipal principal, Guid? tenantId, Guid userId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ForbidResult(new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme }, new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidAccessToken"]
                }));
            }

            var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
            claimsPrincipal.SetScopes(principal.GetScopes());
            claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));
            await SetClaimsDestinationsAsync(context, claimsPrincipal);
            return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}
