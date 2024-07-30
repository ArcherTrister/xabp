// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity;

[Authorize(AbpIdentityProPermissions.UserLookup.Default)]
public class IdentityUserLookupAppService : IdentityAppServiceBase, IIdentityUserLookupAppService
{
    protected IdentityUserRepositoryExternalUserLookupServiceProvider UserLookupServiceProvider { get; }

    public IdentityUserLookupAppService(
        IdentityUserRepositoryExternalUserLookupServiceProvider userLookupServiceProvider)
    {
        UserLookupServiceProvider = userLookupServiceProvider;
    }

    public virtual async Task<UserData> FindByIdAsync(Guid id)
    {
        var userData = await UserLookupServiceProvider.FindByIdAsync(id);
        return userData == null ? null : new UserData(userData);
    }

    public virtual async Task<UserData> FindByUserNameAsync(string userName)
    {
        var userData = await UserLookupServiceProvider.FindByUserNameAsync(userName);
        return userData == null ? null : new UserData(userData);
    }

    public async Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
    {
        var users = await UserLookupServiceProvider.SearchAsync(
            input.Sorting,
            input.Filter,
            input.MaxResultCount,
            input.SkipCount);

        return new ListResultDto<UserData>(
            users
                .Select(u => new UserData(u))
                .ToList());
    }

    public async Task<long> GetCountAsync(UserLookupCountInputDto input)
    {
        return await UserLookupServiceProvider.GetCountAsync(input.Filter);
    }
}
