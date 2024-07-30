// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Authentication;

namespace X.Abp.Account.Public.Web.Impersonation;

public abstract class AbpAccountImpersonationChallengeAccountController : ChallengeAccountController
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> ImpersonateTenantAsync(Guid tenantId, string tenantUserName, string returnUrl)
    {
        if (CurrentUser.IsAuthenticated)
        {
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
            {
                { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation },
                { AbpAccountImpersonationConsts.TenantId, tenantId.ToString() },
                { AbpAccountImpersonationConsts.TenantUserName, tenantUserName },
                {
                    AbpAccountImpersonationConsts.ReturnUrl,
                    configuration["App:SelfUrl"] + returnUrl.EnsureStartsWith('/', StringComparison.Ordinal)
                }
            })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }

    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> ImpersonateUserAsync(Guid userId)
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
            {
                { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation },
                { AbpAccountImpersonationConsts.UserId, userId.ToString() }
            })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }

    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> BackToImpersonatorAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
            {
                { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation }
            })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }

    [IgnoreAntiforgeryToken]
    [Authorize]
    public virtual Task<IActionResult> DelegatedImpersonateAsync(Guid userDelegationId)
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
            {
                { AbpAccountImpersonationConsts.DelegatedImpersonate, AbpAccountImpersonationConsts.DelegatedImpersonate },
                { AbpAccountImpersonationConsts.UserDelegationId, userDelegationId.ToString() },
            })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }
}
