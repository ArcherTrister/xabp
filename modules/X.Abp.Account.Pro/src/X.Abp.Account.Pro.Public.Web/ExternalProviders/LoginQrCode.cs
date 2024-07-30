// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Public.Web.ExternalProviders;

/// <summary>
/// 登录二维码实体
/// </summary>
[Serializable]
public class LoginQrCode
{
    /// <summary>
    /// 二维码状态
    /// </summary>
    public QrCodeStatus QrCodeStatus { get; set; }

    /// <summary>
    /// 扫码登录Token
    /// </summary>
    public string ScanCodeToken { get; set; }
}
