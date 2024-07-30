// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Web;

public static class ExternalLoginConsts
{
    /// <summary>
    /// 扩展登录HttpClient名称
    /// </summary>
    public const string ExternalLoginHttpClientName = "IdentityServerExternalLoginHttpClient";

    /// <summary>
    /// Code
    /// </summary>
    public const string Code = "code";

    /// <summary>
    /// 手机号
    /// </summary>
    public const string PhoneNumber = "phone_number";

    /// <summary>
    /// 手机验证码
    /// </summary>
    public const string VerifyCode = "verify_code";

    /// <summary>
    /// 验证额外参数
    /// </summary>
    public const string VerifyParameter = "verify_parameter";

    /// <summary>
    /// Spa扩展登录第三方id
    /// </summary>
    public const string SpaExternalLoginProviderKey = "SpaExternalLoginProviderKey";

    /// <summary>
    /// Spa扩展登录第三方名称
    /// </summary>
    public const string SpaExternalLoginProviderName = "login_provider";

    /// <summary>
    /// Spa扩展登录租户Id
    /// </summary>
    public const string SpaExternalLoginTenantId = "SpaExternalLoginTenantId";

    /// <summary>
    /// 扫码用户Id
    /// </summary>
    public const string ScanCodeLoginUserId = "ScanCodeUserId";

    /// <summary>
    /// 扫码租户Id
    /// </summary>
    public const string ScanCodeLoginTenantId = "ScanCodeTenantId";

    /// <summary>
    /// 是否已有账号
    /// </summary>
    public const string IsHasAccount = "is_has_account";

    /// <summary>
    /// 是否创建新账号
    /// </summary>
    public const string IsCreateNewAccount = "is_create_account";
}
