// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Requests
{
    public class PaymentRequestGetListInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public DateTime? CreationDateMax { get; set; }

        public DateTime? CreationDateMin { get; set; }

        public PaymentType? PaymentType { get; set; }

        public PaymentRequestState? Status { get; set; }
    }
}
