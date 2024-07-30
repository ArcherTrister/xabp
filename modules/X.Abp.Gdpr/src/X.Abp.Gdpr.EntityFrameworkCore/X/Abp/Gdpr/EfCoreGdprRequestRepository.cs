// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Gdpr;

public class EfCoreGdprRequestRepository :
EfCoreRepository<IGdprDbContext, GdprRequest, Guid>,
IGdprRequestRepository
{
    public EfCoreGdprRequestRepository(
      IDbContextProvider<IGdprDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountByUserIdAsync(
      Guid userId,
      CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(gdprRequest => gdprRequest.UserId == userId).LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<GdprRequest>> GetListByUserIdAsync(
      Guid userId,
      CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(gdprRequest => gdprRequest.UserId == userId).OrderByDescending(gdprRequest => gdprRequest.CreationTime).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<DateTime?> FindLatestRequestTimeOfUserAsync(
      Guid userId,
      CancellationToken cancellationToken = default)
    {
        return new DateTime?(await (await GetQueryableAsync()).Where(gdprRequest => gdprRequest.UserId == userId).OrderByDescending(gdprRequest => gdprRequest.CreationTime).Select(gdprRequest => gdprRequest.CreationTime).FirstOrDefaultAsync(GetCancellationToken(cancellationToken)));
    }

    public override async Task<IQueryable<GdprRequest>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(gdprRequest => gdprRequest.Infos);
    }
}
