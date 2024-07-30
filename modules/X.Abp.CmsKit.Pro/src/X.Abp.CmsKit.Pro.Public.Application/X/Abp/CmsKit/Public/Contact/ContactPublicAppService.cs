// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Services;

using X.Abp.CmsKit.Contact;
using X.Captcha.Re;

namespace X.Abp.CmsKit.Public.Contact;

public class ContactPublicAppService : ApplicationService, IContactPublicAppService
{
    // public IAbpCaptchaValidatorFactory CaptchaValidatorFactory { get; set; }

    // protected IOptionsSnapshot<CaptchaOptions> CaptchaOptions { get; }
    protected IReCaptchaV3SiteVerify ReCaptchaV3SiteVerify { get; }

    protected CmsKitContactOptions CmsKitContactOptions { get; }

    protected ContactEmailSender ContactEmailSender { get; }

    public ContactPublicAppService(
        IOptions<CmsKitContactOptions> cmsKitContactOptions,
        ContactEmailSender contactEmailSender)
    {
        CmsKitContactOptions = cmsKitContactOptions.Value;
        ContactEmailSender = contactEmailSender;

        // CaptchaValidatorFactory = NullAbpCaptchaValidatorFactory.Instance;
    }

    public virtual async Task SendMessageAsync(ContactCreateInput input)
    {
        if (!VerifyCaptchaToken(input.CaptchaToken))
        {
            throw new UserFriendlyException(L["CaptchaCodeErrorMessage"]);
        }

        await ContactEmailSender.SendAsync(input.Name, input.Subject, input.Email, input.Message);
    }

    private bool VerifyCaptchaToken(string token)
    {
        if (!CmsKitContactOptions.IsCaptchaEnabled)
        {
            return true;
        }

        // var captchaVersion = await SettingProvider.GetOrNullAsync(CaptchaSettings.Version);
        // await CaptchaOptions.SetAsync(captchaVersion);
        // var captchaValidator = await CaptchaValidatorFactory.CreateAsync();
        // return await captchaValidator.VerifyAsync(token);
        // TODO: Verify
        return false;
    }
}
