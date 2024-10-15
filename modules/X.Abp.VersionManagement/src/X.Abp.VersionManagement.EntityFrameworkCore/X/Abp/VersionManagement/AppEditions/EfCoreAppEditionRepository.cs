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

using X.Abp.VersionManagement.EntityFrameworkCore;

namespace X.Abp.VersionManagement.AppEditions;

public class EfCoreAppEditionRepository : EfCoreRepository<IVersionManagementDbContext, AppEdition, Guid>, IAppEditionRepository
{
    public EfCoreAppEditionRepository(IDbContextProvider<IVersionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<AppEdition> CheckUpdateAsync(
        string appName,
        PlatformType platformType,
        string arch,
        string channel,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Arch == arch &&
            p.Channel == channel &&
            p.PlatformType == platformType &&
            p.PublishType == PublishType.Released)
            .OrderByDescending(p => p.Major)
            .ThenByDescending(p => p.Minor)
            .ThenByDescending(p => p.Build)
            .ThenByDescending(p => p.Revision);
        return await query.AsNoTracking().FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<AppEdition> CheckUpdateAndroidAsync(
        string appName,
        string channel,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Channel == channel &&
            p.PlatformType == PlatformType.Android &&
            p.PublishType == PublishType.Released)
            .OrderByDescending(p => p.Major)
            .ThenByDescending(p => p.Minor)
            .ThenByDescending(p => p.Build)
            .ThenByDescending(p => p.Revision);

        return await query.AsNoTracking().FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<AppEdition> CheckUpdateWinAsync(
        string appName,
        string arch,
        string channel,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Arch == arch &&
            p.Channel == channel &&
            p.PlatformType == PlatformType.Windows &&
            p.PublishType == PublishType.Released)
            .OrderByDescending(p => p.Major)
            .ThenByDescending(p => p.Minor)
            .ThenByDescending(p => p.Build)
            .ThenByDescending(p => p.Revision);

        return await query.AsNoTracking().FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<bool> CheckUploaderVersionAsync(
        string appName,
        PlatformType platformType,
        string arch,
        string channel,
        string version,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Arch == arch &&
            p.Channel == channel &&
            p.Version == version &&
            p.PlatformType == platformType &&
            p.PublishType == PublishType.Released);

        return await query.AsNoTracking().AnyAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<AppEdition> GetLatestVersion(
        string appName,
        PlatformType platformType,
        string arch,
        string channel,
        bool isPublish = true,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Arch == arch &&
            p.Channel == channel &&
            p.PlatformType == platformType &&
            p.PublishType == PublishType.Released)
            .OrderByDescending(p => p.Major)
            .ThenByDescending(p => p.Minor)
            .ThenByDescending(p => p.Build)
            .ThenByDescending(p => p.Revision);

        return await query.AsNoTracking().FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<AppEdition> FindAsync(
        string appName,
        PlatformType platformType,
        string arch,
        string channel,
        string version,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(p =>
            p.AppName == appName &&
            p.Arch == arch &&
            p.Channel == channel &&
            p.Version == version &&
            p.PlatformType == platformType);

        return await query.AsNoTracking().FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<AppEdition>> GetListAsync(
        string sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        string appName = null,
        string arch = null,
        string version = null,
        string channel = null,
        PlatformType? platformType = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(
            appName,
            arch,
            version,
            channel,
            platformType);

        return await query.OrderBy(sorting.IsNullOrWhiteSpace() ? $"{nameof(AppEdition.CreationTime)} desc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(
        string appName = null,
        string arch = null,
        string version = null,
        string channel = null,
        PlatformType? platformType = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(
            appName,
            arch,
            version,
            channel,
            platformType);

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual async Task<IQueryable<AppEdition>> GetListQueryAsync(
          string appName = null,
          string arch = null,
          string version = null,
          string channel = null,
          PlatformType? platformType = null)
    {
        return (await GetDbSetAsync()).AsNoTracking()
                .WhereIf(!appName.IsNullOrWhiteSpace(), p => p.AppName.Contains(appName))
                .WhereIf(!arch.IsNullOrWhiteSpace(), p => p.Arch.Contains(arch))
                .WhereIf(!version.IsNullOrWhiteSpace(), p => p.Version.Contains(version))
                .WhereIf(!channel.IsNullOrWhiteSpace(), p => p.Channel.Contains(channel))
                .WhereIf(platformType.HasValue, p => p.PlatformType == platformType.Value);
    }
}
