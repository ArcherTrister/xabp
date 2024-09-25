// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(DelegatedImpersonateModel))]
public class OpenIddictDelegatedImpersonateModel : DelegatedImpersonateModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictDelegatedImpersonateModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserDelegationManager identityUserDelegationManager,
        IOptions<AbpAccountOpenIddictOptions> options)
      : base(currentPrincipalAccessor, identityUserDelegationManager)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (Request.Query.TryGetValue(OpenIddictConstants.Destinations.AccessToken, out var _))
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
                IdentityUserDelegation userDelegation = await IdentityUserDelegationManager.FindActiveDelegationByIdAsync(UserDelegationId);

                if (userDelegation == null)
                {
                    throw new BusinessException("Volo.Account:InvalidUserDelegationId");
                }

                if (CurrentUser.Id.HasValue && userDelegation.TargetUserId == CurrentUser.Id.GetValueOrDefault())
                {
                    Guid sourceUserId = userDelegation.SourceUserId;

                    if (CurrentUser.Id.HasValue && (sourceUserId == CurrentUser.Id.GetValueOrDefault()))
                    {
                        throw new BusinessException("Volo.Account:YouCanNotImpersonateYourself");
                    }

                    IdentityUser user = await UserManager.FindByIdAsync(userDelegation.SourceUserId.ToString());
                    if (user == null)
                    {
                        throw new BusinessException("Volo.Account:Volo.Account:ThereIsNoUserWithId").WithData("UserId", userDelegation.SourceUserId);
                    }

                    List<Claim> claimList = new List<Claim>();

                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        claimList.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                        claimList.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            claimList.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.GetId().ToString()));
                        }
                    }

                    Claim claim = CurrentUser.FindClaim(AbpClaimTypes.RememberMe);
                    if (claim != null)
                    {
                        claimList.Add(claim);
                    }

                    try
                    {
                        return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, claimList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw new BusinessException("Volo.Account:ImpersonateError");
                    }
                }
            }
        }

        return await base.OnGetAsync();
    }
}
