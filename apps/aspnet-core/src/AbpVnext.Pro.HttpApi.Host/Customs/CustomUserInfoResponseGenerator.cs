// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;

using Microsoft.Extensions.Logging;

namespace AbpVnext.Pro.Customs;

/// <summary>
/// The userinfo response generator
/// </summary>
/// <seealso cref="IUserInfoResponseGenerator" />
public class CustomUserInfoResponseGenerator : IUserInfoResponseGenerator
{
    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The profile service
    /// </summary>
    protected IProfileService Profile { get; }

    /// <summary>
    /// The resource store
    /// </summary>
    protected IResourceStore Resources { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomUserInfoResponseGenerator"/> class.
    /// </summary>
    /// <param name="profile">The profile.</param>
    /// <param name="resourceStore">The resource store.</param>
    /// <param name="logger">The logger.</param>
    public CustomUserInfoResponseGenerator(IProfileService profile, IResourceStore resourceStore, ILogger<CustomUserInfoResponseGenerator> logger)
    {
        Profile = profile;
        Resources = resourceStore;
        Logger = logger;
    }

    /// <summary>
    /// Creates the response.
    /// </summary>
    /// <param name="validationResult">The userinfo request validation result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Profile service returned incorrect subject value</exception>
    public virtual async Task<Dictionary<string, object>> ProcessAsync(UserInfoRequestValidationResult validationResult)
    {
        Logger.LogDebug("Creating userinfo response");

        // extract scopes and turn into requested claim types
        var scopes = validationResult.TokenValidationResult.Claims.Where(c => c.Type == JwtClaimTypes.Scope).Select(c => c.Value);

        var validatedResources = await GetRequestedResourcesAsync(scopes);
        var requestedClaimTypes = await GetRequestedClaimTypesAsync(validatedResources);

        Logger.LogDebug("Requested claim types: {ClaimTypes}", requestedClaimTypes.ToSpaceSeparatedString());

        // call profile service
        var context = new ProfileDataRequestContext(
            validationResult.Subject,
            validationResult.TokenValidationResult.Client,
            IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint,
            requestedClaimTypes)
        {
            RequestedResources = validatedResources
        };

        await Profile.GetProfileDataAsync(context);
        var profileClaims = context.IssuedClaims;

        // construct outgoing claims
        var outgoingClaims = new List<Claim>();

        if (profileClaims == null)
        {
            Logger.LogInformation("Profile service returned no claims (null)");
        }
        else
        {
            outgoingClaims.AddRange(profileClaims);
            Logger.LogInformation("Profile service returned the following claim types: {Types}", profileClaims.Select(c => c.Type).ToSpaceSeparatedString());
        }

        var subClaim = outgoingClaims.SingleOrDefault(x => x.Type == JwtClaimTypes.Subject);
        if (subClaim == null)
        {
            outgoingClaims.Add(new Claim(JwtClaimTypes.Subject, validationResult.Subject.GetSubjectId()));
        }
        else if (subClaim.Value != validationResult.Subject.GetSubjectId())
        {
            Logger.LogError("Profile service returned incorrect subject value: {Sub}", subClaim);
            throw new InvalidOperationException("Profile service returned incorrect subject value");
        }

        return outgoingClaims.ToClaimsDictionary();
    }

    /// <summary>
    ///  Gets the identity resources from the scopes.
    /// </summary>
    /// <param name="scopes">范围</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected internal virtual async Task<ResourceValidationResult> GetRequestedResourcesAsync(IEnumerable<string> scopes)
    {
        if (scopes == null || !scopes.Any())
        {
            return null;
        }

        var scopeString = string.Join(" ", scopes);
        Logger.LogDebug("Scopes in access token: {Scopes}", scopeString);

        // if we ever parameterize identity scopes, then we would need to invoke the resource validator's parse API here
        var identityResources = await Resources.FindEnabledIdentityResourcesByScopeAsync(scopes);

        var resources = new Resources(identityResources, Enumerable.Empty<ApiResource>(), Enumerable.Empty<ApiScope>());
        var result = new ResourceValidationResult(resources);

        return result;
    }

    /// <summary>
    /// Gets the requested claim types.
    /// </summary>
    /// <param name="resourceValidationResult">验证结果</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected internal virtual Task<IEnumerable<string>> GetRequestedClaimTypesAsync(ResourceValidationResult resourceValidationResult)
    {
        IEnumerable<string> result = null;

        if (resourceValidationResult == null)
        {
            result = Enumerable.Empty<string>();
        }
        else
        {
            var identityResources = resourceValidationResult.Resources.IdentityResources;
            result = identityResources.SelectMany(x => x.UserClaims).Distinct();
        }

        return Task.FromResult(result);
    }
}
