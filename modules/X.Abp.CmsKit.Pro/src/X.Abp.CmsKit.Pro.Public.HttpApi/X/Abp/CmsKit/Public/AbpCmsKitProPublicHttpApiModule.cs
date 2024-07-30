// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Modularity;
using Volo.CmsKit.Public;

namespace X.Abp.CmsKit.Public;

[DependsOn(
    typeof(AbpCmsKitProPublicApplicationContractsModule),
    typeof(CmsKitPublicHttpApiModule))]
public class AbpCmsKitProPublicHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(source => source.AddApplicationPartIfNotExists(typeof(AbpCmsKitProPublicHttpApiModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.TokenCookie.Expiration = new TimeSpan?(TimeSpan.FromDays(365.0));
            options.AutoValidateIgnoredHttpMethods.Add("POST");
        });
    }
}
