// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Gateways;

public interface IPaymentGateway
{
    bool IsValid(PaymentRequest paymentRequest, Dictionary<string, string> properties);

    Task<PaymentRequestStartResult> StartAsync(
      PaymentRequest paymentRequest,
      PaymentRequestStartInput input);

    Task<PaymentRequest> CompleteAsync(Dictionary<string, string> parameters);

    Task HandleWebhookAsync(string payload, Dictionary<string, string> headers);
}
