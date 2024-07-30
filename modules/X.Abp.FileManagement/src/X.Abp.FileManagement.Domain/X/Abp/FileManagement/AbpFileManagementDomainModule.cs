// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement;

[DependsOn(
    typeof(AbpBlobStoringModule),
    typeof(AbpFileManagementDomainSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpFileManagementDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpFileManagementDomainModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpFileManagementDomainMappingProfile>(validate: true);
        });

        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            options.EtoMappings.Add<FileDescriptor, FileDescriptorEto>(typeof(AbpFileManagementDomainModule));
            options.EtoMappings.Add<DirectoryDescriptor, DirectoryDescriptorEto>(typeof(AbpFileManagementDomainModule));
        });
    }
}
