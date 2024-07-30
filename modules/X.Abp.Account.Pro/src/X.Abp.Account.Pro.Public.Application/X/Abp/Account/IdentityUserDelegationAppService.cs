// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

using X.Abp.Account.AuthorityDelegation;
using X.Abp.Account.Dtos;
using X.Abp.Account.Localization;

using IIdentityUserRepository = X.Abp.Identity.IIdentityUserRepository;

namespace X.Abp.Account;

[Authorize]
public class IdentityUserDelegationAppService : ApplicationService, IIdentityUserDelegationAppService
{
    protected IdentityUserDelegationManager IdentityUserDelegationManager { get; }

    protected IIdentityUserRepository IdentityUserRepository { get; }

    protected AbpAccountAuthorityDelegationOptions Options { get; }

    public IdentityUserDelegationAppService(
      IdentityUserDelegationManager identityUserDelegationManager,
      IIdentityUserRepository identityUserRepository,
      IOptions<AbpAccountAuthorityDelegationOptions> options)
    {
        IdentityUserDelegationManager = identityUserDelegationManager;
        IdentityUserRepository = identityUserRepository;
        Options = options.Value;
        LocalizationResource = typeof(AccountResource);
    }

    public virtual async Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync()
    {
        await CheckUserDelegationOperationAsync();
        var identityUserDelegationList = await IdentityUserDelegationManager.GetListAsync(CurrentUser.GetId());
        return await GetDelegationsAsync(identityUserDelegationList.Select(x => x.TargetUserId), identityUserDelegationList, false);
    }

    public virtual async Task<ListResultDto<UserDelegationDto>> GetMyDelegatedUsersAsync()
    {
        await CheckUserDelegationOperationAsync();

        var identityUserDelegationList = await IdentityUserDelegationManager.GetListAsync(null, CurrentUser.GetId());
        return await GetDelegationsAsync(identityUserDelegationList.Select(x => x.SourceUserId), identityUserDelegationList);
    }

    public virtual async Task<ListResultDto<UserDelegationDto>> GetActiveDelegationsAsync()
    {
        await CheckUserDelegationOperationAsync();
        var identityUserDelegationList = await IdentityUserDelegationManager.GetActiveDelegationsAsync(CurrentUser.GetId());
        return await GetDelegationsAsync(identityUserDelegationList.Select(x => x.SourceUserId), identityUserDelegationList);
    }

    public virtual async Task<ListResultDto<UserLookupDto>> GetUserLookupAsync(GetUserLookupInput input)
    {
        await CheckUserDelegationOperationAsync();
        if (input.Filter.IsNullOrWhiteSpace())
        {
            return new ListResultDto<UserLookupDto>();
        }

        var identityUserList = await IdentityUserRepository.GetListAsync(filter: input.Filter);

        return new ListResultDto<UserLookupDto>(ObjectMapper.Map<List<IdentityUser>, List<UserLookupDto>>(identityUserList).Where(u =>
        {
            return u.Id != CurrentUser.Id;
        }).ToList());
    }

    protected virtual async Task<ListResultDto<UserDelegationDto>> GetDelegationsAsync(
      IEnumerable<Guid> userIds,
      List<IdentityUserDelegation> delegations,
      bool isSourceUser = true)
    {
        await CheckUserDelegationOperationAsync();
        var source = await IdentityUserRepository.GetListByIdsAsync(userIds);

        var userDelegationDtoList = new List<UserDelegationDto>();
        foreach (var delegation in delegations)
        {
            var userDelegationDto = new UserDelegationDto
            {
                Id = delegation.Id,
                UserName = source.FirstOrDefault(x => !isSourceUser ? x.Id == delegation.TargetUserId : x.Id == delegation.SourceUserId)?.UserName,
                StartTime = delegation.StartTime,
                EndTime = delegation.EndTime
            };
            userDelegationDtoList.Add(userDelegationDto);
        }

        return new ListResultDto<UserDelegationDto>(userDelegationDtoList);
    }

    public virtual async Task DelegateNewUserAsync(DelegateNewUserInput input)
    {
        await CheckUserDelegationOperationAsync();
        var identityUser = await IdentityUserRepository.FindAsync(input.TargetUserId, true);
        if (identityUser == null)
        {
            throw new UserFriendlyException(L["Volo.Account:ThereIsNoUserWithId", input.TargetUserId]);
        }

        if (input.StartTime > input.EndTime)
        {
            throw new UserFriendlyException(L["Volo.Account:StartTimeMustBeLessThanEndTime"]);
        }

        await IdentityUserDelegationManager.DelegateNewUserAsync(CurrentUser.GetId(), identityUser.Id, input.StartTime, input.EndTime);
    }

    public virtual async Task DeleteDelegationAsync(Guid id)
    {
        await CheckUserDelegationOperationAsync();

        await IdentityUserDelegationManager.DeleteDelegationAsync(id, CurrentUser.GetId());
    }

    protected virtual Task CheckUserDelegationOperationAsync()
    {
        return !Options.EnableDelegatedImpersonation
            ? throw new UserFriendlyException(L["Volo.Account:DelegatedImpersonationIsDisabled"])
            : CurrentUser.FindImpersonatorUserId().HasValue
            ? throw new UserFriendlyException(L["Volo.Account:UserDelegationIsNotAvailableForImpersonatedUsers"])
            : Task.CompletedTask;
    }
}
