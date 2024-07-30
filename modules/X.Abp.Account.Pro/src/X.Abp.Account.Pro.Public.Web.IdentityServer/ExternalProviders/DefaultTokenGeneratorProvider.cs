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

    /*
    protected AbpUserClaimsPrincipalFactory ClaimsPrincipalFactory { get; }

    protected ITokenCreationService TokenCreationService { get; }

    protected IRefreshTokenService RefreshTokenService { get; }

    protected IClientStore ClientStore { get; }
    */

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
    public virtual async Task<TokenGeneratorResult> CreateAccessTokenAsync(IdentityUser user, string clientId, ICollection<string> audiences, IEnumerable<string> scopes, bool isExternalLogin)
    {
        var client = await ClientStore.FindEnabledClientByIdAsync(clientId);
        if (client == null)
        {
            Logger.LogError("Unknown or disabled client: {ClientId}.", clientId);
            throw new UserFriendlyException(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        var principal = await ClaimsPrincipalFactory.CreateAsync(user);

        if (!scopes.Any())
        {
            scopes = new List<string> { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.OfflineAccess };
        }

        var claims = principal.Claims.ToList();

        foreach (var scope in scopes)
        {
            claims.Add(new Claim(JwtClaimTypes.Scope, scope));
        }

        // iat claim as required by JWT profile
        var unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);
        claims.Add(new Claim(JwtClaimTypes.IssuedAt, unixTimeSeconds, ClaimValueTypes.Integer64));
        claims.Add(new Claim(JwtClaimTypes.AuthenticationTime, unixTimeSeconds, ClaimValueTypes.Integer64));

        if (isExternalLogin)
        {
            const string ExternalAuthenticationMethod = "external";
            claims.Add(new Claim(JwtClaimTypes.AuthenticationMethod, ExternalAuthenticationMethod));
            claims.Add(new Claim(JwtClaimTypes.IdentityProvider, user.Logins.Select(p => p.LoginProvider).First()));
        }
        else
        {
            if (user.TwoFactorEnabled)
            {
                claims.Add(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.MultiFactorAuthentication));
            }
            else
            {
                claims.Add(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
            }

            claims.Add(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
        }

        claims.Add(new Claim(JwtClaimTypes.ClientId, clientId));

        var issuer = IdentityServerOptions.IssuerUri;

        // JwtId
        var token = new Token(OidcConstants.TokenTypes.AccessToken)
        {
            CreationTime = DateTime.UtcNow,
            Audiences = audiences,
            Issuer = issuer,
            Lifetime = client.AccessTokenLifetime,
            ClientId = clientId,
            AccessTokenType = AccessTokenType.Jwt,
            Claims = claims.Distinct(new ClaimComparer()).ToList(),

            // Description = request.Description,
        };
        var accessToken = await TokenCreationService.CreateTokenAsync(token);

        var offline = scopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess);
        if (offline)
        {
            Logger.LogDebug("Client found: {ClientId} / {ClientName}", client.ClientId, client.ClientName);
            var refreshToken = await RefreshTokenService.CreateRefreshTokenAsync(principal, token, client);

            return new TokenGeneratorResult(accessToken, client.AccessTokenLifetime, "Bearer", scopes.JoinAsString(" "), refreshToken);
        }

        return new TokenGeneratorResult(accessToken, client.AccessTokenLifetime, "Bearer", scopes.JoinAsString(" "), null);
    }
    */

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

    /*
    ///// <summary>
    ///// Creates an access token.
    ///// </summary>
    ///// <param name="request">The token creation request.</param>
    ///// <returns>
    ///// An access token
    ///// </returns>
    //public virtual async Task<Token> CreateAccessTokenAsync(Client client)
    //{
    //    Logger.LogTrace("Creating access token");

    //    var claims = new List<Claim>();
    //    claims.AddRange(await GetAccessTokenClaimsAsync(
    //        request.Subject,
    //        request.ValidatedResources,
    //        request.ValidatedRequest));

    //    if (request.ValidatedRequest.Client.IncludeJwtId)
    //    {
    //        claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)));
    //    }

    //    if (request.ValidatedRequest.SessionId.IsPresent())
    //    {
    //        claims.Add(new Claim(JwtClaimTypes.SessionId, request.ValidatedRequest.SessionId));
    //    }

    //    // iat claim as required by JWT profile
    //    claims.Add(new Claim(JwtClaimTypes.IssuedAt, Clock.Now.ToUnixTimeSeconds().ToString(),
    //        ClaimValueTypes.Integer64));

    //    var issuer = IdentityServerOptions.IssuerUri;
    //    var token = new Token(OidcConstants.TokenTypes.AccessToken)
    //    {
    //        CreationTime = Clock.UtcNow.UtcDateTime,
    //        Issuer = issuer,
    //        Lifetime = request.ValidatedRequest.AccessTokenLifetime,
    //        Claims = claims.Distinct(new ClaimComparer()).ToList(),
    //        ClientId = request.ValidatedRequest.Client.ClientId,
    //        Description = request.Description,
    //        AccessTokenType = request.ValidatedRequest.AccessTokenType,
    //        AllowedSigningAlgorithms = request.ValidatedResources.Resources.ApiResources.FindMatchingSigningAlgorithms()
    //    };

    //    // add aud based on ApiResources in the validated request
    //    foreach (var aud in request.ValidatedResources.Resources.ApiResources.Select(x => x.Name).Distinct())
    //    {
    //        token.Audiences.Add(aud);
    //    }

    //    if (IdentityServerOptions.EmitStaticAudienceClaim)
    //    {
    //        token.Audiences.Add(string.Format(IdentityServerConstants.AccessTokenAudience, issuer.EnsureTrailingSlash()));
    //    }

    //    // add cnf if present
    //    if (request.ValidatedRequest.Confirmation.IsPresent())
    //    {
    //        token.Confirmation = request.ValidatedRequest.Confirmation;
    //    }
    //    else
    //    {
    //        if (Options.MutualTls.AlwaysEmitConfirmationClaim)
    //        {
    //            var clientCertificate = await ContextAccessor.HttpContext.Connection.GetClientCertificateAsync();
    //            if (clientCertificate != null)
    //            {
    //                token.Confirmation = clientCertificate.CreateThumbprintCnf();
    //            }
    //        }
    //    }

    //    return token;
    //}


    /// <summary>
    /// Returns claims for an access token.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="client">The client.</param>
    /// <returns>
    /// Claims for the access token
    /// </returns>
    public virtual async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client) // ResourceValidationResult resourceResult, ValidatedRequest request)
    {
        Logger.LogDebug($"Getting claims for access token for client: {client.ClientId}");

        var outputClaims = new List<Claim>
        {
            new Claim(JwtClaimTypes.ClientId, client.ClientId)
        };

        // check for client claims
        if (client.Claims != null && client.Claims.Any())
        {
            if (subject == null || client.AlwaysSendClientClaims)
            {
                foreach (var claim in client.Claims)
                {
                    var claimType = claim.Type;

                    if (!client.ClientClaimsPrefix.IsNullOrWhiteSpace())
                    {
                        claimType = client.ClientClaimsPrefix + claimType;
                    }

                    outputClaims.Add(new Claim(claimType, claim.Value, claim.ValueType));
                }
            }
        }

        // add scopes (filter offline_access)
        // we use the ScopeValues collection rather than the Resources.Scopes because we support dynamic scope values
        // from the request, so this issues those in the token.
        foreach (var scope in resourceResult.RawScopeValues.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess))
        {
            outputClaims.Add(new Claim(JwtClaimTypes.Scope, scope));
        }

        // a user is involved
        if (subject != null)
        {
            if (resourceResult.Resources.OfflineAccess)
            {
                outputClaims.Add(new Claim(JwtClaimTypes.Scope, IdentityServerConstants.StandardScopes.OfflineAccess));
            }

            Logger.LogDebug($"Getting claims for access token for subject: {subject.GetSubjectId()}");

            outputClaims.AddRange(GetStandardSubjectClaims(subject));
            outputClaims.AddRange(GetOptionalClaims(subject));

            // fetch all resource claims that need to go into the access token
            var additionalClaimTypes = new List<string>();
            foreach (var api in resourceResult.Resources.ApiResources)
            {
                // add claims configured on api resource
                if (api.UserClaims != null)
                {
                    foreach (var claim in api.UserClaims)
                    {
                        additionalClaimTypes.Add(claim);
                    }
                }
            }

            foreach (var scope in resourceResult.Resources.ApiScopes)
            {
                // add claims configured on scopes
                if (scope.UserClaims != null)
                {
                    foreach (var claim in scope.UserClaims)
                    {
                        additionalClaimTypes.Add(claim);
                    }
                }
            }

            // filter so we don't ask for claim types that we will eventually filter out
            additionalClaimTypes = FilterRequestedClaimTypes(additionalClaimTypes).ToList();

            var context = new ProfileDataRequestContext(
                subject,
                client,
                IdentityServerConstants.ProfileDataCallers.ClaimsProviderAccessToken,
                additionalClaimTypes.Distinct())
            {
                RequestedResources = resourceResult,
                ValidatedRequest = request
            };

            await Profile.GetProfileDataAsync(context);

            var claims = FilterProtocolClaims(context.IssuedClaims);
            if (claims != null)
            {
                outputClaims.AddRange(claims);
            }
        }

        return outputClaims;
    }
    */
}
