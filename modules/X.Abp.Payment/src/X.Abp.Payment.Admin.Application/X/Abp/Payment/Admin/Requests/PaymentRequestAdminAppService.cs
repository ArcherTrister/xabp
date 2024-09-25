// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;

using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Requests
{
  [Authorize(AbpPaymentAdminPermissions.PaymentRequests.Default)]
  public class PaymentRequestAdminAppService :
  PaymentAdminAppServiceBase,
  IPaymentRequestAdminAppService
  {
    protected IPaymentRequestRepository PaymentRequestRepository { get; }

    public PaymentRequestAdminAppService(IPaymentRequestRepository paymentRequestRepository)
    {
      PaymentRequestRepository = paymentRequestRepository;
    }

    public virtual async Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(
      PaymentRequestGetListInput input)
    {
      List<PaymentRequest> paymentRequests = await PaymentRequestRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.CreationDateMax, input.CreationDateMin, input.PaymentType, input.Status, true);
      return new PagedResultDto<PaymentRequestWithDetailsDto>(await PaymentRequestRepository.GetCountAsync(input.Filter, input.CreationDateMax, input.CreationDateMin, input.PaymentType, input.Status), ObjectMapper.Map<List<PaymentRequest>, List<PaymentRequestWithDetailsDto>>(paymentRequests));
    }

    public virtual async Task<PaymentRequestWithDetailsDto> GetAsync(Guid id)
    {
      PaymentRequest paymentRequest = await PaymentRequestRepository.GetAsync(id);
      return ObjectMapper.Map<PaymentRequest, PaymentRequestWithDetailsDto>(paymentRequest);
    }
  }
}
