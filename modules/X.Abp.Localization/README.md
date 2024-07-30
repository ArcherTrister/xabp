# abp-localization

Localization module for ABP framework.

## 模块说明

解决存在多种格式的区域性本地化问题  

See: https://github.com/maliming/Owl.Abp.CultureMap

### 如何使用

```csharp

    [DependsOn(
        typeof(AbpLocalizationCultureMapModule))]
    public class YouProjectModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationCultureMapOptions>(options =>
            {
                var zhHantCultureMapInfo = new CultureMapInfo
                {
                    TargetCulture = "zh-Hant",
                    SourceCultures = new string[] { "zh_tw", "zh-TW", "zh_hk", "zh-HK" }
                };

                options.CulturesMaps.Add(zhHantCultureMapInfo);
                options.UiCulturesMaps.Add(zhHantCultureMapInfo);
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            IApplicationBuilder app = context.GetApplicationBuilder();

            // app.UseAbpRequestLocalization();
            app.UseMapRequestLocalization();
        }
    }

```
