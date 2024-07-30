// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.TextTemplateManagement;

[DependsOn(typeof(AbpTextTemplateManagementDomainSharedModule), typeof(AbpDddApplicationContractsModule), typeof(AbpAuthorizationModule))]
public class AbpTextTemplateManagementApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpTextTemplateManagementApplicationContractsModule>());
    }
}
