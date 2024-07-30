// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Identity;

using X.Abp.Identity;

namespace X.Abp.Account.Phone;

public interface IAccountPhoneService
{
    Task SendConfirmationCodeAsync(IdentityUser user, string confirmationToken);

    Task SendSecurityCodeAsync(IdentityUser user, string code);
}
