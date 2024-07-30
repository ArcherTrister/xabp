// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp.Options;
using Volo.Abp.Settings;

using X.Abp.Account.Settings;
using X.Captcha;

namespace X.Abp.Account.Public.Web.Security.Captcha;

public class AbpCaptchaOptionsManager : AbpDynamicOptionsManager<CaptchaOptions>
{
    protected ISettingProvider SettingProvider { get; }

    public AbpCaptchaOptionsManager(IOptionsFactory<CaptchaOptions> factory, ISettingProvider settingProvider)
        : base(factory)
    {
        SettingProvider = settingProvider;
    }

    protected override async Task OverrideOptionsAsync(string name, CaptchaOptions options)
    {
        options.VerifyBaseUrl = await GetSettingOrDefaultValue(AccountSettingNames.Captcha.VerifyBaseUrl, options.VerifyBaseUrl);
        options.SiteKey = await GetSettingOrDefaultValue(AccountSettingNames.Captcha.SiteKey, options.SiteKey);
        options.SiteSecret = await GetSettingOrDefaultValue(AccountSettingNames.Captcha.SiteSecret, options.SiteSecret);
    }

    protected virtual async Task<string> GetSettingOrDefaultValue(string name, string defaultValue)
    {
        var value = await SettingProvider.GetOrNullAsync(name);
        return value.IsNullOrWhiteSpace() ? defaultValue : value;
    }
}
