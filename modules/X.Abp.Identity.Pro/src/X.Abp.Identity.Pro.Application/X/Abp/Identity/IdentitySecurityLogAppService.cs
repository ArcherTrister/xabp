// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity;

public class IdentitySecurityLogAppService : IdentityAppServiceBase, IIdentitySecurityLogAppService
{
    protected IIdentitySecurityLogRepository IdentitySecurityLogRepository { get; }

    public IdentitySecurityLogAppService(IIdentitySecurityLogRepository identitySecurityLogRepository)
    {
        IdentitySecurityLogRepository = identitySecurityLogRepository;
    }

    [Authorize(AbpIdentityProPermissions.SecurityLogs.Default)]
    public async Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetIdentitySecurityLogListInput input)
    {
        var securityLogs = await IdentitySecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.ActionName,
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId);

        var totalCount = await IdentitySecurityLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.ActionName,
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId);

        var securityLogDtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(securityLogs);
        return new PagedResultDto<IdentitySecurityLogDto>(totalCount, securityLogDtos);
    }

    [Authorize(AbpIdentityProPermissions.SecurityLogs.Default)]
    public async Task<IdentitySecurityLogDto> GetAsync(Guid id)
    {
        var securityLog = await IdentitySecurityLogRepository.GetAsync(id);
        return ObjectMapper.Map<IdentitySecurityLog, IdentitySecurityLogDto>(securityLog);
    }

    [Authorize]
    public async Task<PagedResultDto<IdentitySecurityLogDto>> GetMyListAsync(GetIdentitySecurityLogListInput input)
    {
        var securityLogs = await IdentitySecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.ActionName,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId);

        var totalCount = await IdentitySecurityLogRepository.GetCountAsync(
            input.StartTime,
            input.EndTime,
            input.ApplicationName,
            input.Identity,
            input.ActionName,
            CurrentUser.GetId(),
            input.UserName,
            input.ClientId,
            input.CorrelationId);

        var securityLogDtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(securityLogs);
        return new PagedResultDto<IdentitySecurityLogDto>(totalCount, securityLogDtos);
    }

    [Authorize]
    public async Task<IdentitySecurityLogDto> GetMyAsync(Guid id)
    {
        var securityLog = await IdentitySecurityLogRepository.GetByUserIdAsync(id, CurrentUser.GetId());
        return ObjectMapper.Map<IdentitySecurityLog, IdentitySecurityLogDto>(securityLog);
    }
}
