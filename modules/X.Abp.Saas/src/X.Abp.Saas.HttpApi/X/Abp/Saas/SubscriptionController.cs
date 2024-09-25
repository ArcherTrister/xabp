// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Payment.Requests;
using X.Abp.Saas.Permissions;
using X.Abp.Saas.Subscription;

namespace X.Abp.Saas;

[Area(AbpSaasRemoteServiceConsts.ModuleName)]
[ControllerName("Edition")]
[Route("/api/saas/subscription")]
[RemoteService(true, Name = AbpSaasRemoteServiceConsts.RemoteServiceName)]
[Authorize(AbpSaasPermissions.Editions.Default)]
[Controller]
public class SubscriptionController : AbpControllerBase, ISubscriptionAppService
{
  protected ISubscriptionAppService SubscriptionAppService { get; }

  public SubscriptionController(ISubscriptionAppService subscriptionAppService)
  {
    SubscriptionAppService = subscriptionAppService;
  }

  [HttpPost]
  public virtual Task<PaymentRequestWithDetailsDto> CreateSubscriptionAsync(Guid editionId, Guid tenantId)
  {
    return SubscriptionAppService.CreateSubscriptionAsync(editionId, tenantId);
  }
}
