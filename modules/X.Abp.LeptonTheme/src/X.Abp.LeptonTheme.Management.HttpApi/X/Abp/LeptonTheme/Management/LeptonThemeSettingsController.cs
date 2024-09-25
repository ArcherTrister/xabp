// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.LeptonTheme.Management
{
  [RemoteService(Name = LeptonThemeManagementRemoteServiceConsts.RemoteServiceName)]
  [Area(LeptonThemeManagementRemoteServiceConsts.ModuleName)]
  [Route("api/lepton-theme-management/settings")]
  public class LeptonThemeSettingsController : AbpControllerBase, ILeptonThemeSettingsAppService
  {
    private readonly ILeptonThemeSettingsAppService _leptonThemeSettingsAppService;

    public LeptonThemeSettingsController(ILeptonThemeSettingsAppService leptonThemeSettingsAppService)
    {
      _leptonThemeSettingsAppService = leptonThemeSettingsAppService;
    }

    [HttpGet]
    public virtual Task<LeptonThemeSettingsDto> GetAsync()
    {
      return _leptonThemeSettingsAppService.GetAsync();
    }

    [HttpPut]
    public virtual Task UpdateAsync(UpdateLeptonThemeSettingsDto input)
    {
      return _leptonThemeSettingsAppService.UpdateAsync(input);
    }
  }
}
