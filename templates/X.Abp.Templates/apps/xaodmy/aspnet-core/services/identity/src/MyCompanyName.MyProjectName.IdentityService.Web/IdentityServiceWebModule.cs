using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Identity.Web;
using X.Abp.OpenIddict.Pro.Web;

namespace MyCompanyName.MyProjectName.IdentityService.Web;

[DependsOn(
    typeof(AbpIdentityProWebModule),
    typeof(AbpOpenIddictProWebModule),
    typeof(IdentityServiceApplicationContractsModule))]
public class IdentityServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<IdentityServiceWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<IdentityServiceWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<IdentityServiceWebModule>(validate: true);
        });
    }
}
