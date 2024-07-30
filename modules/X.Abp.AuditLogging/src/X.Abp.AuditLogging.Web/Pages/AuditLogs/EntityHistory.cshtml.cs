// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging.Web.Pages.AuditLogging
{
    public class EntityHistoryModel : AuditLoggingPageModel
    {
        public List<EntityChangeWithUsernameDto> EntityChanges { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EntityId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EntityTypeFullName { get; set; }

        protected IAuditLogsAppService AuditLogsAppService { get; }

        public EntityHistoryModel(IAuditLogsAppService auditLogsAppService)
        {
            AuditLogsAppService = auditLogsAppService;
        }

        public virtual async Task OnGetAsync()
        {
            EntityChanges = await AuditLogsAppService.GetEntityChangesWithUsernameAsync(new EntityChangeFilter()
            {
                EntityId = EntityId,
                EntityTypeFullName = EntityTypeFullName
            });
        }

        public virtual Task<IActionResult> OnPostAsync()
        {
            return Task.FromResult<IActionResult>(Page());
        }
    }
}
