// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Payment.Gateways;
using X.Abp.Payment.Plans;
using X.Abp.Payment.Requests;
using X.Abp.Payment.Subscription;

namespace X.Abp.Payment;

public class AbpPaymentApplicationAutoMapperProfile : Profile
{
    public AbpPaymentApplicationAutoMapperProfile()
    {
        CreateMap<PaymentRequest, PaymentRequestDto>();
        CreateMap<PaymentRequestProduct, PaymentRequestProductDto>();

        CreateMap<PaymentRequest, PaymentRequestWithDetailsDto>()
            .ForMember(p => p.TotalPrice, opts => opts.MapFrom(paymentRequest => paymentRequest.Products.Sum(paymentRequestProduct => paymentRequestProduct.TotalPrice)));

        CreateMap<PaymentRequest, SubscriptionCreatedEto>()
            .ForMember(p => p.PaymentRequestId, opts => opts.MapFrom(paymentRequest => paymentRequest.Id))
            .Ignore(subscriptionCreatedEto => subscriptionCreatedEto.Properties)
            .Ignore(subscriptionCreatedEto => subscriptionCreatedEto.PeriodEndDate);

        CreateMap<PaymentRequestProduct, PaymentRequestProductCompletedEto>()
            .ForMember(p => p.Properties, opts => opts.MapFrom(
                p => p.ExtraProperties.ToDictionary(s => s.Key, s => s.Value.ToString())));

        CreateMap<PaymentRequest, SubscriptionCreatedEto>()
            .ForMember(subscriptionCreatedEto => subscriptionCreatedEto.PaymentRequestId, opts => opts.MapFrom(paymentRequest => paymentRequest.Id))
            .Ignore(subscriptionCreatedEto => subscriptionCreatedEto.Properties)
            .Ignore(subscriptionCreatedEto => subscriptionCreatedEto.PeriodEndDate);

        CreateMap<PaymentRequest, SubscriptionCanceledEto>()
            .ForMember(subscriptionCanceledEto => subscriptionCanceledEto.PaymentRequestId, opts => opts.MapFrom(paymentRequest => paymentRequest.Id))
            .Ignore(subscriptionCanceledEto => subscriptionCanceledEto.Properties)
            .Ignore(subscriptionCanceledEto => subscriptionCanceledEto.PeriodEndDate);

        CreateMap<PaymentRequest, SubscriptionUpdatedEto>()
            .ForMember(subscriptionUpdatedEto => subscriptionUpdatedEto.PaymentRequestId, opts => opts.MapFrom(paymentRequest => paymentRequest.Id))
            .Ignore(subscriptionUpdatedEto => subscriptionUpdatedEto.Properties)
            .Ignore(subscriptionUpdatedEto => subscriptionUpdatedEto.PeriodEndDate);

        CreateMap<GatewayPlan, GatewayPlanDto>().MapExtraProperties();

        CreateMap<Plan, PlanDto>().MapExtraProperties();

        CreateMap<PaymentRequestStartResult, PaymentRequestStartResultDto>().MapExtraProperties();

        CreateMap<PaymentRequestStartInput, PaymentRequestStartDto>().MapExtraProperties().ReverseMap();
    }
}
