// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement.EntityFrameworkCore;

[DependsOn(
    typeof(AbpTextTemplateManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpTextTemplateManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<TextTemplateManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<ITextTemplateManagementDbContext>();

            options.AddRepository<TextTemplateContent, EfCoreTextTemplateContentRepository>();
        });
    }
}
