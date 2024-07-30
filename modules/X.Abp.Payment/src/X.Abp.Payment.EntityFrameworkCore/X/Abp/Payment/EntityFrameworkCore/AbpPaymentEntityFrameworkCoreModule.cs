// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using X.Abp.Payment.Plans;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.EntityFrameworkCore;

[DependsOn(
    typeof(AbpPaymentDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpPaymentEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<PaymentDbContext>(options =>
        {
            options.AddDefaultRepositories<IPaymentDbContext>();

            options.AddRepository<PaymentRequest, EfCorePaymentRequestRepository>();
            options.AddRepository<Plan, EfCorePlanRepository>();
        });
    }
}
