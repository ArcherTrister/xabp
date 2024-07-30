// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public interface ISmsVerifyCodeProvider
{
    /// <summary>
    /// 存储验证码，用于验证其有效性
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="smsVerifyCode">验证码</param>
    /// <param name="verifyParameter">额外参数</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveAsync(string phoneNumber, string smsVerifyCode, string verifyParameter);

    /// <summary>
    /// 验证验证码有效性
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="smsVerifyCode">验证码</param>
    /// <param name="verifyParameter">额外参数</param>
    /// <returns>A <see cref="bool"/> representing the asynchronous operation.</returns>
    Task<bool> CheckAsync(string phoneNumber, string smsVerifyCode, string verifyParameter);
}
