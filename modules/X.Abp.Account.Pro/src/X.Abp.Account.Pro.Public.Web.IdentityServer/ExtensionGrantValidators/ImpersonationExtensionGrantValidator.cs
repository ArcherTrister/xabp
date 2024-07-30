// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using IdentityServer4.Validation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.ExtensionGrantValidators;

public class ImpersonationExtensionGrantValidator : IExtensionGrantValidator
{
    public const string ExtensionGrantType = "Impersonation";

    public string GrantType => ExtensionGrantType;

    protected ITokenValidator TokenValidator { get; }

    protected IPermissionChecker PermissionChecker { get; }

    protected ICurrentTenant CurrentTenant { get; }

    protected ICurrentUser CurrentUser { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected ILogger<ImpersonationExtensionGrantValidator> Logger { get; }

    protected IStringLocalizer<AccountResource> Localizer { get; }

    protected AbpAccountOptions AbpAccountOptions { get; }

    protected IUserClaimsPrincipalFactory<IdentityUser> ClaimsFactory { get; }

    protected ITenantStore TenantStore { get; }

    public ImpersonationExtensionGrantValidator(
        ITokenValidator tokenValidator,
        IPermissionChecker permissionChecker,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IdentityUserManager userManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentitySecurityLogManager identitySecurityLogManager,
        ILogger<ImpersonationExtensionGrantValidator> logger,
        IStringLocalizer<AccountResource> localizer,
        IOptions<AbpAccountOptions> abpAccountOptions,
        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
        ITenantStore tenantStore)
    {
        TokenValidator = tokenValidator;
        PermissionChecker = permissionChecker;
        CurrentTenant = currentTenant;
        CurrentUser = currentUser;
        UserManager = userManager;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        IdentitySecurityLogManager = identitySecurityLogManager;
        Logger = logger;
        Localizer = localizer;
        ClaimsFactory = claimsFactory;
        TenantStore = tenantStore;
        AbpAccountOptions = abpAccountOptions.Value;
    }

    public virtual async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var accessToken = context.Request.Raw["access_token"];
        if (accessToken.IsNullOrWhiteSpace())
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = Localizer["Volo.Account:InvalidAccessToken"]
            };
            return;
        }

        var result = await TokenValidator.ValidateAccessTokenAsync(accessToken);
        if (result.IsError)
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = result.Error,
                ErrorDescription = result.ErrorDescription
            };
            return;
        }

        using (CurrentPrincipalAccessor.Change(result.Claims))
        {
            var tenantId = await GetRawValueOrNullAsync(context, "TenantId");
            var userId = await GetRawValueOrNullAsync(context, "UserId");

            var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();

            if (userId == null && tenantId == null && impersonatorUserId != null)
            {
                await BackToImpersonatorAsync(
                    context,
                    CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId(),
                    impersonatorUserId.Value);
                return;
            }
            else
            {
                if (impersonatorUserId != null)
                {
                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = Localizer["Volo.Account:NestedImpersonationIsNotAllowed"]
                    };
                    return;
                }

                if (CurrentTenant.IsAvailable)
                {
                    // Tenant
                    if (userId != null)
                    {
                        await ImpersonateUserAsync(context, CurrentTenant.Id, userId.Value);
                        return;
                    }
                }
                else
                {
                    // Host
                    if (userId == null && tenantId != null)
                    {
                        await ImpersonateTenantAsync(context, tenantId.Value);
                        return;
                    }

                    if (userId != null && tenantId == null)
                    {
                        await ImpersonateUserAsync(context, null, userId.Value);
                        return;
                    }
                }
            }

            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = Localizer["Volo.Account:InvalidTenantIdOrUserId"]
            };
        }
    }

    protected virtual async Task ImpersonateTenantAsync(ExtensionGrantValidationContext context, Guid tenantId)
    {
        if (AbpAccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() ||
            await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationTenantPermission))
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByNameAsync(AbpAccountOptions.TenantAdminUserName);
                if (user != null)
                {
                    var sub = await UserManager.GetUserIdAsync(user);

                    var additionalClaims = new List<Claim>()
                    {
                        new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()),
                        new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName)
                    };

                    await AddCustomClaimsAsync(additionalClaims, user, context);

                    context.Result = new GrantValidationResult(
                        sub,
                        GrantType,
                        additionalClaims.ToArray());

                    // save security log to admin user.
                    var userPrincipal = await ClaimsFactory.CreateAsync(user);
                    userPrincipal.Identities.First().AddClaims(additionalClaims);
                    using (CurrentPrincipalAccessor.Change(userPrincipal))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = "ImpersonateUser"
                        });
                    }
                }
                else
                {
                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = Localizer["Volo.Account:ThereIsNoUserWithUserName"].Value
                            .Replace("{UserName}", AbpAccountOptions.TenantAdminUserName, StringComparison.OrdinalIgnoreCase)
                    };
                }
            }
        }
        else
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = Localizer["Volo.Account:RequirePermissionToImpersonateTenant"].Value
                    .Replace("{PermissionName}", AbpAccountOptions.ImpersonationTenantPermission, StringComparison.OrdinalIgnoreCase)
            };
        }
    }

    protected virtual async Task ImpersonateUserAsync(ExtensionGrantValidationContext context, Guid? tenantId, Guid userId)
    {
        if (userId == CurrentUser.Id)
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = Localizer["Volo.Account:YouCanNotImpersonateYourself"]
            };
            return;
        }

        if (AbpAccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace() ||
            await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationUserPermission))
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    var sub = await UserManager.GetUserIdAsync(user);

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

                    await AddCustomClaimsAsync(additionalClaims, user, context);

                    context.Result = new GrantValidationResult(
                        sub,
                        GrantType,
                        additionalClaims.ToArray());

                    // save security log to user.
                    var userPrincipal = await ClaimsFactory.CreateAsync(user);
                    userPrincipal.Identities.First().AddClaims(additionalClaims);
                    using (CurrentPrincipalAccessor.Change(userPrincipal))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = "ImpersonateUser"
                        });
                    }
                }
                else
                {
                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = Localizer["Volo.Account:ThereIsNoUserWithId"].Value
                        .Replace("{UserId}", userId.ToString(), StringComparison.OrdinalIgnoreCase)
                    };
                }
            }
        }
        else
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = Localizer["Volo.Account:RequirePermissionToImpersonateUser"].Value
                .Replace("{PermissionName}", AbpAccountOptions.ImpersonationUserPermission, StringComparison.OrdinalIgnoreCase)
            };
        }
    }

    protected virtual async Task BackToImpersonatorAsync(ExtensionGrantValidationContext context, Guid? tenantId, Guid userId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var sub = await UserManager.GetUserIdAsync(user);
                var additionalClaims = new List<Claim>();

                await AddCustomClaimsAsync(additionalClaims, user, context);

                context.Result = new GrantValidationResult(
                    sub,
                    GrantType,
                    additionalClaims.ToArray());
            }
            else
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = Localizer["Volo.Account:InvalidAccessToken"]
                };
            }
        }
    }

    protected virtual Task<Guid?> GetRawValueOrNullAsync(ExtensionGrantValidationContext context, string key)
    {
        var str = context.Request.Raw[key];
        return str.IsNullOrWhiteSpace()
            ? Task.FromResult<Guid?>(null)
            : Guid.TryParse(str, out var guid) ? Task.FromResult<Guid?>(guid) : Task.FromResult<Guid?>(null);
    }

    protected virtual Task AddCustomClaimsAsync(ICollection<Claim> customClaims, IdentityUser user, ExtensionGrantValidationContext context)
    {
        if (user.TenantId.HasValue)
        {
            customClaims.Add(new Claim(AbpClaimTypes.TenantId, user.TenantId?.ToString()));
        }

        return Task.CompletedTask;
    }
}
