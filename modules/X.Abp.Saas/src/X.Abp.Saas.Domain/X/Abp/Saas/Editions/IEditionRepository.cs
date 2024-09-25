// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Saas.Editions;

public interface IEditionRepository : IBasicRepository<Edition, Guid>
{
    Task<List<Edition>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(string filter, CancellationToken cancellationToken = default);

    Task<bool> CheckNameExistAsync(string displayName, CancellationToken cancellationToken = default);

    Task<List<EditionWithTenantCount>> GetListWithTenantCountAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string filter = null,
      bool includeDetails = false,
      CancellationToken cancellationToken = default);

    Task<Edition> FindByDisplayNameAsync(
      string displayName,
      bool includeDetails = true,
      CancellationToken cancellationToken = default);
}
