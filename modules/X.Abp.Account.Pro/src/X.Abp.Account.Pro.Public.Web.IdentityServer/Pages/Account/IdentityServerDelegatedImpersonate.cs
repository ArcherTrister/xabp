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
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.Pages.Account;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(DelegatedImpersonateModel))]
public class IdentityServerDelegatedImpersonateModel : DelegatedImpersonateModel
{
    protected AbpAccountIdentityServerOptions Options { get; }

    public IdentityServerDelegatedImpersonateModel(
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserDelegationManager identityUserDelegationManager,
        IOptions<AbpAccountIdentityServerOptions> options)
      : base(signInManager, userManager, identitySecurityLogManager, currentPrincipalAccessor, identityUserDelegationManager)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (Request.Query.TryGetValue("access_token", out var _))
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                if (authenticateResult.Failure != null)
                {
                    Logger.LogException(authenticateResult.Failure);
                }

                throw new BusinessException("Volo.Account:ImpersonateError");
            }

            using (base.CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                var userDelegation = await IdentityUserDelegationManager.FindActiveDelegationByIdAsync(UserDelegationId);
                if (userDelegation != null)
                {
                    var targetUserId = userDelegation.TargetUserId;
                    var id = CurrentUser.Id;
                    if (!(targetUserId != id))
                    {
                        targetUserId = userDelegation.SourceUserId;
                        if (targetUserId == id)
                        {
                            throw new BusinessException("Volo.Account:YouCanNotImpersonateYourself");
                        }

                        var user = await UserManager.FindByIdAsync(userDelegation.SourceUserId.ToString());
                        if (user == null)
                        {
                            throw new BusinessException("Volo.Account:ThereIsNoUserWithUsernameInTheTenant")
                                .WithData("UserId", userDelegation.SourceUserId);
                        }

                        var additionalClaims = new List<Claim>();
                        if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                            if (CurrentTenant.IsAvailable)
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()));
                            }
                        }

                        try
                        {
                            await IdentityServerAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, additionalClaims.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            throw new BusinessException("Volo.Account:ImpersonateError");
                        }

                        return new EmptyResult();
                    }
                }

                throw new BusinessException("Volo.Account:InvalidUserDelegationId");
            }
        }

        return await base.OnGetAsync();
    }
}
