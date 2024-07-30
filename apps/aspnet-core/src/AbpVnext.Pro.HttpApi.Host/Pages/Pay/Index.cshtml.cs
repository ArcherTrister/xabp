// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using X.Abp.Payment.Requests;

namespace AbpVnext.Pro.Pages.Pay
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentRequestAppService _paymentRequestAppService;

        public IndexModel(IPaymentRequestAppService paymentRequestAppService)
        {
            _paymentRequestAppService = paymentRequestAppService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var paymentRequest = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreateDto()
            {
                Currency = "USD",
                Products = new List<PaymentRequestProductCreateDto>()
            {
                new PaymentRequestProductCreateDto
                {
                    Code = "Product_01",
                    Name = "LEGO Super Mario",
                    Count = 2,
                    UnitPrice = 10,
                    TotalPrice = 20
                }
            }
            });

            return LocalRedirectPreserveMethod("/Payment/GatewaySelection?paymentRequestId=" + paymentRequest.Id);
        }
    }
}
