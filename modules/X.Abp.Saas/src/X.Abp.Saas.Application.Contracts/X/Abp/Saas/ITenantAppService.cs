// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Saas.Dtos;

namespace X.Abp.Saas;

public interface ITenantAppService : ICrudAppService<SaasTenantDto, Guid, GetTenantsInput, SaasTenantCreateDto, SaasTenantUpdateDto>
{
    Task<SaasTenantDatabasesDto> GetDatabasesAsync();

    Task<SaasTenantConnectionStringsDto> GetConnectionStringsAsync(Guid id);

    Task UpdateConnectionStringsAsync(Guid id, SaasTenantConnectionStringsDto input);

    Task ApplyDatabaseMigrationsAsync(Guid id);

    Task<List<EditionLookupDto>> GetEditionLookupAsync();

    Task<bool> CheckConnectionStringAsync(string connectionString);

    Task SetPasswordAsync(Guid id, SaasTenantSetPasswordDto input);
}
