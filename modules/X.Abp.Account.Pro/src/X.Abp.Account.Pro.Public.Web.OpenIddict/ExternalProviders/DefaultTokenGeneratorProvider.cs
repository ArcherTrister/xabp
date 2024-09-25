// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.DependencyInjection;

using X.Abp.Account.Public.Web.ExternalProviders;

namespace X.Abp.Account.Web.ExternalProviders;

public class DefaultTokenGeneratorProvider : ITokenGeneratorProvider, ITransientDependency
{
    /*
     * GenerateTokenContext
     * GenerateIdentityModelToken
     * See https://github.com/openiddict/openiddict-core/blob/1f630eb4d8f18d52d373ebaec02ebbb9e47760e9/src/OpenIddict.Server/OpenIddictServerHandlers.Protection.cs#L1466
    protected AbpUserClaimsPrincipalFactory ClaimsPrincipalFactory { get; }

    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger Logger { get; }

    public DefaultTokenGeneratorProvider(
        AbpUserClaimsPrincipalFactory claimsPrincipalFactory,
        ILogger<DefaultTokenGeneratorProvider> logger)
    {
        ClaimsPrincipalFactory = claimsPrincipalFactory;
        Logger = logger;
    }

    /// <summary>
    /// 创建Jwt AccessToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="clientId">客户端Id</param>
    /// <param name="audiences">aud声明</param>
    /// <param name="scopes">授权范围</param>
    /// <param name="isExternalLogin">是否扩展登录</param>
    /// <returns>accessToken, refreshToken?</returns>
    public virtual Task<TokenGeneratorResult> CreateAccessTokenAsync(IdentityUser user, string clientId, ICollection<string> audiences, IEnumerable<string> scopes, bool isExternalLogin)
    {
        // TODO: CreateAccessTokenAsync
        throw new NotImplementedException();
    }
    */

    public virtual Task<TokenGeneratorResult> CreateSpaExternalLoginAccessTokenAsync(string loginProvider, string providerKey, Guid? tenantId, string clientId, string clientSecret, string scope)
  {
    throw new NotImplementedException();
  }

  public virtual Task<TokenGeneratorResult> CreateScanCodeLoginAccessTokenAsync(Guid userId, Guid? tenantId, string clientId, string clientSecret, string scope)
  {
    throw new NotImplementedException();
  }
}
