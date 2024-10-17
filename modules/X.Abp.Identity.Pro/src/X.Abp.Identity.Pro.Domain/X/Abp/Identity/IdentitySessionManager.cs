// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Users;

using X.Abp.Identity.Settings;

namespace X.Abp.Identity;

public class IdentitySessionManager : DomainService
{
    protected IIdentitySessionRepository IdentitySessionRepository { get; }

    protected ICurrentUser CurrentUser { get; }

    protected IDistributedCache<IdentitySessionCacheItem> Cache { get; }

    protected ISettingProvider SettingProvider { get; }

    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }

    public IdentitySessionManager(
      IIdentitySessionRepository identitySessionRepository,
      ICurrentUser currentUser,
      IDistributedCache<IdentitySessionCacheItem> cache,
      ISettingProvider settingProvider,
      IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache)
    {
        IdentitySessionRepository = identitySessionRepository;
        CurrentUser = currentUser;
        Cache = cache;
        SettingProvider = settingProvider;
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
    }

    public virtual async Task<IdentitySession> CreateAsync(
      string sessionId,
      string device,
      string deviceInfo,
      Guid userId,
      Guid? tenantId,
      string clientId,
      string ipAddresses)
    {
        Check.NotNullOrWhiteSpace(sessionId, nameof(sessionId));
        Check.NotNullOrWhiteSpace(device, nameof(device));
        IdentitySession session = await IdentitySessionRepository.FindAsync(sessionId);
        if (session == null)
        {
            Logger.LogInformation("Creating identity session for session id: {SessionId}, device: {Device}, user id: {UserId}, tenant id: {TenantId}, client id: {ClientId}", sessionId, device, userId, tenantId, clientId);
            session = await IdentitySessionRepository.InsertAsync(new IdentitySession(GuidGenerator.Create(), sessionId, device, deviceInfo, userId, tenantId, clientId, ipAddresses, Clock.Now));
        }

        IdentityProPreventConcurrentLoginBehaviour concurrentLoginBehaviour = await IdentityProPreventConcurrentLoginBehaviourSettingHelper.Get(SettingProvider);
        if (concurrentLoginBehaviour != IdentityProPreventConcurrentLoginBehaviour.LogoutFromSameTypeDevices)
        {
            if (concurrentLoginBehaviour == IdentityProPreventConcurrentLoginBehaviour.LogoutFromAllDevices)
            {
                await RevokeAllAsync(userId, session.Id);
            }
        }
        else
        {
            await RevokeAllAsync(userId, device, session.Id);
        }

        return session;
    }

    public virtual async Task UpdateAsync(IdentitySession session)
    {
        await IdentitySessionRepository.UpdateAsync(session);
    }

    public virtual async Task<List<IdentitySession>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      Guid? userId = null,
      string device = null,
      string clientId = null)
    {
        List<IdentitySession> sessions = await IdentitySessionRepository.GetListAsync(sorting, maxResultCount, skipCount, userId, device, clientId);
        KeyValuePair<string, IdentitySessionCacheItem>[] source = await Cache.GetManyAsync(sessions.Select(s => s.SessionId).ToArray());
        List<IdentitySession> changedSessions = new List<IdentitySession>();
        foreach (KeyValuePair<string, IdentitySessionCacheItem> keyValuePair in source.Where(x => x.Value != null))
        {
            IdentitySession session = sessions.FirstOrDefault(s => s.SessionId == keyValuePair.Key);

            if (session != null)
            {
                bool flag = await UpdateSessionFromCacheAsync(session, keyValuePair.Value);

                if (flag)
                {
                    changedSessions.Add(session);
                    session = null;
                    continue;
                }
            }
        }

        if (changedSessions.Count != 0)
        {
            await IdentitySessionRepository.UpdateManyAsync(changedSessions);
        }

        if (sorting.IsNullOrWhiteSpace())
        {
            sorting = $"{nameof(IdentitySession.LastAccessed)} desc";
        }

        if (sorting.Contains($"{nameof(IdentitySession.LastAccessed)}", StringComparison.OrdinalIgnoreCase))
        {
            sessions = sorting.Contains("desc", StringComparison.OrdinalIgnoreCase) ? sessions.OrderByDescending(s => s.LastAccessed).ToList() : sessions.OrderBy(s => s.LastAccessed).ToList();
        }

        return sessions;
    }

    public virtual async Task<IdentitySession> GetAsync(Guid id)
    {
        var session = await IdentitySessionRepository.GetAsync(id);
        return await UpdateSessionFromCacheAsync(session);
    }

    public virtual async Task<IdentitySession> FindAsync(Guid id)
    {
        var session = await IdentitySessionRepository.FindAsync(id);
        return await UpdateSessionFromCacheAsync(session);
    }

    public virtual async Task<IdentitySession> FindAsync(string sessionId)
    {
        var session = await IdentitySessionRepository.FindAsync(sessionId);
        return await UpdateSessionFromCacheAsync(session);
    }

    public virtual async Task<bool> ExistAsync(Guid id)
    {
        return await IdentitySessionRepository.ExistAsync(id);
    }

    public virtual async Task<bool> ExistAsync(string sessionId)
    {
        return await IdentitySessionRepository.ExistAsync(sessionId);
    }

    public virtual async Task<IdentitySession> UpdateSessionFromCacheAsync(string sessionId)
    {
        var identitySession = await IdentitySessionRepository.FindAsync(sessionId);
        return await UpdateSessionFromCacheAsync(identitySession);
    }

    protected virtual async Task<IdentitySession> UpdateSessionFromCacheAsync(IdentitySession session)
    {
        if (session == null)
        {
            return null;
        }

        IdentitySessionCacheItem sessionCacheItem = await Cache.GetAsync(session.SessionId);
        if (await UpdateSessionFromCacheAsync(session, sessionCacheItem))
        {
            await IdentitySessionRepository.UpdateAsync(session);
        }

        return session;
    }

    protected virtual Task<bool> UpdateSessionFromCacheAsync(IdentitySession session, IdentitySessionCacheItem sessionCacheItem)
    {
        if (session == null)
        {
            return Task.FromResult(false);
        }

        if (sessionCacheItem == null)
        {
            return Task.FromResult(false);
        }

        bool result = false;
        DateTime? cacheLastAccessed = sessionCacheItem.CacheLastAccessed;
        if (cacheLastAccessed.HasValue)
        {
            cacheLastAccessed = session.LastAccessed;
            if (cacheLastAccessed.HasValue)
            {
                cacheLastAccessed = sessionCacheItem.CacheLastAccessed;
                DateTime? lastAccessed = session.LastAccessed;

                if (!(cacheLastAccessed.HasValue & lastAccessed.HasValue && cacheLastAccessed.GetValueOrDefault() > lastAccessed.GetValueOrDefault()))
                {
                    goto SetIpAddress;
                }
            }

            session.UpdateLastAccessedTime(sessionCacheItem.CacheLastAccessed);
            result = true;
        }

SetIpAddress:
        if (!sessionCacheItem.IpAddress.IsNullOrWhiteSpace())
        {
            List<string> list = session.GetIpAddresses().ToList();
            list.RemoveAll(x => x == sessionCacheItem.IpAddress);
            list.Add(sessionCacheItem.IpAddress);
            session.SetIpAddresses(list);
            result = true;
        }

        return Task.FromResult(result);
    }

    public virtual async Task RevokeAsync(Guid id)
    {
        IdentitySession session = await IdentitySessionRepository.FindAsync(id);
        if (session != null)
        {
            await RevokeAsync(session);
        }
    }

    public virtual async Task RevokeAsync(string sessionId)
    {
        IdentitySession session = await IdentitySessionRepository.FindAsync(sessionId);
        if (session != null)
        {
            await RevokeAsync(session);
        }
    }

    public virtual async Task RevokeAsync(IdentitySession session)
    {
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(session.UserId, session.TenantId);
        try
        {
            await IdentitySessionRepository.DeleteAsync(session);
        }
        catch (AbpDbConcurrencyException ex)
        {
            Logger.LogError(ex, $"Failed to delete the session for user: {session.UserId}, tenant: {session.TenantId}");
        }
    }

    public virtual async Task RevokeAllAsync(Guid userId, Guid? exceptSessionId = null)
    {
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(userId);

        await IdentitySessionRepository.DeleteAllAsync(userId, exceptSessionId);
    }

    public virtual async Task RevokeAllAsync(Guid userId, string device, Guid? exceptSessionId = null)
    {
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(userId);

        await IdentitySessionRepository.DeleteAllAsync(userId, device, exceptSessionId);
    }

    public virtual async Task DeleteAllAsync(TimeSpan inactiveTimeSpan)
    {
        await IdentitySessionRepository.DeleteAllAsync(inactiveTimeSpan);
    }
}
