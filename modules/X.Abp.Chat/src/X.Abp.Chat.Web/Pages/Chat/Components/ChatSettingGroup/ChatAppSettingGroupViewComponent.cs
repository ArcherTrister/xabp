// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Chat.Settings;

namespace X.Chat.Web.Pages.Chat.Components.ChatSettingGroup
{
    public class ChatAppSettingGroupViewComponent : AbpViewComponent
    {
        public ChatSettingsViewModel SettingsViewModel { get; set; }

        protected ISettingsAppService SettingsAppService { get; }

        public ChatAppSettingGroupViewComponent(ISettingsAppService settingsAppService)
        {
            SettingsAppService = settingsAppService;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            SettingsViewModel = new ChatSettingsViewModel
            {
                ChatSettings = await SettingsAppService.GetAsync()
            };

            return View("/Pages/Chat/Components/ChatSettingGroup/Default.cshtml", SettingsViewModel);
        }

        public class ChatSettingsViewModel
        {
            public ChatSettingsDto ChatSettings { get; set; }
        }
    }
}
