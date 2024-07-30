// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Payment.Admin.Requests;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Requests.Products
{
    public class IndexModel : PaymentPageModel
    {
        protected IPaymentRequestAdminAppService PaymentRequestAppService { get; }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public PaymentRequestWithDetailsDto PaymentRequest { get; set; }

        public IndexModel(
          IPaymentRequestAdminAppService paymentRequestAppService)
        {
            PaymentRequestAppService = paymentRequestAppService;
        }

        public async Task OnGetAsync()
        {
            PaymentRequest = await PaymentRequestAppService.GetAsync(Id);
        }
    }
}
