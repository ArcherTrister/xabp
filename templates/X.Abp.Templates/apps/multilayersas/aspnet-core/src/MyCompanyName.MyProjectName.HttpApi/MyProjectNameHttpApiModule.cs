using Localization.Resources.AbpUi;

using MyCompanyName.MyProjectName.Localization;

using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

using X.Abp.Account;
using X.Abp.AuditLogging;
using X.Abp.FileManagement;
using X.Abp.Gdpr;
using X.Abp.Identity;
#if IdentityServer4
using X.Abp.IdentityServer;
#else
using X.Abp.OpenIddict;
#endif
using X.Abp.LanguageManagement;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpIdentityProHttpApiModule),
#if IdentityServer4
    typeof(AbpIdentityServerProHttpApiModule),
#else
    typeof(AbpOpenIddictProHttpApiModule),
#endif
    typeof(AbpAccountAdminHttpApiModule),
    typeof(AbpAccountPublicHttpApiModule),
    typeof(AbpTextTemplateManagementHttpApiModule),
    typeof(AbpAuditLoggingHttpApiModule),
    typeof(AbpLanguageManagementHttpApiModule),
    typeof(AbpFileManagementHttpApiModule),
    typeof(AbpSaasHttpApiModule),
    typeof(AbpGdprHttpApiModule))]
public class MyProjectNameHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<MyProjectNameResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
