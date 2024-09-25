// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using X.Abp.Payment.Requests;
using X.Abp.Payment.Web;

namespace X.Abp.Payment.WeChatPay.Pages.Payment.WeChatPay
{
    public class PostPaymentModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        protected IOptions<PaymentWebOptions> PaymentWebOptions { get; }

        protected IPaymentRequestAppService PaymentRequestAppService { get; }

        protected ILogger<PostPaymentModel> Logger { get; set; }

        public PostPaymentModel(
          IPaymentRequestAppService paymentRequestAppService,
          IOptions<PaymentWebOptions> paymentWebOptions)
        {
            PaymentRequestAppService = paymentRequestAppService;
            PaymentWebOptions = paymentWebOptions;
            Logger = NullLogger<PostPaymentModel>.Instance;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            if (Token.IsNullOrWhiteSpace())
            {
                return BadRequest();
            }

            PaymentRequestWithDetailsDto requestWithDetailsDto = await PaymentRequestAppService.CompleteAsync(
                WeChatPayConsts.GatewayName,
                new Dictionary<string, string>
                {
                    { "Token", Token }
                });

            if (!PaymentWebOptions.Value.CallbackUrl.IsNullOrWhiteSpace())
            {
                Response.Redirect($"{PaymentWebOptions.Value.CallbackUrl}?paymentRequestId={requestWithDetailsDto.Id}");
            }

            return Page();
        }

        public virtual IActionResult OnPost()
        {
            return BadRequest();
        }
    }
}
