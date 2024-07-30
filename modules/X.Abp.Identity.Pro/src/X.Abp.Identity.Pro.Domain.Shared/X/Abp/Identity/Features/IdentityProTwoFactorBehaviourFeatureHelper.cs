// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Features;

namespace X.Abp.Identity.Features;

public static class IdentityProTwoFactorBehaviourFeatureHelper
{
    public static async Task<IdentityProTwoFactorBehaviour> Get([NotNull] IFeatureChecker featureChecker)
    {
        Check.NotNull(featureChecker, nameof(featureChecker));

        var value = await featureChecker.GetOrNullAsync(IdentityProFeature.TwoFactor);
        return value.IsNullOrWhiteSpace() || !Enum.TryParse<IdentityProTwoFactorBehaviour>(value, out var behaviour)
            ? throw new AbpException($"{IdentityProFeature.TwoFactor} feature value is invalid")
            : behaviour;
    }
}
