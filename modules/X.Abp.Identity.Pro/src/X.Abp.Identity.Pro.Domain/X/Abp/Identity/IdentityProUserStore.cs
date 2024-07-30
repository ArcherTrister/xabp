// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Settings;

using X.Abp.Identity.Features;
using X.Abp.Identity.Settings;

namespace X.Abp.Identity;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IdentityProUserStore), typeof(IdentityUserStore))]
public class IdentityProUserStore : IdentityUserStore
{
    protected IFeatureChecker FeatureChecker { get; }

    protected ISettingProvider SettingProvider { get; }

    public IdentityProUserStore(
      IIdentityUserRepository userRepository,
      IIdentityRoleRepository roleRepository,
      IGuidGenerator guidGenerator,
      ILogger<IdentityRoleStore> logger,
      ILookupNormalizer lookupNormalizer,
      IFeatureChecker featureChecker,
      ISettingProvider settingProvider,
      IdentityErrorDescriber describer = null)
      : base(userRepository, roleRepository, guidGenerator, logger, lookupNormalizer, describer)
    {
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;
    }

    public override async Task<bool> GetTwoFactorEnabledAsync(
      IdentityUser user,
      CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        switch (await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker))
        {
            case IdentityProTwoFactorBehaviour.Disabled:
                return false;
            case IdentityProTwoFactorBehaviour.Forced:
                return true;
            default:
                switch (await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider))
                {
                    case IdentityProTwoFactorBehaviour.Disabled:
                        return false;
                    case IdentityProTwoFactorBehaviour.Forced:
                        return true;
                    default:
                        return user.TwoFactorEnabled;
                }
        }
    }
}
