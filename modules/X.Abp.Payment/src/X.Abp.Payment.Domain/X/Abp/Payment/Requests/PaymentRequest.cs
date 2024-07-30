// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.Payment.Requests;

public class PaymentRequest : CreationAuditedAggregateRoot<Guid>
{
    public virtual ICollection<PaymentRequestProduct> Products { get; protected set; }

    public PaymentRequestState State { get; private set; }

    public string Currency { get; set; }

    public string Gateway { get; set; }

    public string FailReason { get; private set; }

    public DateTime? EmailSendDate { get; set; }

    public string ExternalSubscriptionId { get; protected set; }

    private PaymentRequest()
    {
    }

    public PaymentRequest(Guid id, string currency = null)
    {
        Id = id;
        Products = new List<PaymentRequestProduct>();
        Currency = currency;
    }

    public void SetExternalSubscriptionId(string externalSubscriptionId)
    {
        ExternalSubscriptionId = ExternalSubscriptionId.IsNullOrEmpty() || !(ExternalSubscriptionId != externalSubscriptionId) ? Check.NotNullOrEmpty(externalSubscriptionId, nameof(externalSubscriptionId)) : throw new BusinessException(AbpPaymentErrorCodes.Requests.CantUpdateExternalSubscriptionId);
    }

    public PaymentRequestProduct AddProduct(
      string code,
      string name,
      PaymentType paymentType = PaymentType.OneTime,
      float unitPrice = 0.0f,
      int count = 1,
      Guid? planId = null,
      float? totalPrice = null,
      Dictionary<string, IPaymentRequestProductExtraParameterConfiguration> extraProperties = null)
    {
        if (paymentType == PaymentType.OneTime && Currency.IsNullOrEmpty())
        {
            throw new BusinessException(AbpPaymentErrorCodes.CurrencyMustBeSet, "Currency of this PaymentRequest is empty. It must be set to continue with OneTime Payment.");
        }

        var source = new PaymentRequestProduct(Id, code, name, paymentType, new float?(unitPrice), count, planId, totalPrice);
        if (extraProperties != null)
        {
            foreach (var extraProperty in extraProperties)
            {
                source.SetProperty(extraProperty.Key, extraProperty.Value);
            }
        }

        Products.Add(source);
        return source;
    }

    public void Waiting()
    {
        State = PaymentRequestState.Waiting;
    }

    public void Complete()
    {
        State = State == PaymentRequestState.Waiting ? PaymentRequestState.Completed : throw new ApplicationException(string.Format("Can not complete a payment in '{0}' state!", State));
    }

    public void Failed(string reason = null)
    {
        State = State == PaymentRequestState.Waiting ? PaymentRequestState.Failed : throw new ApplicationException(string.Format("Can not fail a payment in '{0}' state!", State));
        FailReason = reason;
    }

    public void Refunded()
    {
        State = State == PaymentRequestState.Completed ? PaymentRequestState.Refunded : throw new ApplicationException(string.Format("Can not refund a payment in '{0}' state!", State));
    }

    public void SetState(PaymentRequestState state)
    {
        switch (state)
        {
            case PaymentRequestState.Waiting:
                Waiting();
                break;
            case PaymentRequestState.Completed:
                Complete();
                break;
            case PaymentRequestState.Failed:
                Failed();
                break;
            case PaymentRequestState.Refunded:
                Refunded();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
