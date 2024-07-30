// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpGdprApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class AbpGdprHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(builder => builder.AddApplicationPartIfNotExists(typeof(AbpGdprHttpApiModule).Assembly));
    }
}
