// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp;
using Volo.Abp.DependencyInjection;

using X.Abp.Identity.Settings;

namespace X.Abp.Identity.ExternalLoginProviders.OAuth;

public class OAuthExternalLoginManager : ITransientDependency
{
    public const string HttpClientName = "OAuthExternalLoginManager";

    protected ILogger<OAuthExternalLoginManager> Logger { get; set; }

    protected IOAuthSettingProvider OAuthSettingProvider { get; }

    protected IHttpClientFactory HttpClientFactory { get; }

    public OAuthExternalLoginManager(
        IOAuthSettingProvider oAuthSettingProvider,
        IHttpClientFactory httpClientFactory)
    {
        OAuthSettingProvider = oAuthSettingProvider;
        HttpClientFactory = httpClientFactory;
        Logger = NullLogger<OAuthExternalLoginManager>.Instance;
    }

    public virtual async Task<bool> AuthenticateAsync(string userName, string password)
    {
        try
        {
            await GetAccessTokenAsync(userName, password);
            return true;
        }
        catch (AbpException ex)
        {
            Logger.LogException(ex);
            return false;
        }
    }

    public virtual async Task<IEnumerable<Claim>> GetUserInfoAsync(string userName, string password)
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            var token = await GetAccessTokenAsync(userName, password);
            var discoveryResponse = await GetDiscoveryResponseAsync();

            using (var request = new UserInfoRequest
            {
                Token = token,
                Address = discoveryResponse.UserInfoEndpoint
            })
            {
                var userinfoResponse = await httpClient.GetUserInfoAsync(request);

                return userinfoResponse.IsError
                    ? throw userinfoResponse.Exception ?? new AbpException("Get user info error: " + userinfoResponse.Raw)
                    : userinfoResponse.Claims;
            }
        }
    }

    protected virtual async Task<string> GetAccessTokenAsync(string userName, string password)
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            var discoveryResponse = await GetDiscoveryResponseAsync();

            using (var request = new PasswordTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = await OAuthSettingProvider.GetClientIdAsync(),
                ClientSecret = await OAuthSettingProvider.GetClientSecretAsync(),
                Scope = await OAuthSettingProvider.GetScopeAsync(),
                UserName = userName,
                Password = password
            })
            {
                var tokenResponse = await httpClient.RequestPasswordTokenAsync(request);
                return tokenResponse.IsError
                    ? throw tokenResponse.Exception ?? new AbpException("Get access token error: " + tokenResponse.Raw)
                    : tokenResponse.AccessToken;
            }
        }
    }

    protected virtual async Task<DiscoveryDocumentResponse> GetDiscoveryResponseAsync()
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            using (var request = new DiscoveryDocumentRequest
            {
                Address = await OAuthSettingProvider.GetAuthorityAsync(),
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = await OAuthSettingProvider.GetRequireHttpsMetadataAsync()
                }
            })
            {
                var discoveryResponse = await httpClient.GetDiscoveryDocumentAsync(request);
                return discoveryResponse.IsError
                    ? throw discoveryResponse.Exception ?? new AbpException("Get discovery error: " + discoveryResponse.Raw)
                    : discoveryResponse;
            }
        }
    }
}
