// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace X.Abp.Payment.Requests;

[EventName("X.Abp.Payment.PaymentRequestCompleted")]
[Serializable]
public class PaymentRequestCompletedEto : EtoBase, IHasExtraProperties
{
    public Guid Id { get; }

    public string Gateway { get; set; }

    public string Currency { get; set; }

    public List<PaymentRequestProductCompletedEto> Products { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; }

    private PaymentRequestCompletedEto()
    {
    }

    public PaymentRequestCompletedEto(
      Guid id,
      string gateway,
      string currency,
      List<PaymentRequestProductCompletedEto> products,
      ExtraPropertyDictionary extraProperties = null)
    {
        Id = id;
        Gateway = gateway;
        Currency = currency;
        Products = products;
        ExtraProperties = extraProperties ?? new ExtraPropertyDictionary();
    }
}
