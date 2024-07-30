// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement.Web.Pages.TextTemplateManagement.Contents
{
    public class InlineContentModel : TextTemplateManagementPageModel
    {
        private readonly ITemplateContentAppService itemplateContentAppService;
        private readonly ITemplateDefinitionAppService itemplateDefinitionAppService;

        [BindProperty(SupportsGet = true)]
        public string TemplateDefinitionName { get; set; }

        [TextArea(Rows = 10)]
        [BindProperty]
        [Required]
        public string TemplateContent { get; set; }

        public TemplateDefinitionDto TemplateDefinition { get; protected set; }

        public InlineContentModel(
          ITemplateContentAppService templateContentAppService,
          ITemplateDefinitionAppService templateDefinitionAppService)
        {
            itemplateContentAppService = templateContentAppService;
            itemplateDefinitionAppService = templateDefinitionAppService;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            TemplateDefinition = await itemplateDefinitionAppService.GetAsync(TemplateDefinitionName);

            TextTemplateContentDto templateContentDto = await itemplateContentAppService.GetAsync(new GetTemplateContentInput()
            {
                CultureName = null,
                TemplateName = TemplateDefinitionName
            });

            TemplateContent = templateContentDto.Content;

            return Page();
        }

        public virtual Task<IActionResult> OnPostAsync() => Task.FromResult<IActionResult>(Page());
    }
}
