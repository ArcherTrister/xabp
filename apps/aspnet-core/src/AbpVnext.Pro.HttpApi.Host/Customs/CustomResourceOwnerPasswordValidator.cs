// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.AspNetIdentity;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Services;

public class CustomResourceOwnerPasswordValidator : AbpResourceOwnerPasswordValidator
{
    public CustomResourceOwnerPasswordValidator(IdentityUserManager userManager, SignInManager<IdentityUser> signInManager, IdentitySecurityLogManager identitySecurityLogManager, ILogger<ResourceOwnerPasswordValidator<IdentityUser>> logger, IStringLocalizer<AbpIdentityServerResource> localizer, IOptions<AbpIdentityOptions> abpIdentityOptions, IServiceScopeFactory serviceScopeFactory, IOptions<IdentityOptions> identityOptions, ISettingProvider settingProvider)
        : base(userManager, signInManager, identitySecurityLogManager, logger, localizer, abpIdentityOptions, serviceScopeFactory, identityOptions, settingProvider)
    {
    }

    /// <summary>
    /// https://github.com/IdentityServer/IdentityServer4/blob/master/src/AspNetIdentity/src/ResourceOwnerPasswordValidator.cs#L53
    /// </summary>
    [UnitOfWork]
    public override async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            await ReplaceEmailToUsernameOfInputIfNeeds(context);

            IdentityUser user = null;

            if (AbpIdentityOptions.ExternalLoginProviders.Any())
            {
                foreach (var externalLoginProviderInfo in AbpIdentityOptions.ExternalLoginProviders.Values)
                {
                    var externalLoginProvider = (IExternalLoginProvider)scope.ServiceProvider
                        .GetRequiredService(externalLoginProviderInfo.Type);

                    if (await externalLoginProvider.TryAuthenticateAsync(context.UserName, context.Password))
                    {
                        user = await UserManager.FindByNameAsync(context.UserName);
                        if (user == null)
                        {
                            user = await externalLoginProvider.CreateUserAsync(context.UserName, externalLoginProviderInfo.Name);
                        }
                        else
                        {
                            await externalLoginProvider.UpdateUserAsync(user, externalLoginProviderInfo.Name);
                        }

                        await SetSuccessResultAsync(context, user);
                        return;
                    }
                }
            }

