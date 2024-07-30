// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Dtos;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

[IgnoreAntiforgeryToken]
public class LinkLoginModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid SourceLinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? SourceLinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string SourceLinkToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid TargetLinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? TargetLinkTenantId { get; set; }

    public string LinkLoginDomain { get; set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected AbpAccountOptions AccountOptions { get; }

    protected ITenantStore TenantStore { get; }

    protected SignInManager<IdentityUser> SignInManager { get; }

    protected IdentityUserManager UserManager { get; }

    protected IIdentityLinkUserAppService IdentityLinkUserAppService { get; }

    public LinkLoginModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptions<AbpAccountOptions> accountOptions,
        ITenantStore tenantStore,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IIdentityLinkUserAppService identityLinkUserAppService)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        AccountOptions = accountOptions.Value;
        TenantStore = tenantStore;
        SignInManager = signInManager;
        UserManager = userManager;
        IdentityLinkUserAppService = identityLinkUserAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (AccountOptions.IsTenantMultiDomain && SourceLinkTenantId != TargetLinkTenantId && CurrentTenant.Id != TargetLinkTenantId)
        {
            var tenantInfo = new BasicTenantInfo(null, null);
            if (TargetLinkTenantId != null)
            {
                var tenantConfiguration = await TenantStore.FindAsync(TargetLinkTenantId.Value);
                tenantInfo = new BasicTenantInfo(tenantConfiguration.Id, tenantConfiguration.Name);
            }

            LinkLoginDomain = (await AccountOptions.GetTenantDomain(HttpContext, tenantInfo)).EnsureEndsWith('/') + "Account/LinkLogin";

            return Page();
        }

        if (TargetLinkUserId == CurrentUser.Id && TargetLinkTenantId == CurrentTenant.Id)
        {
            return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
        }

        if (await IdentityLinkUserAppService.VerifyLinkLoginTokenAsync(new VerifyLinkLoginTokenInput()
        {
            UserId = SourceLinkUserId,
            TenantId = SourceLinkTenantId,
            Token = SourceLinkToken
        }))
        {
            using (CurrentTenant.Change(SourceLinkTenantId))
            {
                var sourceUser = await UserManager.GetByIdAsync(SourceLinkUserId);
                using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(sourceUser)))
                {
                    if (await IdentityLinkUserAppService.IsLinkedAsync(new IsLinkedInput
                    {
                        UserId = TargetLinkUserId,
                        TenantId = TargetLinkTenantId
                    }))
                    {
                        var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
                        await SignInManager.SignOutAsync();
                        using (CurrentTenant.Change(TargetLinkTenantId))
                        {
                            var targetUser = await UserManager.GetByIdAsync(TargetLinkUserId);
                            await SignInManager.SignInAsync(targetUser, isPersistent);
                        }

                        return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
                    }
                }
            }
        }

        throw new UserFriendlyException(L["TheTargetAccountIsNotLinkedToYou"]);
    }
}
