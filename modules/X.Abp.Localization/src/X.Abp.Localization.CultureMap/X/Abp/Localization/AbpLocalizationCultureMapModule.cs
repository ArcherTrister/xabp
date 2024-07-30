// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Builder;

using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace X.Abp.Localization
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpLocalizationCultureMapModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationCultureMapOptions>(options =>
            {
                var zhHansCultureMapInfo = new CultureMapInfo
                {
                    TargetCulture = "zh-Hans",
                    SourceCultures = new string[] { "zh", "zh_CN", "zh_cn", "zh-CN", "zh-CN" }
                };

                options.CulturesMaps.Add(zhHansCultureMapInfo);
                options.UiCulturesMaps.Add(zhHansCultureMapInfo);
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            app.UseMapRequestLocalization();
        }
    }
}
