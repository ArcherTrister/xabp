// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement;

[DependsOn(
    typeof(AbpFileManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class AbpFileManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpFileManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateFileInputWithStream));
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UploadInput));
        });
    }
}
