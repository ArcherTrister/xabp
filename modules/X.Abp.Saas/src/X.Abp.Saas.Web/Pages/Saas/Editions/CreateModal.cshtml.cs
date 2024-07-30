// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.ObjectExtending;

using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Saas.Editions;

public class CreateModalModel : SaasPageModel
{
    [BindProperty]
    public EditionInfoModel Edition { get; set; }

    public List<SelectListItem> Plans { get; private set; }

    protected IEditionAppService EditionAppService { get; }

    public CreateModalModel(IEditionAppService editionAppService)
    {
        EditionAppService = editionAppService;
    }

    public virtual async Task OnGetAsync()
    {
        Edition = new EditionInfoModel();
        Plans = (await EditionAppService.GetPlanLookupAsync()).Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var input = ObjectMapper.Map<EditionInfoModel, EditionCreateDto>(Edition);
        await EditionAppService.CreateAsync(input);
        return NoContent();
    }

    public class EditionInfoModel : ExtensibleObject
    {
        [Required]
        [StringLength(128)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [SelectItems("Plans")]
        public Guid? PlanId { get; set; }
    }
}
