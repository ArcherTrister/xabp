// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.Identity;

public interface IIdentitySettingsAppService : IApplicationService
{
    Task<IdentitySettingsDto> GetAsync();

    Task UpdateAsync(IdentitySettingsDto input);

    Task<IdentityLdapSettingsDto> GetLdapAsync();

    Task UpdateLdapAsync(IdentityLdapSettingsDto input);

    Task<IdentityOAuthSettingsDto> GetOAuthAsync();

    Task UpdateOAuthAsync(IdentityOAuthSettingsDto input);
}
