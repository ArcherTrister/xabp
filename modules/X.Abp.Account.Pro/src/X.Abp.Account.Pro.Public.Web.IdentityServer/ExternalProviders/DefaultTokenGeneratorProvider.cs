// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Net.Http;
using System.Threading.Tasks;

using IdentityServer4.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

using X.Abp.Account.Public.Web.ExternalProviders;
using X.Abp.Account.Web.Extensions;

namespace X.Abp.Account.Web.ExternalProviders;

public class DefaultTokenGeneratorProvider : ITokenGeneratorProvider, ITransientDependency
{
    protected IHttpClientFactory HttpClientFactory { get; }

    protected IdentityServerOptions IdentityServerOptions { get; }

    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger Logger { get; }

    public DefaultTokenGeneratorProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<IdentityServerOptions> options,
        ILogger<DefaultTokenGeneratorProvider> logger)
    {
        HttpClientFactory = httpClientFactory;
        IdentityServerOptions = options.Value;
        Logger = logger;
    }

    public virtual async Task<TokenGeneratorResult> CreateSpaExternalLoginAccessTokenAsync(string loginProvider, string providerKey, Guid? tenantId, string clientId, string clientSecret, string scope)
    {
        var client = HttpClientFactory.CreateClient(ExternalLoginConsts.ExternalLoginHttpClientName);

        using (var tokenRequest = new SpaExternalLoginTokenRequest
        {
            Address = $"{IdentityServerOptions.IssuerUri}/connect/token",
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scope = scope,
            LoginProvider = loginProvider,
            ProviderKey = providerKey,
            TenantId = tenantId,
        })
        {
            var tokenResponse = await client.RequestSpaExternalLoginTokenAsync(tokenRequest);
            if (tokenResponse.IsError)
            {
                return new TokenGeneratorResult(tokenResponse.Error);
            }

            return new TokenGeneratorResult(tokenResponse.AccessToken, tokenResponse.ExpiresIn, tokenResponse.TokenType, tokenResponse.Scope, tokenResponse.RefreshToken);
        }
    }

    public virtual async Task<TokenGeneratorResult> CreateScanCodeLoginAccessTokenAsync(Guid userId, Guid? tenantId, string clientId, string clientSecret, string scope)
    {
        var client = HttpClientFactory.CreateClient(ExternalLoginConsts.ExternalLoginHttpClientName);

        using (var tokenRequest = new ScanCodeLoginTokenRequest
        {
            Address = $"{IdentityServerOptions.IssuerUri}/connect/token",
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scope = scope,
            UserId = userId,
            TenantId = tenantId,
        })
        {
            var tokenResponse = await client.RequestScanCodeLoginTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
            {
                return new TokenGeneratorResult(tokenResponse.Error);
            }

            return new TokenGeneratorResult(tokenResponse.AccessToken, tokenResponse.ExpiresIn, tokenResponse.TokenType, tokenResponse.Scope, tokenResponse.RefreshToken);
        }
    }
}
