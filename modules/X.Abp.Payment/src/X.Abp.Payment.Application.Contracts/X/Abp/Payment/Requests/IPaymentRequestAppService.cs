// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.Payment.Requests;

public interface IPaymentRequestAppService : IApplicationService
{
    Task<PaymentRequestWithDetailsDto> CreateAsync(
      PaymentRequestCreateDto input);

    Task<PaymentRequestWithDetailsDto> GetAsync(Guid id);

    Task<PaymentRequestStartResultDto> StartAsync(
      string paymentGateway,
      PaymentRequestStartDto input);

    Task<PaymentRequestWithDetailsDto> CompleteAsync(
      string paymentGateway,
      Dictionary<string, string> parameters);

    Task<bool> HandleWebhookAsync(
      string paymentGateway,
      string payload,
      Dictionary<string, string> headers);
}
