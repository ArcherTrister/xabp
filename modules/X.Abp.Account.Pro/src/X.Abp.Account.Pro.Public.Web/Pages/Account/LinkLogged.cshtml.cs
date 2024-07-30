// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account;

[Authorize]
public class LinkLoggedModel : AccountPageModel
{
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid TargetLinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? TargetLinkTenantId { get; set; }

    [HiddenInput]
    public string TargetLinkTenantName { get; set; }

    public string TargetLinkTenantAndUserName { get; set; }

    public bool IsSpaReturnUrl { get; set; }

    protected IdentityUserManager UserManager { get; }

    protected IIdentityLinkUserAppService IdentityLinkUserAppService { get; }

    public LinkLoggedModel(IdentityUserManager userManager, IIdentityLinkUserAppService identityLinkUserAppService)
    {
        UserManager = userManager;
        IdentityLinkUserAppService = identityLinkUserAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        if (!await IdentityLinkUserAppService.IsLinkedAsync(new IsLinkedInput
        {
            UserId = TargetLinkUserId,
            TenantId = TargetLinkTenantId
        }))
        {
            return await RedirectToLoginPageAsync();
        }

        using (CurrentTenant.Change(TargetLinkTenantId))
        {
            TenantConfiguration tenant = null;
            if (TargetLinkTenantId.HasValue)
            {
                var tenantStore = HttpContext.RequestServices.GetRequiredService<ITenantStore>();
                tenant = await tenantStore.FindAsync(TargetLinkTenantId.Value);
                TargetLinkTenantName = tenant.Name;
            }

            var user = await UserManager.FindByIdAsync(TargetLinkUserId.ToString());
            if (user == null)
            {
                return await RedirectToLoginPageAsync();
            }

            TargetLinkTenantAndUserName = tenant != null ? $"{tenant.Name}\\{user.UserName}" : user.UserName;
        }

        // TODO: Change handler=linkLogin to a special URL.
        IsSpaReturnUrl = !ReturnUrl.IsNullOrWhiteSpace() &&
                         ReturnUrl.Contains("handler=linkLogin", StringComparison.OrdinalIgnoreCase);

        return Page();
    }

    public virtual Task<string> GetReturnUrlAsync(string returnUrl, string returnUrlHash)
    {
        return base.GetRedirectUrlAsync(returnUrl, returnUrlHash);
    }

    public virtual string GetSpaReturnUrl(string returnUrl)
    {
        try
        {
            returnUrl = new Uri(returnUrl).GetLeftPart(UriPartial.Path);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
        }

        return returnUrl;
    }

    public virtual string GetSpaLinkLoginReturnUrl(string returnUrl)
    {
        try
        {
            returnUrl = $"{new Uri(returnUrl).GetLeftPart(UriPartial.Path).RemovePostFix("/")}?handler=linkLogin&linkUserId={CurrentUser.Id.Value:D}";
            if (CurrentTenant.Id.HasValue)
            {
                returnUrl += $"&linkTenantId={CurrentTenant.Id.Value:D}";
            }
        }
        catch (Exception e)
        {
            Logger.LogException(e);
        }

        return returnUrl;
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    protected virtual Task<IActionResult> RedirectToLoginPageAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("./Login", new
        {
            ReturnUrl,
            ReturnUrlHash
        }));
    }
}
