// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AuditLogging.Localization;

namespace X.Abp.AuditLogging.Web.Pages.AuditLogging;

public abstract class AuditLoggingPageModel : AbpPageModel
{
    protected AuditLoggingPageModel()
    {
        LocalizationResourceType = typeof(AuditLoggingResource);
        ObjectMapperContext = typeof(AbpAuditLoggingWebModule);
    }
}
