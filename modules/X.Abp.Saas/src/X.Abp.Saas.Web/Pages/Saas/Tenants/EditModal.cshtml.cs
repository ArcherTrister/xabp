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

using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

using X.Abp.Saas;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class EditModalModel : SaasPageModel
{
    [BindProperty]
    public TenantInfoModel Tenant { get; set; }

    protected ITenantAppService TenantAppService { get; }

    protected IEditionAppService EditionAppService { get; }

    public List<SelectListItem> EditionsComboboxItems { get; set; } = new List<SelectListItem>();

    public bool HasSubscription { get; protected set; }

    public EditModalModel(ITenantAppService tenantAppService, IEditionAppService editionAppService)
    {
        TenantAppService = tenantAppService;
        EditionAppService = editionAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        var source = await TenantAppService.GetAsync(id);
        var editionEndDateUtc = source.EditionEndDateUtc;

        HasSubscription = editionEndDateUtc.HasValue && editionEndDateUtc.GetValueOrDefault() > DateTime.UtcNow;

        Tenant = ObjectMapper.Map<SaasTenantDto, TenantInfoModel>(source);

        var input = new GetEditionsInput
        {
            MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
        };
        var pagedResultDto = await EditionAppService.GetListAsync(input);
        EditionsComboboxItems.Add(new SelectListItem(string.Empty, string.Empty));

        EditionsComboboxItems.AddRange(pagedResultDto.Items.Select(s => new SelectListItem(s.PlanName, s.PlanId.ToString())).ToList());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var input = ObjectMapper.Map<TenantInfoModel, SaasTenantUpdateDto>(Tenant);
        await TenantAppService.UpdateAsync(Tenant.Id, input);
        return NoContent();
    }

    public class TenantInfoModel : ExtensibleObject, IHasConcurrencyStamp
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [SelectItems("EditionsComboboxItems")]
        public Guid? EditionId { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        public TenantActivationState ActivationState { get; set; }

        public DateTime? ActivationEndDate { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; }
    }
}
