// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.IdentityServer;
using Volo.Abp.Security.Claims;

namespace X.Abp.Account.Web.Services;

public class CustomClaimsService : DefaultClaimsService
{
    protected AbpClaimsServiceOptions Options { get; }

    private static readonly string[] AdditionalOptionalClaimNames =
    {
            AbpClaimTypes.TenantId,
            AbpClaimTypes.ImpersonatorTenantId,
            AbpClaimTypes.ImpersonatorUserId,
            AbpClaimTypes.Name,
            AbpClaimTypes.SurName,
            JwtRegisteredClaimNames.UniqueName,
            JwtClaimTypes.PreferredUserName,
            JwtClaimTypes.GivenName,
            JwtClaimTypes.FamilyName,
    };

    private static readonly string[] ClaimsServiceFilterClaimTypes =
        {
                // TODO: consider JwtClaimTypes.AuthenticationContextClassReference,
                JwtClaimTypes.AccessTokenHash,
                JwtClaimTypes.Audience,
                JwtClaimTypes.AuthenticationMethod,
                JwtClaimTypes.AuthenticationTime,
                JwtClaimTypes.AuthorizedParty,
                JwtClaimTypes.AuthorizationCodeHash,
                JwtClaimTypes.ClientId,
                JwtClaimTypes.Expiration,
                JwtClaimTypes.IdentityProvider,
                JwtClaimTypes.IssuedAt,
                JwtClaimTypes.Issuer,
                JwtClaimTypes.JwtId,
                JwtClaimTypes.Nonce,
                JwtClaimTypes.NotBefore,
                JwtClaimTypes.ReferenceTokenId,
                JwtClaimTypes.SessionId,
                JwtClaimTypes.Subject,
                JwtClaimTypes.Scope,
                JwtClaimTypes.Confirmation
        };

    public CustomClaimsService(
        IProfileService profile,
        ILogger<DefaultClaimsService> logger,
        IOptions<AbpClaimsServiceOptions> options)
        : base(profile, logger)
    {
        Options = options.Value;
    }

    public override async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resources, bool includeAllIdentityClaims, ValidatedRequest request)
    {
        // return base.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request);
        Logger.LogDebug("Getting claims for identity token for subject: {SubjectId} and client: {ClientId}", subject.GetSubjectId(), request.Client.ClientId);

        var outputClaims = new List<Claim>(GetStandardSubjectClaims(subject));
        outputClaims.AddRange(GetOptionalClaims(subject));

        // fetch all identity claims that need to go into the id token
        if (includeAllIdentityClaims || request.Client.AlwaysIncludeUserClaimsInIdToken)
        {
            var additionalClaimTypes = new List<string>();

            foreach (var identityResource in resources.Resources.IdentityResources)
            {
                foreach (var userClaim in identityResource.UserClaims)
                {
                    additionalClaimTypes.Add(userClaim);
                }
            }

            // filter so we don't ask for claim types that we will eventually filter out
            additionalClaimTypes = FilterRequestedClaimTypes(additionalClaimTypes).ToList();

            var context = new ProfileDataRequestContext(
                subject,
                request.Client,
                IdentityServerConstants.ProfileDataCallers.ClaimsProviderIdentityToken,
                additionalClaimTypes)
            {
                RequestedResources = resources,
                ValidatedRequest = request
            };

            await Profile.GetProfileDataAsync(context);

            var claims = FilterProtocolClaims(context.IssuedClaims);
            if (claims != null)
            {
                outputClaims.AddRange(claims);
            }
        }
        else
        {
            Logger.LogDebug("In addition to an id_token, an access_token was requested. No claims other than sub are included in the id_token. To obtain more user claims, either use the user info endpoint or set AlwaysIncludeUserClaimsInIdToken on the client configuration.");
        }

        return outputClaims;
    }

    public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resourceResult, ValidatedRequest request)
    {
        // return base.GetAccessTokenClaimsAsync(subject, resourceResult, request);
        Logger.LogDebug("Getting claims for access token for client: {ClientId}", request.Client.ClientId);

        var outputClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.ClientId, request.ClientId)
            };

        // log if client ID is overwritten
        if (!string.Equals(request.ClientId, request.Client.ClientId, System.StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogDebug("Client {Client} is impersonating {ClientId}", request.Client.ClientId, request.ClientId);
        }

        // check for client claims
        if (request.ClientClaims != null && request.ClientClaims.Count != 0)
        {
            if (subject == null || request.Client.AlwaysSendClientClaims)
            {
                foreach (var claim in request.ClientClaims)
                {
                    var claimType = claim.Type;

                    if (!request.Client.ClientClaimsPrefix.IsNullOrWhiteSpace())
                    {
                        claimType = request.Client.ClientClaimsPrefix + claimType;
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

            Logger.LogDebug("Getting claims for access token for subject: {SubjectId}", subject.GetSubjectId());

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
                request.Client,
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

    protected override IEnumerable<Claim> GetStandardSubjectClaims(ClaimsPrincipal subject)
    {
        // return base.GetStandardSubjectClaims(subject);
        var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, subject.GetSubjectId()),
                new Claim(JwtClaimTypes.AuthenticationTime, subject.GetAuthenticationTimeEpoch().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtClaimTypes.IdentityProvider, subject.GetIdentityProvider())
            };

        claims.AddRange(subject.GetAuthenticationMethods());

        return claims;
    }

    protected override IEnumerable<Claim> GetOptionalClaims(ClaimsPrincipal subject)
    {
        // return base.GetOptionalClaims(subject).Union(GetAdditionalOptionalClaims(subject));
        var claims = new List<Claim>();
        var acr = subject.FindFirst(JwtClaimTypes.AuthenticationContextClassReference);
        if (acr != null)
        {
            claims.Add(acr);
        }

        return claims.Union(GetAdditionalOptionalClaims(subject));
    }

    protected override IEnumerable<Claim> FilterProtocolClaims(IEnumerable<Claim> claims)
    {
        // return base.FilterProtocolClaims(claims);
        var claimsToFilter = claims.Where(x => ClaimsServiceFilterClaimTypes.Contains(x.Type));
        if (claimsToFilter.Any())
        {
            var types = claimsToFilter.Select(x => x.Type);
            Logger.LogDebug("Claim types from profile service that were filtered: {Types}", types);
        }

        return claims.Except(claimsToFilter);
    }

    protected override IEnumerable<string> FilterRequestedClaimTypes(IEnumerable<string> claimTypes)
    {
        // return base.FilterRequestedClaimTypes(claimTypes).Union(Options.RequestedClaims);
        var claimTypesToFilter = claimTypes.Where(x => ClaimsServiceFilterClaimTypes.Contains(x));
        var exceptClaimTypes = claimTypes.Except(claimTypesToFilter);
        return exceptClaimTypes.Union(Options.RequestedClaims);
    }

    protected virtual IEnumerable<Claim> GetAdditionalOptionalClaims(ClaimsPrincipal subject)
    {
        foreach (var claimName in AdditionalOptionalClaimNames)
        {
            var claim = subject.FindFirst(claimName);
            if (claim != null)
            {
                yield return claim;
            }
        }
    }
}
