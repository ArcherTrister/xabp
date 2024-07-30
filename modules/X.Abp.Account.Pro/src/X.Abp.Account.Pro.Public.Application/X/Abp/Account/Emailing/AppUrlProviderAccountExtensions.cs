// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.UI.Navigation.Urls;

namespace X.Abp.Account.Emailing;

public static class AppUrlProviderAccountExtensions
{
    public static Task<string> GetResetPasswordUrlAsync(this IAppUrlProvider appUrlProvider, string appName)
    {
        return appUrlProvider.GetUrlAsync(appName, AccountUrlNames.PasswordReset);
    }

    public static Task<string> GetEmailConfirmationUrlAsync(this IAppUrlProvider appUrlProvider, string appName)
    {
        return appUrlProvider.GetUrlAsync(appName, AccountUrlNames.EmailConfirmation);
    }
}
