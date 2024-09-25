// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Settings;

namespace X.Abp.Identity.Settings;

public static class IdentityProPreventConcurrentLoginBehaviourSettingHelper
{
    public static async Task<IdentityProPreventConcurrentLoginBehaviour> Get(ISettingProvider settingProvider)
    {
        Check.NotNull(settingProvider, nameof(settingProvider));
        string preventConcurrentLogin = await settingProvider.GetOrNullAsync(IdentityProSettingNames.Session.PreventConcurrentLogin);
        IdentityProPreventConcurrentLoginBehaviour result;
        if (preventConcurrentLogin.IsNullOrWhiteSpace() || !Enum.TryParse(preventConcurrentLogin, out result))
        {
            throw new AbpException("Abp.Identity.Session.PreventConcurrentLogin setting value is invalid");
        }

        return result;
    }
}
