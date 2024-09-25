// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.AuditLogging;

[RemoteService(Name = AbpAuditLoggingRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAuditLoggingRemoteServiceConsts.ModuleName)]
[ControllerName("AuditLogSettings")]
[Route("api/audit-logging/settings")]
public class AuditLogSettingsController : AbpControllerBase, IAuditLogSettingsAppService
{
    protected IAuditLogSettingsAppService AuditLogSettingsAppService { get; }

    public AuditLogSettingsController(IAuditLogSettingsAppService auditLogSettingsAppService)
    {
        AuditLogSettingsAppService = auditLogSettingsAppService;
    }

    [HttpGet]
    public virtual Task<AuditLogSettingsDto> GetAsync()
    {
        return AuditLogSettingsAppService.GetAsync();
    }

    [HttpPut]
    public virtual Task UpdateAsync(AuditLogSettingsDto input)
    {
        return AuditLogSettingsAppService.UpdateAsync(input);
    }

    [HttpGet]
    [Route("global")]
    public virtual Task<AuditLogGlobalSettingsDto> GetGlobalAsync()
    {
        return AuditLogSettingsAppService.GetGlobalAsync();
    }

    [HttpPut]
    [Route("global")]
    public virtual Task UpdateGlobalAsync(AuditLogGlobalSettingsDto input)
    {
        return AuditLogSettingsAppService.UpdateGlobalAsync(input);
    }
}
