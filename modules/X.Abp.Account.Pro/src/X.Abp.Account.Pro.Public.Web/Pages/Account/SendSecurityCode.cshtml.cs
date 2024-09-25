// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.Uow;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class SendSecurityCodeModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    public IReadOnlyList<SelectListItem> Providers { get; set; }

    [BindProperty]
    public string SelectedProvider { get; set; }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToPage("./Login");
        }

        // TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???
        CheckCurrentTenant(user.TenantId);

        Providers = (await AccountAppService.GetTwoFactorProvidersAsync(new GetTwoFactorProvidersInput
        {
            UserId = user.Id,
            Token = await UserManager.GenerateUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                nameof(Microsoft.AspNetCore.Identity.SignInResult.RequiresTwoFactor))
        })).Select(userProvider => new SelectListItem
        {
            Text = userProvider,
            Value = userProvider
        }).ToList();

        return Page();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // TODO: CheckCurrentTenant
        // CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???
        CheckCurrentTenant(user.TenantId);

        await AccountAppService.SendTwoFactorCodeAsync(new SendTwoFactorCodeInput()
        {
            UserId = user.Id,
            Provider = SelectedProvider,
            Token = await UserManager.GenerateUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                nameof(Microsoft.AspNetCore.Identity.SignInResult.RequiresTwoFactor))
        });

        return RedirectToPage("./VerifySecurityCode", new
        {
            provider = SelectedProvider,
            returnUrl = ReturnUrl,
            returnUrlHash = ReturnUrlHash,
            rememberMe = RememberMe,
            linkUserId = LinkUserId,
            linkTenantId = LinkTenantId,
            linkToken = LinkToken
        });
    }
}
