// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Subscription;

[EventName("X.Abp.Payment.RecurringPaymentUpdated")]
[Serializable]
public class SubscriptionUpdatedEto : EtoBase, IHasExtraProperties
{
    public Guid PaymentRequestId { get; set; }

    public PaymentRequestState State { get; set; }

    public string Currency { get; set; }

    public string Gateway { get; set; }

    public string ExternalSubscriptionId { get; set; }

    public DateTime PeriodEndDate { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }
}
