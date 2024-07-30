// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Quartz.Permissions;

public class AbpQuartzPermissions
{
    public const string GroupName = "Quartz";

    public static class Calendars
    {
        public const string Default = GroupName + ".Calendar";

        public const string Create = Default + ".Create";

        // public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ExecutionHistory
    {
        public const string Default = GroupName + ".ExecutionHistory";

        // public const string Create = Default + ".Create";
        // public const string Update = Default + ".Update";
        // public const string Delete = Default + ".Delete";
    }

    public static class Jobs
    {
        public const string Default = GroupName + ".Job";

        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Schedulers
    {
        public const string Default = GroupName + ".Scheduler";

        // public const string Create = Default + ".Create";
        // public const string Update = Default + ".Update";
        // public const string Delete = Default + ".Delete";
    }

    public static class Triggers
    {
        public const string Default = GroupName + ".Trigger";

        // public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";

        // public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpQuartzPermissions));
    }
}
