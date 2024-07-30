// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;

using Volo.Abp.Application.Services;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;
public interface IIdentityUserDelegationAppService : IApplicationService
{
    Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync();

    Task<ListResultDto<UserDelegationDto>> GetMyDelegatedUsersAsync();

    Task<ListResultDto<UserDelegationDto>> GetActiveDelegationsAsync();

    Task<ListResultDto<UserLookupDto>> GetUserLookupAsync(GetUserLookupInput input);

    Task DelegateNewUserAsync(DelegateNewUserInput input);

    Task DeleteDelegationAsync(Guid id);
}
