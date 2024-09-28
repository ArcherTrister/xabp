// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.ObjectExtending;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement
{
    public class CreateModel : LanguageManagementPageModel
    {
#pragma warning disable SA1122 // Use string.Empty for empty strings
#pragma warning disable CA2227 // 集合属性应为只读
        public List<SelectListItem> CultureSelectList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> UiCultureSelectList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> FlagSelectList { get; set; } = new List<SelectListItem>();

        protected ILanguageAppService LanguageAppService { get; }

        [BindProperty]
        public LanguageCreateModalView Language { get; set; }

        public CreateModel(ILanguageAppService languageAppService) => LanguageAppService = languageAppService;

        public virtual Task OnGetAsync()
        {
            Language = new LanguageCreateModalView();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            List<RegionInfo> list = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(c => new RegionInfo(c.Name))
                .GroupBy(r => r.TwoLetterISORegionName).Select(g => g.First()).ToList();
            foreach (string flagCode in LanguageManagementFlagCodeConsts.FlagCodes)
            {
                RegionInfo regionInfo = list.FirstOrDefault(r => flagCode.Equals(r.TwoLetterISORegionName, StringComparison.OrdinalIgnoreCase));
                if (regionInfo != null && regionInfo.EnglishName != null)
                {
                    FlagSelectList.Add(new SelectListItem(regionInfo.EnglishName, flagCode));
                }
            }

            CultureSelectList = cultures.Select(c => new SelectListItem(c.EnglishName, c.Name)).ToList();
            if (CultureSelectList.Count != 0)
            {
                CultureSelectList.FirstOrDefault().Text = "";
                CultureSelectList.FirstOrDefault().Value = "";
            }

            UiCultureSelectList = CultureSelectList;
            return Task.CompletedTask;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            CreateLanguageDto input = ObjectMapper.Map<LanguageCreateModalView, CreateLanguageDto>(Language);
            LanguageDto languageDto = await LanguageAppService.CreateAsync(input);
            return NoContent();
        }

        public class LanguageCreateModalView : ExtensibleObject
        {
            [Required]
            [SelectItems("CultureSelectList")]
            public string CultureName { get; set; }

            [Required]
            [SelectItems("CultureSelectList")]
            public string UiCultureName { get; set; }

            [Required]
            public string DisplayName { get; set; }

            public string FlagIcon { get; set; }

            public bool IsEnabled { get; set; } = true;
        }
    }
}
