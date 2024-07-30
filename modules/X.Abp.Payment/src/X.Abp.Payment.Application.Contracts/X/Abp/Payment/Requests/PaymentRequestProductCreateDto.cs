// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.Payment.Requests;

[Serializable]
public class PaymentRequestProductCreateDto
{
    [Required]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; }

    public float UnitPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Count { get; set; }

    public float? TotalPrice { get; set; }

    public PaymentType PaymentType { get; set; }

    public Guid? PlanId { get; set; }

    public Dictionary<string, IPaymentRequestProductExtraParameterConfiguration> ExtraProperties { get; set; }

    public PaymentRequestProductCreateDto()
    {
        ExtraProperties = new Dictionary<string, IPaymentRequestProductExtraParameterConfiguration>();
    }
}
