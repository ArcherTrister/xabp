// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Gdpr;

public interface IGdprRequestRepository :
IBasicRepository<GdprRequest, Guid>
{
    Task<long> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<GdprRequest>> GetListByUserIdAsync(
      Guid userId,
      CancellationToken cancellationToken = default);

    Task<DateTime?> FindLatestRequestTimeOfUserAsync(
      Guid userId,
      CancellationToken cancellationToken = default);
}
