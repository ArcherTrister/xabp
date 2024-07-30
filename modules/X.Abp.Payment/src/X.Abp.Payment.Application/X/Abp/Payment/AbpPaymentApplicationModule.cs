// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace X.Abp.Payment;

[DependsOn(
    typeof(AbpPaymentDomainModule),
    typeof(AbpAutoMapperModule))]
public class AbpPaymentApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpPaymentApplicationModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpPaymentApplicationAutoMapperProfile>(true));
    }
}
