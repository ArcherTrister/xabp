// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Util;

namespace X.Abp.Quartz.Dtos;

public class GroupMatcherDto
{
    public string NameContains { get; set; }

    public string NameEndsWith { get; set; }

    public string NameStartsWith { get; set; }

    public string NameEquals { get; set; }

    public GroupMatcher<TriggerKey> GetTriggerGroupMatcher()
    {
        return GetGroupMatcher<TriggerKey>();
    }

    public GroupMatcher<JobKey> GetJobGroupMatcher()
    {
        return GetGroupMatcher<JobKey>();
    }

    private GroupMatcher<T> GetGroupMatcher<T>()
        where T : Key<T>
    {
        return !string.IsNullOrWhiteSpace(NameContains)
            ? GroupMatcher<T>.GroupContains(NameContains)
            : !string.IsNullOrWhiteSpace(NameStartsWith)
            ? GroupMatcher<T>.GroupStartsWith(NameStartsWith)
            : !string.IsNullOrWhiteSpace(NameEndsWith)
            ? GroupMatcher<T>.GroupEndsWith(NameEndsWith)
            : !string.IsNullOrWhiteSpace(NameEquals) ? GroupMatcher<T>.GroupEquals(NameEquals) : GroupMatcher<T>.AnyGroup();
    }
}
