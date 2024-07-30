// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Payment.Requests;

public interface IPaymentRequestRepository :
IBasicRepository<PaymentRequest, Guid>
{
    Task<List<PaymentRequest>> GetPagedListAsync(
      int skipCount,
      int maxResultCount,
      string sorting,
      string filter,
      DateTime? creationDateMax = null,
      DateTime? creationDateMin = null,
      PaymentType? paymentType = null,
      PaymentRequestState? state = null,
      bool includeDetails = false,
      CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(
      string filter,
      DateTime? creationDateMax = null,
      DateTime? creationDateMin = null,
      PaymentType? paymentType = null,
      PaymentRequestState? state = null,
      CancellationToken cancellationToken = default);

    Task<List<PaymentRequest>> GetListAsync(
      DateTime startDate,
      DateTime endDate,
      CancellationToken cancellationToken = default);

    Task<PaymentRequest> GetBySubscriptionAsync(
      string externalSubscriptionId,
      CancellationToken cancellationToken = default);
}
