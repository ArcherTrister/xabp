// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using X.Abp.Gdpr;

namespace Microsoft.AspNetCore.Builder;

public static class AbpGdprApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAbpCookieConsent(this IApplicationBuilder app)
    {
        var service = app.ApplicationServices.GetService<IOptions<AbpCookieConsentOptions>>();
        if (service != null && service.Value.IsEnabled)
        {
            app.UseCookiePolicy();
        }

        return app;
    }
}
