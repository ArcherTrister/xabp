// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;
using Volo.Abp.Ui.LayoutHooks;

using X.Abp.Gdpr.Web.Pages.Gdpr.Components.CookieConsent;

namespace X.Abp.Gdpr.Web.Extensions;

public static class AbpGdprServiceCollectionExtensions
{
    public static IServiceCollection AddAbpCookieConsent(
      this IServiceCollection services,
      Action<AbpCookieConsentOptions> cookieConsentAction = null)
    {
        Check.NotNull(services, nameof(services));
        services.Configure<AbpLayoutHookOptions>(hookOptions => hookOptions.Add("Body.First", typeof(AbpCookieConsentViewComponent), "Application"));
        services.AddCookiePolicy(options => options.CheckConsentNeeded = context => true);
        return services.Configure<AbpCookieConsentOptions>(options =>
        {
            cookieConsentAction?.Invoke(options);
        });
    }
}
