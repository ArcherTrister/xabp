// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Application.Services;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;

public interface IProfileAppService : IApplicationService
{
    Task<ProfileDto> GetAsync();

    Task<ProfileDto> UpdateAsync(UpdateProfileDto input);

    Task ChangePasswordAsync(ChangePasswordInput input);

    Task<bool> GetTwoFactorEnabledAsync();

    Task SetTwoFactorEnabledAsync(bool enabled);

    Task<bool> CanEnableTwoFactorAsync();

    Task<List<NameValue>> GetTimezonesAsync();
}
