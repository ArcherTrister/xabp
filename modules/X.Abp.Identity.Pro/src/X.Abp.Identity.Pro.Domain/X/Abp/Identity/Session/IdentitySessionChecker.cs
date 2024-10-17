// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace X.Abp.Identity.Session;
public class IdentitySessionChecker : ITransientDependency
{
    public ILogger<IdentitySessionChecker> Logger { get; set; }

    protected IOptions<Volo.Abp.Security.Claims.AbpClaimsPrincipalFactoryOptions> AbpClaimsPrincipalFactoryOptions { get; }

    protected IdentitySessionManager IdentitySessionManager { get; }

    protected IDistributedCache<IdentitySessionCacheItem> Cache { get; }

    protected IClock Clock { get; }

    protected IWebClientInfoProvider WebClientInfoProvider { get; }

    protected IOptions<IdentitySessionCheckerOptions> Options { get; }

    public IdentitySessionChecker(
      IOptions<Volo.Abp.Security.Claims.AbpClaimsPrincipalFactoryOptions> abpClaimsPrincipalFactoryOption,
      IdentitySessionManager identitySessionManager,
      IDistributedCache<IdentitySessionCacheItem> cache,
      IClock clock,
      IWebClientInfoProvider webClientInfoProvider,
      IOptions<IdentitySessionCheckerOptions> options)
    {
        Logger = NullLogger<IdentitySessionChecker>.Instance;
        AbpClaimsPrincipalFactoryOptions = abpClaimsPrincipalFactoryOption;
        IdentitySessionManager = identitySessionManager;
        Cache = cache;
        Clock = clock;
        WebClientInfoProvider = webClientInfoProvider;
        Options = options;
    }

    public virtual async Task<bool> IsValidateAsync(string sessionId)
    {
        if (!AbpClaimsPrincipalFactoryOptions.Value.IsDynamicClaimsEnabled)
        {
            Logger.LogInformation("Dynamic claims is disabled, The SessionId({SessionId}) will not be checked.", sessionId);
            return true;
        }

        if (sessionId.IsNullOrWhiteSpace())
        {
            Logger.LogWarning("SessionId is null or empty!");
            return false;
        }

        IdentitySessionCacheItem sessionCacheItem = await Cache.GetOrAddAsync(sessionId, async () =>
        {
            Logger.LogDebug("Get session from IdentitySessionManager for {SessionId}", sessionId);
            IdentitySession identitySession = await IdentitySessionManager.FindAsync(sessionId);
            if (identitySession == null)
            {
                Logger.LogWarning("Could not find a session with ID: {SessionId}", sessionId);
                return null;
            }

            Logger.LogDebug("Found session from database for {SessionId}", sessionId);
            return new IdentitySessionCacheItem()
            {
                Id = identitySession.Id,
                SessionId = identitySession.SessionId
            };
        });

        if (sessionCacheItem != null)
        {
            Logger.LogDebug("SessionId({SessionId}) found in cache, Updating hit count({HitCount}), last access time({CacheLastAccessed}) and IP address({IpAddress}).", sessionCacheItem.SessionId, sessionCacheItem.HitCount, sessionCacheItem.CacheLastAccessed, sessionCacheItem.IpAddress);

            sessionCacheItem.CacheLastAccessed = Clock.Now;
            sessionCacheItem.IpAddress = WebClientInfoProvider.ClientIpAddress;
            ++sessionCacheItem.HitCount;

            await Cache.SetAsync(sessionId, sessionCacheItem);
            if (sessionCacheItem.HitCount == 1)
            {
                Logger.LogDebug("Updating the session from cache on the first check.");
                await IdentitySessionManager.UpdateSessionFromCacheAsync(sessionId);
            }
            else if (sessionCacheItem.HitCount > Options.Value.UpdateSessionAfterCacheHit)
            {
                Logger.LogDebug("Update the session from cache because reached the maximum cache hit count({HitCount}).", Options.Value.UpdateSessionAfterCacheHit);
                sessionCacheItem.HitCount = 0;
                await Cache.SetAsync(sessionId, sessionCacheItem);
                await IdentitySessionManager.UpdateSessionFromCacheAsync(sessionId);
            }

            return true;
        }

        await Cache.RemoveAsync(sessionId);
        return false;
    }
}
