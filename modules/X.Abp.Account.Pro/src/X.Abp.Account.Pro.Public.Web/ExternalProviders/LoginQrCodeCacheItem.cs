// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Public.Web.ExternalProviders;

/// <summary>
/// 登录二维码缓存
/// </summary>
[Serializable]
[IgnoreMultiTenancy]
public class LoginQrCodeCacheItem
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public LoginQrCodeCacheItem()
    {
        QrCodeStatus = QrCodeStatus.Normal;
    }

    /// <summary>
    /// 二维码状态
    /// </summary>
    public QrCodeStatus QrCodeStatus { get; protected set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid? UserId { get; protected set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 扫码登录Token
    /// </summary>
    public string ScanCodeToken { get; protected set; }

    public LoginQrCodeCacheItem ScanCode(Guid userId, Guid? tenantId)
    {
        QrCodeStatus = QrCodeStatus.ScanedCode;
        UserId = userId;
        TenantId = tenantId;
        return this;
    }

    public LoginQrCodeCacheItem Confirm(string scanCodeToken)
    {
        QrCodeStatus = QrCodeStatus.Confirmed;
        ScanCodeToken = scanCodeToken;
        return this;
    }

    /*
    public override int GetHashCode()
    {
        return (QrCodeKey, UserId, TenantId).GetHashCode();
    }
    */

    public LoginQrCode ToLoginQrCode()
    {
        return new LoginQrCode { QrCodeStatus = QrCodeStatus, ScanCodeToken = ScanCodeToken };
    }
}

/// <summary>
/// 二维码状态
/// </summary>
public enum QrCodeStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 失效
    /// </summary>
    Invalid,

    /// <summary>
    /// 已被扫码
    /// </summary>
    ScanedCode,

    /// <summary>
    /// 已被确认
    /// </summary>
    Confirmed,
}
