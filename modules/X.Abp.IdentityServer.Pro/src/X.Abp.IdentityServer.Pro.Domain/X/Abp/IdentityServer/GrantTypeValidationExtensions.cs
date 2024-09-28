// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using IdentityServer4.Models;

namespace X.Abp.IdentityServer;

public static class GrantTypeValidationExtensions
{
#pragma warning disable CA1851 // “IEnumerable”集合可能的多个枚举
    public static void Validate(this IEnumerable<string> grantTypes)
    {
        ArgumentNullException.ThrowIfNull(grantTypes);

        // spaces are not allowed in grant types
        foreach (var type in grantTypes)
        {
            if (type.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Grant types cannot contain spaces");
            }
        }

        // single grant type, seems to be fine
        if (grantTypes.Count() == 1)
        {
            return;
        }

        // don't allow duplicate grant types
        if (grantTypes.Count() != grantTypes.Distinct().Count())
        {
            throw new InvalidOperationException("Grant types list contains duplicate values");
        }

        // would allow response_type downgrade attack from code to token
        DisallowGrantTypeCombination(GrantType.Implicit, GrantType.AuthorizationCode, grantTypes);
        DisallowGrantTypeCombination(GrantType.Implicit, GrantType.Hybrid, grantTypes);

        DisallowGrantTypeCombination(GrantType.AuthorizationCode, GrantType.Hybrid, grantTypes);
    }

    public static void ValidateGrantTypes(IEnumerable<string> grantTypes)
    {
        ArgumentNullException.ThrowIfNull(grantTypes);

        // spaces are not allowed in grant types
        foreach (var type in grantTypes)
        {
            if (type.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Grant types cannot contain spaces");
            }
        }

        // single grant type, seems to be fine
        if (grantTypes.Count() == 1)
        {
            return;
        }

        // don't allow duplicate grant types
        if (grantTypes.Count() != grantTypes.Distinct().Count())
        {
            throw new InvalidOperationException("Grant types list contains duplicate values");
        }

        // would allow response_type downgrade attack from code to token
        DisallowGrantTypeCombination(GrantType.Implicit, GrantType.AuthorizationCode, grantTypes);
        DisallowGrantTypeCombination(GrantType.Implicit, GrantType.Hybrid, grantTypes);

        DisallowGrantTypeCombination(GrantType.AuthorizationCode, GrantType.Hybrid, grantTypes);
    }

    private static void DisallowGrantTypeCombination(string value1, string value2, IEnumerable<string> grantTypes)
    {
        if (grantTypes.Contains(value1, StringComparer.Ordinal) &&
            grantTypes.Contains(value2, StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"Grant types list cannot contain both {value1} and {value2}");
        }
    }
}
