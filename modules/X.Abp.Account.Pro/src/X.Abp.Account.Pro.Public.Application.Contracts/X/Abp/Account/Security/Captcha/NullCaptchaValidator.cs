// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

namespace X.Abp.Account.Security.Captcha;

public class NullCaptchaValidator : ICaptchaValidator
{
    public static NullCaptchaValidator Instance { get; } = new NullCaptchaValidator();

    public Task ValidateAsync(string captchaResponse)
    {
        return Task.CompletedTask;
    }
}
