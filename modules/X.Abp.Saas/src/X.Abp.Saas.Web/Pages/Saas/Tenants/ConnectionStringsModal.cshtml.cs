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

using Volo.Abp.ObjectExtending;

using X.Abp.Saas;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class ConnectionStringsModalModel : SaasPageModel
{
    [BindProperty]
    public TenantConnectionStringsModel ConnectionStrings { get; set; }

    public string DatabaseName { get; set; }

    public string ConnectionString { get; set; }

    public List<SelectListItem> DatabaseSelectListItems { get; set; }

    protected ITenantAppService TenantAppService { get; }

    public ConnectionStringsModalModel(ITenantAppService tenantAppService)
    {
        TenantAppService = tenantAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        ConnectionStrings = ObjectMapper.Map<SaasTenantConnectionStringsDto, TenantConnectionStringsModel>(await TenantAppService.GetConnectionStringsAsync(id));
        ConnectionStrings.Id = id;
        ConnectionStrings.UseSharedDatabase = ConnectionStrings.Default.IsNullOrWhiteSpace() && (ConnectionStrings.Databases.IsNullOrEmpty() || ConnectionStrings.Databases.All(x => x.ConnectionString.IsNullOrWhiteSpace()));
        ConnectionStrings.UseSpecificDatabase = ConnectionStrings.Databases.Any(x => !x.ConnectionString.IsNullOrEmpty());
        DatabaseSelectListItems = ConnectionStrings.Databases.Where(x => x.ConnectionString.IsNullOrWhiteSpace()).Select(x => new SelectListItem(x.DatabaseName, x.DatabaseName)).ToList();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        if (ConnectionStrings.UseSharedDatabase)
        {
            await TenantAppService.UpdateConnectionStringsAsync(ConnectionStrings.Id, new SaasTenantConnectionStringsDto()
            {
                Databases = new List<SaasTenantDatabaseConnectionStringsDto>()
            });
        }
        else
        {
            var connectionStringsDto = ObjectMapper.Map<TenantConnectionStringsModel, SaasTenantConnectionStringsDto>(ConnectionStrings);
            await TenantAppService.UpdateConnectionStringsAsync(ConnectionStrings.Id, connectionStringsDto);
        }

        return NoContent();
    }

    public class TenantConnectionStringsModel : ExtensibleObject
    {
        [HiddenInput]
        public Guid Id { get; set; }

        public bool UseSharedDatabase { get; set; }

        public bool UseSpecificDatabase { get; set; }

        [StringLength(1024)]
        public string Default { get; set; }

        public List<TenantDatabaseConnectionStringsModel> Databases { get; set; }
    }

    public class TenantDatabaseConnectionStringsModel : ExtensibleObject
    {
        public string DatabaseName { get; set; }

        [StringLength(1024)]
        public string ConnectionString { get; set; }
    }
}
