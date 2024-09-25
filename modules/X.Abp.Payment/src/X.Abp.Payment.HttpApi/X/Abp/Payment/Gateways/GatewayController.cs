// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

namespace X.Abp.Payment.Gateways;

[RemoteService(true, Name = AbpPaymentCommonRemoteServiceConsts.RemoteServiceName)]
[Area(AbpPaymentCommonRemoteServiceConsts.ModuleName)]
[Route("api/payment/gateways")]
public class GatewayController :
PaymentCommonController,
IGatewayAppService
{
  protected IGatewayAppService GatewayAppService { get; }

  public GatewayController(IGatewayAppService gatewayAppService)
  {
    GatewayAppService = gatewayAppService;
  }

  [HttpGet]
  public virtual Task<List<GatewayDto>> GetGatewayConfigurationAsync()
  {
    return GatewayAppService.GetGatewayConfigurationAsync();
  }

  [Route("subscription-supported")]
  [HttpGet]
  public virtual Task<List<GatewayDto>> GetSubscriptionSupportedGatewaysAsync()
  {
    return GatewayAppService.GetSubscriptionSupportedGatewaysAsync();
  }
}
