// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Forms;

[DependsOn(
    typeof(AbpFormsDomainModule),
    typeof(AbpFormsApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpEmailingModule))]
public class AbpFormsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpFormsApplicationModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpFormsApplicationModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpFormsApplicationModule>(validate: true);
        });
    }
}
