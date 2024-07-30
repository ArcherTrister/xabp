// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.Security.Claims;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.ExtensionGrantValidators;

public class SpaExternalLoginExtensionGrantValidator : IExtensionGrantValidator
{
    public const string ExtensionGrantType = "SpaExternalLogin";

    public string GrantType => ExtensionGrantType;

    protected ICurrentTenant CurrentTenant { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected ILogger<SpaExternalLoginExtensionGrantValidator> Logger { get; }

    protected IStringLocalizer<AbpIdentityServerResource> IdentityServerLocalizer { get; }

    protected IUserClaimsPrincipalFactory<IdentityUser> ClaimsFactory { get; }

    public SpaExternalLoginExtensionGrantValidator(
        ICurrentTenant currentTenant,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        ILogger<SpaExternalLoginExtensionGrantValidator> logger,
        IStringLocalizer<AbpIdentityServerResource> identityServerLocalizer,
        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory)
    {
        CurrentTenant = currentTenant;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        UserManager = userManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
        Logger = logger;
        IdentityServerLocalizer = identityServerLocalizer;
        ClaimsFactory = claimsFactory;
    }

    public virtual async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var loginProvider = context.Request.Raw.Get(ExternalLoginConsts.SpaExternalLoginProviderName);
        if (string.IsNullOrWhiteSpace(loginProvider))
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant)
            {
                ErrorDescription = IdentityServerLocalizer[ExternalLoginErrorCodes.ProviderNameCannotBeEmpty],
                Error = ExternalLoginErrorCodes.ProviderNameCannotBeEmpty,
                IsError = true,
            };
            return;
        }

        // providerKey
        var providerKey = context.Request.Raw.Get(ExternalLoginConsts.SpaExternalLoginProviderKey);
        if (string.IsNullOrWhiteSpace(providerKey))
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant)
            {
                ErrorDescription = IdentityServerLocalizer[ExternalLoginErrorCodes.ProviderKeyCannotBeEmpty],
                Error = ExternalLoginErrorCodes.ProviderKeyCannotBeEmpty,
                IsError = true,
            };
            return;
        }

        Guid? spaExternalLoginTenantId = null;
        var tenantId = context.Request.Raw.Get(ExternalLoginConsts.SpaExternalLoginTenantId);
        if (!tenantId.IsNullOrWhiteSpace())
        {
            if (!Guid.TryParse(tenantId, out var parsedGuid))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "invalid_spa_external_login_tenant_id"
                };
                return;
            }

            Logger.LogInformation($"SpaExternalLoginTenantId:{parsedGuid}");
            spaExternalLoginTenantId = parsedGuid;
        }

        using (CurrentTenant.Change(spaExternalLoginTenantId))
        {
            var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            if (user != null)
            {
                var additionalClaims = new List<Claim>()
                {
                    new Claim(CustomClaimTypes.ProviderKey, providerKey)
                };

                if (user.TenantId.HasValue)
                {
                    additionalClaims.Add(new Claim(AbpClaimTypes.TenantId, user.TenantId?.ToString()));
                }

                context.Result = new GrantValidationResult(
                    user.Id.ToString(),
                    ExtensionGrantType,
                    additionalClaims.ToArray(),
                    loginProvider);

                // save security log to user.
                var userPrincipal = await ClaimsFactory.CreateAsync(user!);
                userPrincipal.Identities.First().AddClaims(additionalClaims);
                using (CurrentPrincipalAccessor.Change(userPrincipal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "SpaExternalLoginUser"
                    });
                }
            }
            else
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = IdentityServerLocalizer["X.Abp.IdentityServer.External:010014"].Value
                    .Replace("{LoginProvider}", loginProvider, StringComparison.OrdinalIgnoreCase)
                };
            }
        }
    }
}
