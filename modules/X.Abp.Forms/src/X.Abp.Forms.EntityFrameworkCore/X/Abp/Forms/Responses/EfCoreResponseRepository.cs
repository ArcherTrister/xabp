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

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Forms.EntityFrameworkCore;

namespace X.Abp.Forms.Responses;

public class EfCoreResponseRepository : EfCoreRepository<IFormsDbContext, FormResponse, Guid>, IResponseRepository
{
  public EfCoreResponseRepository(IDbContextProvider<IFormsDbContext> dbContextProvider)
      : base(dbContextProvider)
  {
  }

  public virtual async Task<List<FormResponse>> GetListAsync(
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      string sorting = null,
      string filter = null,
      CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(FormResponse.CreationTime) : sorting)
        .PageBy(skipCount, maxResultCount)
        .ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<List<FormResponse>> GetListByFormIdAsync(
      Guid formId,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      string sorting = null,
      string filter = null,
      CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .IncludeDetails(true)
        .Where(q => q.FormId == formId)
        .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(FormResponse.CreationTime) : sorting)
        .PageBy(skipCount, maxResultCount)
        .ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<List<FormWithResponse>> GetByUserId(
      Guid userId,
      CancellationToken cancellationToken = default)
  {
    var dbContext = await GetDbContextAsync();

    var query = from response in dbContext.FormResponses
                where response.UserId == userId
                join form in dbContext.Forms on response.FormId equals form.Id into formWithResponses
                from form in formWithResponses.DefaultIfEmpty()
                select new FormWithResponse
                {
                  Form = form,
                  Response = response
                };

    return await query.ToListAsync(cancellationToken: cancellationToken);
  }

  public virtual async Task<long> GetCountByFormIdAsync(
      Guid formId,
      string filter = null,
      CancellationToken cancellationToken = default)
  {
    var query = await GetQueryableAsync();

    return await query.LongCountAsync(q => q.FormId == formId, GetCancellationToken(cancellationToken));
  }

  public virtual async Task<bool> UserResponseExistsAsync(Guid formId, Guid userId, CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .AnyAsync(x =>
                x.FormId == formId &&
                x.UserId.HasValue &&
                x.UserId == userId,
            GetCancellationToken(cancellationToken));
  }

  public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync()).LongCountAsync(GetCancellationToken(cancellationToken));
  }

  public override async Task<IQueryable<FormResponse>> WithDetailsAsync()
  {
    return (await GetQueryableAsync()).IncludeDetails();
  }
}
