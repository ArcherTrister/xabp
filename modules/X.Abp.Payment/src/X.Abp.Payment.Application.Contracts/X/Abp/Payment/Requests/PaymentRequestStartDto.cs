// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Payment.Requests;

[Serializable]
public class PaymentRequestStartDto : ExtensibleObject
{
    public Guid PaymentRequestId { get; set; }

    [Required]
    public string ReturnUrl { get; set; }

    public string CancelUrl { get; set; }
}
