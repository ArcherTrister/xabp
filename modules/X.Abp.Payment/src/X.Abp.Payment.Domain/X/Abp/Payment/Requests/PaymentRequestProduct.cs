// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Payment.Requests;

public class PaymentRequestProduct : Entity, IHasExtraProperties
{
    public Guid PaymentRequestId { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public float UnitPrice { get; private set; }

    public int Count { get; private set; }

    public float TotalPrice { get; private set; }

    public PaymentType PaymentType { get; private set; }

    public Guid? PlanId { get; private set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    private PaymentRequestProduct()
    {
        ExtraProperties = new ExtraPropertyDictionary();
    }

    internal PaymentRequestProduct(
      Guid paymentRequestId,
      string code,
      string name,
      PaymentType paymentType = PaymentType.OneTime,
      float? unitPrice = null,
      int count = 1,
      Guid? planId = null,
      float? totalPrice = null)
    {
        PaymentRequestId = paymentRequestId;
        Code = Check.NotNullOrWhiteSpace(code, nameof(code));
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        if (paymentType == PaymentType.Subscription && !planId.HasValue)
        {
            throw new ArgumentNullException(nameof(planId), string.Format("{0} is required when payment type is {1}", nameof(planId), PaymentType.Subscription));
        }

        if (paymentType == PaymentType.OneTime && !unitPrice.HasValue)
        {
            throw new ArgumentNullException(nameof(unitPrice), string.Format("{0} is required when payment type is {1}", nameof(unitPrice), PaymentType.OneTime));
        }

        UnitPrice = unitPrice.Value;
        Count = count;
        TotalPrice = totalPrice ?? UnitPrice * Count;
        PlanId = planId;
        PaymentType = paymentType;
        ExtraProperties = new ExtraPropertyDictionary();
    }

    public override object[] GetKeys()
    {
        return new object[]
        {
            PaymentRequestId, Code
        };
    }
}
