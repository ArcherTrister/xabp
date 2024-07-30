# abp-localization

Localization module for ABP framework.

## ģ��˵��

������ڶ��ָ�ʽ�������Ա��ػ�����  

See: https://github.com/maliming/Owl.Abp.CultureMap

### ���ʹ��

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
