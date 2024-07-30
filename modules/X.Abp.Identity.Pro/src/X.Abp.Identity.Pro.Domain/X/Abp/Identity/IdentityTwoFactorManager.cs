// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Domain.Services;
using Volo.Abp.Features;
using Volo.Abp.Settings;

using X.Abp.Identity.Features;
using X.Abp.Identity.Settings;

namespace X.Abp.Identity;

public class IdentityTwoFactorManager : IDomainService
{
    protected IFeatureChecker FeatureChecker { get; }

    protected ISettingProvider SettingProvider { get; }

    public IdentityTwoFactorManager(IFeatureChecker featureChecker, ISettingProvider settingProvider)
    {
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;
    }

    public virtual async Task<bool> IsOptionalAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Optional)
        {
            var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
            if (setting == IdentityProTwoFactorBehaviour.Optional)
            {
                return true;
            }
        }

        return false;
    }

    public virtual async Task<bool> IsForcedEnableAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Forced)
        {
            return true;
        }

        var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
        return setting == IdentityProTwoFactorBehaviour.Forced;
    }

    public virtual async Task<bool> IsForcedDisableAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Disabled)
        {
            return true;
        }

        var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
        return setting == IdentityProTwoFactorBehaviour.Disabled;
    }
}
