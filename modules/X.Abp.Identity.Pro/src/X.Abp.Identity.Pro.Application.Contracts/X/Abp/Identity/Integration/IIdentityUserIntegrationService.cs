// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;

namespace X.Abp.Identity.Integration;
public interface IIdentityUserIntegrationService
{
    Task<UserData> FindByIdAsync(Guid id);

    Task<UserData> FindByUserNameAsync(string userName);

    Task<long> GetCountAsync(UserLookupCountInputDto input);

    Task<string[]> GetRoleNamesAsync(Guid id);

    Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input);
}
