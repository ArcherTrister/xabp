// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement.Texts
{
    public class IndexModel : LanguageManagementPageModel
    {
        public List<SelectListItem> BaseCultureNameSelectItems { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> TargetCultureNameSelectItems { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ResourceNameSelectItems { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> GetOnlyEmptyValuesSelectItems { get; set; } = new List<SelectListItem>();

        [SelectItems("BaseCultureNameSelectItems")]
        public string BaseCultureName { get; set; }

        [SelectItems("TargetCultureNameSelectItems")]
        public string TargetCultureName { get; set; }

        [SelectItems("ResourceNameSelectItems")]
        public string ResourceName { get; set; }

        [DisplayName("TargetValue")]
        [SelectItems("GetOnlyEmptyValuesSelectItems")]
        public string GetOnlyEmptyValues { get; set; }

        public string Filter { get; set; }

        protected ILanguageAppService LanguageAppService { get; }

        public IndexModel(ILanguageAppService languageAppService) => LanguageAppService = languageAppService;

        public virtual async Task OnGetAsync()
        {
            ListResultDto<LanguageDto> languages = await LanguageAppService.GetAllListAsync();
            List<LanguageResourceDto> source = await LanguageAppService.GetResourcesAsync();

            BaseCultureNameSelectItems = languages.Items.Select(p => new SelectListItem(p.DisplayName, p.CultureName, p.CultureName == BaseCultureName)).ToList();
            TargetCultureNameSelectItems = languages.Items.Select(p => new SelectListItem(p.DisplayName, p.CultureName, p.CultureName == TargetCultureName)).ToList();
            ResourceNameSelectItems = source.Select(l => new SelectListItem(l.Name, l.Name)).ToList();

            ResourceNameSelectItems.AddFirst(new SelectListItem("", ""));
            GetOnlyEmptyValuesSelectItems = new List<SelectListItem>()
              {
                new SelectListItem(L["All"].Value, "false"),
                new SelectListItem(L["OnlyEmptyValues"].Value, "true")
              };
            BaseCultureName = CalculateBaseCultureName(languages.Items);
            TargetCultureName = CalculateTargetCultureName(languages.Items, BaseCultureName);
        }

        public virtual Task<IActionResult> OnPostAsync() => Task.FromResult<IActionResult>(Page());

        protected virtual string CalculateBaseCultureName(IReadOnlyList<LanguageDto> languages)
        {
            string currentUiCulture = CultureInfo.CurrentUICulture.Name;
            LanguageDto language = languages.FirstOrDefault(l => l.UiCultureName == currentUiCulture);
            if (language != null)
            {
                return language.UiCultureName;
            }

            if (currentUiCulture.Contains("-"))
            {
                currentUiCulture = currentUiCulture[..currentUiCulture.IndexOf('-')];
            }

            language = languages.FirstOrDefault(l => l.UiCultureName == currentUiCulture);
            if (language != null)
            {
                return language.UiCultureName;
            }

            language = languages.FirstOrDefault(l => l.IsDefaultLanguage);
            return language != null ? language.UiCultureName : "en";
        }

        protected virtual string CalculateTargetCultureName(
          IReadOnlyList<LanguageDto> languages,
          string baseCultureName)
        {
            LanguageDto language = languages.FirstOrDefault(l => l.UiCultureName != baseCultureName && l.IsEnabled);
            return language != null ? language.UiCultureName : baseCultureName;
        }
    }
}
