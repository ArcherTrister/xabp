// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Payment.Requests;

[Serializable]
public class PaymentRequestDto : ExtensibleEntityDto<Guid>
{
    public string Currency { get; set; }

    public PaymentRequestState State { get; set; }

    public string FailReason { get; set; }

    public DateTime? EmailSendDate { get; set; }

    public string Gateway { get; set; }

    public string ExternalSubscriptionId { get; set; }
}
