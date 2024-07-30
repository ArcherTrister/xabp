// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public interface ITokenGeneratorProvider
{
    /*
    /// <summary>
    /// 创建Jwt AccessToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="clientId">客户端Id</param>
    /// <param name="audiences">aud声明</param>
    /// <param name="scopes">授权范围</param>
    /// <param name="isExternalLogin">是否扩展登录</param>
    /// <returns>accessToken, refreshToken?</returns>
    Task<TokenGeneratorResult> CreateAccessTokenAsync(IdentityUser user, string clientId, ICollection<string> audiences, IEnumerable<string> scopes, bool isExternalLogin);
    */

    /// <summary>
    /// 创建Jwt AccessToken
    /// </summary>
    /// <param name="loginProvider">第三方提供者</param>
    /// <param name="providerKey">第三方id</param>
    /// <param name="tenantId">租户Id</param>
    /// <param name="clientId">客户端Id</param>
    /// <param name="clientSecret">客户端Secret</param>
    /// <param name="scope">授权范围</param>
    Task<TokenGeneratorResult> CreateSpaExternalLoginAccessTokenAsync(string loginProvider, string providerKey, Guid? tenantId, string clientId, string clientSecret, string scope);

    /// <summary>
    /// 创建Jwt AccessToken
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="tenantId">用户所属租户Id</param>
    /// <param name="clientId">客户端Id</param>
    /// <param name="clientSecret">客户端Secret</param>
    /// <param name="scope">授权范围</param>
    Task<TokenGeneratorResult> CreateScanCodeLoginAccessTokenAsync(Guid userId, Guid? tenantId, string clientId, string clientSecret, string scope);
}
