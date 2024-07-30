// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Json;

using X.Abp.AuditLogging.Dtos;
using X.Abp.AuditLogging.Features;
using X.Abp.AuditLogging.Permissions;

namespace X.Abp.AuditLogging;

[RequiresFeature(AbpAuditLoggingFeatures.Enable)]
[Authorize(AbpAuditLoggingPermissions.AuditLogs.Default)]
[DisableAuditing]
public class AuditLogsAppService : AuditLogsAppServiceBase, IAuditLogsAppService
{
    protected IAuditLogRepository AuditLogRepository { get; }

    protected IJsonSerializer JsonSerializer { get; }

    protected IPermissionChecker PermissionChecker { get; }

    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }

    public AuditLogsAppService(
        IAuditLogRepository auditLogRepository,
        IJsonSerializer jsonSerializer,
        IPermissionChecker permissionChecker,
        IPermissionDefinitionManager permissionDefinitionManager)
    {
        AuditLogRepository = auditLogRepository;
        JsonSerializer = jsonSerializer;
        PermissionChecker = permissionChecker;
        PermissionDefinitionManager = permissionDefinitionManager;
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogListDto input)
    {
        var auditLogs = await AuditLogRepository.GetListAsync(
             input.Sorting,
             input.MaxResultCount,
             input.SkipCount,
             input.StartTime,
             input.EndTime,
             input.HttpMethod,
             input.Url,
             input.ClientId,
             input.UserId,
             input.UserName,
             input.ApplicationName,
             input.ClientIpAddress,
             input.CorrelationId,
             input.MaxExecutionDuration,
             input.MinExecutionDuration,
             input.HasException,
             input.HttpStatusCode);

        var totalCount = await AuditLogRepository.GetCountAsync(
             input.StartTime,
             input.EndTime,
             input.HttpMethod,
             input.Url,
             input.ClientId,
             input.UserId,
             input.UserName,
             input.ApplicationName,
             input.ClientIpAddress,
             input.CorrelationId,
             input.MaxExecutionDuration,
             input.MinExecutionDuration,
             input.HasException,
             input.HttpStatusCode);

        var dtos = ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(auditLogs);

        return new PagedResultDto<AuditLogDto>(totalCount, dtos);
    }

    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        var log = await AuditLogRepository.GetAsync(id);
        var logDto = ObjectMapper.Map<AuditLog, AuditLogDto>(log);

        foreach (var action in logDto.Actions)
        {
            if (action.Parameters.IsNullOrEmpty())
            {
                action.Parameters = "{}";
            }

            var parsedJson = JsonSerializer.Deserialize<object>(action.Parameters);
            action.Parameters = JsonSerializer.Serialize(parsedJson, true, true);
        }

        return logDto;
    }

    public virtual async Task<GetErrorRateOutput> GetErrorRateAsync(GetErrorRateFilter filter)
    {
        var endTime = filter.EndDate.AddDays(1);

        var successfulLogCount = await AuditLogRepository.GetCountAsync(
            filter.StartDate,
            endTime,
            hasException: false);

        var errorLogCount = await AuditLogRepository.GetCountAsync(
            filter.StartDate,
            endTime,
            hasException: true);

        return new GetErrorRateOutput
        {
            Data = new Dictionary<string, long>
            {
                { L["Fault"], errorLogCount },
                { L["Success"], successfulLogCount }
            }
        };
    }

    public virtual async Task<GetAverageExecutionDurationPerDayOutput> GetAverageExecutionDurationPerDayAsync(GetAverageExecutionDurationPerDayInput filter)
    {
        var result =
            await AuditLogRepository.GetAverageExecutionDurationPerDayAsync(filter.StartDate, filter.EndDate);

        return new GetAverageExecutionDurationPerDayOutput
        {
            Data = result.ToDictionary(x => x.Key.ToString("d"), x => x.Value)
        };
    }

    public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesDto input)
    {
        var entityChanges = await AuditLogRepository.GetEntityChangeListAsync(
                                                    input.Sorting,
                                                    input.MaxResultCount,
                                                    input.SkipCount,
                                                    input.AuditLogId,
                                                    input.StartDate,
                                                    input.EndDate,
                                                    input.EntityChangeType,
                                                    input.EntityId,
                                                    input.EntityTypeFullName,
                                                    true);

        var entityChangesCount = await AuditLogRepository.GetEntityChangeCountAsync(input.AuditLogId,
                                                                                    input.StartDate,
                                                                                    input.EndDate,
                                                                                    input.EntityChangeType,
                                                                                    input.EntityId,
                                                                                    input.EntityTypeFullName);

        var dtos = ObjectMapper.Map<List<EntityChange>, List<EntityChangeDto>>(entityChanges);

        return new PagedResultDto<EntityChangeDto>(entityChangesCount, dtos);
    }

    [AllowAnonymous]
    public virtual async Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(EntityChangeFilter input)
    {
        await CheckPermissionForEntityAsync(input.EntityTypeFullName);
        var entityChanges = await AuditLogRepository.GetEntityChangesWithUsernameAsync(input.EntityId, input.EntityTypeFullName);

        return ObjectMapper.Map<List<EntityChangeWithUsername>, List<EntityChangeWithUsernameDto>>(entityChanges);
    }

    public virtual async Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId)
    {
        var entityChange = await AuditLogRepository.GetEntityChangeWithUsernameAsync(entityChangeId);
        await CheckPermissionForEntityAsync(entityChange.EntityChange.EntityTypeFullName);

        return ObjectMapper.Map<EntityChangeWithUsername, EntityChangeWithUsernameDto>(entityChange);
    }

    public virtual async Task<EntityChangeDto> GetEntityChangeAsync(Guid entityChangeId)
    {
        var entityChange = await AuditLogRepository.GetEntityChange(entityChangeId);
        await CheckPermissionForEntityAsync(entityChange.EntityTypeFullName);

        var entityChangeDto = ObjectMapper.Map<EntityChange, EntityChangeDto>(entityChange);
        return entityChangeDto;
    }

    protected virtual async Task CheckPermissionForEntityAsync(string entityFullName)
    {
        var permissionName = $"AuditLogging.ViewChangeHistory:{entityFullName}";

        var permission = PermissionDefinitionManager.GetOrNullAsync(permissionName);

        if (permission == null)
        {
            await AuthorizationService.CheckAsync(AbpAuditLoggingPermissions.AuditLogs.Default);
        }
        else
        {
            if (!await PermissionChecker.IsGrantedAsync(permissionName))
            {
                await AuthorizationService.CheckAsync(AbpAuditLoggingPermissions.AuditLogs.Default);
            }
        }
    }
}
