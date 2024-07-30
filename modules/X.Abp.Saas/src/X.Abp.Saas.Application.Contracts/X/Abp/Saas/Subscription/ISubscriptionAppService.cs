// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using X.Abp.Payment.Requests;

namespace X.Abp.Saas.Subscription;

public interface ISubscriptionAppService
{
    Task<PaymentRequestWithDetailsDto> CreateSubscriptionAsync(Guid editionId, Guid tenantId);
}
