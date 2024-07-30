// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Net;

using Volo.Abp.Application.Dtos;

using Volo.Abp.Validation;

namespace X.Abp.AuditLogging.Dtos;

public class GetAuditLogListDto : PagedAndSortedResultRequestDto
{
    public DateTime StartTime { get; set; } = DateTime.MinValue;

    public DateTime EndTime { get; set; } = DateTime.MaxValue;

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.UrlFilterMaxLength))]
    public string Url { get; set; }

    public string ClientId { get; set; }

    public Guid? UserId { get; set; }

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.UserNameFilterMaxLength))]
    public string UserName { get; set; }

    public string ApplicationName { get; set; }

    public string ClientIpAddress { get; set; }

    public string CorrelationId { get; set; }

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.HttpMethodFilterMaxLength))]
    public string HttpMethod { get; set; }

    public HttpStatusCode? HttpStatusCode { get; set; }

    public int? MaxExecutionDuration { get; set; }

    public int? MinExecutionDuration { get; set; }

    public bool? HasException { get; set; }
}
