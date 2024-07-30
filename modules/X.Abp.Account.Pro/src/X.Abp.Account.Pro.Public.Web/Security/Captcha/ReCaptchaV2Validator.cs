// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

using X.Abp.Account.Localization;
using X.Captcha;
using X.Captcha.Re;

namespace X.Abp.Account.Public.Web.Security.Captcha;

[ExposeServices(typeof(ReCaptchaV2Validator))]
public class ReCaptchaV2Validator : CaptchaValidatorBase, ITransientDependency
{
    protected IReCaptchaV2SiteVerify ReCaptchaV2SiteVerify { get; }

    public ReCaptchaV2Validator(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<AccountResource> stringLocalizer,
        IJsonSerializer jsonSerializer,
        IReCaptchaV2SiteVerify reCaptchaV2SiteVerify)
        : base(httpContextAccessor,
            stringLocalizer,
            jsonSerializer)
    {
        ReCaptchaV2SiteVerify = reCaptchaV2SiteVerify;
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

        var response = await ReCaptchaV2SiteVerify.Verify(new CaptchaSiteVerifyRequest
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
