// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Features;

using Volo.Abp.Settings;

using X.Abp.Identity.Features;
using X.Abp.Identity.Settings;

namespace X.Abp.Identity;

public class IdentityProTwoFactorManager : IDomainService, ITransientDependency
{
    protected IFeatureChecker FeatureChecker { get; }

    protected ISettingProvider SettingProvider { get; }

    public IdentityProTwoFactorManager(
      IFeatureChecker featureChecker,
      ISettingProvider settingProvider)
    {
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;
    }

    public virtual async Task<bool> IsOptionalAsync()
    {
        if (await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker) == IdentityProTwoFactorBehaviour.Optional)
        {
            if (await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider) == IdentityProTwoFactorBehaviour.Optional)
            {
                return true;
            }
        }

        return false;
    }

    public virtual async Task<bool> IsForcedEnableAsync()
    {
        return await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker) == IdentityProTwoFactorBehaviour.Forced || await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider) == IdentityProTwoFactorBehaviour.Forced;
    }

    public virtual async Task<bool> IsForcedDisableAsync()
    {
        return await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker) == IdentityProTwoFactorBehaviour.Disabled || await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider) == IdentityProTwoFactorBehaviour.Disabled;
    }
}
