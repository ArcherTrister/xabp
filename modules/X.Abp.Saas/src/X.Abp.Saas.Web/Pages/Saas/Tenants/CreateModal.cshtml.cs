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
using Volo.Abp.Auditing;
using Volo.Abp.ObjectExtending;

using X.Abp.Saas;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class CreateModalModel : SaasPageModel
{
    [BindProperty]
    public TenantInfoModel Tenant { get; set; }

    public string DatabaseName { get; set; }

    public string ConnectionString { get; set; }

    public List<SelectListItem> DatabaseSelectListItems { get; set; }

    public List<SelectListItem> EditionsComboboxItems { get; set; } = new List<SelectListItem>();

    protected ITenantAppService TenantAppService { get; }

    protected IEditionAppService EditionAppService { get; }

    public CreateModalModel(
      ITenantAppService tenantAppService,
      IEditionAppService editionAppService)
    {
        TenantAppService = tenantAppService;
        EditionAppService = editionAppService;
    }

    public virtual async Task OnGetAsync()
    {
        Tenant = new TenantInfoModel()
        {
            UseSharedDatabase = true,
            ConnectionStrings = new TenantConnectionStringsModel()
        };
        DatabaseSelectListItems = new List<SelectListItem>();
        foreach (var database in (await TenantAppService.GetDatabasesAsync()).Databases)
        {
            DatabaseSelectListItems.Add(new SelectListItem(database, database));
        }

        var input = new GetEditionsInput
        {
            MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
        };
        var pagedResultDto = await EditionAppService.GetListAsync(input);
        EditionsComboboxItems.Add(new SelectListItem(string.Empty, string.Empty, true));
        EditionsComboboxItems.AddRange(pagedResultDto.Items.Select(e => new SelectListItem(e.DisplayName, e.Id.ToString())).ToList());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var input = ObjectMapper.Map<TenantInfoModel, SaasTenantCreateDto>(Tenant);
        if (Tenant.UseSharedDatabase)
        {
            input.ConnectionStrings = null;
        }

        await TenantAppService.CreateAsync(input);
        return NoContent();
    }

    public class TenantInfoModel : ExtensibleObject
    {
        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [SelectItems("EditionsComboboxItems")]
        public Guid? EditionId { get; set; }

        [StringLength(256)]
        [EmailAddress]
        [Required]
        public string AdminEmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [StringLength(128)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        public bool UseSharedDatabase { get; set; }

        public bool UseSpecificDatabase { get; set; }

        public TenantConnectionStringsModel ConnectionStrings { get; set; }

        public TenantActivationState ActivationState { get; set; }

        public DateTime? ActivationEndDate { get; set; }
    }

    public class TenantConnectionStringsModel : ExtensibleObject
    {
        [StringLength(1024)]
        [DisableAuditing]
        public string Default { get; set; }

        public List<TenantDatabaseConnectionStringsModel> Databases { get; set; }
    }

    public class TenantDatabaseConnectionStringsModel : ExtensibleObject
    {
        public string DatabaseName { get; set; }

        [StringLength(1024)]
        [DisableAuditing]
        public string ConnectionString { get; set; }
    }
}
