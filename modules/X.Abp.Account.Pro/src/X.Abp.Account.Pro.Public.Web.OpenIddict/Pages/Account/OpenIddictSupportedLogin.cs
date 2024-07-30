// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;

using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Security.Captcha;
using X.Captcha;

using static OpenIddict.Server.OpenIddictServerEvents;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

[ExposeServices(typeof(LoginModel))]
public class OpenIddictSupportedLoginModel : LoginModel
{
    protected AbpOpenIddictRequestHelper OpenIddictRequestHelper { get; }

    public OpenIddictSupportedLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IAbpCaptchaValidatorFactory captchaValidatorFactory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IIdentityLinkUserAppService identityLinkUserAppService,
        IOptions<IdentityOptions> identityOptions,
        IOptionsSnapshot<CaptchaOptions> captchaOptions,
        AbpOpenIddictRequestHelper openIddictRequestHelper)
        : base(schemeProvider, accountOptions, captchaValidatorFactory, accountExternalProviderAppService, currentPrincipalAccessor, signInManager, userManager, identitySecurityLogManager, identityLinkUserAppService, identityOptions, captchaOptions)
    {
        OpenIddictRequestHelper = openIddictRequestHelper;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();
        var openIddictRequest = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl);
        if (openIddictRequest != null && openIddictRequest.ClientId != null)
        {
            LoginInput.UserNameOrEmailAddress = openIddictRequest.LoginHint;

            var tenant = openIddictRequest.GetParameter(TenantResolverConsts.DefaultTenantKey)?.ToString();
            if (!string.IsNullOrEmpty(tenant))
            {
                CurrentTenant.Change(Guid.Parse(tenant));
                Response.Cookies.Append(TenantResolverConsts.DefaultTenantKey, tenant);
            }
        }

        return await base.OnGetAsync();
    }

    public override async Task<IActionResult> OnPostAsync(string action)
    {
        if (action == "Cancel")
        {
            var request = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl);
            var transaction = HttpContext.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction;

            if (request?.ClientId != null && transaction != null)
            {
                transaction.EndpointType = OpenIddictServerEndpointType.Authorization;
                transaction.Request = request;

                var notification = new ValidateAuthorizationRequestContext(transaction);
                transaction.SetProperty(typeof(ValidateAuthorizationRequestContext).FullName!, notification);

                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return Redirect("~/");
        }

        return await base.OnPostAsync(action);
    }

    public override async Task<IActionResult> OnPostExternalLoginAsync(string provider)
    {
        return AccountOptions.WindowsAuthenticationSchemeName == provider
            ? await ProcessWindowsLoginAsync()
            : await base.OnPostExternalLoginAsync(provider);
    }

    protected virtual async Task<IActionResult> ProcessWindowsLoginAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
        if (authenticateResult.Succeeded)
        {
            AuthenticationProperties props = new()
            {
                RedirectUri = Url.Page("./Login",
                "ExternalLoginCallback",
                new { ReturnUrl, ReturnUrlHash }),
                Items =
                {
                    { "LoginProvider", AccountOptions.WindowsAuthenticationSchemeName }
                }
            };
            ClaimsIdentity claimsIdentity = new(AccountOptions.WindowsAuthenticationSchemeName);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, authenticateResult.Principal.FindFirstValue(ClaimTypes.PrimarySid)));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, authenticateResult.Principal.FindFirstValue(ClaimTypes.Name)));
            await HttpContext.SignInAsync(IdentityConstants.ExternalScheme, new ClaimsPrincipal(claimsIdentity), props);
            return Redirect(props.RedirectUri);
        }

        return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
    }
}
