// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.AuditLogging
{
    public interface IAuditLogSettingsAppService : IApplicationService
    {
        Task<AuditLogSettingsDto> GetAsync();

        Task UpdateAsync(AuditLogSettingsDto input);

        Task<AuditLogGlobalSettingsDto> GetGlobalAsync();

        Task UpdateGlobalAsync(AuditLogGlobalSettingsDto input);
    }
}
