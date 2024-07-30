// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Quartz.Histories.Dtos;

public class ExecutionHistoryDto
{
    public long EntryId { get; set; }

    public string JobName { get; set; }

    public string JobGroup { get; set; }

    public string TriggerName { get; set; }

    public string TriggerGroup { get; set; }

    public DateTimeOffset FiredTime { get; set; }

    public DateTimeOffset ScheduledTime { get; set; }

    public TimeSpan RunTime { get; set; }

    public bool Error { get; set; }

    public string ErrorMessage { get; set; }
}
