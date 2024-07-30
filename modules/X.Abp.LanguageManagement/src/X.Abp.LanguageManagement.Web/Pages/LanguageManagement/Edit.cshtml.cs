using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement
{
    public class EditModel : LanguageManagementPageModel
    {
        protected
#nullable disable
        ILanguageAppService LanguageAppService
        { get; }

        [BindProperty]
        public LanguageEditModalView Language { get; set; }

        public List<SelectListItem> FlagSelectList { get; set; } = new List<SelectListItem>();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public EditModel(ILanguageAppService languageAppService) => LanguageAppService = languageAppService;

        public virtual async Task OnGetAsync()
        {
            List<RegionInfo> list = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(c => new RegionInfo(c.Name)).GroupBy(r => r.TwoLetterISORegionName).Select(g => g.First()).ToList();
            foreach (string flagCode1 in LanguageManagementFlagCodeConsts.FlagCodes)
            {
                string flagCode = flagCode1;
                RegionInfo regionInfo = list.FirstOrDefault(r => flagCode.Equals(r.TwoLetterISORegionName, StringComparison.InvariantCultureIgnoreCase));
                if (regionInfo != null && regionInfo.EnglishName != null)
                {
                    FlagSelectList.Add(new SelectListItem(regionInfo.EnglishName, flagCode));
                }
            }

            LanguageDto source = await LanguageAppService.GetAsync(Id);
            Language = ObjectMapper.Map<LanguageDto, LanguageEditModalView>(source);
        }

        public virtual async Task<NoContentResult> OnPostAsync()
        {
            UpdateLanguageDto input = ObjectMapper.Map<LanguageEditModalView, UpdateLanguageDto>(Language);
            LanguageDto languageDto = await LanguageAppService.UpdateAsync(Language.Id, input);
            return NoContent();
        }

        public class LanguageEditModalView : ExtensibleObject, IHasConcurrencyStamp
        {
            [HiddenInput]
            public Guid Id { get; set; }

            public string DisplayName { get; set; }

            public string FlagIcon { get; set; }

            public bool IsEnabled { get; set; }

            [HiddenInput]
            public string ConcurrencyStamp { get; set; }
        }
    }
}
