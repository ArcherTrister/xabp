// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.Payment.Plans;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.EntityFrameworkCore;

public static class PaymentDbContextModelBuilderExtensions
{
    public static void ConfigurePayment(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        if (builder.IsTenantOnlyDatabase())
        {
            return;
        }

        builder.Entity<PaymentRequest>(b =>
        {
            b.ToTable(PaymentDbProperties.DbTablePrefix + "PaymentRequests", PaymentDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(paymentRequest => paymentRequest.State).IsRequired();
            b.Property(paymentRequest => paymentRequest.FailReason);
            b.HasMany(paymentRequest => paymentRequest.Products).WithOne().HasForeignKey(paymentRequestProduct => paymentRequestProduct.PaymentRequestId).IsRequired(true);
            b.HasIndex(paymentRequest => paymentRequest.CreationTime);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<PaymentRequestProduct>(b =>
        {
            b.ToTable(PaymentDbProperties.DbTablePrefix + "PaymentRequestProducts", PaymentDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(paymentRequestProduct => new
            {
                paymentRequestProduct.PaymentRequestId,
                paymentRequestProduct.Code
            });
            b.HasOne<PaymentRequest>().WithMany(paymentRequest => paymentRequest.Products).HasForeignKey(paymentRequestProduct => paymentRequestProduct.PaymentRequestId).IsRequired(true);
            b.Property(paymentRequestProduct => paymentRequestProduct.Code).IsRequired(true);
            b.Property(paymentRequestProduct => paymentRequestProduct.Name).IsRequired(true);
            b.Property(paymentRequestProduct => paymentRequestProduct.UnitPrice).IsRequired(true);
            b.Property(paymentRequestProduct => paymentRequestProduct.Count).IsRequired(true);
            b.Property(paymentRequestProduct => paymentRequestProduct.TotalPrice);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<Plan>(b =>
        {
            b.ToTable(PaymentDbProperties.DbTablePrefix + "Plans", PaymentDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(plan => plan.Name).HasMaxLength(PlanConsts.MaxNameLength).IsRequired(true);
            b.HasMany(plan => plan.GatewayPlans).WithOne().HasForeignKey(gatewayPlan => gatewayPlan.PlanId);
        });

        builder.Entity<GatewayPlan>(b =>
        {
            b.ToTable(PaymentDbProperties.DbTablePrefix + "GatewayPlans", PaymentDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(gatewayPlan => new
            {
                gatewayPlan.PlanId,
                gatewayPlan.Gateway
            });
            b.Property(gatewayPlan => gatewayPlan.Gateway).IsRequired(true);
            b.Property(gatewayPlan => gatewayPlan.ExternalId).IsRequired(true);
        });

        builder.TryConfigureObjectExtensions<PaymentDbContext>();
    }
}
