// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging.Web.Pages.Shared.Components.EntityChangeWidget
{
    public class EntityChangeWidgetViewComponent : AuditLogsComponentBase
    {
        public virtual IViewComponentResult Invoke(
          IEnumerable<EntityChangeWithUsernameDto> entityChanges)
        {
            return View("/Pages/Shared/Components/EntityChangeWidget/Default.cshtml", entityChanges.ToList());
        }
    }
}
