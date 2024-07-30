// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement.EntityFrameworkCore;

[DependsOn(
    typeof(AbpFileManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpFileManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<FileManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<IFileManagementDbContext>();

            options.AddRepository<DirectoryDescriptor, EfCoreDirectoryDescriptorRepository>();
            options.AddRepository<FileDescriptor, EfCoreFileDescriptorRepository>();
        });
    }
}
