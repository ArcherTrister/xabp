﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AbpVnext.Pro.Customs;

/// <summary>
/// Default token service
/// </summary>
public class CustomDefaultTokenService : ITokenService
{
    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The HTTP context accessor
    /// </summary>
    protected IHttpContextAccessor ContextAccessor { get; }

    /// <summary>
    /// The claims provider
    /// </summary>
    protected IClaimsService ClaimsProvider { get; }

    /// <summary>
    /// The reference token store
    /// </summary>
    protected IReferenceTokenStore ReferenceTokenStore { get; }

    /// <summary>
    /// The signing service
    /// </summary>
    protected ITokenCreationService CreationService { get; }

    /// <summary>
    /// The clock
    /// </summary>
    protected ISystemClock Clock { get; }

    /// <summary>
    /// The key material service
    /// </summary>
    protected IKeyMaterialService KeyMaterialService { get; }

    /// <summary>
    /// The IdentityServer options
    /// </summary>
    protected IdentityServerOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTokenService" /> class.
    /// </summary>
    /// <param name="claimsProvider">The claims provider.</param>
    /// <param name="referenceTokenStore">The reference token store.</param>
    /// <param name="creationService">The signing service.</param>
    /// <param name="contextAccessor">The HTTP context accessor.</param>
    /// <param name="clock">The clock.</param>
    /// <param name="keyMaterialService">keyMaterialService</param>
    /// <param name="options">The IdentityServer options</param>
    /// <param name="logger">The logger.</param>
    public CustomDefaultTokenService(
        IClaimsService claimsProvider,
        IReferenceTokenStore referenceTokenStore,
        ITokenCreationService creationService,
        IHttpContextAccessor contextAccessor,
        ISystemClock clock,
        IKeyMaterialService keyMaterialService,
        IdentityServerOptions options,
        ILogger<DefaultTokenService> logger)
    {
        ContextAccessor = contextAccessor;
        ClaimsProvider = claimsProvider;
        ReferenceTokenStore = referenceTokenStore;
        CreationService = creationService;
        Clock = clock;
        KeyMaterialService = keyMaterialService;
        Options = options;
        Logger = logger;
    }

