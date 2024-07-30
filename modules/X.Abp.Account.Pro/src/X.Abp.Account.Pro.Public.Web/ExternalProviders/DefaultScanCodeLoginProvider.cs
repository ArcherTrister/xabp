// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Account.Public.Web.ExternalProviders;

/// <summary>
/// 扫码登录服务
/// </summary>
public class DefaultScanCodeLoginProvider : IScanCodeLoginProvider, ITransientDependency
{
    protected IDistributedCache<LoginQrCodeCacheItem, string> DistributedCache { get; }

    public DefaultScanCodeLoginProvider(IDistributedCache<LoginQrCodeCacheItem, string> distributedCache)
    {
        DistributedCache = distributedCache;
    }

    public async Task<string> GenerateQrCodeAsync()
    {
        var qrCodeKey = Guid.NewGuid().ToString("N");
        await DistributedCache.SetAsync(qrCodeKey, new LoginQrCodeCacheItem(), new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(300) });
        return qrCodeKey;
    }

    public async Task<LoginQrCode> CheckQrCodeAsync(string qrCodeKey)
    {
        var qrcode = await DistributedCache.GetAsync(qrCodeKey);
        if (qrcode == null)
        {
            return new LoginQrCode { QrCodeStatus = QrCodeStatus.Invalid };
        }

        return qrcode.ToLoginQrCode();
    }

    public async Task<LoginQrCodeCacheItem> GetQrCodeAsync(string qrCodeKey)
    {
        return await DistributedCache.GetAsync(qrCodeKey);
    }

    public async Task<LoginQrCode> ScanCodeAsync(string qrCodeKey, Guid userId, Guid? tenantId)
    {
        var qrcode = await DistributedCache.GetAsync(qrCodeKey);
        if (qrcode == null)
        {
            return new LoginQrCode { QrCodeStatus = QrCodeStatus.Invalid };
        }

        LoginQrCodeCacheItem update = qrcode.ScanCode(userId, tenantId);

        await DistributedCache.SetAsync(qrCodeKey, update, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(180) });

        return update.ToLoginQrCode();
    }

    public async Task<LoginQrCode> ConfirmLoginAsync(string qrCodeKey, string scanCodeToken)
    {
        var qrcode = await DistributedCache.GetAsync(qrCodeKey);
        if (qrcode == null)
        {
            return new LoginQrCode { QrCodeStatus = QrCodeStatus.Invalid };
        }

        LoginQrCodeCacheItem update = qrcode.Confirm(scanCodeToken);

        await DistributedCache.SetAsync(qrCodeKey, update, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(180) });

        return update.ToLoginQrCode();
    }
}
