// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class EmailConfirmationModel : AccountPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }

    [Required]
    [BindProperty(SupportsGet = true)]
    public string ConfirmationToken { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    public bool EmailConfirmed { get; set; }

    protected IAccountAppService AccountAppService { get; }

    public EmailConfirmationModel(IAccountAppService accountAppService)
    {
        AccountAppService = accountAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ReturnUrl = await GetRedirectUrlAsync(ReturnUrl, ReturnUrlHash);

        try
        {
            await AccountAppService.ConfirmEmailAsync(new ConfirmEmailInput
            {
                UserId = UserId,
                Token = ConfirmationToken
            });
        }
        catch (Exception e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        EmailConfirmed = true;
        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
