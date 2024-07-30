// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AbpVnext.Pro.Customs;

public static class ResourceExtensions
{
    internal static async Task<string> GetCookieAuthenticationSchemeAsync(this HttpContext context)
    {
        var options = context.RequestServices.GetRequiredService<IdentityServerOptions>();
        if (options.Authentication.CookieAuthenticationScheme != null)
        {
            return options.Authentication.CookieAuthenticationScheme;
        }

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await schemes.GetDefaultAuthenticateSchemeAsync();
        return scheme == null
            ? throw new InvalidOperationException("No DefaultAuthenticateScheme found or no CookieAuthenticationScheme configured on IdentityServerOptions.")
            : scheme.Name;
    }

    public static Dictionary<string, object> ToClaimsDictionary(this IEnumerable<Claim> claims)
    {
        var d = new Dictionary<string, object>();

        if (claims == null)
        {
            return d;
        }

        var distinctClaims = claims.Distinct(new ClaimComparer());

        foreach (var claim in distinctClaims)
        {
            if (!d.TryGetValue(claim.Type, out var value))
            {
                d.Add(claim.Type, GetValue(claim));
            }
            else
            {
                // var value = value;
                if (value is List<object> list)
                {
                    list.Add(GetValue(claim));
                }
                else
                {
                    d.Remove(claim.Type);
                    d.Add(claim.Type, new List<object> { value, GetValue(claim) });
                }
            }
        }

        return d;
    }

    private static object GetValue(Claim claim)
    {
        if (claim.ValueType is ClaimValueTypes.Integer or
            ClaimValueTypes.Integer32)
        {
            if (int.TryParse(claim.Value, out var value))
            {
                return value;
            }
        }

        if (claim.ValueType == ClaimValueTypes.Integer64)
        {
            if (long.TryParse(claim.Value, out var value))
            {
                return value;
            }
        }

        if (claim.ValueType == ClaimValueTypes.Boolean)
        {
            if (bool.TryParse(claim.Value, out var value))
            {
                return value;
            }
        }

        if (claim.ValueType == IdentityServerConstants.ClaimValueTypes.Json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<JsonElement>(claim.Value);
            }
            catch
            {
            }
        }

        return claim.Value;
    }

    internal static ICollection<string> FindMatchingSigningAlgorithms(this IEnumerable<ApiResource> apiResources)
    {
        var apis = apiResources.ToList();

        if (apis.IsNullOrEmpty())
        {
            return new List<string>();
        }

        // only one API resource request, forward the allowed signing algorithms (if any)
        if (apis.Count == 1)
        {
            return apis.First().AllowedAccessTokenSigningAlgorithms;
        }

        var allAlgorithms = apis.Where(r => r.AllowedAccessTokenSigningAlgorithms.Any()).Select(r => r.AllowedAccessTokenSigningAlgorithms).ToList();

        // resources need to agree on allowed signing algorithms
        if (allAlgorithms.Any())
        {
            var allowedAlgorithms = IntersectLists(allAlgorithms);

            return allowedAlgorithms.Any()
                ? (ICollection<string>)allowedAlgorithms.ToHashSet()
                : throw new InvalidOperationException("Signing algorithms requirements for requested resources are not compatible.");
        }

        return new List<string>();
    }

    private static IEnumerable<T> IntersectLists<T>(IEnumerable<IEnumerable<T>> lists)
    {
        return lists.Aggregate((l1, l2) => l1.Intersect(l2));
    }
}
