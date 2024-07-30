// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement;

[DependsOn(
    typeof(AbpVersionManagementDomainModule),
    typeof(AbpVersionManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
[DependsOn(typeof(AbpBlobStoringModule))]
public class AbpVersionManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpVersionManagementApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpVersionManagementApplicationModule>(validate: true);
        });

        // Configure<AbpBlobStoringOptions>(options =>
        // {
        //     options.Containers.Configure<ApplicationFileContainer>(container =>
        //     {
        //         container.IsMultiTenant = true;
        //     });
        // });
    }
}