    /// <summary>
    /// Creates an identity token.
    /// </summary>
    /// <param name="request">The token creation request.</param>
    /// <returns>
    /// An identity token
    /// </returns>
    public virtual async Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
    {
        Logger.LogTrace("Creating identity token");
        request.Validate();

        // todo: Dom, add a test for this. validate the at and c hashes are correct for the id_token when the client's alg doesn't match the server default.
        var credential = await KeyMaterialService.GetSigningCredentialsAsync(request.ValidatedRequest.Client.AllowedIdentityTokenSigningAlgorithms);
        if (credential == null)
        {
            throw new InvalidOperationException("No signing credential is configured.");
        }

        var signingAlgorithm = credential.Algorithm;

        // host provided claims
        var claims = new List<Claim>();

        // if nonce was sent, must be mirrored in id token
        if (request.Nonce.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.Nonce, request.Nonce));
        }

        // add iat claim
        claims.Add(new Claim(JwtClaimTypes.IssuedAt, Clock.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        // add at_hash claim
        if (request.AccessTokenToHash.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.AccessTokenHash, CryptoHelper.CreateHashClaimValue(request.AccessTokenToHash, signingAlgorithm)));
        }

        // add c_hash claim
        if (request.AuthorizationCodeToHash.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.AuthorizationCodeHash, CryptoHelper.CreateHashClaimValue(request.AuthorizationCodeToHash, signingAlgorithm)));
        }

        // add s_hash claim
        if (request.StateHash.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.StateHash, request.StateHash));
        }

        // add sid if present
        if (request.ValidatedRequest.SessionId.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.SessionId, request.ValidatedRequest.SessionId));
        }

        claims.AddRange(await ClaimsProvider.GetIdentityTokenClaimsAsync(
            request.Subject,
            request.ValidatedResources,
            request.IncludeAllIdentityClaims,
            request.ValidatedRequest));

        var issuer = ContextAccessor.HttpContext.GetIdentityServerIssuerUri();

        var token = new Token(OidcConstants.TokenTypes.IdentityToken)
        {
            CreationTime = Clock.UtcNow.UtcDateTime,
            Audiences = { request.ValidatedRequest.Client.ClientId },
            Issuer = issuer,
            Lifetime = request.ValidatedRequest.Client.IdentityTokenLifetime,
            Claims = claims.Distinct(new ClaimComparer()).ToList(),
            ClientId = request.ValidatedRequest.Client.ClientId,
            AccessTokenType = request.ValidatedRequest.AccessTokenType,
            AllowedSigningAlgorithms = request.ValidatedRequest.Client.AllowedIdentityTokenSigningAlgorithms
        };

        return token;
    }

    /// <summary>
    /// Creates an access token.
    /// </summary>
    /// <param name="request">The token creation request.</param>
    /// <returns>
    /// An access token
    /// </returns>
    public virtual async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
    {
        Logger.LogTrace("Creating access token");
        request.Validate();

        var claims = new List<Claim>();
        claims.AddRange(await ClaimsProvider.GetAccessTokenClaimsAsync(
            request.Subject,
            request.ValidatedResources,
            request.ValidatedRequest));

        if (request.ValidatedRequest.Client.IncludeJwtId)
        {
            claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)));
        }

        if (request.ValidatedRequest.SessionId.IsPresent())
        {
            claims.Add(new Claim(JwtClaimTypes.SessionId, request.ValidatedRequest.SessionId));
        }

        // iat claim as required by JWT profile
        claims.Add(new Claim(JwtClaimTypes.IssuedAt, Clock.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        var issuer = ContextAccessor.HttpContext.GetIdentityServerIssuerUri();
        var token = new Token(OidcConstants.TokenTypes.AccessToken)
        {
            CreationTime = Clock.UtcNow.UtcDateTime,
            Issuer = issuer,
            Lifetime = request.ValidatedRequest.AccessTokenLifetime,
            Claims = claims.Distinct(new ClaimComparer()).ToList(),
            ClientId = request.ValidatedRequest.Client.ClientId,
            Description = request.Description,
            AccessTokenType = request.ValidatedRequest.AccessTokenType,
            AllowedSigningAlgorithms = request.ValidatedResources.Resources.ApiResources.FindMatchingSigningAlgorithms()
        };

        // add aud based on ApiResources in the validated request
        foreach (var aud in request.ValidatedResources.Resources.ApiResources.Select(x => x.Name).Distinct())
        {
            token.Audiences.Add(aud);
        }

        if (Options.EmitStaticAudienceClaim)
        {
            token.Audiences.Add(string.Format(IdentityServerConstants.AccessTokenAudience, issuer.EnsureTrailingSlash()));
        }

        // add cnf if present
        if (request.ValidatedRequest.Confirmation.IsPresent())
        {
            token.Confirmation = request.ValidatedRequest.Confirmation;
        }
        else
        {
            if (Options.MutualTls.AlwaysEmitConfirmationClaim)
            {
                var clientCertificate = await ContextAccessor.HttpContext.Connection.GetClientCertificateAsync();
                if (clientCertificate != null)
                {
                    token.Confirmation = clientCertificate.CreateThumbprintCnf();
                }
            }
        }

        return token;
    }

    /// <summary>
    /// Creates a serialized and protected security token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>
    /// A security token in serialized form
    /// </returns>
    /// <exception cref="InvalidOperationException">Invalid token type.</exception>
    public virtual async Task<string> CreateSecurityTokenAsync(Token token)
    {
        string tokenResult;

        if (token.Type == OidcConstants.TokenTypes.AccessToken)
        {
            if (token.AccessTokenType == AccessTokenType.Jwt)
            {
                Logger.LogTrace("Creating JWT access token");

                tokenResult = await CreationService.CreateTokenAsync(token);
            }
            else
            {
                Logger.LogTrace("Creating reference access token");

                var handle = await ReferenceTokenStore.StoreReferenceTokenAsync(token);

                tokenResult = handle;
            }
        }
        else if (token.Type == OidcConstants.TokenTypes.IdentityToken)
        {
            Logger.LogTrace("Creating JWT identity token");

            tokenResult = await CreationService.CreateTokenAsync(token);
        }
        else
        {
            throw new InvalidOperationException("Invalid token type.");
        }

        return tokenResult;
    }
}
