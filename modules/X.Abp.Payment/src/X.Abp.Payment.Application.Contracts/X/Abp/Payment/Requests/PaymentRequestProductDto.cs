// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Data;

namespace X.Abp.Payment.Requests;

[Serializable]
public class PaymentRequestProductDto : IHasExtraProperties
{
    public Guid PaymentRequestId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public float UnitPrice { get; set; }

    public int Count { get; set; }

    public float TotalPrice { get; set; }

    public PaymentType PaymentType { get; private set; }

    public Guid PlanId { get; private set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }

    public PaymentRequestProductDto()
    {
        ExtraProperties = new ExtraPropertyDictionary();
    }
}
