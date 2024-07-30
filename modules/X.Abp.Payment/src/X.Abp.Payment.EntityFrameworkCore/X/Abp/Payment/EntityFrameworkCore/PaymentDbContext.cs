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
public class PaymentDbContext : AbpDbContext<PaymentDbContext>, IEfCoreDbContext, IPaymentDbContext
{
    public DbSet<PaymentRequest> PaymentRequests { get; set; }

    public DbSet<Plan> Plans { get; set; }

    public DbSet<GatewayPlan> GatewayPlans { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigurePayment();
    }
}
