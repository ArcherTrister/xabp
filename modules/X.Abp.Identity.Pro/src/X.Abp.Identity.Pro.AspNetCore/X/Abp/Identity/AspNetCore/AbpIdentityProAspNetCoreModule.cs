// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Identity;

using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;

namespace X.Abp.Identity.AspNetCore;

[DependsOn(typeof(AbpIdentityProDomainModule), typeof(AbpIdentityAspNetCoreModule))]
public class AbpIdentityProAspNetCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IdentityBuilder>(builder =>
        {
            builder.AddTokenProvider<ScanCodeUserTokenProvider>(ScanCodeUserTokenProviderConsts.ScanCodeUserTokenProviderName);
        });
    }
}
