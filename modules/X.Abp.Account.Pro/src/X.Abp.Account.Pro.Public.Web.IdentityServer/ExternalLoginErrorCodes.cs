// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Web;

public static class ExternalLoginErrorCodes
{
    // Add your business exception error codes here...

    /// <summary>
    /// code不能为空
    /// </summary>
    public const string CodeCannotBeEmpty = "X.Abp.IdentityServer:External:010001";

    /// <summary>
    /// 未绑定系统用户
    /// </summary>
    public const string UnboundSystemUser = "X.Abp.IdentityServer:External:010002";

    /// <summary>
    /// 登录失败
    /// </summary>
    public const string LoginFailed = "X.Abp.IdentityServer:External:010003";

    /// <summary>
    /// 检索用户信息时出错
    /// </summary>
    public const string ErrorRetrievingUserInformation = "X.Abp.IdentityServer:External:010004";

    /// <summary>
    /// 参数错误
    /// </summary>
    public const string ParameterError = "X.Abp.IdentityServer:External:010005";

    /// <summary>
    /// 无效的验证码
    /// </summary>
    public const string InvalidVerificationCode = "X.Abp.IdentityServer:External:010006";

    /// <summary>
    /// 创建新用户失败
    /// </summary>
    public const string FailedToCreateNewUser = "X.Abp.IdentityServer:External:010007";

    /// <summary>
    /// 用户不存在
    /// </summary>
    public const string UserDoesNotExist = "X.Abp.IdentityServer:External:010008";

    /// <summary>
    /// 第三方提供者密钥不能为空
    /// </summary>
    public const string ProviderKeyCannotBeEmpty = "X.Abp.IdentityServer:External:010009";

    /// <summary>
    /// 第三方提供者名称不能为空
    /// </summary>
    public const string ProviderNameCannotBeEmpty = "X.Abp.IdentityServer:External:010010";

    /// <summary>
    /// 未找到第三方登录信息
    /// </summary>
    public const string ExternalLoginInfoIsNotAvailable = "X.Abp.IdentityServer:External:010011";

    /// <summary>
    /// 未找到要绑定的用户信息
    /// </summary>
    public const string UserInformationToBeBoundWasNotFound = "X.Abp.IdentityServer:External:010012";

    /// <summary>
    /// 获取微信用户信息失败
    /// </summary>
    public const string FailedToGetWeChatUserInformation = "X.Abp.IdentityServer:External:010013";
}
