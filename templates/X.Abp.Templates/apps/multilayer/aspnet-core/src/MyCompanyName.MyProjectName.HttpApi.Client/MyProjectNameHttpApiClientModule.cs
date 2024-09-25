using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;

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
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpIdentityProHttpApiClientModule),
#if IdentityServer4
    typeof(AbpIdentityServerProHttpApiClientModule),
#else
    typeof(AbpOpenIddictProHttpApiClientModule),
#endif
    typeof(AbpAccountAdminHttpApiClientModule),
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpSaasHttpApiClientModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpTextTemplateManagementHttpApiClientModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpFileManagementHttpApiClientModule),
    typeof(AbpGdprHttpApiClientModule))]
public class MyProjectNameHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(
            typeof(MyProjectNameApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNameHttpApiClientModule>();
        });
    }
}
