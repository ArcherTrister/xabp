// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Payment.Requests;

public class CompletePaymentRequestDto : ExtensibleObject
{
    public Guid Id { get; set; }

    public string GateWay { get; set; }

    public bool IsSubscription { get; set; }

    public CompletePaymentRequestSubscriptionDto SubscriptionInfo { get; set; }
}
