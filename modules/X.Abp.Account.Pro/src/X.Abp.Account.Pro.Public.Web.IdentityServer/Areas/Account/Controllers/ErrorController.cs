// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Views.Error;
using Volo.Abp.Http;

namespace X.Abp.Account.Web.Areas.Account.Controllers;

[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
public class ErrorController : AbpController
{
    protected IIdentityServerInteractionService IdentityServerInteractionService { get; }

    protected IWebHostEnvironment Environment { get; }

    protected AbpErrorPageOptions AbpErrorPageOptions { get; }

    public ErrorController(
        IIdentityServerInteractionService identityServerInteractionService,
        IWebHostEnvironment environment,
        IOptions<AbpErrorPageOptions> abpErrorPageOptions)
    {
        IdentityServerInteractionService = identityServerInteractionService;
        Environment = environment;
        AbpErrorPageOptions = abpErrorPageOptions.Value;
    }

    public virtual async Task<IActionResult> Index(string errorId)
    {
        var errorMessage = await IdentityServerInteractionService.GetErrorContextAsync(errorId) ?? new ErrorMessage
        {
            Error = L["Error"]
        };

        if (!Environment.IsDevelopment())
        {
            // Only show in development
            errorMessage.ErrorDescription = null;
        }

        const int statusCode = (int)HttpStatusCode.InternalServerError;

        return View(GetErrorPageUrl(statusCode), new AbpErrorViewModel
        {
            ErrorInfo = new RemoteServiceErrorInfo(errorMessage.Error, errorMessage.ErrorDescription),
            HttpStatusCode = statusCode
        });
    }

    protected virtual string GetErrorPageUrl(int statusCode)
    {
        var page = AbpErrorPageOptions.ErrorViewUrls.GetOrDefault(statusCode.ToString());

        return string.IsNullOrWhiteSpace(page) ? "~/Views/Error/Default.cshtml" : page;
    }
}
