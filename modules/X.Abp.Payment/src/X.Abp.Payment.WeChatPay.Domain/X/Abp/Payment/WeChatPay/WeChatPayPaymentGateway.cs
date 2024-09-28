// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay.V3.Domain;
using Essensoft.Paylink.WeChatPay.V3.Request;

using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

using X.Abp.Payment.Gateways;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.WeChatPay
{
    public class WeChatPayPaymentGateway : IPaymentGateway, ITransientDependency
    {
        protected IWeChatPayClient WeChatPayClient { get; }

        protected WeChatPayOptions WeChatPayOptions { get; }

        // protected IPurchaseParameterListGenerator PurchaseParameterListGenerator { get; }
        protected IPaymentRequestRepository PaymentRequestRepository { get; }

        public WeChatPayPaymentGateway(
            IWeChatPayClient weChatPayClient,
            IOptions<WeChatPayOptions> weChatPayOptions,
            IPaymentRequestRepository repository)
        {
            WeChatPayClient = weChatPayClient;
            WeChatPayOptions = weChatPayOptions.Value;

            // PurchaseParameterListGenerator = parameters;
            PaymentRequestRepository = repository;
        }

        public async Task<PaymentRequestStartResult> StartAsync(PaymentRequest paymentRequest, PaymentRequestStartInput input)
        {
            // PurchaseParameterListGenerator.GetExtraParameterConfiguration(paymentRequest);
            var model = new WeChatPayTransactionsNativeBodyModel
            {
                AppId = WeChatPayOptions.AppId,
                MchId = WeChatPayOptions.MchId,
                Amount = new Amount { Total = (int)paymentRequest.Products.Sum(p => p.TotalPrice) * 100, Currency = "CNY" }, // WeChatPayOptions.Currency
                Description = string.Join(" ", paymentRequest.Products.Select(p => p.Name)),
                NotifyUrl = input.ReturnUrl,
                OutTradeNo = paymentRequest.Id.ToString("N"),
            };

            var request = new WeChatPayTransactionsNativeRequest();
            request.SetBodyModel(model);

            var response = await WeChatPayClient.ExecuteAsync(request, WeChatPayOptions);
            if (!response.IsError)
            {
                // response.CodeUrl 给前端生成二维码
                return new PaymentRequestStartResult()
                {
                    CheckoutLink = response.CodeUrl
                };
            }

            throw new Exception(response.Message);
        }

        public Task<PaymentRequest> CompleteAsync(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public Task HandleWebhookAsync(string payload, Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(PaymentRequest paymentRequest, Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }
}
