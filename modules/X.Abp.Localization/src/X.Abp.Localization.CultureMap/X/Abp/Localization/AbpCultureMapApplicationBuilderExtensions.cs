// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using X.Abp.Localization;

namespace Microsoft.AspNetCore.Builder;
public static class AbpCultureMapApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMapRequestLocalization(
        this IApplicationBuilder app,
        Action<RequestLocalizationOptions> optionsAction = null)
    {
        return app.UseAbpRequestLocalization(options =>
        {
            options.RequestCultureProviders.Insert(0, new AbpCultureMapRequestCultureProvider());
            optionsAction?.Invoke(options);
        });
    }
}
