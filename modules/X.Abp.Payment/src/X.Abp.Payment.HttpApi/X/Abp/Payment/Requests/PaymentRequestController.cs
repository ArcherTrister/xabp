// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

namespace X.Abp.Payment.Requests;

[Route("api/payment")]
[RemoteService(true, Name = AbpPaymentCommonRemoteServiceConsts.RemoteServiceName)]
[Area(AbpPaymentCommonRemoteServiceConsts.ModuleName)]
public class PaymentRequestController :
PaymentCommonController,
IPaymentRequestAppService
{
    protected IPaymentRequestAppService PaymentRequestAppService { get; }

    public PaymentRequestController(IPaymentRequestAppService paymentRequestAppService)
    {
        PaymentRequestAppService = paymentRequestAppService;
    }

    [Route("{paymentMethod}/complete")]
    [HttpPost]
    public Task<PaymentRequestWithDetailsDto> CompleteAsync(
      string paymentGateway,
      Dictionary<string, string> parameters)
    {
        return PaymentRequestAppService.CompleteAsync(paymentGateway, parameters);
    }

    [HttpPost]
    [Route("requests")]
    public Task<PaymentRequestWithDetailsDto> CreateAsync(
      PaymentRequestCreateDto input)
    {
        return PaymentRequestAppService.CreateAsync(input);
    }

    [Route("requests/{id}")]
    [HttpGet]
    public Task<PaymentRequestWithDetailsDto> GetAsync(Guid id)
    {
        return PaymentRequestAppService.GetAsync(id);
    }

    [HttpPost]
    [Route("{paymentMethod}/webhook")]
    public Task<bool> HandleWebhookAsync(
      string paymentGateway,
      string payload,
      [FromHeader] Dictionary<string, string> headers)
    {
        return PaymentRequestAppService.HandleWebhookAsync(paymentGateway, payload, Request.Headers.ToDictionary(k => k.Key, v => v.Value.ToString()));
    }

    [Route("{paymentMethod}/start")]
    [HttpPost]
    public Task<PaymentRequestStartResultDto> StartAsync(
      string paymentGateway,
      PaymentRequestStartDto input)
    {
        return PaymentRequestAppService.StartAsync(paymentGateway, input);
    }
}
