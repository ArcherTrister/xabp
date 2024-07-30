// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

namespace X.Abp.Account.Public.Web.ExternalProviders;

/// <summary>
/// 扫码登录服务
/// </summary>
public interface IScanCodeLoginProvider
{
    /// <summary>
    /// 生成二维码
    /// </summary>
    Task<string> GenerateQrCodeAsync();

    /// <summary>
    /// 检查二维码
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    Task<LoginQrCode> CheckQrCodeAsync(string qrCodeKey);

    /// <summary>
    /// 获取二维码
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    Task<LoginQrCodeCacheItem> GetQrCodeAsync(string qrCodeKey);

    /// <summary>
    /// 扫码
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    /// <param name="userId">userId</param>
    /// <param name="tenantId">tenantId</param>
    Task<LoginQrCode> ScanCodeAsync(string qrCodeKey, Guid userId, Guid? tenantId);

    /// <summary>
    /// 确认登录
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    /// <param name="scanCodeToken">scanCodeToken</param>
    Task<LoginQrCode> ConfirmLoginAsync(string qrCodeKey, string scanCodeToken);
}
