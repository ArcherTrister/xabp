// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Requests
{
  [RemoteService(true, Name = AbpPaymentAdminRemoteServiceConsts.RemoteServiceName)]
  [Route("api/payment-admin/payment-requests")]
  [Authorize(AbpPaymentAdminPermissions.PaymentRequests.Default)]
  [Area(AbpPaymentAdminRemoteServiceConsts.ModuleName)]
  public class PaymentRequestAdminController :
  PaymentAdminController,
  IPaymentRequestAdminAppService
  {
    protected IPaymentRequestAdminAppService PaymentRequestAdminAppService { get; }

    public PaymentRequestAdminController(IPaymentRequestAdminAppService paymentRequestAdminAppService)
    {
      PaymentRequestAdminAppService = paymentRequestAdminAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(PaymentRequestGetListInput input)
    {
      return PaymentRequestAdminAppService.GetListAsync(input);
    }

    [Route("{id}")]
    [HttpGet]
    public virtual Task<PaymentRequestWithDetailsDto> GetAsync(Guid id)
    {
      return PaymentRequestAdminAppService.GetAsync(id);
    }
  }
}
