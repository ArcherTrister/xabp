// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.Localization;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement.Web.Pages.TextTemplateManagement.Contents
{
    public class IndexModel : TextTemplateManagementPageModel
    {
        protected ILanguageProvider LanguageProvider { get; }

        protected ITemplateDefinitionAppService TemplateDefinitionAppService { get; }

        [BindProperty(SupportsGet = true)]
        public string TemplateDefinitionName { get; set; }

        public List<SelectListItem> BaseCultureNameSelectItems { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> TargetCultureNameSelectItems { get; set; } = new List<SelectListItem>();

        [SelectItems("BaseCultureNameSelectItems")]
        public string BaseCultureName { get; set; }

        [SelectItems("TargetCultureNameSelectItems")]
        public string TargetCultureName { get; set; }

        [TextArea(Rows = 10)]
        public string BaseContent { get; set; }

        [Required]
        [TextArea(Rows = 10)]
        [BindProperty]
        public string TargetContent { get; set; }

        public TemplateDefinitionDto TemplateDefinition { get; protected set; }

        public IndexModel(
          ILanguageProvider languageProvider,
          ITemplateDefinitionAppService templateDefinitionAppService)
        {
            LanguageProvider = languageProvider;
            TemplateDefinitionAppService = templateDefinitionAppService;
        }

        public async Task OnGetAsync()
        {
            TemplateDefinition = await TemplateDefinitionAppService.GetAsync(TemplateDefinitionName);
            await SetCultureNameSelectLists();
        }

        protected virtual async Task<IActionResult> SetCultureNameSelectLists()
        {
            IReadOnlyList<LanguageInfo> source = await LanguageProvider.GetLanguagesAsync();
            BaseCultureNameSelectItems = source.Select(l => new SelectListItem(l.DisplayName, l.CultureName, l.CultureName == CultureInfo.CurrentUICulture.Name)).ToList();
            bool flag = false;
            foreach (LanguageInfo languageInfo in (IEnumerable<LanguageInfo>)source)
            {
                TargetCultureNameSelectItems.Add(new SelectListItem(languageInfo.DisplayName, languageInfo.CultureName, !flag && languageInfo.CultureName != CultureInfo.CurrentUICulture.Name));
                flag = true;
            }

            return Page();
        }

        public virtual Task<IActionResult> OnPostAsync() => Task.FromResult<IActionResult>(Page());
    }
}
