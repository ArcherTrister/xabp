// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging.Web.Pages.AuditLogging
{
    public class DetailModel : AuditLoggingPageModel
    {
        public AuditLogDto AuditLog { get; private set; }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        protected IAuditLogsAppService AuditLogsAppService { get; }

        public DetailModel(IAuditLogsAppService auditLogsAppService)
        {
            AuditLogsAppService = auditLogsAppService;
        }

        public virtual async Task OnGetAsync()
        {
            AuditLog = await AuditLogsAppService.GetAsync(Id);
        }

        public virtual Task<IActionResult> OnPostAsync()
        {
            return Task.FromResult<IActionResult>(Page());
        }

        public virtual string SerializeDictionary(Dictionary<string, object> dictionary)
        {
            return JsonSerializer.Serialize(dictionary, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
    }
}
