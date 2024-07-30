// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.VersionManagement.AppEditions;

public interface IAppEditionRepository : IBasicRepository<AppEdition, Guid>
{
    Task<AppEdition> CheckUpdateAsync(string appName, PlatformType platformType, string arch, string channel, CancellationToken cancellationToken = default);

    Task<bool> CheckUploaderVersionAsync(string appName, PlatformType platformType, string arch, string channel, string version, CancellationToken cancellationToken = default);

    Task<AppEdition> GetLatestVersion(string appName, PlatformType platformType, string arch, string channel, bool isPublish = true, CancellationToken cancellationToken = default);

    Task<AppEdition> FindAsync(string appName, PlatformType platformType, string arch, string channel, string version, CancellationToken cancellationToken = default);

    Task<List<AppEdition>> GetListAsync(
        string sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        string appName = null,
        string arch = null,
        string version = null,
        string channel = null,
        PlatformType? platformType = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string appName = null,
        string arch = null,
        string version = null,
        string channel = null,
        PlatformType? platformType = null,
        CancellationToken cancellationToken = default);
}
