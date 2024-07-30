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

using X.Abp.Account.Localization;
using X.Captcha;
using X.Captcha.L;

namespace X.Abp.Account.Public.Web.Security.Captcha;

[ExposeServices(typeof(LCaptchaV2Validator))]
public class LCaptchaV2Validator : CaptchaValidatorBase, ITransientDependency
{
    protected ILCaptchaV2SiteVerify LCaptchaV2SiteVerify { get; }

    public LCaptchaV2Validator(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<AccountResource> stringLocalizer,
        IJsonSerializer jsonSerializer,
        ILCaptchaV2SiteVerify lCaptchaV2SiteVerify)
        : base(httpContextAccessor,
            stringLocalizer,
            jsonSerializer)
    {
        LCaptchaV2SiteVerify = lCaptchaV2SiteVerify;
    }

    public override async Task ValidateAsync(string captchaResponse)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new Exception("LCaptchaValidator should be used in a valid HTTP context!");
        }

        if (captchaResponse.IsNullOrEmpty())
        {
            throw new UserFriendlyException(StringLocalizer["CaptchaCanNotBeEmpty"]);
        }

        var response = await LCaptchaV2SiteVerify.Verify(new CaptchaSiteVerifyRequest
        {
            Response = captchaResponse,
            RemoteIp = httpContext.Connection?.RemoteIpAddress?.ToString()
        });

        if (!response.Success)
        {
            Logger.LogWarning(JsonSerializer.Serialize(response));
            throw new UserFriendlyException(StringLocalizer["IncorrectCaptchaAnswer"]);
        }
    }
}
