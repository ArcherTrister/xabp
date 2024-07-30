// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.EventBus.Local;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement.Texts
{
    public class EditModel : LanguageManagementPageModel
    {
        [BindProperty]
        public EditLanguageTextModalView LanguageTextToEdit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ResourceName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TargetCultureName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string BaseCultureName { get; set; }

        protected ILanguageTextAppService LanguageTextAppService { get; }

        protected ILocalEventBus LocalEventBus { get; }

        public EditModel(ILanguageTextAppService languageTextAppService, ILocalEventBus localEventBus)
        {
            LanguageTextAppService = languageTextAppService;
            LocalEventBus = localEventBus;
        }

        public virtual async Task OnGetAsync()
        {
            LanguageTextDto languageTextDto = await LanguageTextAppService.GetAsync(ResourceName, TargetCultureName, Name, BaseCultureName);
            LanguageTextToEdit = new EditLanguageTextModalView()
            {
                BaseCultureName = languageTextDto.BaseCultureName,
                BaseCultureValue = languageTextDto.BaseValue,
                Name = languageTextDto.Name,
                ResourceName = languageTextDto.ResourceName,
                TargetCultureName = languageTextDto.CultureName,
                TargetCultureValue = languageTextDto.Value
            };
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            ValidateModel();
            if (LanguageTextToEdit.RestoreToDefault)
            {
                await LanguageTextAppService.RestoreToDefaultAsync(LanguageTextToEdit.ResourceName, LanguageTextToEdit.TargetCultureName, LanguageTextToEdit.Name);
                return NoContent();
            }

            await LanguageTextAppService.UpdateAsync(LanguageTextToEdit.ResourceName, LanguageTextToEdit.TargetCultureName, LanguageTextToEdit.Name, LanguageTextToEdit.TargetCultureValue);
            await LocalEventBus.PublishAsync(new CurrentApplicationConfigurationCacheResetEventData());
            return NoContent();
        }

        public class EditLanguageTextModalView
        {
            [HiddenInput]
            public string Name { get; set; }

            [HiddenInput]
            public string ResourceName { get; set; }

            [HiddenInput]
            public string BaseCultureName { get; set; }

            [HiddenInput]
            public string TargetCultureName { get; set; }

            [TextArea(Rows = 4)]
            public string BaseCultureValue { get; set; }

            [TextArea(Rows = 4)]
            public string TargetCultureValue { get; set; }

            [HiddenInput]
            public bool RestoreToDefault { get; set; }
        }
    }
}
