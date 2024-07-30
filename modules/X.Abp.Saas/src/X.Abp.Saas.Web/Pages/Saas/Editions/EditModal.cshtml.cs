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
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

using X.Abp.Saas;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Saas.Editions;

public class EditModalModel : SaasPageModel
{
    [BindProperty]
    public EditionInfoModel Edition { get; set; }

    public List<SelectListItem> Plans { get; private set; }

    protected IEditionAppService EditionAppService { get; }

    public EditModalModel(IEditionAppService editionAppService)
    {
        EditionAppService = editionAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        Plans = (await EditionAppService.GetPlanLookupAsync()).Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
        Edition = ObjectMapper.Map<EditionDto, EditionInfoModel>(await EditionAppService.GetAsync(id));
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var input = ObjectMapper.Map<EditionInfoModel, EditionUpdateDto>(Edition);
        await EditionAppService.UpdateAsync(Edition.Id, input);
        return NoContent();
    }

    public class EditionInfoModel : ExtensibleObject, IHasConcurrencyStamp
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [StringLength(128)]
        [Required]
        public string DisplayName { get; set; }

        [SelectItems("Plans")]
        public Guid? PlanId { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; }
    }
}
