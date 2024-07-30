// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Quartz.Histories.Dtos;

[Serializable]
public sealed class ExecutionHistoryEntry
{
    public long EntryId { get; set; }

    public string JobGroup { get; set; }

    public string TriggerGroup { get; set; }

    public DateTimeOffset FiredTime { get; set; }

    public DateTimeOffset ScheduledTime { get; set; }

    public TimeSpan RunTime { get; set; }

    public bool Error { get; set; }

    public string FireInstanceId { get; set; }

    public string SchedulerInstanceId { get; set; }

    public string SchedulerName { get; set; }

    public string JobName { get; set; }

    public string TriggerName { get; set; }

    public DateTime? ScheduledFireTimeUtc { get; set; }

    public DateTime ActualFireTimeUtc { get; set; }

    /// <summary>
    /// 是否恢复中
    /// </summary>
    public bool Recovering { get; set; }

    /// <summary>
    /// 执行是否被否决
    /// </summary>
    public bool Vetoed { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? FinishedTimeUtc { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }
}
