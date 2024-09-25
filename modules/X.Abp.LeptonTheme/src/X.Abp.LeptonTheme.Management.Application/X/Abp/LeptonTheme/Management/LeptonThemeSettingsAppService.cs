// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Services;
using Volo.Abp.Features;
using Volo.Abp.SettingManagement;

using X.Abp.LeptonTheme.Management.Features;
using X.Abp.LeptonTheme.Management.Permissions;
using X.Abp.LeptonTheme.Management.Settings;

namespace X.Abp.LeptonTheme.Management
{
  [RequiresFeature(LeptonThemeManagementFeatures.Enable)]
  [Authorize(LeptonThemeManagementPermissions.Settings.SettingsGroup)]
  public class LeptonThemeSettingsAppService : ApplicationService, ILeptonThemeSettingsAppService
  {
    protected ISettingManager SettingManager { get; }

    public LeptonThemeSettingsAppService(ISettingManager settingManager)
    {
      SettingManager = settingManager;
    }

    public virtual async Task<LeptonThemeSettingsDto> GetAsync()
    {
      Enum.TryParse(
          await SettingManager.GetOrNullForCurrentUserAsync(LeptonThemeSettingNames.Layout.MenuPlacement),
          out MenuPlacement menuPlacement
      );

      Enum.TryParse(
          await SettingManager.GetOrNullForCurrentUserAsync(LeptonThemeSettingNames.Layout.MenuStatus),
          out MenuStatus menuStatus
      );

      Enum.TryParse(
          await SettingManager.GetOrNullForCurrentUserAsync(LeptonThemeSettingNames.Style),
          out LeptonStyle style
      );

      Enum.TryParse(
          await SettingManager.GetOrNullForCurrentUserAsync(LeptonThemeSettingNames.PublicLayoutStyle),
          out LeptonStyle publicLayoutStyle
      );

      return new LeptonThemeSettingsDto
      {
        BoxedLayout = Convert.ToBoolean(await SettingManager.GetOrNullForCurrentUserAsync(LeptonThemeSettingNames.Layout.Boxed)),
        MenuPlacement = menuPlacement,
        MenuStatus = menuStatus,
        Style = style,
        PublicLayoutStyle = publicLayoutStyle
      };
    }

    public virtual async Task UpdateAsync(UpdateLeptonThemeSettingsDto input)
    {
      await SettingManager.SetForCurrentTenantAsync(LeptonThemeSettingNames.Layout.Boxed, input.BoxedLayout.ToString());
      await SettingManager.SetForCurrentTenantAsync(LeptonThemeSettingNames.Layout.MenuPlacement, input.MenuPlacement.ToString());
      await SettingManager.SetForCurrentTenantAsync(LeptonThemeSettingNames.Layout.MenuStatus, input.MenuStatus.ToString());
      await SettingManager.SetForCurrentTenantAsync(LeptonThemeSettingNames.Style, input.Style.ToString());
      await SettingManager.SetForCurrentTenantAsync(LeptonThemeSettingNames.PublicLayoutStyle, input.PublicLayoutStyle.ToString());
    }
  }
}
