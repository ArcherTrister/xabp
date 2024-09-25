// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityServer4.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(LogoutModel))]
public class IdentityServerSupportedLogoutModel : LogoutModel
{
    protected IIdentityServerInteractionService Interaction { get; }

    public IdentityServerSupportedLogoutModel(
        IIdentityServerInteractionService interaction)
        : base()
    {
        Interaction = interaction;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        await SignInManager.SignOutAsync();

        var logoutId = Request.Query["logoutId"].ToString();

        if (!string.IsNullOrEmpty(logoutId))
        {
            var logoutContext = await Interaction.GetLogoutContextAsync(logoutId);

            await SaveSecurityLogAsync(logoutContext?.ClientId);

            await SignInManager.SignOutAsync();

            // for ui to see an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            var vm = new LoggedOutModel()
            {
                PostLogoutRedirectUri = logoutContext?.PostLogoutRedirectUri,
                ClientName = logoutContext?.ClientName,
                SignOutIframeUrl = logoutContext?.SignOutIFrameUrl
            };

            var queryCulture = logoutContext?.Parameters.Get(IdentityServerReturnUrlRequestCultureProvider.QueryStringKey);
            var queryUICulture = logoutContext?.Parameters.Get(IdentityServerReturnUrlRequestCultureProvider.UIQueryStringKey);
            if (!queryCulture.IsNullOrWhiteSpace())
            {
                vm.Culture = queryCulture;
            }

            if (!queryUICulture.IsNullOrWhiteSpace())
            {
                vm.UICulture = queryUICulture;
            }

            Logger.LogInformation("Redirecting to LoggedOut Page...");
            return RedirectToPage("./LoggedOut", vm);
        }

        await SaveSecurityLogAsync();

        if (ReturnUrl != null)
        {
            return LocalRedirect(ReturnUrl);
        }

        Logger.LogInformation(
            $"IdentityServerSupportedLogoutModel couldn't find postLogoutUri... Redirecting to:/Account/Login..");
        return RedirectToPage("/Account/Login");
    }

    protected virtual async Task SaveSecurityLogAsync(string clientId = null)
    {
        if (CurrentUser.IsAuthenticated)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout,
                ClientId = clientId
            });
        }
    }
}
