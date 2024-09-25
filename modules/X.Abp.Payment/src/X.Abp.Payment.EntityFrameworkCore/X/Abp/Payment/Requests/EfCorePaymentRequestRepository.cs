// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Payment.EntityFrameworkCore;

namespace X.Abp.Payment.Requests;

public class EfCorePaymentRequestRepository : EfCoreRepository<IPaymentDbContext, PaymentRequest, Guid>, IPaymentRequestRepository
{
  public EfCorePaymentRequestRepository(IDbContextProvider<IPaymentDbContext> dbContextProvider)
    : base(dbContextProvider)
  {
  }

  public override async Task<IQueryable<PaymentRequest>> WithDetailsAsync()
  {
    return (await GetQueryableAsync()).IncludeDetails();
  }

  public virtual async Task<List<PaymentRequest>> GetListAsync(
      DateTime startDate,
      DateTime endDate,
      CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync())
        .Where(paymentRequest => paymentRequest.State != PaymentRequestState.Completed &&
        paymentRequest.CreationTime >= startDate &&
        paymentRequest.CreationTime <= endDate &&
        paymentRequest.CreatorId.HasValue).ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<PaymentRequest> GetBySubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync()).FirstOrDefaultAsync(paymentRequest => paymentRequest.ExternalSubscriptionId == externalSubscriptionId, GetCancellationToken(cancellationToken)) ?? throw new EntityNotFoundException(typeof(PaymentRequest));
  }

  public virtual async Task<List<PaymentRequest>> GetPagedListAsync(
    int skipCount,
    int maxResultCount,
    string sorting,
    string filter,
    DateTime? creationDateMax = null,
    DateTime? creationDateMin = null,
    PaymentType? paymentType = null,
    PaymentRequestState? state = null,
    bool includeDetails = false,
    CancellationToken cancellationToken = default)
  {
    var queryable = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();
    var source = CreateFilteredQuery(queryable, filter, creationDateMax, creationDateMin, paymentType, state).Skip(skipCount).Take(maxResultCount);
    if (!sorting.IsNullOrEmpty())
    {
      source = source.OrderBy(sorting);
    }

    return await source.ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<int> GetCountAsync(
    string filter,
    DateTime? creationDateMax = null,
    DateTime? creationDateMin = null,
    PaymentType? paymentType = null,
    PaymentRequestState? state = null,
    CancellationToken cancellationToken = default)
  {
    var queryable = await GetQueryableAsync();
    return await CreateFilteredQuery(queryable, filter, creationDateMax, creationDateMin, paymentType, state).CountAsync(GetCancellationToken(cancellationToken));
  }

  private static IQueryable<PaymentRequest> CreateFilteredQuery(IQueryable<PaymentRequest> queryable, string filter, DateTime? creationDateMax = null, DateTime? creationDateMin = null, PaymentType? paymentType = null, PaymentRequestState? state = null)
  {
    return queryable.WhereIf(!filter.IsNullOrEmpty(),
        paymentRequest => paymentRequest.Currency.Contains(filter) ||
        paymentRequest.Gateway.Contains(filter) ||
        paymentRequest.ExternalSubscriptionId == filter)
        .WhereIf(creationDateMax.HasValue,
        paymentRequest => paymentRequest.CreationTime <= creationDateMax)
        .WhereIf(creationDateMin.HasValue,
        paymentRequest => paymentRequest.CreationTime >= creationDateMin)
        .WhereIf(paymentType.HasValue, paymentRequest => paymentRequest.Products.Any(paymentRequestProduct => paymentRequestProduct.PaymentType == paymentType))
        .WhereIf(state.HasValue, paymentRequest => paymentRequest.State == state);
  }
}
