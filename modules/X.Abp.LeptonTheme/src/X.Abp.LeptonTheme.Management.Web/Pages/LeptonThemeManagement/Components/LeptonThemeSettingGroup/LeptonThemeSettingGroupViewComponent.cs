// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.LeptonTheme.Management.Web.Areas.LeptonThemeManagement.Models;

namespace X.Abp.LeptonTheme.Management.Web.Pages.LeptonThemeManagement.Components.LeptonThemeSettingGroup
{
    public class LeptonThemeSettingGroupViewComponent : AbpViewComponent
    {
        protected ILeptonThemeSettingsAppService LeptonThemeSettingsAppService { get; }

        public LeptonThemeSettingGroupViewComponent(ILeptonThemeSettingsAppService leptonThemeSettingsAppService)
        {
            ObjectMapperContext = typeof(AbpLeptonThemeManagementWebModule);

            LeptonThemeSettingsAppService = leptonThemeSettingsAppService;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var model = ObjectMapper.Map<LeptonThemeSettingsDto, LeptonThemeSettingsViewModel>(await LeptonThemeSettingsAppService.GetAsync());

            return View("~/Pages/LeptonThemeManagement/Components/LeptonThemeSettingGroup/Default.cshtml", model);
        }
    }
}
