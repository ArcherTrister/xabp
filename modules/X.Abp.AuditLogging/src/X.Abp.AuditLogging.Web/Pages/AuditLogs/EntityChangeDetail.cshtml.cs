// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging.Web.Pages.AuditLogging
{
    public class EntityChangeDetailModel : AuditLoggingPageModel
    {
        public EntityChangeWithUsernameDto EntityChangeWithUsername { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid EntityChangeId { get; set; }

        protected IAuditLogsAppService AuditLogsAppService { get; }

        public EntityChangeDetailModel(IAuditLogsAppService auditLogsAppService)
        {
            AuditLogsAppService = auditLogsAppService;
        }

        public virtual async Task OnGetAsync()
        {
            EntityChangeWithUsername = await AuditLogsAppService.GetEntityChangeWithUsernameAsync(EntityChangeId);
        }

        public virtual Task<IActionResult> OnPostAsync()
        {
            return Task.FromResult<IActionResult>(Page());
        }
    }
}
