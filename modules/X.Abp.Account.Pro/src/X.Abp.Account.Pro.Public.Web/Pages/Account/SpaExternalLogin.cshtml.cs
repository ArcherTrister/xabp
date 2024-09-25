// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.ExternalProviders;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class SpaExternalLoginModel : AbpPageModel
    {
        protected SignInManager<IdentityUser> SignInManager => LazyServiceProvider.LazyGetRequiredService<SignInManager<IdentityUser>>();

        protected IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();

        protected IOptions<IdentityOptions> IdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();

        protected ITokenGeneratorProvider TokenGeneratorProvider => LazyServiceProvider.LazyGetRequiredService<ITokenGeneratorProvider>();

        public virtual async Task<IActionResult> OnGetAsync()
        {
            return await Task.FromResult(NotFound());
        }

        /// <summary>
        /// SPA第三方账号登录
        /// </summary>
        /// <param name="provider">第三方提供者</param>
        /// <param name="clientId">客户端Id</param>
        /// <param name="clientSecret">客户端Secret</param>
        /// <param name="scope">scope</param>
        /// <param name="returnUrl">登录成功回调地址</param>
        /// <returns>IActionResult</returns>
        public virtual async Task<IActionResult> OnPostAsync(string provider, string clientId, string clientSecret, string scope, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(returnUrl))
            {
                Logger.LogWarning("The parameter is incorrect");
                return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
                {
                    ["error"] = "The parameter is incorrect"
                }));
            }

            var tenantId = CurrentTenant.Id;
            Logger.LogInformation("CurrentTenant:{TenantId}", tenantId);

            var redirectUrl = Url.Page("SpaExternalLogin", pageHandler: "Callback", values: new { returnUrl, tenantId, clientId, clientSecret, scope });

            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["scheme"] = provider;

            return await Task.FromResult(Challenge(properties, provider));
        }

        /// <summary>
        /// SPA第三方账号登录回调
        /// </summary>
        /// <param name="returnUrl">登录成功回调地址</param>
        /// <param name="clientId">客户端Id</param>
        /// <param name="clientSecret">客户端Secret</param>
        /// <param name="scope">scope</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="remoteError">错误信息</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task<IActionResult> OnGetCallbackAsync(string returnUrl, string clientId, string clientSecret, string scope, Guid? tenantId, string remoteError = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || string.IsNullOrWhiteSpace(clientId))
            {
                Logger.LogWarning("The parameter is incorrect");
                return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
                {
                    ["error"] = "The parameter is incorrect"
                }));
            }

            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                Logger.LogWarning("External login callback error: {RemoteError}", remoteError);
                return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
                {
                    ["error"] = remoteError
                }));
            }

            Logger.LogInformation("CurrentTenant:{TenantId}", tenantId);

            await IdentityOptions.SetAsync();

            var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                Logger.LogWarning("External login info is not available");
                return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
                {
                    ["error"] = "External login info is not available"
                }));
            }

            var result = await SpaExternalLoginSignInAsync(tenantId, loginInfo.LoginProvider, loginInfo.ProviderKey, clientId, clientSecret, scope);

            return result.IsError
            ? Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
            {
                ["error"] = result.Error
            }))
            : Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
            {
                ["access_token"] = result.AccessToken,
                ["refresh_token"] = result.RefreshToken,
                ["expires_in"] = result.ExpiresIn.ToString(),
                ["token_type"] = result.TokenType
            }));
        }

        protected virtual async Task<TokenGeneratorResult> SpaExternalLoginSignInAsync(Guid? tenantId, string loginProvider, string providerKey, string clientId, string clientSecret, string scope)
        {
            // Cleanup cookie
            await SignInManager.SignOutAsync();

            return await TokenGeneratorProvider.CreateSpaExternalLoginAccessTokenAsync(loginProvider, providerKey, tenantId, clientId, clientSecret, scope);
        }

        // TODO: 用户不存在是否需要创建
        protected virtual async Task<IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
        {
            await IdentityOptions.SetAsync();

            var emailAddress = info.Principal.FindFirstValue(AbpClaimTypes.Email);

            var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

            CheckIdentityErrors(await UserManager.CreateAsync(user));
            CheckIdentityErrors(await UserManager.SetEmailAsync(user, emailAddress));
            CheckIdentityErrors(await UserManager.AddLoginAsync(user, info));
            CheckIdentityErrors(await UserManager.AddDefaultRolesAsync(user));

            user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
            user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);

            var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
            if (!phoneNumber.IsNullOrWhiteSpace())
            {
                var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.OrdinalIgnoreCase);
                user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
            }

            CheckIdentityErrors(await UserManager.UpdateAsync(user));

            return user;
        }

        protected virtual void CheckIdentityErrors(IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
            }

            // identityResult.CheckIdentityErrors(LocalizationManager); //TODO: Get from old Abp
        }
    }
}
