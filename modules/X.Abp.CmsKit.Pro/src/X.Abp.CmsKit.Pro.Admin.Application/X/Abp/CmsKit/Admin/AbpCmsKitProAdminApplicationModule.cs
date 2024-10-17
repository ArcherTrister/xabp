// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.CmsKit.Admin;

namespace X.Abp.CmsKit.Admin;

[DependsOn(
    typeof(AbpCmsKitProDomainModule),
    typeof(AbpCmsKitProAdminApplicationContractsModule),
    typeof(CmsKitAdminApplicationModule))]
public class AbpCmsKitProAdminApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpCmsKitProAdminApplicationModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpCmsKitProAdminApplicationModule>(true));
    }
}
