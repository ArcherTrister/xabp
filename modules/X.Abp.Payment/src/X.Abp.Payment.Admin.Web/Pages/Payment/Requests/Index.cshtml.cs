// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Requests
{
    public class IndexModel : PaymentPageModel
    {
        [Display(Name = "PaymentRequests:Search")]
        public string Filter { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreationDateMax { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreationDateMin { get; set; }

        [Display(Name = "PaymentRequests:PaymentType")]
        public PaymentType? PaymentType { get; set; }

        [Display(Name = "PaymentRequests:Status")]
        public PaymentRequestState? Status { get; set; }
    }
}
