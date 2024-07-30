// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

using X.Abp.Payment.Plans;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.EntityFrameworkCore;

[ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
[IgnoreMultiTenancy]
public interface IPaymentDbContext : IEfCoreDbContext
{
    DbSet<PaymentRequest> PaymentRequests { get; }

    DbSet<Plan> Plans { get; }

    DbSet<GatewayPlan> GatewayPlans { get; }
}
