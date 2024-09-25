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

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.ExtensionGrantValidators;

public class ScanCodeLoginExtensionGrantValidator : IExtensionGrantValidator
{
    public const string ExtensionGrantType = "ScanCodeLogin";

    public string GrantType => ExtensionGrantType;

    protected ICurrentTenant CurrentTenant { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected ILogger<ScanCodeLoginExtensionGrantValidator> Logger { get; }

    protected IStringLocalizer<AbpIdentityServerResource> IdentityServerLocalizer { get; }

    protected IUserClaimsPrincipalFactory<IdentityUser> ClaimsFactory { get; }

    public ScanCodeLoginExtensionGrantValidator(
        ICurrentTenant currentTenant,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        ILogger<ScanCodeLoginExtensionGrantValidator> logger,
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
        var loginProvider = context.Request.Raw.Get(ExternalLoginConsts.ScanCodeLoginUserId);
        if (string.IsNullOrWhiteSpace(loginProvider))
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant)
            {
                ErrorDescription = IdentityServerLocalizer[ExternalLoginErrorCodes.UserDoesNotExist],
                Error = ExternalLoginErrorCodes.UserDoesNotExist,
                IsError = true,
            };
            return;
        }

        Guid? scanCodeLoginTenantId = null;
        var tenantId = context.Request.Raw.Get(ExternalLoginConsts.ScanCodeLoginTenantId);
        if (!tenantId.IsNullOrWhiteSpace())
        {
            if (!Guid.TryParse(tenantId, out var parsedGuid))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "invalid_spa_external_tenant_id"
                };
                return;
            }

            Logger.LogInformation("SpaExternalTenantId:{SpaExternalTenantId}", parsedGuid);
            scanCodeLoginTenantId = parsedGuid;
        }

        using (CurrentTenant.Change(scanCodeLoginTenantId))
        {
            var user = await UserManager.FindByIdAsync(loginProvider);
            if (user != null)
            {
                var additionalClaims = new List<Claim>();

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
                        Action = "ScanCodeLoginUser"
                    });
                }
            }
            else
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = ExternalLoginErrorCodes.UserDoesNotExist,
                    ErrorDescription = IdentityServerLocalizer[ExternalLoginErrorCodes.UserDoesNotExist],
                };
            }
        }
    }
}