            user = await UserManager.FindByNameAsync(context.UserName);
            string errorDescription;
            if (user != null)
            {
                await IdentityOptions.SetAsync();
                var result = await SignInManager.CheckPasswordSignInAsync(user, context.Password, true);
                if (result.Succeeded)
                {
                    if (await IsTfaEnabledAsync(user))
                    {
                        await HandleTwoFactorLoginAsync(context, user);
                    }
                    else
                    {
                        await SetSuccessResultAsync(context, user);
                    }

                    return;
                }

                if (result.IsLockedOut)
                {
                    Logger.LogInformation("Authentication failed for username: {UserName}, reason: locked out", context.UserName);
                    errorDescription = Localizer["UserLockedOut"];
                }
                else if (result.IsNotAllowed)
                {
                    Logger.LogInformation("Authentication failed for username: {UserName}, reason: not allowed", context.UserName);

                    if (user.ShouldChangePasswordOnNextLogin)
                    {
                        await HandleShouldChangePasswordOnNextLoginAsync(context, user, context.Password);
                        return;
                    }

                    if (await UserManager.ShouldPeriodicallyChangePasswordAsync(user))
                    {
                        await HandlePeriodicallyChangePasswordAsync(context, user, context.Password);
                        return;
                    }

                    errorDescription = Localizer["LoginIsNotAllowed"];
                }
                else
                {
                    Logger.LogInformation("Authentication failed for username: {UserName}, reason: invalid credentials", context.UserName);
                    errorDescription = Localizer["InvalidUserNameOrPassword"];
                }

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = result.ToIdentitySecurityLogAction(),
                    UserName = context.UserName,
                    ClientId = await FindClientIdAsync(context)
                });
            }
            else
            {
                Logger.LogInformation("No user found matching username: {UserName}", context.UserName);
                errorDescription = Localizer["InvalidUsername"];

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = IdentityServerSecurityLogActionConsts.LoginInvalidUserName,
                    UserName = context.UserName,
                    ClientId = await FindClientIdAsync(context)
                });
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, errorDescription);
        }
    }

    protected override async Task HandleTwoFactorLoginAsync(ResourceOwnerPasswordValidationContext context, IdentityUser user)
    {
        var recoveryCode = context.Request?.Raw?["RecoveryCode"];
        if (!recoveryCode.IsNullOrWhiteSpace())
        {
            var result = await UserManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
            if (result.Succeeded)
            {
                await SetSuccessResultAsync(context, user);
                return;
            }

            Logger.LogInformation("Authentication failed for username: {UserName}, reason: InvalidRecoveryCode", context.UserName);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, Localizer["InvalidRecoveryCode"]);
        }

        var twoFactorProvider = context.Request?.Raw?["TwoFactorProvider"];
        var twoFactorCode = context.Request?.Raw?["TwoFactorCode"];
        if (!twoFactorProvider.IsNullOrWhiteSpace() && !twoFactorCode.IsNullOrWhiteSpace())
        {
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
            if (providers.Contains(twoFactorProvider) && await UserManager.VerifyTwoFactorTokenAsync(user, twoFactorProvider, twoFactorCode))
            {
                await SetSuccessResultAsync(context, user);
                return;
            }

            await UserManager.AccessFailedAsync(user);

            Logger.LogInformation("Authentication failed for username: {UserName}, reason: InvalidAuthenticatorCode", context.UserName);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, Localizer["InvalidAuthenticatorCode"]);
        }
        else
        {
            Logger.LogInformation("Authentication failed for username: {UserName}, reason: RequiresTwoFactor", context.UserName);
            var twoFactorToken = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor));
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                nameof(SignInResult.RequiresTwoFactor),
                new Dictionary<string, object>()
                {
                    { "userId", user.Id },
                    { "twoFactorToken", twoFactorToken }
                });

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                Action = IdentityServerSecurityLogActionConsts.LoginRequiresTwoFactor,
                UserName = context.UserName,
                ClientId = await FindClientIdAsync(context)
            });
        }
    }

    protected override async Task HandleShouldChangePasswordOnNextLoginAsync(ResourceOwnerPasswordValidationContext context, IdentityUser user, string currentPassword)
    {
        await HandlerChangePasswordAsync(context, user, currentPassword, ChangePasswordType.ShouldChangePasswordOnNextLogin);
    }

    protected override async Task HandlePeriodicallyChangePasswordAsync(ResourceOwnerPasswordValidationContext context, IdentityUser user, string currentPassword)
    {
        await HandlerChangePasswordAsync(context, user, currentPassword, ChangePasswordType.PeriodicallyChangePassword);
    }

    protected override async Task HandlerChangePasswordAsync(ResourceOwnerPasswordValidationContext context, IdentityUser user, string currentPassword, ChangePasswordType changePasswordType)
    {
        var changePasswordToken = context.Request?.Raw?["ChangePasswordToken"];
        var newPassword = context.Request?.Raw?["NewPassword"];
        if (!changePasswordToken.IsNullOrWhiteSpace() && !currentPassword.IsNullOrWhiteSpace() && !newPassword.IsNullOrWhiteSpace())
        {
            if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, changePasswordType.ToString(), changePasswordToken))
            {
                var changePasswordResult = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (changePasswordResult.Succeeded)
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                        Action = IdentitySecurityLogActionConsts.ChangePassword,
                        UserName = context.UserName,
                        ClientId = await FindClientIdAsync(context)
                    });

                    if (changePasswordType == ChangePasswordType.ShouldChangePasswordOnNextLogin)
                    {
                        user.SetShouldChangePasswordOnNextLogin(false);
                    }

                    await UserManager.UpdateAsync(user);
                    await SetSuccessResultAsync(context, user);
                }
                else
                {
                    Logger.LogInformation("ChangePassword failed for username: {UserName}, reason: {ChangePasswordResult}", context.UserName, changePasswordResult);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, changePasswordResult.Errors.Select(x => x.Description).JoinAsString(", "));
                }
            }
            else
            {
                Logger.LogInformation("Authentication failed for username: {UserName}, reason: InvalidAuthenticatorCode", context.UserName);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, Localizer["InvalidAuthenticatorCode"]);
            }
        }
        else
        {
            Logger.LogInformation("Authentication failed for username: {{{UserName}}}, reason: {{{ChangePasswordType}}}", context.UserName, changePasswordType.ToString());
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                nameof(user.ShouldChangePasswordOnNextLogin),
                new Dictionary<string, object>()
                {
                    { "userId", user.Id },
                    { "changePasswordToken", await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, changePasswordType.ToString()) }
                });

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                Action = IdentityServerSecurityLogActionConsts.LoginNotAllowed,
                UserName = context.UserName,
                ClientId = await FindClientIdAsync(context)
            });
        }
    }

    protected override async Task SetSuccessResultAsync(ResourceOwnerPasswordValidationContext context, IdentityUser user)
    {
        var sub = await UserManager.GetUserIdAsync(user);

        Logger.LogInformation("Credentials validated for username: {UserName}", context.UserName);

        var additionalClaims = new List<Claim>();

        await AddCustomClaimsAsync(additionalClaims, user, context);

        context.Result = new GrantValidationResult(
            sub,
            OidcConstants.AuthenticationMethods.Password,
            additionalClaims.ToArray());

        await IdentitySecurityLogManager.SaveAsync(
            new IdentitySecurityLogContext
            {
                Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                Action = IdentityServerSecurityLogActionConsts.LoginSucceeded,
                UserName = context.UserName,
                ClientId = await FindClientIdAsync(context)
            });
    }

    protected override async Task ReplaceEmailToUsernameOfInputIfNeeds(ResourceOwnerPasswordValidationContext context)
    {
        if (!ValidationHelper.IsValidEmailAddress(context.UserName))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(context.UserName);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(context.UserName);
        if (userByEmail == null)
        {
            return;
        }

        context.UserName = userByEmail.UserName;
    }

    protected override Task<string> FindClientIdAsync(ResourceOwnerPasswordValidationContext context)
    {
        return Task.FromResult(context.Request?.Client?.ClientId);
    }

    protected override async Task<bool> IsTfaEnabledAsync(IdentityUser user)
        => UserManager.SupportsUserTwoFactor &&
           await UserManager.GetTwoFactorEnabledAsync(user) &&
           (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

    protected override Task AddCustomClaimsAsync(List<Claim> customClaims, IdentityUser user, ResourceOwnerPasswordValidationContext context)
    {
        if (user.TenantId.HasValue)
        {
            customClaims.Add(
                new Claim(
                    AbpClaimTypes.TenantId,
                    user.TenantId?.ToString()));
        }

        return Task.CompletedTask;
    }
}
