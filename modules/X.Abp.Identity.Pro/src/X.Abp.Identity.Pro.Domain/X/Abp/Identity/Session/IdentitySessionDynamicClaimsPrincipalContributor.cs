// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Volo.Abp.AspNetCore.Authentication;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace X.Abp.Identity.Session;

public class IdentitySessionDynamicClaimsPrincipalContributor : AbpDynamicClaimsPrincipalContributorBase
{
    /// <summary>
    /// IAbpClaimsPrincipalFactory
    /// https://github.com/abpframework/abp/blob/905d4b5d26c9f84797ae166ef01aeb1c016d3dcf/framework/src/Volo.Abp.Security/Volo/Abp/Security/Claims/AbpClaimsPrincipalFactory.cs#L56
    /// </summary>
    public override async Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        ClaimsIdentity identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        if (identity != null)
        {
            Guid? userId = identity.FindUserId();
            if (userId.HasValue)
            {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<IdentitySessionDynamicClaimsPrincipalContributor>>();
                bool logout = false;
                string sessionId = identity.FindSessionId();
                if (sessionId.IsNullOrWhiteSpace())
                {
                    logger.LogWarning(message: "SessionId claim not found for user: {UserId}, log out.", userId);
                    logout = true;
                }
                else
                {
                    var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
                    var identitySessionChecker = context.ServiceProvider.GetRequiredService<IdentitySessionChecker>();

                    using (currentTenant.Change(identity.FindTenantId()))
                    {
                        if (!await identitySessionChecker.IsValidateAsync(sessionId))
                        {
                            logger.LogWarning(message: "SessionId({SessionId}) not found for user: {UserId}, log out.", sessionId, userId);
                            logout = true;
                        }
                    }
                }

                if (logout)
                {
                    await context.ServiceProvider.GetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>().ClearAsync(userId.Value);
                    context.ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                    logger.LogWarning("The token is no longer valid because the user's session expired.");
                    var tokenUnauthorizedErrorInfo = context.ServiceProvider.GetRequiredService<AbpAspNetCoreTokenUnauthorizedErrorInfo>();
                    tokenUnauthorizedErrorInfo.Error = "invalid_token";
                    tokenUnauthorizedErrorInfo.ErrorDescription = "SessionExpired";
                }
            }
        }
    }
}
