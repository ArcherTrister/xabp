// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Identity;

using X.Abp.Identity;

namespace X.Abp.Account.Emailing;

public interface IAccountEmailer
{
    Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string resetToken,
        string appName,
        string returnUrl = null,
        string returnUrlHash = null);

    Task SendEmailConfirmationLinkAsync(
        IdentityUser user,
        string confirmationToken,
        string appName,
        string returnUrl = null,
        string returnUrlHash = null);

    Task SendEmailSecurityCodeAsync(
        IdentityUser user,
        string code);
}
