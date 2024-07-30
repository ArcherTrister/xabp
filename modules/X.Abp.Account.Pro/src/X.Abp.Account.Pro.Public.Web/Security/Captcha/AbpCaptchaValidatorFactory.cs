// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

using X.Abp.Account.Security.Captcha;
using X.Abp.Account.Settings;

namespace X.Abp.Account.Public.Web.Security.Captcha;

public class AbpCaptchaValidatorFactory : IAbpCaptchaValidatorFactory, ITransientDependency
{
    protected ISettingProvider SettingProvider { get; }

    protected IServiceProvider ServiceProvider { get; }

    public AbpCaptchaValidatorFactory(
        ISettingProvider settingProvider,
        IServiceProvider serviceProvider)
    {
        SettingProvider = settingProvider;
        ServiceProvider = serviceProvider;
    }

    public virtual async Task<ICaptchaValidator> CreateAsync()
    {
        return await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.Version) switch
        {
            "ReCaptchaV3" => ServiceProvider.GetService<ReCaptchaV3Validator>(),
            "ReCaptchaV2" => ServiceProvider.GetService<ReCaptchaV2Validator>(),
            "HCaptcha" => ServiceProvider.GetService<HCaptchaValidator>(),
            _ => throw new ArgumentException(nameof(AccountSettingNames.Captcha.Version)),
        };
    }
}
