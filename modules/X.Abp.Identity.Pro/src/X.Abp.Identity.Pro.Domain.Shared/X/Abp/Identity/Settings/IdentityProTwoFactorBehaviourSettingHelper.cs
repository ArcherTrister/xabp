// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Settings;

using X.Abp.Identity.Features;

namespace X.Abp.Identity.Settings;

public static class IdentityProTwoFactorBehaviourSettingHelper
{
    public static async Task<IdentityProTwoFactorBehaviour> Get([NotNull] ISettingProvider settingProvider)
    {
        Check.NotNull(settingProvider, nameof(settingProvider));

        var value = await settingProvider.GetOrNullAsync(IdentityProSettingNames.TwoFactor.Behaviour);
        return value.IsNullOrWhiteSpace() || !Enum.TryParse<IdentityProTwoFactorBehaviour>(value, out var behaviour)
            ? throw new AbpException($"{IdentityProSettingNames.TwoFactor.Behaviour} setting value is invalid")
            : behaviour;
    }
}
