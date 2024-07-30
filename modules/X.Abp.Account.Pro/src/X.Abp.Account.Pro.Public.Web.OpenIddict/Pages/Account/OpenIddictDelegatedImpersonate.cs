// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

[ExposeServices(typeof(DelegatedImpersonateModel))]
public class OpenIddictDelegatedImpersonateModel : DelegatedImpersonateModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictDelegatedImpersonateModel(
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserDelegationManager identityUserDelegationManager,
        IOptions<AbpAccountOpenIddictOptions> options)
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

            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
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
                            throw new BusinessException("Volo.Account:Volo.Account:ThereIsNoUserWithId").WithData("UserId", userDelegation.SourceUserId);
                        }

                        var additionalClaims = new List<Claim>();
                        if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                            if (CurrentTenant.IsAvailable)
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.GetId().ToString()));
                            }
                        }

                        try
                        {
                            return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, additionalClaims.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            throw new BusinessException("Volo.Account:ImpersonateError");
                        }
                    }
                }

                throw new BusinessException("Volo.Account:InvalidUserDelegationId");
            }
        }

        return await base.OnGetAsync();
    }
}
