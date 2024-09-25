// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace X.Abp.Account.Public.Web.Pages.Account;

[Authorize]
[IgnoreAntiforgeryToken]
public class DelegatedImpersonateModel : AccountPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid UserDelegationId { get; set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IdentityUserDelegationManager IdentityUserDelegationManager { get; }

    public DelegatedImpersonateModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserDelegationManager identityUserDelegationManager)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        IdentityUserDelegationManager = identityUserDelegationManager;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (CurrentUser.FindImpersonatorUserId().HasValue)
        {
            throw new BusinessException("Volo.Account:NestedImpersonationIsNotAllowed");
        }

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
                    throw new BusinessException("Volo.Account:Volo.Account:ThereIsNoUserWithId")
                        .WithData("UserId", userDelegation.SourceUserId);
                }

                var isPersistent = ((await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent).GetValueOrDefault();
                await SignInManager.SignOutAsync();

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

                await SignInManager.SignInWithClaimsAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, additionalClaims);

                var claimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
                claimsPrincipal.Identities.First().AddClaims(additionalClaims);
                using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "DelegatedImpersonate"
                    });
                }

                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

                return Redirect("~/");
            }
        }

        throw new BusinessException("Volo.Account:InvalidUserDelegationId");
    }
}
