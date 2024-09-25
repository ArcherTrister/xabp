// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;

using X.Abp.Payment.Gateways;

namespace X.Abp.Payment.Requests;

public class PaymentRequestAppService :
PaymentAppServiceBase,
IPaymentRequestAppService
{
  protected IPaymentRequestRepository PaymentRequestRepository { get; }

  protected PaymentGatewayResolver PaymentGatewayResolver { get; }

  protected IDistributedEventBus DistributedEventBus { get; }

  public PaymentRequestAppService(
    IPaymentRequestRepository paymentRequestRepository,
    PaymentGatewayResolver paymentMethodResolver,
    IDistributedEventBus distributedEventBus)
  {
    PaymentRequestRepository = paymentRequestRepository;
    PaymentGatewayResolver = paymentMethodResolver;
    DistributedEventBus = distributedEventBus;
  }

  public virtual async Task<PaymentRequestWithDetailsDto> CreateAsync(
    PaymentRequestCreateDto input)
  {
    var paymentRequest = new PaymentRequest(GuidGenerator.Create(), input.Currency);
    foreach (var extraProperty in input.ExtraProperties)
    {
      paymentRequest.SetProperty(extraProperty.Key, extraProperty.Value);
    }

    foreach (var product in input.Products)
    {
      paymentRequest.AddProduct(product.Code, product.Name, product.PaymentType, product.UnitPrice, product.Count, product.PlanId, product.TotalPrice, product.ExtraProperties);
    }

    var source = await PaymentRequestRepository.InsertAsync(paymentRequest, true);
    return ObjectMapper.Map<PaymentRequest, PaymentRequestWithDetailsDto>(source);
  }

  public virtual async Task<PaymentRequestWithDetailsDto> GetAsync(Guid id)
  {
    return await GetPaymentRequestWithDetailsDtoAsync(await PaymentRequestRepository.GetAsync(id));
  }

  public virtual async Task<PaymentRequestStartResultDto> StartAsync(
    string paymentGateway,
    PaymentRequestStartDto input)
  {
    var paymentRequest = await PaymentRequestRepository.GetAsync(input.PaymentRequestId);
    paymentRequest.Gateway = paymentGateway;
    await PaymentRequestRepository.UpdateAsync(paymentRequest);

    var requestStartInput = ObjectMapper.Map<PaymentRequestStartDto, PaymentRequestStartInput>(input);
    Replacement(input, requestStartInput);
    var requestStartResult = await PaymentGatewayResolver.Resolve(paymentGateway).StartAsync(paymentRequest, requestStartInput);
    var requestStartResultDto = ObjectMapper.Map<PaymentRequestStartResult, PaymentRequestStartResultDto>(requestStartResult);
    Replacement(requestStartResult, requestStartResultDto);

    return requestStartResultDto;
  }

  public virtual async Task<PaymentRequestWithDetailsDto> CompleteAsync(
    string paymentGateway,
    Dictionary<string, string> parameters)
  {
    var paymentRequest = await PaymentGatewayResolver.Resolve(paymentGateway).CompleteAsync(parameters);
    if (paymentRequest.State == PaymentRequestState.Completed)
    {
      await DistributedEventBus.PublishAsync(new PaymentRequestCompletedEto(paymentRequest.Id, paymentRequest.Gateway, paymentRequest.Currency, ObjectMapper.Map<ICollection<PaymentRequestProduct>, List<PaymentRequestProductCompletedEto>>(paymentRequest.Products), paymentRequest.ExtraProperties), true, true);
    }

    var requestWithDetailsDto = await GetPaymentRequestWithDetailsDtoAsync(paymentRequest);

    return requestWithDetailsDto;
  }

  public virtual async Task<bool> HandleWebhookAsync(
    string paymentGateway,
    string payload,
    Dictionary<string, string> headers)
  {
    await PaymentGatewayResolver.Resolve(paymentGateway).HandleWebhookAsync(payload, headers);
    return true;
  }

  private async Task<PaymentRequestWithDetailsDto> GetPaymentRequestWithDetailsDtoAsync(PaymentRequest paymentRequest)
  {
    return ObjectMapper.Map<PaymentRequest, PaymentRequestWithDetailsDto>(await PaymentRequestRepository.GetAsync(paymentRequest.Id));
  }

  private static void Replacement(
    IHasExtraProperties extraPropertiesInput,
    IHasExtraProperties extraPropertiesOutput)
  {
    foreach (var key in extraPropertiesInput.ExtraProperties.Keys)
    {
      extraPropertiesOutput.ExtraProperties[key] = extraPropertiesInput.ExtraProperties[key];
    }
  }
}
