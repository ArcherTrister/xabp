// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.Settings;

using X.Abp.Account.Localization;
using X.Abp.Account.Settings;
using X.Captcha;
using X.Captcha.G;

namespace X.Abp.Account.Public.Web.Security.Captcha;

[ExposeServices(typeof(GCaptchaV3Validator))]
public class GCaptchaV3Validator : CaptchaValidatorBase, ITransientDependency
{
    protected IGCaptchaV3SiteVerify GCaptchaV3SiteVerify { get; }

    protected ISettingProvider SettingProvider { get; }

    public GCaptchaV3Validator(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<AccountResource> stringLocalizer,
        IJsonSerializer jsonSerializer,
        IGCaptchaV3SiteVerify gCaptchaV3SiteVerify,
        ISettingProvider settingProvider)
        : base(httpContextAccessor, stringLocalizer, jsonSerializer)
    {
        GCaptchaV3SiteVerify = gCaptchaV3SiteVerify;
        SettingProvider = settingProvider;
    }

    public override async Task ValidateAsync(string captchaResponse)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new Exception("ReCaptchaValidator should be used in a valid HTTP context!");
        }

        if (captchaResponse.IsNullOrEmpty())
        {
            throw new UserFriendlyException(StringLocalizer["CaptchaCanNotBeEmpty"]);
        }

        var response = await GCaptchaV3SiteVerify.Verify(new CaptchaSiteVerifyRequest
        {
            Response = captchaResponse,
            RemoteIp = httpContext.Connection?.RemoteIpAddress?.ToString()
        });

        var score = await SettingProvider.GetAsync<double>(AccountSettingNames.Captcha.Score);
        if (!response.Success || response.Score < score)
        {
            Logger.LogError("google response: " + JsonSerializer.Serialize(response));
            throw new ScoreBelowThresholdException(StringLocalizer["ScoreBelowThreshold"]);
        }
    }
}
