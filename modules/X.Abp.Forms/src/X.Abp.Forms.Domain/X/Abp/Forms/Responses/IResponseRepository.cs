// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Forms.Responses;

public interface IResponseRepository : IBasicRepository<FormResponse, Guid>
{
    public Task<List<FormResponse>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = null,
        string filter = null,
        CancellationToken cancellationToken = default);

    public Task<List<FormResponse>> GetListByFormIdAsync(
        Guid formId,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = null,
        string filter = null,
        CancellationToken cancellationToken = default);

    public Task<List<FormWithResponse>> GetByUserId(Guid userId, CancellationToken cancellationToken = default);

    public Task<long> GetCountByFormIdAsync(Guid formId, string filter = null, CancellationToken cancellationToken = default);

    public Task<bool> UserResponseExistsAsync(Guid formId, Guid userId, CancellationToken cancellationToken = default);

    public Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default);
}
