// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.Json;

using X.Abp.Account.Localization;
using X.Abp.Account.Security.Captcha;

namespace X.Abp.Account.Public.Web.Security.Captcha;

public abstract class CaptchaValidatorBase : ICaptchaValidator
{
    public const string CaptchaResponseKey = "captcha-response";

    protected ILogger<CaptchaValidatorBase> Logger { get; }

    protected IHttpContextAccessor HttpContextAccessor { get; }

    protected IStringLocalizer<AccountResource> StringLocalizer { get; }

    protected IJsonSerializer JsonSerializer { get; }

    protected CaptchaValidatorBase(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<AccountResource> stringLocalizer,
        IJsonSerializer jsonSerializer)
    {
        HttpContextAccessor = httpContextAccessor;
        StringLocalizer = stringLocalizer;
        JsonSerializer = jsonSerializer;

        Logger = NullLogger<CaptchaValidatorBase>.Instance;
    }

    public abstract Task ValidateAsync(string captchaResponse);
}
