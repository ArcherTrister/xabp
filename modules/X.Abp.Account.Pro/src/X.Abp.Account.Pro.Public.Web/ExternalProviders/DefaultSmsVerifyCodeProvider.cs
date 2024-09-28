// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

using X.Abp.Account.Public.Web.ExternalProviders;

namespace X.Abp.Account.Web.ExternalProviders;

/// <summary>
/// 默认验证码提供者
/// </summary>
// https://docs.abp.io/zh-Hans/abp/5.2/Dependency-Injection
// [Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
// [ExposeServices(typeof(ISmsVerifyCodeProvider))]
// [Dependency(TryRegister = true)]
public class DefaultSmsVerifyCodeProvider : ISmsVerifyCodeProvider, ITransientDependency
{
    protected IDistributedCache<string> DistributedCache { get; }

    public DefaultSmsVerifyCodeProvider(IDistributedCache<string> distributedCache)
    {
        DistributedCache = distributedCache;
    }

    public virtual async Task<bool> CheckAsync(string phoneNumber, string smsVerifyCode, string verifyParameter)
    {
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(verifyParameter!, WriteOptions);

        var verifyCodeId = parameters?.GetValueOrDefault("VerifyCodeId");
        var code = await DistributedCache.GetAsync($"{phoneNumber}:{verifyCodeId}");
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        await DistributedCache.RemoveAsync($"{phoneNumber}:{verifyCodeId}");
        return true;
    }

    public virtual async Task SaveAsync(string phoneNumber, string smsVerifyCode, string verifyParameter)
    {
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(verifyParameter!, WriteOptions);

        var verifyCodeId = parameters?.GetValueOrDefault("VerifyCodeId");
        await DistributedCache.SetAsync($"{phoneNumber}:{verifyCodeId}", smsVerifyCode, options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
    }

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}
