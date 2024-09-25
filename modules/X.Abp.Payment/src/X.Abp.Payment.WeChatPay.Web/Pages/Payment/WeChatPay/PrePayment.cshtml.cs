// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.Payment.Requests;
using X.Abp.Payment.Web;

namespace X.Abp.Payment.WeChatPay.Pages.Payment.WeChatPay
{
    public class PrePaymentModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PaymentRequestId { get; set; }

        public PaymentRequestWithDetailsDto PaymentRequest { get; set; }

        public PaymentRequestStartResultDto PaymentRequestStartResult { get; set; }

        protected PaymentWebOptions PaymentWebOptions { get; }

        protected IPaymentRequestAppService PaymentRequestAppService { get; }

        public PrePaymentModel(
          IOptions<PaymentWebOptions> paymentWebOptions,
          IPaymentRequestAppService paymentRequestAppService)
        {
            PaymentWebOptions = paymentWebOptions.Value;
            PaymentRequestAppService = paymentRequestAppService;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            PaymentRequest = await PaymentRequestAppService.GetAsync(PaymentRequestId);

            PaymentRequestStartResult = await PaymentRequestAppService.StartAsync(WeChatPayConsts.GatewayName, new PaymentRequestStartDto()
            {
                PaymentRequestId = PaymentRequestId,
                ReturnUrl = PaymentWebOptions.RootUrl.RemovePostFix("/") + WeChatPayConsts.PostPaymentUrl,
                CancelUrl = PaymentWebOptions.RootUrl
            });

            return Page();
        }

        public virtual IActionResult OnPost()
        {
            /*
            PaymentRequest = await PaymentRequestAppService.GetAsync(PaymentRequestId);

            PaymentRequestStartResultDto requestStartResultDto = await PaymentRequestAppService.StartAsync(WeChatPayConsts.GatewayName, new PaymentRequestStartDto()
            {
                PaymentRequestId = PaymentRequestId,
                ReturnUrl = PaymentWebOptions.RootUrl.RemovePostFix("/") + WeChatPayConsts.PostPaymentUrl,
                CancelUrl = PaymentWebOptions.RootUrl
            });
            Response.Redirect(requestStartResultDto.CheckoutLink);
            */
            return BadRequest();
        }
    }
}
