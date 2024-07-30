// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Localization;
using X.Abp.Account.Settings;
using X.Abp.Identity.Features;
using X.Abp.Identity.Settings;

namespace X.Abp.Account;

[Authorize(AccountPermissions.SettingManagement)]
public class AccountSettingsAppService : ApplicationService, IAccountSettingsAppService
{
    protected ISettingManager SettingManager { get; }

    protected ExternalProviderSettingsHelper ExternalProviderSettingsHelper { get; }

    public AccountSettingsAppService(ISettingManager settingManager, ExternalProviderSettingsHelper externalProviderSettingsHelper)
    {
        SettingManager = settingManager;
        ExternalProviderSettingsHelper = externalProviderSettingsHelper;
        LocalizationResource = typeof(AccountResource);
    }

    public virtual async Task<AccountSettingsDto> GetAsync()
    {
        return new AccountSettingsDto
        {
            IsSelfRegistrationEnabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.IsSelfRegistrationEnabled),
            EnableLocalLogin = await SettingProvider.GetAsync<bool>(AccountSettingNames.EnableLocalLogin)
        };
    }

    public virtual async Task UpdateAsync(AccountSettingsDto input)
    {
        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.IsSelfRegistrationEnabled, input.IsSelfRegistrationEnabled.ToString());
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.EnableLocalLogin, input.EnableLocalLogin.ToString());
        }
    }

    public virtual async Task<AccountTwoFactorSettingsDto> GetTwoFactorAsync()
    {
        await CheckTwoFactorAvailableAsync();

        return new AccountTwoFactorSettingsDto
        {
            TwoFactorBehaviour = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider),
            IsRememberBrowserEnabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled),
            UsersCanChange = await SettingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange)
        };
    }

    public virtual async Task UpdateTwoFactorAsync(AccountTwoFactorSettingsDto input)
    {
        await CheckTwoFactorAvailableAsync();

        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.TwoFactor.Behaviour, input.TwoFactorBehaviour.ToString());
            if (input.TwoFactorBehaviour == IdentityProTwoFactorBehaviour.Optional)
            {
                await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.TwoFactor.UsersCanChange, input.UsersCanChange.ToString());
            }

            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled, input.IsRememberBrowserEnabled.ToString());
        }
    }

    public virtual async Task<AccountExternalProviderSettingsDto> GetExternalProviderAsync()
    {
        return new AccountExternalProviderSettingsDto
        {
            Settings = await ExternalProviderSettingsHelper.GetAllAsync()
        };
    }

    public virtual async Task UpdateExternalProviderAsync(List<UpdateExternalProviderDto> input)
    {
        foreach (var setting in input)
        {
            await ExternalProviderSettingsHelper.SetAsync(new ExternalProviderSettings
            {
                Name = setting.Name,
                Enabled = setting.Enabled,
                Properties = setting.Properties,
                SecretProperties = setting.SecretProperties
            });

            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }

    protected virtual async Task CheckTwoFactorAvailableAsync()
    {
        var behaviour = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (behaviour == IdentityProTwoFactorBehaviour.Disabled)
        {
            throw new BusinessException(message: L["TwoFactorHasBeenDisabled"]);
        }
    }

    public virtual async Task<AccountCaptchaSettingsDto> GetRecaptchaAsync()
    {
        var settings = new AccountCaptchaSettingsDto
        {
            UseCaptchaOnLogin = await SettingProvider.GetAsync<bool>(AccountSettingNames.Captcha.UseCaptchaOnLogin),
            UseCaptchaOnRegistration = await SettingProvider.GetAsync<bool>(AccountSettingNames.Captcha.UseCaptchaOnRegistration),
            VerifyBaseUrl = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.VerifyBaseUrl),
            SiteKey = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.SiteKey),
            SiteSecret = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.SiteSecret),
            Version = await SettingProvider.GetAsync<int>(AccountSettingNames.Captcha.Version),
            Score = await SettingProvider.GetAsync<double>(AccountSettingNames.Captcha.Score)
        };

        if (CurrentTenant.IsAvailable)
        {
            settings.SiteKey = await SettingManager.GetOrNullForTenantAsync(AccountSettingNames.Captcha.SiteKey, CurrentTenant.GetId(), false);
            settings.SiteSecret = await SettingManager.GetOrNullForTenantAsync(AccountSettingNames.Captcha.SiteSecret, CurrentTenant.GetId(), false);
        }

        return settings;
    }

    public virtual async Task UpdateRecaptchaAsync(AccountCaptchaSettingsDto input)
    {
        if (!CurrentTenant.IsAvailable)
        {
            if ((input.UseCaptchaOnLogin || input.UseCaptchaOnRegistration) &&
                (input.SiteKey.IsNullOrWhiteSpace() || input.SiteSecret.IsNullOrWhiteSpace()))
            {
                throw new UserFriendlyException(L["InvalidSiteKeyOrSiteSecret"]);
            }

            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.UseCaptchaOnLogin, input.UseCaptchaOnLogin.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration, input.UseCaptchaOnRegistration.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.VerifyBaseUrl, input.VerifyBaseUrl);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.SiteKey, input.SiteKey);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.SiteSecret, input.SiteSecret);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.Version, input.Version.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.Score, input.Score.ToString());
        }
        else
        {
            var globalVersion = (await SettingManager.GetOrNullGlobalAsync(AccountSettingNames.Captcha.Version)).To<int>();

            if (globalVersion != input.Version &&
                (input.SiteKey.IsNullOrWhiteSpace() || input.SiteSecret.IsNullOrWhiteSpace()))
            {
                throw new UserFriendlyException(L["InvalidSiteKeyOrSiteSecret"]);
            }

            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.Version, input.Version.ToString());
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.SiteKey, input.SiteKey.IsNullOrWhiteSpace() ? null : input.SiteKey);
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.SiteSecret, input.SiteSecret.IsNullOrWhiteSpace() ? null : input.SiteSecret);
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.Score, input.Score.ToString());
        }
    }
}
