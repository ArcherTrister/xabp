// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

using X.Abp.LeptonTheme.Management;
using X.Abp.LeptonTheme.Management.Settings;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    public class LeptonSettingsProvider : ILeptonSettingsProvider, ITransientDependency
  {
    private readonly ISettingProvider _settingProvider;

    public LeptonSettingsProvider(ISettingProvider settingProvider)
    {
      _settingProvider = settingProvider;
    }

    public virtual async Task<bool> IsBoxedAsync()
    {
      return await _settingProvider.GetAsync<bool>(LeptonThemeSettingNames.Layout.Boxed);
    }

    public virtual async Task<MenuPlacement> GetMenuPlacementAsync()
    {
      return Enum.Parse<MenuPlacement>(await _settingProvider.GetOrNullAsync(LeptonThemeSettingNames.Layout.MenuPlacement));
    }

    public virtual async Task<MenuStatus> GetMenuStatusAsync()
    {
      return Enum.Parse<MenuStatus>(await _settingProvider.GetOrNullAsync(LeptonThemeSettingNames.Layout.MenuStatus));
    }

    public virtual async Task<LeptonStyle> GetStyleAsync()
    {
      return Enum.Parse<LeptonStyle>(await _settingProvider.GetOrNullAsync(LeptonThemeSettingNames.Style));
    }
  }
}
