// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Payment.Requests;

[Serializable]
public class PaymentRequestCreateDto : ExtensibleObject
{
    public string Currency { get; set; }

    public List<PaymentRequestProductCreateDto> Products { get; set; } = new List<PaymentRequestProductCreateDto>();
}
